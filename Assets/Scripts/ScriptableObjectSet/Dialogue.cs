using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName ="dialogue",menuName ="Wun/剧情/对话")]
public class Dialogue : ScriptableObject
{
    [SerializeField]
    private string _ID;
    public string ID
    {
        get
        {
            return _ID;
        }
    }

    [SerializeField]
    private bool useUnifiedNPC;
    public bool UseUnifiedNPC
    {
        get
        {
            return useUnifiedNPC;
        }
    }

    [SerializeField]
    private bool useTalkerInfo;
    public bool UseTalkerInfo
    {
        get
        {
            return useTalkerInfo;
        }
    }
    
    [SerializeField]
    private TalkerInfo unifiedNPC;
    public TalkerInfo UnifiedNPC
    {
        get
        {
            return unifiedNPC;
        }
    }

    [SerializeField]
    private List<DialogueWords> words = new List<DialogueWords>();
    public List<DialogueWords> Words
    {
        get
        {
            return words;
        }
    }

    public int IndexOfWords(DialogueWords words)
    {
        return Words.IndexOf(words);
    }
}
[Serializable]
public class DialogueWords
{
    public string TalkerName
    {
        get
        {
            if (TalkerType == TalkerType.NPC)
                if (TalkerInfo)
                    return TalkerInfo.Name;
                else return string.Empty;
            else return "玩家角色";
        }
    }

    [SerializeField]
    private TalkerType talkerType;
    public TalkerType TalkerType
    {
        get
        {
            return talkerType;
        }
    }

    [SerializeField]
    private TalkerInfo talkerInfo;
    public TalkerInfo TalkerInfo
    {
        get
        {
            return talkerInfo;
        }
    }

    [SerializeField, TextArea(3, 10)]
    private string words;
    public string Words
    {
        get
        {
            return words;
        }
    }

    [SerializeField]
    private int indexOfCorrectOption;
    public int IndexOfCorrectOption
    {
        get
        {
            return indexOfCorrectOption;
        }
    }


}


public enum WordsOptionType
{
    BranchWords,
    BranchDialogue,
    Choice,
    SubmitAndGet,
    OnlyGet
}
public enum TalkerType
{
    NPC,
    Player
}
