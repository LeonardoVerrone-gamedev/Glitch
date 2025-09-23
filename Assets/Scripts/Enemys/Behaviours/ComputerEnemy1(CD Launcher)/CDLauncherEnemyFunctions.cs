using UnityEngine;
using Unity.Behavior;

public class CDLauncherEnemyFunctions : MonoBehaviour
{
    [SerializeField] BehaviorGraphAgent agent;
    [SerializeField] GameObject player;
    [SerializeField] string CDProjectileTAG;
    public bool PlayerIsInRange;
    [SerializeField] SpriteRenderer sprite;

    [SerializeField] Transform[] all_shootPoints;

    [SerializeField] Transform current_shootPoint;

    [SerializeField]Vector2 _shootDirection;

    public void Shoot(){
        ObjectPoolManager.Instance.SpawnFromPool(CDProjectileTAG, current_shootPoint.position, Quaternion.identity).GetComponent<CDProjectileScript>().Initialize(_shootDirection);
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null){
            agent.BlackboardReference.GetVariableValue("Target", out player);
        }
        agent.BlackboardReference.GetVariableValue("isPlayerNear", out PlayerIsInRange);

        if(PlayerIsInRange){
            if(player.transform.position.x < transform.position.x){
                sprite.flipX = true;
                current_shootPoint = all_shootPoints[1];
                _shootDirection = new Vector2(-1f, 0f);
            }else{
                sprite.flipX = false;
                current_shootPoint = all_shootPoints[0];
                _shootDirection = new Vector2(1f, 0f);
            }
        }
    }
}
