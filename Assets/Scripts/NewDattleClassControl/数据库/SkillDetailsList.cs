using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;

public class SkillDetailsList : MonoBehaviour
{
    #region 技能prefab列表
    [Header("全技能按钮transform")]
    public List<Transform> AllSkillList;
    #endregion
    public static List<OneSkill> WriteOneSkillList(Job _job, Race _race, int quality)
    {
        List<OneSkill> all = new List<OneSkill>();
        if (_job == Job.Fighter)
        {
            all = skillList_fighter(_race,quality);
        }
        else if(_job == Job.Ranger)
        {
            all = skillList_ranger(_race, quality);
        }
        else if (_job == Job.Priest)
        {
            all = skillList_priest(_race, quality);
        }
        else if (_job == Job.Caster)
        {
            all = skillList_caster(_race, quality);
        }
        for(int i = 0; i < all.Count; i++)
        {
            all[i].skillId = i+1;
        }
        return all;
    }
    public static List<OneSkill> skillList_fighter(Race _race, int quality)
    {
        List<OneSkill> skills = new List<OneSkill>();
        int starmax = SDConstants.UnitMAxStarNum;
        skills.Add(new OneSkill()
        {
            SkillName = "斩击"
    ,
            SkillFunctionID = 0
    ,
            Desc = "给予单体敌人物理伤害"
        });
        skills.Add(new OneSkill()
        {
            SkillName = "嘲讽"
,
            SkillFunctionID = 1
,
            Desc = "提升自身嘲讽值"
        }) ;
        if (_race == Race.Human)
        {
            skills.Add(new OneSkill()
            {
                SkillName = "霰弹欢迎"
            ,
                SkillFunctionID = 2
            ,
                Desc = "使用霰弹枪射击前排敌人"
            ,
                SkillAoe = SDConstants.AOEType.Vertical1
            });
            if (quality > 0)
            {

                if (quality > 1)
                {

                }
            }
        }
        else if (_race == Race.Elf)
        {
            skills.Add(new OneSkill() 
            { 
                SkillName = "精灵舞步"
                , 
                SkillFunctionID = 10
                ,
                Desc = "大幅提升自身闪避值" 
                ,
                Aim = SkillAim.Self
                ,
                SkillAoe = SDConstants.AOEType.None
                ,
                Kind = SkillKind.Physical
                ,
                MpTpAddType = SDConstants.AddMpTpType.PreferMp
            });
            if (quality > 0)
            {

                if (quality > 1)
                {

                }
            }
        }
        else if (_race == Race.Dragonborn)
        {
            skills.Add(new OneSkill()
            {
                SkillName="龙吼"
                ,
                SkillFunctionID=7
                ,
                Desc="运用强精神力压制全场"
                ,
                MpTpAddType = SDConstants.AddMpTpType.PreferTp
                ,
            });
            if (quality > 0)
            {

                if (quality > 1)
                {

                }
            }
        }
        return skills;
    }
    public static List<OneSkill> skillList_ranger(Race _race , int quality)
    {
        List<OneSkill> skills = new List<OneSkill>();
        skills.Add(new OneSkill()
        {
            SkillName = "突袭"
    ,
            SkillFunctionID = 0
    ,
            Desc = "给予单体敌人物理伤害"
        });
        skills.Add(new OneSkill()
        {
            SkillName = "穿刺攻击"
,
            SkillFunctionID = 5
,
            Desc = "给予横排敌人物理伤害"
        });

        if (_race == Race.Human)
        {
            skills.Add(new OneSkill()
            {
                SkillName = "快枪射击",
                SkillFunctionID = 2,
                Desc = "给予单个敌人远程物理伤害",
            });
            if (quality > 0)
            {

            if (quality > 1)
                {

                }
            }
        }
        else if (_race == Race.Elf)
        {
            if (quality > 0)
            {

                if (quality > 1)
                {

                }
            }
        }
        else if (_race == Race.Dragonborn)
        {
            if (quality > 0)
            {

                if (quality > 1)
                {

                }
            }
        }
        return skills;
    }
    public static List<OneSkill> skillList_priest(Race _race,int quality)
    {
        List<OneSkill> skills = new List<OneSkill>();
        skills.Add(new OneSkill()
        {
            SkillName = "重击"
    ,
            SkillFunctionID = 0
    ,
            Desc = "给予单体敌人物理伤害"
        });
        skills.Add(new OneSkill()
        {
            SkillName = "圣歌"
,
            SkillFunctionID = 3
,
            Desc = "给予单体友军生命治疗"
        });
        skills.Add(new OneSkill()
        {
            SkillName = "守护"
    ,
            SkillFunctionID = 10
    ,
            Desc = "提升一纵列友军的物理防御一段时间"
        });

        if (_race == Race.Human)
        {
            if (quality > 0)
            {

                if (quality > 1)
                {

                }
            }
        }
        else if (_race == Race.Elf)
        {
            if (quality > 0)
            {

                if (quality > 1)
                {

                }
            }
        }
        else if (_race == Race.Dragonborn)
        {
            if (quality > 0)
            {

                if (quality > 1)
                {

                }
            }
        }
        return skills;
    }
    public static List<OneSkill> skillList_caster(Race _race , int quality)
    {
        List<OneSkill> skills = new List<OneSkill>();
        skills.Add(new OneSkill()
        {
            SkillName = "甩杖"
    ,
            SkillFunctionID = 0
    ,
            Desc = "给予单体敌人物理伤害"
        });
        skills.Add(new OneSkill()
        {
            SkillName = "火球术"
,
            SkillFunctionID = 4
,
            Desc = "给予单体敌人远程法术伤害"
        });
        skills.Add(new OneSkill()
        {
            SkillName = "元素震爆"
,
            SkillFunctionID = 7
,
            Desc = "给予全部敌人远程法术伤害"
        });
        if (_race == Race.Human)
        {
            if (quality > 0)
            {

                if (quality > 1)
                {

                }
            }
        }
        else if (_race == Race.Elf)
        {
            if (quality > 0)
            {

                if (quality > 1)
                {

                }
            }
        }
        else if (_race == Race.Dragonborn)
        {
            if (quality > 0)
            {

                if (quality > 1)
                {

                }
            }
        }
        return skills;
    }

}