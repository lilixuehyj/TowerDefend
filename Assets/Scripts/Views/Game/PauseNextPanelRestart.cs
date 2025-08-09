using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseNextPanelRestart : BasePanel_
{
    public Button btnSure;
    public Button btnCancel;

    public override void Init()
    {
        btnSure.onClick.AddListener(() =>
        {
            PauseManager.ResumeGame();
            TowerManager.GetInstance().ClearAllTowers();
            UIMgr.Instance.HideAllPanels();
            //先卸载当前关卡场景
            string currentScene = SceneManager.GetActiveScene().name;
            SceneLoaderWithTransition.Instance.SwitchBusinessScene(currentScene);
            PoolManager.GetInstance().ClearAllActiveObjects();
            //清空对象池
            PoolManager.GetInstance().ClearAllPools();
            //清塔
            TowerManager.GetInstance().ClearAllTowers();
            //再重新加载新的当前关卡场景
            SceneLoaderWithTransition.Instance.LoadSceneWithTransition(currentScene, LoadSceneMode.Additive);
        });
        btnCancel.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<PauseNextPanelRestart>();
            PauseManager.ResumeGame();
        });
    }
}
