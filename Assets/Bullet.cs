using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float AutoDestroyTime = 10f;
    public Rigidbody rb;
    public GameObject ImpactParticleSystem;

    private const string DISABLE_METHOD_NAME = "Disable";

    void Awake()
    {
       rb = GetComponent<Rigidbody>(); 
    }

    void OnEnable()
    {
        CancelInvoke(DISABLE_METHOD_NAME);
        Invoke(DISABLE_METHOD_NAME, AutoDestroyTime);
    }

    void OnCollisionEnter(Collision collision){
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        Instantiate(ImpactParticleSystem, pos, rot);
        Destroy(gameObject);
    }


    private void Disable(){
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
