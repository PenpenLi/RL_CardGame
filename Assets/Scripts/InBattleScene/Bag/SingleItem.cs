using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using System;

/// <summary>
/// 通用物件选择项
/// </summary>
public class SingleItem : MonoBehaviour
{
    public Image itemImg;
    public Image itemBgImg;
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
    [HideInInspector]
    public EquipPosition equipPos;
    public ItemStarVision starVision;
    public Transform slider;
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
    public Transform EmptyPanel;
    public Transform SelectedPanel;
    public Transform UnablePanel;
    public CanvasGroup canvasGroup;
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
                if (SDGameManager.Instance.heroSelectType
                    == SDConstants.HeroSelectType.Battle)
                {
                    chooseHeroToBattle();
                }
                else if (SDGameManager.Instance.heroSelectType
                    == SDConstants.HeroSelectType.Wake)
                {
                    chooseHeroToWake();
                }
                else if (SDGameManager.Instance.heroSelectType
                    == SDConstants.HeroSelectType.Dying)
                {
                    chooseHeroToTreat();
                }
                else if(SDGameManager.Instance.heroSelectType
                    == SDConstants.HeroSelectType.Mission)
                {

                }
                else if (SDGameManager.Instance.heroSelectType
                    == SDConstants.HeroSelectType.Altar)
                {

                }
                else if (SDGameManager.Instance.heroSelectType
                    == SDConstants.HeroSelectType.All)
                {
                    chooseHeroToDetail();
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
            else if(type == SDConstants.ItemType.Consumable)
            {
                if (SDGameManager.Instance.stockUseType == SDConstants.StockUseType.detail)
                {
                    chooseConsumableToShowDetail();
                }
                else if (SDGameManager.Instance.stockUseType == SDConstants.StockUseType.work)
                {
                    choosePropToChangeSlot();
                }
                else if (SDGameManager.Instance.stockUseType == SDConstants.StockUseType.detail)
                {
                    chooseConsumableToShowDetail();
                }
            }
            else if(type == SDConstants.ItemType.Rune)
            {
                if(SDGameManager.Instance.stockUseType == SDConstants.StockUseType.detail)
                {
                    chooseRuneToShowDetail();
                }
                else if(SDGameManager.Instance.stockUseType == SDConstants.StockUseType.work)
                {
                    chooseEquipedRune();
                }
                else if(SDGameManager.Instance.stockUseType == SDConstants.StockUseType.sell)
                {

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
            //
            SDDataManager.Instance.setHeroTeam(STUP.CurrentTeamId
                , STUP.currentHeroIndexInTeam
                , itemHashcode) ;
            //
            STUP.refreshUI();
            //
            BHS.heroSelectPanel.heroPanelInit();
        }
    }
    public void chooseHeroToWake()
    {

    }
    public void chooseHeroToDetail()
    {
        BasicHeroSelect BHS = GetComponentInParent<BasicHeroSelect>();
        if (BHS)
        {
            //显示英雄详情
            SDHeroDetail HDP = BHS.heroDetails.GetComponent<SDHeroDetail>();
            HDP.HeroWholeMessage.currentHeroHashcode = itemHashcode;
            HDP.HeroWholeMessage.whenOpenThisPanel();
        }
    }
    public void chooseHeroToTreat()
    {
        HospitalPanel hp = GetComponentInParent<HospitalPanel>();
        if (hp && hp.HaveEmptySickBed(out int index))
        {
            bool flag = SDDataManager.Instance.AddTimeTask(SDConstants.timeTaskType.HOSP
                , itemHashcode, itemId, hp.getTaskIdFromIndex(index));
            if (flag)
            {
                hp.initHospital();
            }
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
        downText.text = SDGameManager.T(dal.Info.Name);
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

        if (careerImg)
        {
            Sprite career = dal.Info.Career.Icon;
            careerImg.sprite = career;
        }
        if (raceImg)
        {
            Sprite race = dal.Info.Race.Icon;
            raceImg.sprite = race;
        }
        itemBgImg.sprite = SDDataManager.Instance.heroBgSpriteByRarity(dal.Info.Rarity);
        frameImg.sprite = SDDataManager.Instance.heroFrameSpriteByRarity(dal.Info.Rarity);
    }
    public void initInjuriedHero(GDEHeroData hero)
    {
        type = SDConstants.ItemType.Hero;
        itemId = hero.id;
        itemHashcode = hero.hashCode;
        //
        float fatigueRate = SDDataManager.Instance.getHeroFatigueRate(hero.hashCode);
        int status = SDDataManager.Instance.getHeroStatus(hero.hashCode);
        Debug.Log("该英雄疲劳值:" + hero.Fatigue);
        //
        ROHeroData dal = SDDataManager.Instance.getHeroDataByID(itemId, hero.starNumUpgradeTimes);

        upText.gameObject.SetActive(true);
        upText.text = "Lv." + SDDataManager.Instance.getLevelByExp(hero.exp);
        downText.text = SDGameManager.T(dal.Info.Name);
        slider?.gameObject.SetActive(true);
        //
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
            if (fatigueRate > 0.1f) { isUnable = false; }
            else isUnable = true;
        }//无事可做状态
        else
        {
            isSelected = false;
            isUnable = true;
        }
        //starVision.StarNum = dal.starNum;
        starVision.gameObject.SetActive(false);
        slider.GetChild(0).localScale 
            = new Vector3(1 - fatigueRate, 1, 1);
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
        if (characterModel != null)
        {
            characterModel.initHeroCharacterModel(itemHashcode, SDConstants.HERO_MODEL_RATIO);
        }
        itemLevel = SDDataManager.Instance.getLevelByExp(hero.exp);
        if (upText)
        {
            upText.gameObject.SetActive(false);
        }
        if (downText)
        {
            downText.text = SDGameManager.T("Lv.") + itemLevel;
        }
        slider?.gameObject.SetActive(false);
        if(starVision) starVision.StarNum = roh.starNum;
        if (statusImg)
        {
            statusImg.gameObject.SetActive(true);
            statusImg.sprite = UIEffectManager.Instance.heroStatusSps[hero.status];
        }

        //
        if (itemImg)
        {

        }
        //
        if (careerImg)
        {
            Sprite career = roh.Info.Career.Icon;
            careerImg.sprite = career;
        }
        if (raceImg)
        {
            Sprite race = roh.Info.Race.Icon;
            raceImg.sprite = race;
        }
        itemBgImg.sprite = SDDataManager.Instance.heroBgSpriteByRarity(roh.Info.Rarity);
        frameImg.sprite = SDDataManager.Instance.heroFrameSpriteByRarity(roh.Info.Rarity);
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
        int quality = SDDataManager.Instance.getHeroInfoById(itemId).Rarity;
        frameImg.sprite = SDDataManager.Instance.raritySprite(quality);
        frameImg.SetNativeSize();
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
            UIEffectManager.Instance.showAnimFadeIn(EP.EDP.transform);
            EP.EDP
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
        SetSelfAsBg(false);
        EquipItem item = SDDataManager.Instance.GetEquipItemById(equip.id);
        equipPos = (EquipPosition)SDDataManager.Instance.getEquipPosById(equip.id);
        itemId = equip.id;
        //
        itemImg.sprite = SDDataManager.Instance.GetEquipIconById(itemId);
        int rarity = item.LEVEL;
        frameImg.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(rarity);
        itemBgImg.sprite = SDDataManager.Instance.baseBgSpriteByRarity(rarity);
        //
        itemHashcode = equip.hashcode;
        itemUpLv = equip.lv;
        if (starVision)
        {
            starVision.gameObject.SetActive(false);
        }
        if (downText) downText.text = SDGameManager.T(item.NAME);
        if (upText)
        {
            upText.gameObject.SetActive(true);
            upText.text = SDDataManager.Instance.rarityString(item.LEVEL);
        }
    }
    #endregion
    #region 目标为Consumable
    #region consumable效果
    public void chooseConsumableToShowDetail()
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
    public void chooseMaterialToSell()
    {

    }
    #endregion
    public void initConsumable(GDEItemData M, bool showTaken = false)
    {
        itemId = M.id;
        type = SDConstants.ItemType.Consumable;
        itemHashcode = 0;
        consumableItem P = SDDataManager.Instance.getConsumableById(itemId);

        fightForceText?.gameObject.SetActive(false);
        itemUpLv = 0;
        upText?.gameObject.SetActive(true);
        upText.text = P.NAME;
        itemNum = M.num;
        downText.gameObject.SetActive(true);
        downText.text = "" + M.num;
        slider?.gameObject.SetActive(false);

        int rarity = P.LEVEL;
        if (starVision != null)
            starVision.StarNum = rarity;

        if (showTaken) isSelected = SDDataManager.Instance.checkIfPropIsTaken(itemId);
    }
    #endregion
    #region 目标为Rune
    #region Rune效果
    public void chooseRuneToShowDetail()
    {
        RunePanel RP = GetComponentInParent<RunePanel>();
        if (RP == null) return;
        RP.ChangeCurrentRuneHashcode(itemHashcode);
        RP.refreshPage();
    }
    public void chooseEquipedRune()
    {
        GoddessDetailPanel GDP = GetComponentInParent<GoddessDetailPanel>();
        if (GDP == null) return;
        SDDataManager.Instance.addRuneToGoddessSlot(itemHashcode, GDP.CurrentGoddess.ID
            ,GDP.currentGoddessRunePos);
        GDP.refreshGoddessList();
        //
        HEWPageController page = GDP.runePanel.GetComponentInChildren<HEWPageController>();
        page.ItemsInit(SDConstants.ItemType.Rune);
        //Debug.Log("C--R");
        GDP.RDP.initDetailPanel(SDDataManager.Instance.getRuneOwnedByHashcode(itemHashcode));
    }
    #endregion
    public void initRuneInPage(GDERuneData E)
    {
        if (E == null || E.Hashcode <= 0)
        {
            isEmpty = true; return;
        }
        isEmpty = false;
        if (upText)
        {
            upText.text = SDGameManager.T("Lv.")
                + E.level;
        }
        if (starVision)
        {
            starVision.gameObject.SetActive(false);
        }
        RuneItem item = SDDataManager.Instance.getRuneItemById(E.id);
        //
        frameImg.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(item.Quality);
        itemBgImg.sprite = SDDataManager.Instance.baseBgSpriteByRarity(item.Quality);
        //
        itemHashcode = E.Hashcode;
        itemId = E.id;
        GoddessDetailPanel GDP = GetComponentInParent<GoddessDetailPanel>();
        if (GDP == null) return;
        string goddessId = GDP.CurrentGoddess.ID;

        isSelected = false;
        if (SDDataManager.Instance.checkRuneEquippedByGoddess(itemHashcode,goddessId,out int pos))
        {
            isSelected = true;
            index = pos;
            upText.text += "-----" + index;
        }
        else if (SDDataManager.Instance.checkRuneStatus(itemHashcode))
        {
            isSelected = true;
        }
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
        if (it == SDConstants.ItemType.Consumable)
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
        //RoGoddessData g = SDDataManager.Instance.getGoddessData(goddess);
        GoddessInfo g = SDDataManager.Instance.getGoddessInfoById(goddess.id);
        //itemHashcode = goddess.GetHashCode();
        upText.gameObject.SetActive(true);
        upText.text = SDGameManager.T("Lv.") + SDDataManager.Instance.getLevelByExp(goddess.exp);
        starVision.StarNum = goddess.star;
        downText.text = SDGameManager.T(g.name);
        itemNum = goddess.volume;
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
    #region 目标为Enemy
    #region Enemy效果
    public void chooseEnemyToShowDetail()
    {
        IllustratePanel IP = GetComponentInParent<IllustratePanel>();
        if (IP)
        {
            IP.initCurrentEnemyIllustarte(itemId);
            sourceController.Select_None();
            isSelected = true;
        }
    }
    #endregion
    public void initEnemy(GDEItemData item)
    {
        itemId = item.id;
        itemNum = item.num;
        EnemyInfo E = SDDataManager.Instance.getEnemyInfoById(itemId);
        itemLevel = E.EnemyRank.Index;
        if (upText)
        {
            upText.gameObject.SetActive(true);
            upText.text = SDGameManager.T(E.Name);
        }
        starVision.StarNum = itemLevel;
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
        if (type == SDConstants.ItemType.Consumable)
        {
            consumableItem data = SDDataManager.Instance.getConsumableById(itemId);
            if (upText) upText.text = SDGameManager.T(data.name);
            string downT = "";
            if (!useDamond)
            {
                bpg = data.buyPrice_coin * Purchase.num;
                downT = SDGameManager.T("金币") + " - " + bpg;
            }
            else
            {
                bpg = data.buyPrice_diamond * Purchase.num;
                downT = SDGameManager.T("钻石") + " - " + bpg;
            }
            if (downText) downText.text = downT;
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








    public void SetSelfAsBg(bool flag = true)
    {
        gameObject.SetActive(!flag);
        //canvasGroup.alpha = flag ? 0 : 1;
        //transform.localScale = flag ? Vector3.zero : Vector3.one;
    }
}
