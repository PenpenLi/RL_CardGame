using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SingleSlot : MonoBehaviour
{
    [Header("Vision")]
    public Image bgIcon;
    public Image Icon;
    public Image frameIcon;
    //
    public ItemStarVision starVision;
    public Text aboveText;
    public Text belowText;
    //
    public Transform emptyPanel;
    private bool _isEmpty;
    public bool isEmpty
    {
        get { return _isEmpty; }
        set
        {
            if(_isEmpty != value)
            {
                _isEmpty = value;
                emptyPanel?.gameObject.SetActive(_isEmpty);
            }
        }
    }
    //
    public Transform lockPanel;
    public bool _isLocked;
    public bool isLocked
    {
        get { return _isLocked;}
        set
        {
            _isLocked = value;
            lockPanel?.gameObject.SetActive(_isLocked);
        }
    }

    public SDHeroImprove HeroImprove;

    //
    [Header("Data")]
    public SDConstants.ItemType ItemType;

    public string _id;
    public int _hashcode;
    public int num = 1;
    private void Start()
    {
        isEmpty = true;
    }

    public void AddHeroInSlot(int hashcode)
    {
        if (isLocked) return;
        GDEHeroData data = SDDataManager.Instance.GetHeroOwnedByHashcode(hashcode);
        if (data == null) return;
        HeroInfo info = SDDataManager.Instance.getHeroInfoById(data.id);
        if (info == null) return;
        //
        isEmpty = false;
        Icon.sprite = info.FaceIcon;
        bgIcon.sprite = SDDataManager.Instance.baseBgSpriteByRarity(info.Rarity);
        frameIcon.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(info.Rarity);
        //
        starVision?.gameObject.SetActive(true);
        starVision.StarNum = data.starNumUpgradeTimes + info.LEVEL;
        aboveText.text = SDGameManager.T("Lv.") + SDDataManager.Instance.getLevelByExp(data.exp);
        belowText?.gameObject.SetActive(false);
        //
        ItemType = SDConstants.ItemType.Hero;
        _id = info.ID;
        _hashcode = data.hashCode;
    }
    public void AddConsumableInSlot(string id)
    {
        if (isLocked) return;
        GDEItemData data = SDDataManager.Instance.getConsumeableDataById(id);
        if (data == null) return;
        if (data.num <= 0) return;
        consumableItem info = SDDataManager.Instance.getConsumableItemById(id);
        if (info == null) return;
        //
        isEmpty = false;
        Icon.sprite = info.IconFromAtlas;
        bgIcon.sprite = SDDataManager.Instance.baseBgSpriteByRarity(info.LEVEL);
        frameIcon.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(info.LEVEL);
        //
        starVision?.gameObject.SetActive(false);
        aboveText?.gameObject.SetActive(false);
        belowText.gameObject.SetActive(false);
        //
        ItemType = SDConstants.ItemType.Consumable;
        _id = id;
        _hashcode = 0;
    }
    public void AddFromStock(RTSingleStockItem stock)
    {
        if( stock.materialType == SDConstants.MaterialType.exp
            ||stock.materialType == SDConstants.MaterialType.star
            ||stock.materialType == SDConstants.MaterialType.skill)
        if(stock.stockType == SDConstants.StockType.hero)
        {
            AddHeroInSlot(stock.hashcode);
        }
        else if(stock.stockType == SDConstants.StockType.material)
        {
            AddConsumableInSlot(stock.itemId);
        }
    }


    public void ClearSlot()
    {
        if (isLocked) return;
        isEmpty = true;
        _id = string.Empty;
        _hashcode = 0;
        ItemType = SDConstants.ItemType.End;
    }

    public void BtnTapped()
    {
        if (HeroImprove)
        {
            ClearSlot();
            HeroImprove.RefreshImprovePanel();
        }
    }

    public bool consumeContentInSlot()
    {
        if(ItemType == SDConstants.ItemType.Hero)
        {
            bool flag = SDDataManager.Instance.consumeHero(_hashcode);
            return flag;

        }
        else if(ItemType == SDConstants.ItemType.Consumable)
        {
            bool flag = SDDataManager.Instance.consumeConsumable
                (_id,out int r, num);
            return flag;
        }
        return false;
    }
}
