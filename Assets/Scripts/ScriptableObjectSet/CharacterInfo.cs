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
        set { _ID = value; }
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
        set { _Name = value; }
    }

    [SerializeField, TextArea]
    private string _DESC;
    public string DESC 
    { get { return _DESC; } protected set { _DESC = value; } }

    [SerializeField]
    [EnumMemberNames("未知", "男", "女")]
    protected CharacterSex sex;
    public CharacterSex Sex
    {
        get
        {
            return sex;
        }
        set
        {
            sex = value;
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
        set { characterType = value; }
    }

    [SerializeField]
    private Sprite faceIcon;
    public Sprite FaceIcon 
    { get { return faceIcon; }set { faceIcon = value; } }

    public virtual void initData(string id,string name,string desc,CharacterSex sex,string faceIcon,SDConstants.CharacterType ctype)
    {
        ID = id;Name = name; DESC = desc; Sex = sex;
        CharacterType = ctype;
    }
}
public enum CharacterSex
{
    Unknown=0,
    Male=1,
    Female=2,
}