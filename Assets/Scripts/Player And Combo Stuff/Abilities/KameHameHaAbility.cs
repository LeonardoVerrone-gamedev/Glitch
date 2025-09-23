using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class KameHameHaAbility : Ability
{
    private bool _isActive;

    [SerializeField] GameObject KameHameHa_projectile_prefab;

    [SerializeField] PlayerInputs inputs;
    [SerializeField] Transform gunPos;
    [SerializeField] float KameHameHa_Duration = 3f;


    ErrorSaysScript errorSays;
    [SerializeField] string errorJoke = "KAME-HAME-HÁAAAAA!!!";

    [SerializeField] float manaCostPerSecond = 15;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] AbilityManager abilityManager;

    public override void Activate()
    {
        _isActive = true;
        ErrorSays_Joke();
        StartCoroutine(KameHameHa());
    }

    void Awake(){
        abilityManager = GetComponent<AbilityManager>();
    }

    public override void Deactivate()
    {
        _isActive = false;
        abilityManager.CheckActiveAbilities();
    }

    public override float GetManaCost()
    {
        return manaCostPerSecond; // Custo de mana por segundo
    }

    public override bool isActive()
    {
        return _isActive;
    }

    public IEnumerator KameHameHa()
    {
        inputs.canMove = false;

        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        GameObject projetil = ObjectPoolManager.Instance.SpawnFromPool("Kamehameha", gunPos.position, transform.rotation);
        yield return new WaitForSeconds(KameHameHa_Duration);
        ObjectPoolManager.Instance.ReturnToPool(projetil, "Kamehameha");
        inputs.canMove = true;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        rb.AddForce(Vector2.down, ForceMode2D.Force);

        Deactivate();
    }

    public void ErrorSays_Joke(){
        if(errorSays == null){
            errorSays = FindObjectOfType<ErrorSaysScript>();
        }
        errorSays.ErrorSaysCall(errorJoke);
    }
}
