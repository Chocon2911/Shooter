using System.Collections.Generic;
using UnityEngine;

public abstract class DetectTarget : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Detect Target===")]
    [Header("Primary Value")]
    [SerializeField] protected Float detectRange;
    [SerializeField] protected List<String> targetTags;
    [SerializeField] protected CBLayerMask layerMask;

    [Header("Reference")]
    [SerializeField] protected List<CBTransform> refTargets;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadValue(ref this.detectRange, transform.Find("Data/DetectRange"), "LoadDetectRange()");
        this.LoadValue(ref this.targetTags, transform.Find("Data/TargetTags"), "LoadTargetTags()");
        this.LoadValue(ref this.layerMask, transform.Find("Data/LayerMask"), "LoadLayerMask()");
    }

    //=======================================Base Component=======================================
    public override void OnUpdate()
    {
        base.OnUpdate();
        this.DetectingTarget();
    }

    //===========================================Method===========================================
    protected abstract void DetectingTarget();
    protected virtual void ApplyTargets(Collider[] hits)
    {
        foreach (var hit in hits)
        {
            if (hit == null || hit.transform == transform) continue;
            
            foreach (String tag in this.targetTags)
            {
                if (hit.tag != tag.Value) continue;

                for (int i = 0; i < this.refTargets.Count; i++)
                {
                    this.refTargets[i].Value = hit.transform;
                }
            }
        }
    }
}
