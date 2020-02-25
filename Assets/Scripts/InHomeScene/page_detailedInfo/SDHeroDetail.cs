using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using System.Linq;

/// <summary>
/// 角色详情页面
/// </summary>
public class SDHeroDetail : BasicRoleProperty
{
    public SDConstants.CharacterType Type = SDConstants.CharacterType.Hero;
    //public string Id;
    public int Hashcode;
    public int Status;
    public Job careerIndex;
    public Race raceIndex;

    public int BattleForce;

    public SDHero _hero;
    public SDHeroSelect heroSelect;
    public BasicHeroSelect BHS;
    [Header("立绘和角色详细信息 栏")]
    public Image heroCharacterDrawingImg;
    public Image HCdI_Bg;

    public Text NameBeforeText;
    public Image RarityImg;
    public ItemStarVision StarNumVision;
    public Text LvText;
    public AttritubeListPanel RALPanel;
    public Transform RoleParticularPanel;
    public Button LockedBtn;
    public Sprite[] LockedSpriteArray;
    private bool _IsLocked;
    public bool IsLocked
    {
        get { return _IsLocked; }
        set 
        {
            _IsLocked = value; 
            LockedBtn.GetComponent<Image>().sprite =
                _IsLocked 
                ? LockedSpriteArray[1] : LockedSpriteArray[0];
        }
    }
    [Header("小人动画模型 栏")]
    public Text NameText;
    public CharacterModelController heroHeadImg;
    public Transform modelSubPanel;
    [Header("装备和身份信息 栏")]
    public HeroEquipList equipList;
    #region AllEquipments
    Helmet _helmet { get { return equipList._helmet; } }
    Breastplate _breastplate { get { return equipList._breastplate; } }
    Gardebras _gardebras { get { return equipList._gardebras; } }
    Legging _legging { get { return equipList._legging; } }
    Jewelry _jewelry0 { get { return equipList._jewelry0; } }
    Jewelry _jewelry1 { get { return equipList._jewelry1; } }
    SDWeapon _weapon { get { return equipList._weapon; } }
    #endregion
    #region MidSubPanelContent
    public Image CareerIconImg;
    public Text CareerText;
    public Image RaceIconImg;
    public Text RaceText;
    public Text battleforceText;
    #endregion
    public Transform EquipSubPanel;
    [Header("角色技能与详情 栏")]
    public string skillid0;
    public string skillid1;
    public string skillidOmega;
    public SkillSlot[] SkillSlots;
    [Header("上级信息 栏")]
    public HeroDetailPanel HeroWholeMessage;


