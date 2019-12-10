using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class DepositoryPanel : BasicSubMenuPanel
{
    //public StockPageController stockPage;
    public HEWPageController page;
    public enum aboveBtnKind
    {
        material,prop,all
    }
    public aboveBtnKind currentBtnKind = aboveBtnKind.material;
    public SDConstants.StockUseType currentStockUseType = SDConstants.StockUseType.detail;
    public BagController BAG;
    [Header("选中物件详细信息显示")]
    public Button ResolveBtn;
    public Transform ItemDetailPanel;
    public string currentItemId;
    public SDConstants.ItemType currentItemType;
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        BAG.gameObject.SetActive(false);
        resetThisPanel();
        page.ItemsInit(SDConstants.ItemType.Material);
    }
    public void resetThisPanel()
    {
        SDGameManager.Instance.stockUseType = currentStockUseType;
    }
    public void btnToMaterial()
    {
        if(currentBtnKind!= aboveBtnKind.material)
        {
            currentBtnKind = aboveBtnKind.material;
            BAG.gameObject.SetActive(false);
            resetThisPanel();
            page.ItemsInit(SDConstants.ItemType.Material);
        }
    }
    public void btnToProp()
    {
        if(currentBtnKind != aboveBtnKind.prop)
        {
            currentBtnKind = aboveBtnKind.prop;
            BAG.gameObject.SetActive(true);
            BAG.InitBag(BagController.useType.change);
            resetThisPanel();
            page.ItemsInit(SDConstants.ItemType.Prop);
        }
    }
    public override void commonBackAction()
    {
        base.commonBackAction();
        page.ResetPage();
        homeScene.SubMenuClose();
    }

    /// <summary>
    /// 材料效果释放
    /// </summary>
    public void BtnToMakeItemResolve()
    {
        ROMaterialData A = SDDataManager.Instance.getMaterialDataById(currentItemId);
        //判断材料功用
        if (A.materialType == SDConstants.MaterialType.equip_reap.ToString())
        {
            if (SDDataManager.Instance.UseChestToGetEquip(A, out List<string> result))
            {
                if (result.Count > 0)
                {
                    string get = "获得 新装备id" + result[0];
                    for(int i = 1; i < result.Count; i++)
                    {
                        get += " ||新装备id" + result[i];
                    }
                    Debug.Log(get);
                }
            }
        }
        else if(A.materialType == SDConstants.MaterialType.prop_reap.ToString())
        {
            if(SDDataManager.Instance.UseChestToGetProp(A,out List<GDEItemData> result))
            {
                if(result.Count > 0)
                {
                    string get = "获得 新道具id" + result[0].id +" +" + result[0].num +"个";
                    for (int i = 1; i < result.Count; i++) 
                    {
                        get += " ||新道具id" + result[i].id + " +" + result[i].num + "个";
                    }
                    Debug.Log(get);
                }
            }
        }

        WhenItemResolveFinish();
    }

    public void WhenItemResolveFinish()
    {
        float V = page.scrollRect.verticalNormalizedPosition;

        if (currentBtnKind == aboveBtnKind.material)
            page.ItemsInit(SDConstants.ItemType.Material);
        else if (currentBtnKind == aboveBtnKind.prop)
            page.ItemsInit(SDConstants.ItemType.Prop);
        for(int i = 0; i < page.itemCount; i++)
        {
            if(page.items[i].itemId == currentItemId)
            {
                page.items[i].isSelected = true;
            }
            else
            {
                page.items[i].isSelected = false;
            }
        }
        page.scrollRect.verticalNormalizedPosition = V;
    }
    public void showCurrentItemDetail(string id, int num)
    {
        currentItemId = id;
        currentItemType = SDDataManager.Instance.getItemTypeById(id);
        if (currentItemType == SDConstants.ItemType.Material)
        {
            ResolveBtn.gameObject.SetActive(true);
        }
        else if (currentItemType == SDConstants.ItemType.Prop)
        {
            ResolveBtn.gameObject.SetActive(false);
        }

    }
}
