using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public int damage;

    [SerializeField] string HitEffectName = "PlayerBulletExplosion";

    [SerializeField] GameObject explosionPrefab;

    [SerializeField] private float speed = 5f; // Velocidade do míssil
    [SerializeField] private float oscillationAmplitude = 0.5f; // Amplitude da oscilação
    [SerializeField] private float oscillationFrequency = 2f; // Frequência da oscilação
    [SerializeField] private float lifetime = 3f; // Tempo de vida do míssil
    [SerializeField] private float rotationSpeed = 5f; // Velocidade de rotação do míssil

    private Transform target; // O alvo que o míssil deve seguir
    private float startTime; // Tempo em que o míssil foi instanciado

    bool canAttackEnemy = false;

    public void Initialize(Transform newTarget)
    {
        target = newTarget; // Define o alvo
        StartCoroutine(StartTimer());
    }

    void OnEnable()
    {
        startTime = Time.time; // Captura o tempo de início
        Destroy(gameObject, lifetime); // Destrói o míssil após o tempo de vida
    }

    IEnumerator StartTimer(){
        yield return new WaitForSeconds(1f);
        canAttackEnemy = true;
    }

    void Update()
    {
        if (target != null)
        {
            // Calcula a direção para o alvo
            Vector3 direction = (target.position - transform.position).normalized;

            // Ajusta a rotação do míssil para apontar na direção do alvo
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Converte para graus
            Quaternion lookRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            // Move o míssil em direção ao alvo
            transform.position += direction * speed * Time.deltaTime;

            // Cálculo da oscilação
            float oscillation = Mathf.Sin((Time.time - startTime) * oscillationFrequency) * oscillationAmplitude;

            // Aplica a oscilação ao movimento do míssil
            transform.position += new Vector3(0, oscillation, 0) * Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Player")){
            //Vector2 damageDirection = (transform.position - collision.transform.position).normalized;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, "");
            ObjectPoolManager.Instance.SpawnFromPool(HitEffectName, collision.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }

        if(collision.gameObject.CompareTag("Enemy")){
            EnemyLife enemyLife =  collision.gameObject.GetComponent<EnemyLife>();
            if(enemyLife != null && canAttackEnemy){
                enemyLife.TakeDamage(damage, false);
                ObjectPoolManager.Instance.SpawnFromPool(HitEffectName, collision.transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
    }
}