using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShockWaveManager : MonoBehaviour
{
    [SerializeField] float shockWaveTime = 0.75f;

    [SerializeField] private float timeBlendFactor = 0.5f; // Fator de mistura entre deltaTime e unscaledDeltaTime

    private Coroutine shockWaveCoroutine;

    [SerializeField]private Material _material;

    private static int waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

    void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    void OnEnable()
    {
        CallShockWave();
    }

    public void CallShockWave()
    {
        shockWaveCoroutine = StartCoroutine(ActionShockWaveCoroutine(-0.1f, 1f));
    }

    private IEnumerator ActionShockWaveCoroutine(float startPos, float endPos)
    {
        _material.SetFloat(waveDistanceFromCenter, startPos);

        float lerpAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < shockWaveTime)
        {
            float blendedDeltaTime = Mathf.Lerp(Time.deltaTime, Time.unscaledDeltaTime, timeBlendFactor);
            elapsedTime += blendedDeltaTime;
            
            lerpAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / shockWaveTime));
            _material.SetFloat(waveDistanceFromCenter, lerpAmount);
            yield return null;
        }

        ObjectPoolManager.Instance.ReturnToPool(this.gameObject, "Shockwave");
    }
}
