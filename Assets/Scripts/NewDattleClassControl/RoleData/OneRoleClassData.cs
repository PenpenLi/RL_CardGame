using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum SelfAbilityElement
{
    Dur,
    Str,
    Wis,
    Dex,
    Per,
    End,
}
//角色基础能力，用于生成角色基本属性
[System.Serializable]
public class RoleSelfAbility
{
    //几大基础属性
    /// <summary>
    /// 耐性
    /// </summary>
    public int Dur { get { return BasicSA[(int)SelfAbilityElement.Dur]; }set { BasicSA[(int)SelfAbilityElement.Dur] = value; } }
    /// <summary>
    /// 力量
    /// </summary>
    public int Str { get { return BasicSA[(int)SelfAbilityElement.Str]; } set { BasicSA[(int)SelfAbilityElement.Str] = value; } }
    /// <summary>
    /// 聪慧
    /// </summary>
    public int Wis { get { return BasicSA[(int)SelfAbilityElement.Wis]; } set { BasicSA[(int)SelfAbilityElement.Wis] = value; } }
    /// <summary>
    /// 敏捷
    /// </summary>
    public int Dex { get { return BasicSA[(int)SelfAbilityElement.Dex]; } set { BasicSA[(int)SelfAbilityElement.Dex] = value; } }
    /// <summary>
    /// 感知
    /// </summary>
    public int Per { get { return BasicSA[(int)SelfAbilityElement.Per]; } set { BasicSA[(int)SelfAbilityElement.Per] = value; } }
    public int[] BasicSA = new int[(int)SelfAbilityElement.End];
    //角色其他属性
    /// <summary>
    /// 社交倾向
    /// </summary>
    public Vector2 Disposition;
    /// <summary>
    /// 角色源面板构建
    /// </summary>
    /// <param name="dur">耐性</param>
    /// <param name="str">力量</param>
    /// <param name="wis">聪慧</param>
    /// <param name="dex">敏捷</param>
    /// <param name="per">感知</param>
    /// <param name="disposition">社交倾向</param>
    public RoleSelfAbility(int dur,int str,int wis,int dex,int per,Vector2 disposition)
    {
        this.Dur = dur;this.Str = str;this.Wis = wis;this.Dex = dex;this.Per = per;this.Disposition = disposition;
    }
    /// <summary>
    /// 构建由源面板拼成的数据
    /// </summary>
    /// <param name="Dur_R">耐性占比</param>
    /// <param name="Str_R">力量占比</param>
    /// <param name="Wis_R">智慧占比</param>
    /// <param name="Dex_R">敏捷占比</param>
    /// <param name="Per_R">感知占比</param>
    /// <returns></returns>
    public int IntgerFromAbility(float Dur_R,float Str_R,float Wis_R,float Dex_R,float Per_R)
    {
        int a = (int)(Dur * Dur_R + Str * Str_R + Wis * Wis_R + Dex * Dex_R + Per * Per_R);
        return a;
    }
}
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
        get { return ThisArray[0]; }
        set { DATA.x = value; }
    }
    public int MP
    {
        get { return ThisArray[1]; }
        set
        {
            DATA.y = value;
        }
    }
    public int TP
    {
        get
        {
            return ThisArray[2];
        }
        set
        {
            DATA.z = value;
        }
    }
    [HideInInspector]
    public int[] ThisArray 
    {
        get { return new int[3] { (int)DATA.x, (int)DATA.y, (int)DATA.z }; }
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
        TP = 0
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
    public int ID = 0;
    public int gender = -1;
    public int quality = 0;
    public bool isRare = false;
}