using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using GameDataEditor;
using System;

public class FactoryWorkCard : UseTimeItem
{
    [Header("ProductingCardMonopoly"),Space]
    public ScrollRect ProductScrollrect;
    [Space,Header("NPC_Set")]
    public Button NPCPlace;
    public Image NPCFace;
    public Text NPCMessage;
    #region NPC_Empty
    public Transform NPC_emptyPanel;
    private bool _NPCIsEmpty;
    public bool NPCIsEmpty
    {
        get { return _NPCIsEmpty; }
        set
        {
            if (_NPCIsEmpty != value)
            {
                _NPCIsEmpty = value;
            }
            NPC_emptyPanel.gameObject.SetActive(_NPCIsEmpty);
        }
    }
    #endregion
    #region NPC_Locked
    public Transform NPC_lockedPanel;
    private bool _NPCIsLocked;
    public bool NPCIsLocked
    {
        get { return _NPCIsLocked; }
        set
        {
            if (_NPCIsLocked != value)
            {
                _NPCIsLocked = value;
            }
            NPC_lockedPanel.gameObject.SetActive(_NPCIsLocked);
        }
    }
    #endregion
    //public UseTimeItem UTI;
    public FactoryPanel FP
    {
        get { return GetComponentInParent<FactoryPanel>(); }
    }
    public List<string> CurrentRewardList = new List<string>();
    //public 
    public override void refreshTimeCondition()
    {
        //base.refreshTimeCondition();
        GDEtimeTaskData TASK = SDDataManager.Instance.GetTimeTaskById(taskId);
        if(TASK!=null && DateTime.TryParse(TASK.startTime, out DateTime starttime))
        {
            TimeSpan span = DateTime.Now - starttime;
            int m = span.Minutes % TASK.timeType;
            float s = Mathf.Min(m * 1f / TASK.timeType, 1);
            timeSlider.localScale = new Vector3(s, 1, 1);
        }
    }
    public override void btnTapped()
    {
        //base.btnTapped();
        if (isUnlocked)
        {
            if (!isEmpty)
            {
                GDEtimeTaskData task = SDDataManager.Instance.GetTimeTaskById(taskId);
                string itemId = task.itemId;
                int oldD = task.oldData;
                if (SDDataManager.Instance.FinishTimeTask(taskId))
                {
                    CurrentRewardList = new List<string>();
                    CurrentRewardList.Add(itemId);
                    for(int i = 0; i < FP.AllProducts.Count; i++)
                    {
                        float r = oldD * 1f / (oldD + i + 2);
                        if (UnityEngine.Random.Range(0,1f)<r)
                        {
                            CurrentRewardList.Add(FP.AllProducts[i].ID);
                            SDDataManager.Instance.addConsumable(FP.AllProducts[i].ID);
                        }
                    }
                    Debug.Log("成功获取工厂成果");
                    string re = CurrentRewardList[0];
                    for(int i = 1; i < CurrentRewardList.Count; i++)
                    {
                        re += " & " + CurrentRewardList[i];
                    }
                    PopoutController.CreatePopoutMessage("获取奖励" + re, 10);
                }
            }
        }
    }
    public void BtnToOpenSlavePanel()
    {
        if (!FP) return;
        FP.chooseOneAssemblyLine(this);
    }

    public void ChangeWorkingNPC(int newHashcode)
    {

    }

    public override void initTimeTask(GDEtimeTaskData task)
    {
        base.initTimeTask(task);
        GDENPCData slave = SDDataManager.Instance.GetNPCOwned(task.itemHashcode);
        if (slave != null)
        {
            int lv = SDDataManager.Instance.getLevelByExp(slave.exp);
            int like = SDDataManager.Instance.getLikeByLikability
                (slave.likability,out float rate);
            NPCMessage.text = lv + "·" + like;
        }
    }
}
