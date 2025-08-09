using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public class PoolPrefab
{
    public PoolType poolType;
    public GameObject prefab;
}

public class PoolManager : SingletonMono<PoolManager>
{
    public List<PoolPrefab> poolPrefabs;

    private Dictionary<PoolType, ObjectPool<GameObject>> poolDict = new();
    private Dictionary<PoolType, HashSet<GameObject>> activeObjects = new();

    private bool isBattle = false;

    protected override void Awake()
    {
        base.Awake();
        CreatePools();
    }

    private void CreatePools()
    {
        foreach (var prefab in poolPrefabs)
        {
            if (poolDict.ContainsKey(prefab.poolType)) continue;

            Transform parent = new GameObject(prefab.prefab.name + "_Pool").transform;
            parent.SetParent(transform);

            PoolType type = prefab.poolType;
            activeObjects[type] = new HashSet<GameObject>();

            var pool = new ObjectPool<GameObject>(
                () =>
                {
                    var obj = Instantiate(prefab.prefab, parent);
                    if (obj.GetComponent<PoolObj>() == null)
                        obj.AddComponent<PoolObj>();
                    return obj;
                },
                obj =>
                {
                    obj.SetActive(true);
                    obj.GetComponent<PoolObj>().MarkAcquired();
                    activeObjects[type].Add(obj);
                },
                obj =>
                {

                    if (obj == null || obj.Equals(null)) return;
                    obj.SetActive(false);
                    obj.GetComponent<PoolObj>().MarkReleased();
                    activeObjects[type].Remove(obj);
                },
                obj =>
                {
                    activeObjects[type].Remove(obj);
                    Debug.Log($"[Pool] 清理并销毁对象：{obj.name}");
                    Destroy(obj);
                }
            );

            poolDict[type] = pool;
        }
    }

    public GameObject Get(PoolType type, Vector3 position)
    {
        if (poolDict.TryGetValue(type, out var pool))
        {
            var obj = pool.Get();
            obj.transform.position = position;
            return obj;
        }

        Debug.LogError($"[Pool] 无此对象池：{type}");
        return null;
    }

    public void Release(PoolType type, GameObject obj)
    {
        if (!poolDict.TryGetValue(type, out var objectPool))
        {
            Debug.LogError($"[Pool] 没有找到对象池：{type}，直接销毁对象 {obj.name}");
            return;
        }

        // 安全释放：确保是已激活对象
        if (!activeObjects.TryGetValue(type, out var set) || !set.Contains(obj))
        {
            Debug.LogWarning($"[Pool] 对象未注册为激活对象：{obj.name}（类型：{type}），已主动销毁释放");

            // 额外加一层保险：如果还没被 Destroy 才销毁
            if (obj != null && !obj.Equals(null))
            {
                obj.SetActive(false); // 防止它残留在画面上
                GameObject.Destroy(obj);
            }
            return;
        }

        objectPool.Release(obj);
    }

    public void ClearAllActiveObjects()
    {
        foreach (var kv in activeObjects)
        {
            var type = kv.Key;
            var objs = kv.Value.ToArray(); // 防止遍历中修改集合
            foreach (var obj in objs)
            {
                Release(type, obj);
            }
        }
    }

    public void ClearAllPools()
    {
        ClearAllActiveObjects();
        foreach (var pool in poolDict.Values)
            pool.Clear();
    }
}
