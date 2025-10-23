using UnityEngine;

public class MoveByForce : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Move By Force===")]
    // Component
    [SerializeField] private Rigidbody rb;
    // Primary Value
    [SerializeField] private Float moveSpeed;
    [SerializeField] private CBVector3 moveDir;
    [SerializeField] private CBForceMode forceMode;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.moveSpeed, transform.Find("Data/MoveSpeed"), "LoadMoveSpeed()");
        this.LoadComponent(ref this.moveDir, transform.Find("Data/MoveDir"), "LoadMoveDir()");
        this.LoadComponent(ref this.forceMode, transform.Find("Data/ForceMode"), "LoadForceMode()");
    }

    //=======================================Base Component=======================================
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        this.rb.AddForce(this.moveDir.Value * this.moveSpeed.Value, this.forceMode.Value);
    }
}
