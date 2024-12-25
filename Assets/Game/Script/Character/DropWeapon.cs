using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapon : MonoBehaviour
{
    public List<GameObject> weapons;

    public void DropWeapons()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.AddComponent<Rigidbody>();
            weapon.AddComponent<BoxCollider>();

            weapon.transform.parent = null;
        }
    }
}
