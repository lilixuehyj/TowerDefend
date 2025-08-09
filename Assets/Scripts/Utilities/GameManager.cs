using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 游戏管理器，负责波次控制、敌人生成与胜利判定
public class GameManager : SingletonMono<GameManager>
{
    // 配置好的波次数据（ScriptableObject）
    public WaveConfigSO waveConfigSO;
    // 敌人出生位置
    public Transform spawnPoint;
    // 敌人寻路目标
    public Transform targetPoint;
    // 每波之间的间隔时间
    public float nextWaveDelay = 3f;

    // 当前波次索引
    public int currentWaveIndex = 0;
    // 场上存活敌人数量
    private int aliveEnemyCount = 0;
    // 是否正在生成敌人
    private bool isSpawning = false;
    // 是否结束
    private bool isGameOver = false;

    private bool isDestroyed = false;


    // 游戏开始时自动启动第一波
    private void Start()
    {
        // 监听死亡回调
        EventManager.EnemyDeadEvent += OnEnemyDead;
        StartCoroutine(StartGameWithCountdown());
    }

    private void OnEnable()
    {
        EventManager.GameOverEvent += OnGameOver;
    }

    private void OnDisable()
    {
        EventManager.GameOverEvent -= OnGameOver;
    }

    private void OnEnemyDead(EnemyModel model)
    {
        // 检查对象是否有效
        if (isDestroyed || this == null || gameObject == null)
        {
            Debug.LogWarning("GameManager已被销毁，忽略敌人死亡事件");
            return;
        }

        aliveEnemyCount--;
        CheckWaveClear();
    }

    private void OnGameOver()
    {
        isGameOver = true;
        // 其它收尾逻辑
        aliveEnemyCount = 0;
        isSpawning = false;
        TowerManager.GetInstance().ClearAllTowers();
    }

    private void OnDestroy()
    {
        isDestroyed = true;
        // 确保解除所有事件订阅
        EventManager.EnemyDeadEvent -= OnEnemyDead;
    }

    // 启动下一波敌人
    private void StartNextWave()
    {
        if (currentWaveIndex < waveConfigSO.waveConfigs.Count && !isGameOver)
        {
            Debug.Log($"开始第 {currentWaveIndex + 1} 波敌人");
            EventManager.CallWaveChangedEvent(currentWaveIndex + 1, waveConfigSO.waveConfigs.Count);
            StartCoroutine(SpawnWave(waveConfigSO.waveConfigs[currentWaveIndex]));
            currentWaveIndex++;
        }
    }

    // 协程：生成一波敌人
    private IEnumerator SpawnWave(WaveConfig waveConfig)
    {
        isSpawning = true;

        // 遍历本波所有敌人类型
        foreach (var waveEnemy in waveConfig.enemiesInWave)
        {
            // 生成指定数量的该类型敌人
            for (int i = 0; i < waveEnemy.enemyCount; i++)
            {
                SpawnEnemy(waveEnemy.enemyConfig);
                yield return new WaitForSeconds(waveConfig.spawnInterval); // 间隔生成
            }
        }

        isSpawning = false;
        Debug.Log("当前波敌人全部生成完成，等待击杀...");
    }

    // 生成单个敌人
    private void SpawnEnemy(EnemyConfig config)
    {
        // 从对象池获取敌人对象
        GameObject enemyObj = PoolManager.GetInstance().Get(config.poolType, spawnPoint.position);

        // 获取敌人控制器并初始化
        EnemyController_NEW controller = enemyObj.GetComponent<EnemyController_NEW>();
        controller.Init(config, targetPoint);

        // 记录存活敌人数
        aliveEnemyCount++;
    }

    // 检查当前波敌人是否全部消灭
    private void CheckWaveClear()
    {
        if (isGameOver || this == null || gameObject == null) return;
        if (!isSpawning && aliveEnemyCount <= 0)
        {
            Debug.Log("当前波敌人已被全部消灭");
            if (currentWaveIndex < waveConfigSO.waveConfigs.Count && !isGameOver && this != null && gameObject != null)
            {
                StartCoroutine(NextWaveDelay());
            }
            else
            {
                Debug.Log("所有波次完成，游戏胜利！");
                // TODO: 触发胜利逻辑
                EventManager.CallGameWinEvent();
            }
        }
    }

    // 协程：延迟启动下一波
    private IEnumerator NextWaveDelay()
    {
        Debug.Log($"下一波将在 {nextWaveDelay} 秒后开始...");
        yield return StartCoroutine(ShowCountDown((int)nextWaveDelay));
        StartNextWave();
    }

    public IEnumerator StartGameWithCountdown()
    {
        EventManager.CallWaveChangedEvent(currentWaveIndex + 1, waveConfigSO.waveConfigs.Count);
        yield return StartCoroutine(ShowCountDown(10));
        StartNextWave();
    }

    private IEnumerator ShowCountDown(int seconds)
    {
        var countDownUI = UIMgr.Instance.ShowPanel<CountDownUI>();
        countDownUI.ShowCountdown(seconds);
        while (seconds > 0)
        {
            countDownUI.UpdateCountdown(seconds);
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        countDownUI.txtCountDown.text = "GO!";
        yield return new WaitForSeconds(1f);
        countDownUI.Hide();

    }
}
