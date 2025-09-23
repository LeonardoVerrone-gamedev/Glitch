using UnityEngine;
using System.Collections;

public class ShrinkingAbility : Ability
{
    [SerializeField] Vector3 SmallScale;
    [SerializeField] Vector3 OriginalScale =Vector3.one;

    [SerializeField] float scaleLerpDuration;

    [SerializeField] float manaCostPerSecond = 5f;
    bool _isActive;

    ErrorSaysScript errorSays;
    [SerializeField] string errorJoke = "E sem precisar de chapéu verde falante!";

    [SerializeField] PlayerMovement playerMovement;

    [SerializeField] float speedMultiplier = .5f;

    void Awake()
    {
        playerMovement =  GetComponent<PlayerMovement>();
    }

    public override void Activate()
    {
        ErrorSays_Joke();
        StopAllCoroutines();
        StartCoroutine(ScaleTo(SmallScale));
        playerMovement.SetSpeedMultiplier(+speedMultiplier);
        _isActive = true;
    }

    public override void Deactivate()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(OriginalScale));
        playerMovement.SetSpeedMultiplier(-speedMultiplier);
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

    public void ErrorSays_Joke(){
        if(errorSays == null){
            errorSays = FindObjectOfType<ErrorSaysScript>();
        }
        errorSays.ErrorSaysCall(errorJoke);
    }
}
