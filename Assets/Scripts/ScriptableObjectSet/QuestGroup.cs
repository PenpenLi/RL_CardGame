using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="quest_group",menuName ="Wun/任务/任务组",order =2)]
public class QuestGroup : ScriptableObject
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
    private string _Name;
    public string Name
    {
        get
        {
            return _Name;
        }
    }
}
