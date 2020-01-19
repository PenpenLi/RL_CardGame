using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SimpleHeroDetailVision : MonoBehaviour
{
    public Image heroCharacterDrawingImg;
    public Image CareerIconImg;
    public Text CareerText;
    public Image RaceIconImg;
    public Text RaceText;
    public Text NameText;
    public Text NameBeforeText;
    public Image RarityIconImg;
    public ItemStarVision StarNumVision;
    public Text LvText;
    public Transform ExpSlider;
    HomeScene _hs;
    public HomeScene hs
    {
        get
        {
            if (_hs == null) _hs = FindObjectOfType<HomeScene>();
            return _hs;
        }
    }
    //public CharacterModelController heroHeadImg;
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
        RarityIconImg.sprite = SDHD.RarityImg.sprite;
        RarityIconImg.gameObject.SetActive(true);
        StarNumVision.StarNum = SDHD.StarNumVision.StarNum;
        LvText.text = SDHD.LvText.text;
        ExpSlider.localScale = SDHD.ExpSlider.localScale;

        //heroheadimg


        MoreInfoBtn.gameObject.SetActive(true);
    }
    public void ReadFromSDGD(SDGoddesDetail SDGD)
    {
        heroCharacterDrawingImg.sprite = SDGD.goddessCharacterDrawingImg.sprite;
        NameText.text = SDGD.nameText.text;
        RarityIconImg.gameObject.SetActive(false);

        LvText.text = SDGameManager.T("Lv.") + SDGD.lv;       
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
        RarityIconImg.sprite = null;
        RarityIconImg.gameObject.SetActive(false);
        StarNumVision.StarNum = 0;
        LvText.text = "";
        ExpSlider.localScale = Vector3.up;

        MoreInfoBtn.gameObject.SetActive(false);
    }

    public void BtnToHeroDetailPanel()
    {
        Debug.Log("进入英雄配置界面");
        //UIEffectManager.Instance.showAnimFadeIn(hs.heroDetailPanel);
        hs.heroDetailBtnTapped();
    }
    public void BtnToGoddessDetailPanel()
    {
        Debug.Log("进入守护者配置界面");
        hs.goddessDetailBtnTapped();
    }
}
