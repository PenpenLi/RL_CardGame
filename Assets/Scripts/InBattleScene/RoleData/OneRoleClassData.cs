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
        get { return baseRALChangeData + extraRALChangeData; }
        set { baseRALChangeData = value; }
    }
    public RoleAttributeList baseRALChangeData;
    public RoleAttributeList extraRALChangeData 
    {
        get { return groupStateRAL + environmentStateRAL; }
    }
    public RoleAttributeList groupStateRAL;
    public RoleAttributeList environmentStateRAL;
    //public RoleAttributeList 

    #region 索引
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
    public int ReadCurrentRoleRA(int Tag, bool IsAttributeData = true)
    {
        if (IsAttributeData)
        {
            AttributeData t = (AttributeData)Mathf.Clamp(Tag, 0, (int)AttributeData.End);
            return ThisRoleAttributes.read(t) + AllARevision.read(t);
        }
        else
        {
            StateTag t = (StateTag)Mathf.Clamp(Tag, 0, (int)StateTag.End);
            return ThisRoleAttributes.read(t) + AllARevision.read(t);
        }
    }
    public int ReadCurrentRoleRA(AttributeData Tag)
    {
        return ThisRoleAttributes.read(Tag) + AllARevision.read(Tag);
    }
    public int ReadCurrentRoleRA(StateTag Tag)
    {
        return ThisRoleAttributes.read(Tag) + AllARevision.read(Tag);
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
    Ranger,
    Priest,
    Caster,
    End,
}
public enum Race
{
    Human=0,
    Elf,
    Dragonborn,
    End,
}
[System.Serializable]
public class RoleBarChart
{
    #region 角色三项可视化数据
    public int HP
    {
        get { return ThisArray(0); }
        set { DATA.x = value; }
    }
    public int MP
    {
        get { return ThisArray(1); }
        set
        {
            DATA.y = value;
        }
    }
    public int TP
    {
        get
        {
            return ThisArray(2);
        }
        set
        {
            DATA.z = value;
        }
    }
    [HideInInspector]
    public int ThisArray(int index)
    {
        switch (index)
        {
            case 0:return (int)DATA.x;
            case 1:return (int)DATA.y;
            case 2:return (int)DATA.z;
            default:return (int)DATA.x;
        }
    }
    public Vector3 DATA;
    #endregion
    #region 计算公式
    public static RoleBarChart operator +(RoleBarChart a
        , RoleBarChart b)
    {
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
        HP = 0,
        MP = 0,
        TP = 0,
        DATA = Vector3.zero
    };
}

/// <summary>
/// 角色基底
/// </summary>
[System.Serializable]
public class ROHeroData
{
    public string Name = "基础人";//名字
    public int starNum=0;//LEVEL

    public RoleAttributeList BasicRAL = RoleAttributeList.zero;
    public RoleAttributeList RALRate = RoleAttributeList.zero;//选择属性加成

    public int CRI = 10;
    public int CRIDmg = 150;
    public int DmgReduction = 0;//(-∞,100)% 伤害修正
    public int DmgReflection = 0;//(0,x)% 伤害反射
    public int RewardRate = 0;//奖励获得概率变动()%
    public int GoldRate = 0;//奖金数量变动()%
    //public int HireCost = 0;//雇佣价格
    public RoleBarChart BarChartRegendPerTurn;//每回合四项可视化值回复量
    public string ID = string.Empty;
    public int gender = -1;
    public int quality = 0;
    public bool isRare = false;
}
/// <summary>
/// 守护者基底
/// </summary>
[System.Serializable]
public class RoGoddessData
{
    public string name;
    public int lv;
    public int star;
    public int volume;
    public string sprite;
    public string ID = string.Empty;
    public int gender = -1;
    public int quality;
    public int race;
    public string mainEffect;
    public string sideEffect;
    public string passiveEffect;
    public List<int> teamIdsUsed;
}

[System.Serializable]
public class ROPropData
{
    public string name;
    public string id;
    public int level;
    public int rarity;
    public string image;
    public int mode;
    public SDConstants.ItemType itemType = SDConstants.ItemType.Prop;
    public int canUseInGame;
    public string des;
    public string specialStr;
    public string target;
    public int buyPrice_gold;
    public int buyPrice_damond;
}

[System.Serializable]
public class ROMaterialData
{
    public string name;
    public string id;
    public int level;
    public int rarity;
    public string materialType;
    public string image;
    public int mode;
    public string kind;
    public SDConstants.ItemType itemType = SDConstants.ItemType.Material;
    public int canUseInGame;
    public string des;
    public string specialStr;
    #region normalMaterial
    public int figure;
    public int exchangeType;
    public int maxStack;
    public int weight;
    #endregion
    #region propMaterial
    public string range;
    public string target;
    #endregion
    public int buyPrice_gold;
    public int buyPrice_damond;

    public int minLv;
    public int maxLv;
}

[System.Serializable]
public class ROEquipData
{
    public string name;
    public string id; 
    public int pos;
    //public int quality;
    public int rarity;
    public string image;
    public int type;
    public string passiveEffect;
    public bool isArmor;
    public string mainEffect;
    public string sideEffect;
    public RoleAttributeList RAL;
}
/// <summary>
/// 敌人基底
/// </summary>
[System.Serializable]
public class ROEnemyData
{
    public string name;
    public string id;
    public int race;
    public string Class;

    private int _rank = -1;
    public int rank
    {
        get 
        {
            if (_rank < 0)
            {
                _rank = 0;
                for (int i = 0; i < (int)SDConstants.EnemyType.end; i++)
                {
                    if(Class == ((SDConstants.EnemyType)i).ToString())
                    {
                        _rank = i;
                    }
                }
            }
            return _rank;
        }
    }

    public int gender;
    public int skeleton;
    public int quality;
    //public string _class;
    public int weight;
    public int appearWeight;
    //[HideInInspector]
    public int dropRate;
    public string dropItems;
    public RoleAttributeList RAL;
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
