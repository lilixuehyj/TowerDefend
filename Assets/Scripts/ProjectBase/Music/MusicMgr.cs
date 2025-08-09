using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicMgr : BaseManager<MusicMgr>
{
    //唯一的背景音乐组件
    private AudioSource bkMusic = null;
    //音乐大小
    private float bkValue = 1;

    //音效依附对象
    private GameObject soundObj = null;
    //音效列表
    private List<GameObject> soundList = new List<GameObject>();
    //音效大小
    private float soundValue = 1;
    //音效是否静音
    private bool isSoundMute = false;

    private bool isDestroyed = false;

    public MusicMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(Update);
    }

    public void InitMusicSettingFromLocal()
    {
        MusicModel musicModel = JsonMgr.Instance.LoadData<MusicModel>("MusicSetting");

        if (musicModel == null)
        {
            // 第一次运行，创建默认配置
            musicModel = new MusicModel()
            {
                musicVolume = 1f,
                soundVolume = 1f,
                isMusicOpen = true,
                isSoundOpen = true
            };
            JsonMgr.Instance.SaveData(musicModel, "MusicSetting");
        }

        Debug.Log("[MusicMgr] 成功初始化音乐设置");

        PlayBkMusic("BKMusic"); // 让设置和播放始终绑定

        // 设置音量与开关状态
        ChangeBKValue(musicModel.musicVolume);
        ChangeSoundValue(musicModel.soundVolume);
        SetBKMusicMute(!musicModel.isMusicOpen);
        SetSoundMute(!musicModel.isSoundOpen);
    }

    private void Update()
    {
        if (isDestroyed) return;

        try
        {
            for (int i = soundList.Count - 1; i >= 0; --i)
            {
                // 检查对象是否有效
                if (soundList[i] == null || soundList[i].Equals(null))
                {
                    // 对象已被销毁，从列表中移除
                    soundList.RemoveAt(i);
                    continue;
                }

                // 检查AudioSource组件是否存在
                AudioSource audioSource = soundList[i].GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    // AudioSource组件不存在，从列表中移除
                    soundList.RemoveAt(i);
                    continue;
                }

                // 检查是否还在播放
                if (!audioSource.isPlaying)
                {
                    try
                    {
                        PoolManager.GetInstance().Release(PoolType.Sound, soundList[i]);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"释放音效到对象池时出错: {e.Message}");
                    }
                    soundList.RemoveAt(i);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MusicMgr Update时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBkMusic(string name)
    {
        if (isDestroyed) return;

        try
        {
            if (bkMusic == null)
            {
                GameObject obj = new GameObject();
                obj.name = "BkMusic";
                bkMusic = obj.AddComponent<AudioSource>();
            }

            // 同步加载
            AudioClip clip = ResMgr.GetInstance().Load<AudioClip>("Music/BK/" + name);

            if (clip == null) return;

            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkValue;
            bkMusic.Play();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"PlayBkMusic时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBKMusic()
    {
        if (isDestroyed || bkMusic == null) return;

        try
        {
            bkMusic.Pause();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"暂停背景音乐时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBKMusic()
    {
        if (isDestroyed || bkMusic == null) return;

        try
        {
            bkMusic.Stop();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"停止背景音乐时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 改变背景音乐 音量大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBKValue(float v)
    {
        if (isDestroyed) return;

        bkValue = v;
        if (bkMusic == null) return;

        try
        {
            bkMusic.volume = bkValue;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"改变背景音乐音量时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(string name, bool isLoop = false, UnityAction<AudioSource> callBack = null)
    {
        if (isDestroyed) return;

        try
        {
            if (soundObj == null)
            {
                soundObj = new GameObject();
                soundObj.name = "Sound";
            }

            // 同步加载
            AudioClip clip = ResMgr.GetInstance().Load<AudioClip>("Music/Sound/" + name);
            if (clip == null) return;

            GameObject go = PoolManager.GetInstance().Get(PoolType.Sound, Vector3.zero);
            if (go != null)
            {
                go.transform.SetParent(soundObj.transform);
                AudioSource source = go.GetComponent<AudioSource>();
                if (source != null)
                {
                    source.clip = clip;
                    source.loop = isLoop;
                    source.volume = soundValue;
                    source.mute = isSoundMute;
                    source.Play();
                    soundList.Add(go);
                    callBack?.Invoke(source);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"PlaySound时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 改变音效声音大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundValue(float value)
    {
        if (isDestroyed) return;

        soundValue = value;

        try
        {
            for (int i = 0; i < soundList.Count; ++i)
            {
                if (soundList[i] != null && !soundList[i].Equals(null))
                {
                    AudioSource audioSource = soundList[i].GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        audioSource.volume = value;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"改变音效音量时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopSound(AudioSource source)
    {
        if (isDestroyed) return;

        try
        {
            for (int i = 0; i < soundList.Count; ++i)
            {
                if (soundList[i] != null && !soundList[i].Equals(null))
                {
                    AudioSource audioSource = soundList[i].GetComponent<AudioSource>();
                    if (audioSource == source)
                    {
                        try
                        {
                            PoolManager.GetInstance().Release(PoolType.Sound, soundList[i]);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"释放音效到对象池时出错: {e.Message}");
                        }
                        soundList.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"停止音效时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 设置背景音乐静音
    /// </summary>
    public void SetBKMusicMute(bool mute)
    {
        if (isDestroyed || bkMusic == null) return;

        try
        {
            bkMusic.mute = mute;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"设置背景音乐静音时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 设置所有音效静音
    /// </summary>
    public void SetSoundMute(bool mute)
    {
        if (isDestroyed) return;

        isSoundMute = mute;

        try
        {
            foreach (GameObject go in soundList)
            {
                if (go != null && !go.Equals(null))
                {
                    AudioSource audioSource = go.GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        audioSource.mute = mute;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"设置音效静音时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    public void Cleanup()
    {
        isDestroyed = true;

        try
        {
            // 清理音效列表
            soundList.Clear();

            // 清理背景音乐
            if (bkMusic != null)
            {
                bkMusic.Stop();
                bkMusic = null;
            }

            // 清理音效对象
            if (soundObj != null)
            {
                GameObject.Destroy(soundObj);
                soundObj = null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"清理MusicMgr时出错: {e.Message}");
        }
    }
}
