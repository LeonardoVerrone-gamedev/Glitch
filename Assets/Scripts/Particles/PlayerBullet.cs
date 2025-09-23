using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Collections;

public class PlayerBullet : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 1;

    [Header("References")]
    private ParticleSystem particleSystem;
    private List<ParticleCollisionEvent> collisionEvents;
    [SerializeField] private CinemachineImpulseSource cameraShake;

    [Header("Effects")]
    [SerializeField] string HitEffectName = "PlayerBulletExplosion";
    [SerializeField] private float slowMotionFactor = 0.1f; // Fator de desaceleração
    [SerializeField] private float slowMotionDuration = 0.05f; // Duração do efeito
    [SerializeField] private float lifetime = 0.25f;

    [SerializeField] AudioClip shootClip;
    [SerializeField] AudioSource bulletAudioSource;

    private Coroutine lifetimeCoroutine;

    void OnEnable()
    {
        Initialize();
    }

    void OnDisable()
    {
        StopCoroutine(lifetimeCoroutine);
        particleSystem.Stop();
    }

    private void Initialize()
    {
        bulletAudioSource.clip = shootClip;
        bulletAudioSource.Play();
        particleSystem = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        particleSystem.Play();
        lifetimeCoroutine = StartCoroutine(DestroyAfterLifetime());
    }

    void OnParticleCollision(GameObject other)
    {
        if (ShouldIgnoreCollision(other))
        {
            return;
        }

        ProcessCollision(other);
        CreateExplosion(other);
    }

    private bool ShouldIgnoreCollision(GameObject other)
    {
        return other.CompareTag("Player") || other.CompareTag("Shield");
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        ReturnToPool();
    }

    private void ProcessCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            PostProcessVolumeManager.Instance.TriggerFlashEffect(3, .5f);
            EnemyLife enemyLife = other.GetComponent<EnemyLife>();
            enemyLife?.TakeDamage(damage, false);
        }
    }

    private void CreateExplosion(GameObject other)
    {
        int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);

        if (numCollisionEvents > 0)
        {
            Vector3 collisionPoint = collisionEvents[0].intersection;
            ObjectPoolManager.Instance.SpawnFromPool(HitEffectName, collisionPoint, Quaternion.identity);
            cameraShake?.GenerateImpulse();

            if (other.CompareTag("Enemy"))
            {
                TimeScaleManager.Instance.SetTimeScale(slowMotionFactor, slowMotionDuration);
            }

            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        ObjectPoolManager.Instance.ReturnToPool(gameObject, "PlayerBullet");
    }
}
