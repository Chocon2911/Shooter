using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseComponent : HuyMonoBehaviour
{
    //==========================================Variable==========================================
    [Header("===Base Component===")]
    [SerializeField] private List<BaseComponent> startQueue;
    [SerializeField] private List<BaseComponent> updateQueue;
    [SerializeField] private List<BaseComponent> fixedUpdateQueue;
    [SerializeField] private List<BaseComponent> endQueue;

    //===========================================Unity============================================
    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var item in startQueue) item.OnStart();
    }

    protected virtual void Update()
    {
        foreach (var item in updateQueue)
        {
            if (item.gameObject.activeSelf) item.OnUpdate(); 
        }
    }

    protected virtual void FixedUpdate()
    {
        foreach (var item in fixedUpdateQueue)
        {
            if (item.gameObject.activeSelf) item.OnFixedUpdate();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        foreach (var item in endQueue) item.OnEnd();
    }

    public override void LoadComponents()
    {
        base.LoadComponents();
        this.startQueue = new List<BaseComponent>();
        this.updateQueue = new List<BaseComponent>();
        this.fixedUpdateQueue = new List<BaseComponent>();
        this.endQueue = new List<BaseComponent>();

        foreach (Transform child in transform)
        {
            var component = child.GetComponent<BaseComponent>();
            if (component == null) continue;

            this.startQueue.Add(component);
            this.updateQueue.Add(component);
            this.fixedUpdateQueue.Add(component);
            this.endQueue.Add(component);
        }    
    }

    //===========================================Method===========================================
    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnEnd() { }
}
