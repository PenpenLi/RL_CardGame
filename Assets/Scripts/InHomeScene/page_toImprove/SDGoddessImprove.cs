using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SDGoddessImprove : BasicImprovePage
{
    public enum ImproveKind
    {
        exp,star,end,
    }
    ImproveKind _cik = ImproveKind.exp;
    public ImproveKind currentImproveKind
    {
        get { return _cik; }
        set
        {
            if (_cik != value)
            {
                _cik = value;
                currentImproveKindIntger = (int)_cik;
            }
        }
    }
    [Space(50)]
    public SDGoddesDetail goddessDetail;
    public Transform emptyPanel;
    [Header("exp_part")]
    public Text lvText;
    public Text expText;
    public Transform expSlider;
    public Transform expSlider_listorder;

    public override void InitImprovePanel()
    {
        base.InitImprovePanel();

        //int hashcode = 
        string id = goddessDetail.Id;
        GDEgoddessData goddess = SDDataManager.Instance.getGDEGoddessDataById(id);

        stockPage.goddessImproveController = this;
        stockInit(ImproveKind.exp);
    }

    public void stockInit(ImproveKind improveKind)
    {
        stockPage.pageIndex = 0;
        SDConstants.MaterialType MType = SDConstants.MaterialType.goddess_exp;
        if(improveKind == ImproveKind.exp)
        {
            maxSelectedNum = 10;
        }
        else if(improveKind == ImproveKind.star)
        {
            MType = SDConstants.MaterialType.goddess_exp;
            maxSelectedNum = 3;
        }

        stockPage.materialType = MType;

        stockPage.maxSelectedNum = maxSelectedNum;
        //stockPage.ItemsInit(SDConstants.StockType.material,MType);
        //stockPage.showMaterialsForGoddessImprove(goddessDetail.Id);
        stockPage.SelectEmpty();
        RefreshImprovePanel();
        if (stockPage.items.Count == 0) emptyPanel.gameObject.SetActive(true);
        else emptyPanel.gameObject.SetActive(false);
    }

    public bool expectImprove_before(List<RTSingleStockItem> list,ImproveKind kind, RTSingleStockItem newStock)
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
            else if(kind == ImproveKind.star)
            {
                figure += SDDataManager.Instance.getFigureFromMaterial(stock.itemId) * stock.UsedNum;
            }
        }
        if(kind == ImproveKind.exp)
        {
            flag = true;
        }
        else if(kind == ImproveKind.star)
        {

        }


        return flag;
    }


    public override void ConsumeToImprove(List<RTSingleStockItem> list)
    {
        base.ConsumeToImprove(list);
        string id = goddessDetail.Id;
        int figure = 0;
        ImproveKind kind = currentImproveKind;
        for (int i = 0; i < list.Count; i++)
        {
            RTSingleStockItem stock = list[i];
            if (kind == ImproveKind.exp)
            {
                if(SDDataManager.Instance.consumeConsumable(stock.itemId,out int leave, stock.UsedNum))
                    figure += SDDataManager.Instance.getFigureFromMaterial(stock.itemId) 
                        * stock.UsedNum;
            }
            else if (kind == ImproveKind.star)
            {
                if(SDDataManager.Instance.consumeConsumable(stock.itemId,out int leave,stock.UsedNum))
                    figure += SDDataManager.Instance.getFigureFromMaterial(stock.itemId) 
                        * stock.UsedNum;
            }
        }
        if(kind == ImproveKind.exp)
        {
            SDDataManager.Instance.addExpToGoddess(id , figure);
        }



        //
        goddessDetail.initgoddessDetailVision(SDDataManager.Instance.getGDEGoddessDataById(id));
    }
}
