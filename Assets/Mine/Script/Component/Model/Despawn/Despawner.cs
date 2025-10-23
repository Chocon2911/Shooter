using UnityEngine;

public class Despawner : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Despawner===")]
    [Header("Primary Value")]
    [SerializeField] private CBTransform mainObj;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.mainObj, transform.Find("Data/MainObj"), "LoadMainObj()");
    }

    //===========================================Method===========================================
    public void Despawn(Spawner spawner)
    {
        spawner.Despawn(this.mainObj.Value);     
    }
}
