using UnityEngine;
// No longer need System.Collections.Generic for List,
// but keep if you're using other List-based features elsewhere.
// using System.Collections.Generic; 

public class FireWallParticlesScript : MonoBehaviour
{
    public int damage = 10;
    public bool canCauseDamage = true;
    [SerializeField] string hitEffect = "EnemyXSlashEffect";

    // Add a reference to your ParticleSystem
    private ParticleSystem ps;
    // **Change this to an array**
    private ParticleSystem.Particle[] activeParticles; 

    void Awake()
    {
        // Get the reference to the ParticleSystem on the same GameObject
        ps = GetComponent<ParticleSystem>();
        
        // Initialize the array with a size. 
        // A good practice is to use ps.main.maxParticles, 
        // which is the maximum number of particles the system can emit.
        activeParticles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void OnParticleCollision(GameObject other)
    {
        // 1. Check if the collided object is not the Player
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        // 2. If 'canCauseDamage' is false, return immediately
        if (!canCauseDamage)
        {
            return;
        }

        // 3. Get the active particles from the system.
        // GetParticles now writes into the array.
        int numParticles = ps.GetParticles(activeParticles);

        // 4. Iterate over the particles to find one that meets the damage condition.
        // We iterate up to numParticles, as not all elements in the array might be filled.
        for (int i = 0; i < numParticles; i++)
        {
            ParticleSystem.Particle p = activeParticles[i];

            // Calculate the normalized lifetime.
            // This is (original lifetime - remaining lifetime) / original lifetime
            float currentNormalizedLifetime = (p.startLifetime - p.remainingLifetime) / p.startLifetime;

            // 5. Check if the particle is still within the first 75% of its life.
            if (currentNormalizedLifetime < 0.75f)
            {
                // 6. Apply damage to the Player
                if (other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, "OUCH! Queimei minha bunda!!"))
                {
                    // 7. Spawn the hit effect
                    ObjectPoolManager.Instance.SpawnFromPool(hitEffect, other.transform.position, Quaternion.identity);
                    // 8. Break the loop as damage has been applied by a valid particle.
                    break; 
                }
            }
        }
    }
}