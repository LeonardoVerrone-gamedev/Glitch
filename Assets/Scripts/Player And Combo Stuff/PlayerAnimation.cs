using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]private Animator animator;
    [SerializeField] Animator asasAnimator;

    [SerializeField]private SpriteRenderer spriteRenderer;
    [SerializeField] private Material simpleUnlit;
    [SerializeField] private Material rainbow;
    
    Rigidbody2D rb;
    private PlayerMovement playerMovement;
    [SerializeField]private ScratchAndStretch scratchAndStretch;

    public bool Planando;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(playerMovement.GetHorizontalValue()));

        animator.SetBool("Jumping", !playerMovement.IsGrounded());
       // animator.SetBool("Planando", playerMovement.estaPlanando);
        //animator.SetBool("LookingUp", playerMovement.isLookingUp);
        //animator.SetBool("PushBack", playerMovement.isBeingPushed);
    }

    public void TransformAnimation(){
        animator.SetTrigger("Transform");
        //GetComponent<Player>().StopMove();
        //GetComponent<Player>().Invoke("StartMove", 0.8f);
    }

    public void PlayAnimationByTrigger(string trigger){
        animator.SetTrigger(trigger);
    }

    public void PlayAsasAnimation()
    {
        asasAnimator.SetTrigger("Bate");
    }

    public void SetAnimationSpeed(float speed)
    {
        animator.speed = speed;
    }
}