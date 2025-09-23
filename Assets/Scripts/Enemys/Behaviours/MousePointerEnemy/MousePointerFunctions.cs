using UnityEngine;
using Unity.Behavior;

public class MousePointerFunctions : MonoBehaviour
{
    [SerializeField] GameObject ColliderOBJ;
    [SerializeField] BehaviorGraphAgent agent;

    [SerializeField] GameObject _player;

    [SerializeField] EnemyLife life;

    public bool isPlayerInsideTrigger;

    bool isPlayerCaptured;


    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player")){
            isPlayerInsideTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.CompareTag("Player")){
            if(_player != null){
                _player.transform.parent = null;
            }
            isPlayerCaptured = false;
            isPlayerInsideTrigger = false;
            ColliderOBJ.SetActive(false);
            life.invulnerable = false;
            agent.BlackboardReference.SetVariableValue("isPlayerCaptured", false);
        }
    }

    public void OnClick(){
        if(isPlayerCaptured){
            Drop();
        }else{
            Catch();
        }
    }

    void Drop(){
        isPlayerCaptured = false;
        if(_player != null){
            _player.transform.parent = null;
        }
        ColliderOBJ.SetActive(false);
        agent.BlackboardReference.SetVariableValue("isPlayerCaptured", false);
    }

    void Catch(){
        agent.BlackboardReference.GetVariableValue("Target", out _player);

        if(!isPlayerInsideTrigger){
            return;
        } 

        _player.transform.position = transform.position;
        ColliderOBJ.SetActive(true);
        _player.transform.parent = ColliderOBJ.transform;
        _player.transform.position = transform.position;
        isPlayerCaptured = true;
        agent.BlackboardReference.SetVariableValue("isPlayerCaptured", true);
        life.invulnerable = true;
    }
}
