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
        MainCastle = 0
        ,
        LevelEnter =1
            ,
        BattleTeam=2
            ,
        HeroStage=3
            ,
        HeroDetails=4
        ,
        Store=5
        ,
        SummonAltar=6
        ,
        Depository=7
        ,
        Hospital=8
        ,
        Goddess=9
            ,
        GoddessDetail=10
            ,
        Equipment=11
            ,
        EquipDetail=12
            ,
        Mission=13
            ,
        Achievement=14
            ,
        PhysicalBuff=15
            ,
        ElementalBuff=16
            ,
        Factory=17
            ,
        End
    }
    private HomeSceneSubMenu _csm = HomeSceneSubMenu.End;
    public HomeSceneSubMenu CurrentSubMenuType
    {
        get { return _csm; }
        set 
        {
            if(value != _csm)
            {
                WhenChangingSubMenu(_csm, value);
                _csm = value;
            }

        }
    }
    public Transform _MainCastlePanel;
    public Transform _LevelEnterPanel;
    public Transform _BattleTeamPanel;
    public Transform _HeroStagePanel;
    public Transform _heroDetailPanel;
    public Transform _StorePanel;
    public Transform _DepositoryPanel;
    public Transform _SummonAltarPanel;
    public Transform _HospitalPanel;
    public Transform _GoddessPanel;
    public Transform _goddessDetailPanel;
    public Transform _equipmentPanel;
    public Transform _equipDetailPanel;
    public Transform _missionPanel;
    public Transform _achievementPanel;
    public Transform _physicalBuffPanel;
    public Transform _elementalBuffPanel;
    public Transform _factoryPanel;
    [Header("菜单快速索引")]
    public Transform[] AllSubMenus;
    [Space(25)]
    public Transform OuterMenuPanel;
    [Header("场景配置")]
    public Transform leftArrow;
    public Transform rightArrow;
    [HideInInspector]
    public List<int> menuEnterHistory;
    [Header("OuterMenu")]
    public Transform InSubMenuPanel;
    public Transform InHomeScenePanel;
    public Button backBtn;
    public Button mapBtn;
    public Button packBtn;

    [Header("特殊设置")]
    public Button SubMenuLvUpBtn;
    private void Awake()
    {
        //建筑Id设置
        for(int i = 0; i < AllSubMenus.Length; i++)
        {
            BasicSubMenuPanel panel = AllSubMenus[i].GetComponent<BasicSubMenuPanel>();
            panel.buildingId = "BUILD#"+(i + 1);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SDGameManager.Instance.INIT();
        //HeroStagePanel.GetComponent<AllOwnedHeroesPanel>().ShowAllOwnedHeroes();
        SubMenuClose(true);
        buildPlayerOwned();
        buildPlayerOwned_equip();
        buildPlayerOwned_material();
        SDDataManager.Instance.PlayerData.JianCai += 500;
        //
        changeToHomeScene();
        refreshAllBuildingCondition();

        //SDTaskManager.Instance.AddNewTask(checkMCP(),"MainCastleOpen");
        //UseKillTask.CreateTask(SDConstants.EnemyType.enemy.ToString(),1, null, null);
    }
    /*
    public IEnumerator checkMCP()
    {
        while (true)
        {
            Debug.Log("running");
            if (_MainCastlePanel.GetComponent<BasicSubMenuPanel>().thisMenuOpened)
            {
                Debug.Log("finished");
                break;
            }
            if (_SummonAltarPanel.GetComponent<BasicSubMenuPanel>().thisMenuOpened)
            {
                Debug.Log("stopped");
                SDTaskManager.Instance.getTaskByName("MainCastleOpen")?.stop();
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
    */

    public void refreshAllBuildingCondition()
    {
        _MainCastlePanel.GetComponent<MainCastlePanel>().refreshAllBuildingsInfor();
    }
    #region 场景建筑按钮
    public void levelEnterBtnTapped(bool fromSubMenu = false)
    {
        if(!fromSubMenu)
            SubMenuClose(true);
        //SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.LevelEnter;
        UIEffectManager.Instance.showAnimFadeIn(_LevelEnterPanel);

        _LevelEnterPanel.GetComponent<LevelEnterPanel>().whenOpenThisPanel();
    }
    public void battleTeamBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu)
        {
            SubMenuClose(true);
        }
        else
        {
            _BattleTeamPanel.GetComponent<BattleTeamPanel>()
                .panelFrom = CurrentSubMenuType;
        }
        CurrentSubMenuType = HomeSceneSubMenu.BattleTeam;
        UIEffectManager.Instance.showAnimFadeIn(_BattleTeamPanel);

        _BattleTeamPanel.GetComponent<BattleTeamPanel>().whenOpenThisPanel();
    }
    public void RoleStageBtnTapped(bool fromSubMenu = false)
    {
        if(!fromSubMenu)
            SubMenuClose(true);
        //SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.HeroStage;
        UIEffectManager.Instance.showAnimFadeIn(_HeroStagePanel);

        _HeroStagePanel.GetComponent<AllOwnedHeroesPanel>().whenOpenThisPanel();
    }
    public void summonAltarBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.SummonAltar;
        UIEffectManager.Instance.showAnimFadeIn(_SummonAltarPanel);

        _SummonAltarPanel.GetComponent<SummonAltarPanel>().whenOpenThisPanel();
    }
    public void hospitalBtnTapped(bool fromSubMenu = false)
    {
        if(!fromSubMenu)SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.Hospital;
        UIEffectManager.Instance.showAnimFadeIn(_HospitalPanel);

        _HospitalPanel.GetComponent<HospitalPanel>().whenOpenThisPanel();
    }
    public void heroDetailBtnTapped(bool fromSubMenu = true)
    {
        if (!fromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.HeroDetails;
        UIEffectManager.Instance.showAnimFadeIn(_heroDetailPanel);

        _heroDetailPanel.GetComponent<HeroDetailPanel>().whenOpenThisPanel();
    }
    public void storeBtnTapped(bool fromSubMenu = true)
    {
        if (!fromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.Store;
        UIEffectManager.Instance.showAnimFadeIn(_StorePanel);

        _StorePanel.GetComponent<StorePanel>().whenOpenThisPanel();
    }
    public void depositoryBtnTapped(bool fromSubMenu = false)
    {
        if(!fromSubMenu)
            SubMenuClose(true);

        CurrentSubMenuType = HomeSceneSubMenu.Depository;
        UIEffectManager.Instance.showAnimFadeIn(_DepositoryPanel);

        _DepositoryPanel.GetComponent<DepositoryPanel>().whenOpenThisPanel();
    }
    public void mainCastleBtnTapped(bool FromSubMenu = false)
    {
        if (!FromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.MainCastle;
        UIEffectManager.Instance.showAnimFadeIn(_MainCastlePanel);

        _MainCastlePanel.GetComponent<MainCastlePanel>().whenOpenThisPanel();
    }
    public void goddessBtnTapped(bool FromSubMenu = false)
    {
        if (!FromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.Goddess;
        UIEffectManager.Instance.showAnimFadeIn(_GoddessPanel);

        _GoddessPanel.GetComponent<GoddessPanel>().whenOpenThisPanel();
    }
    public void goddessDetailBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.GoddessDetail;
        UIEffectManager.Instance.showAnimFadeIn(_goddessDetailPanel);

        _goddessDetailPanel.GetComponent<GoddessDetailPanel>().whenOpenThisPanel();
    }
    public void equipmentBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.Equipment;
        UIEffectManager.Instance.showAnimFadeIn(_equipmentPanel);

        _equipmentPanel.GetComponent<EquipmentPanel>().whenOpenThisPanel();
    }
    public void equipDetailBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.EquipDetail;
        UIEffectManager.Instance.showAnimFadeIn(_equipDetailPanel);

        _equipDetailPanel.GetComponent<EquipDetailPanel>().whenOpenThisPanel();
    }
    public void missionBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.Mission;
        UIEffectManager.Instance.showAnimFadeIn(_missionPanel);

        _missionPanel.GetComponent<MissionPanel>().whenOpenThisPanel();
    }
    public void achievementBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.Achievement;
        UIEffectManager.Instance.showAnimFadeIn(_achievementPanel);

        _achievementPanel.GetComponent<AchievementPanel>().whenOpenThisPanel();
    }
    public void physicalBuffBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.PhysicalBuff;
        UIEffectManager.Instance.showAnimFadeIn(_physicalBuffPanel);

        _physicalBuffPanel.GetComponent<PhysicalBuffPanel>().whenOpenThisPanel();
    }
    public void elementalBuffBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.ElementalBuff;
        UIEffectManager.Instance.showAnimFadeIn(_elementalBuffPanel);

        _elementalBuffPanel.GetComponent<ElementalBuffPanel>().whenOpenThisPanel();
    }
    public void factoryBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        CurrentSubMenuType = HomeSceneSubMenu.Factory;
        UIEffectManager.Instance.showAnimFadeIn(_factoryPanel);

        _factoryPanel.GetComponent<FactoryPanel>().whenOpenThisPanel();
    }
    /// <summary>
    /// 快速进入对应子菜单
    /// </summary>
    /// <param name="SMT">子菜单索引</param>
    /// <param name="fromSubMenu"></param>
    public void UseSMTToSubMenu(HomeSceneSubMenu SMT,bool fromSubMenu = false)
    {
        if(SMT == HomeSceneSubMenu.LevelEnter)
        {
            levelEnterBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.BattleTeam)
        {
            battleTeamBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.HeroStage)
        {
            RoleStageBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.SummonAltar)
        {
            summonAltarBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.Hospital)
        {
            hospitalBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.HeroDetails)
        {
            heroDetailBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.Depository)
        {
            depositoryBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.Store)
        {
            storeBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.MainCastle)
        {
            mainCastleBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.Goddess)
        {
            goddessBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.Equipment)
        {
            equipmentBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.EquipDetail)
        {
            equipDetailBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.Mission)
        {
            missionBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.Achievement)
        {
            achievementBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.PhysicalBuff)
        {
            physicalBuffBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.ElementalBuff)
        {
            elementalBuffBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.Factory)
        {
            factoryBtnTapped(fromSubMenu);
        }
    }
    #endregion

    #region 快速索引器使用区域
    public void SubMenuClose(bool CloseAllSubMenu = false)
    {
        if(CurrentSubMenuType != HomeSceneSubMenu.End)
        {
            int type = (int)CurrentSubMenuType;
            if (!CloseAllSubMenu)
            {
                UIEffectManager.Instance.hideAnimFadeOut(AllSubMenus[type]);
                resetOneSubMenu(CurrentSubMenuType);
                CurrentSubMenuType = HomeSceneSubMenu.End;
                return;
            }
        }
        for (int i = 0; i < AllSubMenus.Length; i++)
        {
        if (AllSubMenus[i] != null)
        {
                AllSubMenus[i].gameObject.SetActive(false);
                AllSubMenus[i].GetComponent<BasicSubMenuPanel>()._canvas.sortingOrder = -1;
                resetOneSubMenu((HomeSceneSubMenu)i);
        }
        }
        menuEnterHistory.Clear();
        CurrentSubMenuType = HomeSceneSubMenu.End;
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
    public void WhenChangingSubMenu
        (HomeSceneSubMenu oldOne, HomeSceneSubMenu newOne)
    {
        if(oldOne != HomeSceneSubMenu.End && newOne == HomeSceneSubMenu.End)
        {
            //返回城镇大街页面
            changeToHomeScene();
        }
        else if(oldOne == HomeSceneSubMenu.End && newOne != HomeSceneSubMenu.End)
        {
            //从城镇大厅进入功能建筑页面         
            changeToSubMenuScene();
        }

        if(oldOne != HomeSceneSubMenu.End)
        {
            AllSubMenus[(int)oldOne].GetComponent<BasicSubMenuPanel>()._canvas.sortingOrder = -1;
        }
        if(newOne != HomeSceneSubMenu.End)
        {
            AllSubMenus[(int)newOne].GetComponent<BasicSubMenuPanel>()._canvas.sortingOrder = 0;
        }

        if(oldOne == HomeSceneSubMenu.BattleTeam && newOne != oldOne)
        {
            _BattleTeamPanel.GetComponent<BattleTeamPanel>()
                .changeSpriterenderersStatus(true);
        }
        else if(newOne == HomeSceneSubMenu.BattleTeam && newOne != oldOne)
        {
            _BattleTeamPanel.GetComponent<BattleTeamPanel>()
                .changeSpriterenderersStatus(false);
        }
    }
    #endregion
    private void FixedUpdate()
    {
        changeDayNightByHour();
    }
    #region outerMenu
    public void changeToHomeScene()
    {
        InSubMenuPanel.gameObject.SetActive(false);
        InHomeScenePanel.gameObject.SetActive(true);
    }
    public void changeToSubMenuScene()
    {
        InSubMenuPanel.gameObject.SetActive(true);
        InHomeScenePanel.gameObject.SetActive(false);
    }
    public void BtnForBack()
    {
        if(CurrentSubMenuType != HomeSceneSubMenu.End)
            if (AllSubMenus[(int)CurrentSubMenuType] != null)
            {
                Debug.Log("BACK");
                resetOneSubMenu(CurrentSubMenuType);
            }
    }
    public void resetOneSubMenu(HomeSceneSubMenu type)
    {
        BasicSubMenuPanel panel = AllSubMenus[(int)type].GetComponent<BasicSubMenuPanel>();
        if (panel.thisMenuOpened) panel.commonBackAction();
    }


    public void checkLvUpBtn()
    {
        BasicSubMenuPanel P = AllSubMenus[(int)CurrentSubMenuType].GetComponent<BasicSubMenuPanel>();
        if (P.CheckIfCanLvUp())
        {
            int lv = SDDataManager.Instance.getLevelByExp(P.exp);
            SubMenuLvUpBtn.GetComponentInChildren<Text>().text
                = "升级 </n> 当前等级" + lv;
            SubMenuLvUpBtn.gameObject.SetActive(true);
        }
        else
        {
            SubMenuLvUpBtn.gameObject.SetActive(false);
        }
    }
    public void lvUpBtnTapped()
    {
        BasicSubMenuPanel P = AllSubMenus[(int)CurrentSubMenuType].GetComponent<BasicSubMenuPanel>();
        if (P.CheckIfCanLvUp())
        {
            P.BtnToLvUp();
        }
    }
    #endregion


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
            string id = s["id"];
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
    public List<string> allRId = new List<string>();
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
            string id = s["id"];
            allRId_equip.Add(id);
        }
        allRId_equip.Sort((x, y) =>
        {
            int _x 
            = SDDataManager.Instance.getInteger(x.Split('#')[1])/10000;
            int _y
            = SDDataManager.Instance.getInteger(y.Split('#')[1]) / 10000;
            return (_x).CompareTo(_y);
        }
        );
        StartCoroutine(WriteToOwned_equip());
    }
    [HideInInspector]
    public List<string> allRId_equip = new List<string>();
    public void AddEquipToPlayerData_equip(int index)
    {
        GDEEquipmentData equip = new GDEEquipmentData(GDEItemKeys.Equipment_EquipEmpty);
        equip.id = allRId_equip[index];
        SDDataManager.Instance.addEquip(equip);
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
            string id = all[i]["id"];
            allRId_material.Add(id);
        }
        allRId_material.Sort();
        StartCoroutine(WriteToOwned_material());
    }
    [HideInInspector]
    public List<string> allRId_material = new List<string>();
    public void AddMaterialToPlayerData(int index)
    {
        SDDataManager.Instance.addMaterial(allRId_material[index], 5);
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
