using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameDataEditor;
using I2.Loc;

public class HomeScene : MonoBehaviour
{
    public enum HomeSceneSubMenu
    {
        LevelEnter
            ,
        BattleTeam
            ,
        HeroDetail
            ,
        Store
        ,
        SummonAltar
        ,
        MainCastle
        ,
        Depository
        ,
        End
    }
    public HomeSceneSubMenu CurrentSubMenuType = HomeSceneSubMenu.End;

    public Transform LevelEnterPanel;
    public Transform BattleTeamPanel;
    public Transform HeroDetailPanel;
    public Transform DepositoryPanel;
    public Transform SummonAltarPanel;
    [Space(25)]
    public Transform OuterMenuPanel;
    [Header("场景配置")]
    public Transform leftArrow;
    public Transform rightArrow;

    // Start is called before the first frame update
    void Start()
    {
        //HeroDetailPanel.GetComponent<AllOwnedHeroesPanel>().ShowAllOwnedHeroes();
        closeAllSubMenu();
        buildPlayerOwned();
        buildPlayerOwned_equip();
        buildPlayerOwned_material();
    }
    #region 场景建筑按钮
    public void levelEnterBtnTapped()
    {
        closeAllSubMenu();
        //SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.LevelEnter;
        UIEffectManager.Instance.showAnimFadeIn(LevelEnterPanel);

        LevelEnterPanel.GetComponent<LevelEnterPanel>().WhenOpenThisMenu();
    }
    public void battleTeamBtnTapped(bool EnterLevel = false)
    {
        closeAllSubMenu();
        CurrentSubMenuType = HomeSceneSubMenu.BattleTeam;
        UIEffectManager.Instance.showAnimFadeIn(BattleTeamPanel);

        BattleTeamPanel.GetComponent<BattleTeamPanel>().WhenOpenThisMenu();
    }
    public void allRoleDetailBtnTapped()
    {
        closeAllSubMenu();
        //SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.HeroDetail;
        UIEffectManager.Instance.showAnimFadeIn(HeroDetailPanel);

        HeroDetailPanel.GetComponent<AllOwnedHeroesPanel>().ShowAllOwnedHeroes();
    }
    public void summonAltarBtnTapped()
    {
        closeAllSubMenu();
        CurrentSubMenuType = HomeSceneSubMenu.SummonAltar;
        UIEffectManager.Instance.showAnimFadeIn(SummonAltarPanel);

        SummonAltarPanel.GetComponent<SummonAltarPanel>().whenOpenThisPanel();
    }
    #endregion
    public Transform getSubMenuByType(HomeSceneSubMenu type)
    {
        switch (type)
        {
            case HomeSceneSubMenu.LevelEnter:return LevelEnterPanel;
            case HomeSceneSubMenu.BattleTeam:return BattleTeamPanel;
            case HomeSceneSubMenu.HeroDetail:return HeroDetailPanel;
            case HomeSceneSubMenu.SummonAltar:return SummonAltarPanel;
            default:return null;//end索引
        }
    }
    public void resetOneSubMenu(HomeSceneSubMenu type)
    {
        switch (type)
        {
            case HomeSceneSubMenu.LevelEnter:
                //LevelEnterPanel.GetComponentInChildren<BasicHeroSelect>().pageController.ResetPage();
                break;
            case HomeSceneSubMenu.BattleTeam:
                if(BattleTeamPanel.GetComponentInChildren<BasicHeroSelect>())
                    BattleTeamPanel.GetComponentInChildren<BasicHeroSelect>().pageController.ResetPage();
                break;
            case HomeSceneSubMenu.HeroDetail:
                //HeroDetailPanel.GetComponentInChildren<BasicHeroSelect>().pageController.ResetPage();
                break;
        }
    }
    public void SubMenuClose(bool CloseAllSubMenu = false)
    {
        if(CurrentSubMenuType!= HomeSceneSubMenu.End)
        {
            if (!CloseAllSubMenu)
            {
                UIEffectManager.Instance.hideAnimFadeOut(getSubMenuByType(CurrentSubMenuType));
                resetOneSubMenu(CurrentSubMenuType);
            }
            else
            {
                for(int i = 0; i < (int)HomeSceneSubMenu.End; i++)
                {
                    if (getSubMenuByType((HomeSceneSubMenu)i) != null)
                    {
                        UIEffectManager.Instance.hideAnimFadeOut(getSubMenuByType((HomeSceneSubMenu)i));
                        resetOneSubMenu((HomeSceneSubMenu)i);
                    }
                }
            }
            CurrentSubMenuType = HomeSceneSubMenu.End;
        }
    }
    public void closeAllSubMenu()
    {
        for (int i = 0; i < (int)HomeSceneSubMenu.End; i++)
        {
            if (getSubMenuByType((HomeSceneSubMenu)i) != null)
            {
                getSubMenuByType((HomeSceneSubMenu)i).localScale = Vector2.zero;
                resetOneSubMenu((HomeSceneSubMenu)i);
            }
        }
    }
    public void OuterMenu_backBtnTapped()
    {
        SubMenuClose();
    }
    public void changeDayNightByHour()
    {
        int forwardHour = SDDataManager.Instance.OpenTime.Hour;
        bool changeDayNight = false;
        if (DateTime.Now.Hour >= forwardHour + SDConstants.HourToChangeDayNight
            || DateTime.Now.Hour < forwardHour)
        {
            changeDayNight = true;
        }
        else if (DateTime.Now.Day != SDDataManager.Instance.OpenTime.Day)
            changeDayNight = true;
        else return;
        if (changeDayNight)
        {
            int a = SDDataManager.Instance.ResidentMovementData.CurrentDayNightId;
            a++; a %= 2;
            SDDataManager.Instance.ResidentMovementData.CurrentDayNightId = a;
        }
    }

