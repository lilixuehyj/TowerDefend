using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 网格地图组件，用于在编辑器中设置地图数据
/// 可在编辑器中执行，用于收集Tilemap中的瓦片信息并保存到MapDataSO中
/// </summary>
[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    // 地图数据对象，用于保存当前地图的瓦片属性信息
    public MapDataSO mapData;

    // 瓦片类型，标识当前Tilemap中所有瓦片的统一类型（如障碍物、可建造区域等）
    public GripType gripType;

    // 当前Tilemap组件的引用
    private Tilemap currentTileMap;

    /// <summary>
    /// 当组件启用时调用，在编辑器模式下初始化Tilemap引用并清空地图数据
    /// </summary>
    private void OnEnable()
    {
        // 仅在编辑器模式下执行
        if (!Application.IsPlaying(this))
        {
            currentTileMap = GetComponent<Tilemap>();

            if (mapData != null)
            {
                mapData.tileProperties.Clear();
            }
        }
    }

    /// <summary>
    /// 当组件禁用时调用，在编辑器模式下更新瓦片属性并标记数据为脏数据触发保存
    /// </summary>
    private void OnDisable()
    {
        // 仅在编辑器模式下执行
        if (!Application.IsPlaying(this))
        {
            currentTileMap = GetComponent<Tilemap>();

            UpdateTileProperties();
#if UNITY_EDITOR
            // 在Unity编辑器中，标记地图数据对象为脏数据以触发保存
            if (mapData!= null)
            {
                EditorUtility.SetDirty(mapData);
            }
#endif
        }
    }

    /// <summary>
    /// 更新瓦片属性信息，遍历Tilemap中的所有瓦片并保存到地图数据对象中
    /// </summary>
    private void UpdateTileProperties()
    {
        // 压缩Tilemap边界以优化遍历范围
        currentTileMap.CompressBounds();
        
        // 仅在编辑器模式下执行
        if (!Application.IsPlaying(this))
        {
            currentTileMap = GetComponent<Tilemap>();

            if (mapData != null)
            {
                // 获取Tilemap的边界范围
                Vector3Int startPos = currentTileMap.cellBounds.min;
                Vector3Int endPos = currentTileMap.cellBounds.max;

                // 遍历所有位置
                for (int x = startPos.x; x < endPos.x; x++)
                {
                    for (int y = startPos.y; y < endPos.y; y++)
                    {
                        // 获取指定位置的瓦片
                        TileBase tile = currentTileMap.GetTile(new Vector3Int(x, y, 0));
                        // 如果该位置存在瓦片，则创建对应的属性数据
                        if (tile != null)
                        {
                            TileProperty newTile = new TileProperty
                            {
                                tilePos = new Vector2Int(x, y),
                                gripType = this.gripType,
                                boolTypeValue = true
                            };
                            mapData.tileProperties.Add(newTile);
                        }
                    }
                }
            }
        }
    }
}