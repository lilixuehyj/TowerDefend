using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanel : BasePanel_
{
    public Button btnSure;
    public Button btnNext;

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
        UIMgr.Instance.HideAllPanels();
        PoolManager.GetInstance().ClearAllActiveObjects();
        PoolManager.GetInstance().ClearAllPools();
        string oldScene = SceneManager.GetActiveScene().name;
        SceneLoaderWithTransition.Instance.SwitchBusinessScene(oldScene);
        UIMgr.Instance.ShowPanel<StartView>();
        // 处理背景音乐
        MusicSceneManager.HandleBackgroundMusic();
        // 清除地图上的塔
        EventManager.CallGameOverEvent();
    }

    public override void Init()
    {
        btnSure.onClick.AddListener(OnQuitChapterEvent);
        btnNext.onClick.AddListener(() =>
        {
            PauseManager.ResumeGame();
            UIMgr.Instance.HideAllPanels();
            PoolManager.GetInstance().ClearAllActiveObjects();
            PoolManager.GetInstance().ClearAllPools();
            string oldScene = SceneManager.GetActiveScene().name;
            SceneLoaderWithTransition.Instance.SwitchBusinessScene(oldScene);
            ChapterUtil.EnterChapter(ChapterIndex.currentChapterIndex + 1);
            // 清除地图上的塔
            EventManager.CallGameOverEvent();
        });
    }
}
