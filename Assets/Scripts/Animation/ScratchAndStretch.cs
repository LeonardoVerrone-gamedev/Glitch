using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchAndStretch : MonoBehaviour
{
    public StretchPatternSO[] patterns; // Usando Scriptable Objects
    public Vector3 originalScale = new Vector3(1f, 1f, 1f);
    private SpriteRenderer spriteRenderer;
    private Coroutine currentCoroutine;
    private string currentAction;
    private int currentPriority; // Adicionando um campo para armazenar a prioridade atual

    [SerializeField] Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentPriority = -1; // Inicializa a prioridade atual como um valor baixo
    }

    public void PlayStretchAnimation(string actionName)
    {
        StretchPatternSO pattern = System.Array.Find(patterns, p => p.actionName == actionName);
        if (pattern != null)
        {
            // Se a animação atual é a mesma e é um loop, não faz nada
            if (currentAction == actionName && pattern.loop)
            {
                return;
            }

            // Verifica a prioridade
            if (pattern.priority >= currentPriority)
            {
                if (currentCoroutine != null)
                {
                    // Garantir que a escala final seja a original
                    transform.localScale = originalScale;
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = originalColor;
                    }
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(StretchCoroutine(pattern));
                currentAction = actionName; // Atualiza a ação atual
                currentPriority = pattern.priority; // Atualiza a prioridade atual
            }
            //Debug.Log("Playing", actionName);
        }
        else
        {
            Debug.LogWarning($"No stretch pattern found for action: {actionName}");
        }
    }

    private IEnumerator StretchCoroutine(StretchPatternSO pattern)
    {
        if (spriteRenderer != null)
        {
            //originalColor = Color.green;
        }

        // Loop se a animação for em loop
        if (pattern.loop)
        {
            while (true)
            {
                // Stretch
                yield return Stretch(originalScale, pattern);

                // Squash
                //yield return Squash(originalScale, pattern);
            }
        }
        else
        {
            // Garantir que a escala inicial seja a original
            transform.localScale = originalScale;

            // Stretch
            yield return Stretch(originalScale, pattern);

            // Squash
            //yield return Squash(originalScale, pattern);

            // Retornar à escala original
            yield return ReturnToOriginalScale(originalScale, pattern);
        }
    }

    private IEnumerator Stretch(Vector3 originalScale, StretchPatternSO pattern)
    {
        float elapsedTime = 0f;
        float totalDuration = pattern.stretchCurve.keys[pattern.stretchCurve.length - 1].time; // Duração total da curva

        while (elapsedTime < totalDuration)
        {
            float t = elapsedTime / totalDuration;
            float stretchFactor = pattern.stretchCurve.Evaluate(t);
            float squashFactor = 1f / stretchFactor; // Calcular o fator de squash para manter a massa constante

            // Aplicar a escala
            transform.localScale = new Vector3(originalScale.x * squashFactor, originalScale.y * stretchFactor, originalScale.z);

            // Aplicar a cor correspondente se a opção de mudança de cor estiver ativada
            if (pattern.changeColor && pattern.colors.Length > 0)
            {
                // Encontrar o índice da chave correspondente
                int keyIndex = Mathf.FloorToInt(t * (pattern.stretchCurve.length - 1));
                keyIndex = Mathf.Clamp(keyIndex, 0, pattern.colors.Length - 1); // Garantir que o índice esteja dentro dos limites

                // Aplicar a cor ao SpriteRenderer
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = pattern.colors[keyIndex];
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Squash(Vector3 originalScale, StretchPatternSO pattern)
    {
        float elapsedTime = 0f;
        float totalDuration = pattern.stretchCurve.keys[pattern.stretchCurve.length - 1].time; // Duração total da curva

        while (elapsedTime < totalDuration)
        {
            float t = elapsedTime / totalDuration;
            float squashFactor = pattern.stretchCurve.Evaluate(1 - t); // Inverso da curva de stretch

            // Aplicar a escala
            transform.localScale = new Vector3(originalScale.x * squashFactor, originalScale.y * squashFactor, originalScale.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ReturnToOriginalScale(Vector3 originalScale, StretchPatternSO pattern)
    {
        float elapsedTime = 0f;
        float totalDuration = pattern.stretchCurve.keys[pattern.stretchCurve.length - 1].time; // Duração total da curva

        while (elapsedTime < totalDuration)
        {
            float t = elapsedTime / totalDuration;
            float returnFactor = Mathf.Lerp(1, 1, t); // Retornar à escala original

            // Aplicar a escala
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, returnFactor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentPriority = 0;

        // Garantir que a escala final seja a original
        transform.localScale = originalScale;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}