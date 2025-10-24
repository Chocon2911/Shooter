using UnityEngine;

public class MoveByVel : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Move By Dir===")]
    // Component
    [SerializeField] private Rigidbody rb;
    // Primary Value
    [SerializeField] private Float moveSpeed;
    [SerializeField] private CBVector3 moveDir;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.moveSpeed, transform.Find("Data/MoveSpeed"), "LoadMoveSpeed()");
        this.LoadComponent(ref this.moveDir, transform.Find("Data/MoveDir"), "LoadMoveDir()");
    }

    //=======================================Base Component=======================================
    public override void OnUpdate()
    {
        this.rb.linearVelocity = this.moveDir.Value * this.moveSpeed.Value;
    }
}
