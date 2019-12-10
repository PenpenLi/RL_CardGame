using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

/// <summary>
/// 角色详情页面
/// </summary>
public class SDHeroDetail : BasicRoleProperty
{
    public SDConstants.CharacterType Type = SDConstants.CharacterType.Hero;
    //public string Id;
    public int Hashcode;
    public int Status;
    public int careerIndex;
    public int raceIndex;

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

    //public Image heroFrameImg;
    //public Image heroStatusImg;

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
    [Header("技能配置 栏")]
    public List<OneSkill> EquipedSkills;
    public List<OneSkill> OwnedSkills;
    public List<OneSkill> AllSkills;
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
            EquipedSkills = new List<OneSkill>();
            setHero(Hashcode);
            setHelmet(Hashcode);
            setBreastplate(Hashcode);
            // setGardebras(Hashcode);//已过期
            setLegging(Hashcode);
            setJewelry(Hashcode , false);
            setJewelry(Hashcode , true);
            setWeapon(Hashcode);
            InitHeroBasicProperties();
            RALPanel.initRAL(this.RoleBasicRA,this.RARate, Type
                , SDDataManager.Instance.getLevelByExp(hero.exp));//视觉展示属性
            setRoleBaseMessiage();

            if (LvText)
            {
                int exp = hero.exp;
                int lv = SDDataManager.Instance.getLevelByExp(exp);
                LvText.text = SDGameManager.T("Lv.") + lv;
                e0 = exp - SDDataManager.Instance.getExpByLevel(lv);
                e1 = (lv + 1) * SDConstants.MinExpPerLevel;
                if(ExpSlider)
                ExpSlider.localScale 
                        = Vector3.up + Vector3.forward + Vector3.right * (e0 * 1f / e1);

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
        CareerText.text = "" + SDDataManager.Instance.getCareerStr(careerIndex, raceIndex, Type);
        //CareerIconImg.sprite = 
        RaceText.text = "" + SDDataManager.Instance.getRaceStr(raceIndex, Type);
        RarityText.text = SDDataManager.Instance.rarityString(dal.quality);
        //RaceIconImg.sprite =
        int grade = SDDataManager.Instance.getLevelByExp(heroData.exp);
        _hero.initData_Hero((Job)careerIndex, (Race)raceIndex, grade, dal.quality, dal.starNum
            , dal.BasicRAL, dal.RALRate
            , dal.CRI, dal.CRIDmg, dal.DmgReduction, dal.DmgReflection, dal.RewardRate
            , dal.BarChartRegendPerTurn, ID, dal.Name, heroData.wakeNum);

        //print("name is " + name);
    }
    public void setHelmet(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipHelmet(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            equipList.initPosEquipVisionEmpty(EquipPosition.Head);
            _helmet.initDataEmpty(); return;
        }
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("equip");
        for (int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if (s["id"] == armor.id)
            {
                string id = s["id"];
                int rarity = SDDataManager.Instance.getEquipRarityById(id);
                equipList.helmetD.initEquipVision("Sprites/EquipImage/" + s["image"], rarity);
                //
                int level = SDDataManager.Instance.getLevelByExp(armor.exp);
                int upLv = SDDataManager.Instance.getLevelByExp(armor.exp);
                RoleAttributeList basicRAL = SDDataManager.Instance.RALByDictionary(s);
                basicRAL = SDDataManager.Instance.getRALByUpLv(basicRAL, upLv);
                string name = s["name"];

                string passiveEffect = s["passiveEffect"];
                RoleAttributeList rateRAL = new RoleAttributeList();
                _helmet.initData(level, basicRAL, rateRAL, 0, 0, 0, 0, 0
                    , new RoleBarChart(), id, name, 0);
                _helmet.PassiveEffectInit(passiveEffect);
                _helmet.armorType = (SDConstants.ArmorType)
                    SDDataManager.Instance.getInteger(s["type"]);
                break;
            }
        }
    }
    public void setBreastplate(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipBreastplate(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            equipList.initPosEquipVisionEmpty(EquipPosition.Breast);
            _breastplate.initDataEmpty(); return;
        }
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("equip");
        for (int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if (s["id"] == armor.id)
            {
                string id = s["id"];
                int rarity = SDDataManager.Instance.getEquipRarityById(id);
                equipList.breastplateD.initEquipVision("Sprites/EquipImage/" + s["image"], rarity);
                //
                int level = SDDataManager.Instance.getLevelByExp(armor.exp);
                int upLv = SDDataManager.Instance.getLevelByExp(armor.exp);
                RoleAttributeList basicRAL = SDDataManager.Instance.RALByDictionary(s);
                basicRAL = SDDataManager.Instance.getRALByUpLv(basicRAL, upLv);
                string name = s["name"];
                string passiveEffect = s["passiveEffect"];
                RoleAttributeList rateRAL = new RoleAttributeList();
                _breastplate.initData(level, basicRAL, rateRAL, 0, 0, 0, 0, 0
                    , new RoleBarChart(), id, name, 0);
                _breastplate.PassiveEffectInit(passiveEffect);
                _breastplate.armorType = (SDConstants.ArmorType)
                    SDDataManager.Instance.getInteger(s["type"]);
                break;
            }
        }
    }
    public void setGardebras(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipGardebras(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            equipList.initPosEquipVisionEmpty(EquipPosition.Arm);
            _gardebras.initDataEmpty(); return;
        }
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("equip");
        for (int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if (s["id"] == armor.id)
            {
                string id = s["id"];
                int rarity = SDDataManager.Instance.getEquipRarityById(id);
                equipList.gardebrasD.initEquipVision("Sprites/EquipImage/" + s["image"],rarity);
                int level = SDDataManager.Instance.getLevelByExp(armor.exp);
                int upLv = SDDataManager.Instance.getLevelByExp(armor.exp);
                RoleAttributeList basicRAL = SDDataManager.Instance.RALByDictionary(s);
                basicRAL = SDDataManager.Instance.getRALByUpLv(basicRAL, upLv);
                string name = s["name"];
                string passiveEffect = s["passiveEffect"];
                RoleAttributeList rateRAL = new RoleAttributeList();
                _gardebras.initData(level, basicRAL, rateRAL, 0, 0, 0, 0, 0, RoleBarChart.zero
                    , id, name, 0);
                _gardebras.PassiveEffectInit(passiveEffect);
                _gardebras.armorType = (SDConstants.ArmorType)
                    SDDataManager.Instance.getInteger(s["type"]);
                break;
            }
        }
    }
    public void setLegging(int hashcode)
    {
        GDEEquipmentData armor = SDDataManager.Instance.getHeroEquipLegging(hashcode);
        if (armor == null || string.IsNullOrEmpty(armor.id))
        {
            equipList.initPosEquipVisionEmpty(EquipPosition.Leg);
            _legging.initDataEmpty(); return;
        }
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("equip");
        for (int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if (s["id"] == armor.id)
            {
                string id = s["id"];
                int rarity = SDDataManager.Instance.getEquipRarityById(id);
                equipList.leggingD.initEquipVision("Sprites/EquipImage/" + s["image"],rarity);

                int level = SDDataManager.Instance.getLevelByExp(armor.exp);
                int upLv = SDDataManager.Instance.getLevelByExp(armor.exp);
                RoleAttributeList basicRAL = SDDataManager.Instance.RALByDictionary(s);
                basicRAL = SDDataManager.Instance.getRALByUpLv(basicRAL, upLv);
                string name = s["name"];
                string passiveEffect = s["passiveEffect"];
                RoleAttributeList rateRAL = new RoleAttributeList();
                _legging.initData(level, basicRAL, rateRAL, 0, 0, 0, 0, 0, RoleBarChart.zero
                    , id, name, 0);
                _legging.PassiveEffectInit(passiveEffect);
                _legging.armorType = (SDConstants.ArmorType)
                    SDDataManager.Instance.getInteger(s["type"]);
                break;
            }
        }
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
            List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("jewelry");
            for (int i = 0; i < itemDatas.Count; i++)
            {
                Dictionary<string, string> s = itemDatas[i];
                if (s["id"] == armor.id)
                {
                    string id = s["id"];
                    int rarity = SDDataManager.Instance.getEquipRarityById(id);
                    equipList.jewelry0D.initEquipVision("Sprites/EquipImage/" + s["image"],rarity);

                    int level = SDDataManager.Instance.getLevelByExp(armor.exp);
                    int upLv = SDDataManager.Instance.getLevelByExp(armor.exp);
                    //主副属性类型装备读取
                    RoleAttributeList basicRAL = SDDataManager.Instance.EquipRALByDictionary(s);
                    basicRAL = SDDataManager.Instance.getRALByUpLv(basicRAL, upLv);
                    string name = s["name"];
                    string passiveEffect = s["passiveEffect"];
                    RoleAttributeList rateRAL = new RoleAttributeList();
                    _jewelry0.initData(level, basicRAL, rateRAL, 0, 0, 0, 0, 0
                        , new RoleBarChart(), id, name, 0);
                    _jewelry0.PassiveEffectInit(passiveEffect);
                    _jewelry0._jewelryType = (SDConstants.JewelryType)
                        SDDataManager.Instance.getInteger(s["type"]);
                    break;
                }
            }
        }
        else
        {
            if (armor == null ||string.IsNullOrEmpty(armor.id))
            {
                equipList.initPosEquipVisionEmpty(EquipPosition.Finger, true);
                _jewelry1.initDataEmpty(); return;
            }
            List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("jewelry");
            for (int i = 0; i < itemDatas.Count; i++)
            {
                Dictionary<string, string> s = itemDatas[i];
                if (s["id"] == armor.id)
                {
                    string id = s["id"];
                    int rarity = SDDataManager.Instance.getEquipRarityById(id);
                    equipList.jewelry1D.initEquipVision("Sprites/EquipImage/" + s["image"],rarity);

                    int level = SDDataManager.Instance.getLevelByExp(armor.exp);
                    int upLv = SDDataManager.Instance.getLevelByExp(armor.exp);
                    //主副属性类型装备读取
                    RoleAttributeList basicRAL = SDDataManager.Instance.EquipRALByDictionary(s);
                    basicRAL = SDDataManager.Instance.getRALByUpLv(basicRAL, upLv);
                    string name = s["name"];
                    string passiveEffect = s["passiveEffect"];
                    RoleAttributeList rateRAL = new RoleAttributeList();
                    _jewelry1.initData(level, basicRAL, rateRAL, 0, 0, 0, 0, 0, RoleBarChart.zero
                        , id, name, 0);
                    _jewelry1.PassiveEffectInit(passiveEffect);
                    _jewelry1._jewelryType = (SDConstants.JewelryType)
                        SDDataManager.Instance.getInteger(s["type"]);
                    break;
                }
            }
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
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("weapon");
        for (int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if (s["id"] == armor.id)
            {
                string id = s["id"];
                int rarity = SDDataManager.Instance.getEquipRarityById(id);
                equipList.weaponD.initEquipVision("Sprites/EquipImage/" + s["image"],rarity);

                int level = SDDataManager.Instance.getLevelByExp(armor.exp);
                int upLv = SDDataManager.Instance.getLevelByExp(armor.exp);
                //主副属性类型装备读取
                RoleAttributeList basicRAL = SDDataManager.Instance.EquipRALByDictionary(s);
                basicRAL = SDDataManager.Instance.getRALByUpLv(basicRAL, upLv);
                string name = s["name"];
                string passiveEffect = s["passiveEffect"];
                RoleAttributeList rateRAL = new RoleAttributeList();
                _weapon.initData(level, basicRAL, rateRAL, 0, 0, 0, 0, 0
                    , new RoleBarChart(), id, name, 0);
                _weapon.PassiveEffectInit(passiveEffect);
                _weapon._weaponType = (SDConstants.WeaponType)
                    SDDataManager.Instance.getInteger(s["type"]);
                break;
            }
        }
    }

