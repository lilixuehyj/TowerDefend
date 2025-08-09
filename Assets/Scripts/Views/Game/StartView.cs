using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartView : BasePanel_
{
    public Button btn_Start;
    public Button btn_Setting;
    public Button btn_Quit;

    public override void Init()
    {
        btn_Start.onClick.AddListener(OnStartClick);
        btn_Setting.onClick.AddListener(OnSettingClick);
        btn_Quit.onClick.AddListener(OnQuitClick);
    }

    private void OnStartClick()
    {
        UIMgr.Instance.ShowPanel<SelectChapterPanel>();
        UIMgr.Instance.HidePanel<StartView>();
    }

    private void OnSettingClick()
    {
        UIMgr.Instance.ShowPanel<SettingPanel>();
    }

    private void OnQuitClick()
    {
        Application.Quit();
    }
}
