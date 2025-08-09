using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using DG.Tweening;

public class EnemyController_NEW : MonoBehaviour
{
    // A*路径寻路组件
    private Seeker seeker;
    // 路径点列表
    private List<Vector3> pathPointList;
    // 当前路径点索引
    private int currentIndex = 0;
    // 敌人移动速度
    private float speed = 0.5f;
    // 敌人朝向（1为右，-1为左）
    private int facingDirection = 1;

    // 寻路目标点
    public Transform targetPos;
    // 当前对象池类型
    public PoolType poolType = PoolType.Enemy;

    // 敌人属性模型，存储血量等数据（由SO配置初始化）
    public EnemyModel enemyModel { get; private set; }
    private float invincibleTime = 0.1f; // 无敌时间
    private float lastDamageTime = 0f;

    // 敌人原色
    private Color originalColor;
    private Tween colorTween;

    // 添加死亡状态标记
    private bool isDead = false;
    private bool isDestroyed = false;

    // 敌人血条
    private EnemyHpView hpBar;

    /// <summary>
    /// 初始化敌人属性和目标点
    /// </summary>
    /// <param name="config">敌人属性配置（来自SO）</param>
    /// <param name="target">寻路目标点</param>
    public void Init(EnemyConfig config, Transform target)
    {
        if (isDestroyed || this == null || gameObject == null) return;

        // 用SO配置初始化敌人属性模型
        this.enemyModel = new EnemyModel(config);
        // 设置寻路目标
        this.targetPos = target;
        // 生成寻路路径
        GeneratePath(target.position);
        // 可选：根据SO配置设置速度等属性
        this.speed = config.moveSpeed;
        // 设置对象池类型
        this.poolType = config.poolType;

        var hpGO = PoolManager.GetInstance().Get(PoolType.EnemyHp, Vector3.zero);
        hpGO.transform.SetParent(GameObject.Find("Canvas(Clone)").transform);

        hpBar = hpGO.GetComponent<EnemyHpView>();
        hpBar.Init(this.transform, new Vector3(0, 1.5f, 0));
        hpBar.UpdateHealth(enemyModel.currentHP, enemyModel.enemyConfig.maxHealth);
        hpBar.gameObject.SetActive(false);

        // 重置状态
        isDead = false;
        lastDamageTime = 0f;
    }

    private void Awake()
    {
        if (this == null || gameObject == null) return;

        try
        {
            originalColor = this.GetComponent<Renderer>().material.color;
            seeker = GetComponent<Seeker>();
            // 如果未指定目标点，则自动查找带Tag的目标
            if (targetPos == null)
            {
                targetPos = GameObject.FindWithTag("Target")?.transform;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"EnemyController_NEW Awake时出错: {e.Message}");
        }
    }

    private void OnDestroy()
    {
        isDestroyed = true;

        // 清理Tween

        if (colorTween != null)
        {
            colorTween.Kill();
            colorTween = null;
        }
    }

    private void Update()
    {
        // 检查对象是否有效
        if (!IsValid()) return;

        // 检查自身和目标Transform是否已被销毁
        if (this == null || targetPos == null)
            return;

        // 如果没有路径，重新生成
        if (pathPointList == null || pathPointList.Count == 0)
        {
            GeneratePath(targetPos.position);
            return;
        }

        // 没到终点则持续移动
        if (Vector2.Distance(transform.position, targetPos.position) > 0.1f)
        {
            Vector2 direction = (pathPointList[currentIndex] - transform.position).normalized;
            transform.Translate(speed * Time.deltaTime * direction, Space.World);

            // 根据移动方向翻转朝向
            FlipSprite(direction.x);

            // 到达当前路径点，前进到下一个
            if (Vector2.Distance(transform.position, pathPointList[currentIndex]) < 0.1f)
            {
                currentIndex++;
                if (currentIndex >= pathPointList.Count)
                {
                    OnReachDestination();
                }
            }
        }
    }

