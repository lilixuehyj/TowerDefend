using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public PlayerModel Player { get; private set; }

    [Header("初始数据")]
    public int startGold = 1000;
    public int startHP = 100;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Player = new PlayerModel(startGold, startHP);

        EventManager.EnemyDeadEvent += OnEnemyDead;
        EventManager.HomeHurtEvent += OnEnemyReachedEnd;
    }

    private void OnDestroy()
    {
        EventManager.EnemyDeadEvent -= OnEnemyDead;
        EventManager.HomeHurtEvent -= OnEnemyReachedEnd;
    }

    private void OnEnemyDead(EnemyModel enemyModel)
    {
        Player.AddGold(enemyModel.enemyConfig.rewardGold);
    }

    private void OnEnemyReachedEnd(int damage)
    {
        Player.TakeDamage(damage);
    }

    public bool TrySpendGold(int amount)
    {
        return Player.SpendGold(amount);
    }

    public void RefundGold(int amount)
    {
        Player.AddGold(amount);
    }
}
