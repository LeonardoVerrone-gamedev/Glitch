using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Behavior;

public class EnemyLife : MonoBehaviour
{
    [SerializeField] BehaviorGraphAgent agent;

    [SerializeField]bool projectile;
    [SerializeField] string projectileTag;

    public bool invulnerable = false;

    void Awake(){
        agent = GetComponent<BehaviorGraphAgent>();
    }

    public void TakeDamage(int damage, bool rompeEscudo){
        
        if(invulnerable && !rompeEscudo){ return; }

        if (!projectile)
        {
            agent.BlackboardReference.SetVariableValue("Damage", damage);

            EnemyStates Enemy_currentState;
            agent.BlackboardReference.GetVariableValue("EnemyStates", out Enemy_currentState);
            if (Enemy_currentState != EnemyStates.Hurt && Enemy_currentState != EnemyStates.Die)
            {
                agent.BlackboardReference.SetVariableValue("EnemyStates", EnemyStates.Hurt);
            }
        }

        if(projectile){
            ObjectPoolManager.Instance.ReturnToPool(this.gameObject, projectileTag);
        }
    }

    public void SetInvencible(int value){
        if (value == 1)
        {
            invulnerable = true;
        }
        else
        {
            invulnerable = false;
        }
    }
    
}
