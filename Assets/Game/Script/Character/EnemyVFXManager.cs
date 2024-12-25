using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect footStep;
    public VisualEffect attackVFX;
    public ParticleSystem BeingHitVFX;

    public void BurstFootStep()
    {
        footStep.SendEvent("OnPlay");
    }

    public void PlayAttackVFX()
    {
        attackVFX.SendEvent("OnPlay");
    }

    public void PlayBeingHitVFX(Vector3 attackerPosition)
    {
        Vector3 forceForward = transform.position - attackerPosition;
        forceForward.Normalize();
        forceForward.y = 0f;
        BeingHitVFX.transform.rotation = Quaternion.LookRotation(forceForward);
        BeingHitVFX.Play();
    }
}
