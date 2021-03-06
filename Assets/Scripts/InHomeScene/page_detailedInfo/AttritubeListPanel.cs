﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AttritubeListPanel : MonoBehaviour
{
    public PerAttritubeBox[] Boxes;
    [Space(15)]
    public RoleAttributeList currentRAL;
    public int CurrentHashCode;
    public SDConstants.CharacterType _type;
    [Space(15)]
    public Transform MoreDetailsPanel;
    [System.Serializable]
    public class SimpleAttiVision
    {
        public Text AttiNameText;
        public Text AttiFigure;
        public AttributeData TAG;
        public void InitData(AttributeData tag,RoleAttributeList ral)
        {
            TAG = tag;
            AttiNameText.text = SDGameManager.T(TAG.ToString().ToLower());
            Refresh(ral);
        }
        public void Refresh(RoleAttributeList ral)
        {
            AttiFigure.text = "" + ral.read(TAG);
        }
    }
    public SimpleAttiVision[] AllAttiVisionArray;
    private void Start()
    {
        
    }
    public void initRAL(RoleAttributeList RAL
        , SDConstants.CharacterType CType, int lv = 0)
    {
        currentRAL = RAL;
        _type = CType;
        initRAL_AD_Panel();
    }
    public void initRAL_AD_Panel()
    {
        #region 写入属性(0)
        Boxes[0].initThisBoxInAD
            (currentRAL.Hp, AttributeData.Hp);
        Boxes[1].initThisBoxInAD
            (currentRAL.Mp, AttributeData.Mp);
        Boxes[2].initThisBoxInAD
            (currentRAL.AT, AttributeData.AT);
        Boxes[3].initThisBoxInAD
            (currentRAL.MT, AttributeData.MT);
        #endregion
    }


    public void Btn_CloseMoreDetails()
    {
        UIEffectManager.Instance.hideAnimFadeOut(MoreDetailsPanel);

    }

    public void Btn_OpenMoreDetails()
    {
        UIEffectManager.Instance.showAnimFadeIn(MoreDetailsPanel);
        for(int i = 0; i < AllAttiVisionArray.Length; i++)
        {
            SimpleAttiVision SAV = AllAttiVisionArray[i];
            SAV.InitData(SAV.TAG, currentRAL);
        }
    }
}
