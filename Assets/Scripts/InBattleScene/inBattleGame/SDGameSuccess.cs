using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SDGameSuccess : MonoBehaviour
{
    public Text goldText;
    //public Text levelRemarkText;
    //public int goldNum;
    //public Transform CritGoldBtn;
    public Transform nextSectionBtn;
    public Transform RetryBtn;
    public Transform BackBtn;

    public Text currPassLvText;
    public Text maxPassLvText;
    public Transform newBest;

    //public RTPageController pageController;
    public HEWPageController pageController;
    public ScrollRect scrollRect;

    public Transform bonusBGTrans;
    public bool isSuccess;
    public Text CoinGetText;
    public Text DropGetText;
    [Space(15)]
    public Transform gameFailPanel;
    //public Transform gameRevivePanel;
    public GameRevive GameR { get { return GetComponent<GameRevive>(); } }
    public GameController GameC { get { return GetComponentInParent<GameController>(); } }
    public void initGameFinishLayer(bool isSuccess)
    {
        Time.timeScale = 1;
        //
        UIEffectManager.Instance.showAnimFadeIn(gameFailPanel);        
        //
        SDDataManager.Instance.SettingData.battleTimes++;
        //
        SDGameManager.Instance.goldRate = 1;
        SDGameManager.Instance.isFastModeEnabled = false;
        //
        this.isSuccess = isSuccess;
        //
        if (!isSuccess)//战斗失败
        {
            RetryBtn.gameObject.SetActive(true);
            nextSectionBtn.gameObject.SetActive(false);
            removeAllRewards();
            // 用于解锁教程的第一次失败
            if (SDDataManager.Instance.SettingData.first_time_fail_game == 0)
            {
                SDDataManager.Instance.SettingData.first_time_fail_game = 1;
                //RetryBtn.gameObject.SetActive(false);
                RetryBtn.GetComponent<Button>().interactable = false;
                //第一次失败弹窗
                string message = "第一次失败";

            }
            else
            {
                RetryBtn.GetComponent<Button>().interactable = true;
            }
        }
        else//战斗成功
        {
            RetryBtn.gameObject.SetActive(false);
            nextSectionBtn.gameObject.SetActive(true);
            StoreAllRewards();
            //
            nextSectionBtn.GetComponent<Button>().interactable = false;
        }
        initAllDropsAndCoins();
    }
    public void initAllDropsAndCoins()
    {
        pageController.ResetPage();
        pageController.showAllDropItems();
        pageController.transform.localScale = new Vector3(1, 0, 1);
        pageController.transform.DOScale(Vector3.one, 0.3f);
    }
    public void removeAllRewards()
    {
        GameC.allCoinsGet = 0;
        GameC.allDropsGet.Clear();
    }
    public void StoreAllRewards()
    {
        GameC.storeFinalDrops();
        SDDataManager.Instance.AddCoin(GameC.allCoinsGet);
    }



    public void Btn_back()
    {
        UIEffectManager.Instance.hideAnimFadeOut(this.transform);
        SLLoadingSceneController.loadingScenePreAnimationName = "black";
        SLLoadingSceneController.LoadScene("MenuScene");
    }
    public void Btn_retry()
    {

    }
    public void Btn_nextSection()
    {

    }
}
