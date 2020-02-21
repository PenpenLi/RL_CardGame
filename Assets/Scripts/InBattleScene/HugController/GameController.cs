using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameDataEditor;
using UnityEngine.U2D;
using System.Linq;
//using Colorful;

/// <summary>
/// 游戏控制类
/// </summary>
public class GameController : MonoBehaviour
{
    public static GameController Instantce;
    public BattleManager BM;
    public GoddessManager GM
    {
        get { return GetComponentInChildren<GoddessManager>(); }
    }
    public Transform[] HeroPos;
    public Transform[] HeroEmptyEffect;
    public Transform[] EnemyPos;
    public Transform[] EnemyPosBgEffect;
    public Transform heroPrefab;
    public Transform enemyPrefab;
    public Transform bossPrefab;
    public Transform heroParent;
    public Transform enemyParent;
    [Space(10)]
    #region 关卡名称与开场动画
    public Text levelText;
    public Animation levelStartAnim;
    public Text levelStartText;
    #endregion
    public Image backgroundImg;
    public List<Sprite> backgroundSps = new List<Sprite>();
    public string currentLevelTheme;
    //
    public Transform nextLevelLayer;
    public Image imgBlack;
    [HideInInspector]
    public List<string> startEnemyGroup;
    [Space(25)]
    public LevelbarManager LBM;
    [Space(15)]
    public int FatigueAddNum = SDConstants.fatigueAddNum;
    #region 结算页面
    [Header("结算UI")]
    public Transform introPanel;
    public Transform gameEndLayer;
    public Transform gameBonusLayer;
    public Transform gameEventLayer;
    [Space(15)]
    public int allCoinsGet;
    public List<GDEItemData> allDropsGet = new List<GDEItemData>();
    #endregion

