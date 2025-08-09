using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerConfig
{
    public int atk;
    public float attackRange;
    public float attackSpeed;
    public float bulletSpeed;
    public BulletType bulletType;
    public GameObject towerPrefab;
}
