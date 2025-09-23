using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    public int damage;

    public bool canCauseDamage = true;

    [SerializeField] string hitEffect = "EnemyXSlashEffect";

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Player") && canCauseDamage){
            //Vector2 damageDirection = (transform.position - collision.transform.position).normalized;
            if(collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, "")){
                ObjectPoolManager.Instance.SpawnFromPool(hitEffect, collision.transform.position, Quaternion.identity);
            }
        }
    }
}
