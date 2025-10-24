using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DetectTargetBySphereCol : DetectTarget
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Sphere Collider===")]
    [Header("Primary Value")]
    [SerializeField] private SphereCollider detectCol;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.detectCol, transform.Find("Data/DetectCol"), "LoadDetectCol()");
    }

    //===========================================Method===========================================
    protected override void DetectingTarget()
    {
        if (this.detectCol == null || this.detectRange == null) return;

        // Sync collider radius with detect range
        this.detectCol.radius = this.detectRange.Value;

        // Detect all colliders within the sphere area
        Collider[] hits = Physics.OverlapSphere(transform.position, this.detectCol.radius, this.layerMask.Value);

        // Clear all reference targets' value
        for (int i = 0; i < this.refTargets.Count; i++)
        {
            this.refTargets[i].Value = null;
        }

        // Apply valid target based on tag
        this.ApplyTargets(hits);
    }
}
