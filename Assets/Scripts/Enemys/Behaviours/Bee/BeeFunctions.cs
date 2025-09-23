using UnityEngine;
using Unity.Behavior;

public class BeeFunctions : MonoBehaviour
{
    [SerializeField] BehaviorGraphAgent agent;
    [SerializeField] GameObject player;
    [SerializeField] float attackSpeed = 5f;

    [SerializeField] Animator anim;

    [SerializeField] BoxCollider2D collider;

    [SerializeField] bool SettingNewPosition;

    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] ParticleSystem blood;

    [SerializeField] AudioSource[] sounds; //0 die; 1 hurt; 2 die

    public bool PlayerIsInRange;


    public void SetPlayer(GameObject player){
        this.player = player;
    }

    public void SetAttackDirection(){
        SettingNewPosition = false;

        Vector2 direction = (player.transform.position - transform.position).normalized;
        float dirX; 
        float dirY;

        dirX = (direction.x > 0f) ? 1f : -1f;

        dirY = (direction.y > 0f) ? 1f : -1f;

        anim.SetFloat("AttackDirectionX", dirX);
        anim.SetFloat("AttackDirectionY", dirY);

        Flip(false);

       // anim.SetTrigger("Attack");
    }

    public void PlaySound(int index){
        sounds[index].Play();
    }

    public void Bleed(){
        blood.Play();
    }

    public void TranslateCurrentPosition(){
        //if(SettingNewPosition){
           // return;
        //}
        anim.speed = 0f;

        SettingNewPosition = true;

        Vector2 _position = transform.position;

        Vector2 _collider_offset = collider.offset;

        Vector2 _scale = transform.localScale;

        Vector2 _realOffset = new Vector2(_collider_offset.x * _scale.x, _collider_offset.y * _scale.y);

        Vector3 newPosition = new Vector3(_position.x + _realOffset.x, _position.y + _realOffset.y, 0f);

        transform.position = newPosition;

        collider.offset = Vector2.zero;

        anim.speed = 1f;
    }

    public void CheckFlip(){
        agent.BlackboardReference.GetVariableValue("isPlayerNear", out PlayerIsInRange);
        if(player == null || !PlayerIsInRange){
            return;
        }

        Vector2 direction = (player.transform.position - transform.position).normalized;
        float dirX; 

        dirX = (direction.x > 0f) ? 1f : -1f;

        bool flipValue = (dirX == -1f);

        Flip(flipValue);
    }

    public void Flip(bool value){
        spriteRenderer.flipX = value;
    }

    Vector2 ConvertToWorldPosition(){
        Vector2 currentWorldPosition = (Vector2)transform.position + (Vector2)collider.offset;
        return currentWorldPosition;
    }
}
