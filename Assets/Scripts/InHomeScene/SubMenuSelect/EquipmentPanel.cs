using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentPanel : BasicSubMenuPanel
{
    public HEWPageController PAGE;
    public EquipDetailPanel EDP;
    [SerializeField,ReadOnly]
    private EquipPosition _currentPos = EquipPosition.Hand;
    public EquipPosition CurrentPos
    {
        get { return _currentPos; }
        set
        {
            if(_currentPos != value)
            {
                _currentPos = value;
                initEquipsOwnedByPos(_currentPos);
            }
        }
    }
    public Transform PosBtnContent;
    [HideInInspector]
    public Button[] AllPosBtns;
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        //initAllEquipsOwned();
        initEquipsOwnedByPos(EquipPosition.Hand);
        EDP.whenOpenThisPanel();
        AllPosBtns = PosBtnContent.GetComponentsInChildren<Button>();
        //
        List<EquipPosition> list = new List<EquipPosition>()
        {
            EquipPosition.Hand
        ,
            EquipPosition.Finger
            ,
            EquipPosition.Leg
            ,
            EquipPosition.Breast
            ,
            EquipPosition.Head
        };
        for(int i = 0; i < AllPosBtns.Length; i++)
        {
            EquipPosition pos = list[i];
            AllPosBtns[i].GetComponentInChildren<Text>().text
                = pos.ToString().ToUpper();
            AllPosBtns[i].onClick.AddListener(delegate ()
            {
                this.BtnToChangePos(pos);
            });
        }
    }
    public override void commonBackAction()
    {
        base.commonBackAction();
        EDP.whenCloseThisPanel();
        homeScene.SubMenuClose();
    }
    void initEquipsOwnedByPos(EquipPosition pos)
    {
        SDGameManager.Instance.stockUseType = SDConstants.StockUseType.detail;
        PAGE.ItemsInit(SDConstants.ItemType.Equip, pos);
    }
    public void BtnToChangePos(EquipPosition pos)
    {
        //Debug.Log(go.name);
        Debug.Log("切换位置为" + pos.ToString());
        CurrentPos = pos;
    }
}
