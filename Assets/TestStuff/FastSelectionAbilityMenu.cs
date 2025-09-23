using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FastSelectionAbilityMenu : MonoBehaviour
{
    [SerializeField] List<Move> moves;

    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI abilityName;

    [SerializeField] PlayerCombat player;

    [SerializeField] Move selectedMove;

    [SerializeField] GameObject menu;

    [SerializeField] Animator WindowAnimator;

    [SerializeField]int index;

    public bool RB;

    public bool inMenu = false; 

    Sprite newIcon;
    string newName;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
        //ResetInventory();
    }

    public void OpenMenu(){
        if(RB){
            index = moves.IndexOf(player.fastMoveRight);
        }else{
            index = moves.IndexOf(player.fastMoveLeft);
        }
        menu.SetActive(true);
        inMenu = true;
        ChangeAbility();
    }

    public void Scroll(string value){
        switch(value){
            case "left":

                if(index > 0){
                    index--;
                }else{
                    index = moves.Count - 1;
                }

                ChangeAbility();
                break;

            case "right":

                if(index < moves.Count - 1){
                    index++;
                }else{
                    index = 0;
                }

                ChangeAbility();
                break;
        }
    }

    public void Cancel(){
        inMenu = false;
        menu.SetActive(false);
    }

    public void ChangeWindow(){
        abilityName.text = newName;
        icon.sprite = newIcon;
    }

    public void ChangeAbility(){
        selectedMove = moves[index];
        newIcon = selectedMove.icon;
        newName = selectedMove.MoveName;

        ChangeWindow();
        WindowAnimator.SetTrigger("SwitchAbility");
    }

    public void SetFastAbility(){
        if(RB){
            player.fastMoveRight = selectedMove;
        }else{
            player.fastMoveLeft = selectedMove;
        }
        Cancel();
        inMenu = false;
    }
}
