using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SDGameSuccess : MonoBehaviour
{
    public Text goldText;
    //public Text levelRemarkText;
    public int goldNum;
    public Transform CritGoldBtn;
    public Transform RetryBtn;

    public Text currPassLvText;
    public Text maxPassLvText;
    public Transform newBest;

    public RTPageController pageController;
    public ScrollRect scrollRect;

    public Transform bonusBGTrans;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void initGameSuccessPanel(int goldGet)
    {
        refreshHutBgAndTitle();
        //hut
        Time.timeScale = 1;
        SDDataManager.Instance.SettingData.battleTimes++;
        //rtratecontroller
        goldNum = goldGet;
        goldText.text = "<color=#FFFFFF>①</color>" + goldNum;
        //
        SDGameManager.Instance.goldRate = 1;
        SDGameManager.Instance.isFastModeEnabled = false;
        if (SDDataManager.Instance.SettingData.first_time_fail_game == 0)
        {
            SDDataManager.Instance.SettingData.first_time_fail_game = 1;
            CritGoldBtn.gameObject.SetActive(false);
            RetryBtn.GetComponent<Button>().interactable = false;
            //第一次失败弹窗
            string message = "第一次失败";

        }
        else
        {
            CritGoldBtn.gameObject.SetActive(true);
            RetryBtn.GetComponent<Button>().interactable = true;
        }

        int currLv = Mathf.Max(SDGameManager.Instance.currentLevel - 1,0);
        int maxLv = SDGameManager.Instance.LastMaxPassLevel;
        currPassLvText.text = "" + currLv;
        maxPassLvText.text = "";
        newBest.gameObject.SetActive(currLv > maxLv ? true : false);
        int maxPassLevel = SDDataManager.Instance.GetMaxPassLevel();
        if (maxPassLevel > SDConstants.chapterMaxNum) maxPassLevel = SDConstants.chapterMaxNum;
        maxPassLvText.text = "Max_Pass_Level" + Mathf.Min(maxPassLevel
            , (SDDataManager.Instance.getDimension() + 1) * SDConstants.LevelNumPerChapter);
        refreshAutoHangBtnState();
        //autoCloseText.gameObject.SetActive(false);
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Normal 
            && SDDataManager.Instance.SettingData.isAutoHang)
        {
            //自动重新开始战斗
        }

        pageController.type = SDConstants.BagItemType.Material;
        bool flag = pageController.showDropMaterials();
        //prop掉落

        //
        bonusBGTrans.gameObject.SetActive(!flag);
        resetScollViewPosition();

    }
    public void refreshHutBgAndTitle()
    {
        if(SDGameManager.Instance.gameType == SDConstants.GameType.Dungeon)
        {

        }
        else if(SDGameManager.Instance.gameType == SDConstants.GameType.Hut)
        {

        }
    }
    public void refreshAutoHangBtnState()
    {
        if (SDDataManager.Instance.SettingData.isAutoHang)
        {

        }
        else
        {

        }
    }
    public void resetScollViewPosition()
    {
        scrollRect.verticalScrollbar.value = 1;
        StartCoroutine(IEResetScollViewPosition());
    }

    public IEnumerator IEResetScollViewPosition()
    {
        yield return new WaitForSeconds(1f);
        scrollRect.verticalScrollbar.value = 1;
    }
}
