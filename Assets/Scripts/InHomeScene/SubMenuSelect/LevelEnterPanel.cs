using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using DG.Tweening;

public class LevelEnterPanel : BasicSubMenuPanel
{
    [Space(25)]
    public Transform chaptersPanel;
    public Transform serialsPanel;
    public Transform sectionsPanel { get { return CLC.transform; } }
    public ChapterLevelController CLC;
    public enum panelContent
    {
        chapters,serials,sections,
    }
    public panelContent currentPanelContent = panelContent.chapters;
    public int currentLocalChapter;
    public int currentLocalSerial;
    public int currentLocalSection;
    [Header("ChapterPanelSet")]
    public List<Button> ChapterBtnList;
    public ScrollRect chapterBtnSC;
    [Header("SectionsPanelSet")]
    public Transform[] AllChapterThemeArray;


    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        CLC.transform.localScale = Vector3.zero;
        currentPanelContent = panelContent.chapters;
        initChapterPanel();
    }
    public void initChapterPanel()
    {
        int maxSectionIndex = SDDataManager.Instance.GetMaxPassSection();
        for(int i = 0; i < ChapterBtnList.Count; i++)
        {
            if (maxSectionIndex >= 1 * SDConstants.SectionNumPerChapter)
            {
                ChapterBtnList[i].interactable = true;
            }
            else ChapterBtnList[i].interactable = false;
        }
        if (maxSectionIndex < 0) ChapterBtnList[0].interactable = true;
        StartCoroutine(resetCPSC());
    }
    IEnumerator resetCPSC()
    {
        yield return new WaitForSeconds(0.1f);
        chapterBtnSC.horizontalNormalizedPosition = 0;
    }
    public void initSerialPanel(int chapter)
    {
        currentLocalChapter = chapter;
        EnterSerialBtn[] allBs = AllChapterThemeArray[currentLocalChapter]
            .GetComponentsInChildren<EnterSerialBtn>();
        foreach(EnterSerialBtn B in allBs)
        {
            B.RefreshBtnStatus(currentLocalChapter);
        }
    }
    public void enterSelectedChapter(int index)
    {
        UIEffectManager.Instance.showAnimFadeIn(serialsPanel);
        //UIEffectManager.Instance.showAnimFadeIn(CLC.transform);
        currentPanelContent = panelContent.serials;
        //CLC.FirstlyShowLevelList();
        for (int i = 0; i < AllChapterThemeArray.Length; i++)
        {
            if (i == index)
            {
                AllChapterThemeArray[i].gameObject.SetActive(true);
            }
            else AllChapterThemeArray[i].gameObject.SetActive(false);
        }
        currentLocalChapter = index;
        initSerialPanel(currentLocalChapter);
    }
    public void enterSelectedSerial(int index)
    {
        UIEffectManager.Instance.showAnimFadeIn(sectionsPanel);
        currentPanelContent = panelContent.sections;
        currentLocalSerial = index;

        CLC.FirstlyShowLevelList();
    }

    public override void commonBackAction()
    {      
        if(currentPanelContent == panelContent.chapters)
        {
            base.commonBackAction();
            homeScene.SubMenuClose();
        }
        else if(currentPanelContent == panelContent.serials)
        {
            UIEffectManager.Instance.hideAnimFadeOut(serialsPanel);
            currentPanelContent = panelContent.chapters;
            initChapterPanel();
        }
        else if(currentPanelContent == panelContent.sections)
        {
            UIEffectManager.Instance.hideAnimFadeOut(sectionsPanel);
            currentPanelContent = panelContent.serials;
            initSerialPanel(currentLocalChapter);
        }
    }
}
