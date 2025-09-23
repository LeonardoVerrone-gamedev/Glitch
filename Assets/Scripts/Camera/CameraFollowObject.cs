using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float flipRotationTime;
    [SerializeField] private float followLerpVelocity;

    [SerializeField]private float moveOffset;
    [SerializeField] private float moveSpeed;

    private Coroutine turnCoroutine;
    private PlayerMovement player;
    private bool isFacingRight;

    public Vector3 cameraManualMoveVector;


    Vector3 targetPosition;

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = playerTransform.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if(cameraManualMoveVector.magnitude > 1){
            cameraManualMoveVector.Normalize();
        }
        if(cameraManualMoveVector == Vector3.zero){
            targetPosition = playerTransform.position;
        }else{
            Vector3 newTargetPosition = playerTransform.position + cameraManualMoveVector * moveOffset;
            targetPosition = Vector3.Lerp(targetPosition, newTargetPosition, moveSpeed);
        }
        transform.position = targetPosition;
    }

    public void CallTurn(){
        turnCoroutine = StartCoroutine(flipLerp());
    }
 
    private IEnumerator flipLerp(){
        float startRotation = playerTransform.localEulerAngles.y;
        float endRotation = determineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;
        while(elapsedTime < flipRotationTime){
            elapsedTime += Time.deltaTime;
            yRotation = Mathf.Lerp(startRotation, endRotation, (elapsedTime / flipRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        }
    }

    private float determineEndRotation(){
        isFacingRight = !isFacingRight;

        if(isFacingRight){
            return 100f;
        }else{
            return 0f;
        }
    }
}
