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
    public string startTimeString;
    public DateTime startTimeDatetime;
    public int taskType;
    public int timeType;
    //[HideInInspector]
    public int itemHashcode;
    public int itemId;
    #region isEmpty
    bool _isempty;
    public bool isEmpty
    {
        get { return _isempty; }
        set { _isempty = value;
            emptyPanel?.gameObject.SetActive(_isempty);
        }
    }
    public Transform emptyPanel;
    #endregion
    #region isUnlocked
    bool _isunlocked;
    public bool isUnlocked
    {
        get { return _isunlocked; }
        set 
        {
            _isunlocked = value;
            lockedPanel?.gameObject.SetActive(!_isunlocked);
        }
    }
    public Transform lockedPanel;
    #endregion
    #region isFinished
    bool _isfinished;
    public bool isFinished
    {
        get { return _isfinished; }
        set
        {
            _isfinished = value;
            finishedPanel?.gameObject.SetActive(_isfinished);
        }
    }
    public Transform finishedPanel;
    #endregion
    public HospitalPanel HP
    {
        get { return GetComponentInParent<HospitalPanel>(); }
    }
    public Image statusImg;
    public SDConstants.ItemType type = SDConstants.ItemType.End;

    public void initTimeTask(GDEtimeTaskData task)
    {
        if(task == null)
        {
            showEmptyPanel();return;
        }
        taskType = task.taskType;
        if (taskType == 0)//英雄治疗任务
        {
            showTreatingHero(task);
        }
    }
    public void showTreatingHero(GDEtimeTaskData task)
    {
        itemHashcode = task.itemChara;
        startTimeString = task.startTime;
        timeType = task.timeType;
        if (DateTime.TryParse(startTimeString, out DateTime starttime))
        {
            startTimeDatetime = starttime;
            refreshTimeCondition();
        }
    }
    public void showEmptyPanel()
    {
        itemHashcode = 0;
        isEmpty = true;
    }
    public void showLockedPanel()
    {
        itemHashcode = 0;
        isUnlocked = false;
    }

    public void refreshTimeCondition()
    {
        if (!isFinished)
        {
            float useHour = timeType * 1f;
            TimeSpan span = DateTime.Now - startTimeDatetime;
            float s = (float)(span.TotalMinutes) / (useHour * 60);
            timeSlider.localScale = new Vector3(s, 1, 1);
            if (s >= 1) { isFinished = true; }
        }
        else
        {

        }

    }

    public void btnTapped()
    {
        if (isUnlocked)
        {
            HP.currentSickBedId = transform.GetSiblingIndex();
            if (!isEmpty)
            {
                if (!isFinished)
                {
                    //槽位任务正在进行中

                }
                else
                {
                    //槽位任务已完成
                    showEmptyPanel();
                    SDDataManager.Instance.setHeroStatus(itemHashcode, 0);
                    for(int i = 0; i < SDDataManager.Instance
                        .PlayerData.TimeTaskList.Count; i++)
                    {
                        GDEtimeTaskData task = SDDataManager.Instance
                        .PlayerData.TimeTaskList[i];
                        if (task.taskType == taskType && task.taskId == itemId)
                        {
                            SDDataManager.Instance
                                .PlayerData.TimeTaskList.Remove(task);
                            break;
                        }
                    }
                }
            }
            else
            {
                //槽位为空
                HP.initHospital();
            }
        }
        else
        {
            //槽位未解锁

        }
    }
}
