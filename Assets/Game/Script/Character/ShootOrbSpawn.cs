using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy02Shoot : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject damageOrb;

    public Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void DamageOrbSpawn()
    {
        Instantiate(damageOrb, shootPoint.position, Quaternion.LookRotation(shootPoint.forward));
    }

    private void Update()
    {
        character.RotateToTarget();
    }
}
