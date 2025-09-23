using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaManager : MonoBehaviour
{
    public float currentMana = 0f;
    public float maxMana = 100f;
    public float manaGainRate = 1f; // Mana ganha por segundo
    [SerializeField] private Image manaImage; // Referência à imagem de mana

    [SerializeField] private float cooldownDuration = 10f; // Duração do cooldown
    [SerializeField] private bool isCooldownActive = false; // Indica se o cooldown está ativo

    [SerializeField] Animator ErrorIconAnim;

    [SerializeField] PlayerMaterialManager materialSwitcher;

    [SerializeField] ErrorSaysScript errorSays;

    [SerializeField] AbilityManager abilityManager;

    void OnEnable()
    {
        GameObject manaBarContent = GameObject.Find("ManaBarContent");
        manaImage = manaBarContent.GetComponent<Image>();

        GameObject ErrorIconOBJ = GameObject.Find("ErrorIcon");
        ErrorIconAnim = ErrorIconOBJ.GetComponent<Animator>();

        // Inicializa a imagem de mana
        if (manaImage != null)
        {
            manaImage.fillAmount = currentMana / maxMana;
        }
    }

    void Update()
    {
        // Atualiza a imagem de mana
        if (manaImage != null)
        {
            manaImage.fillAmount = currentMana / maxMana;
        }
    }

    public void GainMana(float amount)
    {
        // Retorna se o cooldown estiver ativo
        if (isCooldownActive)
        {
            return;
        }

        currentMana += amount;
        if (currentMana > maxMana)
        {
            currentMana = maxMana; // Limita a mana ao máximo
        }
    }

    public void UseMana(float amount)
    {
        currentMana -= amount;
        if (currentMana <= 0)
        {
            abilityManager.PlayAbility(0);

            currentMana = 0; // Limita a mana ao mínimo
            StartCooldown(); // Inicia o cooldown quando a mana é zerada
        }
    }

    private void StartCooldown()
    {
        isCooldownActive = true;

        ErrorIconAnim.SetTrigger("Ops");

        if (errorSays == null)
        {
            errorSays = FindObjectOfType<ErrorSaysScript>();
        }
        errorSays.ErrorSaysCall("Você esgotou minha mana! Aquieta aí um pouco...");
        StartCoroutine(coolDownCoroutine());
    }

    private IEnumerator coolDownCoroutine()
    {
        yield return new WaitForSeconds(0.25f); //delay
        materialSwitcher.SetMaterial(1);
        yield return new WaitForSeconds(cooldownDuration);
        materialSwitcher.SetMaterial(0);
        isCooldownActive = false;
    }

    void Awake()
    {
        abilityManager = GetComponent<AbilityManager>();
    }
}
