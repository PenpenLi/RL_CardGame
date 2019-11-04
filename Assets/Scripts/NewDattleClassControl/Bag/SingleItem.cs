using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using System;

/// <summary>
/// 通用武器选择项
/// </summary>
public class SingleItem : MonoBehaviour
{
    public Image itemImg;
    public CharacterModelController characterModel;
    public Image frameImg;
    public Image careerImg;
    public Image statusImg;
    public Text levelText;
    public Text numText;
    public Text nameText;
    public Text fightForceText;
    public SDConstants.ItemType type;
    public int itemId;
    public int itemExp = 0;
    public int itemHashcode;
    public int itemNum;
    public int itemLevel;
    public int itemUpLv;
    public int hireBtnIndex;
    public ItemStarVision starVision;
    public Transform slider;

    bool _isSelected = false;
    public bool isSelected 
    {
        get { return _isSelected; }
        set 
        {
            _isSelected = value;
            SelectedPanel?.gameObject.SetActive(_isSelected);
        }
    }
    bool _isUnable = false;
    public bool isUnable
    {
        get { return _isUnable; }
        set 
        {
            _isUnable = value;
            UnablePanel?.gameObject.SetActive(_isUnable);
        }
    }
    [Space(25)]
    //public Transform HeroPanel;
    public Transform EmptyPanel;
    public Transform SelectedPanel;
    public Transform UnablePanel;

    public RTEquipUpTimes equipUpTimes;
    public RTBagItemDetail bagItemDetail;
    public HEWPageController sourceController;

    public void equipDetailBtnTapped()
    {
        bagItemDetail.equipType = type;
        bagItemDetail.equipItem = this;
        bagItemDetail.initEquipItemDetail();
        bagItemDetail.transform.localScale = Vector3.one;
        //UIEffectManager
    }
    public void BtnTapped()
    {
        if(type == SDConstants.ItemType.Hero)
        {
            if (SDGameManager.Instance.isSelectHero)
            {
                Debug.Log("当前类别" + SDGameManager.Instance.heroSelectType);
                if(SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.Battle)
                {
                    chooseHeroToBattle();
                }
                else if(SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.Dispatch)
                {
                    chooseHeroToDispatch();
                }
                else if (SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.TrainConsume)
                {
                    chooseHeroToTrainConsume();
                }
                else if (SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.PromoteConsume)
                {
                    chooseHeroToPromoteConsume();
                }
                else if (SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.Train)
                {
                    chooseHeroToTrain();
                }
                else if (SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.StarUp)
                {
                    chooseHeroToStarUp();
                }
                else if (SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.Wake)
                {
                    chooseHeroToWake();
                }
                else if (SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.Replace)
                {
                    chooseHeroToReplace();
                }
                else if (SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.Promote)
                {
                    chooseHeroToPromote();
                }
                else if(SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.Hospital)
                {
                    chooseHeroToTreat();
                }
                else if(SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.All)
                {
                    chooseHeroToDetail();
                }
            }
        }
        else if(type == SDConstants.ItemType.Helmet||type == SDConstants.ItemType.Breastplate
            || type == SDConstants.ItemType.Gardebras || type == SDConstants.ItemType.Legging
            || type == SDConstants.ItemType.Weapon || type == SDConstants.ItemType.Jewelry)
        {
            if (!SDGameManager.Instance.isStrengthenEquip)
            {

            }
            else
            {

            }

            chooseEquipmentToShowDetail();
        }
    }
    #region 目标为Hero
    #region Hero效果
    public void chooseHeroToBattle()
    {
        if(SDDataManager.Instance.getHeroStatus(itemHashcode) == 0)
        {
            isSelected = true;
        }
        else
        {
            SDDataManager.Instance.removeFromTeam(itemHashcode);
            isSelected = false;
        }
        BasicHeroSelect BHS = GetComponentInParent<BasicHeroSelect>();
        if (BHS)
        {
            SelectTeamUnitPanel STUP = GetComponentInParent<SelectTeamUnitPanel>();
            SDDataManager.Instance.setHeroTeam(STUP.CurrentTeamId
                , STUP.currentHeroIndexInTeam
                , itemHashcode) ;

            BHS.heroSelectPanel.heroPanelInit();
            //BHS.closeBtnTapped();
        }
    }
    public void chooseHeroToDispatch()
    {

    }
    public void chooseHeroToTrainConsume()
    {

    }
    public void chooseHeroToPromoteConsume()
    {

    }
    public void chooseHeroToTrain()
    {

    }
    public void chooseHeroToStarUp()
    {

    }
    public void chooseHeroToWake()
    {

    }
    public void chooseHeroToReplace()
    {

    }
    public void chooseHeroToPromote()
    {

    }
    public void chooseHeroToDetail()
    {
        BasicHeroSelect BHS = GetComponentInParent<BasicHeroSelect>();
        if (BHS)
        {
            //显示英雄详情
            SDHeroDetail HDP = BHS.heroDetails.GetComponent<SDHeroDetail>();
            HDP.initHeroDetailPanel(itemHashcode);
            HDP.HeroWholeMessage.OpenThisPanel();
        }
    }
    public void chooseHeroToTreat()
    {
        HospitalPanel hp = GetComponentInParent<HospitalPanel>();
        if (SDDataManager.Instance.getHeroStatus(itemHashcode) == 2
            && hp.SickBed(hp.currentSickBedId).isEmpty)
        {
            SDDataManager.Instance.setHeroStatus(itemHashcode, 3);

            if (hp)
            {
                GDEtimeTaskData task = new GDEtimeTaskData(GDEItemKeys.timeTask_emptyTimeTask)
                {
                    taskId = hp.currentSickBedId
                    ,
                    isFinished = false
                    ,
                    itemChara = itemHashcode
                    ,
                    taskType = 0
                    ,
                    startTime = DateTime.Now.ToString()
                };
                int quality = SDDataManager.Instance.getHeroQualityById(itemId);
                int starNum = SDDataManager.Instance.getHeroStarNumByHashcode(itemHashcode);
                task.timeType = quality + starNum + 1;

                //
                SDDataManager.Instance.PlayerData.TimeTaskList.Add(task);
                SDDataManager.Instance.PlayerData.Set_TimeTaskList();
            }
        }
        else
        {

        }

    }
    #endregion

