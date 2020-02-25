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
    
    public Text lvText;
    public Text NameText;
    public Text NumText;
    public SDConstants.StockType stockType;
    //public SDConstants.ItemType itemType;
    public SDConstants.MaterialType materialType = SDConstants.MaterialType.end;
    public string itemId;
    public int hashcode;
    public int itemNum;
    public int itemLv;
    public string itemName;
    //public int itemUpLv;
    public ItemStarVision starVision;

    //
    public Image selectImg;
    bool _isSelected;
    public bool isSelected 
    {
        get { return _isSelected; }
        set 
        {
            _isSelected = value;
            selectImg?.gameObject.SetActive(_isSelected);
            if (!_isSelected)
            {
                if (NumText) NumText.text = "" + itemNum;
            }
        }
    }

    public Transform lockedPanel;
    bool _isLocked;
    public bool isLocked
    {
        get { return _isLocked; }
        set
        {
            _isLocked = value;
            lockedPanel?.gameObject.SetActive(_isLocked);
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
        if (isLocked) 
        {
            Debug.Log("被锁定无法添加");
            return;
        }
        if (UsedNum >= itemNum) return;
        SDConstants.StockUseType useType = stockPage.currentUseType;
        //
        if (stockType == SDConstants.StockType.prop)
        {

        }
        else
        {
            if (useType == SDConstants.StockUseType.work)
            {
                if ( materialType == SDConstants.MaterialType.exp
                    || materialType == SDConstants.MaterialType.star
                    || materialType == SDConstants.MaterialType.skill)
                {
                    stockPage.chooseStock(this);
                }
            }
        }
    }
    void chooseHeroToDetail()
    {

    }
    void chooseHeroToSell()
    {

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
        //
        isLocked = data.locked;
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
        if (NumText) NumText.text = UsedNum + " / " + itemNum;

        itemImg.sprite = item.IconFromAtlas;
        bgImg.sprite = SDDataManager.Instance.baseBgSpriteByRarity(item.LEVEL);
        frameImg.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(item.LEVEL);

        //
        isLocked = false;
    }
}
