using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class MissionPanel : BasicSubMenuPanel
{
    [Space(25)]
    public Image currentMissionImg;
    public Image missionKindIcon;
    public Text missionRequirement;
    public enum MissionType
    {
        type0,type1,type2
    }

    [SerializeField]
    [EnumMemberNames("剧情","日常","周常")]
    private MissionType _currentKind = MissionType.type0;
    public MissionType CurrentKind
    {
        get { return _currentKind; }
        set 
        {
            if(_currentKind != value)
            {
                MissionType _MK = _currentKind;
                _currentKind = value;
                whenChangingKind(_MK, _currentKind);
            }
        }
    }


    public Button[] AllKindBtns;
    public Transform _Card;
    public ScrollRect CardSR;
    public List<MissionCard> CardList;
    //[Space, Header("QuestMission")]
    //public List<Quest> PlotQuestList = new List<Quest>();
    //public List<Quest> DailyQuestList = new List<Quest>();
    //public List<Quest> WeeklyQuestList = new List<Quest>();
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        AllKindBtns[0].interactable = false;
        type0_initPanel();
    }


    public override void commonBackAction()
    {
        base.commonBackAction();
        homeScene.SubMenuClose();
    }
    public void whenChangingKind(MissionType _old, MissionType _new)
    {
        if (AllKindBtns[(int)_old] != null) AllKindBtns[(int)_old].interactable = true;
        if (AllKindBtns[(int)_new] != null) AllKindBtns[(int)_new].interactable = false;
    }

    #region 切换任务类型

    public void ClearCards()
    {
        for(int i = 0; i < CardList.Count; i++)
        {
            Destroy(CardList[i].gameObject);
        }
        CardList.Clear();
    }

    public void type0_initPanel()
    {
        ClearCards();
        CurrentKind = MissionType.type0;
        //
        int a = Mathf.Min((int)CurrentKind, NPC_s.Count - 1);
        if(NPC_s[a]&&NPC_s[a].Data)
            initMissionPanel(NPC_s[a].Data.questInstances);
    }
    public void type1_initPanel()
    {
        ClearCards();
        CurrentKind = MissionType.type1;
        //
        int a = Mathf.Min((int)CurrentKind, NPC_s.Count - 1);
        if (NPC_s[a] && NPC_s[a].Data)
            initMissionPanel(NPC_s[a].Data.questInstances);
    }
    public void type2_initPanel()
    {
        ClearCards();
        CurrentKind = MissionType.type2;
        //
        int a = Mathf.Min((int)CurrentKind, NPC_s.Count - 1);
        if (NPC_s[a] && NPC_s[a].Data)
            initMissionPanel(NPC_s[a].Data.questInstances);
    }

    public void type3_initPanel()
    {
        ClearCards();

    }
    public void initMissionPanel(List<Quest> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            Quest quest = list[i];
            Transform C = Instantiate(_Card) as Transform;
            C.SetParent(CardSR.content);
            C.localScale = Vector3.one;
            C.gameObject.SetActive(true);
            MissionCard _c = C.GetComponent<MissionCard>();
            _c.MP = this;
            _c.initMissionCard(quest);
            CardList.Add(_c);
        }
    }
    #endregion

}
