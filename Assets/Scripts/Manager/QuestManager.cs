using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class QuestManager : PersistentSingleton<QuestManager>
{
    [SerializeField, Header("任务列表")]
    private List<Quest> questsOngoing = new List<Quest>();
    public List<Quest> QuestsOngoing
    {
        get { return questsOngoing; }
    }
#if UNITY_EDITOR
    [ReadOnly]
#endif
    [SerializeField]
    private List<Quest> questsComplete = new List<Quest>();
    public List<Quest> QuestsComplete
    {
        get { return questsComplete; }
    }

    //[SerializeField]
    private Quest selectedQuest;

    #region 任务处理相关
    /// <summary>
    /// 接取任务
    /// </summary>
    /// <param name="quest">要接取的任务</param>
    /// <param name="loadMode">是否读档模式</param>
    /// <returns></returns>
    public bool AcceptQuest(Quest quest, bool loadMode = false)
    {
        if(!quest || !quest.IsValid)
        {
            //PopoutController.CreatePopoutMessage("无效任务", 25);
            Debug.Log("无效任务");
            return false;
        }
        if (HasOngoingQuest(quest))
        {
            //PopoutController.CreatePopoutMessage("任务正在执行", 25);
            Debug.Log("任务正在执行");
            return false;
        }

        if (quest.Group)
        {

        }

        foreach(Objective o in quest.ObjectiveInstances)
        {
            if(o is CollectObjective)
            {
                CollectObjective co = o as CollectObjective;
                TriggerManager.Instance.OnGetItemEvent += co.UpdateCollectAmount;
                TriggerManager.Instance.OnLoseItemEvent += co.UpdateCollectAmountDown;

            }
            if(o is KillObjective)
            {
                KillObjective ko = o as KillObjective;
                TriggerManager.Instance.OnUnitDeathEvent += ko.UpdateKillAmount;
            }
            if (o is TalkObjective)
            {

            }
            if (o is SubmitObjective)
            {

            }
            if (o is CustomObjective)
            {
                CustomObjective cuo = o as CustomObjective;
                TriggerManager.Instance.OnTriggerSetEvent += cuo.UpdateTriggerState;
                if (cuo.CheckStateAtAcpt)
                    TriggerManager.Instance.SetTrigger(cuo.TriggerName
                        , TriggerManager.Instance.GetTriggerState(cuo.TriggerName));
            }
            o.OnStateChangeEvent += OnObjectiveStateChange;
        }
        quest.IsOngoing = true;
        QuestsOngoing.Add(quest);
        if (!quest.SbmtOnOriginalNPC)
        {
            
        }

        if (!loadMode)
        {
            PopoutController.CreatePopoutMessage("接取任务[" + quest.TITLE + "]", 50);
        }
        if (quest.ObjectiveInstances.Count > 0)
        {
            Objective firstObj = quest.ObjectiveInstances[0];
            //
        }
        return true;
    }
    /// <summary>
    /// 放弃任务
    /// </summary>
    /// <param name="quest">放弃的任务</param>
    /// <param name="loadMode">是否为读档模式</param>
    /// <returns></returns>
    public bool AbandonQuest(Quest quest,bool loadMode = false)
    {
        if(HasOngoingQuest(quest) && quest && quest.Abandonable)
        {
            quest.IsOngoing = false;
            QuestsOngoing.Remove(quest);
            foreach(Objective o in quest.ObjectiveInstances)
            {
                if(o is CollectObjective)
                {
                    CollectObjective co = o as CollectObjective;
                    TriggerManager.Instance.OnGetItemEvent += co.UpdateCollectAmount;
                    TriggerManager.Instance.OnLoseItemEvent += co.UpdateCollectAmountDown;
                }
                if(o is KillObjective)
                {
                    KillObjective ko = o as KillObjective;
                    ko.CurrentAmount = 0;
                    TriggerManager.Instance.OnUnitDeathEvent -= ko.UpdateKillAmount;
                }
                if(o is TalkObjective)
                {

                }
                if(o is SubmitObjective)
                {

                }
                if(o is CustomObjective)
                {
                    CustomObjective cuo = o as CustomObjective;
                    cuo.CurrentAmount = 0;
                    TriggerManager.Instance.OnTriggerSetEvent -= cuo.UpdateTriggerState;
                }
            }
            if (!quest.SbmtOnOriginalNPC)
            {

            }
            if (QuestsOngoing.Count < 1)
            {

            }
            return true;
        }
        else if (!quest.Abandonable)
        {
            PopoutController.CreatePopoutAlert("", "该任务无法放弃"
                , 50, true, PopoutController.PopoutWIndowAlertType.ConfirmMessage
                , null);
        }
        return false;
    }
    /// <summary>
    /// 放弃当前选中的任务
    /// </summary>
    public void AbandonSelectedQuest()
    {
        if (!selectedQuest) return;
        PopoutController.CreatePopoutAlert("","确认放弃该任务吗(已消耗道具不会退回)",50
            ,true,PopoutController.PopoutWIndowAlertType.ConfirmOrCancel
            , (PopoutController c,PopoutController.PopoutWindowAlertActionType a)
            =>
            {
                if(a == PopoutController.PopoutWindowAlertActionType.OnConfirm)
                {
                    AbandonQuest(selectedQuest);
                }
                else
                {
                    StartCoroutine(c.IEWaitAndDismiss(0.3f));
                }
            }
            );
    }   
    /// <summary>
    /// 完成任务
    /// </summary>
    /// <param name="quest">完成的任务</param>
    /// <param name="loadMode">是否为读档模式</param>
    /// <returns></returns>
    public bool CompleteQuest(Quest quest , bool loadMode = false)
    {
        if (!quest) return false;
        if(HasOngoingQuest(quest) && quest.IsComplete)
        {
            if (!loadMode)
            {
                foreach(ItemInfo rwi in quest.QuestItemReward.RewardItems)
                {
                    //if()
                }
                foreach(QuestItemReward.DictItemInfo dwi 
                    in quest.QuestItemReward.DictRewardItems)
                {
                    //if()
                }

                List<Quest> questsReqThisQuestItem = new List<Quest>();
                foreach(Objective o in quest.ObjectiveInstances)
                {
                    if(o is CollectObjective)
                    {
                        CollectObjective co = o as CollectObjective;
                        questsReqThisQuestItem = QuestRequiredItem(co.Item
                            , SDDataManager.Instance.GetItemAmount(co.Item.ID) - o.Amount)
                            .ToList();
                    }
                    if(questsReqThisQuestItem.Contains(quest) 
                        && questsReqThisQuestItem.Count > 1)
                        //需要道具的任务群包含该任务且数量多于一个，说明其他任务对该任务提交的道具存在依赖
                    {
                        PopoutController.CreatePopoutMessage
                            ("提交失败，其他任务对该任务提交物品有需求",50);
                        return false;
                    }
                }
            }
            quest.IsOngoing = false;
            QuestsOngoing.Remove(quest);
            RemoveQuestAgentByQuest(quest);
            //quest.current
            QuestsComplete.Add(quest);
            //
            foreach(Objective o in quest.ObjectiveInstances)
            {
                o.OnStateChangeEvent -= OnObjectiveStateChange;
                if(o is CollectObjective)
                {
                    CollectObjective co = o as CollectObjective;
                    TriggerManager.Instance.OnGetItemEvent += co.UpdateCollectAmount;
                    TriggerManager.Instance.OnLoseItemEvent += co.UpdateCollectAmountDown;
                    if (!loadMode && co.LoseItemAtSbmt)
                        SDDataManager.Instance.LoseItem(co.Item.ID, o.Amount);
                }
                if(o is KillObjective)
                {
                    KillObjective ko = o as KillObjective;
                    TriggerManager.Instance.OnUnitDeathEvent -= ko.UpdateKillAmount;
                }
                if(o is TalkObjective)
                {

                }
                if(o is SubmitObjective)
                {

                }
                if(o is CustomObjective)
                {
                    CustomObjective cuo = o as CustomObjective;
                    TriggerManager.Instance.OnTriggerSetEvent -= cuo.UpdateTriggerState;
                }
            }
            if (!loadMode)
            {
                SDDataManager.Instance.AddCoin(quest.RewardCoin);
                SDDataManager.Instance.AddDamond(quest.RewardDamond);
                SDDataManager.Instance.AddJiancai(quest.RewardJianCai);

                float R = UnityEngine.Random.Range(0, 1);
                if (R <= quest.QuestItemReward.GetPossible)
                {
                    foreach (ItemInfo info in quest.QuestItemReward.RewardItems)
                    {
                        SDDataManager.Instance.AddItem(info.ItemID, info.Amount);
                    }
                    foreach(QuestItemReward.DictItemInfo INFO 
                        in quest.QuestItemReward.DictRewardItems)
                    {
                        float _R = UnityEngine.Random.Range(0, 1);
                        if (_R <= INFO.Possibility)
                        {
                            SDDataManager.Instance.AddItem(INFO.ID, INFO.amount);
                        }
                    }
                }
                PopoutController.CreatePopoutMessage
                    ("完成任务 [" + quest.TITLE + "]", 50);
            }
            return true;
        }
        return false;
    }




    /// <summary>
    /// 追踪当前选择任务进行中的目标
    /// </summary>
    public void TraceSelectedQuest()
    {
        TraceQuest(selectedQuest);
    }
    public void TraceQuest(Quest quest)
    {
        if (!quest || !quest.IsValid) return;
        if (quest.IsComplete)
        {
            //在目标建筑显示提醒
        }
        else if(quest.ObjectiveInstances.Count>0)
            using (var objectiveEnum = quest.ObjectiveInstances.GetEnumerator())
            {
                Objective currentObj = null;
                List<Objective> concurrentObj = new List<Objective>();
                while (objectiveEnum.MoveNext())
                {
                    currentObj = objectiveEnum.Current;
                    if (!currentObj.IsComplete)
                    {
                        if (currentObj.Concurrent && currentObj.AllPrevObjCmplt)
                        {
                            if (!(currentObj is CollectObjective))
                                concurrentObj.Add(currentObj);
                        }
                        else break;
                    }
                }
                if (concurrentObj.Count > 0)
                {
                    int index = Random.Range(0, concurrentObj.Count);
                    //如果目标并发则UI则选一个
                    currentObj = concurrentObj[index];
                }
                if(currentObj is TalkObjective)
                {
                    TalkObjective to = currentObj as TalkObjective;

                }
                else if(currentObj is KillObjective)
                {
                    KillObjective ko = currentObj as KillObjective;
                    if (ko.Enemy != null)
                    {

                    }
                }

            }
    }
    private void OnObjectiveStateChange(Objective objective , bool stateBef)
    {
        Objective nextToDo = null;
        Quest quest = objective.runtimeParent;
        List<Objective> concurrentObj = new List<Objective>();
        for (int i = 0; i < quest.ObjectiveInstances.Count - 1; i++)
        {
            if (quest.ObjectiveInstances[i] == objective)
            {
                for (int j = i - 1; j > -1; j--)//往前找可以并发的目标
                {
                    Objective prevObj = quest.ObjectiveInstances[j];
                    if (!prevObj.Concurrent) break;//只要碰到一个不能并发的，就中断
                    else concurrentObj.Add(prevObj);
                }
                for (int j = i + 1; j < quest.ObjectiveInstances.Count; j++)//往后找可以并发的目标
                {
                    Objective nextObj = quest.ObjectiveInstances[j];
                    if (!nextObj.Concurrent)//只要碰到一个不能并发的，就中断
                    {
                        if (nextObj.AllPrevObjCmplt)
                            nextToDo = nextObj;//同时，若该不能并发目标的所有前置目标都完成了，那么它就是下一个要做的目标
                        break;
                    }
                    else concurrentObj.Add(nextObj);
                }
                break;
            }
        }
        if (!nextToDo)//当目标不能并发时此变量才不为空，所以此时表示所有后置目标都是可并发的，或者不存在后置目标
        {
            concurrentObj.RemoveAll(x => x.IsComplete);//把所有已完成的可并发目标去掉
            if (concurrentObj.Count > 0)//若还剩下可并发目标，则随机选一个作为下一个要做的目标
            {
                int index = Random.Range(0, concurrentObj.Count);
                nextToDo = concurrentObj[index];
            }
        }
        if (!stateBef && objective.IsComplete)//从没完成变为完成
        {
            //RemoveObjectiveIcon(objective);
            //CreateObjectiveIcon(nextToDo);
        }
    }

    #endregion

    #region 可视化相关
    public float QuestProgress(Quest quest)
    {
        if (HasCompleteQuestWithID(quest.ID)) return 1f;//已完成
        else if (!HasOngoingQuestWithID(quest.ID)) return 0f;//未开始
        //
        if (quest.ObjectiveInstances.Count > 0)
        {
            float current = 0;
            foreach(Objective o in quest.ObjectiveInstances)
            {
                float n = o.CurrentAmount * 1f / o.Amount;
                current += n;
            }
            return current * (1f / quest.ObjectiveInstances.Count);
        }
        return 0f;
    }
    public float QuestProgressWithId(string id)
    {
        if (HasCompleteQuestWithID(id)) return 1f;//已完成
        else if (!HasOngoingQuestWithID(id)) return 0f;//未开始
        //
        Quest quest = QuestsOngoing.Find(x => x.ID == id);
        if (quest.ObjectiveInstances.Count > 0)
        {
            float current = 0;
            foreach (Objective o in quest.ObjectiveInstances)
            {
                float n = o.CurrentAmount * 1f / o.Amount;
                current += n;
            }
            return current * (1f / quest.ObjectiveInstances.Count);
        }
        return 0f;
    }
    #endregion

    #region 其他工具
    /// <summary>
    /// 判断该任务是否处于进行中任务集合
    /// </summary>
    /// <param name="quest">该任务</param>
    /// <returns></returns>
    public bool HasOngoingQuest(Quest quest)
    {
        return QuestsOngoing.Contains(quest);
    }
    public bool HasOngoingQuestWithID(string questID)
    {
        return QuestsOngoing.Exists(x => x.ID == questID);
    }
    public bool HasCompleteQuest(Quest quest)
    {
        return QuestsComplete.Contains(quest);
    }
    public bool HasCompleteQuestWithID(string questID)
    {
        return QuestsComplete.Exists(x => x.ID == questID);
    }
    private void RemoveQuestAgentByQuest(Quest quest)
    {
        
    }

    public bool HasRequiredItem(ItemBase item,int leftAmount)
    {
        return QuestRequiredItem(item, leftAmount).Count() > 0;
    }
    public bool HasRequiredItem(string itemId, int leftAmount)
    {
        return QuestRequiredItem(itemId, leftAmount).Count() > 0;
    }
    private IEnumerable<Quest> QuestRequiredItem(ItemBase item, int leftAmount)
    {
        return QuestsOngoing.FindAll(x => x.RequiredItem(item, leftAmount)).AsEnumerable();
    }
    private IEnumerable<Quest> QuestRequiredItem(string itemId, int leftAmount)
    {
        return QuestsOngoing.FindAll(x => x.RequiredItem(itemId, leftAmount)).AsEnumerable();
    }
    #endregion
}
