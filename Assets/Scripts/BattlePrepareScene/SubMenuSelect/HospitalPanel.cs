using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class HospitalPanel : MonoBehaviour
{
    HomeScene _hs;
    public HomeScene homeScene
    {
        get
        {
            if (_hs == null) _hs = FindObjectOfType<HomeScene>();
            return _hs;
        }

    }
    public Transform PerSickBed;
    public ScrollRect SB_SR;
    public HEWPageController pageController;
    //[HideInInspector]
    public UseTimeItem SickBed(int index)
    {
        UseTimeItem[] all = SB_SR.content.GetComponentsInChildren<UseTimeItem>();
        if (index < all.Length) { return all[index]; }
        else return null;
    }
    [HideInInspector]
    public List<GDEtimeTaskData> currentTreatingList = new List<GDEtimeTaskData>();
    public int currentSickBedId;
    private float refreshInterval = 0.25f;
    float CD;
    [HideInInspector]
    public bool whenShowTHisPanel = false;

    public void whenOpenThisPanel()
    {
        currentSickBedId = 0;
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
        currentTreatingList = new List<GDEtimeTaskData>();
        for(int i = 0; i < allList.Count; i++)
        {
            if(allList[i].taskType == 0)//确认任务为角色治疗任务
            {
                currentTreatingList.Add(allList[i]);
            }
        }

        UseTimeItem[] all = SB_SR.content.GetComponentsInChildren<UseTimeItem>();
        for (int i =0; i< all.Length; i++)
        {
            if (i < sickbedNum)
            {
                all[i].isUnlocked = true;
            }
            else
            {
                all[i].showLockedPanel();
                continue;
            }

            GDEtimeTaskData tt = null;
            foreach(GDEtimeTaskData task in currentTreatingList)
            {
                if(task.taskId == i)
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



    public void commonBackAction()
    {
        homeScene.SubMenuClose();
    }
}
