using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataSO", menuName = "Map/MapData")]
public class MapDataSO : ScriptableObject
{
    public string sceneName;

    public List<TileProperty> tileProperties;
}
