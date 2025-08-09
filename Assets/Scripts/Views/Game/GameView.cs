using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameView : BasePanel_
{
    public Image imgHp;
    public Text txtHp;
    public Text txtWave;
    public Text txtCoin;
    public Button btnBack;
    public Image hpImage;
    public int startCountdownSeconds = 10;

    private void OnEnable()
    {
        EventManager.PlayerGoldChangedEvent += UpdateGoldUI;
        EventManager.PlayerHPChangedEvent += UpdateHPUI;
        EventManager.GameOverEvent += OnGameOver;
        EventManager.WaveChangedEvent += UpdateWaveUI;
        EventManager.GameWinEvent += OnGameWin;
    }

    private void OnDisable()
    {
        EventManager.PlayerGoldChangedEvent -= UpdateGoldUI;
        EventManager.PlayerHPChangedEvent -= UpdateHPUI;
        EventManager.GameOverEvent -= OnGameOver;
        EventManager.WaveChangedEvent -= UpdateWaveUI;
        EventManager.GameWinEvent -= OnGameWin;
    }

    protected override void Start()
    {
        // 初始化UI
        UpdateGoldUI(PlayerController.Instance.startGold);
        UpdateHPUI(PlayerController.Instance.startHP);

        Init();

        if (ChapterIndex.currentChapterIndex == 0)
        {
            StartCoroutine(ShowRookieGuide());
        }
    }

    private IEnumerator ShowRookieGuide()
    {
        yield return new WaitForSeconds(1f);
        UIMgr.Instance.ShowPanel<RookieGuidePanel>();
    }

    public override void Init()
    {
        btnBack.onClick.AddListener(() =>
        {
            //TODO:弹出是否退出提示框
            Debug.Log("弹出是否退出提示框");
            UIMgr.Instance.ShowPanel<PausePanel>();
        });
        EventManager.CallWaveChangedEvent(GameManager.GetInstance().currentWaveIndex + 1, GameManager.GetInstance().waveConfigSO.waveConfigs.Count);
    }

    private void UpdateGoldUI(int gold)
    {
        txtCoin.text = $"{gold}";
    }

    private void UpdateHPUI(int hp)
    {
        txtHp.text = $"{hp}/100";
        (hpImage.transform as RectTransform).sizeDelta = new Vector2((float)hp / 100 * 550, 30);
    }

    private void UpdateWaveUI(int currentWave, int totalWaves)
    {
        txtWave.text = $"波次: {currentWave}/{totalWaves}";
    }

    private void OnGameOver()
    {
        Debug.Log("游戏结束，显示失败界面");
        // TODO: 弹出游戏结束UI
        UIMgr.Instance.HidePanel<GameView>();
        UIMgr.Instance.ShowPanel<FailPanel>();
    }

    private void OnGameWin()
    {
        ChaptorData.PassLevel(ChapterIndex.currentChapterIndex);
        TowerManager.GetInstance().ClearAllTowers();
        UIMgr.Instance.HidePanel<GameView>();
        UIMgr.Instance.ShowPanel<WinPanel>();
    }
}
