using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel_
{
    public Button btnBack;
    public Button btnSure;

    public Toggle togMusic;
    public Toggle togSound;

    public Slider sliderMusic;
    public Slider sliderSound;

    private MusicModel musicModel;

    private bool oldTogMusic = false;
    private bool oldTogSound = false;
    private float oldMusicValue = 0f;
    private float oldSoundValue = 0f;


    public override void Init()
    {
        // 读取本地设置
        musicModel = JsonMgr.Instance.LoadData<MusicModel>("MusicSetting");

        oldMusicValue = musicModel.musicVolume;
        oldSoundValue = musicModel.soundVolume;
        oldTogMusic = musicModel.isMusicOpen;
        oldTogSound = musicModel.isSoundOpen;

        // 刷新UI
        sliderMusic.value = musicModel.musicVolume;
        sliderSound.value = musicModel.soundVolume;
        togMusic.isOn = musicModel.isMusicOpen;
        togSound.isOn = musicModel.isSoundOpen;

        // 设置初始音量和静音
        MusicMgr.GetInstance().ChangeBKValue(musicModel.musicVolume);
        MusicMgr.GetInstance().ChangeSoundValue(musicModel.soundVolume);
        MusicMgr.GetInstance().SetBKMusicMute(!musicModel.isMusicOpen);
        MusicMgr.GetInstance().SetSoundMute(!musicModel.isSoundOpen);

        // 监听音量滑动
        sliderMusic.onValueChanged.AddListener((value) =>
        {
            musicModel.musicVolume = value;
            MusicMgr.GetInstance().ChangeBKValue(value);
        });
        sliderSound.onValueChanged.AddListener((value) =>
        {
            musicModel.soundVolume = value;
            MusicMgr.GetInstance().ChangeSoundValue(value);
        });

        // 监听开关
        togMusic.onValueChanged.AddListener((isOn) =>
        {
            musicModel.isMusicOpen = isOn;
            MusicMgr.GetInstance().SetBKMusicMute(!isOn);
        });
        togSound.onValueChanged.AddListener((isOn) =>
        {
            musicModel.isSoundOpen = isOn;
            MusicMgr.GetInstance().SetSoundMute(!isOn);
        });

        btnBack.onClick.AddListener(() =>
        {
            RevertSetting();
            UIMgr.Instance.HidePanel<SettingPanel>();
        });

        btnSure.onClick.AddListener(() =>
        {
            SaveSetting();
            UIMgr.Instance.HidePanel<SettingPanel>();
        });
    }

    private void SaveSetting()
    {
        JsonMgr.Instance.SaveData(musicModel, "MusicSetting");
    }

    private void RevertSetting()
    {
        sliderMusic.value = oldMusicValue;
        sliderSound.value = oldSoundValue;
        togMusic.isOn = oldTogMusic;
        togSound.isOn = oldTogSound;

        MusicMgr.GetInstance().ChangeBKValue(oldMusicValue);
        MusicMgr.GetInstance().ChangeSoundValue(oldSoundValue);
        MusicMgr.GetInstance().SetBKMusicMute(!oldTogMusic);
        MusicMgr.GetInstance().SetSoundMute(!oldTogSound);

        musicModel.musicVolume = oldMusicValue;
        musicModel.soundVolume = oldSoundValue;
        musicModel.isMusicOpen = oldTogMusic;
        musicModel.isSoundOpen = oldTogSound;
    }
}
