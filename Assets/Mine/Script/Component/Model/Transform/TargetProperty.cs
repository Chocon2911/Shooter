using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TargetProperty : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Target Property===")]
    [Header("Component")]
    [SerializeField] private CBTransform mainObj;

    [Header("Primary Value")]
    [SerializeField] private CBTransform target;

    [Header("Reference")]
    [SerializeField] private List<Transform> refTargets;
    [SerializeField] private List<CBVector3> refDir3d;
    [SerializeField] private List<CBVector3> refDirXY;
    [SerializeField] private List<CBVector3> refDirXZ;
    [SerializeField] private List<CBVector3> refDirYZ;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadValue(ref this.mainObj, transform.Find("Data/MainObj"), "LoadMainObj()");
        this.LoadValue(ref this.target, transform.Find("Data/Target"), "LoadTarget()");
    }

    //=======================================Base Component=======================================
    public override void OnUpdate()
    {
        base.OnUpdate();
        this.ApplyTargets();
        this.ApplyDir3d();
        this.ApplyDirXY();
        this.ApplyDirXZ();
        this.ApplyDirYZ();
    }

    //===========================================Method===========================================
    private void ApplyTargets()
    {
        for (int i = 0; i < this.refTargets.Count; i++)
        {
            this.refTargets[i] = this.target.Value;
        }
    }

    private void ApplyDir3d()
    {
        if (this.mainObj.Value == null) this.ApplyDirZero();
        else if (this.target.Value == null) this.ApplyDirZero();
        else
        {
            foreach (var item in refDir3d)
            {
                item.Value = (this.target.Value.position - this.mainObj.Value.position).normalized;
            }
        }
    }

    private void ApplyDirXY()
    {
        if (this.mainObj.Value == null) this.ApplyDirZero();
        else if (this.target.Value == null) this.ApplyDirZero();
        else
        {
            foreach (var item in refDirXY)
            {
                item.Value = new Vector3(
                    this.target.Value.position.x - this.mainObj.Value.position.x,
                    this.target.Value.position.y - this.mainObj.Value.position.y,
                    0).normalized;
            }
        }
    }

    private void ApplyDirXZ()
    {
        if (this.mainObj.Value == null) this.ApplyDirZero();
        else if (this.target.Value == null) this.ApplyDirZero();
        else
        {
            foreach (var item in refDirXZ)
            {
                item.Value = new Vector3(this.target.Value.position.x - this.mainObj.Value.position.x,
                    0,
                    this.target.Value.position.z - this.mainObj.Value.position.z).normalized;
            }
        }
    }

    private void ApplyDirYZ()
    {
        if (this.mainObj.Value == null) this.ApplyDirZero();
        else if (this.target.Value == null) this.ApplyDirZero();
        else
        {
            foreach (var item in refDirYZ)
            {
                item.Value = new Vector3(
                    0,
                    this.target.Value.position.y - this.mainObj.Value.position.y,
                    this.target.Value.position.z - this.mainObj.Value.position.z).normalized;
            }
        }
    }

    //==========================================Support===========================================
    private void ApplyDirZero()
    {
        foreach (var item in refDir3d)
        {
            item.Value = Vector3.zero;
        }
        foreach (var item in refDirXY)
        {   
            item.Value = Vector3.zero;
        }
        foreach (var item in refDirXZ)
        {
            item.Value = Vector3.zero;
        }
        foreach (var item in refDirYZ)
        {
            item.Value = Vector3.zero;
        }
    }
}
