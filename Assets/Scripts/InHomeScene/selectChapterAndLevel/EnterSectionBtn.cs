using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterSectionBtn : MonoBehaviour
{
    public int level = 0;
    public int sectionIndex = 0;
    [HideInInspector]
    public ChapterLevelController CLC;
    public Text BtnNameText;
    // Start is called before the first frame update
    void Start()
    {
        CLC = GetComponentInParent<ChapterLevelController>();
    }
    public void initThisBtn_perChapter(int index)
    {
        if (CLC == null) CLC = GetComponentInParent<ChapterLevelController>();
        //index表示btnLevel%chapterMaxNum
        level = index*SDConstants.LevelNumPerSection
            + CLC.CurrentChapterIndex * SDConstants.LevelNumPerChapter;
        sectionIndex = index;
        int SectionIndexPerBossLv = (sectionIndex * SDConstants.LevelNumPerSection)
            % SDConstants.PerBossAppearLevel;

        if (SectionIndexPerBossLv == SDConstants.PerBossAppearLevel - 1)
        {
            BtnNameText.text = string.Format("{0:D}=>=BOSS"
                , level
                );
        }
        else
        {
            BtnNameText.text = string.Format("{0:D}=>={1:D}"
                , level
                , level + SDConstants.LevelNumPerSection - 1
                );
        }
    }

    public void initThisBtn_perBoss(int index)
    {
        if (CLC == null) CLC = GetComponentInParent<ChapterLevelController>();

        level = index * SDConstants.LevelNumPerSection
            + CLC.CurrentBossLvIndex * SDConstants.PerBossAppearLevel
            + CLC.CurrentChapterIndex * SDConstants.LevelNumPerChapter;
        sectionIndex = index;
        if(index == SDConstants.PerBossAppearLevel/SDConstants.LevelNumPerSection - 1)
        {
            BtnNameText.text = string.Format("{0:D}=>=BOSS"
                , level
                );
        }
        else
        {
            BtnNameText.text = string.Format("{0:D}=>={1:D}"
                , level
                , level + SDConstants.LevelNumPerSection - 1
                );
        }
    }
    public void ThisSectionBtnTapped()
    {
        if (CLC == null) CLC = GetComponentInParent<ChapterLevelController>();

        SDGameManager.Instance.currentLevel = level;
        //CLC.GoToBattleScene();
        CLC.GoToTeamSelectPanel();
    }
}
