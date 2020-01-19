﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SDEquipDetail : MonoBehaviour
{
    public string equipId;
    public int equipHashcode;
    public int equipLv;
    public EquipItem equipData;
    [Header("装备信息可视化")]
    public Image equipIcon;
    public Image equipBgIcon;
    public Text nameText;
    public Text nameBeforeText;
    public Text RarityText;
    [Space(10)]
    public Text expText;
    //public Transform expSlider;
    public Image equipPosImg;
    public Text equipPosText;
    public Image equipMoldImg;
    public Text equipMoldText;
    public ItemStarVision starVision;
    //
    EquipDetailPanel EDP
    {
        get { return GetComponentInParent<EquipmentPanel>().EDP; }
    }
    public void initEquipDetailVision(GDEEquipmentData equip)
    {
        equipId = equip.id;
        equipHashcode = equip.hashcode;
        equipData = SDDataManager.Instance.GetEquipItemById(equipId);

        //
        nameText.text = equipData.name;
        RarityText.text = SDDataManager.Instance.rarityString(equipData.LEVEL);
        int lv = equip.lv ;
        expText.text = SDGameManager.T("Lv.") + lv;
        equipLv = lv;
        equipPosText.text = ((EquipPosition)equipData.EquipPos).ToString();
        starVision.StarNum = equip.quality;

        //
        EDP.EmptyPanel.gameObject.SetActive(false);
    }
    public void initEquipDetailVision(int equip_hashcode)
    {
        GDEEquipmentData equip = SDDataManager.Instance.getEquipmentByHashcode(equip_hashcode);
        initEquipDetailVision(equip);
    }
}
