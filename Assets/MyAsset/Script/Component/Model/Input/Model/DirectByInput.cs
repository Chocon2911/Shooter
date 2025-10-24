using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirectByInput : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Move Input===")]
    [SerializeField] private List<CBVector2> refDir2D;
    [SerializeField] private List<CBVector3> refDirXY;
    [SerializeField] private List<CBVector3> refDirXZ;
    [SerializeField] private List<CBVector3> refDirYZ;

    //=======================================Base Component=======================================
    public override void OnUpdate()
    {
        Vector2 moveDir = this.GetInputDir2D();
        this.ApplyDir2D(moveDir);
        this.ApplyDirXY(moveDir);
        this.ApplyDirXZ(moveDir);
        this.ApplyDirYZ(moveDir);
    }

    //===========================================Method===========================================
    private void ApplyDir2D(Vector2 moveDir)
    {
        if (this.refDir2D.Count <= 0) return;

        foreach (CBVector2 moveDir2D in this.refDir2D)
        {
            moveDir2D.Value = moveDir;
        }
    }

    private void ApplyDirXY(Vector2 moveDir)
    {
        if (this.refDirXY.Count <= 0) return;

        foreach (CBVector3 moveDirXY in this.refDirXY)
        {
            moveDirXY.Value = new Vector3(moveDir.x, moveDir.y, 0);
        }
    }

    private void ApplyDirXZ(Vector2 moveDir)
    {
        if (this.refDirXZ.Count <= 0) return;

        foreach (CBVector3 moveDirXZ in this.refDirXZ)
        {
            moveDirXZ.Value = new Vector3(moveDir.x, 0, moveDir.y);
        }
    }

    private void ApplyDirYZ(Vector2 moveDir)
    {
        if (this.refDirYZ.Count <= 0) return;

        foreach (CBVector3 moveDirYZ in this.refDirYZ)
        {
            moveDirYZ.Value = new Vector3(0, moveDir.x, moveDir.y);
        }
    }

    //==========================================Support===========================================
    private Vector2 GetInputDir2D()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        return new Vector2(
            x > 0 ? 1f : (x < 0 ? -1f : 0f),
            y > 0 ? 1f : (y < 0 ? -1f : 0f)
        );
    }
}
