using UnityEngine;
using UnityEngine.Events;

public class TriggerByInput : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Trigger By Input===")]
    [Header("Primary Value")]
    [SerializeField] private CBKeyCode key;
    [SerializeField] private CBUnityEvent @event;
    [SerializeField] private CBTriggerInputType type;

    //=======================================Base Component=======================================
    public override void OnUpdate()
    {
        base.OnUpdate();
        this.Triggering();
    }

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.key, transform.Find("Data/Key"), "LoadKey()");
        this.LoadComponent(ref this.@event, transform.Find("Data/Event"), "LoadTrigger()");
        this.LoadComponent(ref this.type, transform.Find("Data/Type"), "LoadType()");
    }

    //===========================================Method===========================================
    private void Triggering()
    {
        switch (this.type.Value)
        {
            case TriggerInputType.PRESS:
                if (Input.GetKeyDown(this.key.Value)) this.@event.Value?.Invoke();
                break;
            case TriggerInputType.HOLD:
                if (Input.GetKey(this.key.Value)) this.@event.Value?.Invoke();
                break;
            default:
                break;
        }
    }
}
