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
        material, prop, all
    }
    public Button[] aboveBtns = new Button[(int)aboveBtnKind.all];
    public aboveBtnKind currentBtnKind = aboveBtnKind.material;
    public SDConstants.StockUseType currentStockUseType = SDConstants.StockUseType.detail;
    public BagController BAG;
    [Header("选中物件详细信息显示")]
    public Button ResolveBtn;
    public Transform ItemDetailPanel;
    public MaterialResolveList MaterialResolve
    {
        get { return ItemDetailPanel.GetComponent<MaterialResolveList>(); }
    }
    public ConsumableDetailPanel CDP
    {
        get { return ItemDetailPanel.GetComponent<ConsumableDetailPanel>(); }
    }

    public SDConstants.ItemType currentItemType;

    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        currentBtnKind = aboveBtnKind.material;
        BAG.gameObject.SetActive(false);
        resetThisPanel();
        page.ItemsInit(SDConstants.ItemType.Consumable, SDConstants.ConsumableType.material);
        if (page.items.Count > 0)
        {
            page.items[0].chooseConsumableToShowDetail();
        }
    }
    public void resetThisPanel()
    {
        SDGameManager.Instance.stockUseType = currentStockUseType;
    }
    #region AbovePanelFunction
    public void btnToMaterial()
    {
        if (currentBtnKind != aboveBtnKind.material)
        {
            currentBtnKind = aboveBtnKind.material;
            foreach(Button b in aboveBtns)
            {
                b.interactable = true;
            }
            aboveBtns[(int)currentBtnKind].interactable = false;
            BAG.gameObject.SetActive(false);
            resetThisPanel();
            page.ItemsInit(SDConstants.ItemType.Consumable, SDConstants.ConsumableType.material);
        }
    }
    public void btnToProp()
    {
        if (currentBtnKind != aboveBtnKind.prop)
        {
            currentBtnKind = aboveBtnKind.prop;
            foreach (Button b in aboveBtns)
            {
                b.interactable = true;
            }
            aboveBtns[(int)currentBtnKind].interactable = false;
            BAG.gameObject.SetActive(true);
            BAG.InitBag(BagController.useType.change);
            resetThisPanel();
            page.ItemsInit(SDConstants.ItemType.Consumable, SDConstants.ConsumableType.prop);
        }
    }
    #endregion
    public override void commonBackAction()
    {
        base.commonBackAction();
        page.ResetPage();
        homeScene.SubMenuClose();
    }

    /// <summary>
    /// 材料效果释放
    /// </summary>

    public void RefreshPanelPage()
    {
        float V = page.scrollRect.verticalNormalizedPosition;

        if (currentBtnKind == aboveBtnKind.material)
        {
            page.ItemsInit(SDConstants.ItemType.Consumable, SDConstants.ConsumableType.material);
        }
        else if (currentBtnKind == aboveBtnKind.prop)
        {
            page.ItemsInit(SDConstants.ItemType.Consumable, SDConstants.ConsumableType.prop);
        }
        for (int i = 0; i < page.itemCount; i++)
        {
            if (page.items[i].itemId == CDP.id)
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
        CDP.id = id;
        consumableItem item = SDDataManager.Instance.getConsumableById(id);
        currentItemType = item.ItemType;
        if (currentItemType == SDConstants.ItemType.Consumable)
        {
            ResolveBtn.gameObject.SetActive(!item.isProp);
        }
        CDP.initDetailPanel(id);

        //
        if (item.MaterialType == SDConstants.MaterialType.equip_reap)
        {
            CDP.SelectedNumSlider.gameObject.SetActive(true);
            ResolveBtn.gameObject.SetActive(true);
            //
            ResolveBtn.GetComponentInChildren<Text>().text
                = SDGameManager.T("解锁");
        }
        else if (item.MaterialType == SDConstants.MaterialType.treasure
            || item.MaterialType == SDConstants.MaterialType.key
            || item.MaterialType == SDConstants.MaterialType.goddess_piece
            || item.MaterialType == SDConstants.MaterialType.end)
        {
            CDP.SelectedNumSlider.gameObject.SetActive(false);
            ResolveBtn.gameObject.SetActive(false);
        }
        else
        {
            CDP.SelectedNumSlider.gameObject.SetActive(true);
            ResolveBtn.gameObject.SetActive(true);
            //
            ResolveBtn.GetComponentInChildren<Text>().text
                = SDGameManager.T("出售");
        }
    }
}
