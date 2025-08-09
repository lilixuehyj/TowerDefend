using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIBuildConfigSO", menuName = "UIBuildConfig")]
public class UIBuildConfigSO:ScriptableObject
{
    public List<UIBuildConfig> UIBuildConfigs;
}
