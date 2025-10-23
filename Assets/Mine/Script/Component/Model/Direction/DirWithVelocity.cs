using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DirWithVelocity : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Dir With Velocity===")]
    [Header("Component")]
    [SerializeField] private Rigidbody rb;

    [Header("Reference")]
    [SerializeField] private List<CBVector3> refDirs;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.rb, transform.Find("Data/MainObj"), "LoadMainObj()");
    }

    //=======================================Base Component=======================================
    public override void OnUpdate()
    {
        base.OnUpdate();
        this.UpdateDir();
    }

    //===========================================Method===========================================
    private void UpdateDir()
    {
        Vector3 vel = this.rb.linearVelocity;
        if ((vel.x < 0.1f && vel.x > -0.1f)
            && (vel.y < 0.1f && vel.y > -0.1f)
            && (vel.z < 0.1f && vel.z > -0.1f)) return;
        
        Vector3 dir = vel.normalized;
        foreach (var item in refDirs) item.Value = dir;
    }
}
