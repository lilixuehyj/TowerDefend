using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

/// <summary>
/// YooAsset资源管理器
/// 负责初始化和管理YooAsset资源加载
/// </summary>
public class YooAssetMgr : BaseManager<YooAssetMgr>
{
    private bool isInitialized = false;
    private ResourcePackage defaultPackage;

    /// <summary>
    /// 初始化YooAsset
    /// </summary>
    public IEnumerator Initialize()
    {
        if (isInitialized)
            yield break;

        // 初始化YooAsset系统
        YooAssets.Initialize();

        // 创建默认资源包
        string packageName = "DefaultPackage";
        defaultPackage = YooAssets.TryGetPackage(packageName);
        if (defaultPackage == null)
        {
            defaultPackage = YooAssets.CreatePackage(packageName);
            YooAssets.SetDefaultPackage(defaultPackage);
        }

        // 根据运行模式初始化资源包
        InitializationOperation initializationOperation = null;

#if UNITY_EDITOR
        // 编辑器下的模拟模式
        var createParameters = new EditorSimulateModeParameters();
        createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
        initializationOperation = defaultPackage.InitializeAsync(createParameters);
#else
        // 单机运行模式
        var createParameters = new OfflinePlayModeParameters();
        initializationOperation = defaultPackage.InitializeAsync(createParameters);
#endif

        yield return initializationOperation;

        if (initializationOperation.Status == EOperationStatus.Succeed)
        {
            Debug.Log("YooAsset初始化成功");
            isInitialized = true;
        }
        else
        {
            Debug.LogError($"YooAsset初始化失败: {initializationOperation.Error}");
        }
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="assetName">资源名称</param>
    /// <returns>加载的资源</returns>
    public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
    {
        if (!isInitialized)
        {
            Debug.LogError("YooAsset未初始化，请先调用Initialize方法");
            return null;
        }

        if (defaultPackage == null)
        {
            Debug.LogError("默认资源包未找到");
            return null;
        }

        var handle = defaultPackage.LoadAssetSync<T>(assetName);
        if (handle.Status == EOperationStatus.Succeed)
        {
            var asset = handle.AssetObject as T;
            if (asset is GameObject)
            {
                return GameObject.Instantiate(asset);
            }
            return asset;
        }
        else
        {
            Debug.LogError($"加载资源失败: {assetName}, 错误: {handle.LastError}");
            return null;
        }
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="assetName">资源名称</param>
    /// <param name="callback">回调函数</param>
    public void LoadAssetAsync<T>(string assetName, System.Action<T> callback) where T : UnityEngine.Object
    {
        if (!isInitialized)
        {
            Debug.LogError("YooAsset未初始化，请先调用Initialize方法");
            callback?.Invoke(null);
            return;
        }

        if (defaultPackage == null)
        {
            Debug.LogError("默认资源包未找到");
            callback?.Invoke(null);
            return;
        }

        var handle = defaultPackage.LoadAssetAsync<T>(assetName);
        handle.Completed += (operationHandle) =>
        {
            if (operationHandle.Status == EOperationStatus.Succeed)
            {
                var asset = operationHandle.AssetObject as T;
                if (asset is GameObject)
                {
                    callback?.Invoke(GameObject.Instantiate(asset));
                }
                else
                {
                    callback?.Invoke(asset);
                }
            }
            else
            {
                Debug.LogError($"异步加载资源失败: {assetName}, 错误: {operationHandle.LastError}");
                callback?.Invoke(null);
            }
        };
    }

    /// <summary>
    /// 检查资源是否存在
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <returns>是否存在</returns>
    public bool HasAsset(string assetName)
    {
        if (!isInitialized || defaultPackage == null)
            return false;

        return defaultPackage.CheckLocationValid(assetName);
    }

    /// <summary>
    /// 释放未使用的资源
    /// </summary>
    public void UnloadUnusedAssets()
    {
        if (!isInitialized || defaultPackage == null)
            return;

        defaultPackage.UnloadUnusedAssets();
    }

    /// <summary>
    /// 强制释放所有资源    
    /// </summary>
    public void ForceUnloadAllAssets()
    {
        if (!isInitialized || defaultPackage == null)
            return;

        defaultPackage.ForceUnloadAllAssets();
    }

    /// <summary>
    /// 获取默认资源包
    /// </summary>
    /// <returns>默认资源包</returns>
    public ResourcePackage GetDefaultPackage()
    {
        return defaultPackage;
    }
}