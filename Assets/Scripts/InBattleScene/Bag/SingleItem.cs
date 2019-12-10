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
    public Image raceImg;
    public Image statusImg;
    public Text upText;
    public Text downText;
    public Text fightForceText;
    public SDConstants.ItemType type;
    public string itemId;
    public int itemExp = 0;
    public int itemHashcode;
    public int itemNum;
    public int useNum;
    public int itemLevel;
    public int itemUpLv;
    public int hireBtnIndex;
    public ItemStarVision starVision;
    public Transform slider;

    [HideInInspector]
    public int index;
    [HideInInspector]
    public bool extraTrigger;
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
    bool _isEmpty = false;
    public bool isEmpty
    {
        get { return _isEmpty; }
        set
        {
            if (_isEmpty != value)
            {
                _isEmpty = value;
                EmptyPanel?.gameObject.SetActive(_isEmpty);
            }
        }
    }
    [Space(25)]
    //public Transform HeroPanel;
    public Transform EmptyPanel;
    public Transform SelectedPanel;
    public Transform UnablePanel;
    [Space(15)]
    public HEWPageController sourceController;
    [Space(15)]
    public RTBagItemDetail bagItemDetail;
    public BagController bagController;
    public bool isUsedAsDropItem = false;
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
        if (!isUsedAsDropItem)
        {
            if (type == SDConstants.ItemType.Hero)
            {
                if (SDGameManager.Instance.isSelectHero)
                {
                    Debug.Log("当前类别" + SDGameManager.Instance.heroSelectType);
                    if (SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.Battle)
                    {
                        chooseHeroToBattle();
                    }
                    else if (SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.Dispatch)
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
                    else if (SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.Hospital)
                    {
                        chooseHeroToTreat();
                    }
                    else if (SDGameManager.Instance.heroSelectType == SDConstants.HeroSelectType.All)
                    {
                        chooseHeroToDetail();
                    }
                }
            }
            else if (type == SDConstants.ItemType.Equip)
            {
                if (!SDGameManager.Instance.isStrengthenEquip)
                {

                }
                else
                {

                }
                if(SDGameManager.Instance.stockUseType == SDConstants.StockUseType.detail)
                {
                    chooseEquipmentToShowDetail();
                }
                else if(SDGameManager.Instance.stockUseType == SDConstants.StockUseType.work)
                {
                    chooseEquipToWork();
                }
            }
            else if(type == SDConstants.ItemType.Goddess)
            {
                chooseGoddessToShowDetail();
            }
            else if(type == SDConstants.ItemType.Prop)
            {
                if(SDGameManager.Instance.stockUseType == SDConstants.StockUseType.detail)
                {
                    choosePropToShowDetail();
                }
                else if(SDGameManager.Instance.stockUseType == SDConstants.StockUseType.work)
                {
                    choosePropToChangeSlot();
                }
                else if(SDGameManager.Instance.stockUseType == SDConstants.StockUseType.sell)
                {
                    choosePropToSell();
                }
            }
            else if(type == SDConstants.ItemType.Material)
            {
                if (SDGameManager.Instance.stockUseType == SDConstants.StockUseType.detail)
                {
                    chooseMaterialToShowDetail();
                }
            }
            else if(type == SDConstants.ItemType.NPC)
            {
                chooseNPCToBuffAssemblyLine();
            }
        }
        else
        {

        }
    }
    #region 目标为Hero
    #region Hero效果
    public void chooseHeroToBattle()
    {
        BasicHeroSelect BHS = GetComponentInParent<BasicHeroSelect>();

        if (BHS)
        {
            SelectTeamUnitPanel STUP = GetComponentInParent<SelectTeamUnitPanel>();
            IEnumerator C = STUP.RPC.IEInitRoleModelToRolePosPlace();


            if (SDDataManager.Instance.getHeroStatus(itemHashcode) == 0)
            {
                isSelected = true;
            }
            else
            {
                SDDataManager.Instance.removeFromTeam(itemHashcode);
                isSelected = false;
            }

            List<GDEHeroData> all = SDDataManager.Instance.getHerosFromTeam(STUP.CurrentTeamId);
            //
            bool flag = false;
            if (isSelected && all.Exists(x=>x.TeamOrder == STUP.currentHeroIndexInTeam))
            {
                flag = true;
            }

            //
            SDDataManager.Instance.setHeroTeam(STUP.CurrentTeamId
                , STUP.currentHeroIndexInTeam
                , itemHashcode) ;
            //
            STUP.refreshUI();
            //
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
            HDP.HeroWholeMessage.whenOpenThisPanel();
        }
    }
    public void chooseHeroToTreat()
    {
        HospitalPanel hp = GetComponentInParent<HospitalPanel>();
        if (hp && hp.SickBed(hp.currentSickBedId).isEmpty)
        {
            SDDataManager.Instance.AddTimeTask(SDConstants.timeTaskType.HOSP
                , itemHashcode, itemId, hp.SickBed(hp.currentSickBedId).taskId);
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

        upText.gameObject.SetActive(true);
        upText.text = SDGameManager.T("Lv.") + SDDataManager.Instance.getLevelByExp(hero.exp);
        downText.text = SDGameManager.T(dal.Name);
        slider?.gameObject.SetActive(false);
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

        upText.gameObject.SetActive(true);
        upText.text = "Lv." + SDDataManager.Instance.getLevelByExp(hero.exp);
        downText.text = SDGameManager.T(dal.Name);
        slider?.gameObject.SetActive(true);
        //
        int status = SDDataManager.Instance.getHeroStatus(hero.hashCode);
        if (status == 2)
        {
            isSelected = false;
            isUnable = false;
        }//受伤且未进行治疗状态
        else if(status == 3)
        {
            isSelected = false;
            isUnable = true;
        }//已经在治疗状态
        else if(status == 0)
        {
            isSelected = false;
            isUnable = false;
        }//无事可做状态
        else
        {
            isSelected = false;
            isUnable = true;
        }
        //starVision.StarNum = dal.starNum;
        starVision.gameObject.SetActive(false);
        slider.GetChild(0).localScale 
            = new Vector3(1 - SDDataManager.Instance.getHeroFatigueRate(hero.hashCode), 1, 1);
    }
    public void initHero(GDEHeroData hero)
    {
        type = SDConstants.ItemType.Hero;
        itemId = hero.id;
        itemHashcode = hero.hashCode;
        if (fightForceText) fightForceText.text 
                = ""+(SDDataManager.Instance.getHeroOriginalBattleForceByHashCode(itemHashcode));//读取角色战斗力
        SDConstants.CharacterAnimType animType = (SDConstants.CharacterAnimType)
            (SDDataManager.Instance.getHeroCareerById(itemId));
        ROHeroData roh = SDDataManager.Instance.getHeroDataByID(itemId,hero.starNumUpgradeTimes);
        //if (frameImg != null) frameImg.gameObject.SetActive(false);
        characterModel?.initCharacterModel(itemHashcode,animType,SDConstants.HERO_MODEL_BIG_RATIO);
        itemLevel = SDDataManager.Instance.getLevelByExp(hero.exp);
        if (upText)
        {
            upText.gameObject.SetActive(true);
            upText.text = SDGameManager.T("Lv.") + itemLevel;
        }
        if (downText)
        {
            downText.text = SDGameManager.T(roh.Name);
        }
        slider?.gameObject.SetActive(false);
        if(starVision) starVision.StarNum = roh.starNum;
        if (statusImg)
        {
            statusImg.gameObject.SetActive(true);
            statusImg.sprite = UIEffectManager.Instance.heroStatusSps[hero.status];
        }
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
        else if (status == 2) s = "怯战";
        else if (status == 3) s = "反省中";
        return s;
    }

    public void initCalledHero(int hashcode)
    {
        initHero(hashcode);
        if(upText)
            upText.text = "";


    }

    public void HeroInfoBtnTapped()
    {

    }
    #endregion
    #region 目标为Equip
    #region equip效果
    public void chooseEquipmentToShowDetail()
    {
        EquipmentPanel EP = GetComponentInParent<EquipmentPanel>();
        if (EP)
        {
            EP.homeScene.equipDetailBtnTapped();
            EP.homeScene._equipDetailPanel
                .GetComponentInChildren<SDEquipDetail>()
                .initEquipDetailVision
                (SDDataManager.Instance.getEquipmentByHashcode(itemHashcode));
            sourceController.Select_None();
            isSelected = true;
        }
    }
    public void chooseEquipToWork()
    {
        SDEquipSelect ES = GetComponentInParent<SDEquipSelect>();
        if (ES)
        {
            //显示装备详情
            ES.refreshSelectedEquipmentDetail(itemHashcode);
        }
    }
    #endregion
    public void initEquip(GDEEquipmentData equip)
    {
        int equipPos = SDDataManager.Instance.getEquipPosById(equip.id);
        //type = SDConstants.ItemType.Equip;
        itemId = equip.id;
        itemHashcode = equip.hashcode;
        if (fightForceText) fightForceText.text
                = "" + (SDDataManager.Instance.getEquipBattleForceByHashCode(itemHashcode));//读取装备战斗力
        itemUpLv = SDDataManager.Instance.getLevelByExp(equip.exp);
        if (downText) downText.text = SDDataManager.Instance.getEquipNameByHashcode(itemHashcode);
        if (upText)
        {
            upText.gameObject.SetActive(true);
            upText.text = SDGameManager.T("Lv.")
                + SDDataManager.Instance.getLevelByExp(equip.exp);
        }

        int rarity = SDDataManager.Instance.getEquipRarityById(itemId);
        //frameImg;
    }
    #endregion
    #region 目标为Prop
    #region prop效果
    public void choosePropToShowDetail()
    {
        DepositoryPanel dp = GetComponentInParent<DepositoryPanel>();
        if (dp)
        {
            dp.showCurrentItemDetail(itemId, itemNum);
            sourceController.Select_None();
            isSelected = true;
        }
    }
    public void choosePropToChangeSlot()
    {
        if (bagController)
        {
            bagController.selectPropToChangeCurrentSlot(itemId);
        }
    }
    public void choosePropToSell()
    {

    }
    #endregion
    public void initProp(GDEItemData prop, bool showTaken = false)
    {
        itemId = prop.id;
        type = SDConstants.ItemType.Prop;
        itemHashcode = 0;
        ROPropData P = SDDataManager.Instance.getPropDataById(itemId);

        fightForceText?.gameObject.SetActive(false);
        itemUpLv = 0;
        upText?.gameObject.SetActive(true);
        upText.text = P.name;
        itemNum = prop.num;
        downText.gameObject.SetActive(true);
        downText.text = "" + prop.num;
        slider?.gameObject.SetActive(false);


        int rarity = P.rarity;
        if(starVision != null)
            starVision.StarNum = rarity;

        if(showTaken) isSelected = SDDataManager.Instance.checkIfPropIsTaken(itemId);
    }
    #endregion
    #region 目标为Material
    #region material效果
    public void chooseMaterialToShowDetail()
    {
        DepositoryPanel dp = GetComponentInParent<DepositoryPanel>();
        if (dp)
        {
            dp.showCurrentItemDetail(itemId, itemNum);
            sourceController.Select_None();
            isSelected = true;
        }
    }
    public void chooseMaterialToSell()
    {

    }
    #endregion
    public void initMateiral(GDEItemData M)
    {
        itemId = M.id;
        type = SDConstants.ItemType.Material;
        itemHashcode = 0;
        ROMaterialData P = SDDataManager.Instance.getMaterialDataById(itemId);

        fightForceText?.gameObject.SetActive(false);
        itemUpLv = 0;
        upText?.gameObject.SetActive(true);
        upText.text = P.name;
        itemNum = M.num;
        downText.gameObject.SetActive(true);
        downText.text = "" + M.num;
        slider?.gameObject.SetActive(false);

        int rarity = P.rarity;
        if (starVision != null)
            starVision.StarNum = rarity;
    }
    #endregion
    #region 目标为Drop
    public void initDrop(GDEItemData drop)
    {
        itemId = drop.id;
        itemHashcode = 0;
        if (fightForceText) fightForceText.gameObject.SetActive(false);
        if (upText) upText.gameObject.SetActive(false);
        SDConstants.ItemType it = SDDataManager.Instance.getItemTypeById(itemId);
        if (it == SDConstants.ItemType.Material || it == SDConstants.ItemType.Prop)
        {
            if (downText) downText.text = "X" + drop.num;
        }

    }
    #endregion
    #region 目标为Goddess
    #region goddess效果
    public void chooseGoddessToShowDetail()
    {
        GoddessPanel GP = GetComponentInParent<GoddessPanel>();
        if (GP)
        {

        }
        else
        {
            chooseGoddessAsTeamLeader();
        }
    }
    public void chooseGoddessAsTeamLeader()
    {
        SelectTeamUnitPanel STUP = GetComponentInParent<SelectTeamUnitPanel>();
        if (STUP)
        {
            string cid = STUP.CurrentTeamId;
            GDEunitTeamData team = SDDataManager.Instance.getHeroTeamByTeamId(cid);
            if (team.goddess == itemId)
            {
                isSelected = true;
            }
            else
            {
                isSelected = false;
            }
            //
            if (!isSelected)
            {
                isSelected = true;
                SDDataManager.Instance.setTeamGoddess(cid, itemId);
            }
            else
            {
                isSelected = false;
                SDDataManager.Instance.setTeamGoddess(cid, string.Empty);
            }
            STUP.goddess_pageController.ItemsInit(SDConstants.ItemType.Goddess);
            GDEgoddessData G = SDDataManager.Instance.getGDEGoddessDataById(itemId);
            STUP.SDGD.initgoddessDetailVision(G);
            STUP.currentGoddessSimpleDetail.ReadFromSDGD(STUP.SDGD);
        }

    }
    #endregion
    public void initGoddess(GDEgoddessData goddess)
    {
        type = SDConstants.ItemType.Goddess;
        itemId = goddess.id;
        RoGoddessData g = SDDataManager.Instance.getGoddessData(goddess);
        //itemHashcode = goddess.GetHashCode();
        upText.gameObject.SetActive(true);
        upText.text = SDGameManager.T("Lv.") + g.lv;
        starVision.StarNum = g.star;
        downText.text = SDGameManager.T(g.name);
        itemNum = g.volume;
        SelectTeamUnitPanel STUP = GetComponentInParent<SelectTeamUnitPanel>();
        if (STUP)
        {
            string cid = STUP.CurrentTeamId;
            GDEunitTeamData team = SDDataManager.Instance.getHeroTeamByTeamId(cid);
            if (team.goddess == itemId)
            {
                isSelected = true;
            }
            else
            {
                isSelected = false;
            }

        }
    }
    #endregion
    #region 目标为NPC
    #region NPC效果
    public void chooseNPCToBuffAssemblyLine()
    {
        FactoryPanel FP = GetComponentInParent<FactoryPanel>();
        if (!FP) return;
        SDDataManager.Instance.ChangeNPCInFactoryAssemblyLine(itemHashcode, FP.SelectedTaskId);
        if (SDDataManager.Instance.haveTimeTaskByTaskId(FP.SelectedTaskId
            ,out GDEtimeTaskData task))
        {
            FP.refreshThisAssemblyLine(task);
        }
    }
    #endregion
    public void initNPC(GDENPCData data)
    {
        //type = SDConstants.ItemType.NPC;
        itemId = data.id;
        itemHashcode = data.hashcode;
        //if(downText)downText.text = 
        
    }
    #endregion

    public void PurchaseBtnTapped()
    {
        choosePurchaseToDetail();
    }

    #region 目标为purchase
    #region purchase效果
    public void choosePurchaseToDetail()
    {
        StorePanel _storePanel = GetComponentInParent<StorePanel>();
        if (_storePanel)
        {
            //_storePanel.consumeToGetItems(itemId, extraTrigger, itemNum);
        }
    }
    public void choosePurchaseToBuy()
    {
        StorePanel _storePanel = GetComponentInParent<StorePanel>();
        if (_storePanel)
        {
            _storePanel.consumeToGetItems(itemId, extraTrigger, itemNum);
        }
    }
    #endregion

    public void initPurchase(GDEItemData Purchase , bool useDamond = false)
    {
        itemId = Purchase.id;
        itemNum = Purchase.num;
        extraTrigger = useDamond;
        SDConstants.ItemType item_type = SDDataManager.Instance.getItemTypeById(itemId);
        type = item_type;
        int bpg = 0;
        if (type == SDConstants.ItemType.Material)
        {
            ROMaterialData data = SDDataManager.Instance.getMaterialDataById(itemId);
            if (upText) upText.text = SDGameManager.T(data.name);
            string downT = "";
            if (!useDamond)
            {
                bpg = data.buyPrice_gold * Purchase.num;
                downT = SDGameManager.T("金币") + " - " + bpg;
            }
            else
            {
                bpg = data.buyPrice_damond * Purchase.num;
                downT = SDGameManager.T("钻石") + " - " + bpg;
            }
            if (downText) downText.text = downT;
        }
        else if(type == SDConstants.ItemType.Prop)
        {

        }


        if (SDDataManager.Instance.PlayerData.damond < bpg)
        {
            downText.transform.parent.GetComponent<Button>().interactable = false;
        }
        else
        {
            downText.transform.parent.GetComponent<Button>().interactable = true;
        }
    }
    #endregion
}
