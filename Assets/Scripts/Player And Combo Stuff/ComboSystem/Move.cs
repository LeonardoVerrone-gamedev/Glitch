using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "New Move")]
public class Move : ScriptableObject
{
    [SerializeField] public string MoveName;

    [SerializeField] public List<string> movesKeyCodes; //lista e sequencias
    [SerializeField] Moves moveType; 
    [SerializeField] int ComboPriorty = 0; // quanto mais complicado maior a prioridade
    public Sprite icon; //imagem das teclas do combo

    public bool isMoveAvilable(List<string> playerKeyCodes) 
    {
        int comboIndex = 0;

        for (int i = 0; i < playerKeyCodes.Count; i++)
        {
            if (playerKeyCodes[i] == movesKeyCodes[comboIndex])
            {
                comboIndex++;
                if (comboIndex == movesKeyCodes.Count) 
                    return true;
            }
            else
                comboIndex = 0;
        }
        return false;
    }

    //Getters
    public int GetMoveComboCount()
    {
        return movesKeyCodes.Count;
    }
    public int GetMoveComboPriorty()
    {
        return ComboPriorty;
    }
    public Moves GetMove()
    {
        return moveType;
    }
}
