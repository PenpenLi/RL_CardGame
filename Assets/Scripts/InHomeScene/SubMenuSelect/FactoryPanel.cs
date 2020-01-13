using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class FactoryPanel : BasicSubMenuPanel
{
    [Space(25)]
    public ScrollRect scrollrect;
    public Transform WorkingCard;
    public List<consumableItem> AllProducts;
    public List<FactoryWorkCard> ALLAssemblyLines;
    [Space]
    public Transform chooseSlavePanel;
    public HEWPageController SlavePage;
    public FactoryWorkCard showingCard;
    [Space]
    public string SelectedTaskId;
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        initAllAssemblyLines();
        chooseSlavePanel.gameObject.SetActive(false);
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
        string fixId = "TT_FACT#" + k;
        return fixId;
    }



    public void initAllAssemblyLines()
    {
        ResetAllLines();
        for(int i = 0; i < Level + 2; i++)
        {
            Transform card = Instantiate(WorkingCard) as Transform;
            card.SetParent(scrollrect.content);
            card.transform.localScale = Vector3.one;
            card.gameObject.SetActive(true);
            FactoryWorkCard fwc = card.GetComponent<FactoryWorkCard>();
            string taskId = string.Format("TT_FACT#{0:D2}", i);
            GDEtimeTaskData TD;
            SDDataManager.Instance.haveTimeTaskByTaskId(taskId, out TD);
            if (TD == null)
            {
                SDDataManager.Instance.AddTimeTask
                    (SDConstants.timeTaskType.FACT, 0, AllProducts[0].ID,taskId);
                SDDataManager.Instance.haveTimeTaskByTaskId(taskId, out TD);
            }
            fwc.initTimeTask(TD);
            ALLAssemblyLines.Add(fwc);
        }
    }
    public void InitThisFactoryTasks()
    {
        for(int i = 0; i < Level + 2; i++)
        {
            string taskId = string.Format("TT_FACT#{0:D2}", i);
            GDEtimeTaskData TD;
            SDDataManager.Instance.haveTimeTaskByTaskId(taskId, out TD);
            if (TD == null)
            {
                SDDataManager.Instance.AddTimeTask
                    (SDConstants.timeTaskType.FACT, 0, AllProducts[0].ID, taskId);
                SDDataManager.Instance.haveTimeTaskByTaskId(taskId, out TD);
            }
        }
    }

    public void ResetAllLines()
    {
        foreach(FactoryWorkCard fwc in ALLAssemblyLines)
        {
            Destroy(fwc.gameObject);
        }
        ALLAssemblyLines.Clear();
    }

    public void chooseOneAssemblyLine(FactoryWorkCard card)
    {
        if(SDDataManager.Instance.haveTimeTaskByTaskId(card.taskId, out GDEtimeTaskData task))
        {
            SelectedTaskId = card.taskId;
            UIEffectManager.Instance.showAnimFadeIn(chooseSlavePanel);

            refreshThisAssemblyLine(task);

        }
    }
    public void refreshThisAssemblyLine(GDEtimeTaskData task)
    {
        //展示列表
        showingCard.initTimeTask(task);

        SlavePage.ItemsInit(SDConstants.ItemType.NPC);
        foreach (SingleItem s in SlavePage.items)
        {
            if (s.itemHashcode == task.itemHashcode)
            {
                s.isSelected = true;
                break;
            }
        }
    }
}
