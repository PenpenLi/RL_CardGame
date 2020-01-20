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
    public int hashcode;
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
    public void ShowHeroMessage(int _hashcode)
    {
        hashcode = _hashcode;
        GDEHeroData data = SDDataManager.Instance.getHeroByHashcode(hashcode);
        if(data==null || hashcode <= 0)
        {
            EmptyVision();return;
        }
        HeroInfo info = SDDataManager.Instance.getHeroInfoById(data.id);
        heroCharacterDrawingImg.sprite = info.FaceIcon;
        CareerIconImg.sprite = info.Career.Icon;
        CareerText.text = info.Career.NAME;
        RaceIconImg.sprite = info.Race.Icon;
        RaceText.text = info.Race.NAME;
        //NameBeforeText.text
        NameText.text = info.Name;
        RarityIconImg.sprite = SDDataManager.Instance.raritySprite(info.Rarity);
        RarityIconImg.gameObject.SetActive(true);
        StarNumVision.StarNum = info.LEVEL + data.starNumUpgradeTimes;
        LvText.text = SDGameManager.T("Lv.")+SDDataManager.Instance.getLevelByExp(data.exp);
        ExpSlider.localScale = new Vector3(SDDataManager.Instance.getExpRateByExp(data.exp), 1, 1);
        MoreInfoBtn.gameObject.SetActive(true);
    }
    public void ShowGoddessMessage(string id)
    {
        GDEgoddessData data = SDDataManager.Instance.getGDEGoddessDataById(id);
        if (data == null)
        {
            EmptyVision(); return;
        }
        GoddessInfo info = SDDataManager.Instance.getGoddessInfoById(id);
        NameText.text = info.Name;
        /*
        heroCharacterDrawingImg.sprite = info.FaceIcon;
        CareerIconImg.sprite = info.Career.Icon;
        CareerText.text = info.Career.NAME;
        RaceIconImg.sprite = info.Race.Icon;
        RaceText.text = info.Race.NAME;
        //NameBeforeText.text
        RarityIconImg.sprite = SDDataManager.Instance.raritySprite(info.Rarity);
        RarityIconImg.gameObject.SetActive(true);
        StarNumVision.StarNum = info.LEVEL + data.starNumUpgradeTimes;
        LvText.text = SDGameManager.T("Lv.") + SDDataManager.Instance.getLevelByExp(data.exp);
        ExpSlider.localScale = new Vector3(SDDataManager.Instance.getExpRateByExp(data.exp), 1, 1);
        */
        MoreInfoBtn.gameObject.SetActive(true);
    }

    public void EmptyVision()
    {
        CareerText.text = "--";
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
        hs._heroDetailPanel.GetComponent<HeroDetailPanel>()
            .detail.HeroWholeMessage.currentHeroHashcode = hashcode;
        hs.heroDetailBtnTapped();
    }
    public void BtnToGoddessDetailPanel()
    {
        Debug.Log("进入守护者配置界面");
        hs.goddessDetailBtnTapped();
    }
}