    private void Awake()
    {
        Instantce = this;
        //if(SDDataManager.Instance.SettingData.)
    }
    public void registerGameObjectPoolEffects()
    {
        //特效
    }
    private void Start()
    {
        registerGameObjectPoolEffects();
        setupHeroTeam();
        playLevelStartAnim();
        StartCoroutine(IEStartGame());
        SDGameManager.Instance.isGameFinished = false;
    }
    public IEnumerator IEStartGame()
    {
        yield return new WaitForSeconds(1.5f);
        setCurrentLevel();
    }
    public void setupHeroTeam()
    {
        GDEunitTeamData team 
            = SDDataManager.Instance.getHeroTeamByTeamId
            (SDGameManager.Instance.currentHeroTeamId);
        GM.initCurrentGoddess(team.goddess);
        List<GDEHeroData> all = SDDataManager.Instance.getHerosFromTeam
            (SDGameManager.Instance.currentHeroTeamId);
        if (all.Count <= 0)
        {
            Transform s = Instantiate(heroPrefab) as Transform;
            s.SetParent(heroParent);
            s.localScale = Vector3.one;
            s.name = SDConstants.HERO_TAG + 0;
            //s.position = HeroPos[index++].position;
            int index = 0;
            s.position = getPosByIndex(index,false);

            BattleRoleData heroUnit = s.GetComponent<BattleRoleData>();
            heroUnit.posIndex = 1;
            //heroUnit.sidePosId = index;
            heroUnit.initHeroController(1);
            return;
        }
        for(int i = 0; i < all.Count; i++)
        {
            if (all[i] != null)
            {
                Transform s = Instantiate(heroPrefab) as Transform;
                s.SetParent(heroParent);
                s.localScale = Vector3.one;
                s.name = SDConstants.HERO_TAG + all[i].hashCode;
                //s.position = HeroPos[index++].position;
                int index = all[i].teamPos;
                s.position = getPosByIndex(index,false);

                BattleRoleData heroUnit = s.GetComponent<BattleRoleData>();
                heroUnit.posIndex = i;
                //heroUnit.sidePosId = index;
                heroUnit.initHeroController(all[i].hashCode);
            }
        }
    }
    public void playLevelStartAnim()
    {
        int LV = SDGameManager.Instance.currentLevel;
        SDGameManager.Instance.isGamePaused = false;
        SDGameManager.Instance.canRevive = true;
        levelStartText.text = (LV < 10 ? "0" : "") + (LV + 1) + " F";
        //不同类型关卡拥有不同名称

        //
        ShowStartAnimation();
        setupBackgroundImg();
    }
    public void ShowStartAnimation()
    {
        levelStartAnim.gameObject.SetActive(true);
    }
    public void playNextLevelAnim()
    {
        setupBackgroundImg();
        nextLevelLayer.gameObject.SetActive(true);
        imgBlack.gameObject.SetActive(true);
        imgBlack.color = Color.black;
        StartCoroutine(IEPlayNextLevelAnim());
    }
    public IEnumerator IEPlayNextLevelAnim()
    {
        float animTime = 0.3f;
        imgBlack.DOFade(0.7f, animTime);
        yield return new WaitForSeconds(animTime);
        imgBlack.DOFade(0.5f, animTime);
        yield return new WaitForSeconds(animTime);
        imgBlack.DOFade(0f, animTime);
        nextLevelLayer.gameObject.SetActive(false);
    }
    public void setupBackgroundImg()
    {
        int level = SDGameManager.Instance.currentLevel;
        //
        int s = level / SDConstants.LevelNumPerSection;
        string nb;
        if (s == 0) nb = "land";
        else if (s == 1) nb = "sea";
        else nb = "land";
        if (currentLevelTheme != nb)
        {
            currentLevelTheme = nb;
            //
            SpriteAtlas atlas = SDDataManager.Instance.atlas_battleBg;
            Sprite[] all = new Sprite[0];
            atlas.GetSprites(all);
            List<Sprite> matchs = all.ToList().FindAll(x => x.name.Contains(currentLevelTheme));
            backgroundSps = matchs;
        }
        if (backgroundSps.Count > 0)
        {
            backgroundImg.sprite = backgroundSps[UnityEngine.Random
                .Range(0, backgroundSps.Count)];
        }
    }
    public void checkHeroesStatus()
    {
        SDGameManager.Instance.showEnemyState = false;
        foreach(BattleRoleData unit in BM.Remaining_SRL)
        {
            if (unit.showEnemyBlood)
            {
                SDGameManager.Instance.showEnemyState = true;
                break;
            }
        }
        foreach(BattleRoleData unit in BM.Remaining_ORL)
        {
            if (unit.HpController != null)
            {
                unit.HpController.checkIfShowHpStatus();
            }
        }

        SDGameManager.Instance.openAllBonus = false;
        foreach(BattleRoleData unit in BM.Remaining_SRL)
        {
            if (unit.openAllBonus)
            {
                SDGameManager.Instance.openAllBonus = true;
                break;
            }
        }

        SDGameManager.Instance.alwaysGoodEvents = false;
        foreach(BattleRoleData unit in BM.Remaining_SRL)
        {
            if (unit.alwaysGoodEvents)
            {
                SDGameManager.Instance.alwaysGoodEvents = true;
                break;
            }
        }
    }
    public void setCurrentLevel()
    {
        //关卡视觉显示详细
        levelText.text = "";
        int FAddNum = FatigueAddNum;
        if(SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            levelText.text = "主线";
        }
        else if(SDGameManager.Instance.gameType == SDConstants.GameType.Dungeon)
        {
            levelText.text = "地牢";
            FAddNum = (int)(FAddNum * 1.5f);
        }
        else if(SDGameManager.Instance.gameType == SDConstants.GameType.DimensionBoss)
        {
            levelText.text = "BOSS";
            FAddNum = (int)(FAddNum * 2.25f);
        }
        addAllHeroesFatigue(FAddNum);
        //
        LBM.setupLevelAnim();
        foreach (Transform t in enemyParent) Destroy(t.gameObject);
        setupEnemies(SDGameManager.Instance.currentLevel);
        StartCoroutine(IEBattleStart());
    }
    public IEnumerator IEBattleStart()
    {
        yield return new WaitForSeconds(1.0f);
        BM.BattleInit();
    }
    public void addAllHeroesFatigue(int data)
    {
        GDEunitTeamData team
            = SDDataManager.Instance.getHeroTeamByTeamId
            (SDGameManager.Instance.currentHeroTeamId);
        //
        List<GDEHeroData> all = SDDataManager.Instance
            .getHerosFromTeam(SDGameManager.Instance.currentHeroTeamId);
        //
        for(int i = 0; i < all.Count; i++)
        {
            SDDataManager.Instance.addHeroFatigue(data, all[i].hashCode);
        }
    }
    public void checkAutoBattleHint()
    {
        /*
        if(SDDataManager.Instance.SettingData.autoBattleHint==0
            && SDDataManager.Instance.GetMaxPassLevel() >=  )
        {

        }
        */
    }

