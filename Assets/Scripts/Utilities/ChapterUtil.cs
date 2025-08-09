using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ChapterUtil
{
    public static ChapterSelectConfigSO configSO;

    public static void EnterChapter(int chapterIndex)
    {
        if (configSO == null)
        {
            Debug.LogError("ChapterSelectConfigSO 未赋值！");
            return;
        }
        ChapterIndex.currentChapterIndex = chapterIndex;
        ChapterSelectConfig config = configSO.chapterSelectConfigs[chapterIndex];

        SceneLoaderWithTransition.Instance.LoadSceneWithTransition(config.chapterName, LoadSceneMode.Additive);

        UIMgr.Instance.HidePanel<SelectChapterPanel>();
    }
}
