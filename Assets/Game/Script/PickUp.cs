using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum ItemType
    {
        Heal, Coin
    }

    public ItemType type;
    public int value;

    public ParticleSystem collectVFX;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<Character>().PickUpItem(this);
            if(collectVFX != null)
            {
                Instantiate(collectVFX, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
