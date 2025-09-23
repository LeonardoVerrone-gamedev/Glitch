using UnityEngine;
using System.Collections;

public class HealAbility : Ability
{
    private bool _isActive;

    [SerializeField] float amount;

    [SerializeField] PlayerHealth health;

    [SerializeField] ManaManager mana;

    [SerializeField] ScratchAndStretch scratchAndStretch;

    [SerializeField] ParticleSystem healParticles;

    ErrorSaysScript errorSays;
    [SerializeField] string errorJoke = "Até que enfim ta me curando. Do jeito que você joga mal, deveria usar essa mais vezes!";

    [SerializeField] float manaCostPerSecond = 50f;
    [SerializeField] AbilityManager abilityManager;

    public override void Activate()
    {
        _isActive = true;
        mana.UseMana(manaCostPerSecond);
        ErrorSays_Joke();
        Heal();
    }

    public override void Deactivate()
    {
        health.invencible = false;
        _isActive = false;
        abilityManager.CheckActiveAbilities();
    }

    public override float GetManaCost()
    {
        return 0f; // Custo de mana por segundo
    }

    public override bool isActive()
    {
        return _isActive;
    }

    public void Heal()
    {
        health.invencible = true;
        health.GainHealth(amount);
        scratchAndStretch.PlayStretchAnimation("Heal");
        healParticles.Play();
        Invoke("Deactivate", 1f);
    }

    public void ErrorSays_Joke()
    {
        if (errorSays == null)
        {
            errorSays = FindObjectOfType<ErrorSaysScript>();
        }
        errorSays.ErrorSaysCall(errorJoke);
    }

    void Awake()
    {
        mana = GetComponent<ManaManager>();
        abilityManager = GetComponent<AbilityManager>();
    }
}
