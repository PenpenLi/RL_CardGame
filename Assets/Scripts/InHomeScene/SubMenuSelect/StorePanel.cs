using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;
using UnityEngine.UI;

public class StorePanel : BasicSubMenuPanel
{
    [Space(50)]
    public HEWPageController page;
    public Image goodsPoster;
    public enum purchaseType
    {
        fresh=0
            ,
        bale=1
            ,
        common=2
            ,
        hero=3
            ,
        equip=4
            ,
        end
            ,
    }
    purchaseType _c_t = purchaseType.end;
    public purchaseType current_type
    {
        get { return _c_t; }
        set { if (_c_t != value)
            {
                purchaseType oldT = _c_t;
                _c_t = value;
                refreshgoodsInCurrentPurchaseType(oldT, _c_t);
            }
        }
    }
    public Transform BtnsContent;
    public bool notUseDamond;

    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        refreshAllPurchaseTypeBtns();
    }
    public override void commonBackAction()
    {
        base.commonBackAction();
        current_type = purchaseType.end;
        homeScene.SubMenuClose();
    }
    public void refreshAllPurchaseTypeBtns()
    {
        for(int i = 0; i < (int)purchaseType.end; i++)
        {
            if (i < BtnsContent.childCount && BtnsContent.GetChild(i).GetComponent<Button>())
            {
                BtnsContent.GetChild(i).GetComponentInChildren<Text>().text 
                    = SDGameManager.T(((purchaseType)i).ToString());
            }
        }
    }
    public void refreshgoodsInCurrentPurchaseType(purchaseType oldT,purchaseType newT)
    {
        if(oldT!= purchaseType.end)
        {
            BtnsContent.GetChild((int)oldT).GetComponent<Button>().interactable = true;
        }
        if (newT != purchaseType.end)
        {
            BtnsContent.GetChild((int)newT).GetComponent<Button>().interactable = false;

        }
    }
    public void initCurrentGoodsList()
    {
        if(current_type == purchaseType.fresh)
        {

        }
        else if(current_type == purchaseType.bale)
        {

        }
        else if(current_type == purchaseType.common)
        {

        }
        else if(current_type == purchaseType.hero)
        {

        }
        else if(current_type == purchaseType.equip)
        {

        }
    }
    #region AllGoodsListInit
    void init_fresh()
    {
        if (!notUseDamond)
        {

        }
        else
        {

        }
    }
    void init_bale()
    {

    }
    void init_common()
    {

    }
    void init_hero()
    {

    }
    void init_equip()
    {

    }
    #endregion
    public void consumeToGetItems(string itemId , bool useDamond , int itemNum = 1)
    {
        SDConstants.ItemType type = SDDataManager.Instance.getItemTypeById(itemId);
        int allConsume = 0;
        if (type == SDConstants.ItemType.Consumable)
        {
            consumableItem data = SDDataManager.Instance.getConsumableItemById(itemId);
            if (!useDamond)
            {
                allConsume = data.buyPrice_coin * itemNum;
            }
            else
            {
                allConsume = data.buyPrice_diamond * itemNum;
            }
        }
        else if (type == SDConstants.ItemType.Consumable)
        {
            consumableItem DATA = SDDataManager.Instance.getConsumableItemById(itemId);
            if (!useDamond)
            {
                allConsume = DATA.buyPrice_coin * itemNum;
            }
            else
            {
                allConsume = DATA.buyPrice_diamond * itemNum;
            }
        }
        else if (type == SDConstants.ItemType.Equip)
        {

        }

        if (allConsume <= 0) return;

        if (!useDamond)
        {
            if (ConsumeCoinToGetItems(allConsume,itemId,out int surplus, itemNum))
            {
                Debug.Log("消耗 金币 " + allConsume + " 获得 商品id " + itemId
                    + " X" + itemNum + "====== 剩余 金币 " + surplus);
            }
        }
        else
        {
            PopoutController.CreatePopoutAlert
                (
                "",
                SDGameManager.T("是否确认购买"),
                5,
                true,
                PopoutController.PopoutWIndowAlertType.ConfirmMessage,
                (PopoutController c, PopoutController.PopoutWindowAlertActionType d) =>
                {
                    if(d == PopoutController.PopoutWindowAlertActionType.OnConfirm)
                    {
                        bool a = ConsumeDamondToGetItems
                        (allConsume, itemId, out int surplus, itemNum);
                        if (a)
                        {
                            Debug.Log("消耗 钻石 " + allConsume + " 获得 商品id " + itemId
                                + " X" + itemNum + "====== 剩余 钻石 " + surplus);
                            c._dismissAfterTapBtn = true;
                        }
                    }
                    else if(d == PopoutController.PopoutWindowAlertActionType.OnCancel)
                    {
                        c._dismissAfterTapBtn = true;
                    }
                    else if(d == PopoutController.PopoutWindowAlertActionType.OnDismiss)
                    {
                        c._dismissAfterTapBtn = true;
                    }
                }
                );           
        }
    }
    public bool ConsumeCoinToGetItems
        (int allConsume, string itemId,out int surplus,int itemNum = 1)
    {
        if (SDDataManager.Instance.PlayerData.coin >= allConsume)
        {
            //购买手续
            SDDataManager.Instance.PlayerData.coin -= allConsume;
            //
            SDDataManager.Instance.addConsumable(itemId, itemNum);
            surplus = SDDataManager.Instance.PlayerData.coin;
            return true;
        }
        else
        {
            surplus = SDDataManager.Instance.PlayerData.coin;
            return false;
        }
    }
    public bool ConsumeDamondToGetItems
        (int allConsume, string itemId,out int surplus, int itemNum = 1)
    {
        if (SDDataManager.Instance.PlayerData.damond >= allConsume)
        {
            //购买手续
            SDDataManager.Instance.PlayerData.damond -= allConsume;
            //
            SDDataManager.Instance.addConsumable(itemId, itemNum);
            surplus = SDDataManager.Instance.PlayerData.damond;
            return true;
        }
        else
        {
            surplus = SDDataManager.Instance.PlayerData.damond;
            return false;
        }
    }

    
    public void BtnToEnterPurchaseType(Transform tappedTarget)
    {
        int index = tappedTarget.GetSiblingIndex();
        current_type = (purchaseType)index;
    }
    public void BtnToChangeConsumeCurrency(bool notusedamond)
    {
        if (!notUseDamond && notusedamond)//使用coin按钮
        {
            notUseDamond = true;
            initCurrentGoodsList();
        }
        else if(notUseDamond && !notusedamond)//使用damond按钮
        {
            notUseDamond = false;
            initCurrentGoodsList();
        }
    }
}
