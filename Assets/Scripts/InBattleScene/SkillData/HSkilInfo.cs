using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSkilInfo : MonoBehaviour
{
    public OneSkill HSDetail;
    public SkillBreed breed;
    public SkillKind kind;
    public bool UseAppointedAtb = false;
    //[ConditionalHide("UseAppointedAtb", true)]
    public AttributeData Atb;
    //[EnumMemberNames("无AOE","选择一条横向","横向1","横向2","选择一条纵向","纵向1","纵向2"
       // ,"随机1个","随机2个","随机3个","连续2次","连续3次","所有","End")]
    public SDConstants.AOEType AOEType = SDConstants.AOEType.None;
    public SDConstants.AddMpTpType AfterwardsAddType = SDConstants.AddMpTpType.Normal;
    [HideInInspector]
    public string ID;
}
