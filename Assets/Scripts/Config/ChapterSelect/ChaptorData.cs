using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaptorData
{
    public List<ChaptorInfo> chaptorInfos = new List<ChaptorInfo>();

    public ChaptorData() { }

    public ChaptorData(int chaptorCount)
    {
        for (int i = 0; i < chaptorCount; i++)
        {
            chaptorInfos.Add(new ChaptorInfo
            {
                unlocked = (i == 0),
                passed = false
            });
        }
    }

    public static void PassLevel(int currentChaptor)
    {
        ChaptorData chaptorData = SelectChapterPanel.chaptorData;
        if (chaptorData == null) return;

        chaptorData.chaptorInfos[currentChaptor].passed = true;
        if (currentChaptor + 1 < chaptorData.chaptorInfos.Count)
            chaptorData.chaptorInfos[currentChaptor + 1].unlocked = true;

        JsonMgr.Instance.SaveData(chaptorData, "chaptorData");
    }
}

public class ChaptorInfo
{
    public bool unlocked;
    public bool passed;
}
