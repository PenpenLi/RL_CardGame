using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BattlePerRoleCard : MonoBehaviour
{
    public SimpleBattleScript SBS;
    public Slider ThisCardHp;
    public Slider ThisCardSp;
    public Slider ThisCardTp;
    public RectTransform ThisCardAllStates;
    [HideInInspector]
    public int TargetNum;
    [HideInInspector]
    public bool IsLeft;
    void Start()
    {
        
    }
    public void SelectThisRole()
    {
        SBS.PlayerSelectSkillTarget(TargetNum, IsLeft);
    }
}
