using System.Collections;
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
    public Image equipFrameIcon;
    [Space]
    public Text nameText;
    public Text nameBeforeText;
    [Space(10)]
    public Image equipPosImg;
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
        int lv = equip.lv;
        nameText.text = (lv>0?SDGameManager.T("Lv.")+lv+"·":"")+equipData.NAME;


        equipLv = lv;
        equipPosImg.sprite = SDDataManager.Instance.equipPosIcon(equipData.EquipPos);
        equipPosImg.SetNativeSize();
        starVision.StarNum = equip.quality;
        //
        equipIcon.sprite = equipData.IconFromAtlas;
        equipBgIcon.sprite = SDDataManager.Instance.baseBgSpriteByRarity(equipData.LEVEL);
        equipFrameIcon.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(equipData.LEVEL);
        //
        EDP.EmptyPanel.gameObject.SetActive(false);
    }
    public void initEquipDetailVision(int equip_hashcode)
    {
        GDEEquipmentData equip = SDDataManager.Instance.getEquipmentByHashcode(equip_hashcode);
        initEquipDetailVision(equip);
    }
}
