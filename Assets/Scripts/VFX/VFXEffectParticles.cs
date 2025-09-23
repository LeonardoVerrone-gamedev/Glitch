using UnityEngine;
using System.Collections;

public class VFXEffectParticles : MonoBehaviour
{
    [SerializeField] string thisPrefabTag;

    [SerializeField] float lifetime;
    [SerializeField] ParticleSystem[] particles;

    [SerializeField] AudioSource soundEffect;

    private Coroutine disableCoroutine;


    void OnEnable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisableAfterLifetime());

        if(soundEffect != null){
            soundEffect.Play();
        }

        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }

    private IEnumerator DisableAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        ObjectPoolManager.Instance.ReturnToPool(this.gameObject, thisPrefabTag);
    }
}
