using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class BasicImprovePage : MonoBehaviour
{
    public int currentImproveKindIntger;
    [Header("UseItemsPanel")]
    public Transform animPlace;
    public StockPageController stockPage;
    public int maxSelectedNum;
    public Transform[] AllImproveTrans;
    public virtual void InitImprovePanel()
    {
        //当前所有材料点击功效改为“使用”
        SDGameManager.Instance.stockUseType = SDConstants.StockUseType.work;
    }
    public virtual void CloseThisPanel()
    {
        stockPage.ResetPage();
    }
    public virtual void RefreshImprovePanel()
    {
        for (int i = 0; i < AllImproveTrans.Length; i++)
        {
            if (currentImproveKindIntger == i)
            {
                AllImproveTrans[i]?.gameObject.SetActive(true);
            }
            else
            {
                AllImproveTrans[i]?.gameObject.SetActive(false);
            }
        }
    }
    public virtual void BtnToCancelImprove()
    {
        stockPage.SelectEmpty();
    }
    public virtual void BtnToConfirmImprove()
    {
        List<RTSingleStockItem> list = new List<RTSingleStockItem>();
        for (int i = 0; i < stockPage.items.Count; i++)
        {
            if (stockPage.items[i].isSelected) list.Add(stockPage.items[i]);
        }
        if (list.Count > 0)
        {
            ConsumeToImprove(list);
            //
            InitImprovePanel();
        }
    }
    public virtual void ConsumeToImprove(List<RTSingleStockItem> list)
    {

    }
}
