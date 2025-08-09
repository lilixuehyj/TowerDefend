using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PoolTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        EventManager.CallBulletEvent(PoolType.Bullet, new Vector3(1,1,1));
    }

}
