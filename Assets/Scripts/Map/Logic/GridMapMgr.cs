using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/// <summary>
/// 网格地图管理器，用于管理游戏中的地图数据和瓦片信息
/// 继承自单例管理器 SingletonMono<GridMapMgr>
/// </summary>
public class GridMapMgr : SingletonMono<GridMapMgr>
{
    // 地图数据列表，包含多个地图配置数据
    public List<MapDataSO> mapDataList;

    // 瓦片详情字典，以坐标+场景名为键，瓦片详情为值
    private Dictionary<string, TileDetails> tileDetailsDic = new Dictionary<string, TileDetails>();

    /// <summary>
    /// 初始化时调用，遍历所有地图数据并初始化瓦片详情字典
    /// </summary>
    private void Start()
    {
        foreach (var mapDataSO in mapDataList)
        {
            InitTileDetailsDict(mapDataSO);
        }
    }

    /// <summary>
    /// 初始化瓦片详情字典
    /// </summary>
    /// <param name="mapDataSo">地图数据对象</param>
    private void InitTileDetailsDict(MapDataSO mapDataSo)
    {
        foreach (TileProperty tileProperty in mapDataSo.tileProperties)
        {
            TileDetails tileDetails = new TileDetails
            {
                gridx = tileProperty.tilePos.x,
                gridy = tileProperty.tilePos.y
            };

            // 生成字典键值，格式为"x_y场景名"
            string key = tileProperty.tilePos.x + "_" + tileProperty.tilePos.y + mapDataSo.sceneName;

            // 如果该键已存在，获取现有瓦片详情
            if (GetTileDetails(key) != null)
            {
                tileDetails = GetTileDetails(key);
            }

            // 根据瓦片类型设置对应属性
            switch (tileProperty.gripType)
            {
                case GripType.Obstacle:
                    tileDetails.isObstacle = tileProperty.boolTypeValue;
                    break;
                case GripType.CanBuild:
                    tileDetails.canBuild = tileProperty.boolTypeValue;
                    break;
            }

            // 更新或添加瓦片详情到字典中
            if (GetTileDetails(key) != null)
            {
                tileDetailsDic[key] = tileDetails;
            }
            else
            {
                tileDetailsDic.Add(key, tileDetails);
            }
        }
    }

    /// <summary>
    /// 根据键名获取瓦片详情
    /// </summary>
    /// <param name="key">瓦片键名，格式为"x_y场景名"</param>
    /// <returns>瓦片详情对象，如果未找到则返回null</returns>
    public TileDetails GetTileDetails(string key)
    {
        if (tileDetailsDic.ContainsKey(key))
        {
            return tileDetailsDic[key];
        }
        return null;
    }

    /// <summary>
    /// 根据鼠标在网格中的位置获取瓦片详情
    /// </summary>
    /// <param name="mouseGridPos">鼠标在网格中的位置</param>
    /// <returns>对应位置的瓦片详情</returns>
    public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
    {
        string key = mouseGridPos.x + "_" + mouseGridPos.y + SceneManager.GetActiveScene().name;
        return GetTileDetails(key);
    }

    /// <summary>
    /// 重置指定位置的建筑物信息
    /// </summary>
    /// <param name="position">需要重置的瓦片位置</param>
    public void ResetTileBuilding(Vector2Int position)
    {
        string key = position.x + "_" + position.y + SceneManager.GetActiveScene().name;
        TileDetails tile = GetTileDetails(key);

        if (tile != null)
        {
            tile.hasBuilding = false;
            tile.buildID = -1;
        }
    }
}