    private void Awake()
    {
        equipList.HD = this;
        if (_hero == null)
        {
            _hero = equipList.gameObject.AddComponent<SDHero>();
        }
    }
    public void initHeroDetailPanel(int hashcode
        , SDConstants.CharacterType type = SDConstants.CharacterType.Hero)
    {
        Type = type;
        if (Type == SDConstants.CharacterType.Hero)
        {
            equipList.BuildEquipListBase();
            equipList.HD = this;
            if (_hero == null)
            {
                _hero = equipList.gameObject.AddComponent<SDHero>();
            }
            ID = SDDataManager.Instance.getHeroIdByHashcode(hashcode);
            GDEHeroData hero = SDDataManager.Instance.GetHeroOwnedByHashcode(hashcode);
            Hashcode = hashcode;

            setHero(Hashcode);
            setHelmet(Hashcode);
            setBreastplate(Hashcode);
            // setGardebras(Hashcode);//已过期
            setLegging(Hashcode);
            setJewelry(Hashcode , false);
            setJewelry(Hashcode , true);
            setWeapon(Hashcode);
            InitHeroBasicProperties();
            RALPanel.initRAL(this.RoleBasicRA
                , Type
                , SDDataManager.Instance.getLevelByExp(hero.exp));//视觉展示属性
            setRoleBaseMessiage();
            if (LvText)
            {
                int exp = hero.exp;
                int lv = SDDataManager.Instance.getLevelByExp(exp);
                LvText.text = SDGameManager.T("Lv.") + lv;
            }


            readHeroSkills();
            //
            heroHeadImg.initHeroCharacterModel(Hashcode, SDConstants.HERO_MODEL_BIG_RATIO);
        }
    }
    #region 读取角色属性
    public void setHero(int hashcode)
    {

        GDEHeroData heroData = SDDataManager.Instance.GetHeroOwnedByHashcode(hashcode);
        string id = heroData.id;
        ID = id;
        careerIndex = SDDataManager.Instance.getHeroCareerById(id);
        raceIndex = SDDataManager.Instance.getHeroRaceById(id);

        ROHeroData dal = SDDataManager.Instance.getHeroDataByID(id, heroData.starNumUpgradeTimes);


        //career
        RoleCareer c = dal.Info.Career;
        CareerIconImg.sprite = c.Icon;
        CareerIconImg.SetNativeSize();
        CareerText.text = SDGameManager.T(c.NAME);
        //race
        HeroRace r = dal.Info.Race;
        RaceIconImg.sprite = r.Icon;
        RaceIconImg.SetNativeSize();
        RaceText.text = SDGameManager.T(r.NAME);
        //rarity
        RarityImg.sprite = SDDataManager.Instance.raritySprite(dal.quality);
        RarityImg.SetNativeSize();
        //personalDrawImg
        if (dal.Info.PersonalDrawImg == null)
        {
            HCdI_Bg.gameObject.SetActive(false);
            heroCharacterDrawingImg.sprite = RaceIconImg.sprite;
            heroCharacterDrawingImg.SetNativeSize();
            heroCharacterDrawingImg.color = Color.grey;
        }
        else
        {
            HCdI_Bg.gameObject.SetActive(true);
            heroCharacterDrawingImg.sprite = dal.Info.PersonalDrawImg;
            heroCharacterDrawingImg.SetNativeSize();
            heroCharacterDrawingImg.color = Color.white;
            HCdI_Bg.sprite = RaceIconImg.sprite;
            HCdI_Bg.SetNativeSize();
            HCdI_Bg.color = Color.white;
        }



        //RaceIconImg.sprite =
        int grade = SDDataManager.Instance.getLevelByExp(heroData.exp);
        _hero.gender = (CharacterSex)heroData.sex;
        _hero.initData_Hero((Job)careerIndex, raceIndex, grade, dal.quality, dal.starNum
            , dal.ExportRAL
            , dal.CRIDmg, dal.DmgReduction, dal.DmgReflection, dal.RewardRate
            , dal.BarChartRegendPerTurn, ID, dal.Info.Name, heroData.wakeNum);
    }
    public void setHelmet(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipHelmet(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            equipList.EquipVision(EquipPosition.Head).initEquipVision(armor);
            _helmet.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            equipList.EquipVision(EquipPosition.Head).initEquipVision(armor);
            _helmet.initDataEmpty(); return;
        }
        //
        _helmet.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        _helmet.PassiveEffectInit(Item.PassiveEffect);
        _helmet.armorRank = Item.ArmorRank;
        //
        equipList.EquipVision(EquipPosition.Head).initEquipVision(armor);
    }
    public void setBreastplate(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipBreastplate(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            equipList.EquipVision(EquipPosition.Breast).initEquipVision(armor);
            _breastplate.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            equipList.EquipVision(EquipPosition.Breast).initEquipVision(armor);
            _breastplate.initDataEmpty(); return;
        }
        //
        _breastplate.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        _breastplate.PassiveEffectInit(Item.PassiveEffect);
        _breastplate.armorRank = Item.ArmorRank;
        //
        equipList.EquipVision(EquipPosition.Breast).initEquipVision(armor);
    }
    public void setGardebras(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipGardebras(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            equipList.EquipVision(EquipPosition.Arm).initEquipVision(armor);
            _gardebras.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            equipList.EquipVision(EquipPosition.Arm).initEquipVision(armor);
            _gardebras.initDataEmpty(); return;
        }
        //
        _gardebras.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        _gardebras.PassiveEffectInit(Item.PassiveEffect);
        _gardebras.armorRank = Item.ArmorRank;
        //
        equipList.EquipVision(EquipPosition.Arm).initEquipVision(armor);
    }
    public void setLegging(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipLegging(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            equipList.EquipVision(EquipPosition.Leg).initEquipVision(armor);
            _legging.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            equipList.EquipVision(EquipPosition.Leg).initEquipVision(armor);
            _legging.initDataEmpty(); return;
        }
        //
        _legging.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        _legging.PassiveEffectInit(Item.PassiveEffect);
        _legging.armorRank = Item.ArmorRank;
        //
        equipList.EquipVision(EquipPosition.Leg).initEquipVision(armor);
    }
    public void setJewelry(int hashcode, bool isSecondPos = false)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipJewelry(hashcode, isSecondPos);
        if (!isSecondPos)
        {
            if (armor == null || string.IsNullOrEmpty(armor.id))
            {
                equipList.EquipVision(EquipPosition.Finger,isSecondPos).initEquipVision(armor);
                _jewelry0.initDataEmpty(); return;
            }
            EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
            if (Item == null)
            {
                equipList.EquipVision(EquipPosition.Finger,isSecondPos).initEquipVision(armor);
                _jewelry0.initDataEmpty(); return;
            }
            //
            _jewelry0.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
                , Item.ID, Item.NAME, 0);
            _jewelry0.PassiveEffectInit(Item.PassiveEffect);
            _jewelry0.armorRank = Item.ArmorRank;
            //
            equipList.EquipVision(EquipPosition.Finger, isSecondPos).initEquipVision(armor);
        }
        else
        {
            if (armor == null ||string.IsNullOrEmpty(armor.id))
            {
                equipList.EquipVision(EquipPosition.Finger, isSecondPos).initEquipVision(armor);
                _jewelry1.initDataEmpty(); return;
            }
            EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
            if (Item == null)
            {
                equipList.EquipVision(EquipPosition.Finger,isSecondPos).initEquipVision(armor);
                _jewelry1.initDataEmpty(); return;
            }
            //
            _jewelry1.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
                , Item.ID, Item.NAME, 0);
            _jewelry1.PassiveEffectInit(Item.PassiveEffect);
            _jewelry1.armorRank = Item.ArmorRank;
            //
            equipList.EquipVision(EquipPosition.Finger, isSecondPos).initEquipVision(armor);
        }
    }
    public void setWeapon(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroWeapon(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id) || armor.hashcode == 0)
        {
            equipList.EquipVision(EquipPosition.Hand).initEquipVision(armor);
            _weapon.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            equipList.EquipVision(EquipPosition.Hand).initEquipVision(armor);
            _weapon.initDataEmpty(); return;
        }
        //
        _weapon.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        _weapon.PassiveEffectInit(Item.PassiveEffect);
        _weapon.armorRank = Item.ArmorRank;
        //
        equipList.EquipVision(EquipPosition.Hand).initEquipVision(armor);
    }

    public void InitHeroBasicProperties()
    {
        if (_hero)
        {
            this.RoleBasicRA = _hero.RoleBasicRA.Clone;
            this.CRIDmg = _hero.CRIDmg;
            this.DmgReduction = _hero.DmgReduction;
            this.DmgReflection = _hero.DmgReflection;
            this.RewardRate = _hero.RewardRate;
            this.BarChartRegendPerTurn = _hero.BarChartRegendPerTurn;

            this.ID = _hero.ID;
            this.Name = SDGameManager.T(_hero.Name);
            this.LEVEL = _hero.LEVEL;
        }

        if (_helmet)
        {
            this.RoleBasicRA += _helmet.RoleBasicRA.Clone;
            this.CRIDmg += _helmet.CRIDmg;
            this.DmgReduction += _helmet.DmgReduction;
            this.DmgReflection += _helmet.DmgReflection;
            this.RewardRate += _helmet.RewardRate;
            this.BarChartRegendPerTurn += _helmet.BarChartRegendPerTurn;
        }
        if (_breastplate)
        {
            this.RoleBasicRA += _breastplate.RoleBasicRA.Clone;
            this.CRIDmg += _breastplate.CRIDmg;
            this.DmgReduction += _breastplate.DmgReduction;
            this.DmgReflection += _breastplate.DmgReflection;
            this.RewardRate += _breastplate.RewardRate;
            this.BarChartRegendPerTurn += _breastplate.BarChartRegendPerTurn;
        }
        if (_gardebras)
        {
            this.RoleBasicRA += _gardebras.RoleBasicRA.Clone;
            this.CRIDmg += _gardebras.CRIDmg;
            this.DmgReduction += _gardebras.DmgReduction;
            this.DmgReflection += _gardebras.DmgReflection;
            this.RewardRate += _gardebras.RewardRate;
            this.BarChartRegendPerTurn += _gardebras.BarChartRegendPerTurn;
        }
        if (_legging)
        {
            this.RoleBasicRA += _legging.RoleBasicRA.Clone;
            this.CRIDmg += _legging.CRIDmg;
            this.DmgReduction += _legging.DmgReduction;
            this.DmgReflection += _legging.DmgReflection;
            this.RewardRate += _legging.RewardRate;
            this.BarChartRegendPerTurn += _legging.BarChartRegendPerTurn;
        }
        if (_jewelry0)
        {
            this.RoleBasicRA += _jewelry0.RoleBasicRA.Clone;
            this.CRIDmg += _jewelry0.CRIDmg;
            this.DmgReduction += _jewelry0.DmgReduction;
            this.DmgReflection += _jewelry0.DmgReflection;
            this.RewardRate += _jewelry0.RewardRate;
            this.BarChartRegendPerTurn += _jewelry0.BarChartRegendPerTurn;
        }
        if (_jewelry1)
        {
            this.RoleBasicRA += _jewelry1.RoleBasicRA.Clone;
            this.CRIDmg += _jewelry1.CRIDmg;
            this.DmgReduction += _jewelry1.DmgReduction;
            this.DmgReflection += _jewelry1.DmgReflection;
            this.RewardRate += _jewelry1.RewardRate;
            this.BarChartRegendPerTurn += _jewelry1.BarChartRegendPerTurn;
        }
        if (_weapon)
        {
            this.RoleBasicRA += _weapon.RoleBasicRA.Clone;
            this.CRIDmg += _weapon.CRIDmg;
            this.DmgReduction += _weapon.DmgReduction;
            this.DmgReflection += _weapon.DmgReflection;
            this.RewardRate += _weapon.RewardRate;
            this.BarChartRegendPerTurn += _weapon.BarChartRegendPerTurn;
        }
        //

        initRoleClassData();
    }

    public override void initRoleClassData()
    {
        base.initRoleClassData();
        GDEHeroData D = SDDataManager.Instance.getHeroByHashcode(Hashcode);
        IsLocked = D.locked;
    }
    #endregion
    public void Btn_ChangeLocked()
    {
        GDEHeroData D = SDDataManager.Instance.getHeroByHashcode(Hashcode);
        bool flag = !IsLocked;
        //
        if(flag != D.locked)
        {
            D.locked = flag;
            IsLocked = D.locked;
        }
    }
    #region 读取角色技能
    public void readHeroSkills()
    {
        for (int i = 0; i < SkillSlots.Length; i++)
        {
            SkillSlots[i].initSkillSlot(Hashcode);
        }
        GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(Hashcode);
        //HeroInfo info = SDDataManager.Instance.getHeroInfoById(ID);
        skillid0 = hero.a_skill0.Id;
        if(hero.a_skill1!=null) skillid1 = hero.a_skill1.Id;
        skillidOmega = hero.a_skillOmega.Id;

    }
    #endregion
    public void setRoleBaseMessiage()
    {
        NameText.text = Name;
        StarNumVision.StarNum = LEVEL;
        //
        BattleForce = RoleBasicRA.Hp
            + RoleBasicRA.AT * 5
            + RoleBasicRA.MT * 5;
        battleforceText.text = "" + BattleForce;
    }
}
