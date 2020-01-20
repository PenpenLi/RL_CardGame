using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName ="goddess_info",menuName ="Wun/角色/女神信息")]
public class GoddessInfo : CharacterInfo
{
    [SerializeField]
    private GoddessRace goddessRace;
    public GoddessRace GoddessRace
    {
        get { return goddessRace; }
    }

    [SerializeField,Range(0,3)]
    private int quality;
    public int Quality
    {
        get { return quality; }
    }

    [SerializeField]
    private int index;
    public int Index { get { return index; } }

    [SerializeField]
    private GoddessAttritube goddessAtti;
    public GoddessAttritube GoddessAtti { get { return goddessAtti; } }

    public GoddessInfo()
    {
        CharacterType = SDConstants.CharacterType.Goddess;
    }

    [Header("女神配属技能"), Space]
    public bool HaveSkill;
#if UNITY_EDITOR
    [ConditionalHide("HaveSkill",true,false)]
#endif
    public GoddessSkillInfo SkillInfo;
}

[System.Serializable]
public class GoddessSkillInfo
{
    public enum GSTag
    {
        damage,heal,addState
    }
    public GSTag SkillTag;
    public Transform Prefab
    {
        get
        {
            string address = "ScriptableObjects / goddess / GoddessSkills/";
            Transform[] all = Resources.LoadAll<Transform>(address);
            GoddessSkill[] _all = all.Select(x => x.GetComponent<GoddessSkill>()).ToArray();
            foreach(GoddessSkill s in _all)
            {
                if(SkillTag == GSTag.damage && s is GSDamage)
                {
                    return s.transform;
                }
                else if(SkillTag == GSTag.heal && s is GSHeal)
                {
                    return s.transform;
                }
                else if(SkillTag == GSTag.addState && s is GSAddState)
                {
                    return s.transform;
                }
            }
            return null;
        }
    }
    [Space]
#if UNITY_EDITOR
    [ConditionalHide("SkillTag",(int)GSTag.damage,true,false)]
#endif
    public NumberData _DamageData;
#if UNITY_EDITOR
    [ConditionalHide("SkillTag", (int)GSTag.heal, true, false)]
#endif
    public NumberData _HealData;
#if UNITY_EDITOR
    [ConditionalHide("SkillTag", (int)GSTag.addState, true, true)]
#endif
    public int UpBySkillGrade;
    [Space]
    public bool TargetIsHero;
    public SDConstants.AOEType AOE;
    //int skillgrade
    public SkillBreed BREED;
    public SkillKind KIND;
    public bool UseState;
#if UNITY_EDITOR
    [ConditionalHide("UseState", true, false)]
#endif
    public GSState State;
}
