using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class OneUnitTeam : MonoBehaviour
{
    public Transform TeamEmpty;
    public Transform[] TeamHeroVisions;
    [HideInInspector]
    public int[] TeamHeroes = new int[4];
    public Transform TeamGoddess;
    public Transform TeamBadgePlace;
    public Text TeamNameText;
    public Button BtnToDelete;
    public Button BtnToEdit;
    public Button BtnToConfirm;
    public int TeamId;

    public BattleTeamPanel BTP;
    public void initThisUnitTeam(GDEunitTeamData Team)
    {
        if (BTP == null) BTP = GetComponentInParent<BattleTeamPanel>();
        TeamEmpty.gameObject.SetActive(false);
        if (Team != null)
        {
            this.TeamId = Team.id;
            if (Team.heroes.Count > 0 && haveRolesInTeam(Team))
            {
                //
                if (Team.goddess > 0)
                {
                    TeamGoddess.GetComponentInChildren<Image>().sprite
                        = Resources.Load<Sprite>("Sprites/"
                        + SDDataManager.Instance.getGoddessSpriteById(Team.goddess));
                }
                else { }
                //
                if (Team.teamName != "" && Team.teamName != null)
                {
                    TeamNameText.text = Team.teamName;
                }
                else { TeamNameText.text = randomTeamNameBefore() + SDGameManager.T("小队") + Team.id; }
                //
                for (int i = 0; i < Team.heroes.Count; i++)
                {
                    TeamHeroes[i] = Team.heroes[i];
                }
                //
                if (Team.badge > 0)
                {

                }
                else { }
            }
            else
            {
                ShowEmpty();
            }
        }
        else
        {
            ShowEmpty();
        }

    }
    public void ShowEmpty()
    {
        TeamEmpty.gameObject.SetActive(true);
    }
    public string randomTeamNameBefore()
    {
        string[] examples = new string[]
        {
            "勇者","冒险","讨伐","战斗","前进"
        };
        return SDGameManager.T(examples[UnityEngine.Random.Range(0, examples.Length)]);
    }
    public bool haveRolesInTeam(GDEunitTeamData team)
    {
        for (int i = 0; i < team.heroes.Count; i++)
        {
            if (team.heroes[i] > 0) return true;
        }
        return false;
    }

    public void Btn_Delete()
    {

    }
    public void Btn_Edit()
    {
        //打开队伍编辑页面
        //SDGameManager.Instance.currentHeroTeamIndex = TeamId;
        if(BTP==null) BTP = GetComponentInParent<BattleTeamPanel>();
        BTP.openEditUnitTeamPanel(TeamId);
    }
    public void Btn_Confirm()
    {
        if (BTP == null) BTP = GetComponentInParent<BattleTeamPanel>();
        BTP.ConfirmBattleTeam(TeamId);
    }


}
