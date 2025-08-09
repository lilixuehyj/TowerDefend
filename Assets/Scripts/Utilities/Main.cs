using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_STANDALONE
        Screen.SetResolution(1280, 720, false);
#elif UNITY_ANDROID || UNITY_IOS
        Screen.fullScreen = true;
#endif
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        MusicMgr.GetInstance().InitMusicSettingFromLocal();

        UIMgr.Instance.ShowPanel<StartView>();
    }

    private void OnDestroy()
    {
        // 清理MusicMgr资源
        try
        {
            if (MusicMgr.GetInstance() != null)
            {
                MusicMgr.GetInstance().Cleanup();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"清理MusicMgr时出错: {e.Message}");
        }

        // 清理事件
        try
        {
            EventManager.ClearAllEvents();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"清理事件时出错: {e.Message}");
        }
    }
}