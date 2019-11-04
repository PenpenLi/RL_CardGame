using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using GameDataEditor;

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
    [HideInInspector]
    public BasicRoleProperty GodnessProperty;
    public BasicRoleProperty ThisBasicRoleProperty()
    {
        if (_Tag == SDConstants.CharacterType.Hero) return HeroProperty;
        if (_Tag == SDConstants.CharacterType.Enemy) return EnemyProperty;
        if (_Tag == SDConstants.CharacterType.Godness) return GodnessProperty;
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
    public bool IsGodness;
    public bool IsFacingRight = true;

    public int rareWeight = 0;
    public bool isLittleBoss = false;

    public int UnitId;
    public int unitHashcode;
    public Transform unit_model;
    public CharacterModelController unit_character_model;
    //
    [HideInInspector]
    public BattleManager BM;

    #region 动画设置
    //[Header("动画设置")]
    private float moveDistance = 0.5f;
    private float moveTime = 0.2f;
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
        APC.transform.localScale = Vector3.one * 1.2f;
        if (GetComponent<HeroController>()) HeroProperty = GetComponent<HeroController>();
        if (GetComponent<EnemyController>()) EnemyProperty = GetComponent<EnemyController>();
        if (GetComponent<BasicRoleProperty>()) GodnessProperty = GetComponent<BasicRoleProperty>();
    }
    private void Start()
    {
        AllStates = new BRD_OneStateController[(int)StateTag.End];
        for (int i = 0; i < AllStates.Length; i++)
        {
            AllStates[i] = new BRD_OneStateController();
            AllStates[i].Clear(true);
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
    public void initEnemyController(int heroId)
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
        HeroProperty._hero.nameBeforeId = heroData.nameBeforeId;

        //
        UnitId = SDDataManager.Instance.getHeroIdByHashcode(unitHashcode);
        HeroProperty.ID = HeroProperty._hero.ID = UnitId;

        int herocareer = SDDataManager.Instance.getHeroCareerById(UnitId);
        int herorace = SDDataManager.Instance.getHeroRaceById(UnitId);
        ROHeroData dal = SDDataManager.Instance.getHeroDataByID(UnitId, heroData.starNumUpgradeTimes);
        SDConstants.CharacterAnimType type
            = (SDConstants.CharacterAnimType)
            (SDDataManager.Instance.getHeroCareerById(UnitId));
        unit_character_model.initCharacterModel(heroHashCode, type, 0.6f);
        int grade = SDDataManager.Instance.getLevelByExp(heroData.exp);
        HeroProperty._hero.grade = grade;
        HeroProperty._hero.initData_Hero
            ((Job)herocareer, (Race)herorace
            , grade, 0, dal.starNum, dal.BasicRAL, dal.RALRate
            , dal.CRI, dal.CRIDmg, dal.DmgReduction, dal.DmgReflection, dal.RewardRate
            , dal.BarChartRegendPerTurn, UnitId, dal.Name, heroData.wakeNum);
        HeroProperty._hero.gender = dal.gender;
        addSkillByCareerByRaceByStarnum(heroHashCode, dal.starNum);
    }

    #region 添加角色配置
    public void initHelmet(int heroHashCode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipHelmet(heroHashCode);
        Helmet a = unit_model.gameObject.AddComponent<Helmet>();
        HeroProperty._helmet = a;
        if (armor == null || armor.equipId == 0)
        {
            HeroProperty._helmet.initDataEmpty();return;
        }
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("equip");
        for(int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if(SDDataManager.Instance.getInteger(s["id"]) == armor.equipId)
            {
                int level = armor.equipLv + armor.upLv;
                int upLv = armor.upLv;
                RoleAttributeList basicRAL = SDDataManager.Instance.RALByDictionary(s);
                basicRAL = SDDataManager.Instance.getRALByUpLv(basicRAL, armor.upLv);
                string name = s["name"];
                int id = SDDataManager.Instance.getInteger(s["id"]);
                string passiveEffect = s["passiveEffect"];
                RoleAttributeList rateRAL = new RoleAttributeList();
                HeroProperty._helmet.initData(level, basicRAL, rateRAL, 0, 0, 0, 0, 0, RoleBarChart.zero
                    , id, name, 0);
                HeroProperty._helmet.PassiveEffectInit(passiveEffect);
                HeroProperty._helmet.armorType = (SDConstants.ArmorType)
                    SDDataManager.Instance.getInteger(s["type"]);
            }
        }
    }
    public void initBreastplate(int heroHashCode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipBreastplate(heroHashCode);
        Breastplate a = unit_model.gameObject.AddComponent<Breastplate>();
        HeroProperty._breastplate = a;
        if (armor == null || armor.equipId == 0)
        {
            HeroProperty._breastplate.initDataEmpty(); return;
        }
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("equip");
        for (int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if (SDDataManager.Instance.getInteger(s["id"]) == armor.equipId)
            {
                int level = armor.equipLv + armor.upLv;
                int upLv = armor.upLv;
                RoleAttributeList basicRAL = SDDataManager.Instance.RALByDictionary(s);
                string name = s["name"];
                int id = SDDataManager.Instance.getInteger(s["id"]);
                string passiveEffect = s["passiveEffect"];
                RoleAttributeList rateRAL = new RoleAttributeList();
                HeroProperty._breastplate.initData(level, basicRAL, rateRAL
                    , 0, 0, 0, 0, 0, RoleBarChart.zero
                    , id, name, 0);
                HeroProperty._breastplate.PassiveEffectInit(passiveEffect);
                HeroProperty._breastplate.armorType = (SDConstants.ArmorType)
                    SDDataManager.Instance.getInteger(s["type"]);
            }
        }
    }
    public void initGardebras(int heroHashCode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipGardebras(heroHashCode);
        Gardebras a = unit_model.gameObject.AddComponent<Gardebras>();
        HeroProperty._gardebras = a;
        if (armor == null || armor.equipId == 0)
        {
            HeroProperty._gardebras.initDataEmpty(); return;
        }
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("equip");
        for (int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if (SDDataManager.Instance.getInteger(s["id"]) == armor.equipId)
            {
                int level = armor.equipLv + armor.upLv;
                int upLv = armor.upLv;
                RoleAttributeList basicRAL = SDDataManager.Instance.RALByDictionary(s);
                string name = s["name"];
                int id = SDDataManager.Instance.getInteger(s["id"]);
                string passiveEffect = s["passiveEffect"];
                RoleAttributeList rateRAL = new RoleAttributeList();
                HeroProperty._gardebras.initData(level, basicRAL, rateRAL
                    , 0, 0, 0, 0, 0, RoleBarChart.zero
                    , id, name, 0);
                HeroProperty._gardebras.PassiveEffectInit(passiveEffect);
                HeroProperty._gardebras.armorType = (SDConstants.ArmorType)
                    SDDataManager.Instance.getInteger(s["type"]);
            }
        }
    }
    public void initLegging(int heroHashCode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipLegging(heroHashCode);
        Legging a = unit_model.gameObject.AddComponent<Legging>();
        HeroProperty._legging = a;
        if (armor == null || armor.equipId == 0)
        {
            HeroProperty._legging.initDataEmpty(); return;
        }
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("equip");
        for (int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if (SDDataManager.Instance.getInteger(s["id"]) == armor.equipId)
            {
                int level = armor.equipLv + armor.upLv;
                int upLv = armor.upLv;
                RoleAttributeList basicRAL = SDDataManager.Instance.RALByDictionary(s);
                string name = s["name"];
                int id = SDDataManager.Instance.getInteger(s["id"]);
                string passiveEffect = s["passiveEffect"];
                RoleAttributeList rateRAL = new RoleAttributeList();
                HeroProperty._legging.initData(level, basicRAL, rateRAL
                    , 0, 0, 0, 0, 0,  RoleBarChart.zero
                    , id, name, 0);
                HeroProperty._legging.PassiveEffectInit(passiveEffect);
                HeroProperty._legging.armorType = (SDConstants.ArmorType)
                    SDDataManager.Instance.getInteger(s["type"]);
            }
        }
    }
    public void initJewelry(int heroHashCode,bool isSecondPos = false)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipJewelry(heroHashCode);
        Jewelry a = unit_model.gameObject.AddComponent<Jewelry>();
        if (!isSecondPos)
        {
            HeroProperty._jewelry0 = a;
            if (armor == null || armor.equipId == 0)
            {
                HeroProperty._jewelry0.initDataEmpty(); return;
            }
            List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("jewelry");
            for (int i = 0; i < itemDatas.Count; i++)
            {
                Dictionary<string, string> s = itemDatas[i];
                if (SDDataManager.Instance.getInteger(s["id"]) == armor.equipId)
                {
                    int level = armor.equipLv + armor.upLv;
                    int upLv = armor.upLv;
                    RoleAttributeList basicRAL = SDDataManager.Instance.EquipRALByDictionary(s);
                    string name = s["name"];
                    int id = SDDataManager.Instance.getInteger(s["id"]);
                    string passiveEffect = s["passiveEffect"];
                    RoleAttributeList rateRAL = new RoleAttributeList();
                    HeroProperty._jewelry0.initData(level, basicRAL, rateRAL
                        , 0, 0, 0, 0, 0, RoleBarChart.zero
                        , id, name, 0);
                    HeroProperty._jewelry0.PassiveEffectInit(passiveEffect);
                    HeroProperty._jewelry0._jewelryType = (SDConstants.JewelryType)
                        SDDataManager.Instance.getInteger(s["type"]);
                }
            }
        }
        else
        {
            HeroProperty._jewelry1 = a;
            if (armor == null || armor.equipId == 0)
            {
                HeroProperty._jewelry1.initDataEmpty(); return;
            }
            List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("jewelry");
            for (int i = 0; i < itemDatas.Count; i++)
            {
                Dictionary<string, string> s = itemDatas[i];
                if (SDDataManager.Instance.getInteger(s["id"]) == armor.equipId)
                {
                    int level = armor.equipLv + armor.upLv;
                    int upLv = armor.upLv;
                    //主副属性类型装备读取
                    RoleAttributeList basicRAL = SDDataManager.Instance.EquipRALByDictionary(s);
                    //
                    string name = s["name"];
                    int id = SDDataManager.Instance.getInteger(s["id"]);
                    string passiveEffect = s["passiveEffect"];
                    RoleAttributeList rateRAL = new RoleAttributeList();
                    HeroProperty._jewelry1.initData(level, basicRAL, rateRAL
                        , 0, 0, 0, 0, 0,  RoleBarChart.zero
                        , id, name, 0);
                    HeroProperty._jewelry1.PassiveEffectInit(passiveEffect);
                    HeroProperty._jewelry1._jewelryType = (SDConstants.JewelryType)
                        SDDataManager.Instance.getInteger(s["type"]);
                }
            }
        }
    }
    public void initWeapon(int heroHashCode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroWeapon(heroHashCode);
        SDWeapon a = unit_model.gameObject.AddComponent<SDWeapon>();
        HeroProperty._weapon = a;
        if (armor == null || armor.equipId == 0)
        {
            HeroProperty._weapon.initDataEmpty(); return;
        }
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("weapon");
        for (int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if (SDDataManager.Instance.getInteger(s["id"]) == armor.equipId)
            {
                int level = armor.equipLv + armor.upLv;
                int upLv = armor.upLv;
                RoleAttributeList basicRAL = SDDataManager.Instance.EquipRALByDictionary(s);
                string name = s["name"];
                int id = SDDataManager.Instance.getInteger(s["id"]);
                string passiveEffect = s["passiveEffect"];
                RoleAttributeList rateRAL = new RoleAttributeList();
                HeroProperty._weapon.initData(level, basicRAL, rateRAL
                    , 0, 0, 0, 0, 0,  RoleBarChart.zero
                    , id, name, 0);
                HeroProperty._weapon.PassiveEffectInit(passiveEffect);
                HeroProperty._weapon._weaponType = (SDConstants.WeaponType)
                    SDDataManager.Instance.getInteger(s["type"]);
            }
        }
    }
    #endregion
    public void addSkillByCareerByRaceByStarnum(int heroHashcode,int quality)
    {
        GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(heroHashcode);
        int skill0Id = hero.skill0Id;
        _skills.Add(SDDataManager.Instance.getOwnedSkillById(skill0Id, heroHashcode));
        if (SDDataManager.Instance.checkHeroEnableSkill1ByHashcode(heroHashcode))
        {
            int skill1Id = hero.skill1Id;
            _skills.Add(SDDataManager.Instance.getOwnedSkillById(skill1Id, heroHashcode));
        }//稀有角色才能拥有额外技能
        int skillOmegaId = hero.skillOmegaId;
        _skills.Add(SDDataManager.Instance.getOwnedSkillById(skillOmegaId, heroHashcode));
        return;


        int herocareer = SDDataManager.Instance.getHeroCareerById(UnitId);
        int herorace = SDDataManager.Instance.getHeroRaceById(UnitId);
        List<OneSkill> skills = SkillDetailsList.WriteOneSkillList(
            (Job)herocareer,(Race)herorace
            ,quality);
        _skills.Add(skills[0]);
        if(skills.Count>1)
            _skills.Add(skills[1]);
        if(skills.Count>2)
            _skills.Add(skills[2]);
    }
    public void initEnemy(int enemyId)
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
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("enemy");
        ROHeroData d = new ROHeroData();
        int race = 0;
        int gender = -1;
        int starNum = 0;
        int quality = 0;
        string name = "";
        for (int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if (s["id"] == enemyId.ToString())
            {
                d.BasicRAL = SDDataManager.Instance.RALByDictionary(s);
                race = SDDataManager.Instance.getInteger(s["race"]);
                gender = SDDataManager.Instance.getInteger(s["gender"]);
                quality = SDDataManager.Instance.getInteger(s["quality"]);
                name = s["name"];
                break;
            }
        }
        int career = (int)Job.End;
        if (rareWeight > 0) career = UnityEngine.Random.Range(0, (int)Job.End);
        d = enemyBuild.AddCareerData(d, career, race);
        d.gender = gender;
        d.Name = name;
        //
        d.RALRate = new RoleAttributeList();
        d.BasicRAL = enemyBuild.RALAddedByLevel(d.BasicRAL);
        //
        if (isLittleBoss) d = enemyBuild.LittleBossData(d, quality);
        //
        EnemyProperty._enemy.initData(starNum, d.BasicRAL, d.RALRate
            , d.CRI, d.CRIDmg, d.DmgReduction
            , d.DmgReflection, d.RewardRate, d.BarChartRegendPerTurn, d.ID, d.Name, 0);
        enemyBuild EB = FindObjectOfType<enemyBuild>();
        if (EB)
        {
            List<OneSkill> skills = EB.WriteEnemySkills(career, race, starNum);
            if(skills!=null)
            for (int i = 0; i < skills.Count; i++) _skills.Add(skills[i]);
        }
    }
    #endregion
    #region 状态类
    #region 全状态列表
    [Header("状态类内容")]
    public Text actionUnitStateText;
    //public Transform statePanel;
    public string SpecialStateSign(StateTag tag)
    {
        switch (tag)
        {
            case StateTag.Bleed: return "①";
            case StateTag.Mind: return "②";
            case StateTag.Fire: return "③";
            case StateTag.Frost: return "④";
            case StateTag.Corrosion: return "⑤";
            case StateTag.Nature: return "⑥";
            case StateTag.Dizzy: return "⑦";
            case StateTag.Confuse: return "⑧";
            default: return null;
        }
    }
    #region 状态可视化内容
    [Header("状态显示设置")]
    public Transform Unit_Bleed;
    public Transform Unit_Mind;
    public Transform Unit_Fire;
    public Transform Unit_Frost;
    public Transform Unit_Corrosion;
    public Transform Unit_Nature;
    public Transform Unit_Dizzy;
    public Transform Unit_Confuse;
    public Transform PerStateUnit(StateTag tag)
    {
        switch (tag)
        {
            case StateTag.Bleed: return Unit_Bleed;
            case StateTag.Mind: return Unit_Mind;
            case StateTag.Fire: return Unit_Fire;
            case StateTag.Frost: return Unit_Frost;
            case StateTag.Corrosion: return Unit_Corrosion;
            case StateTag.Nature: return Unit_Nature;
            case StateTag.Dizzy: return Unit_Dizzy;
            case StateTag.Confuse: return Unit_Confuse;
            default: return null;
        }
    }
    //
    public Transform Unit_Die;
    #endregion
    #region　撕裂状态①
    public BRD_OneStateController stateBleed
    { get { return AllStates[(int)StateTag.Bleed]; }
        set { AllStates[(int)StateTag.Bleed] = value; }
    }
    #endregion
    #region 压力状态②
    public BRD_OneStateController stateMind
    {
        get { return AllStates[(int)StateTag.Mind]; }
        set { AllStates[(int)StateTag.Mind] = value; }
    }
    #endregion
    #region 灼烧状态③
    public BRD_OneStateController stateFire
    {
        get { return AllStates[(int)StateTag.Fire]; }
        set { AllStates[(int)StateTag.Fire] = value; }
    }
    #endregion
    #region 霜冻状态④
    public BRD_OneStateController stateFrost
    {
        get { return AllStates[(int)StateTag.Frost]; }
        set { AllStates[(int)StateTag.Frost] = value; }
    }
    #endregion
    #region 腐蚀状态⑤
    public BRD_OneStateController stateCorrosion
    {
        get { return AllStates[(int)StateTag.Corrosion]; }
        set { AllStates[(int)StateTag.Corrosion] = value; }
    }
    #endregion
    #region 天性状态⑥
    public BRD_OneStateController stateNature
    {
        get { return AllStates[(int)StateTag.Nature]; }
        set { AllStates[(int)StateTag.Nature] = value; }
    }
    #endregion
    #region 眩晕状态⑦
    public BRD_OneStateController stateDizzy
    {
        get { return AllStates[(int)StateTag.Dizzy]; }
        set { AllStates[(int)StateTag.Dizzy] = value; }
    }
    #endregion
    #region 混乱状态⑧
    public BRD_OneStateController stateConfuse
    {
        get { return AllStates[(int)StateTag.Confuse]; }
        set { AllStates[(int)StateTag.Confuse] = value; }
    }
    #endregion
    [HideInInspector]
    public BRD_OneStateController[] AllStates;
    public BRD_OneStateController OneState(StateTag tag)
    {
        return AllStates[(int)tag];
    }
    #endregion
    #region 应对单个状态脚本
    public void show_state(StateTag tag)
    {
        PerStateUnit(tag)?.gameObject.SetActive(true);
        //
        if (!actionUnitStateText.text.Contains(SpecialStateSign(tag)))
        {
            actionUnitStateText.text += SpecialStateSign(tag);
        }
    }
    public void hide_state(StateTag tag)
    {
        PerStateUnit(tag)?.gameObject.SetActive(false);
        //
        if (actionUnitStateText.text.Contains(SpecialStateSign(tag)))
        {
            string s = actionUnitStateText.text;
            s = s.Replace(SpecialStateSign(tag), "");
            actionUnitStateText.text = s;
        }
    }
    public void clear_state(StateTag tag)
    {
        OneState(tag).Clear();
    }
    public void checkPerState(StateTag tag)
    {
        if (OneState(tag).LastTime > 0)
        {
            OneState(tag).LastTime--;
            if (OneState(tag).LastTime == 0)
            {
                hide_state(tag);
                clear_state(tag);
            }
        }
    }
    #endregion
    public void hideAllStates()
    {
        for (int i = 0; i < (int)StateTag.End; i++)
        {
            hide_state((StateTag)i);
        }
    }
    public void hideStatusAndShadow()
    {
        actionUnitStateText.text = "";
        //statePanel
    }
    #region 快速状态索引器
    public bool ReadThisStateEnable(StateTag tag)
    {
        int c = OneState(tag).stateCondition;
        return c != 0;
    }
    public int ReadThisStateLastTime(StateTag tag)
    {
        return OneState(tag).LastTime;
    }
    public StateEffectStorage ReadThisStateEffect(StateTag tag)
    {
        return OneState(tag).Storage;
    }
    public int ReadThisStateDmg(StateTag tag)
    {
        return OneState(tag).ExtraDmg;
    }
    /// <summary>
    /// 控制状态图标的显示与隐藏
    /// </summary>
    /// <param name="Tag">状态标签</param>
    /// <param name="Show">状态显示true;隐藏false</param>
    public void ControlStateVisual(StateTag Tag, bool Show, int freshtime = 0)
    {
        bool ChangeVisual = false;
        if (Show && OneState(Tag).stateCondition == 0)
        {
            OneState(Tag).stateCondition = 1;
            ChangeVisual = true;
            OneState(Tag).LastTime = freshtime != 0 ? freshtime : 0;
            show_state(Tag);
        }
        else if (!Show && OneState(Tag).stateCondition != 0)
        {
            OneState(Tag).stateCondition = 0;
            ChangeVisual = true;
            hide_state(Tag);
        }
        switch (Tag)
        {
            case StateTag.Bleed:
                break;
            case StateTag.Mind:
                break;
            case StateTag.Fire:
                break;
            case StateTag.Frost:
                break;
            case StateTag.Corrosion:
                break;
            case StateTag.Nature:
                break;
            case StateTag.Dizzy:
                break;
            case StateTag.Confuse:
                break;
        }
        if (ChangeVisual)//修改状态列表图片时触发
        {

        }
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
        ThisBasicRoleProperty()._role.AllARevision = new RoleAttributeList();
        for(int i = 0; i < (int)StateTag.End; i++)
        {
            checkPerState((StateTag)i);
            ThisBasicRoleProperty()._role.AllARevision
                += ReadThisStateEffect((StateTag)i).UniqueEffectInRA;
        }
        #region consume or add BarChart
        RoleBarChart rbc = checkStatesChangingBC();
        if (rbc.HP > 0) 
            HpController.addHp(rbc.HP);
        else HpController.consumeHp(Mathf.Abs(rbc.HP));
        if (rbc.MP > 0)
            MpController.addMp(rbc.MP);
        else MpController.consumeMp(Mathf.Abs(rbc.MP));
        if (rbc.TP > 0) 
            TpController.addTp(rbc.TP);
        else TpController.consumeTp(Mathf.Abs(rbc.TP));
        #endregion
        #region dmg
        int dmg = checkStatesDamaging();
        HpController.getExtraDamage(dmg);
        #endregion
        #region otherBuff
        if (!IsEnemy && !IsGodness)
        {
            Race _race = HeroProperty._hero._heroRace;
            RoleAttributeList basic = ThisBasicRoleProperty()._role.ThisRoleAttributes;
            ThisBasicRoleProperty()._role.AllARevision += SDDataManager.Instance.BuffFromDaynight(basic);
            ThisBasicRoleProperty()._role.AllARevision += SDDataManager.Instance.BuffFromRace(basic, _race);
        }

        #endregion
        #region extraState
        RoleExtraState.checkExtraStates();
        #endregion
        //writeData();
    }
    public void writeData()
    {
        string d = "";
        RoleAttributeList l = ThisBasicRoleProperty()._role.AllARevision;
        for (int i = 0; i < l.AllAttributeData.Length; i++)
        {
            if (l.AllAttributeData[i] != 0)
            {
                d += (AttributeData)i + " " + l.AllAttributeData[i] + "|| ";
            }
        }
        string d1 = "";
        RoleBarChart rbc = checkStatesChangingBC();
        for (int i = 0; i < 3; i++)
        {
            if(rbc.ThisArray(i) != 0)
            {
                d1 += "BAR_CHART:" + i + " COST: " + rbc.ThisArray(i) + "=== "; 
            }
        }
        string d2 = "";
        int dmg = checkStatesDamaging();
        if (dmg != 0)
        {
            d2 += " 状态持续造成伤害:" + dmg;
        }
        string all = d + d1 + d2;
        if (all != "" && all != null )
        {
            Debug.Log((d!=""?(d + "\n"):null)
                + (d1 != "" ? (d1+ "\n") : null)
                + (d2!=""?d2:null));
        }
    }




    #region 状态造成伤害汇总
    public RoleBarChart checkStatesChangingBC()
    {
        RoleBarChart rbc = RoleBarChart.zero;
        if (ThisBasicRoleProperty().BarChartRegendPerTurn != null)
        {
            rbc += ThisBasicRoleProperty().BarChartRegendPerTurn;
        }
        for (int i = 0; i < (int)StateTag.End; i++)
        {
            if (AllStates[i].stateCondition != 0)
            {
                rbc += AllStates[i].Storage.BCArray;
            }
        }
        return rbc;
    }
    public int checkStatesDamaging()
    {
        int d = 0;
        for (int i = 0; i < (int)StateTag.End; i++)
        {
            if (AllStates[i].stateCondition != 0)
            {
                d += AllStates[i].ExtraDmg;
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
        realC = (int)(realC * AllRandomSetClass.SimplePercentToDecimal
            (ThisBasicRoleProperty().CRI + 100));
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
            int baseA = ThisBasicRoleProperty().ReadRA((int)AttributeData.Accur);
            int realA = (int)(baseA * AllRandomSetClass.SimplePercentToDecimal(_skill.AccuracyR + 100));
            int Evo = _target.ReadRA(AttributeData.Evo);
            float Rate = AdolescentSet.AccurFunction(realA, Evo);
            float t = UnityEngine.Random.Range(0, 1f);
            bool _IsAccur = t < Rate;
            AccurHappen = _IsAccur;
            //Debug.Log("精准度计算：" + _IsAccur +" || "+ t + " " + Rate );
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
        int _expect = ThisBasicRoleProperty().ReadRA((int)AttributeData.Expect);
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
        unit_model.GetComponent<CanvasGroup>().DOFade(0, moveTime * 0.5f);
        yield return new WaitForSeconds(moveTime * 0.5f);
        if (IsFacingRight)
        {
            unit_model.position = targetPos - new Vector2(moveDistance, 0);
        }
        else
        {
            unit_model.position = targetPos + new Vector2(moveDistance, 0);
        }
        unit_model.GetComponent<CanvasGroup>().DOFade(1, moveTime * 0.5f);
        yield return new WaitForSeconds(moveTime * 0.5f);
    }
    public void playMoveBackAnimation()
    {
        StartCoroutine(IEPlayMoveBackAnimation());
    }
    public IEnumerator IEPlayMoveBackAnimation()
    {
        unit_model.GetComponent<CanvasGroup>().DOFade(0, moveTime * 0.5f);
        yield return new WaitForSeconds(moveTime * 0.5f);
        unit_model.localPosition = modelOriginLocalPos;
        unit_model.GetComponent<CanvasGroup>().DOFade(1, moveTime * 0.5f);
        yield return new WaitForSeconds(moveTime * 0.5f);
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
    #region 角色死亡与复活设置
    public void playDieAnimation()
    {
        Debug.Log(this.name + "死亡");
        StartCoroutine(IEDie());
    }
    //单位死亡逻辑处理
    public IEnumerator IEDie()
    {
        bool showNextUnitFlag = false;
        if(_Tag == SDConstants.CharacterType.Hero)
        {
            BM.Remaining_SRL.Remove(this);
            hideAllStates();
            //hidestatusandshadow();
        }
        else
        {
            BM.Remaining_ORL.Remove(this);
            gameObject.tag = "Untagged";
            showNextUnitFlag = BM.CheckBattleSuccess();
            hideAllStates();
        }
        yield return new WaitForSeconds(DAMAGE_ANIM_TIME);
        Debug.Log(transform.name + " die.");
        #region 死亡动画
        if (_Tag == SDConstants.CharacterType.Hero)
        {
            unit_character_model.CurrentCharacterModel.ChangeModelAnim(SDConstants.AnimName_DIE);

        }
        else
        {
            unit_character_model.CurrentCharacterModel.isEnemy = true;
            if (IsBoss)
            {
                unit_character_model.CurrentCharacterModel.isBoss = true;
                //BM.
            }
            unit_character_model.CurrentCharacterModel.ChangeModelAnim(SDConstants.AnimName_DIE);
        }
        #endregion
        #region 死亡掉落
        if (_Tag == SDConstants.CharacterType.Hero)
        {
            SDDataManager.Instance.setHeroStatus(unitHashcode, 2);
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
        unit_character_model.CurrentCharacterModel.ChangeModelAnim(SDConstants.AnimName_IDLE, true);
    }
    #endregion
}
