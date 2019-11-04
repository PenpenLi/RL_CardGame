using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleHeroDetailVision : MonoBehaviour
{   

    public Image heroCharacterDrawingImg;
    public Image CareerIconImg;
    public Text CareerText;
    public Image RaceIconImg;
    public Text RaceText;
    public Text NameText;
    public Text NameBeforeText;
    public Text RarityText;
    public ItemStarVision StarNumVision;
    public Text LvText;
    public Transform ExpSlider;

    public CharacterModelController heroHeadImg;
    HomeScene hs;
    public Button MoreInfoBtn;
    public void ReadFromSDHD(SDHeroDetail SDHD)
    {
        heroCharacterDrawingImg.sprite = SDHD.heroCharacterDrawingImg.sprite;
        CareerIconImg.sprite = SDHD.CareerIconImg.sprite;
        CareerText.text = SDHD.CareerText.text;
        RaceIconImg.sprite = SDHD.RaceIconImg.sprite;
        RaceText.text = SDHD.RaceText.text;
        NameText.text = SDHD.NameText.text;
        NameBeforeText.text = SDHD.NameBeforeText.text;
        RarityText.text = SDHD.RarityText.text;
        StarNumVision.StarNum = SDHD.StarNumVision.StarNum;
        LvText.text = SDHD.LvText.text;
        ExpSlider.localScale = SDHD.ExpSlider.localScale;

        //heroheadimg


        MoreInfoBtn.gameObject.SetActive(true);
    }
    public void EmptyVision()
    {
        //heroCharacterDrawingImg.sprite = SDHD.heroCharacterDrawingImg.sprite;
        //CareerIconImg.sprite = SDHD.CareerIconImg.sprite;
        CareerText.text = "--";
        //RaceIconImg.sprite = SDHD.RaceIconImg.sprite;
        RaceText.text = "--";
        NameText.text = "";
        NameBeforeText.text = "";
        RarityText.text = "";
        StarNumVision.StarNum = 0;
        LvText.text = "";
        ExpSlider.localScale = Vector3.up;

        MoreInfoBtn.gameObject.SetActive(false);
    }

    public void BtnToHeroDetailPanel()
    {
        Debug.Log("进入英雄配置界面");
        if (hs == null) hs = FindObjectOfType<HomeScene>();
        //UIEffectManager.Instance.showAnimFadeIn(hs.heroDetailPanel);
        hs.heroDetailBtnTapped();
    }
}
