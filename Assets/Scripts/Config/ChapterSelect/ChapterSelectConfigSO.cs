using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterSelectConfigSO", menuName = "Game/ChapterSelectConfig")]
public class ChapterSelectConfigSO : ScriptableObject
{
    public List<ChapterSelectConfig> chapterSelectConfigs;
}
