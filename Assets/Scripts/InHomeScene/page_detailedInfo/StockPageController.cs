using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

/// <summary>
/// 材料与道具选择项控制类
/// </summary>
public class StockPageController : MonoBehaviour
{
    public Transform SItem;
    public List<RTSingleStockItem> items = new List<RTSingleStockItem>();
    public SDConstants.StockType stockType = SDConstants.StockType.material;
    public SDConstants.MaterialType materialType = SDConstants.MaterialType.all;
    public int pageIndex;
    public int itemCount;
    //
    public ScrollRect scrollRect;
    #region ImprovePanel
    public SDHeroImprove heroImproveController;
    public SDEquipImprove equipImproveController;
    public SDGoddessImprove goddessImproveController;
    #endregion
    public int maxSelectedNum;
    public int currentSelectedNum 
    {
        get 
        {
            int flag = 0;
            for(int i = 0; i < items.Count; i++)
            {
                if (items[i].isSelected)
                {
                    if(items[i].stockType == SDConstants.StockType.hero)
                        flag++;
                    else if(items[i].stockType == SDConstants.StockType.material)
                    {
                        int num = items[i].UsedNum;
                        flag += num;
                    }
                }
            }
            return flag;
        }
    }
    public SDConstants.StockUseType currentUseType
    {
        get { return SDGameManager.Instance.stockUseType; }
    }
    public Transform emptyPanel;
    public void ResetPage()
    {
        for(int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
        }
        items.Clear();
        scrollRectReset();
    }
    public void SelectEmpty()
    {
        for(int i = 0; i < items.Count; i++)
        {
            items[i].isSelected = false;
            items[i].UsedNum = 0;
        }
    }
    public void scrollRectReset()
    {
        scrollRect.content.anchoredPosition = Vector2.zero;
        scrollRect.verticalNormalizedPosition = 0;
    }
    public void checkPanelIsEmpty()
    {
        if (itemCount <= 0)
        {
            emptyPanel.gameObject.SetActive(true);
        }
        else
        {
            emptyPanel.gameObject.SetActive(false);
        }
    }
    #region Item_init
    /// <summary>
    /// 用于通用强化
    /// </summary>
    /// <param name="SType">强化材料类型</param>
    /// <param name="MType">强化方式</param>
    public void ItemsInit(SDConstants.StockType SType, SDConstants.MaterialType MType = SDConstants.MaterialType.all)
    {
        itemCount = 0;
        stockType = SType;
        if(SType == SDConstants.StockType.material)
        {
            showMaterialsOwned(MType);
        }
        else if(SType == SDConstants.StockType.hero)
        {
            showHeroesOwnned();
        }
        else if(SType == SDConstants.StockType.prop)
        {

        }
        else if(SType == SDConstants.StockType.equip)
        {

        }
        else if(SType == SDConstants.StockType.all)
        {

        }
        checkPanelIsEmpty();
    }
    /// <summary>
    /// 用于英雄强化
    /// </summary>
    /// <param name="MType">强化方式</param>
    public void ItemInit(SDConstants.MaterialType MType)
    {
        #region 英雄强化用
        List<GDEHeroData> heroes = SDDataManager.Instance.PlayerData.herosOwned;
        List<GDEItemData> all = SDDataManager.Instance.PlayerData.materials;

        materialType = MType;
        //对应材料构建
        for (int i = 0; i < all.Count; i++)
        {
            if (MType != SDConstants.MaterialType.all)
            {
                if (SDDataManager.Instance.getMaterialTypeById(all[i].id) == MType.ToString())
                {
                    if (MType == SDConstants.MaterialType.star
                        && SDDataManager.Instance.getMaterialLevelById(all[i].id)
                        != heroImproveController.heroDetail.StarNumVision.StarNum)
                        continue;

                    Transform s = Instantiate(SItem) as Transform;
                    s.transform.SetParent(scrollRect.content);
                    s.localScale = Vector3.one;
                    RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
                    _s.stockPage = this;
                    _s.initStock(all[i], SDConstants.StockType.material, MType);
                    items.Add(_s);
                }
            }
            else
            {
                string mt = SDDataManager.Instance.getMaterialTypeById(all[i].id);
                if (mt == SDConstants.MaterialType.exp.ToString()
                    || mt == SDConstants.MaterialType.skill.ToString()
                    || mt == SDConstants.MaterialType.star.ToString())
                {
                    Transform s = Instantiate(SItem) as Transform;
                    s.transform.SetParent(scrollRect.content);
                    s.localScale = Vector3.one;
                    RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
                    _s.stockPage = this;
                    _s.initStock(all[i]);
                    items.Add(_s);
                }
            }
        }
        //英雄(作为材料)构建
        string heroId = heroImproveController.heroDetail.ID;
        int heroStarNum = heroImproveController.heroDetail.StarNumVision.StarNum;
        for (int i = 0; i < heroes.Count; i++)
        {
            bool flag;
            if (MType == SDConstants.MaterialType.skill)
            {
                if (heroes[i].id == heroId)
                {
                    flag = true;
                }
                else flag = false;
            }
            else if (MType == SDConstants.MaterialType.star)
            {
                ROHeroData roh = SDDataManager.Instance.getHeroDataByID(heroId, heroes[i].starNumUpgradeTimes);
                if (roh.starNum == heroStarNum) flag = true;
                else flag = false;
            }
            else if (MType == SDConstants.MaterialType.likability)
            {
                flag = false;
            }
            else
            {
                flag = true;
            }
            if (flag && heroes[i].hashCode != heroImproveController.heroDetail.Hashcode)
            {
                Transform s = Instantiate(SItem) as Transform;
                s.transform.SetParent(scrollRect.content);
                s.localScale = Vector3.one;
                RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
                _s.stockPage = this;
                _s.initStock(heroes[i]);
                items.Add(_s);
            }
        }
        #endregion
        itemCount = items.Count;
        checkPanelIsEmpty();
    }
    public void showHeroesOwnned()
    {
        ResetPage();
        List<GDEHeroData> heroes = SDDataManager.Instance.PlayerData.herosOwned;
        itemCount = heroes.Count;
        for(int i = 0; i < itemCount; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.transform.SetParent(scrollRect.content);
            s.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            _s.initStock(heroes[i]);
        }
    }
    public void showMaterialsOwned(SDConstants.MaterialType mType = SDConstants.MaterialType.all)
    {
        ResetPage();
        List<GDEItemData> all = SDDataManager.Instance.PlayerData.materials;
        List<GDEItemData> _materials = new List<GDEItemData>();
        if (mType == SDConstants.MaterialType.all)
        {
            //int index = 0;
            itemCount = all.Count;
            for (int i = 0; i < itemCount; i++)
            {
                _materials.Add(all[i]);
            }
        }
        else
        {
            for (int i = 0; i < all.Count; i++)
            {
                string _mtype = SDDataManager.Instance.getMaterialTypeById(all[i].id);
                if (_mtype == mType.ToString())
                {
                    _materials.Add(all[i]);
                }
            }
            itemCount = _materials.Count;
        }
        for (int i = 0; i < itemCount; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.transform.SetParent(scrollRect.content);
            s.transform.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            _s.initStock(_materials[i] , SDConstants.StockType.material , mType);
            items.Add(_s);
        }
    }
    public void showPropsOwned()
    {
        ResetPage();
        List<GDEItemData> props = SDDataManager.Instance.PlayerData.props;
        itemCount = props.Count;
        for(int i = 0; i < itemCount; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.transform.SetParent(scrollRect.content);
            s.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            _s.initStock(props[i], SDConstants.StockType.prop);
            items.Add(_s);
        }
    }
    public bool checkIfCanWork(RTSingleStockItem newStock)
    {
        List<RTSingleStockItem> list = new List<RTSingleStockItem>();
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].isSelected) list.Add(items[i]);
        }
        if(materialType == SDConstants.MaterialType.exp)
        {
            return heroImproveController.expectImprove_before(list, SDHeroImprove.ImproveKind.exp, newStock);
        }
        else if(materialType == SDConstants.MaterialType.star)
        {
            return heroImproveController.expectImprove_before(list, SDHeroImprove.ImproveKind.star, newStock);
        }
        else if (materialType == SDConstants.MaterialType.skill)
        {
            return heroImproveController.expectImprove_before(list, SDHeroImprove.ImproveKind.skill,newStock);
        }
        else if(materialType == SDConstants.MaterialType.likability)
        {
            return heroImproveController.expectImprove_before(list, SDHeroImprove.ImproveKind.likability, newStock);
        }
        else if(materialType == SDConstants.MaterialType.equip_exp)
        {
            return equipImproveController.expectImprove_before(list, SDEquipImprove.ImproveKind.exp, newStock);
        }
        else if(materialType == SDConstants.MaterialType.equip_fix)
        {
            return equipImproveController.expectImprove_before(list, SDEquipImprove.ImproveKind.fix, newStock);
        }
        return false;
    }
    #endregion
}
