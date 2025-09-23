using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostAbility : Ability
{
    private bool _isActive;

    [SerializeField] private Color GhostAbilityColor;

    [SerializeField] PlayerMaterialManager materialSwitcher;

    ErrorSaysScript errorSays;

    [SerializeField] string errorJoke = "Booo, eu sou um fantasma!";

    [SerializeField] float manaCostPerSecond = 5f;

    public override void Activate()
    {
        ErrorSays_Joke();
        materialSwitcher.SetMaterial(2);

        // Desativa a colisão
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Wall"), true);
        _isActive = true;
    }

    public override void Deactivate()
    {
        materialSwitcher.SetMaterial(0);
        //GetComponent<PlayerCombat>().isIntangible = false;

        // Ativa a colisão
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Wall"), false);
        
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

    public void ErrorSays_Joke(){
        if(errorSays == null){
            errorSays = FindObjectOfType<ErrorSaysScript>();
        }
        errorSays.ErrorSaysCall(errorJoke);
    }
}
