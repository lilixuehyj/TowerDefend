using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 初始化敌人
    /// </summary>
    public void Init()
    {
        if (animator != null)
        {
            PlaySpawnAnimation();
        }
    }

    /// <summary>
    /// 播放出生动画
    /// </summary>
    public void PlaySpawnAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Spawn");
        }
    }

    /// <summary>
    /// 播放死亡动画
    /// </summary>
    public void PlayDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    /// <summary>
    /// 动画事件回调 - 死亡动画播放完毕
    /// </summary>
    public void OnDeathAnimationEnd()
    {
        // 回收到对象池
        PoolManager.GetInstance().Release(PoolType.Enemy, this.gameObject);
    }
}
