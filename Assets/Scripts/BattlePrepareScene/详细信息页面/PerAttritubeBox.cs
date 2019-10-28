using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PerAttritubeBox : MonoBehaviour
{
    public Text NameText;
    public Image ICON;
    public Text BasicFigureText;
    public Text ExtraFigureText;
    public Transform BasicFigureSlider;
    public Transform ExtraFigureSlider;
    public OneAttritube DATA = new OneAttritube();
    public int extraData;
    [HideInInspector]
    public RectTransform leftPanel;
    [HideInInspector]
    public RectTransform basicPanel;
    private float LeftLength = 75;
    private float AnimTime = 0.2f;
    [HideInInspector]
    public bool isShowingNameText;
    // Start is called before the first frame update
    void Start()
    {
        leftPanel = transform.GetChild(0).GetComponent<RectTransform>();
        basicPanel = transform.GetChild(1).GetComponent<RectTransform>();
    }
    #region AD
    public void initThisBoxInAD(int basicRa, int extraRa,AttributeData tag)
    {
        DATA.isAD = true;DATA.index = (int)tag;
        NameText.text = SDGameManager.T(tag.ToString());
        RefreshData_AD(basicRa, extraRa);
    }

    public void RefreshData_AD(int basicRA,int extraRA)
    {
        BasicFigureText.text = basicRA + " ";
        ExtraFigureText.text = "+" + extraRA;
        BasicFigureSlider.localScale = new Vector3
            (Mathf.Max(0, basicRA) * 1f / maxFigure_AD(), 1, 1);
        ExtraFigureSlider.localScale = new Vector3
            (Mathf.Max(0, extraRA) * 1f / maxFigure_AD(), 1, 1);
        DATA.figure = basicRA;
        extraData = extraRA;
    }
    public IEnumerator DetailVisionAnim(bool detailIsOn)
    {
        if (detailIsOn)
        {
            leftPanel.DOSizeDelta(new Vector2(LeftLength, leftPanel.sizeDelta.y), AnimTime);
            basicPanel.DOAnchorPos(Vector2.right * LeftLength, AnimTime);
        }
        else
        {
            leftPanel.DOSizeDelta(new Vector2(0, leftPanel.sizeDelta.y), AnimTime);
            basicPanel.DOAnchorPos(Vector2.zero, AnimTime);
        }
        yield return new WaitForSeconds(AnimTime);
    }
    public void AnimController(bool clickTrigger)
    {
        if (isShowingNameText != clickTrigger)
        {
            isShowingNameText = clickTrigger;
            StartCoroutine(DetailVisionAnim(isShowingNameText));
        }
    }
    public int maxFigure_AD()
    {
        int lv = 0;
        AttritubeListPanel ALP = GetComponentInParent<AttritubeListPanel>();
        if (ALP) lv = ALP.currentLv;
        return SDDataManager.Instance.getRoleRAMaxNumPerLv((AttributeData)DATA.index, lv);
    }
    #endregion
    #region SR
    public void initThisBoxInSR(int basicRa,int extraRa, StateTag tag)
    {
        DATA.isAD = false;DATA.index = (int)tag;
        NameText.text = SDGameManager.T(tag.ToString());
        RefreshData_SR(basicRa, extraRa);
    }
    public void RefreshData_SR(int basicRA, int extraRA)
    {
        BasicFigureText.text = basicRA + "";
        ExtraFigureText.text = "+" + extraRA;
        BasicFigureSlider.localScale = new Vector3
            (1, Mathf.Max(0, basicRA) * 1f / maxFigure_SR(), 1);
        ExtraFigureSlider.localScale = new Vector3
            (1, Mathf.Max(0, extraRA) * 1f / maxFigure_SR(), 1);
        DATA.figure = basicRA;
        extraData = extraRA;
    }
    public int maxFigure_SR()
    {
        int lv = 0;
        AttritubeListPanel ALP = GetComponentInParent<AttritubeListPanel>();
        if (ALP) lv = ALP.currentLv;
        return SDDataManager.Instance.getRoleSRMaxNumPerLv(lv);
    }
    #endregion

}
