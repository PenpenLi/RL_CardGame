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
    public string TeamId;

    public BattleTeamPanel BTP;
    public void initThisUnitTeam(GDEunitTeamData Team)
    {
        if (BTP == null) BTP = GetComponentInParent<BattleTeamPanel>();
        TeamEmpty.gameObject.SetActive(false);
        if (Team != null)
        {
            this.TeamId = Team.id;
            List<GDEHeroData> all = SDDataManager.Instance.getHerosFromTeam(TeamId);
            if (all.Count > 0)
            {
                //
                if (!string.IsNullOrEmpty(Team.goddess))
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
                for (int i = 0; i < all.Count; i++)
                {
                    TeamHeroes[i] = all[i].hashCode;
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
