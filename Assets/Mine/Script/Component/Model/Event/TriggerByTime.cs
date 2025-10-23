using UnityEngine;

public class TriggerByTime : BaseComponent
{
    //==========================================Variable==========================================
    [Header("===Trigger By Time===")]
    [Header("Primary Value")]
    [SerializeField] private CBCooldown cooldown;
    [SerializeField] private CBUnityEvent trigger;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.cooldown, transform.Find("Data/Cooldown"), "LoadCooldown()");
        this.LoadComponent(ref this.trigger, transform.Find("Data/Trigger"), "LoadTrigger()");
    }

    //=======================================Base Component=======================================
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        this.CoolingDown();
    }

    public override void OnStart()
    {
        base.OnStart();
        this.cooldown.Value.ResetStatus();
    }

    //===========================================Method===========================================
    private void CoolingDown()
    {
        this.cooldown.Value.CoolingDown();
        if (!this.cooldown.Value.IsReady) return;

        this.trigger.Value?.Invoke();
    }
}
