using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{
    private bool isGameOver = false;

    public int Gold { get; private set; }
    public int HP { get; private set; }

    public PlayerModel(int startGold, int startHP)
    {
        Gold = startGold;
        HP = startHP;

        // 初始化 UI
        EventManager.CallPlayerGoldChangedEvent(Gold);
        EventManager.CallPlayerHPChangedEvent(HP);

        isGameOver = false;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        EventManager.CallPlayerGoldChangedEvent(Gold);
    }

    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            EventManager.CallPlayerGoldChangedEvent(Gold);
            return true;
        }
        Debug.LogWarning("金币不足！");
        return false;
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver) return;

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            isGameOver = true;
            EventManager.CallGameOverEvent();
        }
        EventManager.CallPlayerHPChangedEvent(HP);
    }
}
