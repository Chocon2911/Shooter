using Unity.VisualScripting;
using UnityEngine;

public class RotateByDir : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Rotate By Dir===")]
    [Header("Primary Value")]
    [SerializeField] private CBTransform mainObj;
    [SerializeField] private CBVector3 dir;
    [SerializeField] private Float speed;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.mainObj, transform.Find("Data/MainObj"), "LoadMainObj()");
        this.LoadComponent(ref this.dir, transform.Find("Data/Dir"), "LoadDir()");
        this.LoadComponent(ref this.speed, transform.Find("Data/Speed"), "LoadSpeed()");
    }

    //=======================================Base Component=======================================
    public override void OnUpdate()
    {
        base.OnUpdate();
        this.Rotating();
    }

    //===========================================Method===========================================
    private void Rotating()
    {
        // Get the direction vector
        Vector3 targetDir = dir.Value.normalized;
        if (targetDir == Vector3.zero) return; // Skip if no direction is given

        // Get the target transform to rotate
        Transform targetTransform = mainObj.Value;
        if (targetTransform == null) return;

        // Calculate the target rotation based on the direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);

        // Smoothly rotate toward the target direction at the given speed
        targetTransform.rotation = Quaternion.Lerp(
            targetTransform.rotation,
            targetRotation,
            speed.Value * Time.deltaTime
        );
    }
}
