using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class HospitalPanel : BasicSubMenuPanel
{
    public Transform PerSickBed;
    public ScrollRect SB_SR;
    public HEWPageController pageController;
    //[HideInInspector]
    public UseTimeItem SickBed(string id)
    {
        UseTimeItem[] all = SB_SR.content.GetComponentsInChildren<UseTimeItem>();
        foreach(UseTimeItem t in all)
        {
            if(t.taskId == id)
            {
                return t;
            }
        }
        return null;
    }
    [HideInInspector]
    public List<GDEtimeTaskData> currentTreatingList = new List<GDEtimeTaskData>();
    public string currentSickBedId;
    private float refreshInterval = 0.25f;
    float CD;
    [HideInInspector]
    public bool whenShowTHisPanel = false;

    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        currentSickBedId = getTaskIdFromIndex(0);
        initAllSickBeds();
        resetSBSR();
        if (SickBed(currentSickBedId).isEmpty)
        {
            initHospital();
        }
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
        SDGameManager.Instance.heroSelectType = SDConstants.HeroSelectType.Hospital;
        pageController.ItemsInit(SDConstants.ItemType.Hero);

        //
        //initAllSickBeds();
        refreshAllSickBeds();

    }
    public void initAllSickBeds()
    {
        List<GDEtimeTaskData> allList = SDDataManager.Instance.PlayerData.TimeTaskList;

        int sickbedNum = SDDataManager.Instance.PlayerData.sickBedNum;
        if (sickbedNum < 1)
        {
            SDDataManager.Instance.PlayerData.sickBedNum = 1;
            sickbedNum = 1;
        }
        currentTreatingList = allList.FindAll(x => x.taskId.Contains("TT_HOSP"));

        UseTimeItem[] all = SB_SR.content.GetComponentsInChildren<UseTimeItem>();
        for (int i =0; i< all.Length; i++)
        {
            if (string.IsNullOrEmpty(all[i].taskId))
            {
                all[i].taskId = string.Format("TT_HOSP#{0:D2}", i);
            }
            if (i < sickbedNum)
            {
                all[i].isUnlocked = true;
            }
            else
            {
                all[i].showLockedPanel();
                continue;
            }

            string fixId = getTaskIdFromIndex(i);

            GDEtimeTaskData tt = null;
            foreach(GDEtimeTaskData task in currentTreatingList)
            {
                if(task.taskId == fixId)
                {
                    tt = task;
                    break;
                }
            }
            if (tt!=null)
            {
                all[i].initTimeTask(tt);
            }
            else
            {
                all[i].showEmptyPanel();
            }
        }
    }
    public void refreshAllSickBeds()
    {
        UseTimeItem[] all = SB_SR.content.GetComponentsInChildren<UseTimeItem>();
        for(int i = 0; i < all.Length; i++)
        {
            if (all[i].isUnlocked)
            {
                if (!all[i].isEmpty)
                {
                    all[i].refreshTimeCondition();
                }
            }
        }
        CD = refreshInterval;
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
        string k = index.ToString();
        while (k.Length < 2)
        {
            k = 0 + k;
        }
        string fixId = "TT_HOSP#" + k;
        return fixId;
    }
}
