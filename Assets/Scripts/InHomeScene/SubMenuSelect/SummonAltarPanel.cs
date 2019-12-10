using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SummonAltarPanel : BasicSubMenuPanel
{
    public ScrollRect rect;
    public PageView pageView { get { return rect.GetComponent<PageView>(); } }
    public OneAltarData AltarData;
    [Header("抽卡结果页面显示")]
    public Transform ResultRewardPanel;
    public Transform RRPContent;
    public Transform roleItem;
    public List<SingleItem> resultRewards = new List<SingleItem>();
    public List<string> rewardIds = new List<string>();
    public Text messageText;
    //[Header("新角色Pose")]
    //public Transform NRSBG;
    //public Transform NewRoleShowPanel;
    //public Image NewRolePoseImg;
    //public Text NewRoleNameText;
    //public Text NewRoleDetailText;
    //public Text NewRoleWelcomeText;
    //private float NewRolePoseTime = 2f;
    public enum panelContent
    {
        main,rrp,pose,end
    }
    [Header("历史记录设置")]
    public panelContent currentPanelContent = panelContent.end;
    [Header("新守护者")]
    public Button btn_goddess_oneTime;
    public Button btn_goddess_tenTimes;
    public Text goddessPoolText;
    public int currentGoddessPoolIndex;
    public List<string> rewardGoddesses = new List<string>();

    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        currentPanelContent = panelContent.main;
        pageView.maxIndex = (int)Job.End;
        rewardIds.Clear();
        refreshCurrentPoolIndex();
    }

    public void SummonOneTime()
    {
        if(SDDataManager.Instance.consumeMaterial("M_M#3010017", out int residue))
        {
            Debug.Log("消耗道具 M_M#3010017 ,剩余" + residue);
        }
        else
        {
            if(SDDataManager.Instance.PlayerData.damond >= SDConstants.altarDamondCost)
            {
                SDDataManager.Instance.ConsumeDamond(SDConstants.altarDamondCost);
            }
            else
            {
                Debug.Log("无法获取：道具或钻石不足");
                return;
            }
        }

        List<string> ids = ConfirmCurrentPool();
        int result = UnityEngine.Random.Range(0, ids.Count);
        Debug.Log("获得英雄 id: " + ids[result]);
        rewardIds.Add(ids[result]);
        initRRP();

        refreshCurrentPoolIndex();
    }
    public void SumonTenTimes()
    {
        if(SDDataManager.Instance.consumeMaterial("M_M#3010018", out int residue))
        {
            Debug.Log("消耗道具 M_M#3010018 ,剩余" + residue);
        }
        else
        {
            if(SDDataManager.Instance.consumeMaterial("M_M#3010017", out int residue1, 10))
            {
                Debug.Log("消耗道具 M_M#3010017 10个 ,剩余" + residue1);
            }
            else
            {
                if (SDDataManager.Instance.PlayerData.damond 
                    >= SDConstants.altarDamondCost * 10)
                {
                    SDDataManager.Instance.ConsumeDamond
                        (SDConstants.altarDamondCost * 10);
                }
                else
                {
                    Debug.Log("无法获取：道具或钻石不足");
                    return;
                }
            }
        }


        List<string> ids = ConfirmCurrentPool();
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
        homeScene.mapBtn.gameObject.SetActive(false);
        for(int i = 0; i < rewardIds.Count; i++)
        {
            bool firstGet = true;
            string id = rewardIds[i];
            foreach (GDEHeroData hero in SDDataManager.Instance.PlayerData.herosOwned)
            {
                if (hero.id == id) firstGet = false;
            }
            int hc = callHeroToPlayerdataExportHashcode(id);
            resultRewards[i].initCalledHero(hc);
            if (firstGet)
            {
                currentPanelContent = panelContent.pose;
                //NRSBG.gameObject.SetActive(true);
                resultRewards[i].upText.text = "NEW!!!";

                if (!excapePose)
                {
                    PopoutController.CreatePopoutUnitSpeak("英雄大名"
                        , "久仰大名", "Textures/xgb33", 10 + rewardIds.Count - i
                        , false, PopoutController.PopoutWIndowAlertType.ConfirmMessage
                        , (PopoutController c, PopoutController.PopoutWindowAlertActionType a)
                         =>
                         {
                             if (a == PopoutController.PopoutWindowAlertActionType.OnConfirm)
                             {
                                 StartCoroutine(c.Dismiss());
                             }
                             else
                                 StartCoroutine(c.IEWaitAndDismiss(0.3f));
                         });
                    yield return new WaitForSeconds(0.3f);
                }
            }
            if (excapePose)
            {
                excapePose = false;
            }
        }
        //NRSBG.gameObject.SetActive(false);
        currentPanelContent = panelContent.rrp;
        homeScene.mapBtn.gameObject.SetActive(true);
    }
    bool excapePose = false;
    public void BtnToExcapePose() 
    {
        if (!excapePose) 
        {
            excapePose = true;
        }
    }
    public List<string> ConfirmCurrentPool()
    {
        List<string> all = new List<string>();
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
                        all.Add(list[j]["id"]);
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
                    all.Add(list[j]["id"]);
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
            + "  ---单抽券数：" + SDDataManager.Instance.getMaterialNum("M_M#3010017")
            + "  ---十连券数：" + SDDataManager.Instance.getMaterialNum("M_M#3010018");
        ConfirmCurrentPool();
    }
    public void initRRP()
    {
        UIEffectManager.Instance.showAnimFadeIn(ResultRewardPanel);
        //构建角色卡牌
        for (int i = 0; i < rewardIds.Count; i++)
        {
            string id = rewardIds[i];

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
    public int callHeroToPlayerdataExportHashcode(string heroId)
    {
        SDDataManager.Instance.addHero(heroId);
        return SDDataManager.Instance.heroNum;
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

    #region GoddessPool
    public List<string> ConfirmGoddessPool()
    {
        List<string> all = new List<string>();
        List<Dictionary<string, string>> list = SDDataManager.Instance.ReadFromCSV("goddess");
        for(int i = 0; i < list.Count; i++)
        {
            all.Add(list[i]["id"]);
        }
        return all;
    }
    public void BtnToAltarGoddess_oneTime()
    {
        if(SDDataManager.Instance.consumeMaterial("M_M#3010017", out int re0))
        {
            Debug.Log("成功消耗3010017 剩余" + re0);
        }
        else
        {
            if (SDDataManager.Instance.PlayerData.damond >= SDConstants.altarDamondCost)
            {
                SDDataManager.Instance.PlayerData.damond -= SDConstants.altarDamondCost;
            }
            else
            {
                Debug.Log("无法获取：道具或钻石不足");
                return;
            }
        }

        List<string> ids = ConfirmGoddessPool();
        int result = UnityEngine.Random.Range(0, ids.Count);
        Debug.Log("获得守护者 id: " + ids[result]);
        rewardGoddesses.Add(ids[result]);
        initGoddess();

        refreshGoddessPool();
        AltarData.refreshThisPool();
    }
    public void BtnToAltarGoddess_tenTimes()
    {
        if (SDDataManager.Instance.consumeMaterial("M_M#3010018", out int re0))
        {
            Debug.Log("成功消耗3010018 剩余" + re0);
        }
        else
        {
            if (SDDataManager.Instance.consumeMaterial("M_M#3010017", out int re1, 10))
            {
                Debug.Log("消耗道具3010017 10个 ,剩余" + re1);
            }
            else
            {
                if (SDDataManager.Instance.PlayerData.damond
                    >= SDConstants.altarDamondCost * 10)
                {
                    SDDataManager.Instance.PlayerData.damond
                        -= SDConstants.altarDamondCost * 10;
                }
                else
                {
                    Debug.Log("无法获取：道具或钻石不足");
                    return;
                }
            }
        }
        List<string> ids = ConfirmGoddessPool();
        for (int i = 0; i < 10; i++)
        {
            int result = UnityEngine.Random.Range(0, ids.Count);
            Debug.Log("获得英雄 id: " + ids[result]);
            rewardGoddesses.Add(ids[result]);
        }
        initGoddess();

        refreshGoddessPool();
        AltarData.refreshThisPool();
    }
    public void initGoddess()
    {
        //构建守护者卡牌
        for(int i = 0; i < rewardGoddesses.Count; i++)
        {
            bool flag = false;
            foreach(GDEgoddessData g in SDDataManager.Instance.PlayerData.goddessOwned)
            {
                if(g.id == rewardGoddesses[i])
                {
                    flag = true;
                    g.volume++;
                    SDDataManager.Instance.PlayerData.Set_goddessOwned();
                    break;
                }
            }
            if (!flag)
            {
                GDEgoddessData g = new GDEgoddessData(GDEItemKeys.goddess_baseGoddess)
                {
                    id = rewardGoddesses[i]
                    ,
                    exp = 0
                    ,
                    star=0
                    ,
                    volume=0
                    ,
                    UseTeamId = new List<int>()
                };
                SDDataManager.Instance.PlayerData.goddessOwned.Add(g);
                SDDataManager.Instance.PlayerData.Set_goddessOwned();
            }
        }
        rewardGoddesses.Clear();
    }


    public void refreshGoddessPool()
    {
        int oneTimeCouponNum = SDDataManager.Instance.getMaterialNum("M_M#3010017");
        int tenTimeCouponNum = SDDataManager.Instance.getMaterialNum("M_M#3010018");
        int damondNum = SDDataManager.Instance.PlayerData.damond;
        //单抽按钮是否可用
        if (oneTimeCouponNum > 0 || damondNum >= SDConstants.altarDamondCost)
        {
            btn_goddess_oneTime.interactable = true;
        }
        else btn_goddess_oneTime.interactable = false;
        //十连按钮是否可用
        if (oneTimeCouponNum >= 10 || damondNum >= SDConstants.altarDamondCost * 10 
            || tenTimeCouponNum > 0)
        {
            btn_goddess_tenTimes.interactable = true;
        }
        else btn_goddess_tenTimes.interactable = false;
    }
    #endregion

    public override void commonBackAction()
    {       
        if(currentPanelContent == panelContent.main)//
        {
            base.commonBackAction();
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
