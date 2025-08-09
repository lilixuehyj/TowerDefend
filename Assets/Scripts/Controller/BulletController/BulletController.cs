using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Transform targetPos;
    private int damage;
    private float speed;
    private BulletType bulletType;
    private int penetrateCount;
    private PoolType poolType;
    private bool isReleased;
    private HashSet<EnemyController_NEW> hitEnemies = new HashSet<EnemyController_NEW>();
    private Vector3 moveDirection;
    private Vector3 finalTargetPoint;
    private float lifeTime = 4f;
    private float aliveTimer = 0f;

    public void Init(Transform targetPos, int damage, float speed, BulletType bulletType, int penetrateCount, PoolType poolType)
    {
        this.targetPos = targetPos;
        this.damage = damage;
        this.speed = speed;
        this.bulletType = bulletType;
        this.penetrateCount = penetrateCount;
        this.poolType = poolType;
        this.isReleased = false;
        hitEnemies.Clear();
        aliveTimer = 0f;

        if (bulletType == BulletType.Penetrate)
        {
            finalTargetPoint = targetPos.position;
            moveDirection = (finalTargetPoint - transform.position).normalized;
        }

        MusicMgr.GetInstance().PlaySound("shoot", false);
    }

    void Update()
    {
        if (isReleased) return;

        aliveTimer += Time.deltaTime;
        if (aliveTimer > lifeTime)
        {
            ReleaseBullet();
            return;
        }

        if (bulletType == BulletType.Penetrate)
        {
            transform.position += moveDirection * speed * Time.deltaTime;
        }
        else
        {
            if (targetPos == null || !targetPos.gameObject.activeInHierarchy)
            {
                ReleaseBullet();
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPos.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos.position) < 0.4f)
            {
                OnHit(targetPos);
            }
        }
    }

    private void OnHit(Transform hitTarget)
    {
        if (isReleased || hitTarget == null)
        {
            ReleaseBullet();
            return;
        }

        EnemyController_NEW enemy = hitTarget.GetComponent<EnemyController_NEW>();
        HandleHitEnemy(enemy);

        if (isReleased) return;

        switch (bulletType)
        {
            case BulletType.Normal:
                ReleaseBullet();
                break;
            case BulletType.Explore:
                DoExplode();
                ReleaseBullet();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isReleased) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyController_NEW enemy = other.GetComponent<EnemyController_NEW>();
            HandleHitEnemy(enemy);

            if (isReleased) return;

            if (bulletType == BulletType.Penetrate)
            {
                penetrateCount--;
                if (penetrateCount <= 0)
                {
                    ReleaseBullet();
                }
                else
                {
                    targetPos = null;
                }
            }
            else
            {
                ReleaseBullet();
            }
        }
    }

    private void HandleHitEnemy(EnemyController_NEW enemy)
    {
        if (enemy == null || enemy.IsDead() || hitEnemies.Contains(enemy)) return;

        hitEnemies.Add(enemy);
        enemy.TakeDamage(damage);
    }

    private void DoExplode()
    {
        Debug.Log("触发爆炸");
        float radius = 5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Enemy"));
        MusicMgr.GetInstance().PlaySound("explode", false);

        CreateExplosionEffect();

        foreach (var hit in hits)
        {
            EnemyController_NEW enemy = hit.GetComponent<EnemyController_NEW>();
            HandleHitEnemy(enemy);
        }
    }

    private void CreateExplosionEffect()
    {
        GameObject explosionObj = PoolManager.GetInstance().Get(PoolType.ExplosionEffect, transform.position);

        if (explosionObj != null)
        {
            ExplosionEffect explosionEffect = explosionObj.GetComponent<ExplosionEffect>();
            if (explosionEffect != null)
            {
                explosionEffect.Play(5f);
            }
            else
            {
                Debug.LogWarning("爆炸特效对象上没有 ExplosionEffect 组件！");
                PoolManager.GetInstance().Release(PoolType.ExplosionEffect, explosionObj);
            }
        }
        else
        {
            Debug.LogWarning("无法从对象池获取爆炸特效！");
        }
    }

    private void ReleaseBullet()
    {
        if (isReleased) return;
        isReleased = true;

        PoolManager.GetInstance().Release(poolType, this.gameObject);
        MusicMgr.GetInstance().StopSound(GetComponent<AudioSource>());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}
