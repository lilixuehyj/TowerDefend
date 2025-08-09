using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpView : MonoBehaviour
{
    public RectTransform imgHpFore;

    private Transform targetPos;
    private Vector3 offset;
    private Coroutine hideCoroutine;
    [SerializeField]
    private float hideTime;

    /// <summary>
    /// 初始化血条，指定要跟随的目标和偏移位置
    /// </summary>
    /// <param name="transform">目标物体（如敌人）</param>
    /// <param name="offset">UI 相对于目标的位置偏移</param>
    public void Init(Transform transform, Vector3 offset)
    {
        this.targetPos = transform;
        this.offset = offset;
        this.gameObject.SetActive(false);

        // 清理旧的隐藏协程
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    /// <summary>
    /// 刷新血量显示，并重置隐藏计时器
    /// </summary>
    /// <param name="current">当前血量</param>
    /// <param name="max">最大血量</param>
    public void UpdateHealth(float current, float max)
    {
        if (imgHpFore != null)
        {
            imgHpFore.sizeDelta = new Vector2(current / max * 50, 5);
        }

        RefreshTimer();
    }

    /// <summary>
    /// 显示血条并重启隐藏计时器
    /// </summary>
    private void RefreshTimer()
    {
        // 重新激活对象（避免隐藏状态下刷新无效）
        gameObject.SetActive(true);

        // 若已有隐藏计时器，先停止
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        // 启动新的隐藏协程
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    /// <summary>
    /// 一段时间后隐藏血条（协程）
    /// </summary>
    private IEnumerator HideAfterDelay()
    {
        // 等待设定时间
        yield return new WaitForSeconds(hideTime);

        // 隐藏血条对象
        gameObject.SetActive(false);
        hideCoroutine = null;
    }

    void Update()
    {
        // 若目标已销毁，隐藏血条
        if (targetPos == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // 计算目标头顶偏移位置在屏幕中的坐标
        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPos.position + offset);

        // 设置 UI 元素的位置为屏幕坐标
        transform.position = screenPos;
    }
}
