using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using GameDataEditor;
using System.Linq;

public class BattleRoleData : MonoBehaviour
{
    [Header("该角色内容")]
    //public OneRoleClassData ThisBasicRoleProperty()._role;
    public SDConstants.CharacterType _Tag;
    //
    public ActionPanelController APC;
    //
    #region RoleProperty列表，对应charactertype选取对应
    [HideInInspector]
    public HeroController HeroProperty;
    [HideInInspector]
    public EnemyController EnemyProperty;
    public BasicRoleProperty ThisBasicRoleProperty()
    {
        if (_Tag == SDConstants.CharacterType.Hero) return HeroProperty;
        if (_Tag == SDConstants.CharacterType.Enemy) return EnemyProperty;
        return HeroProperty;
    }
    #endregion
    #region 角色BarChart控制
    public HP_Controller HpController;
    public MP_Controller MpController;
    public TP_Controller TpController;
    #endregion
    public SDGoldController GoldC;
    #region 强伤害影响
    public int ElemModify
    {
        get { return SkillKindModify[(int)SkillKind.Elemental]; }
        set { SkillKindModify[(int)SkillKind.Elemental] = value; }
    }
    public int PhysModify
    {
        get { return SkillKindModify[(int)SkillKind.Physical]; }
        set { SkillKindModify[(int)SkillKind.Physical] = value; }
    }
    public int ArcaModify
    {
        get { return SkillKindModify[(int)SkillKind.Arcane]; }
        set { SkillKindModify[(int)SkillKind.Arcane] = value; }
    }
    [HideInInspector]
    public int[] SkillKindModify = new int[(int)SkillKind.End];
    #endregion
    //
    public bool IsEnemy;
    public bool IsDead;
    public bool IsActed;
    public bool IsOptionTarget;
    public bool IsBoss;
    public bool IsFacingRight = true;

    public int rareWeight = 0;
    public bool isLittleBoss = false;

    public string UnitId;
    public int unitHashcode;
    public Transform unit_model;
    public CharacterModelController unit_character_model;
    //
    [HideInInspector]
    public BattleManager BM;

    #region 动画设置
    //[Header("动画设置")]
    private float moveDistance = 0.5f;
    private float moveTime = 0.15f;
    private float DAMAGE_ANIM_TIME = 0.7f;
    private float REWARD_ANIM_TIME = 1f;
    private Vector2 bulletStartPosOffset = new Vector2(0.2f, 0.3f);
    private float bulletTime = 0.2f;
    public Vector2 modelOriginLocalPos;
    #endregion
    #region 当前行动指示器
    public Transform actionSign;
    public Transform optionSign;
    #endregion
    #region 视觉设置
    public Image[] StarImgs;
    public int posIndex = 0;
    public bool showEnemyBlood = false;
    public bool openAllBonus = false;
    public bool alwaysGoodEvents = false;
    #endregion
    private void Awake()
    {
        APC = GetComponentInChildren<ActionPanelController>();
        BM = FindObjectOfType<BattleManager>();
        GoldC = GetComponentInChildren<SDGoldController>();
        if(APC) APC.transform.localScale = Vector3.one * 1.2f;
        if (GetComponent<HeroController>()) HeroProperty = GetComponent<HeroController>();
        if (GetComponent<EnemyController>()) EnemyProperty = GetComponent<EnemyController>();
    }
    private void Start()
    {
        //statetag 代表的状态列表
        AllSTStates = new BRD_OneStateController[(int)StateTag.End];
        for (int i = 0; i < AllSTStates.Length; i++)
        {
            AllSTStates[i] = new BRD_OneStateController();
            AllSTStates[i].stateTag = (StateTag)i;
            AllSTStates[i].Clear(true);
        }

    }
    #region 角色构建
    public void initHeroController(int heroHashCode)
    {
        _skills = new List<OneSkill>();
        IsFacingRight = true;
        IsEnemy = false;
        initHero(heroHashCode);
        //装备数据读取
        initHelmet(heroHashCode);
        initBreastplate(heroHashCode);
        initGardebras(heroHashCode);
        initLegging(heroHashCode);
        initJewelry(heroHashCode);
        initJewelry(heroHashCode, true);
        initWeapon(heroHashCode);
        //
        HeroProperty.InitHeroBasicProperties();
        APC.initActionPanel(_skills, HeroProperty.ID);
        modelOriginLocalPos = unit_model.localPosition;
    }
    public void showHeroStars()
    {
        int sn = HeroProperty.LEVEL;
        for (int i = 0; i < StarImgs.Length; i++)
        {
            if (i < sn)
            {
                StarImgs[i].gameObject.SetActive(true);
            }
            else
            {
                StarImgs[i].gameObject.SetActive(false);
            }
        }
    }
    public void initEnemyController(string heroId)
    {
        _skills = new List<OneSkill>();
        IsFacingRight = false;
        IsEnemy = true;
        initEnemy(heroId);
        EnemyProperty.initEnemyBasicProperties();


        //
        OneSkill s = new OneSkill()
        {
            SkillName="挠"
            ,SkillFunctionID = 0
            ,Desc = "怪物有爪"
        };
        _skills.Add(s);
        OneSkill s1 = new OneSkill()
        {
            SkillName = "双重攻击"
            ,
            SkillFunctionID = 11
            ,
            Desc = "随机攻击两个敌人"
        };
        _skills.Add(s1);

        //
        APC.initActionPanel(_skills, heroId);
        modelOriginLocalPos = unit_model.localPosition;
    }
    public void initHero(int heroHashCode)
    {
        unitHashcode = heroHashCode;
        SDHero h;
        if (GetComponent<SDHero>()) h = GetComponent<SDHero>();
        else h = gameObject.AddComponent<SDHero>();
        HeroProperty._hero = h;
        GDEHeroData heroData
                = SDDataManager.Instance.GetHeroOwnedByHashcode(unitHashcode);
        //
        if (heroData != null)
        {
            HeroProperty._hero.nameBeforeId = heroData.nameBeforeId;

            //
            UnitId = SDDataManager.Instance.getHeroIdByHashcode(unitHashcode);
            HeroProperty.ID = HeroProperty._hero.ID = UnitId;
            ROHeroData dal = SDDataManager.Instance
                .getHeroDataByID(UnitId, heroData.starNumUpgradeTimes);
            //
            unit_character_model.initHeroCharacterModel(heroHashCode, SDConstants.HERO_MODEL_RATIO);
            //
            int grade = SDDataManager.Instance.getLevelByExp(heroData.exp);
            HeroProperty._hero.grade = grade;

            RoleAttributeList _ral = dal.ExportRAL;
            _ral.Add(RoleAttributeList.GDEToRAL(heroData.RoleAttritubeList));
            HeroProperty._hero.initData_Hero
                (dal.Info.Career.Career, dal.Info.Race.Race
                , grade, 0, dal.starNum
                , _ral
                , dal.CRIDmg, dal.DmgReduction, dal.DmgReflection, dal.RewardRate
                , dal.BarChartRegendPerTurn, UnitId, dal.Info.Name, heroData.wakeNum); ;
            HeroProperty._hero.gender = dal.Info.Sex;
            addSkillByCareerByRaceByStarnum(heroHashCode, dal.starNum);
        }
    }

