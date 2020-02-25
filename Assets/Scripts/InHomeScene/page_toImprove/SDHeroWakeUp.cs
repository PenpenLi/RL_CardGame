using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SDHeroWakeUp : MonoBehaviour
{
    public SDHeroDetail HeroDetail;
    [Header("AbovePanel")]
    public Image CareerIcon;
    public Text CareerText;
    public Image RaceIcon;
    public Text RaceText;
    public Image RarityImg;
    [Space(10)]
    public Image PoseImg;
    public Image PoseBgImg;
    //
    [Header("MidPanel")]
    public Image OldWakeIconImg;
    public Image NewWakeIconImg;

    public void RefreshWakeUpPanel()
    {
        HeroInfo info = SDDataManager.Instance.getHeroInfoById(HeroDetail.ID);
        HeroRace hrace = info.Race;
        RaceIcon.sprite = hrace.Icon;RaceIcon.SetNativeSize();
        RaceText.text = SDGameManager.T(hrace.NAME);
        //
        RoleCareer rcareer = info.Career;
        CareerIcon.sprite = rcareer.Icon;CareerIcon.SetNativeSize();
        CareerText.text = SDGameManager.T(rcareer.NAME);
        //
        RarityImg.sprite = SDDataManager.Instance.raritySprite(info.Rarity);
        RarityImg.SetNativeSize();
        if(info.PersonalDrawImg == null)
        {
            PoseImg.gameObject.SetActive(false);
            PoseBgImg.sprite = SDDataManager.Instance.heroRaceBgIcon(hrace.Race);
        }
        else
        {
            PoseImg.gameObject.SetActive(true);

        }
    }
}
