using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameDataEditor;
using I2.Loc;
using System.Linq;

public class HomeScene : MonoBehaviour
{
    public enum HomeSceneSubMenu
    {
        MainCastle = 0
        ,
        LevelEnter = 1
            ,
        BattleTeam = 2
            ,
        HeroStage = 3
            ,
        HeroDetails = 4
        ,
        Store = 5
        ,
        SummonAltar = 6
        ,
        Depository = 7
        ,
        Hospital = 8
        ,
        Goddess = 9
            ,
        GoddessDetail = 10
            ,
        Equipment = 11
            ,
        EquipDetail = 12
            ,
        Mission = 13
            ,
        Achievement = 14
            ,
        PhysicalBuff = 15
            ,
        ElementalBuff = 16
            ,
        Factory0 = 17
            ,
        Factory1 = 18
            ,
        Rune = 19
            ,
        Illustrate = 20
            ,
        End
    }
    [SerializeField]
#if UNITY_EDITOR
    [ReadOnly]
#endif
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
    public Transform _missionPanel;
    public Transform _achievementPanel;
    public Transform _physicalBuffPanel;
    public Transform _elementalBuffPanel;
    public Transform _factoryPanel0;
    public Transform _factoryPanel1;
    public Transform _runePanel;
    public Transform _illustratePanel;
    [Header("菜单快速索引")]
    public Transform[] AllSubMenus;
    [Space(25)]
    public Transform OuterMenuPanel;
    [HideInInspector]
    public List<int> menuEnterHistory;
    [Header("OuterMenu")]
    public Transform InSubMenuPanel;
    public Transform InHomeScenePanel;
    public Transform AbovePlayerMessagePanel;
    public Transform ExtraBtnList;
    public Button backBtn;
    public Button mapBtn;
    [Space]
    public OuterMenuController OuterMenu;
    [Space]
    [Header("特殊设置")]
    public Button SubMenuLvUpBtn;
    public HeroInfo BasicHero;
    public HeroInfo BasicAuxiliaryHero;
    public HeroAltarPool MoreHerosPool;
    public int MoreNum;
    private void Awake()
    {
        //建筑Id设置
        for(int i = 0; i < AllSubMenus.Length; i++)
        {
            if (AllSubMenus[i] == null) continue;
            BasicSubMenuPanel panel = AllSubMenus[i].GetComponent<BasicSubMenuPanel>();
            if(panel) panel.buildingId = "BUILD#"+(i + 1);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SDGameManager.Instance.INIT();

        //  DATA INIT
        SDDataManager.Instance.ReadAtlas();
        SDDataManager.Instance.ReadAllSOs();
        SDDataManager.Instance.ReadAllSkeletonAssets();
        //

        SubMenuClose(true);
        foreach(FactoryPanel p in FindObjectsOfType<FactoryPanel>())
        {
            p.InitThisFactoryTasks();
        }
        //
        changeToHomeScene();
        refreshAllBuildingCondition();
        refreshAllGoddessesCondition();
        //
        OuterMenu.gameObject.SetActive(true);
        OuterMenu.SettingPanel.gameObject.SetActive(false);
        RefreshDataInOuterMenu();
        //
        StartCoroutine(IEBuildFirstly());
    }
    IEnumerator IEBuildFirstly()
    {
        yield return new WaitForSeconds(0.15f);
        bool flag = BuildFirstEnterGameData();
        if (flag)
        {
            SDDataManager.Instance.PlayerData.JianCai += 500;
            SDDataManager.Instance.AddDamond(500);
            SDDataManager.Instance.AddCoin(50000);
            //
            SDDataManager.Instance.addConsumable
                (_SummonAltarPanel.GetComponent<SummonAltarPanel>().Coupon_n_oneTime.ID, 10);
            SDDataManager.Instance.addConsumable
                (_SummonAltarPanel.GetComponent<SummonAltarPanel>().Coupon_n_tenTimes.ID, 10);
            //
            AddHeroPools();

            test_runeAd t = FindObjectOfType<test_runeAd>();
            t.Test_AddConsumableItems();
            t.AddEquips();
            t.AddExpConsumables();
        }
    }
    public bool BuildFirstEnterGameData()
    {
        if (!SDDataManager.Instance.CheckHaveHeroById(BasicHero.ID))
        {
            SDDataManager.Instance.PlayerData.maxPassSection = -1;

            //
            SDDataManager.Instance.addHero(BasicHero.ID);
            SDDataManager.Instance.addHero(BasicAuxiliaryHero.ID);

            List<string> all = MoreHerosPool.HeroList.FindAll(x => x.Rarity < 2)
                .Select(x => x.ID).ToList(); ;
            for (int i = 0; i < MoreNum; i++)
            {
                SDDataManager.Instance.addHero(all[UnityEngine.Random.Range(0, all.Count)]);
            }
            return true;
        }
        return false;
    }

    public void refreshAllBuildingCondition()
    {
        _MainCastlePanel.GetComponent<MainCastlePanel>().refreshAllBuildingsInfor();
    }
    public void refreshAllGoddessesCondition()
    {
        _goddessDetailPanel.GetComponent<GoddessDetailPanel>().refreshAllGoddessesCondition();
    }
    #region 场景建筑按钮
    public void levelEnterBtnTapped(bool fromSubMenu = false)
    {
        if(!fromSubMenu)
            SubMenuClose(true);
        //
        _LevelEnterPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        //
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
        _BattleTeamPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.BattleTeam;
        UIEffectManager.Instance.showAnimFadeIn(_BattleTeamPanel);

        _BattleTeamPanel.GetComponent<BattleTeamPanel>().whenOpenThisPanel();
    }
    public void RoleStageBtnTapped(bool fromSubMenu = false)
    {
        if(!fromSubMenu)
            SubMenuClose(true);
        //SubMenuClose(true);
        _HeroStagePanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;

        CurrentSubMenuType = HomeSceneSubMenu.HeroStage;
        UIEffectManager.Instance.showAnimFadeIn(_HeroStagePanel);

        _HeroStagePanel.GetComponent<AllOwnedHeroesPanel>().whenOpenThisPanel();
    }
    public void summonAltarBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _SummonAltarPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.SummonAltar;
        UIEffectManager.Instance.showAnimFadeIn(_SummonAltarPanel);

        _SummonAltarPanel.GetComponent<SummonAltarPanel>().whenOpenThisPanel();
    }
    public void hospitalBtnTapped(bool fromSubMenu = false)
    {
        if(!fromSubMenu)SubMenuClose(true);
        _HospitalPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.Hospital;
        UIEffectManager.Instance.showAnimFadeIn(_HospitalPanel);

        _HospitalPanel.GetComponent<HospitalPanel>().whenOpenThisPanel();
    }
    public void heroDetailBtnTapped(bool fromSubMenu = true)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _heroDetailPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;

