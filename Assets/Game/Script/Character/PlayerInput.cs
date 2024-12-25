using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [HideInInspector]
    public float HorizontalInput;
    [HideInInspector]
    public float VerticalInput;
    [HideInInspector]
    public bool AttackInput = false;
    [HideInInspector]
    public bool RollInput = false;

    private void Update()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Fire1"))   
        {
            AttackInput = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollInput = true;
        }
    }
    
    private void OnDisable()
    {
        ClearCache();
    }
    
    public void ClearCache()
    {
        AttackInput = false;
        RollInput = false;
        HorizontalInput = 0;
        VerticalInput = 0;
    }
}
