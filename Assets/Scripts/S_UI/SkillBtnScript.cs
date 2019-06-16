using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBtnScript : MonoBehaviour
{
    public int OpSkillNum;
    //public bool ClickEnable;
    public SimpleBattleScript SBS;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void OnClick()
    {
        SBS.PlayerAct(OpSkillNum);
    }
}

