using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : BasePanel_
{
    public Button btnReStart;
    public Button btnQuit;
    public Button btnClose;

    public override void Init()
    {
        btnReStart.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<PausePanel>(false);
            UIMgr.Instance.ShowPanel<PauseNextPanelRestart>();
        });
        btnQuit.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<PausePanel>(false);
            UIMgr.Instance.ShowPanel<PauseNextPanelQuit>();
        });
        btnClose.onClick.AddListener(() =>
        {
            PauseManager.ResumeGame();
            UIMgr.Instance.HidePanel<PausePanel>();
        });
    }

    public override void ShowMe()
    {
        isShow = true;
        gameObject.SetActive(true);
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f;
        // 动画
        canvasGroup.DOFade(1f, 0.3f);
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                PauseManager.PauseGame(); // 动画播完再暂停
            });
    }
}
