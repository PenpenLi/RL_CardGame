using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="character_info",menuName ="Wun/角色/角色信息")]
public class CharacterInfo : ScriptableObject
{
    [SerializeField]
    protected string _ID;
    public string ID
    {
        get
        {
            return _ID;
        }
    }

    public int N_ID
    {
        get { return SDDataManager.Instance.getInteger(ID.Split('#')[1]); }
    }

    [SerializeField]
    protected string _Name;
    public string Name
    {
        get
        {
            return _Name;
        }
    }

    [SerializeField]
    [EnumMemberNames("未知", "男", "女")]
    protected CharacterSex sex;
    public CharacterSex Sex
    {
        get
        {
            return sex;
        }
    }
    public int Gender 
    {
        get 
        {
            if (Sex == CharacterSex.Male) return 0;
            else if (Sex == CharacterSex.Female) return 1;
            else return -1;
        }
        set
        {
            if (value == 0) sex = CharacterSex.Male;
            else if (value == 1) sex = CharacterSex.Female;
            else sex = CharacterSex.Unknown;
        }
    }

    [SerializeField, ReadOnly]
    private SDConstants.CharacterType characterType;
    public SDConstants.CharacterType CharacterType
    {
        get { return characterType; }
        protected set { characterType = value; }
    }
}
public enum CharacterSex
{
    Unknown,
    Male,
    Female,
}