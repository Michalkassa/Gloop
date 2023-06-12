using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    public enum collisionType{head,body,legs}
    public collisionType damageType;

    public PlayerController controller;
    public void HIT(float value){
        try{
            controller.health -= value;
            if(controller.health <= 0){
                controller.Die();
            }
        }catch{
            Debug.Log("Controller is not connected.");
        }
    }
}
