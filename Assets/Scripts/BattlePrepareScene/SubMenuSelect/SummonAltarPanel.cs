using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SummonAltarPanel : MonoBehaviour
{
    HomeScene _homescene;
    public HomeScene homeScene
    { get { if (_homescene == null) { _homescene = FindObjectOfType<HomeScene>(); }
            return _homescene;
        } }
    public ScrollRect rect;
    public PageView pageView { get { return rect.GetComponent<PageView>(); } }
    public OneAltarData AltarData;
    [Header("抽卡结果页面显示")]
    public Transform ResultRewardPanel;
    public Transform RRPContent;
    public Transform roleItem;
    public List<SingleItem> resultRewards = new List<SingleItem>();
    public List<int> rewardIds = new List<int>();
    public Text messageText;
    [Header("新角色Pose")]
    public Transform NRSBG;
    public Transform NewRoleShowPanel;
    public Image NewRolePoseImg;
    public Text NewRoleNameText;
    public Text NewRoleDetailText;
    public Text NewRoleWelcomeText;
    private float NewRolePoseTime = 2f;

    public enum panelContent
    {
        main,rrp,pose,end
    }
    [Header("历史记录设置")]
    public panelContent currentPanelContent = panelContent.end;
    public void whenOpenThisPanel()
    {
        currentPanelContent = panelContent.main;
        pageView.maxIndex = (int)Job.End;
        rewardIds.Clear();
        refreshCurrentPoolIndex();
    }

    public void SummonOneTime()
    {
        if (SDDataManager.Instance.getMaterialNum(3010017) > 0)
        {
            SDDataManager.Instance.consumeMaterial(3010017);
        }
        else if (SDDataManager.Instance.PlayerData.damond >= SDConstants.altarDamondCost)
        {
            SDDataManager.Instance.PlayerData.damond -= SDConstants.altarDamondCost;
        }
        else
        {
            Debug.Log("无法获取：道具或钻石不足");
            return;
        }

        List<int> ids = ConfirmCurrentPool();
        int result = UnityEngine.Random.Range(0, ids.Count);
        Debug.Log("获得英雄 id: " + ids[result]);
        rewardIds.Add(ids[result]);
        initRRP();

        refreshCurrentPoolIndex();
    }
    public void SumonTenTimes()
    {
        if (SDDataManager.Instance.getMaterialNum(3010018) > 0)
        {
            SDDataManager.Instance.consumeMaterial(3010018);
        }
        else if (SDDataManager.Instance.getMaterialNum(3010017) >= 10)
        {
            SDDataManager.Instance.consumeMaterial(3010017, 10);
        }
        else if (SDDataManager.Instance.PlayerData.damond >= SDConstants.altarDamondCost * 10)
        {
            SDDataManager.Instance.PlayerData.damond -= SDConstants.altarDamondCost * 10;
        }
        else
        {
            Debug.Log("无法获取：道具或钻石不足");
            return;
        }


        List<int> ids = ConfirmCurrentPool();
        for(int i = 0; i < 10; i++)
        {
            int result = UnityEngine.Random.Range(0, ids.Count);
            Debug.Log("获得英雄 id: " + ids[result]);
            rewardIds.Add(ids[result]);
        }
        initRRP();

        refreshCurrentPoolIndex();
    }

    public IEnumerator IEInitCallingRewards()
    {
        for(int i = 0; i < rewardIds.Count; i++)
        {
            bool firstGet = true;
            int id = rewardIds[i];
            foreach (GDEHeroData hero in SDDataManager.Instance.PlayerData.herosOwned)
            {
                if (hero.id == id) firstGet = false;
            }
            int hc = callHeroToPlayerdataExportHashcode(id);
            resultRewards[i].initCalledHero(hc);
            if (firstGet)
            {
                currentPanelContent = panelContent.pose;
                NRSBG.gameObject.SetActive(true);
                resultRewards[i].levelText.text = "NEW!!!";

                if (!excapePose)
                {
                    UIEffectManager.Instance.showAnimFadeIn(NewRoleShowPanel);

                    yield return new WaitForSeconds(NewRolePoseTime);

                    UIEffectManager.Instance.hideAnimFadeOut(NewRoleShowPanel);
                    yield return new WaitForSeconds(0.3f);
                }
            }
            if (excapePose)
            {
                excapePose = false;
            }
        }
        NRSBG.gameObject.SetActive(false);
        currentPanelContent = panelContent.rrp;
    }
    bool excapePose = false;
    public void BtnToExcapePose() 
    {
        if (!excapePose) 
        {
            excapePose = true;
        }
    }
    public List<int> ConfirmCurrentPool()
    {
        List<int> all = new List<int>();
        int index = pageView.currentIndex;


        //当前为测试用卡池

        //职业卡池
        if(index < (int)Job.End)
        {
            for (int i = 0; i < (int)Job.End; i++)
            {
                if (index == i)
                {
                    List<Dictionary<string, string>> list = SDDataManager.Instance.ReadHeroFromCSV(i);
                    for (int j = 0; j < list.Count; j++)
                    {
                        all.Add(SDDataManager.Instance.getInteger(list[j]["id"]));
                    }
                    AltarData.PoolDesc.text = "当前卡池类型为: " + ((Job)i).ToString();
                    break;
                }
            }
        }
        //全英雄卡池
        else
        {
            for (int i = 0; i < (int)Job.End; i++)
            {
                List<Dictionary<string, string>> list = SDDataManager.Instance.ReadHeroFromCSV(i);
                for (int j = 0; j < list.Count; j++)
                {
                    all.Add(SDDataManager.Instance.getInteger(list[j]["id"]));
                }
            }
            AltarData.PoolDesc.text = "当前卡池类型为: 所有";
        }
        AltarData.initAltarPool();

        return all;
    }

    public void refreshCurrentPoolIndex()
    {
        messageText.text = "水晶数：" + SDDataManager.Instance.PlayerData.damond
            + "  ---单抽券数：" + SDDataManager.Instance.getMaterialNum(3010017)
            + "  ---十连券数：" + SDDataManager.Instance.getMaterialNum(3010018);
        ConfirmCurrentPool();
    }
    public void initRRP()
    {
        UIEffectManager.Instance.showAnimFadeIn(ResultRewardPanel);
        //构建角色卡牌
        for (int i = 0; i < rewardIds.Count; i++)
        {
            int id = rewardIds[i];

            Transform s = Instantiate(roleItem) as Transform;
            s.SetParent(RRPContent);
            s.localScale = Vector3.one;
            s.gameObject.SetActive(true);
            SingleItem _s = s.GetComponent<SingleItem>();
            resultRewards.Add(_s);
        }
        //
        StartCoroutine(IEInitCallingRewards());
    }
    public int callHeroToPlayerdataExportHashcode(int heroId)
    {
        SDDataManager.Instance.heroNum++;
        GDEHeroData hero = new GDEHeroData(GDEItemKeys.Hero_BasicHero)
        {
            id = heroId
            ,
            hashCode=SDDataManager.Instance.heroNum
            ,
            status = 0
            ,
            starNumUpgradeTimes=1
            ,
            exp=0
        };
        List<GDEASkillData> list = SDDataManager.Instance.addStartSkillsWhenSummoning(hero.id);
        for(int i = 0; i < list.Count; i++)
        {
            hero.skillsOwned.Add(list[i]);
            if (SDDataManager.Instance.getSkillByHeroId(list[i].Id, hero.id).isOmegaSkill)
            {
                hero.skillOmegaId = list[i].Id;
            }
            else
            {
                if (hero.skill0Id > 0 && SDDataManager.Instance.checkHeroEnableSkill1ById(hero.id))
                {
                    hero.skill1Id = list[i].Id;
                }
                else
                {
                    hero.skill0Id = list[i].Id;
                }
            }
        }
        //
        SDDataManager.Instance.PlayerData.herosOwned.Add(hero);
        SDDataManager.Instance.PlayerData.Set_herosOwned();
        //rewardHCs.Add(hero.hashCode);
        return hero.hashCode;
    }
    public void initSkillListForHero(GDEHeroData hero)
    {
        Job j = (Job)SDDataManager.Instance.getHeroCareerById(hero.id);
        Race r = (Race)SDDataManager.Instance.getHeroRaceById(hero.id);
        int quality = SDDataManager.Instance.getHeroQualityById(hero.id);
        List<OneSkill> all = SkillDetailsList.WriteOneSkillList(j,r,quality);

        SDDataManager.Instance.getHeroByHashcode(hero.hashCode).skillsOwned
            .Add(new GDEASkillData(GDEItemKeys.ASkill_normalAttack)
            {
                Id = all[0].skillId
                ,
                Lv = 0
            });
        SDDataManager.Instance.getHeroByHashcode(hero.hashCode).Set_skillsOwned();

        List<int> otherMayGetList = RandomIntger.NumListReturn(2, all.Count - 1);
        SDDataManager.Instance.getHeroByHashcode(hero.hashCode).skillsOwned
            .Add(new GDEASkillData(GDEItemKeys.ASkill_normalAttack)
            {
                Id = all[otherMayGetList[0] + 1].skillId
                ,
                Lv = 0
            });
        SDDataManager.Instance.getHeroByHashcode(hero.hashCode).Set_skillsOwned();

        SDDataManager.Instance.getHeroByHashcode(hero.hashCode).skillsOwned
            .Add(new GDEASkillData(GDEItemKeys.ASkill_normalAttack)
            {
                Id = all[otherMayGetList[1] + 1].skillId
                ,
                Lv = 0
            });
        SDDataManager.Instance.getHeroByHashcode(hero.hashCode).Set_skillsOwned();
    }
    public void exitRRP()
    {
        rewardIds.Clear();
        for(int i = 0; i < resultRewards.Count; i++)
        {
            Destroy(resultRewards[i].gameObject);
        }
        resultRewards.Clear();
        currentPanelContent = panelContent.main;
        UIEffectManager.Instance.hideAnimFadeOut(ResultRewardPanel);
    }



    public void commonBackAction()
    {
        if(currentPanelContent == panelContent.main)//
        {
            homeScene.SubMenuClose();
        }
        else if(currentPanelContent == panelContent.rrp)
        {
            exitRRP();
        }
        else if(currentPanelContent == panelContent.pose)
        {
            BtnToExcapePose();
        }
        else if(currentPanelContent == panelContent.end)
        {

        }
    }
}
