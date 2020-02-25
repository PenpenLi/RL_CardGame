using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OneRoleClassData 
{
    /// <summary>
    /// 开始时属性
    /// </summary>
    public RoleAttributeList ThisRoleAttributes
    {
        get { return _thisRoleAttritube; }
    }
#if UNITY_EDITOR
    [SerializeField,ReadOnly]
#endif
    private RoleAttributeList _thisRoleAttritube = RoleAttributeList.zero;
    public void InitThisRoleAttritube(RoleAttributeList ral)
    {
        _thisRoleAttritube = ral.Clone;
    }

    public RoleAttributeList extraRALChangeData;
    /// <summary>
    /// BarChart最大值读取
    /// </summary>
    public RoleBarChart ReadAllMaxSSD
    {
        get
        {
            RoleBarChart a = new RoleBarChart();
            a.HP = ReadCurrentRoleRA(AttributeData.Hp);
            a.MP = ReadCurrentRoleRA(AttributeData.Mp);
            a.TP = ReadCurrentRoleRA(AttributeData.Tp);
            return a;
        }
    }
    //加减int值，用于显示实时属性增减
    public RoleAttributeList AllARevision;
#if UNITY_EDITOR
    [Header("当前输出总属性显示"), ReadOnly]
#endif
    public RoleAttributeList CurrentExportRAL = new RoleAttributeList();
    public void RefreshCERAL()
    {
        RoleAttributeList ral = ThisRoleAttributes.Clone;
        ral.Add(AllARevision);
        ral.Add(extraRALChangeData);
        CurrentExportRAL = ral.Clone;
    }


    #region 索引
    public int ReadCurrentRoleRA(int Tag, bool IsAttributeData = true)
    {
        if (IsAttributeData)
        {
            AttributeData t = (AttributeData)Mathf.Clamp(Tag, 0, (int)AttributeData.End);
            return ThisRoleAttributes.read(t) + AllARevision.read(t) + extraRALChangeData.read(t);
        }
        else
        {
            StateTag t = (StateTag)Mathf.Clamp(Tag, 0, (int)StateTag.End);
            return ThisRoleAttributes.read(t) + AllARevision.read(t) + extraRALChangeData.read(t);
        }
    }
    public int ReadCurrentRoleRA(AttributeData Tag)
    {
        return ThisRoleAttributes.read(Tag) + AllARevision.read(Tag) + extraRALChangeData.read(Tag);
    }
    public int ReadCurrentRoleRA(StateTag Tag)
    {
        return ThisRoleAttributes.read(Tag) + AllARevision.read(Tag) + extraRALChangeData.read(Tag);
    }
    #endregion
    #region 战斗属性快速索引
    public int hp
    {
        get { return ReadCurrentRoleRA(AttributeData.Hp); }
    }
    public int mp
    {
        get { return ReadCurrentRoleRA(AttributeData.Mp); }
    }
    public int tp { get { return ReadCurrentRoleRA(AttributeData.Tp); } }
    public int at { get { return ReadCurrentRoleRA(AttributeData.AT); } }
    public int ad { get { return ReadCurrentRoleRA(AttributeData.AD); } }
    public int mt { get { return ReadCurrentRoleRA(AttributeData.MT); } }
    public int md { get { return ReadCurrentRoleRA(AttributeData.MD); } }
    public int speed { get { return ReadCurrentRoleRA(AttributeData.Speed); } }
    public int taunt { get { return ReadCurrentRoleRA(AttributeData.Taunt); } }
    public int accur { get { return ReadCurrentRoleRA(AttributeData.Accur); } }
    public int evo { get { return ReadCurrentRoleRA(AttributeData.Evo); } }
    public int crit { get { return ReadCurrentRoleRA(AttributeData.Crit); } }
    public int expect { get { return ReadCurrentRoleRA(AttributeData.Expect); } }
    #endregion
}





