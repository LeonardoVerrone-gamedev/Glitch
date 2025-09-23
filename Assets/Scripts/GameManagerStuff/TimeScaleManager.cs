using UnityEngine;
using System.Collections;

public class TimeScaleManager : MonoBehaviour
{

    public static TimeScaleManager Instance { get; private set; }

    [SerializeField] float OriginalTimeScale = 1f;

    [SerializeField] Coroutine setTimeScaleCurrentCoroutine;

    bool permanent;

    void Awake()
    {
        //SINGLETON
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }


    public void SetTimeScale(float newTimeScale, float duration)
    {
        if (permanent)
        {
            return;
        }
        if (setTimeScaleCurrentCoroutine != null)
        {
            StopCoroutine(setTimeScaleCurrentCoroutine);
            ResetTimeScale();
        }
        setTimeScaleCurrentCoroutine = StartCoroutine(SetTimeScaleCoroutine(newTimeScale, duration));
    }

    private IEnumerator SetTimeScaleCoroutine(float newTimeScale, float duration)
    {
        Time.timeScale = newTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Ajusta o tempo fixo para física

        // Aguarda a duração do efeito
        yield return new WaitForSecondsRealtime(duration);
        Debug.Log("volta o tempo ao normal");
        // Restaura o tempo
        ResetTimeScale();
    }

    public void ResetTimeScale()
    {
        permanent = false;
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f; // Restaura o tempo fixo para física
    }

    public void SetPermanentTimeScale(float factor)
    {
        SetTimeScale(factor, Mathf.Infinity);
        permanent = true;
    }
}
