using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class OneAltarData : MonoBehaviour
{
    public Image BgImg;
    public Image PoolImg;
    public Image CoverRoleImg;
    public Text PoolDesc;
    //
    public enum PoolType 
    {
        basic
            ,
        theme
            ,
        rare
            ,
    }
    public PoolType altarPoolType;
    //
    public Button BtnDetail;
    public Button BtnOneTime;
    public Button BtnTenTimes;
    //
    public SummonAltarPanel SAP { get { return GetComponentInParent<SummonAltarPanel>(); } }

    public void initAltarPool()
    {


        refreshThisPool();
    }
    public void refreshThisPool()
    {
        if(altarPoolType == PoolType.basic)
        {

        }
        else if (altarPoolType == PoolType.theme)
        {
            int oneTimeCouponNum = SDDataManager.Instance.getConsumableNum
                (SAP.Coupon_n_oneTime.ID);
            int tenTimeCouponNum = SDDataManager.Instance.getConsumableNum
                (SAP.Coupon_n_tenTimes.ID);
            int damondNum = SDDataManager.Instance.PlayerData.damond;
            //单抽按钮是否可用
            if (oneTimeCouponNum > 0 || damondNum >= SDConstants.altarDamondCost)
            {
                BtnOneTime.interactable = true;
            }
            else BtnOneTime.interactable = false;
            //十连按钮是否可用
            if (oneTimeCouponNum >= 10 || damondNum >= SDConstants.altarDamondCost * 10 || tenTimeCouponNum > 0)
            {
                BtnTenTimes.interactable = true;
            }
            else BtnTenTimes.interactable = false;
        }
        else if(altarPoolType == PoolType.rare)
        {
            int rareRoleCouponNum = SDDataManager.Instance.getConsumableNum
                (SAP.Coupon_r_oneTimes.ID);
            if(rareRoleCouponNum > 0)
            {
                BtnOneTime.interactable = true;
            }
            else BtnOneTime.interactable = false;
            if(rareRoleCouponNum >= 10)
            {
                BtnTenTimes.interactable = true;
            }
            else BtnTenTimes.interactable = false;
        }
    }

    public void BtnCallOneTime()
    {
        if(altarPoolType == PoolType.rare)
        {
            SAP.SummonOneTime_r();
        }
        else SAP.SummonOneTime_n();
        refreshThisPool();
    }
    public void BtnCallTenTimes()
    {
        SAP.SumonTenTimes_n();
        refreshThisPool();
    }
    public void BtnShowDetail()
    {

    }
}
