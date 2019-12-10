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

    }
}
