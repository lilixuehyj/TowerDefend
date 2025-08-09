using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr
{
    private static UIMgr instance = new UIMgr();
    public static UIMgr Instance => instance;

    //用于存储显示着的面板的 每显示一个面板 就会存入这个字典
    //隐藏面板时 直接获取字典中的对应面板 进行隐藏
    private Dictionary<string, BasePanel_> panelDic = new Dictionary<string, BasePanel_>();

    //场景中的 Canvas对象 用于设置为面板的父对象
    private Transform canvasTrans;

    private UIMgr()
    {
        //得到场景中的Canvas对象
        GameObject canvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvas.transform;
        //通过过场景不移除该对象 保证这个游戏过程中 只有一个 canvas对象
        GameObject.DontDestroyOnLoad(canvas);
    }

    //显示面板
    public T ShowPanel<T>() where T : BasePanel_
    {
        //泛型T的类型和面板预设体名字一样
        string panelName = typeof(T).Name;

        //判断 字典中 是否已经显示了这个面板
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;

        //显示面板 根据面板名字 动态的创建预设体 设置父对象
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));
        //把这个对象 放到场景中的 Canvas下面
        panelObj.transform.SetParent(canvasTrans, false);
        panelObj.transform.SetAsLastSibling(); // 保证在最上层

        //指向面板上 显示逻辑 并且应该把它保存起来
        T panel = panelObj.GetComponent<T>();
        //把这个面板脚本 存储到字典中 方便之后的 获取 和 隐藏
        panelDic.Add(panelName, panel);
        //调用自己的显示逻辑
        panel.ShowMe();

        return panel;
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T">面板类名</typeparam>
    /// <param name="isFade">是否淡出完毕过后才删除面板 默认是ture</param>
    public void HidePanel<T>(bool isFade = true) where T : BasePanel_
    {
        //根据泛型得名字
        string panelName = typeof(T).Name;
        //判断当前显示的面板 有没有你想要隐藏的
        if (panelDic.ContainsKey(panelName))
        {
            if (isFade)
            {
                //就是让面板 淡出完毕过后 再删除它 
                panelDic[panelName].HideMe(() =>
                {
                    //删除对象
                    GameObject.Destroy(panelDic[panelName].gameObject);
                    //删除字典里面存储的面板脚本
                    panelDic.Remove(panelName);
                });
            }
            else
            {
                //删除对象
                GameObject.Destroy(panelDic[panelName].gameObject);
                //删除字典里面存储的面板脚本
                panelDic.Remove(panelName);
            }
        }
    }

    //得到面板
    public T GetPanel<T>() where T : BasePanel_
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        //如果没有对应面板显示 就返回空
        return null;
    }

    public void HideAllPanels(System.Type exceptType = null)
    {
        var keys = new List<string>(panelDic.Keys);
        foreach (var key in keys)
        {
            if (key == typeof(TransitionPanelController).Name)
                continue;

            // 跳过 exceptType
            if (exceptType != null && key == exceptType.Name)
                continue;
            GameObject.Destroy(panelDic[key].gameObject);
        }
        // 只移除被销毁的面板
        if (exceptType != null && panelDic.ContainsKey(exceptType.Name))
        {
            var temp = panelDic[exceptType.Name];
            panelDic.Clear();
            panelDic.Add(exceptType.Name, temp);
        }
        else
        {
            panelDic.Clear();
        }

        var transPanel = UIMgr.Instance.GetPanel<TransitionPanelController>();
        if (transPanel != null && !panelDic.ContainsKey(typeof(TransitionPanelController).Name))
        {
            panelDic.Add(typeof(TransitionPanelController).Name, transPanel);
        }
    }
}