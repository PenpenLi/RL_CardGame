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
    public int currentLevel;
    public int startLevel;
    //
    public bool showEnemyState = false;
    public bool openAllBonus = false;
    public bool alwaysGoodEvents = false;
    //
    public int currentHeroTeamIndex;
    //
    public bool isGamePaused = false;
    public bool isGaming = false;
    public bool isGameFinished = false;
    public bool canRevive = true;
    public bool isBuyItemFromStore = false;
    public bool isSellItemFromStore = false;
    public bool isHireHero = false;
    public bool isSelectHero = false;
    public SDConstants.HeroSelectType heroSelectType;
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
}
