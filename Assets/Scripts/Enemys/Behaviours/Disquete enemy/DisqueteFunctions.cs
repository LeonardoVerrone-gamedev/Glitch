using UnityEngine;
using Unity.Behavior;
using Cinemachine;

public class DisqueteFunctions : MonoBehaviour
{
    [SerializeField] bool falling;
    [SerializeField] bool returningToStartPosition;
    [SerializeField] float fall_gravityScale = 3f;
    [SerializeField] Vector3 startPosition;
    [SerializeField] float returnSpeed = 5f;

    [SerializeField] float landTime = .5f;

    [SerializeField] BehaviorGraphAgent agent;
    [SerializeField] ParticleSystem dust_particle;
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody2D rb;

    [SerializeField] CinemachineImpulseSource impulse;
   // [SerializeField] DamageArea hitScript;

    void Start()
    {
        startPosition = transform.position;
    }

    public void Fall()
    {
        returningToStartPosition = false;
        falling = true;
        rb.gravityScale = fall_gravityScale;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if ((other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Wall")) && falling)
        {
            anim.SetTrigger("Land");
            impulse.GenerateImpulse();
           // hitScript.canCauseDamage = false;
            Invoke("ResetEnemy", 1.30f);
            dust_particle.Play();
            falling = false;
        }
    }

    void ResetEnemy()
    {
        rb.gravityScale = 0f;
        returningToStartPosition = true;
       // hitScript.canCauseDamage = true;
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }

    void FixedUpdate()
    {
        if (returningToStartPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed);

            if (transform.position == startPosition)
            {
                agent.BlackboardReference.SetVariableValue("EnemyStates", EnemyStates.Idle);
                returningToStartPosition = false;
            }
        }
    }
}
