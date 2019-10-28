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
    public SDConstants.GameType CurrentGameType;
    public void WhenOpenThisMenu()
    {
        CLC.transform.localScale = Vector3.zero;

    }
    public void BtnToEnterMainSeries(int index)
    {
        enterSelectedChapter(index);
    }
    public void enterSelectedChapter(int index
        , SDConstants.GameType GType = SDConstants.GameType.Normal)
    {
        UIEffectManager.Instance.showAnimFadeIn(CLC.transform);
        CLC.FirstlyShowLevelList();

    }
}
