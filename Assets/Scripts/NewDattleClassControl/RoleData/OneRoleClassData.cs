using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RoleAttributeList
{
    public int Hp;//public int MaxHp;
    public int Sp;//public int MaxSp;
    public int Mp;//public int MAxMp;
    public int Tp;//public int MAxTp;
    //
    public int AT;public int AD;//物理攻防
    public int MT;public int MD;//法术攻防
    public int Speed;//行动条速度
    public int Taunt;//嘲讽值
    public int Evo;//闪避值
    public int Crit;//暴击值
    public int CritD;//暴击伤害加成值

    //恢复率影响值
    public int Hp_Up;
    public int Sp_Up;
    public int Mp_Up;
    public int Tp_Up;


}

//角色基础能力，用于生成角色基本属性
[System.Serializable]
public class RoleSelfAbility
{
    //几大基础属性
    /// <summary>
    /// 耐性
    /// </summary>
    public int Dur;
    /// <summary>
    /// 力量
    /// </summary>
    public int Str;
    /// <summary>
    /// 聪慧
    /// </summary>
    public int Wis;
    /// <summary>
    /// 敏捷
    /// </summary>
    public int Dex;
    /// <summary>
    /// 感知
    /// </summary>
    public int Per;


    //角色其他属性
    /// <summary>
    /// 社交倾向
    /// </summary>
    public Vector2 Disposition;



}

public enum Job
{
    Fighter,
    Caster,
    Ranger,
    Other,
}
public enum Race
{
    Human,
    Elf,
    Draeni,

}

[System.Serializable]
public class OneRoleClassData 
{
    public OneRoleShow ThisRoleShow;
    #region 开始时角色数据
    //角色详细属性（正式战斗时实时显示）
    public RoleAttributeList ThisRoleAttributes;
    //角色装备
    public OneEquipageData[] ThisRoleEquipageList;
    //角色职业
    Job _thisjob;
    public Job ThisJob { get { return _thisjob; }private set { _thisjob = value; } }
    //角色种族
    Race _thisrace;
    public Race ThisRace { get { return _thisrace; }private set { _thisrace = value; } }







    //角色详细属性生成工具
    public void RefreshAbility()
    {

    }
    #endregion
    #region 角色实时数据变化
    private int _hp;
    public int Hp {get { return _hp; } private set { _hp = value; } }
    private int _sp;
    public int Sp { get { return _sp; }private set { _sp = value; } }
    private int _mp;
    public int Mp { get { return _mp; }private set { _mp = value; } }
    private int _tp;
    public int Tp { get { return _tp; }private set { _tp = value; } }
    //
    public List<OneState> StateList = new List<OneState>();
    #endregion


    public virtual void ReceiveEffect(SkillKind ThisSkillKind, OneEffect ThisEffect,bool UseDef)
    {
        int Num = ThisEffect.Data;
        if (!UseDef)
        {
            //直接有效不受角色属性影响
        }
        else
        {
            if(ThisSkillKind == SkillKind.Physical)
            {
                Num = Num * 3 + ThisRoleAttributes.AD * 2;//物理伤害计算公式
            }
            else if(ThisSkillKind == SkillKind.Elemental)
            {
                Num = Num - ThisRoleAttributes.MD;//元素伤害计算公式
            }
            else if(ThisSkillKind == SkillKind.Arcane)
            {
                //Num = Num;//神秘伤害计算公式
            }
        }
        switch (ThisEffect.ThisDataType)
        {
            case DataType.Normal:
                {
                    break;
                }
            case DataType.HP_SP:
                {
                    if (this.Sp + Num < 0)
                    {
                        this.Sp = 0; this.Hp += this.Sp + Num;
                    }
                    else
                    {
                        this.Sp += Num;
                    }
                    break;
                }
            case DataType.HPOnly:
                {
                    this.Hp += Num;
                    break;
                }
            case DataType.SPOnly:
                {
                    this.Sp += Num;
                    break;
                }
            case DataType.MP:
                {
                    this.Mp += Num;
                    break;
                }
            case DataType.TP:
                {
                    this.Tp += Num;
                    break;
                }
            default:
                {
                    Debug.Log(ThisRoleShow.Name +"#" + ThisRoleShow.Id + " 受到的影响不存在或影响类型不存在");
                    break;
                }
        }
    }
    public virtual bool ReceiveState(OneState ThisState)
    {
        StateList.Add(ThisState);
        return true;
    }

    virtual public void UseSkill(OneSkill UseThisSkill)
    {
        //Cost
        for(int i = 0; i < UseThisSkill.AllSkillResults.Length; i++)
        {
            //技能消耗
            if (UseThisSkill.AllSkillResults[i].ThisSkillTarget == SkillAim.Self)
            {
                ReceiveEffect(UseThisSkill.ThisSkillKind, UseThisSkill.AllSkillResults[i], false);
            }
        }
        if(UseThisSkill.SkillState.ThisSkillTarget == SkillAim.Self)
        {
            ReceiveState(UseThisSkill.SkillState);
        }

    }


}



//
[System.Serializable]
public class RoleSkillCardsEntry
{
    public OneSkill[] OneRoleCardPool;//总卡池
    public List<OneSkill> CurrentCardPool;//战斗时卡组
    public List<OneSkill> CurrentCardCemetery;//战斗时墓地

    //当前获取卡牌
    public int MaxShowingCardNum;   
    public List<OneSkill> OnShowingCards;
}



[System.Serializable]
public class OneRoleShow//可用于角色库观察
{
    public int Id;
    public string Name;
    public string Desc;
    public string Icon;
    public string BgIcon;
    //
    public int Count;
    [Range(0, 5)]
    public int Quality;
    public int Level;
    public int Mastery;

    //角色基础能力，用于生成角色详细属性
    public RoleSelfAbility ThisRoleAbility;
}