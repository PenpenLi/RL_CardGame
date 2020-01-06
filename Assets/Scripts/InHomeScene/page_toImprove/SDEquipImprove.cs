using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;
using UnityEngine.UI;

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

        int hashcode = equipDetail.equipHashcode;
        GDEEquipmentData equip = SDDataManager.Instance.getEquipmentByHashcode(hashcode);
        if(lvText && expText && expSlider && expSlider_listorder)
        {
            int exp = equip.exp;
            int lv = SDDataManager.Instance.getLevelByExp(exp);
            lvText.text = SDGameManager.T("Lv.") + lv;
            int e0 = exp - SDDataManager.Instance.getMinExpReachLevel(lv);
            int e1 = SDDataManager.Instance.ExpBulkPerLevel(lv+1);
            expText.text = e0 + "/" + e1;
            expSlider.localScale = Vector3.up + Vector3.forward + Vector3.right * SDDataManager.Instance.getExpRateByExp(exp);

        }
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
    public override void ConsumeToImprove(List<RTSingleStockItem> list)
    {
        base.ConsumeToImprove(list);

        ImproveKind kind = currentImproveKind;
        int hashcode = equipDetail.equipHashcode;
        for(int i = 0; i < list.Count; i++)
        {
            RTSingleStockItem stock = list[i];
            if (stock.stockType == SDConstants.StockType.material)
            {
                if(kind == ImproveKind.exp 
                    && stock.materialType == SDConstants.MaterialType.equip_exp)
                {
                    int useNum = stock.UsedNum;
                    int figure = SDDataManager.Instance.getFigureFromMaterial(stock.itemId) * useNum;
                    int exp = SDDataManager.Instance.getEquipmentByHashcode(equipDetail.equipHashcode).exp;
                    int level = SDDataManager.Instance.getLevelByExp(exp + figure);
                    //
                    float rate = SDDataManager.Instance.getPossibleLvupByTargetLevel(level, out int visualRate);
                    if (UnityEngine.Random.Range(0, 1) < rate)
                    {
                        SDDataManager.Instance.addExpToEquipByHashcode(hashcode, figure);
                    }
                }
                else if(kind == ImproveKind.fix
                    && stock.materialType == SDConstants.MaterialType.equip_fix)
                {
                    int useNum = stock.UsedNum;
                    if (SDDataManager.Instance.checkEquipFixIfSuccess(hashcode))
                    {
                        foreach (GDEEquipmentData e in SDDataManager.Instance.PlayerData.equipsOwned)
                        {
                            if(e.hashcode == hashcode)
                            {
                                e.quality ++;
                                if (e.quality > SDConstants.equipMaxQuality)
                                {
                                    e.quality = SDConstants.equipMaxQuality;
                                }
                                SDDataManager.Instance.PlayerData.Set_equipsOwned();
                                break;
                            }
                        }
                    }
                }
            }
        }

        equipDetail.initEquipDetailVision(hashcode);
    }
    public bool expectImprove_before(List<RTSingleStockItem> list
        , ImproveKind kind, RTSingleStockItem newStock)
    {
        bool flag = false;
        int figure = 0;
        for(int i = 0; i < list.Count; i++)
        {
            RTSingleStockItem stock = list[i];
            if(kind == ImproveKind.exp)
            {
                figure += SDDataManager.Instance.getFigureFromMaterial(stock.itemId) * stock.UsedNum;
            }
            else if(kind == ImproveKind.fix)
            {
                figure += SDDataManager.Instance.getFigureFromMaterial(stock.itemId) * stock.UsedNum;
            }
        }
        if(kind == ImproveKind.exp)
        {
            int exp = SDDataManager.Instance.getEquipmentByHashcode(equipDetail.equipHashcode).exp;
            int level = SDDataManager.Instance.getLevelByExp(exp + figure);
            //
            float rate = SDDataManager.Instance.getPossibleLvupByTargetLevel(level, out int visualRate);
            lvUpSuccessPossibleText.text = SDGameManager.T("成功率") + ": " + visualRate + "%";
            if (rate < 0.5f)
            {
                lvUpSuccessPossibleText.color = Color.red;
            }
            else
            {
                lvUpSuccessPossibleText.color = Color.white;
            }
            if(rate>0.1f)
            {
                flag = true;
            }
        }
        else if(kind == ImproveKind.fix)
        {
            lvUpSuccessPossibleText.text = string.Empty;
            int quality = SDDataManager.Instance.getEquipQualityByHashcode(equipDetail.equipHashcode);
            if (quality < SDConstants.equipMaxQuality)
            {
                flag = true;
            }
        }



        return flag;
    }

}