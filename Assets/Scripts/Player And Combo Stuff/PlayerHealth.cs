using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float currentLife;
    [SerializeField] private float MaxLife;

    [SerializeField] ScratchAndStretch scratchAndStretch;
    [SerializeField] PlayerAnimation anim;
    [SerializeField] PlayerInputs playerInputs;
    [SerializeField] PlayerCombat playerCombat;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Animator animator;
    
    [SerializeField] float pushBackForce;

    [SerializeField] Rigidbody2D rb;

    [SerializeField] public bool invencible;

    [SerializeField] CinemachineImpulseSource impulseSource;

    [SerializeField] private Image healthImage;

    [SerializeField] Animator ErrorIconAnim;

    [SerializeField] Color damageColor;

    [SerializeField] Color normalColor;

    [SerializeField] Color LowLifeColor;

    [SerializeField] AudioSource healthAudioSource;
    [SerializeField] AudioClip hurtClip;

    ErrorSaysScript errorSays;

    bool inLowLife;

    void OnEnable()
    {
        playerInputs = GetComponent<PlayerInputs>();

        GameObject healthBarContent = GameObject.Find("HealthBarContent");
        healthImage = healthBarContent.GetComponent<Image>();

        GameObject ErrorIconOBJ = GameObject.Find("ErrorIcon");
        ErrorIconAnim = ErrorIconOBJ.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (healthImage != null)
        {
            healthImage.fillAmount = currentLife / MaxLife;
        }
    }

    public bool TakeDamage(int damage, string ouchText){
        if(!invencible){
            if(errorSays == null){
                errorSays = FindObjectOfType<ErrorSaysScript>();
            }
            if (ouchText == "")
            {
                errorSays.ErrorSaysCall("OUCH! Joga direito p#%&* !");
            }
            else
            {
                errorSays.ErrorSaysCall(ouchText);
            }

            TimeScaleManager.Instance.SetTimeScale(0f, 0.05f);
            PostProcessVolumeManager.Instance.TriggerFlashEffect(2, 1f);
            StartCoroutine(_TakeDamage(damage));
            return true;
        }
        return false;
    }

    public void GainHealth(float amount)
    {
        currentLife += amount;
        if (currentLife > MaxLife)
        {
            currentLife = MaxLife; // Limita a vida ao máximo
        }

        if (currentLife > (MaxLife / 4f) && inLowLife)
        {
            inLowLife = false;
            healthImage.color = normalColor;
            PostProcessVolumeManager.Instance.ActivateLowLifeVolume(false);
        }
    }

    private IEnumerator _TakeDamage(int damage)
    {
        Debug.Log("Entrou em take demage");
        invencible = true;
        healthAudioSource.clip = hurtClip;
        healthAudioSource.Play();
        healthImage.color = damageColor;
        currentLife -= damage;
        playerCombat.IsShooting = false;
        playerInputs.canMove = false;
        anim.PlayAnimationByTrigger("Damage");
        ErrorIconAnim.SetTrigger("Hurt");
        rb.linearVelocity = new Vector2(0f, 10f);
        impulseSource.GenerateImpulse();
        scratchAndStretch.PlayStretchAnimation("Damage");

        //yield return new WaitUntil(() => IsAnimationFinished());
        yield return new WaitForSeconds(1f);

        playerInputs.canMove = true;

        yield return new WaitForSeconds(.25f);

        invencible = false;
        if (currentLife <= (MaxLife / 4f) && !inLowLife)
        {
            inLowLife = true;
            healthImage.color = LowLifeColor;
            PostProcessVolumeManager.Instance.ActivateLowLifeVolume(true);
        }
        else
        {
           healthImage.color = normalColor; 
        }
    }

    private bool IsAnimationFinished()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime >= 1f;
    }
}
