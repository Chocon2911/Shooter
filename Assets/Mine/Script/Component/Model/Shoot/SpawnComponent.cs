using UnityEngine;
using UnityEngine.Events;

public class SpawnComponent : BaseComponent
{
    //==========================================Variable==========================================
    [Space(25)]
    [Header("===Shoot===")]
    [Header("Component")]
    [SerializeField] private Spawner spawner;

    [Header("Primary Value")]
    [SerializeField] private CBTransform prefab;
    [SerializeField] private CBVector3 spawnPoint;

    //===========================================Unity============================================
    public override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadComponent(ref this.prefab, transform.Find("Data/Bullet"), "LoadBullet()");
        this.LoadComponent(ref this.spawnPoint, transform.Find("Data/SpawnPoint"), "LoadSpawnPoint()");
    }

    //===========================================Method===========================================
    public void Spawn()
    {
        Transform spawnObj = this.spawner.SpawnByObj(
            this.prefab.Value, 
            this.spawnPoint.transform.position, 
            this.spawnPoint.transform.rotation);
        spawnObj.gameObject.SetActive(true);
    }
}