        CurrentSubMenuType = HomeSceneSubMenu.HeroDetails;
        UIEffectManager.Instance.showAnimFadeIn(_heroDetailPanel);

        _heroDetailPanel.GetComponent<HeroDetailPanel>().whenOpenThisPanel();
    }
    public void storeBtnTapped(bool fromSubMenu = true)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _StorePanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.Store;
        UIEffectManager.Instance.showAnimFadeIn(_StorePanel);

        _StorePanel.GetComponent<StorePanel>().whenOpenThisPanel();
    }
    public void depositoryBtnTapped(bool fromSubMenu = false)
    {
        if(!fromSubMenu)
            SubMenuClose(true);
        _DepositoryPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.Depository;
        UIEffectManager.Instance.showAnimFadeIn(_DepositoryPanel);

        _DepositoryPanel.GetComponent<DepositoryPanel>().whenOpenThisPanel();
    }
    public void mainCastleBtnTapped(bool FromSubMenu = false)
    {
        if (!FromSubMenu) SubMenuClose(true);
        _MainCastlePanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.MainCastle;
        UIEffectManager.Instance.showAnimFadeIn(_MainCastlePanel);

        _MainCastlePanel.GetComponent<MainCastlePanel>().whenOpenThisPanel();
    }
    public void goddessBtnTapped(bool FromSubMenu = false)
    {
        if (!FromSubMenu) SubMenuClose(true);
        _GoddessPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.Goddess;
        UIEffectManager.Instance.showAnimFadeIn(_GoddessPanel);

        _GoddessPanel.GetComponent<GoddessPanel>().whenOpenThisPanel();
    }
    public void goddessDetailBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _goddessDetailPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.GoddessDetail;
        UIEffectManager.Instance.showAnimFadeIn(_goddessDetailPanel);

        _goddessDetailPanel.GetComponent<GoddessDetailPanel>().whenOpenThisPanel();
    }
    public void equipmentBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _equipmentPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.Equipment;
        UIEffectManager.Instance.showAnimFadeIn(_equipmentPanel);

        _equipmentPanel.GetComponent<EquipmentPanel>().whenOpenThisPanel();
    }
    public void missionBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _missionPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.Mission;
        UIEffectManager.Instance.showAnimFadeIn(_missionPanel);

        _missionPanel.GetComponent<MissionPanel>().whenOpenThisPanel();
    }
    public void achievementBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _achievementPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.Achievement;
        UIEffectManager.Instance.showAnimFadeIn(_achievementPanel);

        _achievementPanel.GetComponent<AchievementPanel>().whenOpenThisPanel();
    }
    public void physicalBuffBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _physicalBuffPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.PhysicalBuff;
        UIEffectManager.Instance.showAnimFadeIn(_physicalBuffPanel);

        _physicalBuffPanel.GetComponent<PhysicalBuffPanel>().whenOpenThisPanel();
    }
    public void elementalBuffBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _elementalBuffPanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.ElementalBuff;
        UIEffectManager.Instance.showAnimFadeIn(_elementalBuffPanel);

        _elementalBuffPanel.GetComponent<ElementalBuffPanel>().whenOpenThisPanel();
    }
    public void factory0BtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _factoryPanel0.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.Factory0;
        UIEffectManager.Instance.showAnimFadeIn(_factoryPanel0);

        _factoryPanel0.GetComponent<FactoryPanel>().whenOpenThisPanel();
    }
    public void factory1BtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _factoryPanel1.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.Factory1;
        UIEffectManager.Instance.showAnimFadeIn(_factoryPanel1);

        _factoryPanel1.GetComponent<FactoryPanel>().whenOpenThisPanel();
    }
    public void runeBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _runePanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.Rune;
        UIEffectManager.Instance.showAnimFadeIn(_runePanel);
        _runePanel.GetComponent<RunePanel>().whenOpenThisPanel();
    }
    public void illustrateBtnTapped(bool fromSubMenu = false)
    {
        if (!fromSubMenu) SubMenuClose(true);
        _illustratePanel.GetComponent<BasicSubMenuPanel>().panelFrom = CurrentSubMenuType;
        CurrentSubMenuType = HomeSceneSubMenu.Illustrate;
        UIEffectManager.Instance.showAnimFadeIn(_illustratePanel);
        _illustratePanel.GetComponent<IllustratePanel>().whenOpenThisPanel();
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
            //equipDetailBtnTapped(fromSubMenu);
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
        else if(SMT == HomeSceneSubMenu.Factory0)
        {
            factory0BtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.Factory1)
        {
            factory1BtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.Rune)
        {
            runeBtnTapped(fromSubMenu);
        }
        else if(SMT == HomeSceneSubMenu.Illustrate)
        {
            illustrateBtnTapped(fromSubMenu);
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
        bool changeDayNight;
        if (DateTime.Now.Hour >= forwardHour + SDConstants.HourToChangeDayNight
            || DateTime.Now.Hour < forwardHour)
        {
            changeDayNight = true;
        }
        else if (DateTime.Now.Day != SDDataManager.Instance.OpenTime.Day)
            changeDayNight = true;
        else return;
        if (changeDayNight && SDDataManager.Instance.ResidentMovementData != null)
        {
            int a = SDDataManager.Instance.ResidentMovementData.CurrentDayNightId;
            a++; a %= 2;
            SDDataManager.Instance.ResidentMovementData.CurrentDayNightId = a;
        }
        RefreshDataInOuterMenu();
    }
    public void WhenChangingSubMenu
        (HomeSceneSubMenu oldOne, HomeSceneSubMenu newOne)
    {
        if(oldOne != HomeSceneSubMenu.End && newOne == HomeSceneSubMenu.End)
        {
            //返回城镇大街页面
            changeToHomeScene();
        }
        else if(newOne != HomeSceneSubMenu.End)
        {
            //进入功能建筑页面       
            BasicSubMenuPanel P = AllSubMenus[(int)newOne].GetComponent<BasicSubMenuPanel>();
            changeToSubMenuScene(P.AboveOuterMenuIsShowing);
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

        //
        RefreshDataInOuterMenu();
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
        AbovePlayerMessagePanel.gameObject.SetActive(true);
        ExtraBtnList.gameObject.SetActive(true);
    }
    public void changeToSubMenuScene(bool showAbove = false)
    {
        InSubMenuPanel.gameObject.SetActive(true);
        InHomeScenePanel.gameObject.SetActive(false);
        AbovePlayerMessagePanel.gameObject.SetActive(showAbove);
        ExtraBtnList.gameObject.SetActive(false);
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
            int lv = P.Level;
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

    public void BtnToShowPropsInDeposity()
    {
        depositoryBtnTapped();
        StartCoroutine(IEToPropBag());
    }
    IEnumerator IEToPropBag()
    {
        yield return new WaitForSeconds(0.1f);
        _DepositoryPanel.GetComponent<DepositoryPanel>().btnToProp();
    }


    public void RefreshDataInOuterMenu()
    {
        OuterMenu.ReadAllDataFromGDE();
    }
    #endregion


    public void AddHeroPools()
    {
        List<HeroAltarPool> All = SDDataManager.Instance.GetAllHeroAltarPoolList;
        foreach (HeroAltarPool p in All)
        {
            GDEHeroAltarPoolData d = new GDEHeroAltarPoolData(GDEItemKeys.HeroAltarPool_emptyPool)
            {
                ID = p.ID,
                Unable = false,
                NotNormalPool = false,
                AltarTimes = 0,
                GetSNum = 0,
                Name = p.Name,
                AllHeroes = p.HeroList.Select(x => x.ID).ToList(),
                starttime = DateTime.Now.ToString(),
                lasttime = 7,
                PoolCapacity=50,
            };
            SDDataManager.Instance.PlayerData.AltarPoolList.Add(d);
            SDDataManager.Instance.PlayerData.Set_AltarPoolList();
        }
    }


}
