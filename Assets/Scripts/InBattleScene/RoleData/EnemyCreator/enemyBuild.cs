using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBuild : MonoBehaviour
{
    #region 生成Enemy
    public static bool CanBeUsedInThisLevel(int weight)
    {
        int lvl = SDGameManager.Instance.currentLevel;
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            if(weight <= lvl)
            {
                return true;
            }
        }
        return false;
    }
    public static int createCareerByLevel()
    {
        int result = 0;
        int lvl = SDGameManager.Instance.currentLevel;
        int round0 = lvl % SDConstants.LevelNumPerSection;
        int round1 = ((lvl % SDConstants.PerBossAppearLevel) 
            - (lvl % SDConstants.PerBossAppearLevel)%SDConstants.LevelNumPerSection)
            / SDConstants.LevelNumPerSection;
        int round2 = ((lvl % SDConstants.LevelNumPerChapter)
            - (lvl % SDConstants.LevelNumPerChapter)%SDConstants.PerBossAppearLevel)
            / SDConstants.PerBossAppearLevel;
        result = Mathf.Clamp(round2 - 1, 0, SDConstants.MaxOtherNum);
        if(SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            if (round0 == SDConstants.LevelNumPerSection - 1)
                result = Mathf.Min(SDConstants.MaxOtherNum, round1 + 1);
            if(lvl % SDConstants.PerBossAppearLevel == SDConstants.PerBossAppearLevel - 1)
            {
                result = Mathf.Min(SDConstants.MaxOtherNum, round1 + 2);
            }
            if (lvl % SDConstants.LevelNumPerChapter == SDConstants.LevelNumPerChapter - 1)
            {
                result = SDConstants.MaxOtherNum;
            }
        }
        return result;
    }
    #region career&&race _data
    public static ROHeroData AddCareerData(ROHeroData basicData, int career, int race)
    {
        ROHeroData h = basicData;
        h.Name += RaceCareerAddName(career, race);
        if(race == 1 || race == 2 || race == 3)
        {
            if (career == 0)//fighter
            {
                h.DmgReduction = 10;
                h.BasicRAL.AT = (int)(h.BasicRAL.AT*1.15f);
                h.BasicRAL.AD = (int)(h.BasicRAL.AD*1.25f);
            }
            else if (career == 1)//ranger
            {
                h.CRI = 50;
                h.BasicRAL.AT = (int)(h.BasicRAL.AT * 1.25f);
                h.BasicRAL.Evo += 10;
            }
            else if (career == 2)//priest
            {
                h.BasicRAL.Hp = (int)(h.BasicRAL.Hp * 1.5f);
                h.BasicRAL.MT += 10;
                h.BasicRAL.MD += 10;
            }
            else if(career == 3)//caster
            {
                h.CRI = 50;
                h.BasicRAL.MT += 10;
                h.BasicRAL.MD += 10;
                h.BasicRAL.Expect += 10;
            }
        }
        return h;
    } 

    public static string RaceCareerAddName(int career , int race)
    {
        string s = "";
        if (race == 0)//元素类
        {
            s = ElementalCareerName((Job)career);
        }
        else if (race == 1)//地精类
        {
            s = GoblinCareerName((Job)career);
        }
        else if(race == 2)//绿皮类
        {
            s = OrcCareerName((Job)career);
        }
        else if(race == 3)//野兽类
        {
            s = BeastCareerName((Job)career);
        }
        return s;
    }
    public static string ElementalCareerName(Job career)
    {
        switch (career)
        {
            case Job.Fighter:
                return "·硬化态";
            case Job.Ranger:
                return "·尖刺态";
            case Job.Priest:
                return "·生长态";
            case Job.Caster:
                return "·放射态";
        }
        return "";
    }
    public static string GoblinCareerName(Job career)
    {
        switch (career)
        {
            case Job.Fighter:
                return "力士";
            case Job.Ranger:
                return "刺客";
            case Job.Priest:
                return "巫医";
            case Job.Caster:
                return "术士";
        }
        return "";
    }
    public static string OrcCareerName(Job career)
    {
        switch (career)
        {
            case Job.Fighter:
                return "大块头";
            case Job.Ranger:
                return "偷袭者";
            case Job.Priest:
                return "先知";
            case Job.Caster:
                return "萨满";
        }
        return "";
    }
    public static string BeastCareerName(Job career)
    {
        switch (career)
        {
            case Job.Fighter:
                return "·结实";
            case Job.Ranger:
                return "·嗜血";
            case Job.Priest:
                return "·支撑";
            case Job.Caster:
                return "·魔化";
        }
        return "";
    }
    #endregion
    #region levelAdd_data
    public static RoleAttributeList RALAddedByLevel(RoleAttributeList basicRAL)
    {
        RoleAttributeList ral = basicRAL;
        int lvl = SDGameManager.Instance.currentLevel;
        //每章结束循环利用数据
        int index = lvl / SDConstants.LevelNumPerChapter;
        ral.AddPerc(75 * index, AttributeData.Hp);
        ral.AddPerc(75 * index, AttributeData.AT);
        ral.AddPerc(75 * index, AttributeData.AD);
        ral.AddPerc(75 * index, AttributeData.MT);
        ral.AddPerc(75 * index, AttributeData.MD);
        ral.Add(5 * index, AttributeData.Evo);
        ral.Add(10 * index, AttributeData.Accur);
        ral.Add(2 * index, AttributeData.Speed);
        return ral;
    }
    #endregion
    #endregion
    #region buildSkills

    public List<OneSkill> WriteEnemySkills(int career , int race , int weight, bool isBoss = false )
    {
        switch (race)
        {
            case 0:
                return ElementalsSkillList(career, isBoss);
            case 1:
                return GoblinsSkillList(career, isBoss);
            case 2:
                return OrcsSkillList(career, isBoss);
            case 3:
                return BeastSkillList(career, isBoss);
            default:
                if (isBoss)
                {

                }
                return null;
        }
    }
    public List<OneSkill> ElementalsSkillList(int career,bool isBoss = false)
    {
        return null;
    }
    public List<OneSkill> GoblinsSkillList(int career, bool isBoss = false)
    {
        return null;
    }
    public List<OneSkill> OrcsSkillList(int career, bool isBoss = false)
    {
        return null;
    }
    public List<OneSkill> BeastSkillList(int career, bool isBoss = false)
    {
        return null;
    }
    #endregion


    #region 生成Boss
    public static ROHeroData LittleBossData(ROHeroData basicD, int bossQuality)
    {
        ROHeroData h = basicD;
        string BaiscName = h.Name;
        h.BasicRAL.AddPercAll(100 * bossQuality + 250, RoleAttributeList.AddType.barChart);
        h.BasicRAL.AddPerc(100 * bossQuality + 125, AttributeData.AT);
        h.BasicRAL.AddPerc(100 * bossQuality + 125, AttributeData.AD);
        h.BasicRAL.AddPerc(100 * bossQuality + 125, AttributeData.MT);
        h.BasicRAL.AddPerc(100 * bossQuality + 125, AttributeData.MD);
        h.BasicRAL.AddPerc(50 * bossQuality + 100, AttributeData.Crit);
        h.BasicRAL.AddPerc(25 * bossQuality + 50, AttributeData.Expect);
        h.BasicRAL.AddPerc(25 * bossQuality + 50, AttributeData.Evo);
        h.BasicRAL.AddPerc(25 * bossQuality + 50, AttributeData.Accur);
        h.BasicRAL.AddPercAll(25 * bossQuality + 100, RoleAttributeList.AddType.resist);

        h.DmgReduction = 5;
        h.DmgReflection = 5;

        h.isRare = true;
        h.BarChartRegendPerTurn = RoleBarChart.zero;
        h.BarChartRegendPerTurn.MP += h.BarChartRegendPerTurn.MP / 25;
        return h;
    }
    #endregion
}
