using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "Skill", menuName = "Wun/技能/新技能")]
public class skillInfo : ScriptableObject
{
    [SerializeField]
    private string _ID;
    public string ID { get { return _ID; } protected set { _ID = value; } }

    [SerializeField]
    private string _NAME;
    public string NAME { get { return _NAME; } protected set { _NAME = value; } }

    [SerializeField, TextArea]
    private string _DESC;
    public string DESC { get { return _DESC; } protected set { _DESC = value; } }
    [SerializeField]
    private bool isOmegaSkill;
    public bool IsOmegaSkill { get { return isOmegaSkill; } protected set { isOmegaSkill = value; } }

    [SerializeField]
    [EnumMemberNames("自己", "友方", "敌方", "无限制", "End")]
    private SkillAim aim = SkillAim.End;
    public SkillAim Aim { get { return aim; } protected set { aim = value; } }

    [SerializeField]
    [EnumMemberNames("远程","近战","无标签")]
    private SkillBreed breed = SkillBreed.Absence;
    public SkillBreed Breed { get { return breed; }protected set { breed = value; } }

    [SerializeField]
    [EnumMemberNames("元素","物理","奥秘","End")]
    private SkillKind kind = SkillKind.End;
    public SkillKind Kind { get { return kind; }protected set { kind = value; } }

    [SerializeField]
    [EnumMemberNames("无AOE", "选择一条横向", "横向1", "横向2", "选择一条纵向", "纵向1", "纵向2"
        , "随机1个", "随机2个", "随机3个", "连续2次", "连续3次", "所有", "End")]
    private SDConstants.AOEType skillAoe;
    public SDConstants.AOEType SkillAoe { get { return skillAoe; }protected set { skillAoe = value; } }

    [SerializeField]
    [EnumMemberNames("标准获取","获取偏向Mp","获取偏向Tp","获取缺少Mp","获取缺少Tp"
        ,"略加强获取量","降低获取量","获取极度偏向Mp","获取极度偏向Tp","极大加强获取量","End")]
    private SDConstants.AddMpTpType mptpAddType;
    public SDConstants.AddMpTpType MpTpAddType { get { return mptpAddType; }protected set { mptpAddType = value; } }

    [DisplayName("使用专用属性")]
    public bool UseAppointedAtb = false;
    [ConditionalHide("UseAppointedAtb", true)]
    public AttributeData Atb;

    [DisplayName("使用专用SkillPrefab")]
    public bool UseAppointedPrefab = false;
    [SerializeField]
    [ConditionalHide("UseAppointedPrefab",true,true)]
    private int functionId;
    public int FunctionId 
    { get { return functionId; } protected set { functionId = value; } }
    [SerializeField]
    [ConditionalHide("UseAppointedPrefab", true, false)]
    private SkillClass appointedSkillClass;
    public SkillClass AppointedSkillClass
    {
        get { return appointedSkillClass; }
        set { appointedSkillClass = value; }
    }
    
    [SerializeField]
    [ConditionalHide("UseAppointedPrefab", true, false)]
    private ExtraSkillDataSet extraDataSet;
    public ExtraSkillDataSet ExtraDataSet
    {
        get { return extraDataSet; }
        set { extraDataSet = value; }
    }
    public Transform SkillPrefab
    {
        get
        {
            return ChooseSkillPrefab.skillprefab(AppointedSkillClass, Breed);
        }
    }

    [System.Serializable]
    public class ExtraSkillDataSet
    {
        public int BCPerformDataUsingRA;//使用对应属性量()%
        public int PDUsingRA_PerLevel;//%
        public int PDUsingRA_PerSkillGrade;//%
        [Header("ExtraStateAdd"), Space(25)]
        public bool UseState;
        [ConditionalHide("UseState", true)]
        public StandardState _standardState;
    }
    #region event
    [SerializeField]
    private UnityEvent extraSkillWork = new UnityEvent();
    public UnityEvent ExtraSkillWork
    {
        get { return extraSkillWork; }
    }
    #endregion
    [Space]
    public string iconImg;
    public string bulletImg;
    public int skillAnimId;

    [SerializeField,Header("技能消耗")]
    private SkillCost _skillCost;
    public SkillCost skillCost
    {
        get { return _skillCost; }
        set { _skillCost = value; }
    }

    [System.Serializable]
    public class SkillCost
    {
        [SerializeField]
        private RoleBarChart _COST_intger;
        public RoleBarChart COST_intger(int LEVEL,int skillGrade)
        {
            return _COST_intger * (1f + 0.1f * LEVEL - 0.085f * skillGrade);
        }
        [SerializeField]
        private RoleBarChart _COST_pc;
        public RoleBarChart COST_pc(int LEVEL, int skillGrade)
        {
            return _COST_pc * (1f + 0.1f * LEVEL - 0.085f * skillGrade);
        }
    }
    public skillInfo(string id,string name,string desc,int functionId,bool isOmega,SkillAim aim,SkillBreed breed
        ,SkillKind kind,SDConstants.AOEType aoe,SDConstants.AddMpTpType mptpAdd)
    {
        ID = id;NAME = name;DESC = desc;FunctionId = functionId;IsOmegaSkill = isOmega;
        this.Aim = aim;this.Breed = breed;this.Kind = kind;
        SkillAoe = aoe;MpTpAddType = mptpAdd;
    }





}

public enum SkillClass
{
    damage,mixedDamage,addState,heal,addHpMpTp,revive,intervene
        ,
    end
}
public class ChooseSkillPrefab
{
    public static Transform skillprefab(SkillClass skill,SkillBreed breed)
    {
        Transform[] allList = Resources.LoadAll<Transform>("ScriptableObjects/skills/skillPrefabs");
        List<Transform> all = new List<Transform>();
        for(int i = 0; i < allList.Length; i++)
        {
            all.Add(allList[i]);
        }
        if(skill == SkillClass.damage)
        {
            if (breed == SkillBreed.Close)
                return all.Find(x => x.GetComponent<Slash>());
            else return all.Find(x => x.GetComponent<Shoot>());
        }
        else if(skill == SkillClass.mixedDamage)
        {
            if (breed == SkillBreed.Close) return all.Find(x => x.GetComponent<MixedSlash>());
            else return all.Find(x => x.GetComponent<MixedShoot>());
        }
        else if(skill == SkillClass.addState)
        {
            return all.Find(x => x.GetComponent<AttritubeChange>());
        }
        else if(skill == SkillClass.heal)
        {
            return all.Find(x => x.GetComponent<AddHeal>());
        }
        else if(skill == SkillClass.addHpMpTp)
        {
            return all.Find(x => x.GetComponent<AddBarChart>());
        }
        else if(skill == SkillClass.revive)
        {
            return all.Find(x => x.GetComponent<ReviveOne>());
        }
        else if(skill == SkillClass.intervene)
        {
            return all.Find(x => x.GetComponent<Intervene>());
        }

        return null;
    }
}
