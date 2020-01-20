using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class MissionCard : MonoBehaviour
{
    public Image MissionImg;
    public Image NPCImg;
    public Text MissionNameText;
    public Text MissionDetailText;
    public ScrollRect RewardSR;
    public Image CardCondition;
    //public ROTaskData DATA;
    public QuestGroup _questGroup;
    public Quest quest;
    public MissionPanel MP;
#if UNITY_EDITOR
    [Space,DisplayName("任务进度")]
#endif
    public Transform Slider;

    public Transform Locked;
    public Transform Completed;

    public void initMissionCard(Quest _quest)
    {
        quest = _quest;
        //
        _questGroup = quest.Group;
        MissionNameText.text = quest.TITLE;
        MissionDetailText.text = quest.DESC;
        //
        refreshMisionState();
    }
    public void refreshMisionState()
    {
        if (!quest.AcceptAble)//未满足接取条件
        {
            GetComponent<Button>().interactable = false;
            Locked.gameObject.SetActive(true);
            return;
        }
        Locked.gameObject.SetActive(false);
        GetComponent<Button>().interactable = true;
        bool OG = QuestManager.Instance.HasOngoingQuestWithID(quest.ID);
        bool CP = QuestManager.Instance.HasCompleteQuestWithID(quest.ID);
        Outline L = GetComponent<Outline>();

        Slider.gameObject.SetActive(false);
        if (!OG && !CP)//未接取任务
        {
            L.enabled = false;
        }
        else if(OG && !CP)//正在运行
        {
            L.enabled = true;
            L.effectColor = new Color(255, 250, 200, 102);
            float Progress = QuestManager.Instance.QuestProgressWithId(quest.ID);
            Slider.gameObject.SetActive(true);
            Slider.localScale = new Vector3(Progress, 1, 1);
        }
        else if(OG&&CP)//可以领取奖励
        {
            L.enabled = true;
            L.effectColor = new Color(135, 255, 0, 102);
        }
        else if (!OG && CP)//任务已经完成
        {
            Completed.gameObject.SetActive(true);

        }
    }
    public void BtnTapped()
    {
        if (!quest.AcceptAble)
        {
            GetComponent<Button>().interactable = false;
            return;
        }
        bool OG = QuestManager.Instance.HasOngoingQuestWithID(quest.ID);
        bool CP = QuestManager.Instance.HasCompleteQuestWithID(quest.ID);
        if(!OG&& !CP)//还未接取
            AcceptMission();
        else if (OG && !CP)//正在运行
        {

        }
        else if (OG && CP)//可以领取奖励
        {
            CompleteMission();
        }
        else if (!OG && CP)//任务已经完成
        {

        }

        refreshMisionState();
    }

    public void AcceptMission()
    {
        //PopoutController.CreatePopoutMessage("接取任务[" + quest.TITLE + "]",50);
        QuestManager.Instance.AcceptQuest(quest);
    }

    public void CompleteMission()
    {
        QuestManager.Instance.CompleteQuest(quest);
    }
}
