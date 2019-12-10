using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                if (SDDataManager.Instance.FinishTimeTask(taskId))
                {
                    Debug.Log("成功获取工厂成果");
                }
            }
        }
    }
    public void BtnToOpenSlavePanel()
    {
        if (!FP) return;

    }

    public void ChangeWorkingNPC(int newHashcode)
    {

    }
}
