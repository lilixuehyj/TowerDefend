using System.Collections;
using UnityEngine;

/// <summary>
/// 爆炸特效控制器
/// 负责管理爆炸特效的显示和自动回收
/// </summary>
public class ExplosionEffect : MonoBehaviour
{
    [Header("特效设置")]
    public float duration = 0.05f; // 特效持续时间
    public bool autoReturnToPool = true; // 是否自动回收到对象池

    private SpriteRenderer spriteRenderer;
    private float elapsedTime = 0f;
    private bool isPlaying = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// 播放爆炸特效
    /// </summary>
    public void Play()
    {
        gameObject.SetActive(true);
        isPlaying = true;
        elapsedTime = 0f;

        // 重置特效状态
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            transform.localScale = Vector3.one;
        }

        // 开始播放动画
        StartCoroutine(PlayAnimation());
    }

    /// <summary>
    /// 播放爆炸特效（带自定义持续时间）
    /// </summary>
    /// <param name="customDuration">自定义持续时间</param>
    public void Play(float customDuration)
    {
        duration = customDuration;
        Play();
    }

    /// <summary>
    /// 停止特效
    /// </summary>
    public void Stop()
    {
        isPlaying = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 播放爆炸动画协程
    /// </summary>
    private IEnumerator PlayAnimation()
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * 10f;

        while (elapsedTime < duration && isPlaying)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Min(elapsedTime / duration * 3f, 1f);

            // 缩放动画（从小到大小）
            float scaleT = t; // 使用正弦函数让动画更自然
            transform.localScale = Vector3.Lerp(startScale, endScale, scaleT);

            // 透明度动画（淡出效果）
            if (spriteRenderer != null)
            {
                float alpha = 1f - t;
                spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            }

            yield return null;
        }

        // 动画结束，回收或销毁
        if (autoReturnToPool)
        {
            ReturnToPool();
        }
        else
        {
            Stop();
        }
    }

    /// <summary>
    /// 回收到对象池
    /// </summary>
    private void ReturnToPool()
    {
        isPlaying = false;
        PoolManager.GetInstance().Release(PoolType.ExplosionEffect, gameObject);
    }

    /// <summary>
    /// 设置特效精灵
    /// </summary>
    /// <param name="sprite">爆炸精灵</param>
    public void SetSprite(Sprite sprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    /// <summary>
    /// 设置特效颜色
    /// </summary>
    /// <param name="color">颜色</param>
    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    /// <summary>
    /// 设置特效大小
    /// </summary>
    /// <param name="scale">缩放值</param>
    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
}