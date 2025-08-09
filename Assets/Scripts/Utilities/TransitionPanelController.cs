using UnityEngine;

public class TransitionPanelController : BasePanel_
{
    public static TransitionPanelController Instance;

    [Header("过渡Panel根节点（自己）")]
    public GameObject panelRoot; // 指向TransitionPanel对象


    public override void Init()
    {
        Instance = this;
    }

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (panelRoot != null)
            panelRoot.SetActive(false); // 初始隐藏
    }

    public static void EnsureLoaded()
    {
        if (Instance == null || Instance.gameObject == null)
        {
            UIMgr.Instance.ShowPanel<TransitionPanelController>();
        }
    }

    /// <summary>
    /// 显示过渡Panel
    /// </summary>
    public void Show()
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    /// <summary>
    /// 隐藏过渡Panel
    /// </summary>
    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    /*public override void ShowMe()
    {
        base.ShowMe();
        DontDestroyOnLoad(gameObject);
    }*/
}