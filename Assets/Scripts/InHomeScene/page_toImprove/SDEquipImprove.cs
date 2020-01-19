using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;
using UnityEngine.UI;
using DG.Tweening;

public class SDEquipImprove : BasicImprovePage
{
    public enum ImproveKind
    {
        exp,
        fix,
        end,
    }
    ImproveKind _currentImproveKind = ImproveKind.exp;
    public ImproveKind currentImproveKind
    {
        get { return _currentImproveKind; }
        set
        {
            if (_currentImproveKind != value)
            {
                _currentImproveKind = value;
                currentImproveKindIntger = (int)_currentImproveKind;
            }
        }
    }
    [Space(50)]
    public SDEquipDetail equipDetail;
    [Header("exp_part")]
    public Text lvUpSuccessPossibleText;
    public Text lvText;
    public Text expText;
    public Transform expSlider;
    public Transform expSlider_listorder;
    public override void InitImprovePanel()
    {
        base.InitImprovePanel();
        stockPage.equipImproveController = this;
        stockInit(currentImproveKind);
    }
    public void stockInit(ImproveKind improveKind)
    {
        stockPage.pageIndex = 0;
        SDConstants.MaterialType MType = SDConstants.MaterialType.equip_exp;
        if (improveKind == ImproveKind.exp)
        {
            MType = SDConstants.MaterialType.equip_exp;
            maxSelectedNum = 10;
        }
        else if(improveKind == ImproveKind.fix)
        {
            MType = SDConstants.MaterialType.equip_fix;
            maxSelectedNum = 1;
        }

        currentImproveKind = improveKind;
        stockPage.ResetPage();
        stockPage.maxSelectedNum = maxSelectedNum;
        stockPage.ItemsInit(SDConstants.StockType.material ,MType);
        stockPage.SelectEmpty();
        RefreshImprovePanel();
    }
    public override void RefreshImprovePanel()
    {
        base.RefreshImprovePanel();

        int hashcode = equipDetail.equipHashcode;
        GDEEquipmentData equip = SDDataManager.Instance.getEquipmentByHashcode(hashcode);
        if (lvText && expText && expSlider && expSlider_listorder)
        {
            int lv = equip.lv;
            lvText.text = SDGameManager.T("Lv.") + lv;
        }
    }
    public override void ConsumeToImprove(List<RTSingleStockItem> list)
    {
        base.ConsumeToImprove(list);

        ImproveKind kind = currentImproveKind;
        int hashcode = equipDetail.equipHashcode;
        for(int i = 0; i < list.Count; i++)
        {
            RTSingleStockItem stock = list[i];

        }

        equipDetail.initEquipDetailVision(hashcode);
    }
}