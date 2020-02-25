using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using System.Linq;
using DG.Tweening;

/// <summary>
/// 材料与道具选择项控制类
/// </summary>
public class StockPageController : MonoBehaviour
{
    public Transform SItem;
    public List<RTSingleStockItem> items = new List<RTSingleStockItem>();
    public SDConstants.StockType stockType = SDConstants.StockType.material;
    public SDConstants.MaterialType materialType = SDConstants.MaterialType.end;
    public int pageIndex;
    public int itemCount;
    //
    public ScrollRect scrollRect;
    private float animInterval = 0.15f;
    #region ImprovePanel
    public SDHeroImprove heroImproveController;
    public SDEquipImprove equipImproveController;
    public SDGoddessImprove goddessImproveController;
    #endregion
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
    public bool checkIfCanWork(RTSingleStockItem item)
    {
        return true;
    }

    public void InitStocks_equipExp()
    {
        ResetPage();
        materialType = SDConstants.MaterialType.equip_exp;
        /*
        // 可用的材料
        List<GDEItemData> allIs = SDDataManager.Instance.getConsumablesOwned.FindAll
            (x =>
            {
                consumableItem item = SDDataManager.Instance.getConsumableItemById(x.id);
                if (item == null) return false;
                return item.MaterialType == SDConstants.MaterialType.equip_exp;
            });
        allIs.Sort((x, y) =>
        {
            consumableItem item_x = SDDataManager.Instance.getConsumableById(x.id);
            consumableItem item_y = SDDataManager.Instance.getConsumableById(y.id);
            return item_x.LEVEL.CompareTo(item_y.LEVEL);
        });
        for (int i = 0; i < allIs.Count; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            _s.initStock(allIs[i], SDConstants.MaterialType.equip_exp);
        }
        */
    }
    public void chooseStock(RTSingleStockItem stock)
    {
        if(materialType == SDConstants.MaterialType.exp)
        {
            heroImproveController.addStockToSlot_Exp(stock);
            refreshStockConditions_heroImprove();
        }
        else if (materialType == SDConstants.MaterialType.star)
        {
            heroImproveController.addStockToSlot_Star(stock);
            refreshStockConditions_heroImprove();
        }
        else if(materialType == SDConstants.MaterialType.skill)
        {
            heroImproveController.addStockToSlot_Skill(stock);
            refreshStockConditions_heroImprove();
        }
    }
    #region HERO_IMPROVE
    public void refreshStockConditions_heroImprove()
    {
        List<SingleSlot> enables = 
            heroImproveController.AllSlots.ToList().FindAll
            (x => !x.isEmpty && !x.isLocked);
        if(materialType == SDConstants.MaterialType.exp)
        {
            heroImproveController.refreshHeroData_exp();
        }
        else if (materialType == SDConstants.MaterialType.star)
        {
            heroImproveController.refreshHeroData_star();
        }
        else if(materialType == SDConstants.MaterialType.skill)
        {
            heroImproveController.refreshHeroData_skill();
        }

        //
        foreach(RTSingleStockItem s in items)
        {
            if (enables.Exists(x => x._id == s.itemId && x._hashcode == s.hashcode))
            {
                s.isSelected = true;
                if (s.stockType == SDConstants.StockType.material)
                {
                    int num = enables.FindAll(x => x._id == s.itemId).Count;
                    s.UsedNum = num;
                    s.NumText.text = s.UsedNum + " / " + s.itemNum;
                }
                else if(s.stockType == SDConstants.StockType.hero)
                {
                    s.UsedNum = s.itemNum;
                }
            }
            else s.isSelected = false;
        }
    }
    public void InitStocks_Exp()
    {
        ResetPage();
        materialType = SDConstants.MaterialType.exp;
        
        // 可用的英雄        
        List<GDEHeroData> allHs = SDDataManager.Instance.getHerosListOwned().FindAll
            (x =>
            {
                if (x.hashCode == heroImproveController.heroDetail.Hashcode) return false;
                HeroInfo info = SDDataManager.Instance.getHeroInfoById(x.id);
                if (info == null) return false;
                return true;
            });
        allHs.Sort((x, y) =>
        {
            HeroInfo info_x = SDDataManager.Instance.getHeroInfoById(x.id);
            HeroInfo info_y = SDDataManager.Instance.getHeroInfoById(y.id);
            if (info_x.Rarity != info_y.Rarity)
            {
                return -info_x.Rarity.CompareTo(info_y.Rarity);
            }
            return -x.exp.CompareTo(y.exp);
        });
        for (int i = 0; i < allHs.Count; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.SetParent(scrollRect.content);
            s.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            _s.initStock(allHs[i], SDConstants.MaterialType.exp);
            items.Add(_s);
        }

        // 可用的材料
        List<GDEItemData> allIs = SDDataManager.Instance.getConsumablesOwned.FindAll
            (x =>
            {
                consumableItem item = SDDataManager.Instance.getConsumableItemById(x.id);
                if (item == null) return false;
                return item.MaterialType == SDConstants.MaterialType.exp;
            });
        allIs.Sort((x, y) =>
        {
            consumableItem item_x = SDDataManager.Instance.getConsumableById(x.id);
            consumableItem item_y = SDDataManager.Instance.getConsumableById(y.id);
            return -item_x.LEVEL.CompareTo(item_y.LEVEL);
        });
        for (int i = 0; i < allIs.Count; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.SetParent(scrollRect.content);
            s.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            _s.initStock(allIs[i], SDConstants.MaterialType.exp);
            items.Add(_s);
        }
        //
        SR_ToStart();
    }
    public void InitStocks_Star(int oldLevel)
    {
        ResetPage();
        materialType = SDConstants.MaterialType.star;
        // 可用的英雄        
        List<GDEHeroData> allHs = SDDataManager.Instance.getHerosListOwned().FindAll
            (x=> 
            {
                if (x.hashCode == heroImproveController.heroDetail.Hashcode) return false;
                HeroInfo info = SDDataManager.Instance.getHeroInfoById(x.id);
                if (info == null) return false;
                int LEVEL = info.LEVEL + x.starNumUpgradeTimes;
                return LEVEL == oldLevel;
            });
        allHs.Sort((x, y) =>
        {
            HeroInfo info_x = SDDataManager.Instance.getHeroInfoById(x.id);
            HeroInfo info_y = SDDataManager.Instance.getHeroInfoById(y.id);
            if(info_x.Rarity != info_y.Rarity)
            {
                return info_x.Rarity.CompareTo(info_y.Rarity);
            }
            return x.exp.CompareTo(y.exp);
        });
        for(int i = 0; i < allHs.Count; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.SetParent(scrollRect.content);
            s.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            _s.initStock(allHs[i],SDConstants.MaterialType.star);
            items.Add(_s);
        }

        // 可用的材料
        List<GDEItemData> allIs = SDDataManager.Instance.getConsumablesOwned.FindAll
            (x =>
            {
                consumableItem item = SDDataManager.Instance.getConsumableItemById(x.id);
                if (item == null) return false;
                return item.MaterialType == SDConstants.MaterialType.star;
            });
        allIs.Sort((x, y) => 
        {
            consumableItem item_x = SDDataManager.Instance.getConsumableById(x.id);
            consumableItem item_y = SDDataManager.Instance.getConsumableById(y.id);
            return item_x.LEVEL.CompareTo(item_y.LEVEL);
        });
        for (int i = 0; i < allIs.Count; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.SetParent(scrollRect.content);
            s.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            _s.initStock(allIs[i],SDConstants.MaterialType.star);
            items.Add(_s);
        }
        //
        SR_ToStart();
    }
    public void InitStocks_skill(string heroId)
    {
        ResetPage();
        materialType = SDConstants.MaterialType.skill;

        //可用的英雄
        List<GDEHeroData> allHs = SDDataManager.Instance.getHerosListOwned().FindAll
            (x =>
            {
                if (x.hashCode == heroImproveController.heroDetail.Hashcode) return false;
                return x.id == heroId;
            });
        allHs.Sort((x, y) =>
        {
            if (x.starNumUpgradeTimes != y.starNumUpgradeTimes)
            {
                return x.starNumUpgradeTimes.CompareTo(y.starNumUpgradeTimes);
            }
            return x.exp.CompareTo(y.exp);
        });
        for (int i = 0; i < allHs.Count; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.SetParent(scrollRect.content);
            s.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            _s.initStock(allHs[i],SDConstants.MaterialType.skill);
            items.Add(_s);
        }

        //可用的材料
        Job _job = SDDataManager.Instance.getHeroCareerById(heroId);
        List<GDEItemData> allIs = SDDataManager.Instance.getConsumablesOwned.FindAll
            (x =>
            {
                consumableItem item = SDDataManager.Instance.getConsumableItemById(x.id);
                if (item == null) return false;
                Job J = SDDataManager.Instance.consumableItemSkill_FixCareer(x.id);
                bool flag = J == _job || J == Job.End;
                return item.MaterialType == SDConstants.MaterialType.skill && flag;
            });
        allIs.Sort((x, y) =>
        {
            consumableItem item_x = SDDataManager.Instance.getConsumableById(x.id);
            consumableItem item_y = SDDataManager.Instance.getConsumableById(y.id);
            return item_x.LEVEL.CompareTo(item_y.LEVEL);
        });
        for (int i = 0; i < allIs.Count; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.SetParent(scrollRect.content);
            s.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            _s.initStock(allIs[i],SDConstants.MaterialType.skill);
            items.Add(_s);
        }
        //
        SR_ToStart();
    }
    #endregion
    void SR_ToStart()
    {
        scrollRect.DOHorizontalNormalizedPos(0, animInterval);
    }
    #endregion
}