    #region 添加角色配置
    public void initHelmet(int heroHashCode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipHelmet(heroHashCode);
        Helmet a = unit_model.gameObject.AddComponent<Helmet>();
        HeroProperty._helmet = a;
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            HeroProperty._helmet.initDataEmpty();return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            HeroProperty._helmet.initDataEmpty(); return;
        }
        //
        HeroProperty._helmet.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        HeroProperty._helmet.PassiveEffectInit(Item.PassiveEffect);
        HeroProperty._helmet.armorRank = Item.ArmorRank;
        int lv = armor.lv;
        HeroProperty._helmet.initGradeShow(lv);
    }
    public void initBreastplate(int heroHashCode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipBreastplate(heroHashCode);
        Breastplate a = unit_model.gameObject.AddComponent<Breastplate>();
        HeroProperty._breastplate = a;
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            HeroProperty._breastplate.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            HeroProperty._breastplate.initDataEmpty(); return;
        }
        //
        HeroProperty._breastplate.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        HeroProperty._breastplate.PassiveEffectInit(Item.PassiveEffect);
        HeroProperty._breastplate.armorRank = Item.ArmorRank;
        int lv = armor.lv;
        HeroProperty._breastplate.initGradeShow(lv);
    }
    public void initGardebras(int heroHashCode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipGardebras(heroHashCode);
        Gardebras a = unit_model.gameObject.AddComponent<Gardebras>();
        HeroProperty._gardebras = a;
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            HeroProperty._gardebras.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            HeroProperty._gardebras.initDataEmpty(); return;
        }
        //
        HeroProperty._gardebras.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        HeroProperty._gardebras.PassiveEffectInit(Item.PassiveEffect);
        HeroProperty._gardebras.armorRank = Item.ArmorRank;
        int lv = armor.lv;
        HeroProperty._gardebras.initGradeShow(lv);
    }
    public void initLegging(int heroHashCode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipLegging(heroHashCode);
        Legging a = unit_model.gameObject.AddComponent<Legging>();
        HeroProperty._legging = a;
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            HeroProperty._legging.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            HeroProperty._legging.initDataEmpty(); return;
        }
        //
        HeroProperty._legging.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        HeroProperty._legging.PassiveEffectInit(Item.PassiveEffect);
        HeroProperty._legging.armorRank = Item.ArmorRank;
        int lv = armor.lv;
        HeroProperty._legging.initGradeShow(lv);
    }
    public void initJewelry(int heroHashCode,bool isSecondPos = false)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipJewelry(heroHashCode);
        Jewelry a = unit_model.gameObject.AddComponent<Jewelry>();
        if (!isSecondPos)
        {
            HeroProperty._jewelry0 = a;
            if (armor == null || string.IsNullOrEmpty(armor.id))
            {
                HeroProperty._jewelry0.initDataEmpty(); return;
            }
            EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
            if (Item == null)
            {
                HeroProperty._jewelry0.initDataEmpty(); return;
            }
            //
            HeroProperty._jewelry0.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
                , Item.ID, Item.NAME, 0);
            HeroProperty._jewelry0.PassiveEffectInit(Item.PassiveEffect);
            HeroProperty._jewelry0.armorRank = Item.ArmorRank;
            int lv = armor.lv;
            HeroProperty._jewelry0.initGradeShow(lv);
        }
        else
        {
            HeroProperty._jewelry1 = a;
            if (armor == null || string.IsNullOrEmpty(armor.id))
            {
                HeroProperty._jewelry1.initDataEmpty(); return;
            }
            EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
            if (Item == null)
            {
                HeroProperty._jewelry1.initDataEmpty(); return;
            }
            //
            HeroProperty._jewelry1.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
                , Item.ID, Item.NAME, 0);
            HeroProperty._jewelry1.PassiveEffectInit(Item.PassiveEffect);
            HeroProperty._jewelry1.armorRank = Item.ArmorRank;
            int lv = armor.lv;
            HeroProperty._jewelry1.initGradeShow(lv);
        }
    }
    public void initWeapon(int heroHashCode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroWeapon(heroHashCode);
        SDWeapon a = unit_model.gameObject.AddComponent<SDWeapon>();
        HeroProperty._weapon = a;
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            HeroProperty._weapon.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            HeroProperty._weapon.initDataEmpty(); return;
        }
        //
        HeroProperty._weapon.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        HeroProperty._weapon.PassiveEffectInit(Item.PassiveEffect);
        HeroProperty._weapon.armorRank = Item.ArmorRank;
        int lv = armor.lv;
        HeroProperty._weapon.initGradeShow(lv);
    }

    #endregion
    public void addSkillByCareerByRaceByStarnum(int heroHashcode,int quality)
    {
        _skills.Add(OneSkill.normalAttack);
        //
        GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(heroHashcode);
        string skill0Id = hero.skill0Id;
        _skills.Add(SDDataManager.Instance.getOwnedSkillById(skill0Id, heroHashcode));
        if (SDDataManager.Instance.checkHeroEnableSkill1ByHashcode(heroHashcode))
        {
            string skill1Id = hero.skill1Id;
            _skills.Add(SDDataManager.Instance.getOwnedSkillById(skill1Id, heroHashcode));
        }//稀有角色才能拥有额外技能
        string skillOmegaId = hero.skillOmegaId;
        _skills.Add(SDDataManager.Instance.getOwnedSkillById(skillOmegaId, heroHashcode));
        return;
    }
    public void initEnemy(string enemyId)
    {
        UnitId = enemyId;
        SDEnemy e;
        if (GetComponent<SDEnemy>()) e = GetComponent<SDEnemy>();
        else e = gameObject.AddComponent<SDEnemy>();
        EnemyProperty._enemy = e;
        //
        EnemyProperty.ID = UnitId;
        EnemyProperty._enemy.ID = UnitId;
        //
        EnemyInfo enemy = SDDataManager.Instance.getEnemyInfoById(enemyId);
        ROEnemyData d = SDDataManager.Instance.getEnemyDataById(enemyId);
        int career = 0;
        if (rareWeight > 0) career = UnityEngine.Random.Range(0, (int)Job.End);
        d = enemyBuild.AddCareerData(d, career);
        //
        RoleAttributeList ral = enemyBuild.RALAddedByLevel(enemy.RAL).Clone;
        //
        float scale = SDConstants.HERO_MODEL_RATIO;
        if (isLittleBoss)
        {
            scale = SDConstants.HERO_MODEL_BIG_RATIO;
            d = enemyBuild.LittleBossData(d, enemy.EnemyRank.Index);
        }
        //
        unit_character_model.initEnemyCharacterModel(UnitId, scale);
        //
        ral.AffectedByRate(d.RALRate);
        //
        EnemyProperty._enemy.initData(enemy.EnemyRank.Index, ral
            , d.CRIDmg, d.DmgReduction
            , d.DmgReflection, d.dropCoins, d.BarChartRegendPerTurn
            , enemy.ID, enemy.Name, 0);
        enemyBuild EB = FindObjectOfType<enemyBuild>();
        if (EB)
        {
            List<OneSkill> skills = EB.WriteEnemySkills(enemy);
            if(skills!=null)
            for (int i = 0; i < skills.Count; i++) _skills.Add(skills[i]);
        }
    }
    #endregion
    #region 状态类
    [ReadOnly]
    public int stateExtraDamage = 0;
    public RoleBarChart AllRegend = RoleBarChart.zero;
    #region 全状态列表
    [Header("状态类内容")]
    public Text actionUnitStateText;
    
    #region State In StateTag Or RegendState
    public string SpecialStateSign(StateTag tag)
    {
        switch (tag)
        {
            case StateTag.Bleed: return "①";
            case StateTag.Mind: return "②";
            case StateTag.Fire: return "③";
            case StateTag.Frost: return "④";
            case StateTag.Corrosion: return "⑤";
            case StateTag.Hush: return "⑥";
            case StateTag.Dizzy: return "⑦";
            case StateTag.Confuse: return "⑧";
            default: return null;
        }
    }
    public string SpecialRegendSign(SDConstants.BCType tag)
    {
        switch (tag)
        {
            case SDConstants.BCType.hp:return "⑨";
            case SDConstants.BCType.mp:return "⑩";
            case SDConstants.BCType.tp:return "⑪";
            default:return null;
        }
    }
    #region 状态可视化内容
    [Header("状态显示设置")]
    public BattleStateVisionController BSVC;
    public Transform Unit_Bleed { get { return BSVC.Unit_ST_State_List[(int)StateTag.Bleed]; } }
    public Transform Unit_Mind { get { return BSVC.Unit_ST_State_List[(int)StateTag.Mind]; } }
    public Transform Unit_Fire { get { return BSVC.Unit_ST_State_List[(int)StateTag.Fire]; } }
    public Transform Unit_Frost { get { return BSVC.Unit_ST_State_List[(int)StateTag.Frost]; } }
    public Transform Unit_Corrosion { get { return BSVC.Unit_ST_State_List[(int)StateTag.Corrosion]; } }
    public Transform Unit_Hush { get { return BSVC.Unit_ST_State_List[(int)StateTag.Hush]; } }
    public Transform Unit_Dizzy { get { return BSVC.Unit_ST_State_List[(int)StateTag.Dizzy]; } }
    public Transform Unit_Confuse { get { return BSVC.Unit_ST_State_List[(int)StateTag.Confuse]; } }

    public Transform Unit_Buff { get { return BSVC.Unit_Standard_State_List[0]; } }
    public Transform Unit_Debuff { get { return BSVC.Unit_Standard_State_List[1]; } }
    public Transform PerStateUnit(StateTag tag)
    {
        switch (tag)
        {
            case StateTag.Bleed: return Unit_Bleed;
            case StateTag.Mind: return Unit_Mind;
            case StateTag.Fire: return Unit_Fire;
            case StateTag.Frost: return Unit_Frost;
            case StateTag.Corrosion: return Unit_Corrosion;
            case StateTag.Hush: return Unit_Hush;
            case StateTag.Dizzy: return Unit_Dizzy;
            case StateTag.Confuse: return Unit_Confuse;
            default: return null;
        }
    }
    //
    public Transform Unit_Die { get { return BSVC.Unit_Die_State; } }
    #endregion
    #region　撕裂状态①
    public BRD_OneStateController stateBleed
    { get { return AllSTStates[(int)StateTag.Bleed]; }
        set { AllSTStates[(int)StateTag.Bleed] = value; }
    }
    #endregion
    #region 压力状态②
    public BRD_OneStateController stateMind
    {
        get { return AllSTStates[(int)StateTag.Mind]; }
        set { AllSTStates[(int)StateTag.Mind] = value; }
    }
    #endregion
    #region 灼烧状态③
    public BRD_OneStateController stateFire
    {
        get { return AllSTStates[(int)StateTag.Fire]; }
        set { AllSTStates[(int)StateTag.Fire] = value; }
    }
    #endregion
    #region 霜冻状态④
    public BRD_OneStateController stateFrost
    {
        get { return AllSTStates[(int)StateTag.Frost]; }
        set { AllSTStates[(int)StateTag.Frost] = value; }
    }
    #endregion
    #region 腐蚀状态⑤
    public BRD_OneStateController stateCorrosion
    {
        get { return AllSTStates[(int)StateTag.Corrosion]; }
        set { AllSTStates[(int)StateTag.Corrosion] = value; }
    }
    #endregion
    #region 禁言状态⑥---无法释放技能
    public BRD_OneStateController stateHush
    {
        get { return AllSTStates[(int)StateTag.Hush]; }
        set { AllSTStates[(int)StateTag.Hush] = value; }
    }
    #endregion
    #region 眩晕状态⑦
    public BRD_OneStateController stateDizzy
    {
        get { return AllSTStates[(int)StateTag.Dizzy]; }
        set { AllSTStates[(int)StateTag.Dizzy] = value; }
    }
    #endregion
    #region 混乱状态⑧
    public BRD_OneStateController stateConfuse
    {
        get { return AllSTStates[(int)StateTag.Confuse]; }
        set { AllSTStates[(int)StateTag.Confuse] = value; }
    }
    #endregion

    /// <summary>
    /// 按照StateTag固定存在的State
    /// </summary>
    [HideInInspector]
    public BRD_OneStateController[] AllSTStates;
    public BRD_OneStateController OneState(StateTag tag)
    {
        return AllSTStates[(int)tag];
    }
    #endregion

    #region State Can Add Or Remove
    public List<OneStateController> CurrentStateList;
    public bool AddStandardState(OneStateController state)
    {
        if (CurrentStateList.Exists(x => x.ID == state.ID))
        {
            return false;
        }
        if (CurrentStateList.Count < SDConstants.UnitStateVolume)
        {
            CurrentStateList.Add(state);return true;
        }
        else
        {
            for(int i = 0; i < CurrentStateList.Count; i++)
            {
                if (CurrentStateList[i].CanSqueeze)
                {
                    CurrentStateList.RemoveAt(i);
                    CurrentStateList.Add(state);
                    return true;
                }
            }
            return false;
        }
    }
    public void checkAllStandardStates()
    {
        RoleAttributeList RAL = new RoleAttributeList();
        for (int i = 0; i < CurrentStateList.Count; i++)
        {
            if (CurrentStateList[i].LastTime <= 0)
            {
                CurrentStateList.RemoveAt(i);
            }
            else
            {
                if(CurrentStateList[i].StateEndType == StandardState.StateEndType.time)
                {
                    CurrentStateList[i].LastTime--;
                }
                //
                RAL.Add(CurrentStateList[i].RAL);
                stateExtraDamage += CurrentStateList[i].ExtraDmg;
                AllRegend += CurrentStateList[i].BarChart;
                CurrentStateList[i].ExtraFunction(this);
            }
        }
        Debug.Log("StandardState_IfHaveData: " + RAL.HaveData);
        ThisBasicRoleProperty()._role.AllARevision.Add(RAL);
        Debug.Log("AllARevision_IfHaveData: " 
            + ThisBasicRoleProperty()._role.AllARevision.HaveData);
        //
        if (RAL.AllAttributeData.ToList().Exists(x=>x>0)
            || RAL.AllResistData.ToList().Exists(x => x > 0))
        {
            Unit_Buff.gameObject.SetActive(true);
        }
        else
        {
            Unit_Buff.gameObject.SetActive(false);
        }
        if (RAL.AllAttributeData.ToList().Exists(x => x < 0)
    || RAL.AllResistData.ToList().Exists(x => x < 0))
        {
            Unit_Debuff.gameObject.SetActive(true);
        }
        else
        {
            Unit_Debuff.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 使用技能后失效的状态
    /// </summary>
    public void CheckStatesWithTag_skill()
    {
        for(int i = 0; i < CurrentStateList.Count; i++)
        {
            if(CurrentStateList[i].StateEndType == StandardState.StateEndType.skill)
            {
                CurrentStateList[i].LastTime = 0;
            }
        }
    }
    /// <summary>
    /// 被攻击后失效的状态
    /// </summary>
    public void CheckStatesWithTag_beAtked()
    {
        for (int i = 0; i < CurrentStateList.Count; i++)
        {
            if (CurrentStateList[i].StateEndType == StandardState.StateEndType.beAtked)
            {
                CurrentStateList[i].LastTime = 0;
            }
        }
    }
    #endregion
    #endregion
    #region 应对单个状态脚本
    public bool checkPerState(StateTag tag)
    {
        return CurrentStateList.FindAll(x => x.StateTag == tag).Count > 0;
    }
    public void clearPerTagStates(StateTag tag)
    {
        for(int i = 0; i < CurrentStateList.Count; i++)
        {
            if(CurrentStateList[i].StateTag == tag)
            {
                CurrentStateList.RemoveAt(i);
            }
        }
    }
    public void clearAllStates()
    {
        CurrentStateList.Clear();
    }
    #endregion
    #region 快速状态索引器
    public bool ReadThisStateEnable(StateTag tag)
    {
        return checkPerState(tag);
    }
    public int ReadThisStateLastTime(StateTag tag)
    {
        List<OneStateController> fixes = CurrentStateList.FindAll(x => x.StateTag == tag);
        IEnumerable<int> timeList = fixes.Select(x => x.LastTime);
        return timeList.Max();
    }
    #region 构建额外状态
    public void initInterveneState(int changeData,int lastTime,bool isBuff = true)
    {
        RoleExtraState.initInterveneState(changeData, lastTime, isBuff);
    }
    public void initReflectState(int changeData, int lastTime,bool isBuff = true)
    {
        RoleExtraState.initReflectState(changeData, lastTime, isBuff);
    }
    #endregion

    public void getBCOA(RoleBarChart BC)
    {
        if (BC.HP > 0)
        {
            HpController.addHp(BC.HP);
        }
        else if (BC.HP < 0)
        {
            HpController.getExtraDamage(BC.HP);
        }

        if (BC.MP > 0)
        {
            MpController.addMp(BC.MP);
        }
        else if(BC.MP < 0)
        {
            MpController.consumeMp(BC.MP);
        }

        if (BC.TP > 0)
        {
            TpController.addTp(BC.TP);
        }
        else if(BC.TP < 0)
        {
            TpController.consumeTp(BC.TP);
        }
    }
    #endregion
    #region 状态施加效果
    public RoleExtraStateController RoleExtraState 
    {
        get { return GetComponent<RoleExtraStateController>(); }
    }
    public void CheckStates()
    {
        ThisBasicRoleProperty()._role.AllARevision.Clear();

        stateExtraDamage = 0;
        AllRegend = ThisBasicRoleProperty().BarChartRegendPerTurn;
        checkAllStandardStates();
        //八大直接显示类状态
        for (int i = 0; i < (int)StateTag.End; i++)
        {
            if (checkPerState((StateTag)i))
            {
                ThisBasicRoleProperty()._role.AllARevision
                    += OneState((StateTag)i).UniqueEffectInRA;
                stateExtraDamage += OneState((StateTag)i).ExtraDmg;
                PerStateUnit((StateTag)i).gameObject.SetActive(true);
            }
            else
            {
                PerStateUnit((StateTag)i).gameObject.SetActive(false);
            }
        }
        HpController.getExtraDamage(stateExtraDamage);

        #region otherBuff
        if (_Tag == SDConstants.CharacterType.Hero)
        {   
            Race _race = HeroProperty._hero._heroRace;  
            RoleAttributeList basic = ThisBasicRoleProperty()._role.ThisRoleAttributes.Clone;
            ThisBasicRoleProperty()._role.AllARevision
                .Add( SDDataManager.Instance.BuffFromDaynight(basic));
            ThisBasicRoleProperty()._role.AllARevision
                .Add( SDDataManager.Instance.BuffFromRace(basic, _race));
        }
        #endregion

        #region GoddessState
        if (BM.GM.haveGoddess)
        {
            ThisBasicRoleProperty()._role.AllARevision.Add(BM.GM.GetRALUpByGoddess
                (ThisBasicRoleProperty()._role.ThisRoleAttributes.Clone));
        }
        #endregion
        #region extraState
        RoleExtraState.checkExtraStates();
        #endregion
        #region fatigueStateCheck
        if (SDDataManager.Instance.checkHeroFatigueTooHigh(unitHashcode))
        {
            Debug.Log("英雄 " + name + " 过度劳累");
            HpController.getExtraDamage(HpController.CurrentHp);
        }
        #endregion
        
        writeData();
    }
    public void writeData()
    {
        string d = "";
        RoleAttributeList l = ThisBasicRoleProperty()._role.AllARevision.Clone;
        for (int i = 0; i < l.AllAttributeData.Length; i++)
        {
            if (l.AllAttributeData[i] != 0)
            {
                d += (AttributeData)i + " " + l.AllAttributeData[i] + "|| ";
            }
        }
        string d2 = "";
        int dmg = checkStatesDamaging();
        if (dmg != 0)
        {
            d2 += " 状态持续造成伤害:" + dmg;
        }
        string all = d + d2;
        Debug.Log(all);
        //
        ThisBasicRoleProperty()._role.RefreshCERAL();
    }




    #region 状态造成伤害汇总
    public int checkStatesDamaging()
    {
        int d = 0;
        for (int i = 0; i < (int)StateTag.End; i++)
        {
            if (AllSTStates[i].stateCondition != 0)
            {
                d += AllSTStates[i].ExtraDmg;
            }
        }
        return d;
    }
    #endregion

    #endregion
    #endregion
    #region 技能类
    public List<OneSkill> _skills;
    #region 完整技能检测机制
    #region 判断技能是否触发暴击
    [HideInInspector]
    public bool CritHappen;//暴击-判定成功
    public void CheckThisSkillCauseCrit(SkillFunction _skill)
    {
        int baseC = ThisBasicRoleProperty().ReadRA(AttributeData.Crit);
        int realC = (int)(baseC * AllRandomSetClass.SimplePercentToDecimal(_skill.CritR + 100));
        float Rate = AdolescentSet.CritFunction(realC);
        bool _IsCrit = UnityEngine.Random.Range(0,1f) < Rate;
        CritHappen = _IsCrit;
    }
    #endregion    
    #region 判断技能是否命中
    [HideInInspector]
    public bool AccurHappen;//精准·闪避-判定成功
    public void CheckThisSkillCauseAccur(SkillFunction _skill,BattleRoleData _targetUnit)
    {
        if(_targetUnit.IsEnemy == IsEnemy)
        {
            AccurHappen = true;
        }
        else
        {
            BasicRoleProperty _target = _targetUnit.ThisBasicRoleProperty();
            int baseA = ThisBasicRoleProperty().ReadRA(AttributeData.Accur);
            int realA = (int)(baseA * AllRandomSetClass.SimplePercentToDecimal(_skill.AccuracyR + 100));
            int Evo = _target.ReadRA(AttributeData.Evo);
            float Rate = AdolescentSet.AccurFunction(realA, Evo);
            float t = UnityEngine.Random.Range(0, 1f);
            bool _IsAccur = t < Rate;
            AccurHappen = _IsAccur;
            Debug.Log("精准度计算：" + _IsAccur + " || " + t + " " + Rate
                + "--evo:" + Evo + "--accur:" + realA + "--baseAccur:"+ baseA
                + "--skillAccuracyR:" + _skill.AccuracyR) ;
        }
    }
    #endregion
    #region 判断是否失误
    [HideInInspector]
    public bool FaultHappen;//失误-判定成功
    public void CheckThisSkillCauseFault()
    {
        int faultrate = 5;
        FaultHappen = AllRandomSetClass.PercentIdentify(faultrate);
    }
    #endregion
    #region 数值浮动设计
    [HideInInspector]
    public int ExpectResult;//期望最终浮动
    public void CheckThisSkillCauseExpect(SkillFunction _skill)
    {
        int _expect = ThisBasicRoleProperty().ReadRA(AttributeData.Expect);
        int _expectR = _skill.ExpectR;
        int Ex = (int)(_expect * AllRandomSetClass.SimplePercentToDecimal(_expectR + 100));
        ExpectResult = Ex;
    }
    #endregion
    public void SkillCheck(SkillFunction _skill, BattleRoleData _targetUnit)
    {
        CheckThisSkillCauseAccur(_skill, _targetUnit);
        CheckThisSkillCauseFault();
        CheckThisSkillCauseCrit(_skill);
        CheckThisSkillCauseExpect(_skill);
    }
    #endregion
    #endregion

    #region 判断是否可成为当前技能目标:影响显示动画
    public void SetOptionSignState()
    {
        if (optionSign != null && optionSign.gameObject != null)
            optionSign.gameObject.SetActive(IsOptionTarget);
    }
    #endregion
    #region 技能释放动画脚本
    public void playMoveTowardAnimation(Vector2 targetPos)
    {
        StartCoroutine(IEPlayMoveTowardAnimation(targetPos));
    }
    public IEnumerator IEPlayMoveTowardAnimation(Vector2 targetPos)
    {
        //unit_model.GetComponent<CanvasGroup>().DOFade(0, moveTime * 0.5f);
        //yield return new WaitForSeconds(moveTime * 0.5f);
        Vector2 pos;
        if (IsFacingRight)
        {
            pos = targetPos - new Vector2(moveDistance, 0);
        }
        else
        {
            pos = targetPos + new Vector2(moveDistance, 0);
        }
        unit_model.DOMove(pos, moveTime);
        //unit_model.GetComponent<CanvasGroup>().DOFade(1, moveTime * 0.5f);
        yield return new WaitForSeconds(moveTime);
    }
    public void playMoveBackAnimation()
    {
        StartCoroutine(IEPlayMoveBackAnimation());
    }
    public IEnumerator IEPlayMoveBackAnimation()
    {
        //unit_model.GetComponent<CanvasGroup>().DOFade(0, moveTime * 0.5f);
        //yield return new WaitForSeconds(moveTime * 0.5f);
        //unit_model.localPosition = modelOriginLocalPos;
        unit_model.DOLocalMove(modelOriginLocalPos, moveTime);
        //unit_model.GetComponent<CanvasGroup>().DOFade(1, moveTime * 0.5f);
        yield return new WaitForSeconds(moveTime);
    }

    public void playBulletCastAnimation(Transform bullet,Vector2 startPos,Vector2 targetPos)
    {
        StartCoroutine(IEPlayBulletCastAnimation(bullet, startPos, targetPos));
    }
    public IEnumerator IEPlayBulletCastAnimation(Transform bullet,Vector2 startPos,Vector2 targetPos)
    {
        bullet.GetComponent<Canvas>().sortingLayerName = "Effects";
        bullet.position = startPos + bulletStartPosOffset;
        bullet.localEulerAngles = Vector3.forward * getRorarionBetweenAAndB(startPos, targetPos);
        bullet.GetComponent<CanvasGroup>().DOFade(1, bulletTime * 0.5f);
        bullet.localScale = Vector3.one * 0.5f;
        yield return new WaitForSeconds(bulletTime * 0.5f);

        bullet.DOMove(targetPos + Vector2.down * bulletStartPosOffset.y, bulletTime);
        bullet.DOScale(Vector3.one, bulletTime);
        yield return new WaitForSeconds(bulletTime);

        bullet.GetComponent<CanvasGroup>().DOFade(0, bulletTime * 0.5f);
        yield return new WaitForSeconds(bulletTime * 0.5f);
    }
    public float getRorarionBetweenAAndB(Vector2 a, Vector2 b)
    {
        float xD = b.x - a.x;float yD = b.y - a.y;
        float r = Mathf.Rad2Deg * Mathf.Atan2(yD , xD);
        return r;
    }
    #endregion
    #region 其他动画脚本
    public void playHurtAnimation()
    {
        if (unit_character_model.CurrentCharacterModel == null) return;
        unit_character_model
            .CurrentCharacterModel.ChangeModelAnim
            (unit_character_model.CurrentCharacterModel.anim_hurt,false);
    }
    #endregion
    #region 角色死亡、消失与复活设置
    public void playFadeAnimation()
    {
        Debug.Log(this.name + "消失");
        StartCoroutine(IEFade());
    }
    public void playDieAnimation()
    {
        Debug.Log(this.name + "死亡");
        Unit_Die.gameObject.SetActive(true);
        StartCoroutine(IEDie());
    }
    //单位消失逻辑处理
    public IEnumerator IEFade()
    {
        bool showNextUnitFlag = false;
        if (_Tag == SDConstants.CharacterType.Hero)
        {
            BM.Remaining_SRL.Remove(this);
            clearAllStates();
        }
        else
        {
            BM.Remaining_ORL.Remove(this);
            gameObject.tag = "Untagged";
            showNextUnitFlag = BM.CheckBattleSuccess();
        }
        yield return new WaitForSeconds(DAMAGE_ANIM_TIME);
        #region 消失动画
        if (_Tag == SDConstants.CharacterType.Hero)
        {
            unit_character_model
                .CurrentCharacterModel.ChangeModelAnim
                (unit_character_model.CurrentCharacterModel.anim_fade);

        }
        else
        {
            unit_character_model.CurrentCharacterModel.isEnemy = true;
            if (IsBoss)
            {
                unit_character_model.CurrentCharacterModel.isBoss = true;
                //BM.
            }
            unit_character_model
                .CurrentCharacterModel.ChangeModelAnim
                (unit_character_model.CurrentCharacterModel.anim_fade);
        }
        #endregion
        #region  结算
        TriggerManager.Instance.WhenUnitDie(this);
        #endregion
        #region 成就记录
        if (_Tag == SDConstants.CharacterType.Hero)
        {
            SDDataManager.Instance.addAchievementDataByType("heroDie");
        }
        else if (_Tag == SDConstants.CharacterType.Enemy)
        {
            string id = ThisBasicRoleProperty().ID;
            ROEnemyData data = SDDataManager.Instance.getEnemyDataById(id);
            string t = "kill_" + data.Info.EnemyRank.EnemyType.ToString();
            SDDataManager.Instance.AddKillingDataToAchievement(id);
            SDDataManager.Instance.addAchievementDataByType(t);
        }
        #endregion
        #region Obj消失
        if (_Tag == SDConstants.CharacterType.Hero)
        {

        }
        else
        {
            gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
        }
        #endregion
        #region 判断游戏是否结束
        if (_Tag == SDConstants.CharacterType.Hero)
        {
            showNextUnitFlag = BM.CheckBattleLose();
            if (!showNextUnitFlag)
            {
                Debug.Log("hero die not lose");
            }
            else
            {
                Debug.Log("hero die then lose");
                BM.ABM.IsWaitingAction = true;
            }
        }
        else
        {
            if (IsBoss)
            {
                EnemyProperty.showDropItems();
            }
            else
            {
                EnemyProperty.showDropItem();
            }

            if (!showNextUnitFlag)
            {
                Debug.Log("enemy die not success");
            }
            else
            {
                Debug.Log("enemy die then success");
                BM.BattleSuccess();
            }
        }
        #endregion
        yield return new WaitForSeconds(0.2f);
        HpController.gameObject.SetActive(false);
        //
        showDead();
    }
    //单位死亡逻辑处理
    public IEnumerator IEDie()
    {
        bool showNextUnitFlag = false;
        if(_Tag == SDConstants.CharacterType.Hero)
        {
            BM.Remaining_SRL.Remove(this);
        }
        else
        {
            BM.Remaining_ORL.Remove(this);
            gameObject.tag = "Untagged";
            showNextUnitFlag = BM.CheckBattleSuccess();
        }
        clearAllStates();

        yield return new WaitForSeconds(DAMAGE_ANIM_TIME);
        Debug.Log(transform.name + " die.");
        #region 死亡动画
        if (unit_character_model && unit_character_model.CurrentCharacterModel)
        {
            if (_Tag == SDConstants.CharacterType.Hero)
            {
                unit_character_model
                    .CurrentCharacterModel.ChangeModelAnim
                    (unit_character_model.CurrentCharacterModel.anim_die,true);

            }
            else if (_Tag == SDConstants.CharacterType.Enemy)
            {
                unit_character_model.CurrentCharacterModel.isEnemy = true;
                if (IsBoss)
                {
                    unit_character_model.CurrentCharacterModel.isBoss = true;
                    //BM.
                }
                unit_character_model
                    .CurrentCharacterModel.ChangeModelAnim
                    (unit_character_model.CurrentCharacterModel.anim_die);
                //
                unit_character_model.CurrentCharacterModel.transform
                    .DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
            }
            else
            {
                unit_character_model
                    .CurrentCharacterModel.ChangeModelAnim
                    (unit_character_model.CurrentCharacterModel.anim_die);
            }
        }
        #endregion
        #region 死亡掉落与结算
        TriggerManager.Instance.WhenUnitDie(this);
        if (_Tag == SDConstants.CharacterType.Hero)
        {
            SDDataManager.Instance.setHeroStatus(unitHashcode
                ,(int)SDConstants.HeroSelectType.Dying);//修改状态为濒死
        }
        else
        {
            //掉落获取
            float goldRatio = 1 + (float)(SDDataManager.Instance.getGoldPerc())/100;
            float goldR = ThisBasicRoleProperty().GoldRate * SDGameManager.Instance.goldRate * goldRatio;
            GoldC.showGold(goldR);
            if (IsBoss)
            {
                EnemyProperty.showDropItems();
            }
            else
            {
                EnemyProperty.showDropItem();
            }
            yield return new WaitForSeconds(REWARD_ANIM_TIME);
        }
        #endregion
        #region 成就记录
        if(_Tag == SDConstants.CharacterType.Hero)
        {
            SDDataManager.Instance.addAchievementDataByType("heroDie");
        }
        else if(_Tag == SDConstants.CharacterType.Enemy)
        {
            string id = ThisBasicRoleProperty().ID;
            ROEnemyData data = SDDataManager.Instance.getEnemyDataById(id);
            string t = "kill_" + data.Info.EnemyRank.EnemyType.ToString();
            SDDataManager.Instance.addAchievementDataByType(t);
        }
        #endregion
        #region Obj消失
        if (_Tag == SDConstants.CharacterType.Hero)
        {

        }
        else
        {
            gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
        }
        #endregion
        #region 判断游戏是否结束
        if (_Tag == SDConstants.CharacterType.Hero)
        {
            showNextUnitFlag = BM.CheckBattleLose();
            if (!showNextUnitFlag)
            {
                Debug.Log("hero die not lose");
            }
            else
            {
                Debug.Log("hero die then lose");
                BM.ABM.IsWaitingAction = true;
            }
        }
        else
        {
            if (IsBoss)
            {
                EnemyProperty.showDropItems();
            }
            else
            {
                EnemyProperty.showDropItem();
            }

            if (!showNextUnitFlag)
            {
                Debug.Log("enemy die not success");
            }
            else
            {
                Debug.Log("enemy die then success");
                BM.BattleSuccess();
            }
        }
        #endregion
        yield return new WaitForSeconds(0.2f);
        HpController.gameObject.SetActive(false);
        //
        showDead();
    }
    public void showDead()
    {
        if (Unit_Die) Unit_Die.gameObject.SetActive(true);
        //unit_character_model.GetComponentInChildren<SpriteRenderer>().color = Color.black;
        unit_model.GetComponentInChildren<SpriteRenderer>()
            .transform.localScale = Vector3.one * 0.25f;
    }
    public void hideDead()
    {
        if (Unit_Die) Unit_Die.gameObject.SetActive(false);
    }
    public void revive( RoleBarChart recoverVal)
    {
        hideDead();
        IsDead = false;
        HpController.addHp(recoverVal.HP);
        MpController.addMp(recoverVal.MP);
        TpController.addTp(recoverVal.TP);
        HpController.BarChartStatus.gameObject.SetActive(true);
        if (!IsEnemy)
        {
            BM.Remaining_SRL.Add(this);

        }
        else
        {
            BM.Remaining_ORL.Add(this);

        }
        unit_character_model.CurrentCharacterModel._isDead = false;
        unit_character_model.CurrentCharacterModel.SetReplaceImgState(true);
        unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (unit_character_model.CurrentCharacterModel.anim_idle, true);
    }
    #endregion
}
