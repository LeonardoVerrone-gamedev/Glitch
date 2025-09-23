using UnityEngine;
using System.Collections;
using UnityEditor.Callbacks;

public class StarAbility : Ability
{

    [SerializeField] bool invencible;


    [SerializeField] SpriteRenderer sprite;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerHealth health;

    [SerializeField] PlayerAnimation playerAnimation;

    [SerializeField] string HitEffectName = "PlayerSlashEffect";

    [SerializeField] Rigidbody2D rb;

    [SerializeField] Animator asasAnim;

    [SerializeField] ParticleSystem brightStars;
    [SerializeField] ParticleSystem defaultFollowParticles;

    ErrorSaysScript errorSays;
    [SerializeField] string errorJoke = "O que? Parece uma Super Estrela? Nada a ver!";

    [SerializeField] float manaCostPerSecond = 20f;

    [SerializeField] float speedMultiplier = 4f;

    private bool _isActive;


    public override void Activate()
    {
        _isActive = true;
        ErrorSays_Joke();
        brightStars.Play();
        playerMovement.SetSpeedMultiplier(+speedMultiplier);

        if (invencible)
        {
            health.invencible = true;
        }

        if (defaultFollowParticles != null)
        {
            defaultFollowParticles.gameObject.SetActive(false);
        }

        playerAnimation.SetAnimationSpeed(2f);
        TimeScaleManager.Instance.SetPermanentTimeScale(0.2f);
        asasAnim.speed = 2f;
        StartCoroutine(ColorCoroutine());
    }

    public override void Deactivate()
    {
        _isActive = false;
        brightStars.Stop();
        sprite.color = Color.white;
        playerMovement.SetSpeedMultiplier(-speedMultiplier);
        playerAnimation.SetAnimationSpeed(1f);
        TimeScaleManager.Instance.ResetTimeScale();
        asasAnim.speed = 1f;

        if (invencible)
        {
            health.invencible = false;
        }

        if (defaultFollowParticles != null)
        {
            defaultFollowParticles.gameObject.SetActive(true);
        }
    }

    public override float GetManaCost()
    {
        return manaCostPerSecond; // Custo de mana por segundo
    }

    public override bool isActive()
    {
        return _isActive;
    }

    private IEnumerator ColorCoroutine()
    {
        float time = 0f;
        while (_isActive)
        {
            time += Time.unscaledDeltaTime; // Aumenta o tempo com base no tempo real
            if (time > .5f)
            {
                time = 0f; // Reseta o tempo após completar um ciclo
            }
            // Normaliza o tempo para o intervalo [0, 1]
            float t = time / .5f; // Divide pelo tempo total para normalizar
            // Calcula a cor do arco-íris
            Color color = Color.HSVToRGB(t, 1, 1);
            sprite.color = color; // Aplica a cor ao SpriteRenderer
            yield return null; // Espera o próximo frame
        }
        // Após sair do loop, você pode definir a cor final, se necessário
        sprite.color = Color.white; // Reseta a cor para branco ou outra cor desejada
    }

    void FixedUpdate()
    {
        if (!_isActive)
        {
            return;
        }
        rb.linearVelocity = rb.linearVelocity + (Vector2.down * 1.5f);
    }
    
    void OnCollisionEnter2D(Collision2D other){
        if (!invencible)
        {
            return;
        }
        
        if (_isActive)
        {
            if (other.gameObject.tag == "Enemy")
            {
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
