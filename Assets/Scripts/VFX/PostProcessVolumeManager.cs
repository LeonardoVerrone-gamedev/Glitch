using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PostProcessVolumeManager : MonoBehaviour
{
    public static PostProcessVolumeManager Instance { get; private set; }

    [Header("Configurações")]
    [Tooltip("Velocidade da transição (valores mais altos = mais rápido)")]
    [SerializeField] private float transitionSpeed = 30f;

    [Header("Volumes URP")]
    [Tooltip("Lista de volumes na ordem desejada (índice 0 = padrão)")]
    [SerializeField] private Volume[] volumesURP;

    private Coroutine activeTransition;
    private int currentActiveIndex = 0;

    bool inLowLifeMode;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    // Interface para chamadas externas
    public void SwitchToVolume(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= volumesURP.Length)
        {
            Debug.LogWarning($"Índice {targetIndex} inválido!");
            return;
        }

        if (activeTransition != null)
            StopCoroutine(activeTransition);

        activeTransition = StartCoroutine(TransitionVolumes(targetIndex));
    }

    // Corotina que faz a transição suave
    private IEnumerator TransitionVolumes(int targetIndex)
    {
        while (!CheckWeights(targetIndex))
        {
            float step = Time.deltaTime * transitionSpeed;

            for (int i = 0; i < volumesURP.Length; i++)
            {
                // Always keep volume 0 active
                if (i == 0 || (i == 1 && inLowLifeMode))
                {
                    volumesURP[i].weight = 1f;
                }
                else
                {
                    volumesURP[i].weight = Mathf.MoveTowards(
                        volumesURP[i].weight,
                        i == targetIndex ? 1f : 0f,
                        step
                    );
                }
            }
            yield return null;
        }

        currentActiveIndex = targetIndex;
    }

    // Verifica se todos os volumes atingiram seus pesos-alvo
    private bool CheckWeights(int targetIndex)
    {
        for (int i = 0; i < volumesURP.Length; i++)
        {
            float targetWeight = (i == 0) ? 1f : (i == targetIndex ? 1f : 0f);
            if (Mathf.Abs(volumesURP[i].weight - targetWeight) > 0.001f)
                return false;
        }
        return true;
    }

    // Método especial para efeitos rápidos (como flash de dano)
    public void TriggerFlashEffect(int flashVolumeIndex, float duration)
    {
        if (flashVolumeIndex < 0 || flashVolumeIndex >= volumesURP.Length) return;

        StartCoroutine(FlashRoutine(flashVolumeIndex, duration));
    }

    private IEnumerator FlashRoutine(int flashIndex, float duration)
    {
        int originalIndex = currentActiveIndex;

        // Ativação instantânea do efeito
        SwitchToVolume(flashIndex);
        yield return new WaitForSeconds(duration);

        // Retorno ao estado original
        ClearEffects();
    }

    // Método para limpar todos os efeitos de pós-processamento
    public void ClearEffects()
    {
        if (activeTransition != null)
            StopCoroutine(activeTransition);

        // Define o peso de todos os volumes para 0, exceto o volume 0
        for (int i = 2; i < volumesURP.Length; i++)
        {
            volumesURP[i].weight = 0f;
        }

        currentActiveIndex = 0; // Indica que o volume 0 está ativo
    }

    public void DeactivateVolume(int index)
    {
        volumesURP[index].weight = 0f;
    }

    public void ActivateLowLifeVolume(bool value)
    {
        volumesURP[1].weight = value ? 1f : 0f;

        inLowLifeMode = value;
    }
}
