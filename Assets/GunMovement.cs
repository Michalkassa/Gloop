using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMovement : MonoBehaviour
{
    public PlayerController PlayerMovement;
    void Update(){
        GetInput();

        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();

        SetPositionRotation();
    }

    Vector2 moveInput;
    Vector2 lookInput;

    void GetInput(){
        moveInput.x = PlayerMovement.horizontal;
        moveInput.y = PlayerMovement.vertical;
        moveInput = moveInput.normalized ;

        lookInput.x = PlayerMovement.mouseX;
        lookInput.y = PlayerMovement.mouseY;
    }

    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    Vector3 swayPos;

    void Sway(){
        Vector3 invertLook = lookInput *-step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook * Time.deltaTime;
    }

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    Vector3 swayEulerRot; 

    void SwayRotation(){
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin {get => Mathf.Sin(speedCurve);}
    float curveCos {get => Mathf.Cos(speedCurve);}

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    Vector3 bobPosition;

    public float bobExaggeration;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;

    void BobOffset(){
        speedCurve += Time.deltaTime * (PlayerMovement.isGrounded ? (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical"))*bobExaggeration : 1f) + 0.01f;

        bobPosition.x = (curveCos*bobLimit.x*(PlayerMovement.isGrounded ? 1:0))-(moveInput.x * travelLimit.x);
        bobPosition.y = (curveSin*bobLimit.y)-(Input.GetAxis("Vertical") * travelLimit.y);
        bobPosition.z = -(moveInput.y * travelLimit.z);
    }

    void BobRotation(){
        bobEulerRotation.x = (moveInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2*speedCurve)) : multiplier.x * (Mathf.Sin(2*speedCurve) / 2));
        bobEulerRotation.y = (moveInput != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (moveInput != Vector2.zero ? multiplier.z * curveCos * moveInput.x : 0);
    }

    float smooth = 10f;
    float smoothRot = 12f;
    void SetPositionRotation(){
        transform.localPosition = 
        Vector3.Lerp(transform.localPosition,
        swayPos,
        Time.deltaTime * smooth);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

}
