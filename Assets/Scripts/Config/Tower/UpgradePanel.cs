using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : BasePanel_
{
    [Header("UI 元素")]
    public Button btnClose;            // 关闭按钮
    public Button btnRemove;           // 拆除按钮
    public GameObject buildButtonPrefab;
    public Transform optionButtonContainer;

    private TileDetails currentTile;
    private UIBuildConfig currentConfig;
    private UIBuildConfig nextLevelConfig;

    private List<GameObject> currentButtons = new List<GameObject>();

    public override void Init()
    {
        btnClose.onClick.AddListener(ClosePanel);
        btnRemove.onClick.AddListener(OnRemoveClicked);
    }

    /// <summary>
    /// 初始化升级面板
    /// </summary>
    /// <param name="tile"></param>
    public void InitUpgradeOptions(TileDetails tile)
    {
        currentTile = tile;
        currentConfig = BuildDataMgr.Instance.GetBuildByID(tile.buildID);
        nextLevelConfig = BuildDataMgr.Instance.GetBuildByID(currentConfig.nexLev);

        if (currentConfig == null)
        {
            Debug.LogError($"未找到建筑ID: {tile.buildID} 的配置");
            ClosePanel();
            return;
        }

        // 清理旧按钮
        foreach (var btn in currentButtons)
            Destroy(btn);
        currentButtons.Clear();

        // 添加“升级”按钮（如果有下一级）
        if (nextLevelConfig != null)
        {
            GameObject upgradeBtn = Instantiate(buildButtonPrefab, optionButtonContainer);
            currentButtons.Add(upgradeBtn);

            upgradeBtn.transform.Find("txtBuild").GetComponent<Text>().text = $"升级（{nextLevelConfig.cost}）";
            upgradeBtn.transform.Find("imgBuild").GetComponent<Image>().sprite = nextLevelConfig.sprite;
            upgradeBtn.transform.Find("btnBuild/textPrice").GetComponent<Text>().text = nextLevelConfig.cost.ToString();

            upgradeBtn.transform.Find("btnBuild").GetComponent<Button>().onClick.AddListener(() =>
            {
                OnUpgradeClicked();
            });
        }
    }

    private void OnUpgradeClicked()
    {
        if (nextLevelConfig == null)
        {
            Debug.LogWarning("该建筑已满级，无法升级！");
            return;
        }

        // 检查金币
        if (!PlayerController.Instance.TrySpendGold(nextLevelConfig.cost))
        {
            Debug.LogWarning("金币不足，无法升级！");
            return;
        }

        // 调用 TowerManager 进行升级
        TowerManager.GetInstance().UpgradeTower(currentTile, nextLevelConfig);

        Debug.Log($"建筑已升级为: {nextLevelConfig.name}");

        // 关闭面板
        UIMgr.Instance.HidePanel<UpgradePanel>();
    }

    private void OnRemoveClicked()
    {
        // 拆除建筑并返还一半金币
        int refundGold = Mathf.FloorToInt(currentConfig.cost * 0.5f);
        PlayerController.Instance.RefundGold(refundGold);

        TowerManager.GetInstance().RemoveTower(currentTile);

        Debug.Log($"建筑已拆除，返还金币: {refundGold}");

        // 关闭面板
        UIMgr.Instance.HidePanel<UpgradePanel>();
    }

    private void ClosePanel()
    {
        UIMgr.Instance.HidePanel<UpgradePanel>();
    }
}
