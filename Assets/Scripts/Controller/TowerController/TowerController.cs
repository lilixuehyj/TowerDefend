using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    // 塔的攻击力
    private int attackPower;
    // 塔的攻击范围
    private float attackRange;
    // 塔的攻击速度
    private float attackSpeed;
    // 塔的子弹类型
    private BulletType bulletType;
    // 塔的攻击计时器
    private float shootTimer;
    // 塔的发射点
    private Transform shootPoint;
    // 塔的子弹预制体
    private GameObject bulletPrefab;
    // 塔的子弹速度
    private float bulletSpeed;

    // 当前锁定的敌人
    private EnemyController_NEW enemyController;
    // 攻击范围内的敌人列表
    private List<EnemyController_NEW> enemiesInRange = new List<EnemyController_NEW>();

    // 订阅敌人死亡事件
    private void OnEnable()
    {
        EventManager.EnemyDeadEvent += OnEnemyDead;
    }

    // 取消订阅敌人死亡事件
    private void OnDisable()
    {
        EventManager.EnemyDeadEvent -= OnEnemyDead;
    }

    /// <summary>
    /// 敌人死亡时的回调，移除列表中的死亡敌人，并切换目标
    /// </summary>
    /// <param name="model">死亡敌人的数据模型</param>
    private void OnEnemyDead(EnemyModel model)
    {
        // 查找并移除死亡敌人
        EnemyController_NEW deadEnemy = enemiesInRange.Find(e => e.enemyModel == model);
        if (deadEnemy != null)
        {
            enemiesInRange.Remove(deadEnemy);
            // 如果当前锁定目标就是死亡敌人，则切换到下一个
            if (enemyController == deadEnemy)
            {
                enemyController = enemiesInRange.Count > 0 ? enemiesInRange[0] : null;
            }
        }
    }

    void Update()
    {
        if (enemyController != null)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= attackSpeed)
            {
                shootTimer = 0f;
                Shoot();
            }
        }
        else
        {
            shootTimer = 0f;
        }
    }

    /// <summary>
    /// 初始化塔的属性（攻击力、范围、速度等），并设置检测范围碰撞体
    /// </summary>
    /// <param name="towerConfig">塔的配置数据</param>
    public void InitTowerInfo(TowerConfig towerConfig)
    {
        attackRange = towerConfig.attackRange;
        attackSpeed = towerConfig.attackSpeed;
        attackPower = towerConfig.atk;
        bulletType = towerConfig.bulletType;
        bulletSpeed = towerConfig.bulletSpeed;

        // 获取或添加CircleCollider2D用于检测敌人进入攻击范围
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        circleCollider.radius = attackRange;
        circleCollider.isTrigger = true;
    }

    /// <summary>
    /// 敌人进入攻击范围时触发，将其加入列表并尝试锁定为目标
    /// </summary>
    /// <param name="other">进入的碰撞体</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyController_NEW>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                Debug.Log($"检测到敌人进入:{enemy.enemyModel.enemyConfig.enemyName}");
                enemiesInRange.Add(enemy);

                // 如果当前没有锁定目标，或者列表中只有一个敌人，就锁定第一个
                if (enemyController == null || enemiesInRange.Count == 1)
                {
                    enemyController = enemiesInRange[0];
                }
            }
        }
    }

    /// <summary>
    /// 敌人离开攻击范围时触发，将其移出列表并切换目标
    /// </summary>
    /// <param name="other">离开的碰撞体</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyController_NEW>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                Debug.Log($"检测到敌人离开:{enemy.enemyModel.enemyConfig.enemyName}");
                enemiesInRange.Remove(enemy);
                // 如果离开的是当前锁定目标，切换到下一个
                if (enemy == enemyController)
                {
                    if (enemiesInRange.Count > 0)
                    {
                        enemyController = enemiesInRange[0];
                    }
                    else
                    {
                        enemyController = null;
                    }
                }
            }
        }
    }

    private void Shoot()
    {
        PoolType poolType;
        switch (bulletType)
        {
            case BulletType.Penetrate: poolType = PoolType.Bullet2; break;
            case BulletType.Explore: poolType = PoolType.Bullet3; break;
            default: poolType = PoolType.Bullet; break;
        }
        GameObject bulletObj = PoolManager.GetInstance().Get(poolType, this.transform.position);
        BulletController bullet = bulletObj.GetComponent<BulletController>();
        if (bullet != null)
        {
            bullet.Init(enemyController.transform, attackPower, bulletSpeed, bulletType, 3, poolType);
        }
    }
}
