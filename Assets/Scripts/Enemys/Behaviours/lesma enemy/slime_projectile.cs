using UnityEngine;

public class slime_projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isDestroying = false;

    [SerializeField] float lifetime = 3f;

    [SerializeField] string poolTag;
    public int damage;
    [SerializeField] string hitEffect = "EnemyXSlashEffect";

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        isDestroying = false;

        Invoke("launch", .2f);
    }

    void launch()
    {
        // Movimento baseado na rotação atual (forward do objeto)
        rb.linearVelocity = transform.right * speed;

        Invoke("DestroyProjectile", lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroying) return;

        if (collision.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, ""))
            {
                ObjectPoolManager.Instance.SpawnFromPool(hitEffect, collision.transform.position, Quaternion.identity);
            }
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        isDestroying = true;
        //rb.linearVelocity = Vector2.zero;

        if (anim != null)
        {
            anim.SetTrigger("Destroy");
        }
        else
        {
            OnAnimationEnd();
        }
    }

    // Chamado por evento de animação
    public void OnAnimationEnd()
    {
        ObjectPoolManager.Instance.ReturnToPool(this.gameObject, poolTag);
    }

    void OnDisable()
    {
        rb.linearVelocity = Vector2.zero;
        transform.rotation = Quaternion.identity;
        CancelInvoke("DestroyProjectile");
    }
}
