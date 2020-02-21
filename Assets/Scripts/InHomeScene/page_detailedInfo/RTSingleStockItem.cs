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
    public Image frameImg;
    public Image bgImg;
    public Image selectImg;
    
    public Text lvText;
    public Text NameText;
    public Text NumText;
    public SDConstants.StockType stockType;
    public SDConstants.MaterialType materialType = SDConstants.MaterialType.end;
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
        if (stockType == SDConstants.StockType.prop)
        {

        }
        else
        {
            if (useType == SDConstants.StockUseType.work)
            {
                if (materialType == SDConstants.MaterialType.star
                    || materialType == SDConstants.MaterialType.skill)
                {
                    stockPage.chooseStock(this);
                }
            }
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







    public void initStock(GDEHeroData data
        , SDConstants.MaterialType MType = SDConstants.MaterialType.end)
    {
        stockType = SDConstants.StockType.hero;
        materialType = MType;
        itemId = data.id;
        hashcode = data.hashCode;
        itemNum = 1;
        HeroInfo info = SDDataManager.Instance.getHeroInfoById(itemId);
        starVision.StarNum = info.LEVEL + data.starNumUpgradeTimes;
        if (NumText) NumText.gameObject.SetActive(false);

        itemImg.sprite = info.FaceIcon;
        bgImg.sprite = SDDataManager.Instance.baseBgSpriteByRarity(info.Rarity);
        frameImg.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(info.Rarity);
    }
    public void initStock(GDEItemData data
        , SDConstants.MaterialType MType = SDConstants.MaterialType.end)

    {
        stockType = SDConstants.StockType.material;
        materialType = MType;
        itemId = data.id;
        itemNum = data.num;
        hashcode = 0;

        consumableItem item = SDDataManager.Instance.getConsumableById(itemId);
        if (starVision) starVision.gameObject.SetActive(false);
        if (NumText) NumText.text = "X" + itemNum;

        itemImg.sprite = item.IconFromAtlas;
        bgImg.sprite = SDDataManager.Instance.baseBgSpriteByRarity(item.LEVEL);
        frameImg.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(item.LEVEL);
    }
}
