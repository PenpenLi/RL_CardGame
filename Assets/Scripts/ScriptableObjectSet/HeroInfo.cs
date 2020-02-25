using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

[CreateAssetMenu(fileName ="hero_info",menuName ="Wun/角色/英雄信息")]
public class HeroInfo : CharacterInfo
{
    [SerializeField]
    private int _LEVEL;
    public int LEVEL
    {
        get { return _LEVEL; }
        set { _LEVEL = value; }
    }
    
    [SerializeField,Range(0,3)]
    private int _rarity;
    public int Rarity
    {
        get { return _rarity; }
        set { _rarity = value; }
    }

    [SerializeField]
    private int skeleton;
    public int Skeleton
    {
        get { return skeleton; }
        set { skeleton = value; }
    }

    [SerializeField]
    private List<WeaponRace> weaponRaceList;
    public List<WeaponRace> WeaponRaceList
    {
        get { return weaponRaceList; }
        set { weaponRaceList = value; }
    }

    [SerializeField]
    private RoleAttributeList _RAL;
    public RoleAttributeList RAL
    {
        get { return _RAL; }
    }

    [SerializeField]
    private RoleCareer _career;
    public RoleCareer Career
    {
        get { return _career; }
        set { _career = value; }
    }

    [SerializeField]
    private HeroRace _race;
    public HeroRace Race
    {
        get { return _race; }
        set { _race = value; }
    }

    [SerializeField]
    private string specialStr;
    public string SpecialStr
    {
        get { return specialStr; }
        set { specialStr = value; }
    }

    #region SkillsData
    [SerializeField]
    private bool _HaveExclusiveSkills;
    public bool HaveExclusiveSkills
    {
        get { return _HaveExclusiveSkills; }
        set { _HaveExclusiveSkills = value; }
    }
#if UNITY_EDITOR
    [SerializeField, ConditionalHide("_HaveExclusiveSkills", true, false)]
#endif
    private skillInfo _skill0Info;
    public skillInfo Skill0Info
    {
        get { return _skill0Info; }
        private set { _skill0Info = value; }
    }

#if UNITY_EDITOR
    [SerializeField, ConditionalHide("_HaveExclusiveSkills", true, false)]
#endif
    private skillInfo _skill1Info;
    public skillInfo Skill1Info
    {
        get { return _skill1Info; }
        private set { _skill1Info = value; }
    }

#if UNITY_EDITOR
    [SerializeField, ConditionalHide("_HaveExclusiveSkills", true, false)]
#endif
    private skillInfo _skillOmegaInfo;
    public skillInfo SkillOmegaInfo
    {
        get { return _skillOmegaInfo; }
        private set { _skillOmegaInfo = value; }
    }
    public void AddSkillData(skillInfo s0,skillInfo s1,skillInfo somega)
    {
        Skill0Info = s0;Skill1Info = s1;SkillOmegaInfo = somega;
    }
    #endregion
    
    public HeroInfo()
    {
        CharacterType = SDConstants.CharacterType.Hero;
    }

    public override void initData(string id, string name, string desc, CharacterSex sex, string faceIcon
        , SDConstants.CharacterType ctype = SDConstants.CharacterType.Hero)
    {
        base.initData(id, name, desc, sex, faceIcon, ctype);
    }
    public void InitRAL(RoleAttributeList ral)
    {
        _RAL = ral;
    }

    [Space]
    public Sprite PersonalDrawImg;
    [Space]
    [SerializeField]
    private bool _useSpineData;
    public bool UseSpineData
    {
        get { return _useSpineData; }
        set { _useSpineData = value; }
    }
#if UNITY_EDITOR
    [SerializeField, ConditionalHide("_useSpineData", true, false)]
#endif
    public RoleSkeletonData SpineData;
}
