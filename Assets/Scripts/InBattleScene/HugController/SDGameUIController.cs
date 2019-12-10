using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏场景UI按钮控制器
/// </summary>
public class SDGameUIController : MonoBehaviour
{
    [Header("自动战斗")]
    public Image imgAutoBattle;
    public Sprite[] spsAutoBattle;
    public Text autoBattleText;
    //
    [Header("快速战斗")]
    public Image imgFast;
    public Sprite[] spsFast;
    public Sprite[] spsFastNoVip;
    public int CurrentSpeed;
    //
    [Header("设置")]
    public Transform settingBtn;
    public Transform SettingPanel;



    [Space(15)]
    public float TIME = 1f;
    public BattleManager BM;
    public Transform autoBattleHint;
    private void Start()
    {
        //自动战斗---记录读取
        if (SDDataManager.Instance.SettingData.isAutoBattle)
        {
            imgAutoBattle.sprite = spsAutoBattle[1];
            autoBattleText.text = "自动战斗";
        }
        else
        {
            imgAutoBattle.sprite = spsAutoBattle[0];
            autoBattleText.text = "";
        }
        //战斗速度---记录读取
        CurrentSpeed = SDDataManager.Instance.SettingData.BattleSpeed;
        if (SDDataManager.Instance.SettingData.FastLeftTime>0)
        {
            SDGameManager.Instance.isFastModeEnabled = true;
        }
        //
        if(!SDDataManager.Instance.SettingData.isFastModeEnabled 
            && SDGameManager.Instance.isFastModeEnabled)
        {
            if(CurrentSpeed == 2)
            {
                CurrentSpeed = 1;
                SDDataManager.Instance.SettingData.BattleSpeed = CurrentSpeed;
            }
        }
        refreshSpeedBtn();
    }
    private void Update()
    {
        if (TIME > 0)
        {
            TIME -= Time.unscaledDeltaTime;
            return;
        }
        TIME = 1f;

    }
    public void autoBattleBtnTapped()
    {
        if (SDGameManager.Instance.isUsingProp) return;
        //if(SDDataManager.Instance.getd)
        if (SDDataManager.Instance.SettingData.autoBattleHint == 0)
        {
            SDDataManager.Instance.SettingData.autoBattleHint = 1;
            autoBattleHint.gameObject.SetActive(false);
        }
        if (SDDataManager.Instance.SettingData.isAutoBattle)
        {
            SDDataManager.Instance.SettingData.isAutoBattle = false;
            imgAutoBattle.sprite = spsAutoBattle[0];
            autoBattleText.text = "";
        }
        else
        {
            SDDataManager.Instance.SettingData.isAutoBattle = true;
            imgAutoBattle.sprite = spsAutoBattle[1];
            autoBattleText.text = "自动战斗";
        }
    }

    public void speedBtnTapped()
    {
        if (SDDataManager.Instance.SettingData.isFastModeEnabled || SDGameManager.Instance.isFastModeEnabled)
        {
            CurrentSpeed = (CurrentSpeed + 1) % 3;
            SDDataManager.Instance.SettingData.BattleSpeed = CurrentSpeed;
            refreshSpeedBtn();
        }
        else
        {
            CurrentSpeed = (CurrentSpeed + 1) % 2;
            SDDataManager.Instance.SettingData.BattleSpeed = CurrentSpeed;
            Debug.Log("速度修改按钮CurrentSpeed: " + SDDataManager.Instance.SettingData.BattleSpeed);
            refreshSpeedBtn();
        }
    }
    public void refreshSpeedBtn()
    {
        if(CurrentSpeed == 0)
        {
            normalSpeedBtnTapped();
        }
        else if(CurrentSpeed == 1)
        {
            twoSpeedBtnTapped();
        }
        else if(CurrentSpeed == 2)
        {
            threeSpeedBtnTapped();
        }
    }
    public void normalSpeedBtnTapped()
    {
        Time.timeScale = 1.0f;
        if (SDDataManager.Instance.SettingData.isFastModeEnabled)
        {
            imgFast.sprite = spsFast[0];
        }
        else
        {
            imgFast.sprite = spsFastNoVip[0];
        }
    }
    public void twoSpeedBtnTapped()
    {
        Time.timeScale = 2.0f;
        if (SDDataManager.Instance.SettingData.isFastModeEnabled)
        {
            imgFast.sprite = spsFast[1];
        }
        else
        {
            imgFast.sprite = spsFastNoVip[1];
        }
    }
    public void threeSpeedBtnTapped()
    {
        Time.timeScale = 4.0f;
        imgFast.sprite = spsFast[2];
    }


    public void homeBtnTapped()
    {
        LoadingSceneController.loadingScenePreAnimationName = "";
        LoadingSceneController.LoadScene("HomeScene");
        //SDGameManager.Instance.ads
    }
}
