using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameDataEditor;

[System.Serializable]
[CreateAssetMenu(fileName ="quest",menuName ="Wun/任务/任务",order =1)]
public class Quest : ScriptableObject
{
    [SerializeField]
    private string _ID;
    public string ID { get { return _ID; } }


    [SerializeField,TextArea(1,1)]
    private string title;
    public string TITLE { get { return title; } }

    [SerializeField, TextArea(3,5)]
    private string _DESC;
    public string DESC { get { return _DESC; } }

    [SerializeField]
    private bool _Abandonable;
    public bool Abandonable { get { return _Abandonable; } }

    [SerializeField]
    private QuestGroup group;
    public QuestGroup Group
    {
        get
        {
            return group;
        }
    }

    [SerializeField,Space,Header("接取条件")]
    private List<QuestAcceptCondition> acceptConditions;
    public List<QuestAcceptCondition> AcceptConditions
    {
        get { return acceptConditions; }
    }
    [Space,Header("任务对话")]
    [SerializeField]
    private Dialogue beginDialogue;
    public Dialogue BeginDialogue
    {
        get
        {
            return beginDialogue;
        }
    }
    [SerializeField]
    private Dialogue ongoingDialogue;
    public Dialogue OngoingDialogue
    {
        get
        {
            return ongoingDialogue;
        }
    }
    [SerializeField]
    private Dialogue completeDialogue;
    public Dialogue CompleteDialogue
    {
        get
        {
            return completeDialogue;
        }
    }
    [Space,Header("任务奖励")]
    #region 货币类奖励
    [SerializeField]
    private int rewardCoin;
    public int RewardCoin
    {
        get
        {
            return rewardCoin;
        }
    }

    [SerializeField]
    private int rewardDamond;
    public int RewardDamond
    {
        get
        {
            return rewardDamond;
        }
    }

    [SerializeField]
    private int rewardJiancai;
    public int RewardJianCai
    {
        get { return rewardJiancai; }
    }

    [SerializeField]
    private int rewardEXP;
    public int RewardEXP
    {
        get
        {
            return rewardEXP;
        }
    }
    #endregion
    #region 道具类奖励
    [SerializeField]
    private QuestItemReward questItemReward;
    public QuestItemReward QuestItemReward
    {
        get { return questItemReward; }
    }

    #endregion
    [Space,Header("任务完成方式")]
    [SerializeField]
    private bool sbmtOnOriginalNPC = true;
    public bool SbmtOnOriginalNPC
    {
        get { return sbmtOnOriginalNPC; }
    }

    [SerializeField]
    private TalkerInfo _NPCToSubmit;
    public TalkerInfo NPCToSubmit
    {
        get
        {
            return _NPCToSubmit;
        }
    }

    [SerializeField]
    [Tooltip("勾选此项，则勾选InOrder的目标按OrderIndex从小到大的顺序执行" +
        "，若相同，则表示可以同时进行；若目标没有勾选InOrder，则表示该目标不受顺序影响。")]
    private bool cmpltObjctvInOrder = false;
    public bool CmpltObjctvInOrder
    {
        get
        {
            return cmpltObjctvInOrder;
        }
    }

    [System.NonSerialized]
    private List<Objective> objectiveInstances = new List<Objective>();
    //存储所有目标，在运行时用到，初始化时自动填，不用人为干预，详见QuestGiver类
    public List<Objective> ObjectiveInstances
    {
        get
        {
            return objectiveInstances;
        }
    }
    #region 全部任务目标类型
    [SerializeField]
    private List<CollectObjective> collectObjectives = new List<CollectObjective>();
    public List<CollectObjective> CollectObjectives
    {
        get { return collectObjectives; }
    }

    [SerializeField]
    private List<KillObjective> killObjectives = new List<KillObjective>();
    public List<KillObjective> KillObjectives
    {
        get { return killObjectives; }
    }

