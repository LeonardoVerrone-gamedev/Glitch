using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float currentSpeed = 6f; // Velocidade de movimento
    [SerializeField] float defeaultSpeed = 6f;
    [SerializeField] float speedMultiplier;

    [SerializeField] float currentJumpForce = 10f; // Força do pulo
    [SerializeField] float defeaultJumpForce = 15f;
    [SerializeField] float jumpForceMultiplier;

    [SerializeField] private LayerMask groundLayer; // Camada do chão
    [SerializeField] private Transform groundCheck; // Ponto de verificação do chão
    [SerializeField] public float groundCheckRadius = 0.2f; // Raio da verificação do chão

    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip LandClip;
    [SerializeField] AudioClip WalkClip;

    [SerializeField] AudioSource WalkAudioSource;
    [SerializeField] AudioSource JumpAudioSource;

    [SerializeField] private ParticleSystem Dust;

    private float pushbackForce = 5f; // Força do pushback

    public float horizontalValue;
    
    public float verticalValue;

    private Rigidbody2D rb;
    [SerializeField] private bool isGrounded;
    private bool wasGrounded = false; // Variável para armazenar o estado anterior

    private bool isFacingRight;

    public bool infinityJump;

    [Header("Camera Settings")]
    private Coroutine resetTriggerCoroutine;
    private CameraFollowObject cameraFollowObject;
    public CinemachineImpulseSource impulseSource; // Referência ao Impulse Source

    private PlayerAnimation playerAnimation;
    private PlayerInputs inputs;
    [SerializeField] private ScratchAndStretch scratchAndStretch;

    // Coyote Time and Jump Buffering
    [SerializeField] private float coyoteTime = 0.2f; // Tempo de coyote
    [SerializeField] private float jumpBufferTime = 0.2f; // Tempo de buffer de pulo

    [SerializeField] private float FlyJumpCoolDown = 0.3f;

    private float FlyJumpTime;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cameraFollowObject = GameObject.FindObjectOfType<CameraFollowObject>();
        playerAnimation = GetComponent<PlayerAnimation>();
        inputs = GetComponent<PlayerInputs>();
    }

    void Update()
    {
        GroundCheck(); // Checagem de chão
        FlipCheck(); // Verifica se o jogador precisa flipar

        // Handle jump buffering
        if (inputs.isJumping)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Handle coyote time
        if (!isGrounded)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        else
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote time when grounded
        }

        // Attempt to jump if jump is buffered and coyote time is valid
        if (jumpBufferCounter > 0)
        {
            Jump();
            jumpBufferCounter = 0; // Reset jump buffer after jumping
        }

        if (FlyJumpTime > 0)
        {
            FlyJumpTime -= Time.unscaledDeltaTime;
        }
    }

    public void SetSpeedMultiplier(float value)
    {
        if (value == 0f)
        {
            speedMultiplier = 1f;

            currentSpeed = defeaultSpeed * speedMultiplier; //6

            currentJumpForce = currentSpeed * 2.5f; //15

           // Debug.Log("Velocidade resetada");

            return; //reseta velocidade
        }

        speedMultiplier = RoundTo((speedMultiplier += value), 1);
        //jumpForceMultiplier = RoundTo((jumpForceMultiplier += value), 1);

        if (speedMultiplier <= 0f)
        {
            speedMultiplier = .1f;
        }

        currentSpeed = defeaultSpeed * speedMultiplier;

        currentJumpForce = currentSpeed * 2.5f;
    }

    float RoundTo(float value, int decimalPlaces = 1)
    {
        float multiplier = Mathf.Pow(10f, decimalPlaces);
        return (Mathf.Round(value * multiplier) / multiplier);
    }

    public void Move(float moveInput)
    {
        if (!inputs.canMove)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        horizontalValue = moveInput;

        if(isGrounded && horizontalValue != 0f && !WalkAudioSource.isPlaying){
            WalkAudioSource.Play();
        }else{
            WalkAudioSource.Stop();
        }
    }
    

    public void Jump()
    {
        if (isGrounded || (infinityJump && FlyJumpTime <= 0) || coyoteTimeCounter > 0)
        {
            jumpBufferCounter = jumpBufferTime;

            if (infinityJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                playerAnimation.PlayAsasAnimation();
                FlyJumpTime = FlyJumpCoolDown;
            }

            if (isGrounded)
            {
                scratchAndStretch.PlayStretchAnimation("Jump");
            }
            
            CreateDust();
            JumpAudioSource.clip = jumpClip;
            JumpAudioSource.Play();

            //AQUI TOCAR SOM DE PULO

           // currentJumpForce = defeaultJumpForce * jumpForceMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce);
        }
    }

    public void Pushback(float direction)
    {
        // Aplica um pushback na direção oposta ao movimento
        Vector2 pushDirection = new Vector2(-direction, 0).normalized;
        rb.AddForce(pushDirection * pushbackForce, ForceMode2D.Impulse);
    }

    void GroundCheck()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
        {
            isGrounded = true;

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("MovingPlatform"))
                {
                    transform.parent = collider.transform;
                }
                else
                {
                    transform.parent = null;
                }
            }
        }

        // Verifica se o jogador aterrissou
        if (isGrounded && !wasGrounded)
        {
            JumpAudioSource.clip = LandClip;
            JumpAudioSource.Play();
            scratchAndStretch.PlayStretchAnimation("Landing");
            CreateDust();
            impulseSource.GenerateImpulse();
        }

        // Atualiza o estado anterior
        wasGrounded = isGrounded;
        coyoteTimeCounter = isGrounded ? coyoteTime : coyoteTimeCounter; // Reset coyote time when grounded
    }

    void FlipCheck()
    {
        if (horizontalValue > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalValue < 0 && isFacingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {
        CreateDust();
        isFacingRight = !isFacingRight;
        transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
        cameraFollowObject.CallTurn();
    }

    public void CreateDust()
    {
        Dust.Play();
    }

    private void OnDrawGizmos()
    {
        // Desenha uma esfera para visualizar a checagem de chão
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public float GetHorizontalValue()
    {
        return horizontalValue;
    }

    public float GetVerticalValue()
    {
        return verticalValue;
    }

    public bool IsFacingRight()
    {
        return isFacingRight;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}