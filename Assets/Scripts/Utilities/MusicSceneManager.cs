using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 音乐场景管理器
/// 负责根据当前场景类型决定是否播放背景音乐
/// </summary>
public static class MusicSceneManager
{
    /// <summary>
    /// 战斗场景名称列表
    /// </summary>
    private static readonly string[] BattleScenes = { "Chapter01", "Chapter02", "Chapter03" };

    /// <summary>
    /// 非战斗场景名称列表
    /// </summary>
    private static readonly string[] NonBattleScenes = { "PersistentScene", "MenuScene" };

    /// <summary>
    /// 静态构造函数，注册事件监听
    /// </summary>
    static MusicSceneManager()
    {
        // 监听场景加载完成事件
        EventManager.AfterSceneLoad += OnAfterSceneLoad;
    }

    /// <summary>
    /// 场景加载完成后的回调
    /// </summary>
    private static void OnAfterSceneLoad()
    {
        // 延迟一帧处理，确保场景完全加载
        MonoMgr.GetInstance().StartCoroutine(HandleMusicAfterSceneLoad());
    }

    /// <summary>
    /// 延迟处理音乐
    /// </summary>
    private static IEnumerator HandleMusicAfterSceneLoad()
    {
        yield return null; // 等待一帧
        HandleBackgroundMusic();
    }

    /// <summary>
    /// 判断当前场景是否为战斗场景
    /// </summary>
    /// <returns>true为战斗场景，false为非战斗场景</returns>
    public static bool IsBattleScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return System.Array.Exists(BattleScenes, scene => scene == currentSceneName);
    }

    /// <summary>
    /// 判断当前场景是否为非战斗场景
    /// </summary>
    /// <returns>true为非战斗场景，false为战斗场景</returns>
    public static bool IsNonBattleScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return System.Array.Exists(NonBattleScenes, scene => scene == currentSceneName);
    }

    /// <summary>
    /// 根据当前场景类型自动处理背景音乐
    /// </summary>
    public static void HandleBackgroundMusic()
    {
        if (IsBattleScene())
        {
            // 战斗场景暂停背景音乐
            MusicMgr.GetInstance().PauseBKMusic();
        }
        else if (IsNonBattleScene())
        {
            // 非战斗场景播放背景音乐
            MusicMgr.GetInstance().PlayBkMusic("BKMusic");
        }
    }

    /// <summary>
    /// 获取当前场景名称
    /// </summary>
    /// <returns>当前场景名称</returns>
    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}