using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;
    public FastSelectionAbilityMenu fastSelectionMenu;

    public InputAction playerControls;

    public bool canMove;

    [SerializeField]private float horizontal;
    public float lastHorizontal = 0f;

    [SerializeField]public float vertical;

    [SerializeField]private Vector2 cameraVector;
    [SerializeField]private CameraFollowObject cameraFollowObject;

    [SerializeField] RunAbility dashControl;
    [SerializeField] Vector2 dashDirection;

    private float switchAbilityValor;

    [SerializeField]private float buttonPressTime;
    private const float longPressThreshold = 0.5f; // Tempo para considerar um pressionamento longo
    private const float doublePressThreshold = 0.3f;// Tempo para considerar um pressionamento curto
    
    public bool isJumping;


    void Start()
    {
        fastSelectionMenu = FindObjectOfType<FastSelectionAbilityMenu>();
        cameraFollowObject = FindObjectOfType<CameraFollowObject>();
    }

    public void CanMove_SetTrue(){
        Debug.Log("can move setado para true");
        canMove = true;
    }
    public void CanMove_SetFalse(){
        Debug.Log("can move setado para falso");
        canMove = false;
    }

    void Update()
    {
        if(canMove){
            playerMovement.horizontalValue = horizontal;
            playerMovement.verticalValue = vertical;
            //if(horizontal == 0f && playerCombat.isRunning){
               // GetComponent<RunAbility>().Deactivate();
                //playerCombat.isRunning = false;
            //}
        }

        if (dashControl.isActive())
        {
            dashControl.dashDirection = new Vector2(horizontal, vertical).normalized;
        }

        cameraFollowObject.cameraManualMoveVector = new Vector3(cameraVector.x, cameraVector.y, 0f);

        if(fastSelectionMenu.inMenu){
            canMove = false;
        }
    }

    void FixedUpdate(){
        playerMovement.Move(horizontal);
    }

    public void Move(InputAction.CallbackContext context){
        horizontal = Mathf.Round(context.ReadValue<Vector2>().x);
        vertical = Mathf.Round(context.ReadValue<Vector2>().y);

        if(context.performed){
            if(fastSelectionMenu.inMenu){
                if(horizontal > 0){
                    fastSelectionMenu.Scroll("right");
                }

                else if(horizontal < 0){
                    fastSelectionMenu.Scroll("left");
                }
            }
        }
    }

    public void CameraMove(InputAction.CallbackContext context){
        cameraVector = new Vector2(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
    }

    public void OnUseFastAbilityLB(InputAction.CallbackContext context) {
        Debug.Log("OnUseFastAbilityLB");
        if(context.performed){
            playerCombat.FastAbilityLeft(); // Executa a habilidade rápida ao pressionar
        }
    }

    public void OnUseFastAbilityRB(InputAction.CallbackContext context) {
        Debug.Log("OnUseFastAbilityRB");
        if (!canMove) {
            return;
        }

        if(context.performed){
            playerCombat.FastAbilityRight(); // Executa a habilidade rápida ao pressionar
        }
    }

    public void OnOpenFastAbilityMenuRB(InputAction.CallbackContext context) {
        if (context.performed) {
            Debug.Log("OnOpenFastAbilityMenuRB");
            if (!fastSelectionMenu.inMenu) {
                fastSelectionMenu.RB = true; // Define que é o botão direito
                fastSelectionMenu.OpenMenu(); // Abre o menu ao pressionar
            }
        }

        if (context.canceled) {
            Debug.Log("Closing Fast Ability Menu RB");
            if (fastSelectionMenu.inMenu) {
                fastSelectionMenu.SetFastAbility(); // Faz a seleção ao soltar
                canMove = true; // Permite que o jogador se mova novamente
            }
        }
    }

    public void OnOpenFastAbilityMenuLB(InputAction.CallbackContext context) {
        if (context.performed) {
            Debug.Log("OnOpenFastAbilityMenuLB");
            if (!fastSelectionMenu.inMenu) {
                fastSelectionMenu.RB = false; // Define que é o botão esquerdo
                fastSelectionMenu.OpenMenu(); // Abre o menu ao pressionar
            }
        }

        if (context.canceled) {
            Debug.Log("Closing Fast Ability Menu LB");
            if (fastSelectionMenu.inMenu) {
                fastSelectionMenu.SetFastAbility(); // Faz a seleção ao soltar
                canMove = true; // Permite que o jogador se mova novamente
            }
        }
    }
        //public void Fire(InputAction.CallbackContext context){
            //if(context.performed && canMove){
                //playerCombat.HandleShooting(true);
                //controlManager.KeysPressed.Add("RightTrigger");
                //controlManager.ResetCheck();
            //}
            //if(context.canceled){
                //playerCombat.HandleShooting(false);
            //}
        //}

    public void A_Button(InputAction.CallbackContext context){
        if(context.canceled){
            isJumping = false;
        }
        if(!canMove){
            return;
        }
        if(context.performed){
            isJumping = true;
            playerMovement.Jump();
            ControlManager.Instance.KeysPressed.Add("A");
            ControlManager.Instance.ResetCheck();
        }
    }

    public void B_Button(InputAction.CallbackContext context){
        if(fastSelectionMenu.inMenu){
            fastSelectionMenu.Cancel();
            canMove = true;
        }
        if(!canMove){
            return;
        }
        if(context.performed){
            ControlManager.Instance.KeysPressed.Add("B");
            ControlManager.Instance.ResetCheck();
        }
    }

    public void Tap_X_Button(InputAction.CallbackContext context)
    {
        if (!canMove)
        {
            return;
        }
        if (context.performed)
        {
            ControlManager.Instance.KeysPressed.Add("X");
            ControlManager.Instance.ResetCheck();
        }

        if (context.started)
        {
            playerCombat.SimpleShoot();
            playerCombat.IsShooting = true;
        }
        if (context.canceled)
        {
            playerCombat.IsShooting = false;
        }

    }
    public void Y_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            ControlManager.Instance.KeysPressed.Add("Y");
            ControlManager.Instance.ResetCheck();
        }
    }

    public void Up_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            ControlManager.Instance.KeysPressed.Add("Up");
            ControlManager.Instance.ResetCheck();
        }
    }

    public void Down_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            ControlManager.Instance.KeysPressed.Add("Down");
            ControlManager.Instance.ResetCheck();
        }
    }

    public void Left_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            ControlManager.Instance.KeysPressed.Add("Left");
            ControlManager.Instance.ResetCheck();
        }
    }

    public void Right_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            ControlManager.Instance.KeysPressed.Add("Right");
            ControlManager.Instance.ResetCheck();
        }
    }

    public void ResetControlManagerCheck(InputAction.CallbackContext context){
        if(context.performed){
            ControlManager.Instance.ResetCheck();
        }
    }
}