    private void FixedUpdate()
    {
        changeDayNightByHour();
    }



    #region 存档生成（测试用）
    #region 角色
    public void buildPlayerOwned()
    {
        List<Dictionary<string, string>> all = new List<Dictionary<string, string>>();
        for (int i = 0; i < (int)Job.End; i++)
        {
            List<Dictionary<string, string>> list = SDDataManager.Instance.ReadHeroFromCSV(i);
            for (int k = 0; k < list.Count; k++)
            {
                all.Add(list[k]);
                //Debug.Log(all[all.Count-1]["id"] +" "+ all[all.Count - 1]["name"]);
            }
        }

        all.Sort((x, y) =>
        {
            return SDDataManager.Instance.getInteger(x["id"]).CompareTo
            (SDDataManager.Instance.getInteger(y["id"]));
        });
        for (int i = 0; i < all.Count; i++)
        {
            Dictionary<string, string> s = all[i];
            int id = SDDataManager.Instance.getInteger(s["id"]);
            allRId.Add(id);
        }


        SDDataManager.Instance.SettingData.seatUnlocked = new List<int>();
        for (int i = 0; i < SDConstants.MaxSelfNum; i++)
        {
            SDDataManager.Instance.SettingData.seatUnlocked.Add(1);
        }
        SDDataManager.Instance.SettingData.Set_seatUnlocked();
        //StartCoroutine(WriteToOwned());
    }
    [HideInInspector]
    public List<int> allRId = new List<int>();
    public void AddHeroToPlayerData(int index)
    {
        GDEHeroData hero = new GDEHeroData(GDEItemKeys.Hero_BasicHero);
        hero.id = allRId[index];
        SDDataManager.Instance.heroNum++;
        hero.hashCode = SDDataManager.Instance.heroNum;
        hero.status = 0;
        hero.starNumUpgradeTimes = 1;
        SDDataManager.Instance.PlayerData.herosOwned.Add(hero);
        SDDataManager.Instance.PlayerData.Set_herosOwned();
        //read();
    }
    public IEnumerator WriteToOwned()
    {
        for (int i = 0; i < allRId.Count; i++)
        {
            AddHeroToPlayerData(i);
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("写入完成");
    }
    #endregion
    #region 装备
    public void buildPlayerOwned_equip()
    {
        List<Dictionary<string, string>> all = new List<Dictionary<string, string>>();
        List<Dictionary<string, string>> equipList = SDDataManager.Instance.ReadFromCSV("equip");
        List<Dictionary<string, string>> jewelryList = SDDataManager.Instance.ReadFromCSV("jewelry");
        List<Dictionary<string, string>> weaponList = SDDataManager.Instance.ReadFromCSV("weapon");
        for (int i = 0; i < equipList.Count; i++) { all.Add(equipList[i]); }
        for (int i = 0; i < jewelryList.Count; i++) { all.Add(jewelryList[i]); }
        for (int i = 0; i < weaponList.Count; i++) { all.Add(weaponList[i]); }
        for (int i = 0; i < all.Count; i++)
        {
            Dictionary<string, string> s = all[i];
            int id = SDDataManager.Instance.getInteger(s["id"]);
            allRId_equip.Add(id);
        }
        allRId_equip.Sort((x, y) =>
        {
            return (x % 10000).CompareTo(y % 10000);
        }
        );
        StartCoroutine(WriteToOwned_equip());
    }
    [HideInInspector]
    public List<int> allRId_equip = new List<int>();
    public void AddEquipToPlayerData_equip(int index)
    {
        GDEEquipmentData equip = new GDEEquipmentData(GDEItemKeys.Equipment_EquipEmpty);
        equip.equipId = allRId_equip[index];
        SDDataManager.Instance.equipNum++;
        equip.hashcode = SDDataManager.Instance.equipNum;
        equip.OwnerHashcode = 0;
        SDDataManager.Instance.PlayerData.equipsOwned.Add(equip);
        SDDataManager.Instance.PlayerData.Set_equipsOwned();
        //read();
    }
    public IEnumerator WriteToOwned_equip()
    {
        for (int i = 0; i < allRId_equip.Count; i++)
        {
            AddEquipToPlayerData_equip(i);
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("写入完成---装备");
    }
    #endregion
    #region 材料
    public void buildPlayerOwned_material()
    {
        List<Dictionary<string, string>> all = SDDataManager.Instance.ReadFromCSV("material");
        for(int i = 0; i < all.Count; i++)
        {
            int id = SDDataManager.Instance.getInteger(all[i]["id"]);
            allRId_material.Add(id);
        }
        allRId_material.Sort();
        StartCoroutine(WriteToOwned_material());
    }
    [HideInInspector]
    public List<int> allRId_material = new List<int>();
    public void AddMaterialToPlayerData(int index)
    {
        GDEAMaterialData material = new GDEAMaterialData(GDEItemKeys.AMaterial_MaterialEmpty);
        material.id = allRId_material[index];
        material.num = 5;
        SDDataManager.Instance.PlayerData.materials.Add(material);
        SDDataManager.Instance.PlayerData.Set_materials();
    }
    public IEnumerator WriteToOwned_material()
    {
        for(int i = 0; i < allRId_material.Count; i++)
        {
            AddMaterialToPlayerData(i);
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("写入完成_材料");
    }
    #endregion
    #endregion
}
