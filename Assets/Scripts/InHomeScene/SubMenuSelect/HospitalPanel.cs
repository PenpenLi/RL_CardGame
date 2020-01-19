using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using System.Linq;

public class HospitalPanel : BasicSubMenuPanel
{
    public Transform PerSickBed;
    public HEWPageController pageController;
    [Space]
    public ScrollRect SB_SR;
    public List<UseTimeItem> AllHealSlots;
    public UseTimeItem SickBed(string id)
    {
        return AllHealSlots.Find(x => x.taskId == id);
    }
    public UseTimeItem SickBedIncludingEmpty(int index)
    {
        return AllHealSlots.Find(x => x.transform.GetSiblingIndex() == index);
    }
    [HideInInspector]
    public List<GDEtimeTaskData> currentTreatingList = new List<GDEtimeTaskData>();
    public string currentSickBedId;
    public int currentSelectedBedIndex;
    private float refreshInterval = 0.25f;
    float CD;
    [HideInInspector]
    public bool whenShowTHisPanel = false;

    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        currentSickBedId = getTaskIdFromIndex(0);
        initHospital();
    }
    public void resetSBSR()
    {
        StartCoroutine(IEResetSBSR());
    }
    public IEnumerator IEResetSBSR()
    {
        yield return new WaitForSeconds(0.05f);
        SB_SR.horizontalNormalizedPosition = 0;
    }
    public void initHospital()
    {
        initAllSickTasks();
        resetSBSR();
        SDGameManager.Instance.heroSelectType = SDConstants.HeroSelectType.Dying;
        pageController.ItemsInit(SDConstants.ItemType.Hero);
    }
    public void initAllSickTasks()
    {
        List<GDEtimeTaskData> allList = SDDataManager.Instance.PlayerData.TimeTaskList;

        int sickbedNum = SDDataManager.Instance.PlayerData.sickBedNum;
        if (sickbedNum < 1)
        {
            SDDataManager.Instance.PlayerData.sickBedNum = 1;
            sickbedNum = 1;
        }

        //读取并在AllHealSlot中构建任务
        currentTreatingList = allList.FindAll(x => x.taskId.Contains("TT_HOSP"));
        for (int i =0; i< AllHealSlots.Count; i++)
        {           
            if (i < sickbedNum)
            {
                AllHealSlots[i].isUnlocked = true;

                string fixId = getTaskIdFromIndex(i);

                GDEtimeTaskData tt = null;
                foreach (GDEtimeTaskData task in currentTreatingList)
                {
                    if (task.taskId == fixId)
                    {
                        tt = task;
                        break;
                    }
                }
                if (tt != null)
                {
                    AllHealSlots[i].initTimeTask(tt);
                }
                else
                {
                    AllHealSlots[i].showEmptyPanel();
                }
            }
            else
            {
                AllHealSlots[i].showLockedPanel();
                continue;
            }

        }
        //refreshAllSickBeds();
    }
    public void refreshAllSickBeds()
    { 
        for(int i = 0; i < AllHealSlots.Count; i++)
        {
            if (AllHealSlots[i].isUnlocked)
            {
                if (!AllHealSlots[i].isEmpty)
                {
                    AllHealSlots[i].refreshTimeCondition();
                }
            }
        }
        CD = refreshInterval;
    }
    public bool HaveEmptySickBed(out int MinIndex)
    {
        List<UseTimeItem> enables = AllHealSlots.FindAll(x => x.isUnlocked && x.isEmpty);
        if (enables.Count > 0)
        {
            MinIndex = enables.Min(x => x.transform.GetSiblingIndex());
            return true;
        }
        else
        {
            MinIndex = 0;
            return false;
        }
    }
    private void FixedUpdate()
    {
        if (CD > 0)
        {
            CD -= Time.deltaTime;return;
        }
        refreshAllSickBeds();
    }



    public override void commonBackAction()
    {
        base.commonBackAction();
        homeScene.SubMenuClose();
    }



    public string getTaskIdFromIndex(int index)
    {
        string fixId = string.Format("TT_HOSP#{0:D2}",index);
        return fixId;
    }
}
