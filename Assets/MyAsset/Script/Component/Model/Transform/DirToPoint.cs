using UnityEngine;

public class DirToPoint : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Dir To Point===")]
    [Header("Primary Value")]
    [SerializeField] private CBVector3 dir;
    [SerializeField] private Float range;
    [SerializeField] private Bool applyZero;

    [Header("Reference")]
    [SerializeField] private CBVector3 refPoint;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.dir, transform.Find("Data/Dir"), "LoadDir()");
        this.LoadComponent(ref this.range, transform.Find("Data/Range"), "LoadRange()");
    }

    //=======================================Base Component=======================================
    public override void OnUpdate()
    {
        base.OnUpdate();
        this.ApplyPoint();
    }

    //===========================================Method===========================================
    public void ApplyPoint()
    {
        if (!this.applyZero.Value && this.dir.Value == Vector3.zero) return;
        this.refPoint.Value = this.dir.Value * this.range.Value;
    }
}
