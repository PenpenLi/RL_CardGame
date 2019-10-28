using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using System;
public class UseTimeItem : MonoBehaviour
{
    public Image bgImg;
    public Image frame;
    public Text aboveText;
    public Text midText;
    public Text belowText;
    public Transform timeSlider;
    public int wholeLastTime;
    //[HideInInspector]
    public int itemHashcode;
    public int itemId;

    public Transform emptyPanel;
    public Image statusImg;
    public SDConstants.ItemType type = SDConstants.ItemType.End;

    public void initItem(SDConstants.ItemType TYPE, int itemFigure, int wholeInterval)
    {
        //if (type == TYPE) return;//相同类型的呼唤
        wholeLastTime = wholeInterval;
        if (TYPE == SDConstants.ItemType.Hero)
        {
            itemHashcode = itemFigure;
            initHeroToTreat(itemHashcode);
        }
        else if (TYPE == SDConstants.ItemType.AllEquip)
        {
            
        }
    }

    public void initHeroToTreat(int hashcode)
    {
        GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(hashcode);
        DateTime startTime = DateTime.Now;
        int id = SDDataManager.Instance.PlayerData.TimeTaskList.Count;
        GDEtimeTaskData task = new GDEtimeTaskData(GDEItemKeys.timeTask_emptyTimeTask)
        {
            taskId = id
            ,
            itemChara = hashcode
            ,
            taskType = 0//确认该角色为英雄
            ,
            startTime = startTime.ToString()
        };
        SDDataManager.Instance.PlayerData.TimeTaskList.Add(task);
        SDDataManager.Instance.PlayerData.Set_TimeTaskList();
    }
}
