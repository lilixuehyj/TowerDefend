using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletSpawner : MonoBehaviour
{
    public Transform targetPos; // 终点位置
    public Vector3 startPos;    // 敌人/子弹生成位置

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 生成敌人
            GameObject enemy = PoolManager.GetInstance().Get(PoolType.Enemy, startPos);

            var controller = enemy.GetComponent<EnemyController_NEW>();
            if (controller != null)
            {
                //controller.Init(targetPos); // 初始化目标点
            }
            else
            {
                Debug.LogError("EnemyController_NEW 未挂在敌人预制体上！");
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            // 生成子弹
            GameObject bullet = PoolManager.GetInstance().Get(PoolType.Bullet, startPos);

            // TODO: 如果需要让子弹飞行后回收，可以在这里写逻辑
            StartCoroutine(ReturnBullet(bullet, 1.5f));
        }
    }

    // 子弹延迟回收
    private IEnumerator ReturnBullet(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        PoolManager.GetInstance().Release(PoolType.Bullet, bullet);
    }
}