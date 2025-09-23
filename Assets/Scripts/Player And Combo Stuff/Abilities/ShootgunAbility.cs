using UnityEngine;

public class ShootgunAbility : Ability
{
    private bool _isActive;

    PlayerCombat playerCombat;

    ErrorSaysScript errorSays;

    [SerializeField] string errorJoke = "Um é pouco, dois é bom, mas três é melhor!!";

    [SerializeField] float manaCostPerSecond = 15f;

    void Awake()
    {
        playerCombat= GetComponent<PlayerCombat>();
    }
    
    public override void Activate()
    {
        ErrorSays_Joke();
        _isActive = true;
        playerCombat.shootgun = true;
    }

    public override void Deactivate()
    {
        playerCombat.shootgun = false;
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
