using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOrb : MonoBehaviour
{
    public float speed = 3;
    public int damage = 30;

    public ParticleSystem hitVFX;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Character cc = other.GetComponent<Character>();
        if(cc != null && cc.isPlayer)
        {
            cc.ApplyDamage(damage, transform.position);
        }

        Instantiate(hitVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
