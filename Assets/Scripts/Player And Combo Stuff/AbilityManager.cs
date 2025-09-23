using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializeField]private List<Ability> abilities = new List<Ability>();
    private Ability currentAbility;
    [SerializeField]private List<Ability> currentAbilities = new List<Ability>();
    private int currentAbilityIndex = 0;

    public float manaCost;

    private ManaManager manaManager; // Referência ao gerenciador de mana

    ErrorSaysScript errorSays;

    [SerializeField] AudioSource AbilityAudioSource;
    [SerializeField] AudioClip noManaClip;
    [SerializeField] AudioClip UseAbilityClip;
    [SerializeField] AudioClip deactiveAbilityClip;

    void Start()
    {
        manaManager = GetComponent<ManaManager>(); // Obtém a referência ao gerenciador de mana

        // Adicione a habilidade nula primeiro
        abilities.Add(GetComponent<NullAbility>()); // Adicione a habilidade nula (INDEX0)
        abilities.Add(GetComponent<GhostAbility>()); // Adicione a habilidade fantasma (INDEX1)
        abilities.Add(GetComponent<RunAbility>()); // Adiciona a habilidade de corrida (INDEX2)
        abilities.Add(GetComponent<FlyAbility>()); // Adiciona a habilidade de voar (INDEX3)
        abilities.Add(GetComponent<InfiniteShootAbility>()); // Adiciona a habilidade de machingeun (INDEX4)
        abilities.Add(GetComponent<KameHameHaAbility>()); //Adiciona a habilidade KameHameHa (INDEX5)
        abilities.Add(GetComponent<HealAbility>()); // adiciona a habilidade de cura (INDEX6)
        abilities.Add(GetComponent<StarAbility>()); //habilidadte estrela (INDEX7)
        abilities.Add(GetComponent<ShrinkingAbility>()); // habilidade de encolher (INDEX 8)
        abilities.Add(GetComponent<GiantAbility>()); // habilidade de Aumentar (INDEX 9)
        abilities.Add(GetComponent<ShootgunAbility>()); // habilidade de Shootgun (INDEX 10)
        abilities.Add(GetComponent<FlamethrowerAbility>()); // habilidade de flamethrower (INDEX 11)

        // Ative a primeira habilidade (nula)
        PlayAbility(0);
    }

    public void PlayAbility(int abilityIndex)
    {
        if (!CanActivateAbility(abilityIndex))
        {
            if (errorSays == null)
            {
                errorSays = FindObjectOfType<ErrorSaysScript>();
            }
            errorSays.ErrorSaysCall("Seria meio imbecil fazer isso agora...");

            AbilityAudioSource.clip = noManaClip;
            AbilityAudioSource.Play();

            return; // Impede ativação se houver conflito
        }

        if (abilityIndex == 0)
        {
            if(manaManager.currentMana > 0){
                AbilityAudioSource.clip = deactiveAbilityClip;
            }else{
                AbilityAudioSource.clip = noManaClip;
            }
            AbilityAudioSource.Play();
            
            SwitchToNullAbility();
            return;
        }
        if (currentAbilities.Contains(abilities[abilityIndex]))
        {
            AbilityAudioSource.clip = deactiveAbilityClip;
            AbilityAudioSource.Play();

            ObjectPoolManager.Instance.SpawnFromPool("Shockwave", transform.position, Quaternion.identity);
            abilities[abilityIndex].Deactivate();
            currentAbilities.Remove(abilities[abilityIndex]);

            ResetMana();

            if (currentAbilities.Count == 0)
            {
                PlayAbility(0); // 0 é o índice da habilidade nula
            }

            return;
        }

        if (manaManager.currentMana <= 0) //(abilities[abilityIndex].GetManaCost() >= manaManager.currentMana)
        {
            AbilityAudioSource.clip = noManaClip;
            AbilityAudioSource.Play();
            return;
        }

        if (currentAbilities.Contains(abilities[0]))
        {
            abilities[0].Deactivate();
            currentAbilities.Remove(abilities[0]);
        }

        AbilityAudioSource.clip = UseAbilityClip;
        AbilityAudioSource.Play();

        currentAbilities.Add(abilities[abilityIndex]);
        abilities[abilityIndex].Activate();
        ResetMana();

        ObjectPoolManager.Instance.SpawnFromPool("Shockwave", transform.position, Quaternion.identity);
    }

    private void Update()
    {
        // Verifica se a mana é suficiente para a habilidade atual

        if (manaCost > 0)
        {
            // Custo de mana positivo
            manaManager.UseMana(manaCost * Time.deltaTime);
        }
        else
        {
            // Custo de mana negativo (recarga de mana)
            manaManager.GainMana(-manaCost * Time.deltaTime);
        }

    }

    public void SwitchToNullAbility()
    {
        List<Ability> abilitiesToRemove = new List<Ability>();

        foreach (Ability activatedAbility in currentAbilities)
        {
            activatedAbility.Deactivate();
            abilitiesToRemove.Add(activatedAbility);
        }

        foreach (Ability ability in abilitiesToRemove)
        {
            currentAbilities.Remove(ability);
        }

       // CheckActiveAbilities();

        manaCost = 0f;
        currentAbilityIndex = 0; // Define a habilidade nula
        abilities[0].Activate();
        manaCost = abilities[0].GetManaCost();
    }

    public void CheckActiveAbilities()
    {

        List<Ability> abilitiesToRemove = new List<Ability>();

        foreach (Ability ability in currentAbilities)
        {
            if (ability.isActive() == false)
            {
                abilitiesToRemove.Add(ability);
            }
        }

        foreach (Ability ability in abilitiesToRemove)
        {
            currentAbilities.Remove(ability);
        }

        ResetMana();

        if (currentAbilities.Count == 0)
        {
            PlayAbility(0); // 0 é o índice da habilidade nula
        }
    }

    void ResetMana()
    {
        manaCost = 0f;
        foreach (Ability ability in currentAbilities)
        {
            manaCost += ability.GetManaCost();
        }
    }

    // Retorna true se a habilidade pode ser ativada
    private bool CanActivateAbility(int abilityIndex) 
    {
        // Dicionário que mapeia cada habilidade para as que não podem ser sobrepostas
        Dictionary<int, int[]> blockingAbilities = new Dictionary<int, int[]>
        {
            {2, new int[] { 7, 5 } }, // Dash (2) não pode ser ativado durante Star (7), nem kamehameha(5)
            {7, new int[] { 2 } },  // Star (7) não pode ser ativado durante Dash (2)
            {5, new int[] { 4, 2, 11 } }, // kamehameha (5) nao pode ser ativado junto com tiro infinito nem dash nem flame
            {4, new int[] { 5} }, // tiro infinito (4) com kamehameha (5)
            {9, new int[] { 8 } }, // giant(9) nao pode com small(8)
            {8, new int[] {9}}, // small(8) nao pode com giant (9)
            {11, new int[]{5}} //flamethrower e kamehameha
            
            // Adicione mais relações conforme necessário
        };

        // Verifica se essa habilidade tem restrições
        if (blockingAbilities.TryGetValue(abilityIndex, out int[] blockedAbilities))
        {
            // Verifica se alguma das habilidades bloqueadas está ativa
            foreach (int blockedIndex in blockedAbilities)
            {
                if (currentAbilities.Contains(abilities[blockedIndex]))
                {
                    return false;
                }
            }
        }

        return true;
    }

}