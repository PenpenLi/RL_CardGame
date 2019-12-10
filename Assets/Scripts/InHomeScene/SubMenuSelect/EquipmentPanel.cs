using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentPanel : BasicSubMenuPanel
{
    public HEWPageController PAGE;
    //public EquipDetailPanel EDP;
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        initAllEquipsOwned();
    }
    public override void commonBackAction()
    {
        base.commonBackAction();
        homeScene.SubMenuClose();
    }


    public void initAllEquipsOwned()
    {
        SDGameManager.Instance.stockUseType = SDConstants.StockUseType.detail;
        PAGE.ItemsInit(SDConstants.ItemType.Equip);
    }
}
