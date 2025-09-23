using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using Cinemachine;

public class GiantAbility : Ability
{
    [SerializeField] Vector3 GiantSize;
    [SerializeField] Vector3 OriginalScale =Vector3.one;

    [SerializeField] float original_GroundCheckRadius = 0.2f;
    [SerializeField] float giant_groundCheckRadius;

    [SerializeField] PlayerMovement movement;

    [SerializeField] float scaleLerpDuration;

    [SerializeField] float manaCostPerSecond = 5f;
    bool _isActive;

    [SerializeField] LayerMask wallLayer;

    [SerializeField] string HitEffectName;

    [SerializeField] string PlayerSlashEffect = "PlayerSlashEffect";

    public CinemachineImpulseSource impulseSource; // Referência ao Impulse Source

    PlayerHealth health;

    [SerializeField] BoxCollider2D wallCollider;

    ErrorSaysScript errorSays;

    [SerializeField] string errorJoke = "Isso sim é um bom jeito de atravessar uma parede!";

    [SerializeField] PlayerMovement playerMovement;
    
    [SerializeField] float speedMultiplier = .15f;


    private void Awake()
    {
        health = GetComponent<PlayerHealth>();

        playerMovement = GetComponent<PlayerMovement>();
    }

    public override void Activate()
    {
        playerMovement.SetSpeedMultiplier(-speedMultiplier);
        ErrorSays_QuebraParede();
        //movement.SetSpeed("giant");
        wallCollider.enabled = true;
        StopAllCoroutines();
        health.invencible = true;
        StartCoroutine(ScaleTo(GiantSize));
        movement.groundCheckRadius = giant_groundCheckRadius;
        _isActive = true;
    }

    public override void Deactivate()
    {
        //movement.SetSpeed("defeault");
        wallCollider.enabled = false;
        health.invencible = false;
        StopAllCoroutines();
        StartCoroutine(ScaleTo(OriginalScale));
        _isActive = false;
        movement.groundCheckRadius = original_GroundCheckRadius;
        playerMovement.SetSpeedMultiplier(+speedMultiplier);
    }

    public override float GetManaCost()
    {
        return manaCostPerSecond; // Custo de mana por segundo
    }

    public override bool isActive()
    {
        return _isActive;
    }

    private IEnumerator ScaleTo(Vector3 size){
        Vector3 originalSize = transform.localScale;
        float elapsedTime = 0f;

        while(elapsedTime < scaleLerpDuration){
            transform.localScale = Vector3.Lerp(originalSize, size, elapsedTime / scaleLerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = size;
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if(!_isActive){
            return;
        }

        if(collision.gameObject.tag == "Enemy"){
            PostProcessVolumeManager.Instance.TriggerFlashEffect(3, .5f);
            ObjectPoolManager.Instance.SpawnFromPool(PlayerSlashEffect, collision.transform.position, Quaternion.identity);

            EnemyLife enemyLife = collision.gameObject.GetComponent<EnemyLife>();
            if (enemyLife != null)
            {
                enemyLife.TakeDamage(20, true);
            }

            return;
        }

        if(!isInLayerMask(collision.gameObject.layer, wallLayer)){
            return;
        }

        Tilemap tilemap = collision.collider.GetComponent<Tilemap>();

        if(tilemap == null){
            return;
        }

        foreach(ContactPoint2D contact in collision.contacts){
            Vector3Int tilePos = tilemap.WorldToCell(contact.point);

            Color tileColor = tilemap.color;

            ObjectPoolManager.Instance.SpawnFromPool(HitEffectName, tilePos, Quaternion.identity).GetComponent<BreakWallEffect_ColorManager>().SetColor(tileColor);
            impulseSource.GenerateImpulse();

            tilemap.SetTile(tilePos, null);
        }
    }

    private bool isInLayerMask(int layer, LayerMask layerMask){
        return(layerMask.value & (1 << layer)) != 0;
    }

    public void ErrorSays_QuebraParede(){
        if(errorSays == null){
            errorSays = FindObjectOfType<ErrorSaysScript>();
        }
        errorSays.ErrorSaysCall(errorJoke);
    }
}
