using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpeedUpByAcel : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Move By Acceleration===")]
    // Primary Value
    [Header("Primary Value")]
    [SerializeField] private Float maxSpeed;
    [SerializeField] private Float speed;
    [SerializeField] private Float acceleration;
    [SerializeField] private Float deceleration;

    // Ref
    [Header("2D")]
    [SerializeField] private List<CBVector2> refMoveDir2D;
    [SerializeField] private List<Rigidbody> refRb2D;
    [SerializeField] private List<CBVector2> refMoveSpeed2D;

    [Header("XY")]
    [SerializeField] private List<CBVector3> refMoveDirXY;
    [SerializeField] private List<Rigidbody> refRbXY;
    [SerializeField] private List<CBVector3> refMoveSpeedXY;

    [Header("XZ")]
    [SerializeField] private List<CBVector3> refMoveDirXZ;
    [SerializeField] private List<Rigidbody> refRbXZ;
    [SerializeField] private List<CBVector3> refMoveSpeedXZ;

    [Header("YZ")]
    [SerializeField] private List<CBVector3> refMoveDirYZ;
    [SerializeField] private List<Rigidbody> refRbYZ;
    [SerializeField] private List<CBVector3> refMoveSpeedYZ;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.maxSpeed, transform.Find("Data/MaxSpeed"), "LoadMaxSpeed()");
        this.LoadComponent(ref this.speed, transform.Find("Data/Speed"), "LoadSpeed()");
        this.LoadComponent(ref this.acceleration, transform.Find("Data/Acceleration"), "LoadAcceleration()");
        this.LoadComponent(ref this.deceleration, transform.Find("Data/Deceleration"), "LoadDeceleration()");
    }

    //=======================================Base Component=======================================
    public override void OnUpdate()
    {
        this.ModifyMoveSpeed2D();
        this.ModifyMoveSpeedXY();
        this.ModifyMoveSpeedXZ();
        this.ModifyMoveSpeedYZ();
    }

    //=====================================Modify Move Speed======================================
    private void ModifyMoveSpeed2D()
    {
        if (this.refRb2D.Count <= 0) return; 
        if (this.refRb2D.Count != this.refMoveDir2D.Count) return;

        for (int i = 0; i < this.refRb2D.Count; i++)
        {
            Vector3 currVel = this.refRb2D[i].linearVelocity;
            Vector2 currVel2D = new Vector2(currVel.x, currVel.y);
            Vector2 moveDir2D = this.refMoveDir2D[i].Value;

            this.refMoveSpeed2D[i].Value = this.GetMoveVel(moveDir2D, currVel2D);
        }
    }

    private void ModifyMoveSpeedXY()
    {
        if (this.refRbXY.Count <= 0) return;
        if (this.refRbXY.Count != this.refMoveDirXY.Count) return;

        for (int i = 0; i < this.refRbXY.Count; i++)
        {
            // Current velocity projected to XY plane
            Vector3 currVel3 = this.refRbXY[i].linearVelocity;
            Vector2 currVelXY = new Vector2(currVel3.x, currVel3.y);

            // Input direction projected to XY plane
            Vector3 moveDir3 = this.refMoveDirXY[i].Value;
            Vector2 moveDirXY = new Vector2(moveDir3.x, moveDir3.y);

            // Compute and store speed magnitude for this plane
            Vector2 targetVel = this.GetMoveVel(moveDirXY, currVelXY);
            this.refMoveSpeedXY[i].Value = new Vector3(targetVel.x, targetVel.y, 0);
        }
    }

    private void ModifyMoveSpeedXZ()
    {
        if (this.refRbXZ.Count <= 0) return;
        if (this.refRbXZ.Count != this.refMoveDirXZ.Count) return;

        for (int i = 0; i < this.refRbXZ.Count; i++)
        {
            // Current velocity projected to XZ plane
            Vector3 currVel3 = this.refRbXZ[i].linearVelocity;
            Vector2 currVelXZ = new Vector2(currVel3.x, currVel3.z);

            // Input direction projected to XZ plane
            Vector3 moveDir3 = this.refMoveDirXZ[i].Value;
            Vector2 moveDirXZ = new Vector2(moveDir3.x, moveDir3.z);

            // Compute and store speed magnitude for this plane
            Vector2 targetVel = this.GetMoveVel(moveDirXZ, currVelXZ);
            this.refMoveSpeedXZ[i].Value = new Vector3(targetVel.x, 0, targetVel.y);
        }
    }

    private void ModifyMoveSpeedYZ()
    {
        if (this.refRbYZ.Count <= 0) return;
        if (this.refRbYZ.Count != this.refMoveDirYZ.Count) return;

        for (int i = 0; i < this.refRbYZ.Count; i++)
        {
            // Current velocity projected to YZ plane
            Vector3 currVel3 = this.refRbYZ[i].linearVelocity;
            Vector2 currVelYZ = new Vector2(currVel3.y, currVel3.z);

            // Input direction projected to YZ plane
            Vector3 moveDir3 = this.refMoveDirYZ[i].Value;
            Vector2 moveDirYZ = new Vector2(moveDir3.y, moveDir3.z);

            // Compute and store speed magnitude for this plane
            Vector2 targetVel = this.GetMoveVel(moveDirYZ, currVelYZ);
            this.refMoveSpeedYZ[i].Value = new Vector3(0, targetVel.x, targetVel.y);
        }
    }


    //==========================================Support===========================================
    private Vector2 GetMoveVel(Vector2 moveDir, Vector2 currVel)
    {
        Vector2 result = Vector2.zero;
        float accelAmount = this.speed.Value * this.acceleration.Value;
        float decelAmount = this.speed.Value * this.deceleration.Value;

        // Stop Move X
        if (moveDir.x < 0.01f && moveDir.x > -0.01f)
        {
            if (currVel.x > 0.01f)  // Đang đi sang phải, cần lực sang trái
            {
                float xTargetSpeed = -Mathf.Min(decelAmount, currVel.x);
                result.x = xTargetSpeed;
            }
            else if (currVel.x < -0.01f)  // Đang đi sang trái, cần lực sang phải
            {
                float xTargetSpeed = Mathf.Min(decelAmount, -currVel.x);
                result.x = xTargetSpeed;
            }
        }

        // Stop Move Y - tương tự
        if (moveDir.y < 0.01f && moveDir.y > -0.01f)
        {
            if (currVel.y > 0.01f)  // Đang đi lên, cần lực xuống
            {
                float yTargetSpeed = -Mathf.Min(decelAmount, currVel.y);
                result.y = yTargetSpeed;
            }
            else if (currVel.y < -0.01f)  // Đang đi xuống, cần lực lên
            {
                float yTargetSpeed = Mathf.Min(decelAmount, -currVel.y);
                result.y = yTargetSpeed;
            }
        }

        // Increase Speed - phần này OK
        if (moveDir.x > 0.1f || moveDir.x < -0.1f)
        {
            float xTargetSpeed = accelAmount * moveDir.x;
            float diffMaxSpeed = this.maxSpeed.Value * moveDir.x - currVel.x;
            if (moveDir.x > 0 && xTargetSpeed > diffMaxSpeed) xTargetSpeed = diffMaxSpeed;
            if (moveDir.x < 0 && xTargetSpeed < diffMaxSpeed) xTargetSpeed = diffMaxSpeed;
            result.x = xTargetSpeed;
        }

        if (moveDir.y > 0.1f || moveDir.y < -0.1f)
        {
            float yTargetSpeed = accelAmount * moveDir.y;
            float diffMaxSpeed = this.maxSpeed.Value * moveDir.y - currVel.y;
            if (moveDir.y > 0 && yTargetSpeed > diffMaxSpeed) yTargetSpeed = diffMaxSpeed;
            if (moveDir.y < 0 && yTargetSpeed < diffMaxSpeed) yTargetSpeed = diffMaxSpeed;
            result.y = yTargetSpeed;
        }

        return result;
    }
}
