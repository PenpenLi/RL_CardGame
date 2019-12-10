using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using GameDataEditor;

public class ChapterLevelController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public int CurrentChapterIndex = 0;
    public int CurrentBossLvIndex = 0;
    [HideInInspector]
    public int maxSectionIndex;
    [HideInInspector]
    public int maxBossIndex;
    private float BtnAppearAnimTime = 0.05f;
    public Button BtnToNext;
    public Button BtnToPrevious;




    public void FirstlyShowLevelList()
    {
        maxSectionIndex = SDConstants.PerBossAppearLevel / SDConstants.LevelNumPerSection;
        maxBossIndex = SDConstants.LevelNumPerChapter / SDConstants.PerBossAppearLevel;

        initThisLevelSelectMenu_bossNumSize(0);
        RefreshBtnForNPSituation();
    }
    public void ResetScrollrect()
    {
        scrollRect.content.anchoredPosition = Vector2.zero;
    }
    public void initThisLevelSelectMenu_chapterNumSize()
    {

    }
    public void initThisLevelSelectMenu_bossNumSize(int bossIndex)
    {
        CurrentBossLvIndex = bossIndex;
        for(int i = 0; i < scrollRect.content.childCount; i++)
        {
            if(i < maxSectionIndex)
            {
                scrollRect.content.GetChild(i)
                    .GetComponent<EnterSectionBtn>().initThisBtn_perBoss(i);
                scrollRect.content.GetChild(i).localScale = Vector3.zero;
            }
        }
        ResetScrollrect();
        StartCoroutine(IEInitLevelSelectMenuBtns());
    }
    public IEnumerator IEInitLevelSelectMenuBtns()
    {
        for(int i = 0; i < scrollRect.content.childCount; i++)
        {
            if(i< maxSectionIndex)
            {
                scrollRect.content.GetChild(i).DOScale(Vector3.one, BtnAppearAnimTime);
                yield return new WaitForSeconds(BtnAppearAnimTime);
            }
        }
    }
    public void BtnForNP(bool ForN)
    {
        if (ForN)
        {
            initThisLevelSelectMenu_bossNumSize(CurrentBossLvIndex + 1);
        }
        else
        {
            initThisLevelSelectMenu_bossNumSize(CurrentBossLvIndex - 1);
        }
        RefreshBtnForNPSituation();
    }
    public void RefreshBtnForNPSituation()
    {
        if (CurrentBossLvIndex == 0) { BtnToPrevious.gameObject.SetActive(false); }
        else { BtnToPrevious.gameObject.SetActive(true); }
        if(CurrentBossLvIndex == maxSectionIndex - 1) { BtnToNext.gameObject.SetActive(false); }
        else { BtnToNext.gameObject.SetActive(true); }
    }


    public void GoToTeamSelectPanel()
    {
        GetComponentInParent<LevelEnterPanel>().homeScene.battleTeamBtnTapped(true);
    }
    public void GoToBattleScene()
    {
        string battleTeamId = SDGameManager.Instance.currentHeroTeamId;
        //GDEunitTeamData Team = SDDataManager.Instance.getHeroTeamByTeamId(battleTeamId);
        List<GDEHeroData> all = SDDataManager.Instance.getHerosFromTeam(battleTeamId);
        if (all.Count > 0)
        {
            if (all[0]!=null && all[0].hashCode>0)
            {
                SceneManager.LoadSceneAsync("BattleScene");
            }
            else
            {
                Debug.Log("队首为空");
            }
        }
        else
        {

        }
        
    }
}
