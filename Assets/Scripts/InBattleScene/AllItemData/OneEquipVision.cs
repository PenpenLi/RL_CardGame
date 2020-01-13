using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

//[System.Serializable]
public class OneEquipVision : MonoBehaviour
{
    public Image IconImg;
    public Image BgIconImg;
    public Text ItemNameText;
    public ItemStarVision starVision;
    public EquipPosition Pos;
    public bool isSecondJewelryPos;
    public void initEquipVision(string iconImgPath,string bgIconImgPath,string name = ""
        ,int starnum = 0)
    {
        IconImg.sprite = Resources.Load<Sprite>(iconImgPath);
        BgIconImg.sprite = Resources.Load<Sprite>(bgIconImgPath);
        if (ItemNameText) ItemNameText.text = name;
        if (starVision) starVision.StarNum = starnum;
    }
    public void initEquipVision(string iconImgPath, int rarity, string name = ""
        , int starnum = 0)
    {
        string rarityPath = "Sprites/EquipBgIcon" + rarity;
        initEquipVision(iconImgPath, rarityPath, name, starnum);
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