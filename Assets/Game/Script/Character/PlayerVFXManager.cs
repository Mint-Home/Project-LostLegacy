using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    public VisualEffect footStep;
    public VisualEffect slash;
    public VisualEffect getHeal;

    public ParticleSystem blade01;
    public ParticleSystem blade02;
    public ParticleSystem blade03;

    public void Update_FootStep(bool state)
    {
        if (state)
        {
            footStep.Play();
        }
        else
        {
            footStep.Stop();
        }
    }

    public void PlayBlade01VFX()
    {
        blade01.Play();
    }

    public void PlayBlade02VFX()
    {
        blade02.Play();
    }

    public void PlayBlade03VFX()
    {
        blade03.Play();
    }

    public void StopBlade()
    {
        blade01.Simulate(0);    //Reset
        blade01.Stop();

        blade02.Simulate(0);
        blade02.Stop();

        blade03.Simulate(0);
        blade03.Stop();
    }

    public void PlaySlash(Vector3 position)
    {
        slash.transform.position = position;
        slash.Play();
    }

    public void PlayGetHealVFX()
    {
        getHeal.Play();
    }
}
