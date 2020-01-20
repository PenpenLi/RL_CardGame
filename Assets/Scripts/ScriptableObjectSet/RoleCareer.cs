using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "role_career", menuName = "Wun/角色/职业")]
public class RoleCareer : BaseRank
{
    [SerializeField]
#if UNITY_EDITOR
    [EnumMemberNames("战士","游侠","圣职","法师","end")]
#endif
    private Job career;
    public Job Career
    {
        get { return career; }
        set { career = value; }
    }
}
