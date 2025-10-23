using UnityEngine;

public class MoveByAddVel : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Move By Dir===")]
    // Component
    [SerializeField] private Rigidbody rb;
    // Primary Value
    [SerializeField] private CBVector3 moveSpeed;
    [SerializeField] private CBVector3 moveDir;
    [SerializeField] private Bool useDir;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.moveSpeed, transform.Find("Data/MoveSpeed"), "LoadMoveSpeed()");
        this.LoadComponent(ref this.moveDir, transform.Find("Data/MoveDir"), "LoadMoveDir()");
        this.LoadComponent(ref this.useDir, transform.Find("Data/UseDir"), "LoadUseDir()");
    }

    //=======================================Base Component=======================================
    public override void OnUpdate()
    {
        Vector3 vel = Vector3.zero;
        if (this.useDir.Value) vel = new Vector3(
            this.moveDir.Value.x * this.moveSpeed.Value.x, 
            this.moveDir.Value.y * this.moveSpeed.Value.y,
            this.moveDir.Value.z * this.moveSpeed.Value.z);
        else vel = new Vector3(this.moveSpeed.Value.x, this.moveSpeed.Value.y, this.moveSpeed.Value.z);
        this.rb.linearVelocity += vel;
    }
}