    [SerializeField]
    private List<TalkObjective> talkObjectives = new List<TalkObjective>();
    public List<TalkObjective> TalkObjectives
    {
        get { return talkObjectives; }
    }

    [SerializeField]
    private List<SubmitObjective> submitObjectives = new List<SubmitObjective>();
    public List<SubmitObjective> SubmitObjectives
    {
        get { return submitObjectives; }
    }

    [SerializeField]
    private List<CustomObjective> customObjectives = new List<CustomObjective>();
    public List<CustomObjective> CustomObjectives
    {
        get { return customObjectives; }
    }
    #endregion


    [HideInInspector]
    public TalkerData originalQuestHolder;
    [HideInInspector]
    public TalkerData currentQuestHolder;
    [HideInInspector]
    public bool IsOngoing { get; set; }//任务是否在执行,在运行时用到
    [HideInInspector]
    public bool IsComplete
    {
        get 
        {
            if (ObjectiveInstances.Exists(x => !x.IsComplete)) return false;
            return true;
        }
    }
    public bool IsValid
    {
        get 
        {
            if (ObjectiveInstances.Count < 1) return false;
            if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(TITLE)) return false;
            if (!SbmtOnOriginalNPC && (!NPCToSubmit)) return false;
            foreach (var co in CollectObjectives)
                if (!co.IsValid) return false;

            return true;
        }
    }
    public bool IsFinished
    {
        get
        {
            return IsComplete && !IsOngoing;
        }
    }
    public bool AcceptAble
    {
        get
        {
            foreach(QuestAcceptCondition qac in AcceptConditions)
            {
                if (!qac.IsEligible) return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 判断任务是否需要某个道具，用于丢弃某个道具时判断能不能丢
    /// </summary>
    /// <param name="item">该道具</param>
    /// <param name="leftAmount">该道具剩余数量</param>
    /// <returns></returns>
    public bool RequiredItem(ItemBase item,int leftAmount)
    {
        if (CmpltObjctvInOrder)
        {
            //当任务目标为收集时进行判断
            foreach(Objective o in ObjectiveInstances)
            {
                if (o.IsComplete && o.InOrder)
                {
                    //当剩余道具数量不足以维持该目标的完成状态时
                    if (o.Amount > leftAmount)
                    {
                        Objective tempObj = o.NextObjective;
                        while (tempObj != null)
                        {
                            //则判断是否有后置目标正在进行以保证打破该目标完成状态时后置目标不受影响
                            if (tempObj.CurrentAmount > 0 && tempObj.OrderIndex > o.OrderIndex)
                            {
                                return true;
                            }
                            tempObj = tempObj.NextObjective;
                        }
                    }
                    return false;
                }
                return false;
            }           
        }
        return false;
    }
    public bool RequiredItem(string itemId, int leftAmount)
    {
        if (CmpltObjctvInOrder)
        {
            //当任务目标为收集时进行判断
            foreach (Objective o in ObjectiveInstances)
            {
                if(o is CollectObjective)
                {
                    bool flag = false;
                    if ((o as CollectObjective).Item != null
                        && itemId == (o as CollectObjective).Item.ID) flag = true;
                    else if (itemId == (o as CollectObjective).ItemID) flag = true;

                    if (flag)
                        if (o.IsComplete && o.InOrder)
                        {
                            //当剩余道具数量不足以维持该目标的完成状态时
                            if (o.Amount > leftAmount)
                            {
                                Objective tempObj = o.NextObjective;
                                while (tempObj != null)
                                {
                                    //则判断是否有后置目标正在进行以保证打破该目标完成状态时后置目标不受影响
                                    if (tempObj.CurrentAmount > 0 && tempObj.OrderIndex > o.OrderIndex)
                                    {
                                        return true;
                                    }
                                    tempObj = tempObj.NextObjective;
                                }
                            }
                        }
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 是否在收集某个道具
    /// </summary>
    /// <param name="item">进行判断的道具</param>
    /// <returns>是否在收集</returns>
    public bool CollectingItem(ItemBase item)
    {
        return CollectObjectives.Exists(x => x.Item == item && !x.IsComplete);
    }


}

#region 任务奖励
[System.Serializable]
public class QuestItemReward
{
    [SerializeField, Range(0, 1)]
    private float _GetPossible = 1;
    public float GetPossible
    {
        get { return _GetPossible; }
    }

    [SerializeField]
    private List<ItemInfo> rewardItems = new List<ItemInfo>();
    public List<ItemInfo> RewardItems
    {
        get { return rewardItems; }
    }

    [SerializeField]
    private List<DictItemInfo> dictRewardItems = new List<DictItemInfo>();
    public List<DictItemInfo> DictRewardItems
    {
        get { return dictRewardItems; }
    }
    [System.Serializable]
    public class DictItemInfo
    {
        public string ID;
        public int amount;
        [Range(0,1)]
        public float Possibility = 1;
    }
}
#endregion

#region 任务条件
/// <summary>
/// 任务接收条件
/// </summary>
[System.Serializable]
public class QuestAcceptCondition
{
    [SerializeField]
#if UNITY_EDITOR
    [EnumMemberNames("无", "等级大于", "等级小于", "等级大于或等于", "等级小于或等于", "完成任务", "拥有道具")]
#endif
    private QuestCondition acceptCondition = QuestCondition.ComplexQuest;
    public QuestCondition AcceptCondition
    {
        get { return acceptCondition; }
    }

    [SerializeField]
    private int level;
    public int LEVEL
    {
        get { return level; }
    }

    [SerializeField]
    private Quest completeQuest;
    public Quest CompleteQuest
    {
        get { return completeQuest; }
    }

    [SerializeField]
    private ItemBase ownedItem;
    public ItemBase OwnedItem
    {
        get { return ownedItem; }
    }

    [SerializeField]
    private string ownedItemId;
    public string OwnedItemId
    {
        get { return ownedItemId; }
    }

    /// <summary>
    /// 是否符合条件
    /// </summary>
    public bool IsEligible
    {
        get
        {
            switch (AcceptCondition)
            {
                case QuestCondition.ComplexQuest:
                    return QuestManager.Instance.HasCompleteQuestWithID(CompleteQuest.ID);
                case QuestCondition.HasItem:
                    if (OwnedItem != null)
                    {
                        return SDDataManager.Instance.CheckIfHaveItemById(OwnedItem.ID);
                    }
                    else
                    {
                        return SDDataManager.Instance.CheckIfHaveItemById(OwnedItemId);
                    }
                default: return true;
            }
        }
    }
}
public enum QuestCondition
{
    None = 1,
    LevelLargeThen = 2,
    LevelLessThen = 4,
    LevelLargeOrEqualsThen = 8,
    LevelLessOrEqualsThen = 16,
    ComplexQuest = 32,
    HasItem = 64,
}
#endregion

#region 任务目标
public delegate void ObjectiveStateListener(Objective objective, bool cmpltStateBef);
/// <summary>
/// 任务目标
/// </summary>
[System.Serializable]
public abstract class Objective
{
    [HideInInspector]
    public string runtimeID;
    [HideInInspector]
    public Quest runtimeParent;
    [SerializeField]
    private string displayName;
    public string DisplayName { get { return displayName; } }

    [SerializeField]
    private bool display = true;
    public bool Display
    {
        get
        {
            if (runtimeParent && !runtimeParent.CmpltObjctvInOrder)
                return true;
            return display;
        }
    }

    [SerializeField]
    private int amount;
    public int Amount { get { return amount; } }

    private int currentAmount;
    public int CurrentAmount
    {
        get { return currentAmount; }
        set
        {
            bool befCmplt = IsComplete;
            if (value < amount && value >= 0)
            {
                currentAmount = value;
            }
            else if (value < 0)
            {
                currentAmount = 0;
            }
            else currentAmount = amount;
            if (!befCmplt && IsComplete)
            {

            }
        }
    }
    public bool IsComplete
    {
        get
        {
            if (currentAmount >= amount)
            {
                return true;
            }
            return false;
        }
    }

    [SerializeField]
    public bool inOrder;
    public bool InOrder
    {
        get { return inOrder; }
    }

    [SerializeField]
    private int orderIndex;
    public int OrderIndex
    {
        get { return orderIndex; }
    }

    public bool IsValid
    {
        get
        {
            if (Amount < 0) return false;
            if (this is CollectObjective && !(this as CollectObjective).Item) return false;
            //if(this is )



            return true;
        }
    }

    [System.NonSerialized]
    public Objective PrevObjective;
    [System.NonSerialized]
    public Objective NextObjective;
    [HideInInspector]
    public ObjectiveStateListener OnStateChangeEvent;

    protected virtual void UpdateAmountUp(int amount = 1)
    {
        if (IsComplete) return;
        if (!InOrder) CurrentAmount += amount;
        else if (AllPrevObjCmplt) CurrentAmount += amount;
        if (CurrentAmount > 0)
        {
            string message = DisplayName + (IsComplete ? "完成" : "[" + CurrentAmount
                + "/" + Amount + "]");
            PopoutController.CreatePopoutMessage(message, 50);
        }
        if (runtimeParent.IsComplete)
            PopoutController.CreatePopoutMessage
                ("[任务 " + runtimeParent.TITLE + " ](已完成)", 50);
    }
    /// <summary>
    /// 判断所有前置目标是否均完成
    /// </summary>
    public bool AllPrevObjCmplt
    {
        get
        {
            Objective tempObj = PrevObjective;
            while (tempObj != null)
            {
                if (!tempObj.IsComplete && tempObj.OrderIndex < OrderIndex)
                {
                    return false;
                }
                tempObj = tempObj.PrevObjective;
            }
            return true;
        }
    }
    /// <summary>
    /// 判断是否存在后置目标正在进行
    /// </summary>
    public bool HasNextObjOngoing
    {
        get
        {
            Objective tempObj = NextObjective;
            while (tempObj != null)
            {
                if (tempObj.CurrentAmount > 0 && tempObj.OrderIndex > OrderIndex)
                {
                    return true;
                }
                tempObj = tempObj.NextObjective;
            }
            return false;
        }
    }
    /// <summary>
    /// 可并发
    /// </summary>
    public bool Concurrent
    {
        get
        {
            if (!InOrder) return true;//不按顺序，说明可以并行
            if (PrevObjective != null && PrevObjective.OrderIndex == OrderIndex)
                return true;//有前置目标，而且顺序码与前置目标相同，说明可以并发执行
            if (NextObjective != null && NextObjective.OrderIndex == OrderIndex)
                return true;//有后置目标，而且顺序码与后置目标相同，说明可以并发执行
            return false;
        }
    }


    /// <summary>
    /// 更新某个收集类任务目标，用于在其他前置目标完成时，更新后置收集类目标
    /// </summary>
    void UpdateNextCollectObjectives()
    {
        Objective tempObj = NextObjective;
        CollectObjective co;
        while (tempObj != null)
        {
            if (!(tempObj is CollectObjective) && tempObj.InOrder && tempObj.NextObjective != null && tempObj.NextObjective.InOrder && tempObj.OrderIndex < tempObj.NextObjective.OrderIndex)
            {
                //若相邻后置目标不是收集类目标，该后置目标按顺序执行，其相邻后置也按顺序执行，且两者不可同时执行，则说明无法继续更新后置的收集类目标
                return;
            }
            if (tempObj is CollectObjective)
            {
                co = tempObj as CollectObjective;
                co.CurrentAmount = SDDataManager.Instance.GetItemAmount(co.Item.ID);
            }
            tempObj = tempObj.NextObjective;
        }
    }

    public static implicit operator bool(Objective self)
    {
        return self != null;
    }
}

#region 收集类目标
/// <summary>
/// 收集类目标
/// </summary>
[System.Serializable]
public class CollectObjective : Objective
{
    [SerializeField]
    private ItemBase item;
    public ItemBase Item
    {
        get { return item; }
    }

    [SerializeField]
    private string itemID;
    public string ItemID
    {
        get { return itemID; }
    }

    [SerializeField]
    private bool checkBagAtAcpt = true;//用于标识是否在接取任务时检查背包道具看是否满足目标，否则目标重头开始计数
    public bool CheckBagAtAcpt
    {
        get { return checkBagAtAcpt; }
    }

    [SerializeField]
    private bool loseItemAtSbmt = true;//用于标识是否在提交任务时失去相应道具
    public bool LoseItemAtSbmt
    {
        get
        {
            return loseItemAtSbmt;
        }
    }

    public void UpdateCollectAmount(ItemBase item, int leftAmount)//得道具时用到
    {
        if (item == Item)
        {
            if (IsComplete) return;
            if (!InOrder) CurrentAmount = leftAmount;
            else if (AllPrevObjCmplt) CurrentAmount = leftAmount;
            if (CurrentAmount > 0)
            {
                string message = DisplayName + (IsComplete ? "(完成)" : "[" + CurrentAmount + "/" + Amount + "]");
                PopoutController.CreatePopoutMessage(message, 50);
            }
            if (runtimeParent.IsComplete)
            {
                string message = "[任务]" + runtimeParent.TITLE + "(已完成)";
                PopoutController.CreatePopoutMessage(message, 50);
            }
        }
    }
    public void UpdateCollectAmount(string itemId, int leftAmount)
    {
        bool flag = false;
        if (Item != null)
        {
            if (itemId == Item.ID)
            {
                flag = true;
            }
        }
        else
        if(itemId == ItemID)
        {
            flag = true;
        }

        if (flag)
        {
            if (IsComplete) return;
            if (!InOrder) CurrentAmount = leftAmount;
            else if (AllPrevObjCmplt) CurrentAmount = leftAmount;
            if (CurrentAmount > 0)
            {
                string message = DisplayName + (IsComplete ? "(完成)" : "[" + CurrentAmount + "/" + Amount + "]");
                PopoutController.CreatePopoutMessage(message, 50);
            }
            if (runtimeParent.IsComplete)
            {
                string message = "[任务]" + runtimeParent.TITLE + "(已完成)";
                PopoutController.CreatePopoutMessage(message, 50);
            }
        }
    }
    public void UpdateCollectAmountDown(ItemBase item, int leftAmount)//丢道具时用到
    {
        if (item == Item && AllPrevObjCmplt && !HasNextObjOngoing)
            //前置目标都完成且没有后置目标在进行时，才允许更新
            CurrentAmount = leftAmount;
    }
    public void UpdateCollectAmountDown(string itemId, int leftAmount)
    {
        if (Item != null)
        {
            if (itemId == Item.ID && AllPrevObjCmplt && !HasNextObjOngoing)
                //前置目标都完成且没有后置目标在进行时，才允许更新
                CurrentAmount = leftAmount;
        }
        else
        if(itemId == ItemID && AllPrevObjCmplt && !HasNextObjOngoing)
            CurrentAmount = leftAmount;
    }
}
#endregion
#region 打怪类目标

/// <summary>
/// 打怪类目标
/// </summary>
[System.Serializable]
public class KillObjective : Objective
{
    [SerializeField]
#if UNITY_EDITOR
    [EnumMemberNames("特定敌人", "特定种群", "特定级别", "任意敌人")]
#endif
    private KillObjectiveType objectiveType;
    public KillObjectiveType ObjectiveType
    {
        get
        {
            return objectiveType;
        }
    }

    [SerializeField]
    private EnemyInfo enemy;
    public EnemyInfo Enemy
    {
        get
        {
            return enemy;
        }
    }

    [SerializeField]
    private string enemyID;
    public string EnemyID
    {
        get { return enemyID; }
    }

    [SerializeField]
    private EnemyRace race;
    public EnemyRace Race
    {
        get
        {
            return race;
        }
    }

    [SerializeField]
    private EnemyRank enemyRank;
    public EnemyRank EnemyRank
    {
        get { return enemyRank; }
    }

    public void UpdateKillAmount(string id)
    {
        bool flag = false;
        //
        if (ObjectiveType == KillObjectiveType.Any) flag = true;
        else if(ObjectiveType == KillObjectiveType.Race)
        {
            ROEnemyData ED = SDDataManager.Instance.getEnemyDataById(id);
            if (ED.race == Race.Index) flag = true;
        }
        else if(ObjectiveType == KillObjectiveType.Rank)
        {
            ROEnemyData ED = SDDataManager.Instance.getEnemyDataById(id);
            if (ED.rank == EnemyRank.Index) flag = true;
        }
        else if(ObjectiveType == KillObjectiveType.Specific)
        {
            ROEnemyData ED = SDDataManager.Instance.getEnemyDataById(id);
            if (Enemy != null && ED.id == Enemy.ID) flag = true;
            else if (Enemy == null && ED.id == EnemyID) flag = true;
        }
        //
        if(flag)
            UpdateAmountUp();
    }
}
public enum KillObjectiveType
{
    /// <summary>
    /// 特定敌人
    /// </summary>
    Specific,

    /// <summary>
    /// 特定种族
    /// </summary>
    Race,

    /// <summary>
    /// 特定级别
    /// </summary>
    Rank,

    /// <summary>
    /// 任意
    /// </summary>
    Any
}
#endregion
#region 谈话类目标
/// <summary>
/// 谈话类目标
/// </summary>
[System.Serializable]
public class TalkObjective : Objective
{
    [SerializeField]
    private TalkerInfo _NPCToTalk;
    public TalkerInfo NPCToTalk
    {
        get
        {
            return _NPCToTalk;
        }
    }

    [SerializeField]
    private Dialogue dialogue;
    public Dialogue Dialogue
    {
        get
        {
            return dialogue;
        }
    }

    public void UpdateTalkState()
    {
        UpdateAmountUp();
    }
}
#endregion
#region 提交类目标
/// <summary>
/// 提交类目标
/// </summary>
[System.Serializable]
public class SubmitObjective : Objective
{
    [SerializeField]
    private TalkerInfo _NPCToSubmit;
    public TalkerInfo NPCToSubmit
    {
        get
        {
            return _NPCToSubmit;
        }
    }

    [SerializeField]
    private ItemBase itemToSubmit;
    public ItemBase ItemToSubmit
    {
        get
        {
            return itemToSubmit;
        }
    }

    [SerializeField]
    private string wordsWhenSubmit;
    public string WordsWhenSubmit
    {
        get
        {
            return wordsWhenSubmit;
        }
    }

    [SerializeField]
#if UNITY_EDITOR
    [EnumMemberNames("提交的NPC", "玩家")]
#endif
    private TalkerType talkerType;
    public TalkerType TalkerType
    {
        get
        {
            return talkerType;
        }
    }
    public void UpdateSubmitState(int amount = 1)
    {
        UpdateAmountUp(amount);
    }
}
#endregion
#region 自定义目标
/// <summary>
/// 自定义目标
/// </summary>
[System.Serializable]
public class CustomObjective : Objective
{
    [SerializeField]
    private string triggerName;
    public string TriggerName
    {
        get
        {
            return triggerName;
        }
    }

    [SerializeField]
    private bool checkStateAtAcpt = true;
    //用于标识是否在接取任务时检触发器状态看是否满足目标，否则目标重头开始等待触发
    public bool CheckStateAtAcpt
    {
        get
        {
            return checkStateAtAcpt;
        }
    }

    public void UpdateTriggerState(string name, bool state)
    {
        if (name != TriggerName) return;
        if (state) UpdateAmountUp();
        else if (AllPrevObjCmplt && !HasNextObjOngoing) CurrentAmount--;
    }
}
#endregion

#endregion


