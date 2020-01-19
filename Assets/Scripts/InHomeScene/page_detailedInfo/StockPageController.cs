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
    public SDConstants.MaterialType materialType = SDConstants.MaterialType.end;
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
    public void ItemsInit(SDConstants.StockType SType, SDConstants.MaterialType MType = SDConstants.MaterialType.end)
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
    public void ItemInitForHero(SDConstants.MaterialType MType)
    {
        #region 英雄强化用
        List<GDEHeroData> heroes = SDDataManager.Instance.PlayerData.herosOwned;
        List<GDEItemData> all = SDDataManager.Instance.getMaterialsOwned;

        materialType = MType;
        //对应材料构建
        if (!SDDataManager.Instance.checkHeroCanImprove
                        (heroImproveController.heroDetail.Hashcode, MType))
            return;
        ROHeroData ro = SDDataManager.Instance.getHeroOriginalDataById(heroImproveController.heroDetail.ID);
        GDEHeroData gd = SDDataManager.Instance.GetHeroOwnedByHashcode(heroImproveController.heroDetail.Hashcode);
        for (int i = 0; i < all.Count; i++)
        {
            if (MType != SDConstants.MaterialType.end)
            {
                if (SDDataManager.Instance.getMaterialTypeById(all[i].id) == MType)
                {
                    if(MType == SDConstants.MaterialType.skill)
                    {
                        string str = SDDataManager.Instance.getMaterialSpecialStr(heroImproveController.heroDetail.ID);
                        str = str.Split('_')[0];
                        Job str_career = ROHelp.getJobByString(str);
                        Job career = ro.Info.Career.Career;
                        bool flag = str_career == career;
                        if (str.Contains("any")) flag = true;
                        if (!flag) continue;
                    }

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
                SDConstants.MaterialType mt = SDDataManager.Instance.getMaterialTypeById(all[i].id);
                if (mt == SDConstants.MaterialType.exp
                    || mt == SDConstants.MaterialType.skill
                    || mt == SDConstants.MaterialType.star)
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
    public void showMaterialsOwned(SDConstants.MaterialType mType = SDConstants.MaterialType.end)
    {
        ResetPage();
        List<GDEItemData> all = SDDataManager.Instance.getMaterialsOwned;
        List<GDEItemData> _materials = new List<GDEItemData>();
        if (mType == SDConstants.MaterialType.end)
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
                SDConstants.MaterialType _mtype = SDDataManager.Instance.getMaterialTypeById(all[i].id);
                if (_mtype == mType)
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
        List<GDEItemData> props = SDDataManager.Instance.getPropsOwned;
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
        else if(materialType == SDConstants.MaterialType.goddess_exp)
        {
            return goddessImproveController.expectImprove_before(list, SDGoddessImprove.ImproveKind.exp, newStock);
        }
        return false;
    }
    public void showMaterialsForGoddessImprove(string goddessId)
    {
        ResetPage();
        List<GDEItemData> all = SDDataManager.Instance.getMaterialsOwned;
        List<GDEItemData> results = all.FindAll(x => 
        {
            consumableItem data = SDDataManager.Instance.getConsumableById(x.id);
            if (data != null && data.MaterialType == SDConstants.MaterialType.goddess_exp)
            {
                return data.Data == goddessId || data.Data == "all";
            }
            else return false;
        });
        itemCount = results.Count;
        for(int i = 0; i < results.Count; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.transform.SetParent(scrollRect.content);
            s.transform.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            _s.initStock(results[i], SDConstants.StockType.material
                ,SDConstants.MaterialType.goddess_exp);
            items.Add(_s);
        }
    }
    #endregion
}
