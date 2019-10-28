using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SelectTeamUnitPanel : MonoBehaviour
{
    public int CurrentTeamId;
    public int currentHeroIndexInTeam;

    public RolePosControllerInTeam RPC { get { return GetComponentInChildren<RolePosControllerInTeam>(); } }
    public BattleTeamPanel BTP { get { return GetComponentInParent<BattleTeamPanel>(); } }
    public void whenOpenThisPanel()
    {
        currentHeroIndexInTeam = 0;
        RPC.initRoleModelToRolePosPlace();

    }

}
