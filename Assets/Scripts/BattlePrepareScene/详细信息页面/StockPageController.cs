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
    public SDHeroImprove heroImproveController;
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
    }
    public void ItemInit(SDConstants.MaterialType MType)
    {
        List<GDEHeroData> heroes = SDDataManager.Instance.PlayerData.herosOwned;
        List<GDEAMaterialData> all = SDDataManager.Instance.PlayerData.materials;

        //对应材料构建
        for (int i = 0; i < all.Count; i++)
        {
            if(MType != SDConstants.MaterialType.all)
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
                    _s.initStock(all[i],SDConstants.StockType.material,MType);
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
        int heroId = heroImproveController.heroDetail.Id;
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
        itemCount = items.Count;

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
        List<GDEAMaterialData> all = SDDataManager.Instance.PlayerData.materials;
        List<GDEAMaterialData> _materials = new List<GDEAMaterialData>();
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
        }
    }
    public void showPropsOwned()
    {
        ResetPage();
        List<GDEAMaterialData> props = SDDataManager.Instance.PlayerData.props;
        itemCount = props.Count;
        for(int i = 0; i < itemCount; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.transform.SetParent(scrollRect.content);
            s.localScale = Vector3.one;
            RTSingleStockItem _s = s.GetComponent<RTSingleStockItem>();
            _s.stockPage = this;
            //_s.initStock()
        }
    }
}
