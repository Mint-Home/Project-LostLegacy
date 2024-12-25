using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCaster : MonoBehaviour
{
    Collider damageCollider;

    public int damage = 30;

    public string targetTag;
    List<Collider> damageTargetList;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.enabled = false;
        damageTargetList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == targetTag && !damageTargetList.Contains(other))
        {
            Character targetCharacter =  other.GetComponent<Character>();

            if (targetCharacter != null)
            {
                targetCharacter.ApplyDamage(damage,transform.parent.position);

                PlayerVFXManager playerVFXManager = transform.parent.GetComponent<PlayerVFXManager>();

                if (playerVFXManager != null)
                {
                    RaycastHit hit;
                    Vector3 center = transform.position - (damageCollider.bounds.extents.z) * transform.forward;
                    bool isHit = Physics.BoxCast(center, damageCollider.bounds.extents / 2, transform.forward, out hit, transform.rotation, damageCollider.bounds.extents.z, 1 << 6);
                    if (isHit)
                    {
                        playerVFXManager.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));    //At several angle, enemy health was decrease but slash VFX was not activated
                    }
                }
            }
            damageTargetList.Add(other);
        }
    }

    public void EnableDamageCaster()
    {
        damageTargetList.Clear();
        damageCollider.enabled = true;
    }

    public void DisableDamageCaster()
    {
        damageTargetList.Clear();
        damageCollider.enabled = false;
    }

    public void OnDrawGizmos()
    {
        if(damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();

        }

        //collide with enemy's capsule and generate Sphere.
        RaycastHit hit;
        Vector3 center = transform.position - (damageCollider.bounds.extents.z) * transform.forward;
        bool isHit = Physics.BoxCast(center, damageCollider.bounds.extents / 2, transform.forward, out hit, transform.rotation, damageCollider.bounds.extents.z, 1 << 6);
        if (isHit)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.point, 0.3f);
        }
    }
}
