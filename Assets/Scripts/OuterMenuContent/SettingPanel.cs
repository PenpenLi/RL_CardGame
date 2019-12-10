using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [Space(25),SerializeField]
    private bool _isInBattleScene;
    public bool IsInBattleScene { get { return _isInBattleScene; } }
    [HideInInspector]
    public bool thisPanelIsOpened;
    public Button Btn_back;
    public Button Btn_giveUp;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }
    public void OpenThisPanel()
    {
        thisPanelIsOpened = true;
        UIEffectManager.Instance.showAnimFadeIn(transform);
        if (IsInBattleScene)
        {
            SDDataManager.Instance.SettingData.isAutoBattle = false;
        }
    }

    public void CloseThisPanel()
    {
        thisPanelIsOpened = false;
        UIEffectManager.Instance.hideAnimFadeOut(transform);
    }


    public void BtnBack()
    {
        CloseThisPanel();
    }
    public void BtnGiveUpBattle()
    {
        BattleManager BM = FindObjectOfType<BattleManager>();
        if (BM)
        {
            //SDGameManager.Instance.isGamePaused = true;
            //BM.BattleFail();
            PopoutController.CreatePopoutAlert("警告","确定放弃战斗吗</n>(有死之荣无生之辱)"
                ,50,true,PopoutController.PopoutWIndowAlertType.ConfirmOrCancel
                ,(PopoutController c, PopoutController.PopoutWindowAlertActionType a)=>
                {
                    if(a == PopoutController.PopoutWindowAlertActionType.OnConfirm)
                    {
                        SDGameManager.Instance.isGamePaused = true;
                        BM.BattleFail();
                    }
                    else
                    {
                        c.StartCoroutine(c.IEWaitAndDismiss(0.2f));
                    }
                });
        }
    }
}
