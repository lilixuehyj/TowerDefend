using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager
{
    public static bool IsPaused { get; private set; } = false;

    public static void PauseGame()
    {
        if (IsPaused) return;
        Time.timeScale = 0f;
        IsPaused = true;
        // 可选：暂停音乐
        // MusicMgr.GetInstance().PauseBKMusic();
    }

    public static void ResumeGame()
    {
        if (!IsPaused) return;
        Time.timeScale = 1f;
        IsPaused = false;
        // 可选：恢复音乐
        // MusicMgr.GetInstance().PlayBkMusic("BKMusic");
    }
}
