using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileProperty
{
    public Vector2Int tilePos;

    public GripType gripType;
    
    public bool boolTypeValue;
}

[System.Serializable]
public class TileDetails
{
    public int gridx, gridy;
	
    public bool canBuild;
    
    public bool isObstacle;
    
    public bool hasBuilding;        // 是否已有建筑
    public int buildID = -1;        // 建筑ID（对应你的 UIBuildConfig.buildID）
    public int buildLevel = 0;      // 建筑等级（0=未建造，1=初级，2=中级...）
}
