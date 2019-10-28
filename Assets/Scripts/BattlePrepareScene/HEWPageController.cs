using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

/// <summary>
/// 通用英雄、装备、武器选择项控制类
/// </summary>
public class HEWPageController : MonoBehaviour
{
    public Transform SItem;
    public List<SingleItem> items = new List<SingleItem>();
    public Job jobType = Job.End;
    public SDConstants.ItemType CurrentType = SDConstants.ItemType.End;
    //public EquipPosition EquipPos;
    public int pageIndex;
    public int itemCount;
    //
    public SDEquipSelect equipSelect;
    public ScrollRect scrollRect;
    private int singleHeroHeight = 250;
    private int singleEquipHeight = 250;

    public void ResetPage()
    {
        for(int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
        }
        items = new List<SingleItem>();
        scrollRectReset();
    }
    public void scrollRectReset()
    {
        scrollRect.content.anchoredPosition = Vector2.zero;

    }
    public void ItemsInit(SDConstants.ItemType type)
    {
        Debug.Log("不同类型列表会进行刷新");
        itemCount = 0;
        CurrentType = type;
        if (type == SDConstants.ItemType.Hero)
        {
            SDConstants.HeroSelectType st = SDGameManager.Instance.heroSelectType;
            if (st == SDConstants.HeroSelectType.All)
            {
                ShowHeroesOwned();
            }
            else if (st == SDConstants.HeroSelectType.Battle)
            {
                ShowHeroesOwnedToBattle();
            }
            else if (st == SDConstants.HeroSelectType.Dispatch)
            {

            }
            else if (st == SDConstants.HeroSelectType.Mission)
            {

            }
            else if (st == SDConstants.HeroSelectType.Recruit)
            {

            }
            else if (st == SDConstants.HeroSelectType.UseProp)
            {

            }
            else if (st == SDConstants.HeroSelectType.Train)
            {

            }
            else if (st == SDConstants.HeroSelectType.TrainConsume)
            {

            }
            else if (st == SDConstants.HeroSelectType.Promote)
            {

            }
            else if (st == SDConstants.HeroSelectType.PromoteConsume)
            {

            }
            else if (st == SDConstants.HeroSelectType.StarUp)
            {

            }
            else if (st == SDConstants.HeroSelectType.Wake)
            {

            }
            else if (st == SDConstants.HeroSelectType.Replace)
            {

            }
        }
        else if (type == SDConstants.ItemType.Helmet)
        {
            showOnePosEquipsOwned(EquipPosition.Head);
        }
        else if (type == SDConstants.ItemType.Breastplate)
        {
            showOnePosEquipsOwned(EquipPosition.Breast);
        }
        else if (type == SDConstants.ItemType.Gardebras)
        {
            showOnePosEquipsOwned(EquipPosition.Arm);
        }
        else if (type == SDConstants.ItemType.Legging)
        {
            showOnePosEquipsOwned(EquipPosition.Leg);
        }
        else if (type == SDConstants.ItemType.Jewelry)
        {
            showOnePosEquipsOwned(EquipPosition.Finger);
        }
        else if (type == SDConstants.ItemType.Weapon)
        {
            showOnePosEquipsOwned(EquipPosition.Hand);
        }
        else if (type == SDConstants.ItemType.AllEquip)
        {
            showAllEquipsOwned();
        }
    }
    #region itemInitContent
    public void ShowHeroesOwned()
    {
        ResetPage();
        List<GDEHeroData> heroes = SDDataManager.Instance.PlayerData.herosOwned;
        //int index = 0;
        itemCount = heroes.Count;
        int maxHeroNum = SDConstants.EquipNumPerPage;
        for(int i = 0; i < itemCount; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.transform.SetParent(scrollRect.content);
            s.transform.localScale = Vector3.one;
            s.gameObject.SetActive(true);
            SingleItem _s = s.GetComponent<SingleItem>();
            _s.sourceController = this;
            _s.initHero(heroes[i]);
            items.Add(_s);
        }
    }
    public void ShowHeroesOwnedToBattle()
    {
        ResetPage();

        List<GDEHeroData> heroes = SDDataManager.Instance.PlayerData.herosOwned;
        itemCount = heroes.Count;
        //if (itemCount <= 0) equipSelect.refreshEmptyEquipPanel(true);
        //else equipSelect.refreshEmptyEquipPanel(false);
        for (int i = 0; i < itemCount; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.transform.SetParent(scrollRect.content);
            s.transform.localScale = Vector3.one;
            s.gameObject.SetActive(true);
            SingleItem _s = s.GetComponent<SingleItem>();
            _s.initBattleHero(heroes[i]);
            _s.sourceController = this;

            if (heroes[i].status == 1)
            {
                _s.isSelected = true;
            }
            else
            {
                _s.isSelected = false;
            }

            items.Add(_s);
        }
    }
    public void showAllEquipsOwned()
    {
        List<GDEEquipmentData> allEquips = SDDataManager.Instance.getAllOwnedEquips();
        //int heroHashcode = SDDataManager.Instance.getherohash
        itemCount = allEquips.Count;
        if (itemCount <= 0) equipSelect.refreshEmptyEquipPanel(true);
        else equipSelect.refreshEmptyEquipPanel(false);
        for(int i = 0; i < itemCount; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.transform.SetParent(scrollRect.content);
            s.transform.localScale = Vector3.one;
            s.gameObject.SetActive(true);
            SingleItem _s = s.GetComponent<SingleItem>();
            //_s.initEquip(allEquips[i].equipId, allEquips[i].upLv);
            _s.sourceController = this;
            _s.initEquip(allEquips[i]);
            items.Add(_s);
        }
    }
    public void showOnePosEquipsOwned(EquipPosition pos)
    {
        ResetPage();
        List<GDEEquipmentData> allEquips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (pos, jobType);
        itemCount = allEquips.Count;
        //if (itemCount <= 0) equipSelect.refreshEmptyEquipPanel(true);
        //else equipSelect.refreshEmptyEquipPanel(false);
        for (int i = 0; i < itemCount; i++)
        {
            Transform s = Instantiate(SItem) as Transform;
            s.transform.SetParent(scrollRect.content);
            s.transform.localScale = Vector3.one;
            SingleItem _s = s.GetComponent<SingleItem>();
            //_s.initEquip(allEquips[i].equipId, allEquips[i].upLv);
            _s.sourceController = this;
            _s.initEquip(allEquips[i]);
            items.Add(_s);
        }
    }
    #endregion
}
