using UnityEngine;
using Unity.Behavior;

public class NurseFunctions : MonoBehaviour
{

    [Header("Geral")]
    [SerializeField] BehaviorGraphAgent agent;
    [SerializeField] GameObject player;

    [SerializeField] Animator anim;
    [SerializeField] Transform current_shootPoint;

    float XDirection;

    [Header("Nurse")]

    [SerializeField] float launchForce = 6f;
    [SerializeField] string pillProjectile_tag;
  

    [Header("Doctor")]

    [SerializeField] float contactRadius;

    [SerializeField] LayerMask playerLayer;

    [SerializeField] int damage;

    [SerializeField] string hitEffect = "EnemyXSlashEffect";


    [SerializeField] Transform[] contactPoints;
    [SerializeField] Transform selectedPoint;

    [Header("Bomberguy")]

    [SerializeField] string BombProjectile_poolTag;


    void Update()
    {
        if (player == null)
        {
            agent.BlackboardReference.GetVariableValue("Target", out player);
        }

        if (player.transform.position.x < transform.position.x)
        {
            XDirection = -1f;
        }
        else
        {
            XDirection = 1f;
        }

        anim.SetFloat("dirX", XDirection);
    }

    public void ShootPill()
    {
        Vector2 shootDir = new Vector2(XDirection, 1f);

        float distance = Mathf.Abs(transform.position.x - player.transform.position.x);
        float realForce = distance * launchForce;
        ObjectPoolManager.Instance.SpawnFromPool(pillProjectile_tag, current_shootPoint.position, Quaternion.identity).GetComponent<Pill_Projectile>().Launch(shootDir, realForce);
    }

    public void ShootBomb()
    {
        Vector2 shootDir = new Vector2(XDirection, 1f);

        float distance = Mathf.Abs(transform.position.x - player.transform.position.x);
        float realForce = distance * launchForce;
        ObjectPoolManager.Instance.SpawnFromPool(BombProjectile_poolTag, current_shootPoint.position, Quaternion.identity).GetComponent<Bomb_Projectile_Script>().Launch(shootDir, realForce);
    }

    public void SpearAttack()
    {
        if (XDirection == 1f)
        {
            selectedPoint = contactPoints[0];
        }
        else
        {
            selectedPoint = contactPoints[1];
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(selectedPoint.position, contactRadius, playerLayer);

        foreach (Collider2D col in colliders)
        {
            PlayerHealth playerLife = col.gameObject.GetComponent<PlayerHealth>();

            if (playerLife != null)
            {
                playerLife.TakeDamage(damage, "AI! Sempre odiei tomar vacina!");
                ObjectPoolManager.Instance.SpawnFromPool(hitEffect, col.transform.position, Quaternion.identity);
            }
        }
    }
}
