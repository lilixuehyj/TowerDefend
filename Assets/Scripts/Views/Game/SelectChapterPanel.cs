using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ChapterIndex
{
    public static int currentChapterIndex;
}

public class SelectChapterPanel : BasePanel_
{
    public Image imgPreviewImage;
    public Text txtChapterName;
    public Button btnTurnLeft;
    public Button btnTurnRight;
    public Button btnSure;
    public Button btnBack;

    public ChapterSelectConfigSO chapterSelectConfigSO;

    private int currentIndex = 0;
    
    public static ChaptorData chaptorData;

    public override void Init()
    {
        btnTurnLeft.onClick.AddListener(OnTurnLeftClicked);
        btnTurnRight.onClick.AddListener(OnTurnRightClicked);
        btnSure.onClick.AddListener(OnSureClicked);
        btnBack.onClick.AddListener(OnBackClicked);

        chaptorData = JsonMgr.Instance.LoadData<ChaptorData>("chaptorData");
        if (chaptorData == null || chaptorData.chaptorInfos.Count != chapterSelectConfigSO.chapterSelectConfigs.Count)
        {
            chaptorData = new ChaptorData(chapterSelectConfigSO.chapterSelectConfigs.Count);
            JsonMgr.Instance.SaveData(chaptorData, "chaptorData");
        }
        ChapterUtil.configSO = chapterSelectConfigSO;
        UpdateUI();
    }

    private void OnTurnRightClicked()
    {
        currentIndex++;
        if (currentIndex >= chapterSelectConfigSO.chapterSelectConfigs.Count)
        {
            currentIndex = chapterSelectConfigSO.chapterSelectConfigs.Count - 1;
        }

        UpdateUI();
    }

    private void OnTurnLeftClicked()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = 0;
        }

        UpdateUI();
    }

    private void OnBackClicked()
    {
        UIMgr.Instance.HidePanel<SelectChapterPanel>();
        UIMgr.Instance.ShowPanel<StartView>();
    }

    private void OnSureClicked()
    {
        if (chaptorData != null && chaptorData.chaptorInfos[currentIndex].unlocked)
        {
            ChapterIndex.currentChapterIndex = currentIndex;
            ChapterUtil.EnterChapter(currentIndex);
        }
        else
        {
            //TODO: 弹出未解锁
        }
    }

    private void UpdateUI()
    {
        var config = chapterSelectConfigSO.chapterSelectConfigs[currentIndex];
        imgPreviewImage.sprite = config.previewImage;
        txtChapterName.text = config.chapterName;

        if (chaptorData != null && currentIndex < chaptorData.chaptorInfos.Count)
        {
            bool unlocked = chaptorData.chaptorInfos[currentIndex].unlocked;
            btnSure.interactable = unlocked;
            // 你也可以显示锁图标，比如
            // lockIcon.SetActive(!unlocked);
        }
        else
        {
            btnSure.interactable = false;
            // lockIcon.SetActive(true);
        }
    }
}
