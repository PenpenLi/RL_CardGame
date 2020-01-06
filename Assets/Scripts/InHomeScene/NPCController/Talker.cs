using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talker : MonoBehaviour
{
    #region NPC信息设置
    [SerializeField]
    private TalkerInfo info;
    public TalkerInfo Info { get { return info; } }

    public string TalkerID 
    {
        get
        {
            if (Info) return Info.ID;
            return string.Empty;
        }
    }
    public string TalkerName
    {
        get
        {
            if (Info) return Info.Name;
            return string.Empty;
        }
    }
    public TalkerData Data { get; private set; }
    public List<Quest> QuestInstances
    {
        get { return Data.questInstances; }
    }
    public virtual void OnTalkBegin()
    {
        Data.OnTalkBegin();
    }
    public virtual void OnTalkFinished()
    {
        Data.OnTalkFinished();
    }
    #endregion

    public void Init()
    {
        if (!info) return;

        Data = new TalkerData();
        Data.info = Info;
        if (Info.IsQuestGiver)
        {
            Data.InitQuest(info.QuestsStored);
            Debug.Log("任务-" + TalkerName + "---" + Data.questInstances.Count);
        }

    }


}


public class TalkerData
{
    public TalkerInfo info;
    public string TalkID
    {
        get
        {
            if (info) return info.ID;
            return string.Empty;
        }
    }
    public string TalkerName
    {
        get
        {
            if (info) return info.Name;
            return string.Empty;
        }
    }

    public Relationship relationshipInstance;

    #region 对话任务与监听器
    public List<TalkObjective> objectivesTalkToThis = new List<TalkObjective>();
    public List<SubmitObjective> objectivesSubmitToThis = new List<SubmitObjective>();

    public delegate void DialogueListener();
    public event DialogueListener OnTalkBeginEvent;
    public event DialogueListener OnTalkFinishedEvent;

    public List<Quest> questInstances = new List<Quest>();

    public virtual void OnTalkBegin()
    {
        OnTalkBeginEvent?.Invoke();
    }

    public virtual void OnTalkFinished()
    {
        OnTalkFinishedEvent?.Invoke();
    }
    #endregion

    public void InitQuest(List<Quest> questsStored)
    {
        if (questsStored == null) return;
        if (questInstances.Count > 0) questInstances.Clear();
        foreach(Quest quest in questsStored)
        {
            if (quest)
            {
                Quest questInstance = Object.Instantiate(quest);
                foreach (CollectObjective co in questInstance.CollectObjectives)
                    questInstance.ObjectiveInstances.Add(co);
                foreach (KillObjective ko in questInstance.KillObjectives)
                    questInstance.ObjectiveInstances.Add(ko);
                foreach (TalkObjective to in questInstance.TalkObjectives)
                    questInstance.ObjectiveInstances.Add(to);
                foreach (SubmitObjective so in questInstance.SubmitObjectives)
                    questInstance.ObjectiveInstances.Add(so);
                foreach (CustomObjective cuo in questInstance.CustomObjectives)
                    questInstance.ObjectiveInstances.Add(cuo);
                questInstance.ObjectiveInstances.Sort((x, y) =>
                {
                    if (x.OrderIndex > y.OrderIndex) return 1;
                    else if (x.OrderIndex < y.OrderIndex) return -1;
                    else return 0;
                });
                if (quest.CmpltObjctvInOrder)
                {
                    for(int i = 1; i < questInstance.ObjectiveInstances.Count; i++)
                    {
                        if(questInstance.ObjectiveInstances[i].OrderIndex
                            >= questInstance.ObjectiveInstances[i - 1].OrderIndex)
                        {
                            questInstance.ObjectiveInstances[i].PrevObjective
                                = questInstance.ObjectiveInstances[i - 1];
                            questInstance.ObjectiveInstances[i - 1].NextObjective
                                = questInstance.ObjectiveInstances[i];
                        }
                    }
                }
                int i0, i1, i2, i3, i4;
                i0 = i1 = i2 = i3 = i4 = 0;
                foreach(Objective o in questInstance.ObjectiveInstances)
                {
                    if(o is CollectObjective)
                    {
                        o.runtimeID = questInstance.ID + "_CO" + i0;
                        i0++;
                    }
                    if(o is KillObjective)
                    {
                        o.runtimeID = questInstance.ID + "_KO" + i1;
                        i1++;
                    }
                    if (o is TalkObjective)
                    {
                        o.runtimeID = questInstance.ID + "_TO" + i2;
                        i2++;
                    }
                    if(o is SubmitObjective)
                    {
                        o.runtimeID = questInstance.ID + "_SO" + i3;
                        i3++;
                    }
                    if(o is CustomObjective)
                    {
                        o.runtimeID = questInstance.ID + "_CUO" + i4;
                        i4++;
                    }
                    o.runtimeParent = questInstance;
                }
                questInstance.originalQuestHolder = this;
                questInstance.currentQuestHolder = this;
                questInstances.Add(questInstance);
            }
        }
    }

    public void TransferQuestHolder(Quest quest)
    {
        if (!quest) return;
        questInstances.Add(quest);
        quest.currentQuestHolder.questInstances.Remove(quest);
        quest.currentQuestHolder = this;
    }
    public static implicit operator bool (TalkerData self)
    {
        return self != null;
    }
}