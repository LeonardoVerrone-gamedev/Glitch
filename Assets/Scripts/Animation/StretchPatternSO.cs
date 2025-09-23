using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewStretchPattern", menuName = "Animation/ScratchAndStretchPattern")]
public class StretchPatternSO : ScriptableObject
{
    public string actionName;               // Nome da ação (ex: "Jump", "Land", "Idle", "Walk")
    public AnimationCurve stretchCurve;      // Curva que controla o efeito de squash e stretch
    public float maxStretchFactor = 1.5f;    // Fator máximo de estiramento
    public bool loop;                        // Se a animação deve ser em loop
    public int priority;

    public bool changeColor;                 // Se a animação deve mudar a cor de renderização
    public Color[] colors;                   // Lista de cores para cada chave na curva
}
