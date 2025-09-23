using UnityEngine;

public class FlamethrowerAbility : Ability
{

    private bool _isActive;

    ErrorSaysScript errorSays;

    PlayerCombat playerCombat;

    [SerializeField] string errorJoke = "Eu adoro um churrasco!";

    [SerializeField] float manaCostPerSecond = 2f;

    public override void Activate()
    {
        _isActive = true;
        playerCombat.StartFlamethrower(true);
        ErrorSays_Joke();
    }

    public override void Deactivate()
    {
        playerCombat.StartFlamethrower(false);
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
        playerCombat = GetComponent<PlayerCombat>();
    }
}
