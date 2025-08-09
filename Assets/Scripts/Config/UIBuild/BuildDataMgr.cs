using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildDataMgr : MonoBehaviour
{
    
    public static BuildDataMgr Instance;

    [Header("建筑配置 ScriptableObject")]
    public UIBuildConfigSO buildConfigSO; // 直接拖数据文件进来

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 获取所有可建造建筑（例如：点击空格子时显示）
    /// </summary>
    public List<UIBuildConfig> GetBuildListForTile(TileDetails tile)
    {
        //这里可以做地图区域/格子限制
        return buildConfigSO.UIBuildConfigs.FindAll(b => b.isBased);
    }

    /// <summary>
    /// 根据ID查找建筑数据（升级用）
    /// </summary>
    public UIBuildConfig GetBuildByID(int buildID)
    {
        return buildConfigSO.UIBuildConfigs.Find(b => b.buildID == buildID);
    }
    
    /// <summary>
    /// 获取当前建筑的下一级建筑数据（升级用）
    /// </summary>
    public UIBuildConfig GetNextLevelBuild(int currentBuildID)
    {
        var currentBuild = GetBuildByID(currentBuildID);
        if (currentBuild != null && currentBuild.nexLev > 0)
        {
            return GetBuildByID(currentBuild.nexLev);
        }
        return null; // 已经是最高级
    }
}
