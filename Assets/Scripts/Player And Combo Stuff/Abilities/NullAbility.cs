using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullAbility : Ability
{
    private bool _isActive;
    [SerializeField] float manaCost = 3f;
    [SerializeField] PlayerMovement playerMovement;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public override void Activate()
    {
        playerMovement.SetSpeedMultiplier(0f);
        // Não faz nada
        _isActive = true;
    }

    public override void Deactivate()
    {
        // Não faz nada
        _isActive = false;
    }

    public override float GetManaCost()
    {
        return manaCost; // Custo de mana por segundo
    }

    public override bool isActive()
    {
        return _isActive;
    }
}
