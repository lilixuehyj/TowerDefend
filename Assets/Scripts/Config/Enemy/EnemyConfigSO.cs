using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/Enemy")]
public class EnemyConfigSO : ScriptableObject
{
    public List<EnemyConfig> EnemyConfigs;
}
