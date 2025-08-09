using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataMgr
{
    private static GameDataMgr instance = new GameDataMgr();
    public static GameDataMgr Instance => instance;
    public MusicModel musicModel;

    private GameDataMgr()
    {
        musicModel = JsonMgr.Instance.LoadData<MusicModel>("MusicModel");
    }

    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicModel, "MusicModel");
    }
}
