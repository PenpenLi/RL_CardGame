using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AttritubeListPanel : MonoBehaviour
{
    public RectTransform TitlePanel;
    public Text TitleText;
    public RectTransform ADList;
    public PerAttritubeBox[] Boxes;
    public RectTransform MoreADList;
    public PerAttritubeBox[] MoreBoxes;
    public RectTransform SDList;
    public PerAttritubeBox[] StateBoxes;
    [Space(15)]
    //public BattleRoleData BRD;
    //public BasicRoleProperty BRP;
    public RoleAttributeList currentBasicRAL;
    public RoleAttributeList currentExtraRAL;

    public int CurrentHashCode;
    public SDConstants.CharacterType _type;
    public bool ShowEllipsisData;
    private float ListExpandTime=0.25f;
    private float littledelay = 0.05f;
    [HideInInspector]
    public int currentLv;
    private void Start()
    {
        MoreADList.localScale = Vector3.right + Vector3.forward;
    }
    public void initRAL(RoleAttributeList _currentBasicRAL
        , RoleAttributeList _extraRAL
        , SDConstants.CharacterType CType, int lv = 0)
    {
        currentBasicRAL = _currentBasicRAL;
        currentExtraRAL = _extraRAL;
        _type = CType;
        currentLv = lv;
        initRAL_AD_Panel();
        if (_type != SDConstants.CharacterType.Goddess)
        {
            initRAL_SR_Panel();
        }
    }
    public void initRAL_AD_Panel()
    {
        #region 写入属性(0)
        Boxes[0].initThisBoxInAD
            (currentBasicRAL.Hp,extraRaResult(AttributeData.Hp), AttributeData.Hp);
        Boxes[1].initThisBoxInAD
            (currentBasicRAL.Mp, extraRaResult(AttributeData.Mp), AttributeData.Mp);
        Boxes[2].initThisBoxInAD
            (currentBasicRAL.AT,extraRaResult(AttributeData.AT), AttributeData.AT);
        Boxes[3].initThisBoxInAD
            (currentBasicRAL.AD,extraRaResult(AttributeData.AD), AttributeData.AD);
        Boxes[4].initThisBoxInAD
            (currentBasicRAL.MT,extraRaResult(AttributeData.MT), AttributeData.MT);
        Boxes[5].initThisBoxInAD
            (currentBasicRAL.MD,extraRaResult(AttributeData.MD), AttributeData.MD);
        Boxes[6].initThisBoxInAD
            (currentBasicRAL.Speed,extraRaResult(AttributeData.Speed), AttributeData.Speed);
        #endregion
        #region 写入属性(1)
        MoreBoxes[0].initThisBoxInAD
            (currentBasicRAL.Accur,extraRaResult(AttributeData.Accur), AttributeData.Accur);
        MoreBoxes[1].initThisBoxInAD
            (currentBasicRAL.Evo,extraRaResult(AttributeData.Evo), AttributeData.Evo);
        MoreBoxes[2].initThisBoxInAD
            (currentBasicRAL.Crit,extraRaResult(AttributeData.Crit), AttributeData.Crit);
        MoreBoxes[3].initThisBoxInAD
            (currentBasicRAL.Expect,extraRaResult(AttributeData.Expect), AttributeData.Expect);
        if(_type == SDConstants.CharacterType.Hero)
        {
            MoreBoxes[4].gameObject.SetActive(true);
            MoreBoxes[5].gameObject.SetActive(true);
            MoreBoxes[4].initThisBoxInAD
                (currentBasicRAL.Tp,extraRaResult(AttributeData.Tp), AttributeData.Tp);
            MoreBoxes[5].initThisBoxInAD
                (currentBasicRAL.Taunt,extraRaResult(AttributeData.Taunt), AttributeData.Taunt);
        }
        else
        {
            MoreBoxes[4].gameObject.SetActive(false);
            MoreBoxes[5].gameObject.SetActive(false);
        }
        #endregion
    }
    public void initRAL_SR_Panel()
    {
        for (int i = 0; i < StateBoxes.Length; i++)
        {
            StateBoxes[i].initThisBoxInSR(currentBasicRAL.AllResistData[i]
                ,extraRaResult((StateTag)i), (StateTag)i);
        }
    }
    public int extraRaResult(AttributeData ADId=AttributeData.End)
    {
        if (ADId != AttributeData.End)
            return (int)(currentExtraRAL.read(ADId)
            );
        return 0;
    }
    public int extraRaResult(StateTag SRId = StateTag.End)
    {
        if (SRId != StateTag.End) 
            return (int)(currentExtraRAL.read(SRId)
            );
        return 0;
    }
    #region 点击显示详细信息
    public void ClickRALPlace()
    {
        ShowEllipsisData = !ShowEllipsisData;
        foreach(PerAttritubeBox b in Boxes)
        {
            b.AnimController(ShowEllipsisData);
        }
        foreach(PerAttritubeBox b in MoreBoxes)
        {
            b.AnimController(ShowEllipsisData);
        }
        StartCoroutine(ListExpandAnim(ShowEllipsisData));
    }
    public IEnumerator ListExpandAnim(bool isExpand)
    {
        if (isExpand)
        {
            foreach(PerAttritubeBox b in MoreBoxes)
            {
                b.transform.localScale = Vector3.right + Vector3.forward;
                b.transform.DOScale(Vector3.one, 0.2f);
            }
            MoreADList.DOScale(Vector3.one, ListExpandTime);
            SDList.GetComponent<CanvasGroup>().DOFade(0.075f, 0.2f);
        }
        else
        {
            foreach(PerAttritubeBox b in MoreBoxes)
            {
                b.transform.DOScale(Vector3.right + Vector3.forward, 0.2f);
            }
            MoreADList.DOScale(Vector3.right + Vector3.forward, ListExpandTime);
            SDList.GetComponent<CanvasGroup>().DOFade(1f, 0.2f);
        }
        yield return new WaitForSeconds(ListExpandTime);
    }
    #endregion
}
