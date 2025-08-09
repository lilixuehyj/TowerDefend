using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : SingletonMono<TowerManager>
{
    private Dictionary<Vector2Int, GameObject> towerDict = new Dictionary<Vector2Int, GameObject>();

    /// <summary>
    /// 在指定格子建造塔
    /// </summary>
    public void BuildTower(UIBuildConfig buildConfig, TileDetails tile, Vector3 buildPosition)
    {
        if (tile.hasBuilding)
        {
            Debug.LogWarning("该格子已存在建筑！");
            return;
        }

        // 生成塔预制体
        GameObject towerObj = Instantiate(buildConfig.towerConfig.towerPrefab, buildPosition, Quaternion.identity);
        towerObj.name = $"Tower_{buildConfig.buildID}_{tile.gridx}_{tile.gridy}";

        // 将塔存入字典
        Vector2Int gridPos = new Vector2Int(tile.gridx, tile.gridy);
        towerDict[gridPos] = towerObj;

        var towerController = towerObj.GetComponent<TowerController>();
        if (towerController != null && buildConfig.towerConfig != null)
        {
            towerController.InitTowerInfo(buildConfig.towerConfig);
        }

        // 更新 TileDetails
        tile.hasBuilding = true;
        tile.buildID = buildConfig.buildID;

        Debug.Log($"已在格子({tile.gridx},{tile.gridy})建造【{buildConfig.name}】");
    }

    /// <summary>
    /// 升级建筑
    /// </summary>
    public void UpgradeTower(TileDetails tile, UIBuildConfig nextLevelConfig)
    {
        Vector2Int gridPos = new Vector2Int(tile.gridx, tile.gridy);
        if (!towerDict.ContainsKey(gridPos))
        {
            Debug.LogWarning("该位置没有可升级的建筑！");
            return;
        }

        // 删除旧建筑
        Destroy(towerDict[gridPos]);

        // 生成新建筑
        GameObject newTower = Instantiate(nextLevelConfig.towerConfig.towerPrefab, CursorManager.GetInstance().CurrentGrid.GetCellCenterWorld(new Vector3Int(tile.gridx, tile.gridy, 0)), Quaternion.identity);
        towerDict[gridPos] = newTower;

        // 初始化新塔属性
        var towerController = newTower.GetComponent<TowerController>();
        if (towerController != null && nextLevelConfig.towerConfig != null)
        {
            towerController.InitTowerInfo(nextLevelConfig.towerConfig);
        }

        // 更新 TileDetails
        tile.buildID = nextLevelConfig.buildID;

        Debug.Log($"已升级建筑到【{nextLevelConfig.name}】");
    }

    /// <summary>
    /// 拆除建筑
    /// </summary>
    public void RemoveTower(TileDetails tile)
    {
        Vector2Int gridPos = new Vector2Int(tile.gridx, tile.gridy);
        if (!towerDict.ContainsKey(gridPos))
        {
            Debug.LogWarning("该位置没有建筑可拆除！");
            return;
        }

        // 删除建筑
        Destroy(towerDict[gridPos]);
        towerDict.Remove(gridPos);

        // 更新 TileDetails
        tile.hasBuilding = false;
        tile.buildID = -1;

        // 拆除建筑后，该位置会重新开始闪烁（在TileFlashManager的协程中自动处理）

        Debug.Log($"已拆除建筑，格子({tile.gridx},{tile.gridy})可再次建造");
    }

    /// <summary>
    /// 清除场上所有塔
    /// </summary>
    public void ClearAllTowers()
    {
        // 1. 销毁所有塔的游戏对象
        foreach (var tower in towerDict.Values)
        {
            if (tower != null)
                Destroy(tower);
        }

        // 2. 记录所有需要重置的格子位置
        List<Vector2Int> positions = new List<Vector2Int>(towerDict.Keys);

        // 3. 清空塔字典
        towerDict.Clear();

        // 4. 重置所有格子的建筑状态
        foreach (Vector2Int pos in positions)
        {
            GridMapMgr.GetInstance().ResetTileBuilding(pos);
        }

        Debug.Log("已清除场上所有塔");
    }
}
