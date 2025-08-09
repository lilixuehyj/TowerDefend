using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailPanel : BasePanel_
{
    public Button btnSure;
    public Button btnReStart;

    public override void Init()
    {
        btnSure.onClick.AddListener(() =>
        {
            PauseManager.ResumeGame();
            UIMgr.Instance.HideAllPanels();
            string oldScene = SceneManager.GetActiveScene().name;
            SceneLoaderWithTransition.Instance.SwitchBusinessScene(oldScene);
            PoolManager.GetInstance().ClearAllActiveObjects();
            PoolManager.GetInstance().ClearAllPools();
            UIMgr.Instance.ShowPanel<StartView>();
            // 处理背景音乐
            MusicSceneManager.HandleBackgroundMusic();
            EventManager.CallGameOverEvent();
        });
        btnReStart.onClick.AddListener(() =>
        {
            PauseManager.ResumeGame();
            UIMgr.Instance.HideAllPanels();
            //先卸载当前关卡场景
            string currentScene = SceneManager.GetActiveScene().name;
            SceneLoaderWithTransition.Instance.SwitchBusinessScene(currentScene);
            PoolManager.GetInstance().ClearAllActiveObjects();
            PoolManager.GetInstance().ClearAllPools();
            //再重新加载新的当前关卡场景
            SceneLoaderWithTransition.Instance.LoadSceneWithTransition(currentScene, LoadSceneMode.Additive);
            EventManager.CallGameOverEvent();
        });
    }
}
