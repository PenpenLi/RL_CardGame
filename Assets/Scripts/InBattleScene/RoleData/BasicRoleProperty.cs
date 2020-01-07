using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRoleProperty : MonoBehaviour
{
    public int LEVEL = 0;//品阶(注意 是星级不是等级)
    public int Quality = 0;
    //
    public RoleAttributeList RoleBasicRA;//角色数据
    public int CRIDmg = 0;//暴击伤害()%
    public int DmgReduction = 0;//(-∞,100)% 伤害修正
    public int DmgReflection = 0;//(0,x)% 伤害反射
    public int ForceShield = 0;//
    public int RewardRate = 0;//奖励获得概率变动()%
    public int GoldRate = 0;//奖金数量变动()%
    public int MpAddRate = 0;//回能强度()%——只影响使用技能时效果
    public int TPAddRate = 0;//回怒强度()%——只影响使用技能时效果
    //
    public int HireCost = 0;//雇佣价格
    //
    public RoleBarChart BarChartRegendPerTurn = RoleBarChart.zero;//每回合三项可视化值回复量
    public string ID = string.Empty;
    public string Name = "";//名字
    [HideInInspector]
    public int UpGradeGap = 3;//大幅提升的等级差

    /// <summary>
    /// 生成用于战斗的角色信息
    /// </summary>
    public OneRoleClassData _role;
    
    public virtual void initData(int level,RoleAttributeList dataRA
        ,int criDmg,int dmgReduction,int dmgReflection,int RewardRate
        ,RoleBarChart bcRegendPerTurn,string id,string name, int wakeNum)
    {
        LEVEL = level;
        RoleBasicRA = dataRA;
        CRIDmg = criDmg;
        DmgReduction = dmgReduction;
        DmgReflection = dmgReflection;
        this.RewardRate = RewardRate;
        BarChartRegendPerTurn = bcRegendPerTurn;
        ID = id;Name = SDGameManager.T(name);
    }
    public void initDataEmpty()
    {
        initData(0, new RoleAttributeList(), 0, 0, 0, 0
            , new RoleBarChart(), string.Empty, "",0);
    }
    /// <summary>
    /// 构建英雄数据
    /// </summary>
    /// <param name="heroJob"></param>
    /// <param name="grade">英雄当前等级</param>
    /// <param name="quality"></param>
    /// <param name="Level"></param>
    /// <param name="ra"></param>
    /// <param name="raRate"></param>
    /// <param name="speed"></param>
    /// <param name="cri"></param>
    /// <param name="criDmg"></param>
    /// <param name="dmgReduction"></param>
    /// <param name="dmgReflection"></param>
    /// <param name="RewardRate"></param>
    /// <param name="bcRegendPerTurn"></param>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="wakeNum"></param>
    public virtual void initData_Hero(Job heroJob, Race heroRace, int grade
        , int quality, int Level
        , RoleAttributeList ra
        , int criDmg, int dmgReduction, int dmgReflection, int RewardRate
        , RoleBarChart bcRegendPerTurn, string id, string name, int wakeNum)
    {
        LEVEL = Level;
        Quality = quality;
        RoleBasicRA = ra;
        CRIDmg = criDmg;
        DmgReduction = dmgReduction; DmgReflection = dmgReflection;
        this.RewardRate = RewardRate;
        BarChartRegendPerTurn = bcRegendPerTurn;
        ID = id; Name = name;
        AddMultiplier(grade);
        AddWakeMultiplier(wakeNum);
        //补充设置(职业影响)
        switch (heroJob)
        {
            case Job.Fighter:
                initData_supplement_H_Fighter(RoleBasicRA, grade);
                break;
            case Job.Ranger:
                initData_supplement_H_Ranger(RoleBasicRA, grade);
                break;
            case Job.Priest:
                initData_supplement_H_Priest(RoleBasicRA, grade);
                break;
            case Job.Caster:
                initData_supplement_H_Caster(RoleBasicRA, grade);
                break;
        }
        switch (heroRace)
        {
            case Race.Human:
                initData_supplement_H_human();
                break;
            case Race.Elf:
                initData_supplement_H_elf();
                break;
            case Race.Dragonborn:
                initData_supplement_H_dragonborn();
                break;
        }
        AddLEVELNumMultiplier(LEVEL);
        Name = SDGameManager.T(name);
    }

    #region 职业
    public void initData_supplement_H_Fighter(RoleAttributeList ra, int grade)
    {
        RoleAttributeList gradeEffect = AdolescentSet.AttritubeShowByLevel_basic(grade
            , 1.2f//生命
            , 0.1f//法力
            , 1f//怒气
            , 0.2f, 1f//物攻防
            , 0.1f, 0.1f//法攻防
            );
        RoleBasicRA = ra + gradeEffect;//等级加成
        AddTempleMultiplier(Job.Fighter);
        //额外效果
        DmgReduction += 10;
    }
    public void initData_supplement_H_Ranger(RoleAttributeList ra, int grade)
    {
        RoleAttributeList gradeEffect = AdolescentSet.AttritubeShowByLevel_basic(grade
            , 0.8f//生命
            , 0.4f//法力
            , 0.75f//怒气
            , 1.2f, 0.3f//物攻防
            , 0.8f, 0.3f//法攻防
            );
        RoleBasicRA = ra + gradeEffect;//等级加成
        AddTempleMultiplier(Job.Ranger);
        //额外效果
        CRIDmg += 25;
    }
    public void initData_supplement_H_Priest(RoleAttributeList ra, int grade)
    {
        RoleAttributeList gradeEffect = AdolescentSet.AttritubeShowByLevel_basic(grade
            , 1.5f//生命
            , 0.5f//法力
            , 0.2f//怒气
            , 0.2f, 0.75f//物攻防
            , 0.2f, 0.75f//法攻防
            );
        RoleBasicRA = ra + gradeEffect;//等级加成
        AddTempleMultiplier(Job.Priest);
        //额外效果
        DmgReflection += 5;
    }
    public void initData_supplement_H_Caster(RoleAttributeList ra, int grade)
    {
        RoleAttributeList gradeEffect = AdolescentSet.AttritubeShowByLevel_basic(grade
            , 0.6f//生命
            , 0.75f//法力
            , 0.6f//怒气
            , 0.2f, 0.2f//物攻防
            , 1.1f, 0.6f//法攻防
            );
        RoleBasicRA = ra + gradeEffect;//等级加成
        AddTempleMultiplier(Job.Priest);
        //额外效果
        CRIDmg += 25;
    }
    #endregion
    #region 种族(阵营)
    public void initData_supplement_H_human()
    {
        RoleBasicRA.MT = (int)(RoleBasicRA.MT * 0.9f);
        RoleBasicRA.MD = (int)(RoleBasicRA.MD * 0.9f);
    }
    public void initData_supplement_H_elf()
    {
        RoleBasicRA.Evo = (int)(RoleBasicRA.Evo * 1.1f);
        RoleBasicRA.Hp = (int)(RoleBasicRA.Hp * 0.9f);
        RoleBasicRA.AD = (int)(RoleBasicRA.AD * 0.8f);
    }
    public void initData_supplement_H_dragonborn()
    {
        RoleBasicRA.Hp = (int)(RoleBasicRA.Hp * 1.1f);
        RoleBasicRA.Crit = (int)(RoleBasicRA.Crit * 0.85f);
        RoleBasicRA.Evo = (int)(RoleBasicRA.Evo * 0.85f);
    }
    #endregion

    public void AddMultiplier(int grade)
    {
        //Hp
        RoleBasicRA.Hp = (int)(RoleBasicRA.Hp*(1f + grade * 1f / UpGradeGap * 0.15f))
            + grade * 2;
        RoleBasicRA.Hp = (int)(RoleBasicRA.Hp * (LEVEL * 0.15f + 1));
        //Mp
        RoleBasicRA.Mp = (int)(RoleBasicRA.Mp*(1f + grade * 1f / UpGradeGap * 0.15f)) 
            + grade;
        RoleBasicRA.Mp = (int)(RoleBasicRA.Mp * (LEVEL * 0.15f + 1));
        //Atd
        RoleBasicRA.AT = (int)(RoleBasicRA.AT * (1f + grade * 1f / UpGradeGap * 0.15f))
        + (int)(grade*0.5f);
        RoleBasicRA.AT = (int)(RoleBasicRA.AT * (LEVEL * 0.15f + 1));
        RoleBasicRA.AD = (int)(RoleBasicRA.AD * (1f + grade * 1f / UpGradeGap * 0.15f))
        + (int)(grade * 0.25f);
        RoleBasicRA.AD = (int)(RoleBasicRA.AD * (LEVEL * 0.15f + 1));
        //Mtd
        RoleBasicRA.MT = (int)(RoleBasicRA.MT * (1f + grade * 1f / UpGradeGap * 0.15f))
        + (int)(grade * 0.5f);
        RoleBasicRA.MT = (int)(RoleBasicRA.MT * (LEVEL * 0.15f + 1));
        RoleBasicRA.MD = (int)(RoleBasicRA.MD * (1f + grade * 1f / UpGradeGap * 0.15f))
        + (int)(grade * 0.25f);
        RoleBasicRA.MD = (int)(RoleBasicRA.MD * (LEVEL * 0.15f + 1));
    }
    public void AddWakeMultiplier(int wakeNum)
    {
        RoleBasicRA.Hp = (int)(RoleBasicRA.Hp * (wakeNum * 0.75f + 1));
        RoleBasicRA.Mp = (int)(RoleBasicRA.Mp * (wakeNum * 0.75f + 1));
        RoleBasicRA.AT = (int)(RoleBasicRA.AT * (wakeNum * 0.75f + 1));
        RoleBasicRA.MT = (int)(RoleBasicRA.MT * (wakeNum * 0.75f + 1));
    }
    public void AddTempleMultiplier(Job job)
    {
        for(int i = 0; i < (int)AttributeData.End; i++)
        {
            RoleBasicRA.AllAttributeData[i] 
                += SDDataManager.Instance.getTempleByType(job, (AttributeData)i);
        }
    }
    public void AddLEVELNumMultiplier(int LEVELNum)
    {
        if(LEVELNum > SDConstants.LEVELHaveSpecialBuff)
        {
            int d = LEVELNum - SDConstants.LEVELHaveSpecialBuff;
            float buff = d * d * 0.25f  + 1;

            RoleBasicRA.Hp = (int)(RoleBasicRA.Hp * buff);
            RoleBasicRA.Mp = (int)(RoleBasicRA.Mp * buff);
            RoleBasicRA.AT = (int)(RoleBasicRA.AT * buff);
            RoleBasicRA.AD = (int)(RoleBasicRA.AD * buff);
            RoleBasicRA.MT = (int)(RoleBasicRA.MT * buff);
            RoleBasicRA.MD = (int)(RoleBasicRA.MD * buff);
        }
    }
    public virtual void initRoleClassData()
    {
        _role.ThisRoleAttributes = RoleBasicRA;
    }


    #region 属性引用
    public int ReadRA(int index,bool notRisist = true)
    {
        if (notRisist)
        {
            int s = _role.ReadCurrentRoleRA((AttributeData)index);
            return s;
        }
        else
        {
            int s = _role.ReadCurrentRoleRA((StateTag)index);
            return s;
        }
    }
    public int ReadRA(AttributeData tag)
    {
        return ReadRA((int)tag);
    }
    public int ReadRA(StateTag tag)
    {
        return ReadRA((int)tag, false);
    }
    #endregion
    public void PassiveEffectInit(string effect)
    {
        
    }
}
