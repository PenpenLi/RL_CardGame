using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

//[System.Serializable]
public class OneEquipVision : MonoBehaviour
{
    public Image IconImg;
    public Image IconFrameImg;
    public Image BgIconImg;
    public Text ItemNameText;
    public ItemStarVision starVision;
    public EquipPosition Pos;
    public bool isSecondJewelryPos;
    public Sprite emptyBgSprite;
    public Sprite emptyFrameSprite;
    public SDHeroDetail DETAIL
    {
        get { return GetComponentInParent<SDHeroDetail>(); }
    }

    public void initEquipVision(GDEEquipmentData data)
    {
        if(data == null || string.IsNullOrEmpty(data.id))
        {
            initAsEmpty();
        }
        else
        {
            EquipItem item = SDDataManager.Instance.GetEquipItemById(data.id);
            if (item)
            {
                IconImg.gameObject.SetActive(true);
                IconImg.sprite = SDDataManager.Instance.GetEquipIconById(data.id);
                BgIconImg.sprite = SDDataManager.Instance.baseBgSpriteByRarity(item.LEVEL);
                IconFrameImg.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(item.LEVEL);
            }
            else
            {
                initAsEmpty();
            }
        }
        if (ItemNameText) ItemNameText.gameObject.SetActive(false);
        if (starVision) starVision.gameObject.SetActive(false);
    }
    void initAsEmpty()
    {
        //IconImg.sprite = null;
        IconImg.gameObject.SetActive(false);
        IconFrameImg.sprite = emptyFrameSprite;
        BgIconImg.sprite = emptyBgSprite;
    }


    public void equipBtnTapped()
    {
        HeroEquipList HEL = GetComponentInParent<HeroEquipList>();
        if (HEL) HEL.equipBtnTapped(Pos, isSecondJewelryPos);
    }
}


public enum EquipPosition
{
    Head=0
        ,
    Breast=1
        ,
    Arm=2
        ,
    Leg=3
        ,
    Finger=4
        ,
    Hand = 5
        ,
    End = 6
}