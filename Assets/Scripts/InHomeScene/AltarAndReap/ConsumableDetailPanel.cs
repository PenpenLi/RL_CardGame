using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class ConsumableDetailPanel : ItemDetailPanel
{
    [Space]
    public ItemStarVision starVision;
    public ScopeInt CurrentSelectedScope;
    public Slider SelectedNumSlider;
    public Text minNumText;
    public Text maxNumText;
    public Text currentNumText;
    public MaterialResolveList MaterialResolve
    {
        get { return GetComponent<MaterialResolveList>(); }
    }
    public DepositoryPanel DP;
    public ConsumableDetailPanel() { itemType = SDConstants.ItemType.Consumable; }
    public override void BtnTappped()
    {
        base.BtnTappped();
        MakeConsumableResolve();
    }
    public void MakeConsumableResolve()
    {
        consumableItem A = SDDataManager.Instance.getConsumableItemById(id);
        //判断材料功用
        if (A.MaterialType == SDConstants.MaterialType.equip_reap)
        {
            if (MaterialResolve.Use__equip_reap(A.ID, out string result))
            {
                string get = "获得 新装备id" + result[0];
                Debug.Log(get);
                SDDataManager.Instance.addEquip(result);
            }
        }
        else if (A.MaterialType == SDConstants.MaterialType.money)
        {

        }

        DP.RefreshPanelPage();
        RefreshSliderCondition();
    }
    public void initDetailPanel(string id)
    {
        GDEItemData d = SDDataManager.Instance.getConsumeableDataById(id);
        if (d == null) return;
        this.id = id; hashcode = 0;
        consumableItem item = SDDataManager.Instance.getConsumableById(id);
        if (!item) return;
        if (itemNameText) itemNameText.text = item.NAME;
        //if(itemExtraText) itemExtraText.text = 
        if (itemDescText) itemDescText.text = item.DESC;
        if (btnToResolve) btnToResolve.gameObject.SetActive(!item.isProp);
        starVision.StarNum = item.LEVEL;
        minNumText.text = "" + 1;
        maxNumText.text = "" + d.num;
        CurrentSelectedScope.Min = 1;
        CurrentSelectedScope.Max = d.num;
        //
        RefreshSliderCondition();
    }
    public void OnSliderChanging()
    {
        CurrentSelectedScope.Current = (int)SelectedNumSlider.value;
        currentNumText.text = "" + CurrentSelectedScope.Current;
    }
    public void RefreshSliderCondition()
    {
        GDEItemData d = SDDataManager.Instance.getConsumeableDataById(id);
        SelectedNumSlider.minValue = 1;
        SelectedNumSlider.value = SelectedNumSlider.minValue;
        SelectedNumSlider.maxValue = d.num;
        currentNumText.text = "" + SelectedNumSlider.value;
    }
}
