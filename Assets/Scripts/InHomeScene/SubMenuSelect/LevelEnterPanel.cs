using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class LevelEnterPanel : BasicSubMenuPanel
{
    public ChapterLevelController CLC;
    //public SDConstants.GameType CurrentGameType;
    public enum panelContent
    {
        main,chapter,section,end
    }
    public panelContent currentPanelContent = panelContent.end;
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        CLC.transform.localScale = Vector3.zero;
        currentPanelContent = panelContent.main;
    }
    public void BtnToEnterMainSeries(int index)
    {
        enterSelectedChapter(index);
    }
    public void enterSelectedChapter(int index
        , SDConstants.GameType GType = SDConstants.GameType.Normal)
    {
        UIEffectManager.Instance.showAnimFadeIn(CLC.transform);
        currentPanelContent = panelContent.chapter;
        CLC.FirstlyShowLevelList();
    }

    public override void commonBackAction()
    {      
        if(currentPanelContent == panelContent.main)
        {
            base.commonBackAction();
            homeScene.SubMenuClose();
        }
        else if(currentPanelContent == panelContent.chapter)
        {
            UIEffectManager.Instance.hideAnimFadeOut(CLC.transform);
            currentPanelContent = panelContent.main;
        }
        else if(currentPanelContent == panelContent.section)
        {

        }
        else if(currentPanelContent == panelContent.end)
        {

        }
    }
}
