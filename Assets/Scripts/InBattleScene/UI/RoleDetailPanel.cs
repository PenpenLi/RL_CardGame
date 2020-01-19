using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleDetailPanel : MonoBehaviour
{
    public Image BG;
    public Text TEXT;
    public string NameBeforeString;
    public string NameString;
    public string RaceString;
    public string CareerString;
    public string DetailString;
    public void initThisPanel(BattleRoleData unit)
    {
        NameString = unit.ThisBasicRoleProperty().Name;
        if (!unit.IsEnemy)
        {
            RaceString = heroRace((int)unit.HeroProperty._hero._heroRace);
            CareerString = heroCareer((int)unit.HeroProperty._hero._heroJob);
            if (unit.HeroProperty._hero.nameBeforeId != 0)
            {
                NameBeforeString = nameBeforeReturn(unit.HeroProperty._hero.nameBeforeId);
            }
        }
        else
        {
            RaceString = enemyRace((int)unit.EnemyProperty._enemy.race);
            CareerString = enemyCareer((int)unit.EnemyProperty._enemy._job);
        }
        string all = "";
        all += NameBeforeString != null?"<size=10><i>" + NameBeforeString + "</i></size></n>"
            : "<size=10> </size></n>";
        all += "<size=20>" + NameString + "</size></n>";
        all += CareerString != null ? CareerString : "";
        all += CareerString != null && RaceString != null ? "·" : "" 
            + (RaceString != null ? RaceString : "</n>");
        all += DetailString;
        TEXT.text = all;
    }
    #region career && race
    public string enemyCareer(int career)
    {
        return null;
    }
    public string enemyRace(int race)
    {
        switch (race)
        {
            case 0:
                return "元素生物";
            case 1:
                return "地精";
            case 2:
                return "兽人";
            case 3:
                return "野兽";
            default:
                return null;
        }
    }

    public string heroCareer(int career)
    {
        switch (career)
        {
            case 0:
                return "战士";
            case 1:
                return "游侠";
            case 2:
                return "牧师";
            case 3:
                return "法师";
            default:
                return null;
        }
    }
    public string heroRace(int race)
    {
        switch (race)
        {
            case 0:
                return "人类";
            case 1:
                return "精灵";
            case 2:
                return "龙裔";
            default:
                return null;
        }
    }
    #endregion
    #region nameBefore
    public string nameBeforeReturn(int id,bool isEnemy = false)
    {
        //List<Dictionary<string, string>> list = SDDataManager.Instance.ReadFromCSV("nameBefore");

        return null;
    }
    #endregion
}