public enum Job
{
    Fighter=0,
    Ranger =1,
    Priest=2,
    Caster=3,
    End,
}
public enum Race
{
    Human=0,
    Elf=1,
    Dragonborn=2,
    End=3,
}
[System.Serializable]
public class RoleBarChart
{
    #region 角色三项可视化数据
    public int HP
    {
        get { return (int)DATA.x; }
        set { DATA.x = value; }
    }
    public int MP
    {
        get { return (int)DATA.y; }
        set
        {
            DATA.y = value;
        }
    }
    public int TP
    {
        get
        {
            return (int)DATA.z;
        }
        set
        {
            DATA.z = value;
        }
    }
    public Vector3 DATA;
    #endregion
    #region 计算公式
    public static RoleBarChart operator +(RoleBarChart a
        , RoleBarChart b)
    {
        if (a == null) a = zero;
        if (b == null) b = zero;
        a.HP += b.HP;
        a.MP += b.MP;
        a.TP += b.TP;
        return a;
    }
    public static RoleBarChart operator +(RoleBarChart a
    , int b)
    {
        a.HP += b; a.MP += b; a.MP += b; return a;
    }
    public static RoleBarChart operator -(RoleBarChart a
        , RoleBarChart b)
    {
        if (a == null) a = zero;
        if (b == null) b = zero;
        a.HP -= b.HP;
        a.MP -= b.MP;
        a.TP -= b.TP;
        return a;
    }
    public static RoleBarChart operator *(RoleBarChart a
        , int b)
    {
        a.HP *= b; a.MP *= b; a.TP *= b; return a;
    }
    public static RoleBarChart operator *(RoleBarChart a
    , float b)
    {
        a.HP = (int)(a.HP*b);
        a.MP = (int)(a.MP*b);
        a.TP = (int)(a.TP*b);
        return a;
    }
    public static RoleBarChart operator *(RoleBarChart a
        , RoleBarChart b)
    {
        a.HP *= b.HP;a.MP *= b.MP;a.TP *= b.TP;
        return a;
    }
    #endregion
    public RoleBarChart ExpandByRBCPercent(RoleBarChart percentArray)
    {
        HP = (int)(HP * AllRandomSetClass.SimplePercentToDecimal(percentArray.HP));
        MP = (int)(MP * AllRandomSetClass.SimplePercentToDecimal(percentArray.MP));
        TP = (int)(TP * AllRandomSetClass.SimplePercentToDecimal(percentArray.TP));
        return this;
    }
    public void ResetSelf()
    {
        HP = MP = TP = 0;
    }
    public static RoleBarChart zero = new RoleBarChart()
    {
        DATA = Vector3.zero
    };
    public RoleBarChart()
    {
        DATA = Vector3.zero;
    }
    public RoleBarChart(int hp,int mp,int tp)
    {
        HP = hp; MP = mp;TP = tp;
    }
}

/// <summary>
/// 角色基底
/// </summary>
[System.Serializable]
public class ROUnitData
{
    public RoleAttributeList RALRate = RoleAttributeList.zero;//选择属性加成
    public virtual RoleAttributeList ExportRAL
    {
        get
        {
            RoleAttributeList L = RoleAttributeList.zero;
            L.AffectedByRate(RALRate);
            return L;
        }
    }
    //public string careerClass;
    public int CRIDmg = 150;
    public int DmgReduction = 0;//(-∞,100)% 伤害修正
    public int DmgReflection = 0;//(0,x)% 伤害反射
    public RoleBarChart BarChartRegendPerTurn;//每回合四项可视化值回复量
    public virtual int quality
    {
        get { return 0; }
    }
    public virtual bool isRare
    {
        get { return false; }
    }
    
}
/// <summary>
/// 英雄基底
/// </summary>
[System.Serializable]
public class ROHeroData:ROUnitData
{
    public HeroInfo Info;
    public int starNumUpGradeTimes = 0;//后天提升的星级
    public int starNum
    {
        get { return starNumUpGradeTimes; }
    }
    public override RoleAttributeList ExportRAL
    {
        get
        {
            if (Info)
            {
                RoleAttributeList L = Info.RAL.Clone;
                L.AffectedByRate(RALRate);
                return L;
            }
            else return RoleAttributeList.zero;
        }
    }
    public override int quality 
    {
        get 
        {
            if (Info) return Info.Rarity;
            else return 0;
        }
    }
    public override bool isRare
    {
        get
        {
            if (Info) return Info.Rarity >= 2;
            else return false;
        }
    }
    public int RewardRate = 0;//奖励获得概率变动()%
    public int GoldRate = 0;//奖金数量变动()%
    public ROHeroData() { }
}
/// <summary>
/// 敌人基底
/// </summary>
[System.Serializable]
public class ROEnemyData : ROUnitData
{
    public EnemyInfo Info;
    public override int quality
    {
        get {
            if (Info) return Info.EnemyRank.Index;
            else return 0;
        }
    }
    public override bool isRare
    {
        get
        {
            if (Info) return Info.EnemyRank.Index > 1;
            else return false;
        }
    }
    public override RoleAttributeList ExportRAL
    {
        get
        {
            if (Info)
            {
                RoleAttributeList L = Info.RAL;
                L.AffectedByRate(RALRate);
                return L;
            }
            else return RoleAttributeList.zero;
        }
    }
    public List<string> dropItems = new List<string>();
    public int dropCoins = 5;
    //
    public ROEnemyData() { }
}



[System.Serializable]
public class ROTaskData
{
    public int taskID;
    public string taskName;
    public int level;
    public string description;
    public string achieveType;
    public string taskConditions;
    public string taskRewards;
    public string taskClass;
    public ROTaskData(string _taskId, string _taskName, string _level
        , string _description,string _achievetype
        , string _taskRewards, string _taskConditions
        ,string _class)
    {
        taskID = SDDataManager.Instance.getInteger(_taskId);
        level = SDDataManager.Instance.getInteger(_level);
        taskName = _taskName;
        description = _description;
        achieveType = _achievetype;
        taskRewards = _taskRewards;
        taskConditions = _taskConditions;
        taskClass = _class;
    }
}



[System.Serializable]
public class ROHeroAltarPool
{
    public HeroAltarPool Pool;
    public int HaveAltarTimes;
    public int HaveAlartSNum;
    public int PoolCapacity;
    public bool Unable;
    public bool NotNormalPool;
}
