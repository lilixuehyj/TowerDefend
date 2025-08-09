using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : BasePanel_
{
    [Header("UI 元素")]
    public Button btnClose;
    public Transform buildButtonContainer; // 挂载按钮的父节点
    public GameObject buildButtonPrefab;   // 单个建筑按钮预制体

    // 当前选中的格子信息（用于建造判断和传递）
    private TileDetails currentTile;
    // 当前建造位置的世界坐标（用于生成建筑）
    private Vector3 buildPosition;
    // 当前建造选项按钮列表
    private List<GameObject> currentButtons = new List<GameObject>();

    // 订阅打开建造面板事件
    private void OnEnable()
    {
        EventManager.OpenBuildPanelEvent += ShowBuildOptions;
    }

    // 取消订阅事件
    private void OnDisable()
    {
        EventManager.OpenBuildPanelEvent -= ShowBuildOptions;
    }

    /// <summary>
    /// 响应事件，显示建造选项
    /// </summary>
    /// <param name="tile">当前点击的格子信息</param>
    private void ShowBuildOptions(TileDetails tile)
    {
        // 计算生成建筑的世界坐标
        Vector3 spawnPos = CursorManager.GetInstance().CurrentGrid.GetCellCenterWorld(new Vector3Int(tile.gridx, tile.gridy, 0));
        InitBuildOptions(tile, spawnPos);
    }

    /// <summary>
    /// 初始化面板（如关闭按钮绑定）
    /// </summary>
    public override void Init()
    {
        btnClose.onClick.AddListener(ClosePanel);
    }

    /// <summary>
    /// 生成对应的建造选项按钮
    /// </summary>
    /// <param name="tile">当前格子</param>
    /// <param name="spawnPos">建筑生成位置</param>
    public void InitBuildOptions(TileDetails tile, Vector3 spawnPos)
    {
        currentTile = tile;
        buildPosition = spawnPos;

        // 清理旧按钮
        foreach (var btn in currentButtons)
        {
            Destroy(btn);
        }

        currentButtons.Clear();

        // 获取可建造选项
        List<UIBuildConfig> buildConfigs = BuildDataMgr.Instance.GetBuildListForTile(currentTile);

        foreach (var config in buildConfigs)
        {
            GameObject btnObj = Instantiate(buildButtonPrefab, buildButtonContainer);
            currentButtons.Add(btnObj);

            // 设置按钮显示内容
            Image imgIcon = btnObj.transform.Find("imgBuild").GetComponent<Image>();
            Text txtName = btnObj.transform.Find("txtBuild").GetComponent<Text>();
            Text txtCost = btnObj.transform.Find("btnBuild/textPrice").GetComponent<Text>();

            imgIcon.sprite = config.sprite;
            txtName.text = config.name;
            txtCost.text = config.cost.ToString();

            Button button = btnObj.transform.Find("btnBuild").GetComponent<Button>();

            UIBuildConfig capturedConfig = config;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                Debug.Log("按钮被点了！");
                TryBuild(capturedConfig);
            });
        }
    }

    /// <summary>
    /// 点击建造按钮时的处理
    /// </summary>
    /// <param name="config">建造配置</param>
    private void TryBuild(UIBuildConfig config)
    {
        // 判断金币是否足够
        if (!PlayerController.Instance.TrySpendGold(config.cost))
        {
            Debug.LogWarning("金币不足，无法建造！");
            return;
        }

        // 调用建造逻辑
        TowerManager.GetInstance().BuildTower(config, currentTile, buildPosition);

        // 关闭建造面板
        UIMgr.Instance.HidePanel<BuildPanel>();
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    private void ClosePanel()
    {
        UIMgr.Instance.HidePanel<BuildPanel>();
    }
}
