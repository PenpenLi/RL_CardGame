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
    public int LocalChapterIndex
    {
        get { return LEP.currentLocalChapter; }
    }
    public int LocalSerialIndex
    {
        get { return LEP.currentLocalSerial; }
    }
    public int LocalSectionIndex
    {
        get { return LEP.currentLocalSection; }
        set { LEP.currentLocalSection = value; }
    }
    //[HideInInspector]
    public int maxSectionIndex;
    //[HideInInspector]
    public int maxBossIndex;
    private float BtnAppearAnimTime = 0.05f;
    public LevelEnterPanel LEP
    {
        get { return GetComponentInParent<LevelEnterPanel>(); }
    }
    public void FirstlyShowLevelList()
    {
        maxSectionIndex = SDConstants.SectionNumPerSerial;
        maxBossIndex = SDConstants.SerialNumPerChapter;

        initThisLevelSelectMenu_bossNumSize();
    }
    public void initThisLevelSelectMenu_bossNumSize()
    {
        for(int i = 0; i < scrollRect.content.childCount; i++)
        {
            if(i < maxSectionIndex)
            {
                scrollRect.content.GetChild(i)
                    .GetComponent<EnterSectionBtn>().initThisBtn_perSerial(i);
                scrollRect.content.GetChild(i).localScale = Vector3.zero;
            }
        }
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
        scrollRect.DOVerticalNormalizedPos(1, BtnAppearAnimTime * 2);
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
