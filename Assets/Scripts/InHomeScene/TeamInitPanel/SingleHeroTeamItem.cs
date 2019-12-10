using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

/// <summary>
/// 英雄出战队伍项
/// </summary>
public class SingleHeroTeamItem : MonoBehaviour
{
    public int index;
    protected SDHeroSelect _heroSelect;
    public int heroHashcode;
    public CharacterModelController heroModel;
    //public Image frameImg;
    public Image addImg;
    public Image selectedImg;

    public Text levelText;
    public Text nameText;


    //public Transform unlockBtn;
    public bool isSeatUnlocked = true;

    // Start is called before the first frame update
    void Start()
    {
        _heroSelect = GetComponentInParent<SDHeroSelect>();
    }

    public void initHero(int hashcode)
    {
        heroHashcode = hashcode;
        refreshUnlockBtn();
        if(heroHashcode == 0)
        {
            initEmptyHero();
        }
        else
        {
            addImg.gameObject.SetActive(false);
            string id = SDDataManager.Instance.getHeroIdByHashcode(heroHashcode);
            GDEHeroData hero = SDDataManager.Instance.GetHeroOwnedByHashcode(heroHashcode);
            ROHeroData dal = SDDataManager.Instance.getHeroDataByID(id, hero.starNumUpgradeTimes);

            levelText.gameObject.SetActive(true);
            levelText.text = "Lv." + SDDataManager.Instance.getLevelByExp(hero.exp);
            nameText.text = SDGameManager.T(dal.Name);
        }

        
    }
    public void refreshUnlockBtn()
    {
        if(index == 0)
        {
            isSeatUnlocked = true;
        }
        else
        {
            isSeatUnlocked = (SDDataManager.Instance.SettingData.seatUnlocked[index] == 1);
        }
        //unlockBtn.gameObject.SetActive(!isSeatUnlocked);
        addImg.gameObject.SetActive(isSeatUnlocked);
    }
    public void initEmptyHero()
    {
        nameText.text = "";
        levelText.gameObject.SetActive(false);
        //frameImg.sprite = rarityFrameSps[0];
        //careerImg = 
        //raceImg = 
        addImg.gameObject.SetActive(isSeatUnlocked);
    }

    public void heroBtnTapped()
    {
        if (!isSeatUnlocked)
        {
            return;
        }

        if (_heroSelect == null) _heroSelect = GetComponentInParent<SDHeroSelect>();


        //if (_heroSelect.MainPanel.usingIEcannotTap) return;
        //_heroSelect.MainPanel.usingIEcannotTap = true;

        _heroSelect.heroBtnTapped(index);
    }
}
