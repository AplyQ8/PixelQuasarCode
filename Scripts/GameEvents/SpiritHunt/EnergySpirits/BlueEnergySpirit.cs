using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueEnergySpirit : EnergySpirit
{
    
    [Header("Blue Spirit")]
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashSpeed;

    private bool dash;

    protected override void Start()
    {
        base.Start();
        
        dash = false;
        StartCoroutine(DashCd());
    }
    protected override void MovingUpdate()
    {
        if(dash)
            rb.velocity = currentDirection * dashSpeed;
        else
            base.MovingUpdate();
    }

    IEnumerator DashCd()
    {
        yield return new WaitForSeconds(Random.Range(0.75f*dashCooldown, 1.25f*dashCooldown));
        dash = true;
        StartCoroutine(DashDur());
    }
    
    IEnumerator DashDur()
    {
        yield return new WaitForSeconds(dashDuration);
        dash = false;
        StartCoroutine(DashCd());
    }
}
