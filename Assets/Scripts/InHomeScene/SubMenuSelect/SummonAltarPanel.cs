using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using GameDataEditor;
using System;

public class SummonAltarPanel : BasicSubMenuPanel
{
    public ScrollRect rect;
    public PageView pageView { get { return rect.GetComponent<PageView>(); } }
    public OneAltarData AltarData;
    [Space]
    public consumableItem Coupon_n_oneTime;
    public consumableItem Coupon_n_tenTimes;
    public consumableItem Coupon_r_oneTimes;
    public enum AltarType
    {
        n_oneTime,n_tenTime,r_oneTime,
    }
    public AltarType CurrentAltarType;

    [Header("抽取设置"), Space]
    public float[] AltarPossibility = new float[4];
    public ROHeroAltarPool CurrentPool;
    public List<GDEHeroAltarPoolData> AllPools;
    [Header("抽卡结果页面显示")]
    public Transform ResultRewardPanel;
    public Transform RRPContent;
    public Transform roleItem;
    public List<SingleItem> resultRewards = new List<SingleItem>();
    public List<int> RewardHCs = new List<int>();
    public Text messageText;
    public enum panelContent
    {
        main,rrp,pose,end
    }
    [Header("历史记录设置")]
    public panelContent currentPanelContent = panelContent.end;

