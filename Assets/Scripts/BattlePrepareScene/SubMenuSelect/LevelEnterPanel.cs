using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class LevelEnterPanel : MonoBehaviour
{
    HomeScene _hs;
    public HomeScene homeScene 
    {
        get 
        {
            if (_hs == null) _hs = FindObjectOfType<HomeScene>();
            return _hs;
        }
        
    }
    public ChapterLevelController CLC;
    //public SDConstants.GameType CurrentGameType;
    public enum panelContent
    {
        main,chapter,section,end
    }
    public panelContent currentPanelContent = panelContent.end;
    public void WhenOpenThisMenu()
    {
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

    public void commonBackAction()
    {
        if(currentPanelContent == panelContent.main)
        {
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
