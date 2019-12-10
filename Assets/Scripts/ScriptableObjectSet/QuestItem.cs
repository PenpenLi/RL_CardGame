using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="quest_item",menuName ="Wun/任务/任务道具",order = 3)]
public class QuestItem : ItemBase
{
    public QuestItem()
    {
        ItemType = SDConstants.ItemType.Quest;
        discardAble = false;
        sellAble = false;
    }
}
