using UnityEngine;
using Unity.Behavior;

public class PlatformEnemyDirectionSetter : MonoBehaviour
{
    [Header("Geral")]
    [SerializeField] BehaviorGraphAgent agent;
    [SerializeField] GameObject player;
    [SerializeField] Animator anim;
    [SerializeField] public float XDirection { get; private set; }
    

    void Update()
    {
        if (player == null)
        {
            agent.BlackboardReference.GetVariableValue("Target", out player);
        }

        if (player.transform.position.x < transform.position.x)
        {
            XDirection = -1f;
        }
        else
        {
            XDirection = 1f;
        }

        anim.SetFloat("dirX", XDirection);
    }
}
