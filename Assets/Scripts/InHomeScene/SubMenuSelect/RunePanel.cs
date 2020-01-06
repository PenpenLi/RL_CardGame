using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class RunePanel : BasicSubMenuPanel
{
    [Space(25)]
    public HEWPageController Page;
    public RuneDetailPanel runeDetail;
    public int currentRuneHashcode
    {
        get { return runeDetail.hashcode; }
        set
        {
            if(runeDetail.hashcode != value)
            {
                GDERuneData rune = SDDataManager.Instance.getRuneOwnedByHashcode(value);
                if (rune == null) return;
                runeDetail.initDetailPanel(rune);
                resetLvupVision();
                refreshComposeCondition();
            }
        }
    }
    [Header("升级页面设置")]
    public Text CoinCostText;
    public Image CoinIcon;
    public Button LvUpConfirmBtn;
    public Button MinusBtn;
    public Button PlusBtn;
    public Text LevelUpNumText;
    [ReadOnly]
    public int ExpectLvupNum = 0;
    [Header("合成页面设置")]
    public simpleSlotSet[] ComposeMaterialSlots;
    public Button ComposeConfirmBtn;
    public Button ComposeCancelBtn;
    public bool runeCanAddToCompose;
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        //RefreshComposePanel();
        ResetComposePanel();
        //refreshPage();
        //initPage();
        resetLvupVision();
        if (currentRuneHashcode > 0) return;
        if(Page.itemCount>0)
            currentRuneHashcode = Page.items[0].itemHashcode;
        //
        refreshPage();
    }
    public override void commonBackAction()
    {
        base.commonBackAction();
        homeScene.SubMenuClose();
        if(panelFrom != HomeScene.HomeSceneSubMenu.End)
        {
            homeScene.CurrentSubMenuType = panelFrom;
        }
    }

    #region compose
    public void ResetComposePanel()
    {
        for (int i = 0; i < ComposeMaterialSlots.Length; i++)
        {
            simpleSlotSet slot = ComposeMaterialSlots[i];
            slot.resetOnBtnTapped();
            slot.OnBtnTapped += ClickSlot;
            slot.hashcode = 0;
            //
            if(!slot.isLocked)
                slot.isEmpty = true;
        }
        initPage();
    }
    public void ClickSlot(int index)
    {
        if (!runeCanAddToCompose) return;
        GDERuneData rune = SDDataManager.Instance.
            getRuneOwnedByHashcode(currentRuneHashcode);

        ComposeMaterialSlots[index].initRune(rune);
        refreshPage();
    }
    public void refreshComposeCondition()
    {
        for (int i = 0; i < ComposeMaterialSlots.Length; i++)
        {
            if(currentRuneHashcode == ComposeMaterialSlots[i].hashcode)
            {
                runeCanAddToCompose = false;return;
            }
        }
        if (!string.IsNullOrEmpty(runeDetail.ownerId) || runeDetail.locked)
        {
            runeCanAddToCompose = false;return;
        }
        runeCanAddToCompose = true;return;
    }
    public void initPage()
    {
        SDGameManager.Instance.stockUseType = SDConstants.StockUseType.detail;
        Page.ItemsInit(SDConstants.ItemType.Rune);
    }
    public void refreshPage()
    {
        foreach(SingleItem s in Page.items)
        {
            s.isSelected = false;
        }
        for(int i = 0; i < ComposeMaterialSlots.Length; i++)
        {
            simpleSlotSet slot = ComposeMaterialSlots[i];
            if (slot.hashcode > 0)
            {
                foreach(SingleItem s in Page.items)
                {
                    if(s.itemHashcode == slot.hashcode)
                    {
                        s.isSelected = true;break;
                    }
                }
            }
        }
        foreach(SingleItem s in Page.items)
        {
            if(s.itemHashcode == currentRuneHashcode)
            {
                s.isSelected = true;
                //s.SelectedPanel.GetComponent<Image>().color = new Color(255,225,255,0.5f);
            }
            else
            {
                //s.SelectedPanel.GetComponent<Image>
            }
        }
        refreshComposeCondition();
    }


    public void Btn_compose_cancel()
    {
        for(int i = 0; i < ComposeMaterialSlots.Length; i++)
        {
            simpleSlotSet slot = ComposeMaterialSlots[i];
            slot.initRune(null);
        }
        refreshPage();
    }
    public void Btn_compose_confirm()
    {
        GDERuneData rune0 = SDDataManager.Instance.getRuneOwnedByHashcode
            (ComposeMaterialSlots[0].hashcode);
        GDERuneData rune1 = SDDataManager.Instance.getRuneOwnedByHashcode
            (ComposeMaterialSlots[1].hashcode);
        GDERuneData rune2 = SDDataManager.Instance.getRuneOwnedByHashcode
            (ComposeMaterialSlots[2].hashcode);
        //
        if (SDDataManager.Instance.CheckIfCanComposeToCreateNewRune
            (rune0,rune1,rune2,out string result))
        {
            SDDataManager.Instance.ConsumeRune(rune0.hashcode);
            SDDataManager.Instance.ConsumeRune(rune1.hashcode);
            SDDataManager.Instance.ConsumeRune(rune2.hashcode);
            SDDataManager.Instance.AddRune(result);
            //
            RuneItem RI = SDDataManager.Instance.getRuneItemById(result);
            PopoutController.CreatePopoutMessage("成功呢获得 "+RI.NAME, 10);
            ResetComposePanel();
            //refreshPage();
        }
    }
    #endregion

    #region lvup
    public void resetLvupVision()
    {
        ExpectLvupNum = 0;
        refreshLvupVision();
    }
    public void refreshLvupVision()
    {
        LevelUpNumText.text = "+ " + ExpectLvupNum;
        int allCost = SDDataManager.Instance.getCoinWillImproveCost
            (runeDetail.level, runeDetail.quality, ExpectLvupNum);
        CoinCostText.text = "" + allCost;
        //
        if (ExpectLvupNum <= 0) MinusBtn.interactable = false;
        else MinusBtn.interactable = true;
        //
        int newCost = SDDataManager.Instance.getCoinWillImproveCost
            (runeDetail.level, runeDetail.quality, ExpectLvupNum + 1);
        if (ExpectLvupNum >= SDConstants.RuneMaxLevel 
            || newCost >= SDDataManager.Instance.PlayerData.coin)
        {
            PlusBtn.interactable = false;
        }
        else PlusBtn.interactable = true;
        //
        if (ExpectLvupNum > 0) LvUpConfirmBtn.interactable = true;
        else LvUpConfirmBtn.interactable = false;
    }
    public void btn_lvup_minus()
    {
        if (ExpectLvupNum <= 0) return;
        ExpectLvupNum--;
        refreshLvupVision();
    }
    public void btn_lvup_plus()
    {
        if (ExpectLvupNum >= SDConstants.RuneMaxLevel) return;
        ExpectLvupNum++;
        refreshLvupVision();
    }
    public void Btn_lvup_confirm()
    {
        if(ExpectLvupNum>0)
            if (SDDataManager.Instance.lvUpRune(currentRuneHashcode, ExpectLvupNum))
            {
                Debug.Log("rune:" + currentRuneHashcode + " 成功提升" + ExpectLvupNum + "级");
                //effect-lvup
                ExpectLvupNum = 0;

                runeDetail.initDetailPanel(currentRuneHashcode);
            }
    }
    #endregion
}
