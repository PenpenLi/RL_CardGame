using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

/// <summary>
/// 出战英雄选择类
/// </summary>
public class SDHeroSelect : MonoBehaviour
{
    public SingleHeroTeamItem[] heroItemsInTeam;
    //public List<int> allTeamIds;
    //public List<int> usedTeamIds;
    //public int CurrentTeamIndex;
    public int[] heroesInTeam;
    public SelectTeamUnitPanel MainPanel;
    public Transform heroesSelectPanel;
    //public Transform heroDetailPanel;
    //public Image adsImg;
    //public Button adsBtn;
    //public Text adsStatusText;
    //public Sprite[] adsStateSps;
    [Header("简易角色信息板")]
    public SimpleHeroDetailVision simpleHDV;
    // Start is called before the first frame update
    void Start()
    {
        //heroPanelInit();
    }
    public void heroPanelInit()
    {
        int currentTeamId = MainPanel.CurrentTeamId;
        GDEunitTeamData unitTeam = SDDataManager.Instance.getHeroTeamByTeamId(currentTeamId);
        heroesInTeam = new int[unitTeam.heroes.Count];
        for(int i = 0; i < unitTeam.heroes.Count; i++)
        {
            heroesInTeam[i] = unitTeam.heroes[i];
            heroItemsInTeam[i].initHero(unitTeam.heroes[i]);
        }
        //initads();

        heroBtnFunction(MainPanel.currentHeroIndexInTeam);
    }

    public void heroBtnTapped(int index)
    {
        SDGameManager.Instance.isSelectHero = true;
        SDGameManager.Instance.heroSelectType = SDConstants.HeroSelectType.Battle;
        //SDGameManager.Instance.currentHeroTeamIndex = index;
        MainPanel.currentHeroIndexInTeam = index;
        //Debug.Log("HeroBtnTapped Index:" + index + " heroesId[index]:"+heroesId[index]);
        heroBtnFunction(index);
    }
    public void heroBtnFunction(int index)
    {
        showHeroesSelectPanel(index);
        if (heroesInTeam[index] == 0)
        {
            //showHeroesSelectPanel(index);
            simpleHDV.EmptyVision();
        }
        else
        {
            BasicHeroSelect BHS = heroesSelectPanel.GetComponent<BasicHeroSelect>();
            if (BHS)
            {
                //显示英雄详情
                SDHeroDetail HDP = BHS.heroDetails.GetComponent<SDHeroDetail>();
                int _hashcode = heroesInTeam[index];
                HDP.gameObject.SetActive(true);
                HDP.initHeroDetailPanel(_hashcode);


                simpleHDV.ReadFromSDHD(HDP);
            }
            else { simpleHDV.EmptyVision(); }
        }
    }
    public void showHeroesSelectPanel(int index)
    {
        heroesSelectPanel.localScale = Vector3.one;

        BasicHeroSelect bhs = heroesSelectPanel.GetComponent<BasicHeroSelect>();
        bhs.index = index;
        //bhs.pageController.scrollRectReset();
        bhs.heroesInit();
        if(!bhs.gameObject.activeSelf)
            UIEffectManager.Instance.showAnimFadeIn(bhs.transform);

    }

}
