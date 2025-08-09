using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单个敌人配置
/// </summary>
[System.Serializable]
public class WaveEnemy
{
    public EnemyConfig enemyConfig; // 敌人配置
    public int enemyCount;          // 该种敌人数量
}

/// <summary>
/// 单波敌人配置
/// </summary>
[System.Serializable]
public class WaveConfig
{
    public List<WaveEnemy> enemiesInWave;
    public float spawnInterval = 1f;
}
