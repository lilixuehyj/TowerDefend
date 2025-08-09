using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物属性配置
/// </summary>
[System.Serializable]
public class EnemyConfig
{
    public int enemyID;
    public string enemyName;
    public float maxHealth;
    public float moveSpeed;
    public int rewardGold;
    public int damage;
    public PoolType poolType;
    //public GameObject enemyPrefab;
}
