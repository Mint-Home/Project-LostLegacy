using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject gateVisual;

    private Collider gateCollider;

    public float openDuration = 1f;
    public float targetY = -2f;

    private void Awake()
    {
        gateCollider = GetComponent<Collider>();
    }

    IEnumerator OpenGateAnimation()
    {
        float currentOpenDuration = 0;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + Vector3.up * targetY;

        while(currentOpenDuration < openDuration)
        {
            currentOpenDuration += Time.deltaTime;
            gateVisual.transform.position = Vector3.Lerp(startPosition, targetPosition, currentOpenDuration / openDuration);
            yield return null;
        }

        gateCollider.enabled = false;
    }

    public void OpenGate()
    {
        StartCoroutine(OpenGateAnimation());
    }
}
