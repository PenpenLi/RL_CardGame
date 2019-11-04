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
    Absence,
    Remote,
    Close,
    End,
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
    public int skillId;
    public string SkillName;
    public int SkillFunctionID;
    public string IconImg;
    public string BulletImg;

    public bool isOmegaSkill;
    public bool isUnlocked;

    public string Desc;
    public int lv;

    public int skillAnimId;

    public SkillAim Aim = SkillAim.End;
    public SkillBreed Breed = SkillBreed.End;
    public SkillKind Kind = SkillKind.End;
    public SDConstants.AOEType SkillAoe = SDConstants.AOEType.End;
    public SDConstants.AddMpTpType MpTpAddType = SDConstants.AddMpTpType.End;
    public static OneSkill normalAttack
    {
        get {
            return new OneSkill()
            {
                skillId = 0,
                SkillName = "NormalAttack",
                SkillFunctionID = 0,
                IconImg = "NormalAttack",
                Desc = "对单体敌人造成基本物理伤害",
                lv = 0
            };
        }
    }
}
