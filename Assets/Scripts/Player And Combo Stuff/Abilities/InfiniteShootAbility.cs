using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteShootAbility : Ability
{
    private bool _isActive;

    [SerializeField] SpriteRenderer spriteRenderer;
    PlayerCombat playerCombat;

    ErrorSaysScript errorSays;

    [SerializeField] string errorJoke = "FOGO!!";

    [SerializeField] float manaCostPerSecond = 25f;

    [SerializeField] PlayerMovement playerMovement;

    [SerializeField] float speedMultiplier = .1f;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat= GetComponent<PlayerCombat>();
    }
    
    public override void Activate()
    {
        playerMovement.SetSpeedMultiplier(-speedMultiplier);
        ErrorSays_Joke();
        _isActive = true;
        playerCombat.infiniteShoot = true;
        StartCoroutine(InfiniteShoot());
    }

    public override void Deactivate()
    {
        playerMovement.SetSpeedMultiplier(+speedMultiplier);
        playerCombat.infiniteShoot = false;
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

    public IEnumerator InfiniteShoot()
    {
        float duration = 1f; // Duração de um ciclo completo
        float time = 0f; // Tempo para o efeito de arco-íris

        while(_isActive)
        {

            time += Time.deltaTime; // Aumenta o tempo com base no tempo real
            float t = time / duration; // Normaliza o tempo

            if (t > 1){
                time = 0f;
                t = 0f; // Reseta o tempo após completar um ciclo
            }

            Color color = Color.HSVToRGB(t, 1, 1);
            spriteRenderer.color = color; // Aplica a cor ao SpriteRenderer

            // Executa a lógica de tiro
            playerCombat.SimpleShoot();

            // Espera 0,10 segundos antes de continuar
            yield return new WaitForSecondsRealtime(0.10f);
        }
    }

    public void ErrorSays_Joke(){
        if(errorSays == null){
            errorSays = FindObjectOfType<ErrorSaysScript>();
        }
        errorSays.ErrorSaysCall(errorJoke);
    }
}
