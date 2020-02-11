using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#region 所有相关Enum
public enum SkillKind
{
    Elemental=0,
    Physical,
    Arcane,
    End,
}
public enum SkillBreed
{    
    Remote,
    Close,
    Absence,
}
public enum SkillAim
{
    Self,
    Friend,
    Other,
    All,
    End,
}
#endregion

[System.Serializable]
public class OneSkill
{
    public string skillId;
    public string SkillName;
    [HideInInspector]
    public int index;

    public bool UseAppointedPrefab;
#if false
    [ConditionalHide("UseAppointedPrefab", true, true)]
#endif
    public int SkillFunctionID;
#if false
    [ConditionalHide("UseAppointedPrefab", true, false)]
#endif
    public Transform SkillPrefab;
#if false
    [ConditionalHide("UseAppointedPrefab", true, false)]
#endif
    public skillInfo.ExtraSkillDataSet DataSet;

    public string IconImg;
    public string BulletImg;

    public bool isOmegaSkill;
    public bool islocked
    {
        get { return lv < 0; }
    }

    public string Desc;
    public int lv;

    public int skillAnimId;

    public SkillAim Aim = SkillAim.End;
    public SkillBreed Breed = SkillBreed.Absence;
    public SkillKind Kind = SkillKind.End;
    public SDConstants.AOEType SkillAoe = SDConstants.AOEType.End;
    public SDConstants.AddMpTpType MpTpAddType = SDConstants.AddMpTpType.End;
    public static OneSkill normalAttack
    {
        get {
            return new OneSkill()
            {
                skillId = "@SH_NOR#000",
                SkillName = "普通攻击",
                SkillFunctionID = 0,
                IconImg = "NormalAttack",
                Desc = "对单体敌人造成基本物理伤害获取行动资源",
                lv = 0
            };
        }
    }
    public OneSkill()
    {
        skillId = string.Empty;
        SkillName = string.Empty;
        SkillFunctionID = -1;
        IconImg = string.Empty;
        Desc = string.Empty;
        lv = 0;
    }
}
