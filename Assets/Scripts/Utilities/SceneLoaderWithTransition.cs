using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoaderWithTransition : MonoBehaviour
{
    public static SceneLoaderWithTransition Instance;

    public float minShowTime = 2f; // 最短显示时间（秒）

    private void Awake()
    {
        Instance = this;
        TransitionPanelController.EnsureLoaded();
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 切换场景并显示过渡动画
    /// </summary>
    public void LoadSceneWithTransition(string sceneName, LoadSceneMode loadSceneMode)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, loadSceneMode));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode loadSceneMode)
    {
        if (loadSceneMode == LoadSceneMode.Additive)
        {
            // 1. 显示过渡Panel
            TransitionPanelController.Instance.Show();
        }

        // 2. 记录显示开始时间
        float startTime = Time.realtimeSinceStartup;

        // 3. 异步加载场景
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        asyncOp.allowSceneActivation = false; // 先不激活

        // 4. 等待加载完成
        while (asyncOp.progress < 0.9f)
        {
            yield return null;
        }

        // 5. 等待最短显示时间
        float elapsed = Time.realtimeSinceStartup - startTime;
        if (elapsed < minShowTime)
        {
            yield return new WaitForSecondsRealtime(minShowTime - elapsed);
        }

        // 6. 激活场景
        asyncOp.allowSceneActivation = true;

        // 7. 等待场景真正切换完成
        while (!asyncOp.isDone)
        {
            yield return null;
        }

        // 7.5 设置新场景为激活场景
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
        }

        // 7.6 通知场景加载完成
        EventManager.CallAfterSceneLoad();

        // 8. 可选：再等一小会儿再隐藏（比如0.3秒）
        yield return new WaitForSecondsRealtime(0.3f);

        // 9. 隐藏过渡Panel
        TransitionPanelController.Instance.Hide();

        // 10. 显示游戏面板
        UIMgr.Instance.ShowPanel<GameView>();

        // 11. 处理背景音乐
        MusicSceneManager.HandleBackgroundMusic();
    }

    public void SwitchBusinessScene(string oldSceneName)
    {
        if (!string.IsNullOrEmpty(oldSceneName) && oldSceneName != "PersistentScene")
        {
            SceneManager.UnloadSceneAsync(oldSceneName);
        }
    }
}
