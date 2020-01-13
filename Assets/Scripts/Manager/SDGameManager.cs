using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using I2.Loc;

/// <summary>
/// 游戏管理类，负责场景间数据传递
/// </summary>
public class SDGameManager : PersistentSingleton<SDGameManager>
{
    public bool isUsingProp = false;//正在使用道具
    #region level\section\bosssAppearLevel\chapter Data
    public int currentLevel;
    public int currentSection
    {
        get
        {
            return (currentLevel - currentLevel % SDConstants.LevelNumPerSection)
                / SDConstants.LevelNumPerSection;
        }
        set
        {
            currentLevel = value * SDConstants.LevelNumPerSection;
        }
    }
    public int current_level_in_section
    {
        get
        {
            return currentLevel % SDConstants.LevelNumPerSection;
        }
    }
    public int currentBossAppearLevelIndex
    {
        get 
        {
            return (currentLevel - currentLevel % SDConstants.PerBossAppearLevel)
                / SDConstants.PerBossAppearLevel;
        }
        set
        {
            currentLevel = value * SDConstants.PerBossAppearLevel;
        }
    }
    public int current_section_in_bossAppearL
    {
        get 
        {
            int sectionN = SDConstants.PerBossAppearLevel / SDConstants.LevelNumPerSection;
            return currentSection % sectionN;
        }
    }
    public int currentChapter
    {
        get
        {
            return (currentLevel - currentLevel % SDConstants.LevelNumPerChapter)
                / SDConstants.LevelNumPerChapter;
        }
        set
        {
            currentLevel = value * SDConstants.LevelNumPerChapter;
        }
    }
    public int current_bal_in_chapter
    {
        get 
        {
            int balN = SDConstants.PerBossAppearLevel / SDConstants.LevelNumPerChapter;
            return currentBossAppearLevelIndex % balN;
        }
    }

    public int startLevel;
    public int startSection
    {
        get 
        {
            return (startLevel - startLevel % SDConstants.LevelNumPerSection)
              / SDConstants.LevelNumPerSection;
        }
        set
        {
            startLevel = value * SDConstants.LevelNumPerSection;
        }
    }
    public int startBossAppearLevelIndex
    {
        get
        {
            return (startLevel - startLevel % SDConstants.PerBossAppearLevel)
                / SDConstants.PerBossAppearLevel;
        }
        set
        {
            startLevel = value * SDConstants.PerBossAppearLevel;
        }
    }
    public int startChapter
    {
        get
        {
            return (startLevel - startLevel % SDConstants.LevelNumPerChapter)
                / SDConstants.LevelNumPerChapter;
        }
        set
        {
            currentLevel = value * SDConstants.LevelNumPerChapter;
        }
    }
    #endregion
    //
    public bool showEnemyState = false;
    public bool openAllBonus = false;
    public bool alwaysGoodEvents = false;
    //
    public string currentHeroTeamId;
    //
    public bool isGamePaused = false;
    public bool isGaming = false;
    public bool isGameFinished = false;
    public bool canRevive = true;
    public bool isBuyItemFromStore = false;
    public bool isSellItemFromStore = false;
    public bool isHireHero = false;
    //public bool isSelectHero = false;
    public SDConstants.HeroSelectType heroSelectType;
    public SDConstants.StockUseType stockUseType;
    //public SDConstants.ItemType ItemSetType;
    //public SDConstants.HeroSelectType goddessSelectType;
    //
    public bool isFirstTimeEnterGame = false;
    public bool isFirstTimeInitAds = false;
    public bool isQuickSellItems = false;
    public bool isFromHeroManagerPanel = false;
    public int GameStartGoldNum = 0;
    public List<int> DropMaterials;
    public float goldRate = 1;
    public int LastMaxPassLevel;
    public bool isFastModeEnabled = false;
    public int FreeCoinGetTimes;

    //广告


    //Debug
    public bool DEBUG_GODLIKE = false;
    public bool DEBUG_NO_MP_USE = false;
    public bool DEBUG_NO_TP_USE = false;




    public SDConstants.GameType gameType = SDConstants.GameType.Normal;
    [NonSerialized]
    public bool isStrengthenEquip = false;




    public static string T(string s)
    {
        string r = LocalizationManager.GetTermTranslation(s);
        if (r == null || r.Length == 0)
        {
            r = s;
        }
        return r;
    }

    public void INIT()
    {
        Debug.Log("INIT");
        foreach(var talker in FindObjectsOfType<Talker>())
        {
            talker.Init();
        }
    }
}
