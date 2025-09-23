using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{
    public static ControlManager Instance { get; private set;}
    [SerializeField] float ComboResetTime = 0.5f; //The Time to reset the Combo Time
    [SerializeField] public List<string> KeysPressed; //List of all the Keys Pressed so far

    public GameObject player;

    public string allKeysPressed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ResetCheck(){
        if (!MovesManager.Instance.CanMove(KeysPressed)) //if there is no avilable Moves reset the list
            StopAllCoroutines();

        StartCoroutine(ResetComboTimer()); //Start the Reseting process
    }

    public void ResetCombo() //Called to Reset the Combo after a move
    {
        KeysPressed.Clear();
        Debug.Log("Cleared");
        allKeysPressed = null;
    }

    IEnumerator ResetComboTimer()
    {
        yield return new WaitForSecondsRealtime(ComboResetTime);

        MovesManager.Instance.PlayMove(KeysPressed); //Run the move from the list
        KeysPressed.Clear(); //Empty the list
        allKeysPressed = null;
    }
}