using UnityEngine;
using Unity.Behavior;

public class FlappyEnemyFunctions : MonoBehaviour
{
    [SerializeField] BehaviorGraphAgent agent;
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;

    [SerializeField] bool isAttacking;
    [SerializeField] float flySpeed;

    [SerializeField] float flyPower;

    [SerializeField] EnemyStates state;

    Vector2 directionToPlayer;
    [SerializeField] float horizontalTolerance = 1.5f;
    

    void Update()
    {
        if (player == null)
        {
            agent.BlackboardReference.GetVariableValue("Target", out player);
            return;
        }

        agent.BlackboardReference.GetVariableValue("EnemyStates", out state);

        if (state == EnemyStates.Attacking)
        {
            isAttacking = true;
        }

        if (state == EnemyStates.Hurt || state == EnemyStates.Die)
        {
            StopAtack();
            return;
        }


        if (!isAttacking)
        {
            return;
        }

        Attack();

        if (player.transform.position.y < transform.position.y)
        {
            anim.SetFloat("dirY", -1f);
        }
        else
        {
            anim.SetFloat("dirY", 1f);
        }
    }

    void Attack()
    {
        if (player == null)
        {
            agent.BlackboardReference.GetVariableValue("Target", out player);
        }

        float horizontalDifference = Mathf.Abs(player.transform.position.x - transform.position.x);

        // Only change direction if the difference is outside the tolerance
        if (horizontalDifference > horizontalTolerance)
        {
            if (player.transform.position.x < transform.position.x)
            {
                anim.SetFloat("dirX", -1f);
                directionToPlayer = new Vector2(-1f, 0f);
            }
            else
            {
                anim.SetFloat("dirX", 1f);
                directionToPlayer = new Vector2(1f, 0f);
            }
        }

        rb.linearVelocity = directionToPlayer * flySpeed;

        isAttacking = true;
    }

    void StopAtack()
    {
        rb.linearVelocity = Vector2.zero;
        isAttacking = false;
    }

    void Fly()
    {
        if (!isAttacking)
        {
            return;
        }
        rb.gravityScale = -flyPower;
       // rb.AddForce(Vector2.up * flyPower, ForceMode2D.Impulse);
    }

    void Fall()
    {
        if (!isAttacking)
        {
            return;
        }
        rb.gravityScale = flyPower;
    }
}
