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
    public static List<OneSkill> WriteOneSkillList(string id)
    {
        HeroInfo hero = SDDataManager.Instance.getHeroInfoById(id);
        Job _job = hero.Career.Career;
        Race _race = hero.Race.Race;
        int LEVEL = hero.LEVEL;
        List<OneSkill> all = new List<OneSkill>();
        if (_job == Job.Fighter)
        {
            all = skillList_fighter(_race,LEVEL);
        }
        else if(_job == Job.Ranger)
        {
            all = skillList_ranger(_race, LEVEL);
        }
        else if (_job == Job.Priest)
        {
            all = skillList_priest(_race, LEVEL);
        }
        else if (_job == Job.Caster)
        {
            all = skillList_caster(_race, LEVEL);
        }
        for(int i = 0; i < all.Count; i++)
        {
            all[i].skillId = string.Format("@SH_N#{0:D3}",i);
        }
        //
        for(int i = 0; i < hero.PersonalSkillList.Count; i++)
        {
            all.Add(InitBySkillInfo(hero.PersonalSkillList[i]));
        }
        //
        return all;
    }
    #region CAREER_RACE_LEVEL
    public static List<OneSkill> skillList_fighter(Race _race, int quality)
    {
        List<OneSkill> skills = new List<OneSkill>();
        int starmax = SDConstants.UnitMAxStarNum;
        skills.Add(new OneSkill()
        {
            SkillName = "斩击"
    ,
            SkillFunctionID = 11
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
        skills.Add(new OneSkill()
        {
            SkillName = "重剑气刃"
,
            SkillFunctionID = 2
,
            Desc = "挥动武器释放剑气攻击前排敌人"
,
            SkillAoe = SDConstants.AOEType.Vertical1
            ,
            isOmegaSkill=true,
        });
        if (_race == Race.Human)
        {

            if (quality > 0)
            {
                skills.Add(new OneSkill() 
                {
                    SkillName = "惩罚",
                    SkillFunctionID=12,
                    Desc="对所有敌人造成混合伤害",
                    SkillAoe=SDConstants.AOEType.All,
                });
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
            SkillFunctionID = 11
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
            ,
            isOmegaSkill = true,
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
                skills.Add(new OneSkill() 
                {
                    SkillName="皎月",
                    SkillFunctionID=12,
                    Desc="对全场敌人造成混合伤害",
                    SkillAoe= SDConstants.AOEType.All,
                });
                if (quality > 1)
                {

                }
            }
        }
        else if (_race == Race.Dragonborn)
        {
            skills.Add(new OneSkill()
            {
                SkillName = "龙印",
                SkillFunctionID = 10,
                Desc = "全队增加伤害",
                SkillAoe = SDConstants.AOEType.All,
                Aim = SkillAim.Self,

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
    public static List<OneSkill> skillList_priest(Race _race,int quality)
    {
        List<OneSkill> skills = new List<OneSkill>();
        skills.Add(new OneSkill()
        {
            SkillName = "重击"
    ,
            SkillFunctionID = 11
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
            ,
            isOmegaSkill = true,

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
            skills.Add(new OneSkill()
            {
                SkillName = "龙印",
                SkillFunctionID = 10,
                Desc = "全队增加伤害",
                SkillAoe = SDConstants.AOEType.All,
                Aim = SkillAim.Self,

            });
            if (quality > 0)
            {
                skills.Add(new OneSkill()
                {
                    SkillName = "龙印",
                    SkillFunctionID = 10,
                    Desc = "全队增加伤害",
                    SkillAoe = SDConstants.AOEType.All,
                    Aim = SkillAim.Self,

                });
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
            SkillName = "贵族防身术"
    ,
            SkillFunctionID = 11
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
            ,
            isOmegaSkill=true,
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
            skills.Add(new OneSkill()
            {
                SkillName = "龙吼"
,
                SkillFunctionID = 7
,
                Desc = "运用强精神力压制全场"
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
    #endregion
    #region SkilInfo To OneSkill
    public static OneSkill InitBySkillInfo(skillInfo info)
    {
        OneSkill s = new OneSkill()
        {
            skillId = info.ID
            ,
            SkillName = info.name
            ,
            UseAppointedPrefab=info.UseAppointedPrefab,
            SkillFunctionID=info.FunctionId,
            SkillPrefab=info.SkillPrefab,
            isOmegaSkill=info.IsOmegaSkill,
            DataSet = info.ExtraDataSet,
        };
        return s;
    }
    #endregion




    public static int AddMpAfterSkill(SDConstants.AddMpTpType addType
        ,BattleRoleData source)
    {
        GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(source.unitHashcode);
        HeroInfo info = SDDataManager.Instance.getHeroInfoById(source.UnitId);
        int baseMp = info.RAL.Mp + hero.RoleAttritubeList.AD_List[(int)AttributeData.Mp];
        int currentMp = source.ThisBasicRoleProperty().RoleBasicRA.Mp;
        //
        float upRate0 = currentMp * 1f / baseMp;
        float upRate1 = 1;
        if(addType == SDConstants.AddMpTpType.PreferMp
            || addType == SDConstants.AddMpTpType.PreferBoth)
        {
            upRate1 = 1.5f;
        }
        else if(addType == SDConstants.AddMpTpType.LowMp
            || addType == SDConstants.AddMpTpType.LowBoth)
        {
            upRate1 = 0.5f;
        }
        else if(addType == SDConstants.AddMpTpType.YearnMp
            || addType == SDConstants.AddMpTpType.YearnBoth)
        {
            upRate1 = 2f;
        }
        return (int)(SDConstants.SkillAddMinMp * upRate0 * upRate1);
    }
    public static int AddTpAfterSkill(SDConstants.AddMpTpType addType
        ,BattleRoleData source)
    {
        GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(source.unitHashcode);
        HeroInfo info = SDDataManager.Instance.getHeroInfoById(source.UnitId);
        int baseTp = info.RAL.Tp + hero.RoleAttritubeList.AD_List[(int)AttributeData.Tp];
        int currentTp = source.ThisBasicRoleProperty().RoleBasicRA.Tp;
        //
        float upRate0 = currentTp * 1f / baseTp;
        float upRate1 = 1;
        if (addType == SDConstants.AddMpTpType.PreferMp
            || addType == SDConstants.AddMpTpType.PreferBoth)
        {
            upRate1 = 1.5f;
        }
        else if (addType == SDConstants.AddMpTpType.LowMp
            || addType == SDConstants.AddMpTpType.LowBoth)
        {
            upRate1 = 0.5f;
        }
        else if (addType == SDConstants.AddMpTpType.YearnMp
            || addType == SDConstants.AddMpTpType.YearnBoth)
        {
            upRate1 = 2f;
        }
        return (int)(SDConstants.SkillAddMinMp * upRate0 * upRate1);
    }
}