using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSkilInfo : MonoBehaviour
{
    public OneSkill HSDetail;
    public SkillBreed breed;
    public SkillKind kind;
    public bool UseAppointedAtb = false;
    [ConditionalHide("UseAppointedAtb", true)]
    public AttributeData Atb;
    public SDConstants.AOEType AOEType = SDConstants.AOEType.None;
    public SDConstants.AddMpTpType AfterwardsAddType = SDConstants.AddMpTpType.Normal;

}
