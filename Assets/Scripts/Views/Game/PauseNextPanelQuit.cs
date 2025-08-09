using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseNextPanelQuit : BasePanel_
{
    public Button btnSure;
    public Button btnCancel;

    private void OnEnable()
    {
        EventManager.QuitChapterEvent += OnQuitChapterEvent;
    }

    private void OnDisable()
    {
        EventManager.QuitChapterEvent -= OnQuitChapterEvent;
    }

    private void OnQuitChapterEvent()
    {
        PauseManager.ResumeGame();
        TowerManager.GetInstance().ClearAllTowers();
        UIMgr.Instance.HideAllPanels();
        string oldScene = SceneManager.GetActiveScene().name;
        SceneLoaderWithTransition.Instance.SwitchBusinessScene(oldScene);
        PoolManager.GetInstance().ClearAllPools();
        PoolManager.GetInstance().ClearAllActiveObjects();
        UIMgr.Instance.ShowPanel<StartView>();
        // 处理背景音乐
        MusicSceneManager.HandleBackgroundMusic();
        EventManager.CallGameOverEvent();
    }

    public override void Init()
    {
        btnSure.onClick.AddListener(() =>
        {
            EventManager.CallQuitChapterEvent();
        });
        btnCancel.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<PauseNextPanelQuit>();
            PauseManager.ResumeGame();
        });
    }
}
