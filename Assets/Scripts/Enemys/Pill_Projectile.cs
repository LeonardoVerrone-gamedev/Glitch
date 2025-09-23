using UnityEngine;

public class Pill_Projectile : MonoBehaviour
{
    [SerializeField] Vector2 direction;
    [SerializeField] float launchForce;

    [SerializeField] Rigidbody2D rb;

    [SerializeField] Animator anim;

    [SerializeField] string hitEffect = "EnemyXSlashEffect";

    public int damage;

    [SerializeField] string poolTag;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 _dir, float _launchForce)
    {
        direction = (Vector2.up + _dir).normalized;

        if(direction.x < 0f){
            anim.SetFloat("dirX", -1f);
        }else{
            anim.SetFloat("dirX",1f);
        }

        rb.AddForce(direction * _launchForce, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Player")){
            //Vector2 damageDirection = (transform.position - collision.transform.position).normalized;
            if(collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, "")){
                ObjectPoolManager.Instance.SpawnFromPool(hitEffect, collision.transform.position, Quaternion.identity);
                anim.SetTrigger("Destroy");
                Invoke("OnDespawn", 0.2f);
            }
        }
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Ground")){
            anim.SetTrigger("Destroy");
            Invoke("OnDespawn", 0.2f);  
        }
    }

    void OnDespawn(){
        ObjectPoolManager.Instance.ReturnToPool(this.gameObject, poolTag);
    }

    void OnEnable(){
        anim.SetTrigger("Respawn");
        rb.linearVelocity = Vector2.zero;
    }


    void OnDisable(){
        rb.linearVelocity = Vector2.zero;
    }
}
