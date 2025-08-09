using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : SingletonMono<CursorManager>
{
    private Camera mainCamera;
    private Grid currentGrid;
    public Grid CurrentGrid => currentGrid;

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private void OnEnable()
    {
        EventManager.AfterSceneLoad += OnAfterSceneLoad;
    }

    private void OnDisable()
    {
        EventManager.AfterSceneLoad -= OnAfterSceneLoad;
    }

    private void OnAfterSceneLoad()
    {
        currentGrid = FindObjectOfType<Grid>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        CheckCursorValid();
    }

    private void CheckCursorValid()
    {
        if (currentGrid == null) return;

        if (mainCamera == null) Debug.LogWarning("mainCamera is null!");

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return; // 鼠标在UI上

        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        TileDetails currentTileDetails = GridMapMgr.GetInstance().GetTileDetailsOnMousePosition(mouseGridPos);
        if (currentTileDetails == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("鼠标点击事件触发");

            // 添加额外检查，确保没有UI面板打开
            if (UIMgr.Instance.GetPanel<BuildPanel>() != null ||
                UIMgr.Instance.GetPanel<UpgradePanel>() != null ||
                UIMgr.Instance.GetPanel<PausePanel>() != null ||
                UIMgr.Instance.GetPanel<RookieGuidePanel>() != null ||
                UIMgr.Instance.GetPanel<WinPanel>() != null ||
                UIMgr.Instance.GetPanel<FailPanel>() != null ||
                UIMgr.Instance.GetPanel<PauseNextPanelQuit>() != null ||
                UIMgr.Instance.GetPanel<PauseNextPanelRestart>() != null
                )
            {
                Debug.Log("有UI面板打开，忽略地图点击");
                return;
            }

            if (currentTileDetails.hasBuilding)
            {
                var panel = UIMgr.Instance.ShowPanel<UpgradePanel>();
                panel.InitUpgradeOptions(currentTileDetails);
                Debug.Log("点击了已有建筑，弹出升级/拆除菜单");
            }
            else if (currentTileDetails.canBuild && !currentTileDetails.isObstacle)
            {
                UIMgr.Instance.ShowPanel<BuildPanel>();
                EventManager.CallOpenBuildPanelEvent(currentTileDetails);
            }
        }
    }
}
