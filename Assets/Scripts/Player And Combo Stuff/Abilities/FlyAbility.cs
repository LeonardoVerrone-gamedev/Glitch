using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAbility : Ability
{
    [SerializeField] private GameObject asas;
    [SerializeField] private PlayerMovement movement;
    private bool _isActive;

    ErrorSaysScript errorSays;

    [SerializeField] string errorJoke = "Me senti o Neo em Matrix quando aprendi a fazer isso!";

    [SerializeField] float manaCostPerSecond = 2f;


    public override void Activate()
    {
        asas.SetActive(true);
        movement.infinityJump = true;
        CameraManagerScript.Instance.SwitchCamera(1);
        _isActive = true;
        ErrorSays_Joke();
    }

    public override void Deactivate()
    {
        asas.SetActive(false);
        movement.infinityJump = false;
        CameraManagerScript.Instance.SwitchCamera(0);
        _isActive = false;
    }

    public override float GetManaCost()
    {
        return manaCostPerSecond; // Custo de mana por segundo
    }

    public override bool isActive()
    {
        return _isActive;
    }

    void Start(){
        movement = GetComponent<PlayerMovement>();
    }

    public void ErrorSays_Joke(){
        if(errorSays == null){
            errorSays = FindObjectOfType<ErrorSaysScript>();
        }
        errorSays.ErrorSaysCall(errorJoke);
    }
}
