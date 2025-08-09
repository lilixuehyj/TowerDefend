using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 全局事件管理器，负责游戏内各类事件的分发和调用
/// 通过静态事件和方法实现不同系统间的解耦
/// </summary>
public static class EventManager
{
    /// <summary>
    /// 子弹生成事件，参数：子弹类型、生成位置
    /// </summary>
    public static event Action<PoolType, Vector3> BulletEvent;
    public static void CallBulletEvent(PoolType poolType, Vector3 pos)
    {
        try
        {
            BulletEvent?.Invoke(poolType, pos);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用BulletEvent时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 敌人生成事件，参数：敌人GameObject
    /// </summary>
    public static event Action<GameObject> EnemyEvent;
    public static void CallEnemyEvent(GameObject enemy)
    {
        try
        {
            EnemyEvent?.Invoke(enemy);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用EnemyEvent时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 敌人死亡事件，参数：敌人数据模型
    /// </summary>
    public static event Action<EnemyModel> EnemyDeadEvent;
    public static void CallEnemyDeadEvent(EnemyModel enemyModel)
    {
        try
        {
            EnemyDeadEvent?.Invoke(enemyModel);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用EnemyDeadEvent时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 场景加载完成事件
    /// </summary>
    public static event Action AfterSceneLoad;
    public static void CallAfterSceneLoad()
    {
        try
        {
            AfterSceneLoad?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用AfterSceneLoad时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 玩家金币变化事件，参数：最新金币数
    /// </summary>
    public static event Action<int> PlayerGoldChangedEvent;
    public static void CallPlayerGoldChangedEvent(int newGold)
    {
        try
        {
            PlayerGoldChangedEvent?.Invoke(newGold);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用PlayerGoldChangedEvent时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 玩家血量变化事件，参数：最新血量
    /// </summary>
    public static event Action<int> PlayerHPChangedEvent;
    public static void CallPlayerHPChangedEvent(int newHP)
    {
        try
        {
            PlayerHPChangedEvent?.Invoke(newHP);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用PlayerHPChangedEvent时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 家园受伤事件，参数：受到的伤害值
    /// </summary>
    public static event Action<int> HomeHurtEvent;
    public static void CallHomeHurtEvent(int hurt)
    {
        try
        {
            HomeHurtEvent?.Invoke(hurt);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用HomeHurtEvent时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 游戏结束事件
    /// </summary>
    public static event Action GameOverEvent;
    public static void CallGameOverEvent()
    {
        try
        {
            GameOverEvent?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用GameOverEvent时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 波次变化事件，参数：当前波次，总波次
    /// </summary>
    public static event Action<int, int> WaveChangedEvent;
    public static void CallWaveChangedEvent(int current, int total)
    {
        try
        {
            WaveChangedEvent?.Invoke(current, total);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用WaveChangedEvent时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 游戏胜利事件
    /// </summary>
    public static event Action GameWinEvent;
    public static void CallGameWinEvent()
    {
        try
        {
            GameWinEvent?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用GameWinEvent时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 打开建造面板事件，参数：格子详情
    /// </summary>
    public static event Action<TileDetails> OpenBuildPanelEvent;
    public static void CallOpenBuildPanelEvent(TileDetails tileDetails)
    {
        try
        {
            OpenBuildPanelEvent?.Invoke(tileDetails);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用OpenBuildPanelEvent时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 打开升级面板事件，参数：格子详情
    /// </summary>
    public static event Action<TileDetails> OpenUpgradePanelEvent;
    public static void CallOpenUpgradePanelEvent(TileDetails tileDetails)
    {
        try
        {
            OpenUpgradePanelEvent?.Invoke(tileDetails);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用OpenUpgradePanelEvent时出错: {e.Message}");
        }
    }

    public static event Action<bool> OpenMusicSoundEvent;
    public static void CallOpenMusicSoundEvent(bool isOpen)
    {
        try
        {
            OpenMusicSoundEvent?.Invoke(isOpen);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用OpenMusicSoundEvent时出错: {e.Message}");
        }
    }

    public static event Action QuitChapterEvent;
    public static void CallQuitChapterEvent()
    {
        try
        {
            QuitChapterEvent?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用QuitChapterEvent时出错: {e.Message}");
        }
    }

    public static event Action EnterNextLevel;
    public static void CallEnterNextLevel()
    {
        try
        {
            EnterNextLevel?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"调用EnterNextLevel时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 清理所有事件（在场景切换时调用）
    /// </summary>
    public static void ClearAllEvents()
    {
        BulletEvent = null;
        EnemyEvent = null;
        EnemyDeadEvent = null;
        AfterSceneLoad = null;
        PlayerGoldChangedEvent = null;
        PlayerHPChangedEvent = null;
        HomeHurtEvent = null;
        GameOverEvent = null;
        WaveChangedEvent = null;
        GameWinEvent = null;
        OpenBuildPanelEvent = null;
        OpenUpgradePanelEvent = null;
        OpenMusicSoundEvent = null;
        QuitChapterEvent = null;
        EnterNextLevel = null;

        Debug.Log("所有事件已清理");
    }
}
