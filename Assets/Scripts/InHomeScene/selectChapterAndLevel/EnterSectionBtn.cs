using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
public class EnterSectionBtn : MonoBehaviour
{
    public int level = 0;
    public int sectionIndex = 0;
    [Space(10)]
    public int localIndex = 0;
    [HideInInspector]
    public ChapterLevelController CLC;
    public Text BtnNameText;
    [Header("SectionRemark")]
    public Image[] starList;
    [SerializeField]
#if UNITY_EDITOR
    [ReadOnly]
#endif
    private int _Remark;
    public int Remark
    {
        get { return _Remark; }
        set 
        {
            _Remark = value;  
            for(int i = 0; i < starList.Length; i++)
            {
                if (i < _Remark)
                {
                    starList[i].gameObject.SetActive(true);
                }
                else
                {
                    starList[i].gameObject.SetActive(false);
                }
            }
        }
    }
    [Header("SectionReward")]
    public Image ChestIcon;
    public Sprite[] ChestStatusSprites;
    private bool _receiveReward;
    public bool ReceiveReward
    {
        get { return _receiveReward; }
        set 
        { 
            _receiveReward = value;
            ChestIcon.sprite = _receiveReward 
                ? ChestStatusSprites[1] : ChestStatusSprites[0];
        }
    }
    [SerializeField]
    private bool _isLocked;
    public bool IsLocked
    {
        get { return _isLocked; }
        set
        {
            if(_isLocked != value)
            {
                _isLocked = value;
                //LockedPanel.gameObject.SetActive(_isLocked);
                gameObject.SetActive(!_isLocked);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        CLC = GetComponentInParent<ChapterLevelController>();
    }

    public void initThisBtn_perSerial(int index)
    {
        if (CLC == null) CLC = GetComponentInParent<ChapterLevelController>();


        level = index * SDConstants.LevelNumPerSection
            + CLC.LocalSerialIndex * SDConstants.LevelNumPerSerial;
        sectionIndex = index 
            + CLC.LocalSerialIndex * SDConstants.SectionNumPerSerial
            + CLC.LocalChapterIndex * SDConstants.SectionNumPerChapter;
        localIndex = index;
        if(localIndex == SDConstants.SectionNumPerSerial - 1)
        {
            BtnNameText.text = string.Format("{0:D}-BOSS"
                , level
                );
        }
        else
        {
            BtnNameText.text = string.Format("{0:D}-{1:D}"
                , level
                , level + SDConstants.LevelNumPerSection - 1
                );
        }
        if (sectionIndex > SDDataManager.Instance.PlayerData.maxPassSection + 1)
        {
            IsLocked = true; return;
        }
        else IsLocked = false;

        //
        GDESectionData data = SDDataManager.Instance.getSectionHistoryByIndex(sectionIndex);
        if(data == null)
        {
            SDDataManager.Instance.SaveSectionHistory(sectionIndex, 0);
            data = SDDataManager.Instance.getSectionHistoryByIndex(sectionIndex);
        }
        Remark = data.remark;
        ReceiveReward = data.receiveReward;
    }
    public void ThisSectionBtnTapped()
    {
        if (IsLocked) return;
        if (CLC == null) CLC = GetComponentInParent<ChapterLevelController>();

        SDGameManager.Instance.currentLevel = level;
        //CLC.GoToBattleScene();
        CLC.GoToTeamSelectPanel();
    }
}
