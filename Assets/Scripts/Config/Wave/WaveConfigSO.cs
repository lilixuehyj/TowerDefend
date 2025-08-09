using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfigSO", menuName = "Configs/WaveConfigSO")]
public class WaveConfigSO : ScriptableObject
{
    public List<WaveConfig> waveConfigs;
}