    /// <summary>
    /// 招募按钮
    /// </summary>
    public void hireBtnTapped()
    {
        if (SDGameManager.Instance.isHireHero)
        {

        }
    }


    /// <summary>
    /// 初始化英雄出战选人页
    /// </summary>
    /// <param name="hero"></param>
    public void initBattleHero(GDEHeroData hero)
    {
        type = SDConstants.ItemType.Hero;
        itemId = hero.id;
        itemHashcode = hero.hashCode;
        //

        //
        ROHeroData dal = SDDataManager.Instance.getHeroDataByID(itemId, hero.starNumUpgradeTimes);
        //if (frameImg != null) frameImg.gameObject.SetActive(false);

        levelText.gameObject.SetActive(true);
        levelText.text = "Lv." + SDDataManager.Instance.getLevelByExp(hero.exp);
        nameText.text = SDGameManager.T(dal.Name);
        //
        int status = SDDataManager.Instance.getHeroStatus(hero.hashCode);
        if (status == 0)
        {
            isSelected = false;
            isUnable = false;
        }//无业
        else if(status == 1)
        {
            isSelected = true;
            isUnable = false;
        }//战斗队伍中
        else
        {
            isSelected = false;
            isUnable = true;
        }//其他状态
        //statusImg.gameObject.SetActive(true);
        //statusImg.sprite = herostat
        starVision.StarNum = dal.starNum;


    }
    public void initInjuriedHero(GDEHeroData hero)
    {
        type = SDConstants.ItemType.Hero;
        itemId = hero.id;
        itemHashcode = hero.hashCode;
        //

        //
        ROHeroData dal = SDDataManager.Instance.getHeroDataByID(itemId, hero.starNumUpgradeTimes);
        //if (frameImg != null) frameImg.gameObject.SetActive(false);

        levelText.gameObject.SetActive(true);
        levelText.text = "Lv." + SDDataManager.Instance.getLevelByExp(hero.exp);
        nameText.text = SDGameManager.T(dal.Name);
        //
        int status = SDDataManager.Instance.getHeroStatus(hero.hashCode);
        if (status == 2)
        {
            isSelected = false;
            isUnable = false;
        }//受伤且未进行治疗状态
        else
        {
            isSelected = false;
            isUnable = true;
        }//其他状态
        //starVision.StarNum = dal.starNum;
        starVision.gameObject.SetActive(false);

    }
    public void initHero(GDEHeroData hero)
    {
        type = SDConstants.ItemType.Hero;
        itemId = hero.id;
        itemHashcode = hero.hashCode;
        if (fightForceText) fightForceText.text 
                = ""+(SDDataManager.Instance.getBattleForceByHashCode(itemHashcode));//读取角色战斗力
        SDConstants.CharacterAnimType animType = (SDConstants.CharacterAnimType)
            (SDDataManager.Instance.getHeroCareerById(itemId));
        ROHeroData roh = SDDataManager.Instance.getHeroDataByID(itemId,hero.starNumUpgradeTimes);
        //if (frameImg != null) frameImg.gameObject.SetActive(false);
        characterModel?.initCharacterModel(itemHashcode,animType,SDConstants.HERO_MODEL_RATIO);
        itemLevel = SDDataManager.Instance.getLevelByExp(hero.exp);
        levelText.gameObject.SetActive(true);
        levelText.text = SDGameManager.T("Lv.") + itemLevel;
        nameText.text = SDGameManager.T(roh.Name);

        starVision.StarNum = roh.starNum;
        if (statusImg)
        {
            statusImg.gameObject.SetActive(true);
            statusImg.sprite = UIEffectManager.Instance.heroStatusSps[hero.status];
        }
        if (numText) numText.gameObject.SetActive(false);
    }
    public void initHero(int hashcode)
    {
        GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(hashcode);
        initHero(hero);
    }
    public string getStatusText(int status)
    {
        string s = "";
        if (status == 0) s = "空闲中";
        else if (status == 1) s = "出战中";

        return s;
    }

