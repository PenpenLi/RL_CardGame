using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

/// <summary>
/// 单材料项
/// </summary>
public class RTSingleStockItem : MonoBehaviour
{
    public Image itemImg;
    //public CharacterModelController characterModel;
    public Image frameImg;
    public Image bgImg;
    public Image selectImg;
    
    public Text lvText;
    public Text NameText;
    public Text NumText;
    public SDConstants.StockType stockType;
    public SDConstants.MaterialType materialType = SDConstants.MaterialType.all;
    public string itemId;
    public int hashcode;
    public int itemNum;
    public int itemLv;
    public string itemName;
    //public int itemUpLv;
    public ItemStarVision starVision;
    bool _isSelected;
    public bool isSelected 
    {
        get { return _isSelected; }
        set 
        {
            if (value != _isSelected)
            {
                _isSelected = value;
                selectImg?.gameObject.SetActive(_isSelected);
                if (!_isSelected)
                {
                    if (NumText) NumText.text = "" + itemNum;
                }
            }
        }
    }
    public int UsedNum = 0;
    public StockPageController stockPage;

    public void BtnTapped()
    {
        //beingTapped = true;
        if (!stockPage.checkIfCanWork(this))
        {
            Debug.Log("无法点击");
            return;
        }
        SDConstants.StockUseType useType = stockPage.currentUseType;
        //
        if (stockType == SDConstants.StockType.hero)
        {
            if(useType == SDConstants.StockUseType.work)
            {
                chooseHeroToConsume();
            }
            else if(useType == SDConstants.StockUseType.detail)
            {
                chooseHeroToDetail();
            }
            else if(useType == SDConstants.StockUseType.sell)
            {
                chooseHeroToSell();
            }
        }
        else if (stockType == SDConstants.StockType.material)
        {
            if(useType == SDConstants.StockUseType.work)
            {
                chooseMaterialToConsume();
            }
            else if(useType == SDConstants.StockUseType.detail)
            {
                chooseMaterialToDetail();
            }
            else if(useType == SDConstants.StockUseType.sell)
            {
                chooseMaterialToSell();
            }
            if (materialType == SDConstants.MaterialType.all)
            {

            }
            else if (materialType == SDConstants.MaterialType.exp)
            {

            }
            else if (materialType == SDConstants.MaterialType.star)
            {

            }
            else if (materialType == SDConstants.MaterialType.skill)
            {

            }
            else if (materialType == SDConstants.MaterialType.money)
            {

            }
        }
        else if (stockType == SDConstants.StockType.prop)
        {

        }
    }
    public void chooseHeroToConsume()
    {
        if(stockPage.maxSelectedNum > 0 
            && stockPage.currentSelectedNum < stockPage.maxSelectedNum)
        {
            if (!SDDataManager.Instance.getHeroIfLocked(hashcode))
            {
                isSelected = true;
                UsedNum = 1;
            }
            else
            {

            }
        }
    }
    public void chooseHeroToDetail()
    {

    }
    public void chooseHeroToSell()
    {

    }
    public void chooseMaterialToConsume()
    {
        if (stockPage.maxSelectedNum > 0
            && stockPage.currentSelectedNum < stockPage.maxSelectedNum)
        {
            isSelected = true;
            if (UsedNum < itemNum)
                UsedNum++;
            NumText.text = UsedNum + "/" + itemNum;
        }
    }
    public void chooseMaterialToDetail()
    {

    }
    public void chooseMaterialToSell()
    {

    }
    public void initStock(GDEHeroData data)
    {
        stockType = SDConstants.StockType.hero;
        itemId = data.id;
        hashcode = data.hashCode;
        itemNum = 1;
        itemLv = SDDataManager.Instance.getLevelByExp(data.exp);
        ROHeroData roh = SDDataManager.Instance.getHeroDataByID(itemId, data.starNumUpgradeTimes);
        starVision.StarNum = roh.starNum;
        itemName = roh.Name;
        //if (NameText) NameText.text = SDDataManager.Instance.getMaterialNameById(itemId);
        if (lvText) lvText.text = SDGameManager.T("Lv.") + itemLv;
        if (NumText) NumText.gameObject.SetActive(false);
        if(NameText) NameText.gameObject.SetActive(false);
    }
    public void initStock(GDEItemData data
                , SDConstants.StockType SType = SDConstants.StockType.material
        , SDConstants.MaterialType MType = SDConstants.MaterialType.all)

    {
        stockType = SType;
        materialType = MType;
        itemId = data.id;
        itemNum = data.num;
        hashcode = 0;

        itemLv = SDDataManager.Instance.getMaterialLevelById(itemId);
        starVision.StarNum = SDDataManager.Instance.getMaterialLevelById(itemId);
        itemName = SDDataManager.Instance.getMaterialNameById(itemId);

        if (NameText) NameText.gameObject.SetActive(false);
        if (NumText) NumText.text = "" + itemNum;
        if (lvText) lvText.gameObject.SetActive(false);

    }
}
