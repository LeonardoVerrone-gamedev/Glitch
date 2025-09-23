using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerFlameThrowerScript : MonoBehaviour
{
    [SerializeField] string hitEffect = "EnemyXSlashEffect";
    public int damage = 1;

    [SerializeField] List<EnemyLife> burnedEnemies = new List<EnemyLife>();
    [SerializeField] float timeToResetList = .5f;
    float timer;

    void OnParticleCollision(GameObject other)
    {
        EnemyLife enemyLife = other.gameObject.GetComponent<EnemyLife>();

        if (enemyLife != null && !burnedEnemies.Contains(enemyLife))
        {
            burnedEnemies.Add(enemyLife);
            enemyLife.TakeDamage(damage, false);
            timer = timeToResetList;
            ObjectPoolManager.Instance.SpawnFromPool(hitEffect, other.transform.position, Quaternion.identity);
        }
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
        }
        else
        {
            burnedEnemies.Clear();
        }
    }
}