    public void initCalledHero(int hashcode)
    {
        initHero(hashcode);
        levelText.text = "";
    }

    public void HeroInfoBtnTapped()
    {

    }
    #endregion





    #region 目标为Equip
    #region equip效果
    public void chooseEquipmentToShowDetail()
    {
        SDEquipSelect ES = GetComponentInParent<SDEquipSelect>();
        if (ES)
        {
            //显示装备详情
            ES.refreshSelectedEquipmentDetail(itemHashcode);
        }
    }
    #endregion
    public void initEquip(int id,int upLv)
    {
        int equipPos = SDDataManager.Instance.getEquipPosById(id);
        type = (SDConstants.ItemType)(equipPos + 1);
        List<Dictionary<string, string>> itemsData = SDDataManager.Instance.ReadFromCSV("equip");
        if (equipPos == 4)
        {
            itemsData = SDDataManager.Instance.ReadFromCSV("jewelry");            
        }
        else if(equipPos == 5)
        {
            itemsData = SDDataManager.Instance.ReadDataFromCSV("weapon");
        }
        for (int i = 0; i < itemsData.Count; i++)
        {
            Dictionary<string, string> s = itemsData[i];
            if (s["id"] == id.ToString())
            {
                itemId = id;
                //itemImg.sprite = Resources.Load<Sprite>("Sprites/EquipImage/" + s["image"]);
                itemUpLv = upLv;
                //equipUpTimes
                if (nameText) nameText.text = SDGameManager.T(s["name"]);
                levelText.gameObject.SetActive(true);
                levelText.text = SDGameManager.T("Lv.") + (itemLevel);
                int rarity = SDDataManager.Instance.getInteger(s["rarity"]);
                //frameImg.sprite = 
                if (numText != null)
                {
                    numText.text = "X" + itemNum;
                    if (itemNum == 1) numText.text = "";
                }
                break;
            }
        }
    }
    public void initEquip(GDEEquipmentData equip)
    {
        int equipPos = SDDataManager.Instance.getEquipPosById(equip.equipId);
        type = (SDConstants.ItemType)(equipPos + 1);
        itemId = equip.equipId;
        itemHashcode = equip.hashcode;
        if (fightForceText) fightForceText.text
                = "" + (SDDataManager.Instance.getEquipBattleForceByHashCode(itemHashcode));//读取装备战斗力
        itemUpLv = equip.upLv;
        if (nameText) nameText.text = SDDataManager.Instance.getEquipNameByHashcode(itemHashcode);
        if (levelText)
        {
            levelText.gameObject.SetActive(true);
            levelText.text = SDGameManager.T("Lv.") + (equip.equipLv + equip.upLv);
        }
        if (numText) numText.gameObject.SetActive(false);

        int rarity = SDDataManager.Instance.getEquipRarityById(itemId);
        //frameImg;
    }
    #endregion


}
