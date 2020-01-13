using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipDetailPanel : MonoBehaviour
{
    public enum subPanel
    {
        infor,lvUp,fix,end
    }
    subPanel _csp = subPanel.end;
    public subPanel currentSubPanel
    {
        get { return _csp; }
        set
        {
            if (_csp != value)
            {
                UIEffectManager.Instance.showAnimFadeIn(equipImprove.transform);

                refreshPanel(_csp, value);
                _csp = value;
            }
            else
            {
                UIEffectManager.Instance.hideAnimFadeOut(equipImprove.transform);
            }
        }
    }
    public SDEquipDetail equipDetail;
    public Transform inforPanel;
    public SDEquipImprove equipImprove;
    [Space]
    public Transform EmptyPanel;

    public void btnToInfor()
    {
        currentSubPanel = subPanel.infor;
    }
    public void btnToLvUp()
    {
        currentSubPanel = subPanel.lvUp;
        equipImprove.currentImproveKind = SDEquipImprove.ImproveKind.exp;
        equipImprove.InitImprovePanel();
    }
    public void btnToFix()
    {
        currentSubPanel = subPanel.fix;
        equipImprove.currentImproveKind = SDEquipImprove.ImproveKind.fix;
        equipImprove.InitImprovePanel();
    }
    public void refreshPanel(subPanel oldPanel, subPanel newPanel)
    {
        if(newPanel == subPanel.infor)
        {
            UIEffectManager.Instance.showAnimFadeIn(inforPanel);
            UIEffectManager.Instance.hideAnimFadeOut(equipImprove.transform);
        }
        else if (oldPanel == subPanel.infor)
        {
            UIEffectManager.Instance.hideAnimFadeOut(inforPanel);
            UIEffectManager.Instance.showAnimFadeIn(equipImprove.transform);
        }
    }


    public void whenOpenThisPanel()
    {
        equipImprove.transform.localScale = Vector3.zero;
        EmptyPanel.gameObject.SetActive(true);
    }
    public void whenCloseThisPanel()
    {
        equipImprove.CloseThisPanel();
    }
}
