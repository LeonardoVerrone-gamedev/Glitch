using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using Cinemachine;
using NUnit.Framework;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public ParticleSystem SmokeEffect;

    [SerializeField] Transform gunTransformPosition;
    [SerializeField] Transform[] shootgunTransformPositions;

    public bool flamethrower;

    [SerializeField] ParticleSystem flamethrowerParticleSystem;
    [SerializeField] ParticleSystem[] shotgun_flamethrowerParticleSystem;

    bool firingFlameThrower;

    public CinemachineImpulseSource impulseSource; // Referência ao Impulse Source
    [SerializeField] private ScratchAndStretch scratchAndStretch;

    [SerializeField] private GameObject currentBullet;

    [Header("Combo stuff")]
    private AbilityManager abilityManager;

    public int CurrentComboPriorty = 0;

    public Move fastMoveRight;
    public Move fastMoveLeft;

    [SerializeField] bool canShoot = true;

    private ManaManager manaManager;

    public Transform groundCheckPoint;

    private PlayerMovement playerMovement;
    private PlayerAnimation playerAnimation;

    [SerializeField] float speedMultiplier = .1f;

    public bool infiniteShoot;

    public bool shootgun;

    // --- MODIFICATION START ---
    private bool _isShooting;

    public bool IsShooting
    {
        get { return _isShooting; }
        set
        {
            if (_isShooting != value)
            {
                _isShooting = value;
                OnIsShootingChanged(); // This function is now called on change
            }
        }
    }

    private void OnIsShootingChanged()
    {
        if (_isShooting)
        {
            playerMovement.SetSpeedMultiplier(-speedMultiplier);
        }
        else
        {
            playerMovement.SetSpeedMultiplier(+speedMultiplier);
        }
    }

    [SerializeField] float shootTimer = .5f;
    [SerializeField] float shootTime = 0f;


    void Start()
    {
        //animator = GetComponent<Animator>();
        abilityManager = GetComponent<AbilityManager>();
        manaManager = GetComponent<ManaManager>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    public void FastAbilityRight()
    {
        PlayMove(fastMoveRight.GetMove(), 10);
    }

    public void FastAbilityLeft()
    {
        PlayMove(fastMoveLeft.GetMove(), 10);
    }

    public void StartFlamethrower(bool value)
    {
        if (value == true)
        {
            flamethrower = true;
        }
        else
        {
            flamethrower = false;
            flamethrowerParticleSystem.loop = false;

            if (shootgun)
            {
                foreach (ParticleSystem flame in shotgun_flamethrowerParticleSystem)
                {
                    flame.loop = false;
                }
            }
        }
    }

    public void SimpleShoot()
    {
        if (flamethrower)
        {
            flamethrowerParticleSystem.loop = true;
            flamethrowerParticleSystem.Play();

            if (shootgun)
            {
                foreach (ParticleSystem flame in shotgun_flamethrowerParticleSystem)
                {
                    flame.loop = true;
                    flame.Play();
                }
            }
            return;
        }

        if (infiniteShoot || canShoot && shootTime <= 0f)//((currentBullet == null || !currentBullet.activeInHierarchy)))
        {
            SmokeEffect.Play();
            scratchAndStretch.PlayStretchAnimation("Shoot");
            impulseSource.GenerateImpulse();
            currentBullet = ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", gunTransformPosition.position, gunTransformPosition.rotation);

            if (shootgun)
            {
                foreach (Transform pos in shootgunTransformPositions)
                {
                    ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", pos.position, pos.rotation);
                }
            }

            shootTime = shootTimer;
        }
    }

    void Update()
    {
        // Use the new property here
        if (IsShooting)
        {
            if (infiniteShoot)
            {
                IsShooting = false;
            }
            SimpleShoot();
        }

        else if (!IsShooting && flamethrower)
        {
            flamethrowerParticleSystem.loop = false;

            if (shootgun)
            {
                foreach (ParticleSystem flame in shotgun_flamethrowerParticleSystem)
                {
                    flame.loop = false;
                }
            }
        
        }

        if (shootTime > 0f)
        {
            shootTime -= Time.unscaledDeltaTime;
        }
    }


    #region Special Attacks Area

    public void PlayMove(Moves move, int ComboPriorty)
    {
        if (Moves.None != move)
        {
            if (ComboPriorty >= CurrentComboPriorty)
            {
                CurrentComboPriorty = ComboPriorty;

                switch (move)
                {
                    // Adicione mais ataques especiais aqui
                    case Moves.NullAbility:
                        abilityManager.PlayAbility(0);
                        break;
                    case Moves.GhostCode:
                        abilityManager.PlayAbility(1);
                        //isIntangible = true;
                        break;
                    case Moves.RunCode:
                        abilityManager.PlayAbility(2);
                        break;
                    case Moves.FlyCode:
                        abilityManager.PlayAbility(3);
                        break;
                    case Moves.InfiniteShoot:
                        abilityManager.PlayAbility(4);
                        break;
                    case Moves.KameHameHa:
                        abilityManager.PlayAbility(5);
                        break;
                    case Moves.Heal:
                        abilityManager.PlayAbility(6);
                        break;
                    case Moves.Star:
                        abilityManager.PlayAbility(7);
                        break;
                    case Moves.Shrinking:
                        abilityManager.PlayAbility(8);
                        break;
                    case Moves.Giant:
                        abilityManager.PlayAbility(9);
                        break;
                    case Moves.Shootgun:
                        abilityManager.PlayAbility(10);
                        break;
                    case Moves.Flamethrower:
                        abilityManager.PlayAbility(11);
                        break;
                }
            }
            else
                return;

            CurrentComboPriorty = 0;

        }
    }
    #endregion
}