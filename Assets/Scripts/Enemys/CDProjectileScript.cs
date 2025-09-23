using UnityEngine;

public class CDProjectileScript : MonoBehaviour
{
    [SerializeField] Vector2 direction;
    [SerializeField] float speed;
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody2D rb;

    [SerializeField] string CDTag;

    [SerializeField] float lifetime;

    [SerializeField] Vector2 launchDir;

    public int damage;

    [SerializeField] string hitEffect = "EnemyXSlashEffect";

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Player")){
            //Vector2 damageDirection = (transform.position - collision.transform.position).normalized;
            if(collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, "Ouch! Por isso que eu migrei pro streaming!")){
                ObjectPoolManager.Instance.SpawnFromPool(hitEffect, collision.transform.position, Quaternion.identity);
                OnDespawn();
            }
        }
    }

    void OnEnable()
    {
        rb.linearVelocity = Vector2.zero;
        CancelInvoke("OnDespawn");
    }

    public void Initialize(Vector2 _direction){
        direction = _direction;
        anim.SetFloat("dirX", Mathf.Round(direction.x));
        Launch();
        Invoke("OnDespawn", lifetime);
    }

    void Launch(){
        launchDir = new Vector2(direction.x, 0f) * speed;
        rb.linearVelocity = launchDir;
    }

    void FixedUpdate(){
        if(rb.linearVelocity.x < launchDir.x){
            Launch();
        }
    }

    void OnDespawn(){
        ObjectPoolManager.Instance.ReturnToPool(this.gameObject, CDTag);
    }

    void OnDisable(){
        CancelInvoke("OnDespawn");
    }
}
