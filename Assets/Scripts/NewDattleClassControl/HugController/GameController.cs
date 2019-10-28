using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameDataEditor;
//using Colorful;

    /// <summary>
    /// 游戏控制类
    /// </summary>
public class GameController : MonoBehaviour
{
    public static GameController Instantce;
    public BattleManager BM;
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
    public Sprite[] backgroundSps;
    public Transform nextLevelLayer;
    public Image imgBlack;
    [HideInInspector]
    public List<int> startEnemyGroup;
    [Space(25)]
    public LevelbarManager LBM;
    #region 结算页面
    [Header("结算UI")]
    public Transform introPanel;
    public Transform gameSuccessLayer;
    public Transform gameFailLayer;
    public Transform gameReviveLayer;
    public Transform gameBonusLayer;
    public Transform gameEventLayer;
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
        setupHeroes();
        playLevelStartAnim();
        StartCoroutine(IEStartGame());
    }
    public IEnumerator IEStartGame()
    {
        yield return new WaitForSeconds(1.5f);
        setCurrentLevel();
    }
    public void setupHeroes()
    {
        GDEunitTeamData team 
            = SDDataManager.Instance.getHeroTeamByTeamId
            (SDGameManager.Instance.currentHeroTeamIndex);
        //int index = 0;
        for(int i = 0; i < team.heroes.Count; i++)
        {
            if (team.heroes[i] != 0)
            {
                Transform s = Instantiate(heroPrefab) as Transform;
                s.SetParent(heroParent);
                s.localScale = Vector3.one;
                s.name = SDConstants.HERO_TAG + team.heroes[i];
                //s.position = HeroPos[index++].position;
                int pos = SDDataManager.Instance.getHeroPosInTeamByHashcode(team.heroes[i]);
                s.position = HeroPos[pos].position;

                BattleRoleData heroUnit = s.GetComponent<BattleRoleData>();
                heroUnit.posIndex = i;
                //heroUnit.sidePosId = index;
                heroUnit.initHeroController(team.heroes[i]);
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
        int index = level% SDConstants.LevelNumPerSection;
        backgroundImg.sprite = backgroundSps[index];
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
        if(SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            levelText.text = "主线";
        }
        else if(SDGameManager.Instance.gameType == SDConstants.GameType.Dungeon)
        {
            levelText.text = "地牢";
        }
        else if(SDGameManager.Instance.gameType == SDConstants.GameType.DimensionBoss)
        {
            levelText.text = "BOSS";
        }
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
    public void checkAutoBattleHint()
    {
        /*
        if(SDDataManager.Instance.SettingData.autoBattleHint==0
            && SDDataManager.Instance.GetMaxPassLevel() >=  )
        {

        }
        */
    }
    public void showBounsLayer()
    {
        StartCoroutine(IEShowBonusLayer());
    }
    public IEnumerator IEShowBonusLayer()
    {
        yield return new WaitForSeconds(1f);//Bosss死亡动画时间
        //通关结算等显示
    }
    public void setupEnemies(int currentLevel)
    {
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Normal
            && currentLevel % SDConstants.PerBossAppearLevel 
            == SDConstants.PerBossAppearLevel - 1)
        {
            setupBoss(currentLevel);
            return;//生成boss
        }

        List<int> enemiesId = new List<int>();

        List<Dictionary<string, string>> enemydatas = SDDataManager.Instance.ReadFromCSV("enemy");
        for(int i = 0; i < enemydatas.Count; i++)
        {
            Dictionary<string, string> s = enemydatas[i];
            int weight = SDDataManager.Instance.getInteger(s["weight"]);
            if (enemyBuild.CanBeUsedInThisLevel(weight) && s["class"] != "boss")
            {
                int enemyId = SDDataManager.Instance.getInteger(s["id"]);
                int appearWeight = SDDataManager.Instance.getInteger(s["appearWeight"]);
                for(int j = 0; j < appearWeight; j++)
                {
                    enemiesId.Add(enemyId);
                    Debug.Log("关卡可用enemy id:" + enemiesId[j]);
                }
            }
        }
        startEnemyGroup.Clear();
        List<int> selectList = RandomIntger.NumListReturn
            ( SDConstants.MaxOtherNum, enemiesId.Count );
        for(int i = 0; i < enemiesId.Count; i++)
        {
            if (selectList.Contains(i))
            {
                startEnemyGroup.Add(enemiesId[i]);
                Debug.Log("选中的Enemy id:" +  enemiesId[i]);
            }
        }

        int num = enemyBuild.createCareerByLevel();
        List<int> rareList = RandomIntger.NumListReturn(num, startEnemyGroup.Count);
        if(SDGameManager.Instance.gameType == SDConstants.GameType.Normal
            && currentLevel % SDConstants.LevelNumPerSection
            == SDConstants.LevelNumPerSection - 1)
        {
            addLittleBoss(startEnemyGroup[0], 1);
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
    public void addEnemy(int enemyId,int index,int rareWeight,bool isBoss = false)
    {
        Transform s;
        if (isBoss) s = Instantiate(bossPrefab) as Transform;
        else s = Instantiate(enemyPrefab) as Transform;
        s.localScale = Vector3.zero;
        s.gameObject.SetActive(false);
        s.SetParent(enemyParent);
        s.position = EnemyPos[index].position;
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
        s.localScale = Vector3.one;
        s.gameObject.SetActive(true);
    }

    public void addLittleBoss(int enemyId, int rareWeight)
    {
        Transform s;
        s = Instantiate(enemyPrefab) as Transform;
        s.localScale = Vector3.zero;
        s.gameObject.SetActive(false);
        s.SetParent(enemyParent);
        s.position = (EnemyPos[0].position + EnemyPos[1].position) / 2;
        s.name = SDConstants.ENEMY_TAG + enemyId + "POS 0.5f";
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

    #region 结算设置
    public void gameSuccess()
    {
        SDGameManager.Instance.isGameFinished = true;
        if(SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            
        }
    }
    public void gameFail()
    {
        SDGameManager.Instance.isGameFinished = true;
        bool checkAdsLoad = true;
        //是否进入复活页面
        //if()
        if(SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {

        }
    }
    #endregion
}