    public void setupEnemies(int currentLevel)
    {
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Normal
            && currentLevel % SDConstants.LevelNumPerSerial
            == SDConstants.LevelNumPerSerial - 1)
        {
            setupBoss(currentLevel);
            return;//生成boss
        }

        //控制敌人种类
        List<string> enemies = new List<string>();

        List<EnemyInfo> enemydatas = SDDataManager.Instance.AllEnemyList;
        for(int i = 0; i < enemydatas.Count; i++)
        {
            EnemyInfo s = enemydatas[i];
            int weight = s.weight;
            if (enemyBuild.CanBeUsedInThisLevel(weight) && s.EnemyRank.Index < 3)
            {
                enemies.Add(enemydatas[i].ID);
            }
        }
        startEnemyGroup.Clear();
        int groupNum = Mathf.Min(SDGameManager.Instance.currentLevel + 1, SDConstants.MaxOtherNum);
        for(int i = 0; i < groupNum; i++)
        {
            string s = enemies[Random.Range(0, enemies.Count)];
            startEnemyGroup.Add(s);
            Debug.Log("选中的Enemy id:" + s);
        }


        int num = enemyBuild.createCareerByLevel();
        List<int> rareList = RandomIntger.NumListReturn(num, startEnemyGroup.Count);
        if(SDGameManager.Instance.gameType == SDConstants.GameType.Normal
            && currentLevel % SDConstants.LevelNumPerSection
            == SDConstants.LevelNumPerSection - 1)
        {
            addLittleBoss(startEnemyGroup[0] , 4 , 1);
            for(int i = 2; i < SDConstants.MaxOtherNum; i++)
            {
                if (rareList.Contains(i))
                {
                    addEnemy(startEnemyGroup[i], i, 1);
                }
                else { addEnemy(startEnemyGroup[i], i, 0); }
            }
        }
        else
        {
            for (int i = 0; i < startEnemyGroup.Count; i++)
            {
                if (rareList.Contains(i))
                {
                    addEnemy(startEnemyGroup[i], i, 1);
                }
                else { addEnemy(startEnemyGroup[i], i, 0); }
            }
        }
    }
    public void setupBoss(int currentlevel)
    {
        
    }
    public void addEnemy(string enemyId,int index,int rareWeight,bool isBoss = false)
    {
        Transform s;
        if (isBoss) s = Instantiate(bossPrefab) as Transform;
        else s = Instantiate(enemyPrefab) as Transform;
        s.localScale = Vector3.zero;
        s.SetParent(enemyParent);
        s.position = getPosByIndex(index,true);
        s.name = SDConstants.ENEMY_TAG + enemyId + "POS" + index;
        BattleRoleData unit = s.GetComponent<BattleRoleData>();
        unit.posIndex = index + SDConstants.MaxSelfNum;
        //unit.sidePosId = index;
        unit.rareWeight = rareWeight;
        unit.initEnemyController(enemyId);
        StartCoroutine(IEShowEnemyBornAnim(s));
    }
    public IEnumerator IEShowEnemyBornAnim(Transform s)
    {
        yield return new WaitForSeconds(0.2f);
        s.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void addLittleBoss(string enemyId,int index, int rareWeight)
    {
        Transform s;
        s = Instantiate(enemyPrefab) as Transform;
        s.localScale = Vector3.zero;
        s.gameObject.SetActive(false);
        s.SetParent(enemyParent);
        s.position = getPosByIndex(index, true);
        s.name = SDConstants.ENEMY_TAG + enemyId + "POS" + index;
        BattleRoleData unit = s.GetComponent<BattleRoleData>();
        unit.posIndex = SDConstants.MaxSelfNum;
        //
        unit.IsBoss = true;
        unit.isLittleBoss = true;
        //
        unit.rareWeight = rareWeight;
        unit.initEnemyController(enemyId);
        StartCoroutine(IEShowLittleBossBornAnim(s));
    }
    public IEnumerator IEShowLittleBossBornAnim(Transform s)
    {
        yield return new WaitForSeconds(0.2f);
        s.localScale = Vector3.one * 1.5f;
        s.gameObject.SetActive(true);
    }

    public Vector2 getPosByIndex(int index,bool isEnemy)
    {
        Transform[] AllPos;
        if (isEnemy)
        {
            AllPos = EnemyPos;
        }
        else
        {
            AllPos = HeroPos;
        }
        if (index <= 3) return AllPos[index].position;
        else if (index == 4)
        {
            return (AllPos[0].position + AllPos[1].position) / 2;
        }
        else if (index == 5)
        {
            return (AllPos[2].position + AllPos[3].position) / 2;
        }
        else if (index == 6)
        {
            return (AllPos[0].position + AllPos[2].position) / 2;
        }
        else if (index == 7)
        {
            return (AllPos[1].position + AllPos[3].position) / 2;
        }
        else
        {
            Vector3 V = Vector2.zero;
            for (int i = 0; i < AllPos.Length; i++)
            {
                V += AllPos[i].position;
            }
            Vector2 _V = V / AllPos.Length;
            return _V;
        }
    }



    public void AddDrop(GDEItemData drop)
    {
        string id = drop.id;
        if(SDDataManager.Instance.getItemTypeById(id) == SDConstants.ItemType.Consumable)
        {
            bool flag = false;
            for (int i = 0; i < allDropsGet.Count; i++)
            {
                if(allDropsGet[i].id == id)
                {
                    allDropsGet[i].num += drop.num;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                allDropsGet.Add(drop);
            }
        }
        else
        {
            allDropsGet.Add(drop);
        }
    }
    public void storeFinalDrops()
    {
        for(int i = 0; i < allDropsGet.Count; i++)
        {
            string id = allDropsGet[i].id;
            int num = allDropsGet[i].num;
            SDConstants.ItemType it = SDDataManager.Instance.getItemTypeById(id);
            if(it == SDConstants.ItemType.Consumable)
            {
                SDDataManager.Instance.addConsumable(id, num);
            }
            else if(it == SDConstants.ItemType.Equip)
            {
                for (int e = 0; e < num; e++)
                {
                    SDDataManager.Instance.addEquip(id);
                }
            }
            else if(it == SDConstants.ItemType.Hero)
            {
                for (int e = 0; e < num; e++)
                {
                    SDDataManager.Instance.addHero(id);
                }
            }
            else if(it == SDConstants.ItemType.Badge)
            {

            }
            else if(it == SDConstants.ItemType.Rune)
            {
                for(int e = 0; e < num; e++)
                {
                    SDDataManager.Instance.AddRune(id);
                }
            }
        }
    }


    #region 结算设置
    public void showRemarkLayer()
    {
        StartCoroutine(IEShowRemarkLayer());
    }
    public IEnumerator IEShowRemarkLayer()
    {
        yield return new WaitForSeconds(1f);//Bosss死亡动画时间
        //通关结算等显示
        gameSuccess();
    }

    public void gameSuccess()
    {
        SDGameManager.Instance.isGameFinished = true;
        //section评价计算

        //
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            UIEffectManager.Instance.showAnimFadeIn(gameEndLayer);
            gameEndLayer.GetComponent<SDGameSuccess>().initGameFinishLayer(true);
        }
    }
    public void gameFail()
    {
        SDGameManager.Instance.isGameFinished = true;
        bool checkAdsLoad = true;
        //是否进入复活页面
        if(SDGameManager.Instance.canRevive 
            && checkAdsLoad 
            && SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            UIEffectManager.Instance.showAnimFadeIn(gameEndLayer);
            gameEndLayer.GetComponent<GameRevive>().initGameRevive();
        }
        else
        {
            if (SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
            {
                UIEffectManager.Instance.showAnimFadeIn(gameEndLayer);
                gameEndLayer.GetComponent<SDGameSuccess>().initGameFinishLayer(false);
            }
        }

    }
    #endregion
}