    [Space(15)]
    public DrawAnimRound DAP;
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        currentPanelContent = panelContent.main;
        RewardHCs.Clear();
        refreshCurrentPoolIndex();
        DAP.AnimOn = false;
    }

    public void SummonOneTime_n()
    {
        CurrentAltarType = AltarType.n_oneTime;
        if (SDDataManager.Instance.getConsumableNum(Coupon_n_oneTime.ID) > 0)
        {
            InitDAP();
        }
        else
        {
            if (SDDataManager.Instance.PlayerData.damond >= SDConstants.altarDamondCost)
            {
                PopoutController.CreatePopoutAlert
                    (
                    SDGameManager.T("提醒")
                    ,SDGameManager.T("确认消耗") 
                    + SDConstants.altarDamondCost 
                    + SDGameManager.T("钻石")
                    , 25, false,
                    PopoutController.PopoutWIndowAlertType.ConfirmMessage
                    ,(PopoutController pop
                    , PopoutController.PopoutWindowAlertActionType type)
                      =>
                      {
                          if(type == PopoutController.PopoutWindowAlertActionType.OnConfirm)
                          {
                              InitDAP(); 
                          }
                          else
                          {
                              
                          }
                          StartCoroutine(pop.Dismiss());
                      }
                    );
            }
            else
            {
                Debug.Log("无法获取：道具或钻石不足");
                return;
            }
        }
    }
    public void SummonOneTime_r()
    {
        CurrentAltarType = AltarType.r_oneTime;
        if (SDDataManager.Instance.getConsumableNum(Coupon_r_oneTimes.ID) > 0)
        {
            InitDAP();
        }
        else
        {
            Debug.Log("无法获取：道具不足");
            return;
        }
    }
    public void SumonTenTimes_n()
    {
        CurrentAltarType = AltarType.n_tenTime;
        if (SDDataManager.Instance.getConsumableNum(Coupon_n_tenTimes.ID) > 0)
        {
            InitDAP();
        }
        else
        {
            if (SDDataManager.Instance.getConsumableNum(Coupon_n_oneTime.ID) > 10)
            {
                InitDAP();
            }
            else
            {
                if (SDDataManager.Instance.PlayerData.damond
                    >= SDConstants.altarDamondCost * 10)
                {
                    if (SDDataManager.Instance.PlayerData.damond >= SDConstants.altarDamondCost)
                    {
                        PopoutController.CreatePopoutAlert
                            (
                            SDGameManager.T("提醒")
                            , SDGameManager.T("确认消耗")
                            + SDConstants.altarDamondCost * 10
                            + SDGameManager.T("钻石")
                            , 25, false,
                            PopoutController.PopoutWIndowAlertType.ConfirmMessage
                            , (PopoutController pop
                             , PopoutController.PopoutWindowAlertActionType type)
                               =>
                            {
                                if (type == PopoutController.PopoutWindowAlertActionType.OnConfirm)
                                {
                                    InitDAP(); //pop.Dismiss();
                                }
                                else
                                {
                                    //pop.Dismiss();
                                }
                                StartCoroutine(pop.Dismiss());
                            }
                            );
                    }
                    else
                    {
                        Debug.Log("无法获取：道具或钻石不足");
                        return;
                    }
                }
            }
        }
    }
    void InitDAP()
    {
        DAP.ResetSelf();
        //
        UIEffectManager.Instance.showAnimFadeIn(DAP.transform);

        DAP.AnimOn = true;
        //

        if (CurrentAltarType == AltarType.n_tenTime)
        {
            DAP.useLR = true;
        }
        else DAP.useLR = false;
        DAP.clearOAESE();
        DAP.CurrentAltarType = CurrentAltarType;
        if (CurrentAltarType == AltarType.n_oneTime)
        {
            DAP.OnAnimEndSetEvent.AddListener(confirm_altar_n_oneTime);
        }
        else if (CurrentAltarType == AltarType.n_tenTime)
        {
            DAP.OnAnimEndSetEvent.AddListener(confirm_altar_n_tenTimes);
        }
        else if(CurrentAltarType == AltarType.r_oneTime)
        {
            DAP.OnAnimEndSetEvent.AddListener(confirm_altar_r_oneTime);
        }
    }
    public void confirm_altar_n_oneTime()
    {
        bool flag = false;
        if (SDDataManager.Instance.getConsumableNum(Coupon_n_oneTime.ID) > 0)
        {
            SDDataManager.Instance.consumeConsumable(Coupon_n_oneTime.ID,out int left);
            flag = true;
        }
        else if(SDDataManager.Instance.PlayerData.damond >= SDConstants.altarDamondCost)
        {
            SDDataManager.Instance.ConsumeDamond(SDConstants.altarDamondCost);
            flag = true;
        }
        if (flag)
        {
            AltarHero(1, false);
        }
        UIEffectManager.Instance.hideAnimFadeOut(DAP.transform);
    }
    public void confirm_altar_n_tenTimes()
    {
        bool flag = false;
        if (SDDataManager.Instance.getConsumableNum(Coupon_n_tenTimes.ID) > 0)
        {
            SDDataManager.Instance.consumeConsumable(Coupon_n_tenTimes.ID, out int left);
            flag = true;
        }
        else if (SDDataManager.Instance.getConsumableNum(Coupon_n_oneTime.ID) > 10)
        {
            SDDataManager.Instance.consumeConsumable(Coupon_n_oneTime.ID, out int left, 10);
            flag = true;
        }
        else if(SDDataManager.Instance.PlayerData.damond >= SDConstants.altarDamondCost * 10)
        {
            SDDataManager.Instance.ConsumeDamond(SDConstants.altarDamondCost * 10);
            flag = true;
        }
        if (flag)
        {
            AltarHero(10, false);
        }
        UIEffectManager.Instance.hideAnimFadeOut(DAP.transform);
    }
    public void confirm_altar_r_oneTime()
    {
        bool flag = false;
        if (SDDataManager.Instance.getConsumableNum(Coupon_r_oneTimes.ID) > 0)
        {
            SDDataManager.Instance.consumeConsumable(Coupon_r_oneTimes.ID, out int left);
            flag = true;
        }
        if (flag)
        {
            AltarHero(1, true);
        }
        UIEffectManager.Instance.hideAnimFadeOut(DAP.transform);
    }

    bool HaveEnoughConsumable(consumableItem item,int num = 1)
    {
        int n = SDDataManager.Instance.getConsumableNum(Coupon_r_oneTimes.ID);
        return n >= num;
    }
    bool HaveEnoughDamond(int num)
    {
        int n = SDDataManager.Instance.GetDamond();
        return n >= num;
    }




    public void AltarHero(int times, bool useRareCoupon)
    {
        RewardHCs.Clear();
        for(int i = 0; i < times; i++)
        {
            HeroInfo H = SDDataManager.Instance.AltarInOnePool
                (AltarPossibility, CurrentPool.Pool.ID,useRareCoupon);
            int hc = SDDataManager.Instance.addHero(H.ID);
            RewardHCs.Add(hc);
        }
        initRRP();
    }
    bool excapePose = false;
    public void BtnToExcapePose() 
    {
        if (!excapePose) 
        {
            excapePose = true;
        }
    }
    public void initRRP()
    {
        UIEffectManager.Instance.showAnimFadeIn(ResultRewardPanel);
        SDGameManager.Instance.heroSelectType = SDConstants.HeroSelectType.Altar;
        //构建角色卡牌
        for (int i = 0; i < RewardHCs.Count; i++)
        {
            Transform s = Instantiate(roleItem) as Transform;
            s.SetParent(RRPContent);
            s.localScale = Vector3.one;
            s.gameObject.SetActive(true);
            SingleItem _s = s.GetComponent<SingleItem>();
            _s.initCalledHero(RewardHCs[i]);
            resultRewards.Add(_s);
        }
        //
        StartCoroutine(IEInitCallingRewards());
    }
    public IEnumerator IEInitCallingRewards()
    {
        homeScene.mapBtn.gameObject.SetActive(false);
        for (int i = 0; i < RewardHCs.Count; i++)
        {
            bool firstGet = true;
            GDEHeroData heroData = SDDataManager.Instance.getHeroByHashcode(RewardHCs[i]);
            string id = heroData.id;
            List<GDEHeroData> _List = SDDataManager.Instance.PlayerData.herosOwned.FindAll
                (x => x.id == id);
            if (_List.Count > 1) firstGet = false;
            resultRewards[i].initCalledHero(RewardHCs[i]);
            if (firstGet)
            {
                currentPanelContent = panelContent.pose;
                resultRewards[i].upText.text = "NEW!!!";

                if (!excapePose)
                {
                    PopoutController.CreatePopoutUnitSpeak("英雄大名"
                        , "久仰大名", "Textures/xgb33", 10 + RewardHCs.Count - i
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
        currentPanelContent = panelContent.rrp;
        homeScene.mapBtn.gameObject.SetActive(true);
    }
    public void exitRRP()
    {
        RewardHCs.Clear();
        for(int i = 0; i < resultRewards.Count; i++)
        {
            Destroy(resultRewards[i].gameObject);
        }
        resultRewards.Clear();
        currentPanelContent = panelContent.main;
        UIEffectManager.Instance.hideAnimFadeOut(ResultRewardPanel);
        //
        refreshCurrentPoolIndex();
    }
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



    #region 构建卡池
    public void refreshCurrentPoolIndex()
    {
        messageText.text = "水晶数：" + SDDataManager.Instance.PlayerData.damond
            + "  ---单抽券数：" + SDDataManager.Instance.getConsumableNum(Coupon_n_oneTime.ID)
            + "  ---十连券数：" + SDDataManager.Instance.getConsumableNum(Coupon_n_tenTimes.ID);
        //
        pageView.maxIndex = SDDataManager.Instance.PlayerData.AltarPoolList.Count;
        AllPools = SDDataManager.Instance.PlayerData.AltarPoolList;
        AllPools.Sort((x,y) => 
        {
            return x.Unable.CompareTo(!y.Unable);
        });
        buildCurrentPool();
    }
    public void buildCurrentPool()
    {
        int index = Mathf.Min(pageView.currentIndex,AllPools.Count-1);
        GDEHeroAltarPoolData PoolData = AllPools[index];
        HeroAltarPool P = SDDataManager.Instance.GetHeroAltarPoolById(PoolData.ID);
        CurrentPool = new ROHeroAltarPool()
        {
            Pool = P
            , HaveAltarTimes = PoolData.AltarTimes
            , HaveAlartSNum = PoolData.GetSNum
            , PoolCapacity=PoolData.PoolCapacity
            , Unable = PoolData.Unable
            , NotNormalPool=PoolData.NotNormalPool
        };
    }

    #endregion
}
