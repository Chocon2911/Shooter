using UnityEngine;

public class TriggerByDistance : BaseComponent
{
    //==========================================Variable==========================================
    [Header("===Trigger By Distance===")]
    [Header("Primary Value")]
    [SerializeField] private CBTransform startPoint;
    [SerializeField] private Float distance;
    [SerializeField] private CBUnityEvent trigger;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.startPoint, transform.Find("Data/StartPoint"), "LoadStartPoint()");
        this.LoadComponent(ref this.distance, transform.Find("Data/Distance"), "LoadDistance()");
        this.LoadComponent(ref this.trigger, transform.Find("Data/Trigger"), "LoadTrigger()");
    }

    //=======================================Base Component=======================================
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        this.CheckingDistance();
    }

    //===========================================Method===========================================
    private void CheckingDistance()
    {
        // Current object position
        Vector2 currPos = transform.position;

        // Ensure startPoint is valid before using it
        if (this.startPoint == null || this.startPoint.Value == null) return;

        // Get starting position
        Vector2 startPos = this.startPoint.Value.position;

        // Calculate the distance between current position and start position
        float currDistance = Vector2.Distance(currPos, startPos);

        // Check if the current distance exceeds the threshold
        if (currDistance >= this.distance.Value)
        {
            // Trigger an event, log, or action here
            this.trigger.Value?.Invoke();
        }
    }
}