    /// <summary>
    /// 生成寻路路径
    /// </summary>
    /// <param name="targetPos">目标点</param>
    private void GeneratePath(Vector3 targetPos)
    {
        if (!IsValid() || seeker == null) return;

        try
        {
            currentIndex = 0;
            seeker.StartPath(transform.position, targetPos, (path) =>
            {

                if (IsValid())
                {
                    pathPointList = path.vectorPath;

                }
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError($"生成路径时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 到达终点的处理（如回收敌人）
    /// </summary>
    private void OnReachDestination()
    {
        if (!IsValid()) return;

        Debug.Log("敌人到达终点，回收");

        try
        {
            EventManager.CallHomeHurtEvent(enemyModel.enemyConfig.damage);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用HomeHurtEvent时出错: {e.Message}");
        }


        Die();
    }

    /// <summary>
    /// 敌人受伤（被塔攻击时调用）
    /// </summary>
    /// <param name="dmg">伤害值</param>
    public void TakeDamage(float dmg)
    {
        // 检查对象是否有效
        if (!IsValid() || isDead) return;

        // 无敌时间检查
        if (Time.time - lastDamageTime < invincibleTime)
        {
            return;
        }

        lastDamageTime = Time.time;

        try
        {
            enemyModel.TakeDamage(dmg);
            MusicMgr.GetInstance().PlaySound("hurt", false);
            Debug.Log(enemyModel.currentHP);
            // 受伤时闪红
            FlashRed();
            // 血量归零则死亡
            if (enemyModel.currentHP <= 0)
            {
                hpBar.gameObject.SetActive(false);
                Die();
            }
            if (hpBar != null)
            {
                hpBar.UpdateHealth(enemyModel.currentHP, enemyModel.enemyConfig.maxHealth);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"敌人受伤处理时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 根据x轴移动方向翻转朝向
    /// </summary>
    /// <param name="directionX">x轴移动方向（正值向右，负值向左）</param>
    private void FlipSprite(float directionX)
    {
        if (!IsValid()) return;

        try
        {
            if (directionX > 0 && facingDirection < 0)
            {
                // 向右移动，但当前朝左，需要翻转回朝右
                facingDirection = 1;
                transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            }
            else if (directionX < 0 && facingDirection > 0)
            {
                // 向左移动，但当前朝右，需要翻转成朝左
                facingDirection = -1;
                transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"翻转精灵时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 敌人死亡处理（播放动画、回收等）
    /// </summary>
    public void Die()
    {
        if (!IsValid() || isDead) return;

        isDead = true;

        try
        {
            EventManager.CallEnemyDeadEvent(enemyModel);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用EnemyDeadEvent失败: {e.Message}");
        }

        var pooled = GetComponent<PoolObj>();
        if (pooled != null && pooled.IsReleased)
        {
            Debug.LogWarning("该敌人已被释放，跳过重复回收！");
            return;
        }

        try
        {
            if (PoolManager.GetInstance() != null)
            {
                PoolManager.GetInstance().Release(poolType, gameObject);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"释放敌人到对象池时出错: {e.Message}");
        }


    }

    public void FlashRed()
    {
        if (!IsValid()) return;

        try
        {
            if (colorTween != null)
            {
                colorTween.Kill();
            }
            colorTween = this.GetComponent<Renderer>().material
            .DOColor(Color.red, 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .OnKill(() =>
            {
                if (IsValid())
                {
                    this.GetComponent<Renderer>().material.color = originalColor;
                }
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError($"闪红效果时出错: {e.Message}");
        }
    }

    // 辅助方法
    private bool IsValid()
    {
        return !isDestroyed && this != null && gameObject != null;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Reset()
    {
        isDead = false;
        lastDamageTime = 0f;
        currentIndex = 0;
        pathPointList = null;
        // 重置其他状态...
    }
}