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

    public SDHero _hero;
    public SDHeroSelect heroSelect;
    public BasicHeroSelect BHS;
    #region 栏No.1
    [Header("立绘和角色详细信息 栏")]
    public Image heroCharacterDrawingImg;
    public Image CareerIconImg;
    public Text CareerText;
    public Image RaceIconImg;
    public Text RaceText;
    public Text NameText;
    public Text NameBeforeText;
    public Text RarityText;
    public ItemStarVision StarNumVision;
    public Text LvText;
    public Transform ExpSlider;
    [HideInInspector]
    public int e0;
    [HideInInspector]
    public int e1;
    public AttritubeListPanel RALPanel;
    public Transform RoleParticularPanel;
    #endregion
    #region 栏No.2
    [Header("装备和小人动画 栏")]
    public CharacterModelController heroHeadImg;
    public HeroEquipList equipList;
    Helmet _helmet { get { return equipList._helmet; } }
    Breastplate _breastplate { get { return equipList._breastplate; } }
    Gardebras _gardebras { get { return equipList._gardebras; } }
    Legging _legging { get { return equipList._legging; } }
    Jewelry _jewelry0 { get { return equipList._jewelry0; } }
    Jewelry _jewelry1 { get { return equipList._jewelry1; } }
    SDWeapon _weapon { get { return equipList._weapon; } }
    //public Transform roleModelPanel;
    public Transform ModelAndEquipsPanel;
    //
    #endregion
    #region 栏No.3
    public string skillid0;
    public string skillid1;
    public string skillidOmega;
    #endregion
    #region 总栏
    [Header("上级信息 栏")]
    public HeroDetailPanel HeroWholeMessage;
    #endregion

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
                , _role.extraRALChangeData
                , Type
                , SDDataManager.Instance.getLevelByExp(hero.exp));//视觉展示属性
            setRoleBaseMessiage();

            if (LvText)
            {
                int exp = hero.exp;
                int lv = SDDataManager.Instance.getLevelByExp(exp);
                LvText.text = SDGameManager.T("Lv.") + lv;
                if(ExpSlider)
                ExpSlider.localScale 
                        = Vector3.up + Vector3.forward + Vector3.right * (SDDataManager.Instance.getExpRateByExp(exp));

            }
            showRoleModelPanel();
            //equipedSkills

            HeroWholeMessage.readHeroEquipedSkills(hashcode);
        }
    }

    public void showRoleModelPanel()
    {
        if(ModelAndEquipsPanel.parent != this)
        {
            ModelAndEquipsPanel.SetParent(transform);
            ModelAndEquipsPanel.localScale = Vector3.one;
            ModelAndEquipsPanel.gameObject.SetActive(true);
        }
    }
    #region 读取角色属性
    public void setHero(int hashcode)
    {
        //if (_hero == null) _hero = equipList.gameObject.AddComponent<SDHero>();

        GDEHeroData heroData = SDDataManager.Instance.GetHeroOwnedByHashcode(hashcode);
        string id = SDDataManager.Instance.getHeroIdByHashcode(hashcode);
        ID = id;
        careerIndex = SDDataManager.Instance.getHeroCareerById(id);
        raceIndex = SDDataManager.Instance.getHeroRaceById(id);

        ROHeroData dal = SDDataManager.Instance.getHeroDataByID(id, heroData.starNumUpgradeTimes);
        CareerText.text = "" + SDDataManager.Instance.getCareerStr(careerIndex, (int)raceIndex, Type);
        //CareerIconImg.sprite = 
        RaceText.text = "" + SDDataManager.Instance.getRaceStr((int)raceIndex, Type);
        RarityText.text = SDDataManager.Instance.rarityString(dal.quality);
        //RaceIconImg.sprite =
        int grade = SDDataManager.Instance.getLevelByExp(heroData.exp);
        _hero.gender = dal.Info.Sex;
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
            equipList.initPosEquipVisionEmpty(EquipPosition.Head);
            _helmet.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            _helmet.initDataEmpty(); return;
        }
        //
        _helmet.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        _helmet.PassiveEffectInit(Item.PassiveEffect);
        _helmet.armorRank = Item.ArmorRank;
    }
    public void setBreastplate(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipBreastplate(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            equipList.initPosEquipVisionEmpty(EquipPosition.Breast);
            _breastplate.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            _breastplate.initDataEmpty(); return;
        }
        //
        _breastplate.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        _breastplate.PassiveEffectInit(Item.PassiveEffect);
        _breastplate.armorRank = Item.ArmorRank;
    }
    public void setGardebras(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipGardebras(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            equipList.initPosEquipVisionEmpty(EquipPosition.Arm);
            _gardebras.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            _gardebras.initDataEmpty(); return;
        }
        //
        _gardebras.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        _gardebras.PassiveEffectInit(Item.PassiveEffect);
        _gardebras.armorRank = Item.ArmorRank;
    }
    public void setLegging(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipLegging(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            equipList.initPosEquipVisionEmpty(EquipPosition.Leg);
            _legging.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            _legging.initDataEmpty(); return;
        }
        //
        _legging.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        _legging.PassiveEffectInit(Item.PassiveEffect);
        _legging.armorRank = Item.ArmorRank;
    }
    public void setJewelry(int hashcode, bool isSecondPos = false)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipJewelry(hashcode, isSecondPos);
        if (!isSecondPos)
        {
            if (armor == null || string.IsNullOrEmpty(armor.id))
            {
                equipList.initPosEquipVisionEmpty(EquipPosition.Finger, false);
                _jewelry0.initDataEmpty(); return;
            }
            EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
            if (Item == null)
            {
                _jewelry0.initDataEmpty(); return;
            }
            //
            _jewelry0.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
                , Item.ID, Item.NAME, 0);
            _jewelry0.PassiveEffectInit(Item.PassiveEffect);
            _jewelry0.armorRank = Item.ArmorRank;
        }
        else
        {
            if (armor == null ||string.IsNullOrEmpty(armor.id))
            {
                equipList.initPosEquipVisionEmpty(EquipPosition.Finger, true);
                _jewelry1.initDataEmpty(); return;
            }
            EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
            if (Item == null)
            {
                _jewelry1.initDataEmpty(); return;
            }
            //
            _jewelry1.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
                , Item.ID, Item.NAME, 0);
            _jewelry1.PassiveEffectInit(Item.PassiveEffect);
            _jewelry1.armorRank = Item.ArmorRank;
        }
    }
    public void setWeapon(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroWeapon(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id) || armor.hashcode == 0)
        {
            equipList.initPosEquipVisionEmpty(EquipPosition.Hand);
            _weapon.initDataEmpty(); return;
        }
        EquipItem Item = SDDataManager.Instance.GetEquipItemById(armor.id);
        if (Item == null)
        {
            _weapon.initDataEmpty(); return;
        }
        //
        _weapon.initData(Item.LEVEL, Item.RAL, 0, 0, 0, 0, RoleBarChart.zero
            , Item.ID, Item.NAME, 0);
        _weapon.PassiveEffectInit(Item.PassiveEffect);
        _weapon.armorRank = Item.ArmorRank;
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

    }

    #endregion
    public void setRoleBaseMessiage()
    {
        NameText.text = Name;
        StarNumVision.StarNum = LEVEL;
    }
}