    public void InitHeroBasicProperties()
    {
        if (_hero)
        {
            this.RoleBasicRA = _hero.RoleBasicRA;
            this.RARate = _hero.RARate;
            this.CRI = _hero.CRI;
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
            this.RoleBasicRA += _helmet.RoleBasicRA;
            this.RARate += _helmet.RARate;
            this.CRI += _helmet.CRI;
            this.CRIDmg += _helmet.CRIDmg;
            this.DmgReduction += _helmet.DmgReduction;
            this.DmgReflection += _helmet.DmgReflection;
            this.RewardRate += _helmet.RewardRate;
            this.BarChartRegendPerTurn += _helmet.BarChartRegendPerTurn;
        }
        if (_breastplate)
        {
            this.RoleBasicRA += _breastplate.RoleBasicRA;
            this.RARate += _breastplate.RARate;
            this.CRI += _breastplate.CRI;
            this.CRIDmg += _breastplate.CRIDmg;
            this.DmgReduction += _breastplate.DmgReduction;
            this.DmgReflection += _breastplate.DmgReflection;
            this.RewardRate += _breastplate.RewardRate;
            this.BarChartRegendPerTurn += _breastplate.BarChartRegendPerTurn;
        }
        if (_gardebras)
        {
            this.RoleBasicRA += _gardebras.RoleBasicRA;
            this.RARate += _gardebras.RARate;
            this.CRI += _gardebras.CRI;
            this.CRIDmg += _gardebras.CRIDmg;
            this.DmgReduction += _gardebras.DmgReduction;
            this.DmgReflection += _gardebras.DmgReflection;
            this.RewardRate += _gardebras.RewardRate;
            this.BarChartRegendPerTurn += _gardebras.BarChartRegendPerTurn;
        }
        if (_legging)
        {
            this.RoleBasicRA += _legging.RoleBasicRA;
            this.RARate += _legging.RARate;
            this.CRI += _legging.CRI;
            this.CRIDmg += _legging.CRIDmg;
            this.DmgReduction += _legging.DmgReduction;
            this.DmgReflection += _legging.DmgReflection;
            this.RewardRate += _legging.RewardRate;
            this.BarChartRegendPerTurn += _legging.BarChartRegendPerTurn;
        }
        if (_jewelry0)
        {
            this.RoleBasicRA += _jewelry0.RoleBasicRA;
            this.RARate += _jewelry0.RARate;
            this.CRI += _jewelry0.CRI;
            this.CRIDmg += _jewelry0.CRIDmg;
            this.DmgReduction += _jewelry0.DmgReduction;
            this.DmgReflection += _jewelry0.DmgReflection;
            this.RewardRate += _jewelry0.RewardRate;
            this.BarChartRegendPerTurn += _jewelry0.BarChartRegendPerTurn;
        }
        if (_jewelry1)
        {
            this.RoleBasicRA += _jewelry1.RoleBasicRA;
            this.RARate += _jewelry1.RARate;
            this.CRI += _jewelry1.CRI;
            this.CRIDmg += _jewelry1.CRIDmg;
            this.DmgReduction += _jewelry1.DmgReduction;
            this.DmgReflection += _jewelry1.DmgReflection;
            this.RewardRate += _jewelry1.RewardRate;
            this.BarChartRegendPerTurn += _jewelry1.BarChartRegendPerTurn;
        }
        if (_weapon)
        {
            this.RoleBasicRA += _weapon.RoleBasicRA;
            this.RARate += _weapon.RARate;
            this.CRI += _weapon.CRI;
            this.CRIDmg += _weapon.CRIDmg;
            this.DmgReduction += _weapon.DmgReduction;
            this.DmgReflection += _weapon.DmgReflection;
            this.RewardRate += _weapon.RewardRate;
            this.BarChartRegendPerTurn += _weapon.BarChartRegendPerTurn;
        }
        //

        //initRoleClassData();
    }
    #endregion
    public void setRoleBaseMessiage()
    {
        NameText.text = Name;
        StarNumVision.StarNum = LEVEL;
    }
}
