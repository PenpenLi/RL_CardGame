using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRevive : MonoBehaviour
{
    public Transform gameFailPanel;
    public Transform gameRevivePanel;
    public Text reviveTimesText;
    public Text autoCloseText;
    private int CDTime;
    private float countDownTime;
    public SDGameSuccess GameS { get { return GetComponent<SDGameSuccess>(); } }
    public void initGameRevive()
    {
        Time.timeScale = 1;
        //
        UIEffectManager.Instance.hideAnimFadeOut(gameFailPanel);
        UIEffectManager.Instance.showAnimFadeIn(gameRevivePanel);
        GameS.initAllDropsAndCoins();
        //
        autoCloseText.gameObject.SetActive(false);
        if(SDDataManager.Instance.SettingData.isAutoHang
            || SDDataManager.Instance.SettingData.isAutoBattle)
        {
            Invoke("closeBtnTapped", 10f);
            CDTime = 10;
            countDownTime = 1f;
            autoCloseText.gameObject.SetActive(true);
            autoCloseText.text = SDGameManager.T("AutoCloseCDTimes") + "00:10";
        }
        reviveTimesText.text = "";
    }
    public void cancelInvokeReplayClose()
    {
        autoCloseText.gameObject.SetActive(false);
        CancelInvoke("closeBtnTapped");
    }
    public void revive()
    {
        UIEffectManager.Instance.hideAnimFadeOut(transform);
        SLLoadingSceneController.loadingScenePreAnimationName = "";
        SLLoadingSceneController.LoadScene("BattleScene");
    }
    public void closeBtnTapped()
    {
        UIEffectManager.Instance.hideAnimFadeOut(gameRevivePanel);
        UIEffectManager.Instance.showAnimFadeIn(gameFailPanel);
        GameS.initGameFinishLayer(false);
    }

    private void FixedUpdate()
    {
        if (CDTime > 0 && countDownTime > 0)
        {
            countDownTime -= Time.deltaTime;
            return;
        }
        countDownTime = 1.0f;
        CDTime--;
        autoCloseText.text = SDGameManager.T("AutoCloseCDTime") + "00:0" + CDTime;
    }
}
