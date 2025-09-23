using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAbility : Ability
{
    [SerializeField] bool OffensiveDash;

    private PlayerMovement playerMovement;
    private bool _isActive;

    [SerializeField] string HitEffectName;

    [SerializeField] float DashDuration = 0.5f;
    [SerializeField] float DashVelocity = 14f;

    [SerializeField] float SlowMotionFactor = 0.4f;
    [SerializeField] float SlowMotionDuration = 0.15f;

    float bonusDuration = 0f;

    [SerializeField] Animator anim;
    [SerializeField] Rigidbody2D rb;

    [SerializeField] ParticleSystem dashTrailParticleSystem;
    [SerializeField] ParticleSystem defaultFollowParticles;
    [SerializeField] PlayerAnimation playerAnimation;


    [SerializeField] SpriteRenderer spriteRenderer;

    PlayerInputs inputs;
    PlayerHealth health;
    Vector2 CurrentDashDirection;

    ErrorSaysScript errorSays;

    [SerializeField] string errorJoke = "Se um ouriço pode dar dash, eu também posso!";

    [SerializeField] float manaCostPerSecond = 10f;

    [SerializeField] ManaManager mana;

    [SerializeField] AbilityManager abilityManager;

    public Vector2 dashDirection;

    bool isDashing;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        inputs = GetComponent<PlayerInputs>();
        health = GetComponent<PlayerHealth>();
        mana = GetComponent<ManaManager>();
        abilityManager = GetComponent<AbilityManager>();
    }

    public override void Activate()
    {
        _isActive = true;
        ErrorSays_Joke();
        bonusDuration = 0f;

        mana.UseMana(manaCostPerSecond);

        if (defaultFollowParticles != null)
        {
            defaultFollowParticles.gameObject.SetActive(false);
        }

        StartCoroutine(Dash());
    }

    public override void Deactivate()
    {
        if (defaultFollowParticles != null)
        {
            defaultFollowParticles.gameObject.SetActive(true);
        }

        _isActive = false;
        abilityManager.CheckActiveAbilities();
    }

    public override float GetManaCost()
    {
        return 0f; // Custo de mana por segundo
    }

    public override bool isActive()
    {
        return _isActive;
    }

    void Update()
    {
        if (isDashing)
        {
            CurrentDashDirection = dashDirection.normalized;

            rb.linearVelocity = CurrentDashDirection * DashVelocity * 2f;
        }
    }

    IEnumerator Dash(){
        isDashing = true;
        playerAnimation.SetAnimationSpeed(2f);
        TimeScaleManager.Instance.SetTimeScale(SlowMotionFactor, DashDuration);

        rb.gravityScale = 0f;
        dashTrailParticleSystem.gameObject.SetActive(true);

        var emission = dashTrailParticleSystem.emission;
        emission.rateOverTime = 1f; // Ajuste conforme necessário

        inputs.canMove = false;
        health.invencible = true;

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Wall"), true);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true);

        transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);

        float duration = DashDuration; // Duração de um ciclo completo
        float time = 0f; // Tempo para o efeito de arco-íris

        while (time < DashDuration + bonusDuration)
        {
            time += Time.unscaledDeltaTime; // Aumenta o tempo com base no tempo real
            float t = time / duration; // Normaliza o tempo

            if (t > 1){
                t = 0f; // Reseta o tempo após completar um ciclo
            }

            // Calcula a cor do arco-íris
            Color color = Color.HSVToRGB(t, 1, 1);
            spriteRenderer.color = color; // Aplica a cor ao SpriteRenderer

            yield return null; // Espera o próximo frame
        }

        StopDash();
    }

    public void StopDash(){
        StopCoroutine(Dash());
        StartCoroutine(finishDash());
    }

    private IEnumerator finishDash(){
        isDashing = false;

        var emission = dashTrailParticleSystem.emission;
        emission.rateOverTime = 0f; // Ajuste conforme necessário

        rb.linearVelocity = Vector2.zero;

        //rb.velocity = Vector2.zero;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), false);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Wall"), false);

        inputs.canMove = true;
        playerAnimation.SetAnimationSpeed(1f);
        spriteRenderer.color = Color.white;

        //AQUI SETE A EMISSAO POR DISTANCIA PARA 0f;

        yield return new WaitForSeconds(.12f);
        dashTrailParticleSystem.gameObject.SetActive(false);
        rb.gravityScale = 3f;
        health.invencible = false;

        Deactivate();
    }

    void OnCollisionEnter2D(Collision2D other){
        if(_isActive && OffensiveDash){
            if(other.gameObject.tag == "Enemy"){
                //bonusDuration+= 0.15f;
                PostProcessVolumeManager.Instance.TriggerFlashEffect(3, .5f);
                ObjectPoolManager.Instance.SpawnFromPool(HitEffectName, transform.position, Quaternion.identity);

                EnemyLife enemyLife = other.gameObject.GetComponent<EnemyLife>();
                if (enemyLife != null)
                {
                    enemyLife.TakeDamage(10, false);
                }
            }
        }
    }

    public void ErrorSays_Joke(){
        if(errorSays == null){
            errorSays = FindObjectOfType<ErrorSaysScript>();
        }
        errorSays.ErrorSaysCall(errorJoke);
    }
}
