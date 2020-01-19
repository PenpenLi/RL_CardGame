using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SelectTeamUnitPanel : MonoBehaviour
{
    public string CurrentTeamId;
    private int _currentHeroIndexInTeam;
    public int currentHeroIndexInTeam
    {
        get { return _currentHeroIndexInTeam; }
        set
        {
            _currentHeroIndexInTeam = value;
            refreshUI();
        }
    }
    //public OneUnitTeam currentTeamData;

    public Image goddessImg;
    public Text teamNameText;
    public InputField nameInputField;

    public Button goddessChangeBtn;
    public Button teamNameChangeBtn;
    [Header("队伍额外配置选择")]
    public Transform goddessSelectSubPanel;
    public SimpleHeroDetailVision currentGoddessSimpleDetail;
    public HEWPageController goddess_pageController;
    public SDGoddesDetail SDGD;
    public RolePosControllerInTeam RPC { get { return GetComponentInChildren<RolePosControllerInTeam>(); } }
    public SDHeroSelect SDHS { get { return GetComponentInChildren<SDHeroSelect>(); } }
    public BattleTeamPanel BTP { get { return GetComponentInParent<BattleTeamPanel>(); } }
    public enum editType
    {
        hero, goddess, badge,
    }
    editType _currentEditType = editType.hero;
    public editType currentEditType
    {
        get { return _currentEditType; }
        set { if (_currentEditType != value)
            {
                changeSelectSubPanel(_currentEditType, value);
                _currentEditType = value;
            }
        }
    }
    //[HideInInspector]
    //public bool usingIEcannotTap = false;

    public void whenOpenThisPanel()
    {
        currentHeroIndexInTeam = 0;

        currentEditType = editType.hero;
        goddessSelectSubPanel.gameObject.SetActive(false);
        goddess_pageController.ResetPage();

        GDEunitTeamData team = SDDataManager.Instance.getHeroTeamByTeamId(CurrentTeamId);

        //goddessImg.sprite = currentTeamData.TeamGoddess.GetComponent<Image>().sprite;
        teamNameText.text = team.teamName;

    }


    public void Btn_Change_Goddess()
    {
        currentEditType = editType.goddess;
    }
    public void changeTeamName(string newName)
    {
        SDDataManager.Instance.setTeamName(CurrentTeamId, newName);
        //nameInputField.gameObject.SetActive(false);
    }
    public void changeSelectSubPanel(editType oldType, editType newType)
    {
        if(newType == editType.goddess)
        {
            goddessSelectSubPanel.gameObject.SetActive(true);
            goddess_pageController.ItemsInit(SDConstants.ItemType.Goddess);
        }

        if(oldType == editType.goddess)
        {
            goddessSelectSubPanel.gameObject.SetActive(false);
            goddess_pageController.ResetPage();

        }
    }




    public void refreshUI()
    {
        for(int i = 0; i < SDHS.heroItemsInTeam.Length; i++)
        {
            if(i == currentHeroIndexInTeam)
            {
                SDHS.heroItemsInTeam[i].selectedImg.gameObject.SetActive(true);
            }
            else
            {
                SDHS.heroItemsInTeam[i].selectedImg.gameObject.SetActive(false);
            }
        }
    }





    public void button_to_continue()
    {
        BTP.ConfirmBattleTeam(CurrentTeamId);
    }
    public void button_to_clear()
    {
        Debug.Log("清空队伍");
    }
}
