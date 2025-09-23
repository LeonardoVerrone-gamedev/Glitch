using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManagerScript : MonoBehaviour
{
    public static CameraManagerScript Instance { get; private set;}

    [SerializeField] private CinemachineVirtualCamera[] cameras;
    //0 regular
    //1 voando
    private int currentCameraIndex = 0;

    void Awake(){
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        SwitchCamera(currentCameraIndex);
    }

    public void SwitchCamera(int newCameraIndex){
        cameras[currentCameraIndex].gameObject.SetActive(false);

        currentCameraIndex = newCameraIndex;
        cameras[currentCameraIndex].gameObject.SetActive(true);

    }
}
