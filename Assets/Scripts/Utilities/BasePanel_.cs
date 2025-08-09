using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasePanel_ : MonoBehaviour
{
    //专门用于控制面板透明度的组件
    private CanvasGroup canvasGroup;

    //当前是隐藏还是显示
    public bool isShow = false;

    protected virtual void Awake()
    {
        //一开始获取面板上挂载的 组件
        canvasGroup = this.GetComponent<CanvasGroup>();
        //如果忘记添加这样一个脚本了
        if (canvasGroup == null)
            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Init();
    }

    /// <summary>
    /// 注册控件事件的方法 所有的子面板 都需要去注册一些控件事件
    /// 所以写成抽象方法 让子类必须去实现
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// 显示自己时做的逻辑
    /// </summary>
    public virtual void ShowMe()
    {
        isShow = true;
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f; // 从小缩放到正常大小
        canvasGroup.DOFade(1f, 0.3f).SetUpdate(true); // 关键：用UnscaledTime
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true); // 关键：用UnscaledTime
    }

    /// <summary>
    /// 隐藏自己时做的逻辑
    /// </summary>
    public virtual void HideMe(UnityAction callBack)
    {
        isShow = false;
        canvasGroup.DOFade(0f, 0.3f).SetUpdate(true);
        transform.DOScale(0.8f, 0.3f)
            .SetEase(Ease.InBack)
            .SetUpdate(true) // 关键：用UnscaledTime
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                callBack?.Invoke();
            });
    }
}
