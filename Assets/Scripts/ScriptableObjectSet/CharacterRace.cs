using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRace : ScriptableObject
{
    [SerializeField]
    #if UNITY_EDITOR
    [DisplayName("识别码")]
    #endif
    private string _ID;
    public string ID
    {
        get
        {
            return _ID;
        }
    }

    [SerializeField]
    #if UNITY_EDITOR
    [DisplayName("种群名")]
    #endif
    private string _name;
    public new string name
    {
        get
        {
            return _name;
        }
    }

    [SerializeField,TextArea(2,4)]
    private string _Desc;
    public string Desc
    {
        get { return _Desc; }
    }


    [Space,SerializeField]
    private int _index;
    public int Index
    {
        get { return _index; }
    }

    [SerializeField,ReadOnly]
    private SDConstants.CharacterType characterType;
    public SDConstants.CharacterType CharacterType
    {
        get { return characterType; }
        protected set { characterType = value; }
    }
}
