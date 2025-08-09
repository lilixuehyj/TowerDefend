using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel
{
    public EnemyConfig enemyConfig;

    public float currentHP;
    public bool isDead => currentHP > 0;

    public EnemyModel(EnemyConfig enemyConfig)
    {
        this.enemyConfig = enemyConfig;
        currentHP = enemyConfig.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Dead();
        }
    }

    public void Reset()
    {
        currentHP = enemyConfig.maxHealth;
    }

    private void Dead()
    {

    }
}
