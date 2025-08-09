using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerConfig", menuName = "Configs/Tower")]
public class TowerConfigSO : ScriptableObject
{
    public List<TowerConfig> towerConfigs;
}
