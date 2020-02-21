using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBuild : MonoBehaviour
{
    #region 生成Enemy
    public static bool CanBeUsedInThisLevel(int weight)
    {
        int lvl = SDGameManager.Instance.currentLevel;
        lvl = lvl % SDConstants.LevelNumPerChapter;
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
        int result=0;
        int lvl = SDGameManager.Instance.currentLevel;
        int round0 = lvl % SDConstants.LevelNumPerSection;
        int round1 = lvl % SDConstants.LevelNumPerSerial;
        int round2 = lvl % SDConstants.LevelNumPerChapter;
        //0
        if(round0 > SDConstants.LevelNumPerSection - 2)
        {
            result ++;
        }
        //1
        float r = round1 * 1f / SDConstants.LevelNumPerSerial;
        if(lvl < SDConstants.LevelNumPerChapter)
        {
            r = Mathf.Max(0, r - 0.5f);
        }
        float _r = Random.Range(0f, 1f);
        if (_r < r) { result ++; }
        //2
        float r1 = Mathf.Max(round2 * 1f / SDConstants.LevelNumPerChapter-0.5f,0);
        float _r1 = Random.Range(0, 1f);
        if(_r1 < r1) { result++; }
        //3
        float r2 = round2 < SDConstants.LevelNumPerChapter / 100 ? 0 : 0.5f;
        float _r2 = Random.Range(0, 1f);
        if(_r2 < r2) { result++; }
        //
        result = Mathf.Min(result, SDConstants.MaxOtherNum);
        return result;
    }
    #region career&&race _data
    public static ROEnemyData AddCareerData(ROEnemyData basicData, int career)
    {
        ROEnemyData h = basicData;

        if (career == 0)//fighter
        {
            h.DmgReduction += 10;
            h.RALRate.AT += 15;
            h.RALRate.AD += 25;
        }
        else if (career == 1)//ranger
        {
            h.RALRate += 25;
            h.RALRate.AT += 25;
            h.RALRate.Evo += 50;
        }
        else if (career == 2)//priest
        {
            h.RALRate.Hp += 30;
            h.RALRate.MT += 10;
            h.RALRate.MD += 10;
        }
        else if (career == 3)//caster
        {
            h.RALRate += 25;
            h.RALRate.MT += 25;
            h.RALRate.MD += 15;
            h.RALRate.Expect += 10;
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

    public List<OneSkill> WriteEnemySkills(EnemyInfo info, bool isBoss = false )
    {
        if (info == null) return null;

        return null;
    }
    #endregion


    #region 生成Boss
    public static ROEnemyData LittleBossData(ROEnemyData basicD, int bossQuality)
    {
        ROEnemyData h = basicD;
        h.RALRate.AddAll(75 * bossQuality + 100, RoleAttributeList.AddType.barChart);
        h.RALRate.Add(75 * bossQuality + 75, AttributeData.AT);
        h.RALRate.Add(75 * bossQuality + 75, AttributeData.AD);
        h.RALRate.Add(75 * bossQuality + 75, AttributeData.MT);
        h.RALRate.Add(75 * bossQuality + 75, AttributeData.MD);
        h.RALRate.Add(50 * bossQuality + 50, AttributeData.Crit);
        h.RALRate.Add(25 * bossQuality + 50, AttributeData.Expect);
        h.RALRate.Add(25 * bossQuality + 50, AttributeData.Evo);
        h.RALRate.Add(25 * bossQuality + 50, AttributeData.Accur);
        h.RALRate.AddAll(50 * bossQuality + 75, RoleAttributeList.AddType.resist);

        h.DmgReduction = 5;
        h.DmgReflection = 5;

        h.BarChartRegendPerTurn = RoleBarChart.zero;
        h.BarChartRegendPerTurn.MP += h.BarChartRegendPerTurn.MP / 25;
        return h;
    }
    #endregion
}
