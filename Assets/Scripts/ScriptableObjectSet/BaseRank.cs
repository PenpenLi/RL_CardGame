using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本分类信息
/// </summary>
[System.Serializable]
public class BaseRank : ScriptableObject
{
    [SerializeField]
#if UNITY_EDITOR
    [DisplayName("识别码")]
#endif
    private string _ID;
    public string ID
    {
        get { return _ID; }
    }

    [SerializeField]
#if UNITY_EDITOR
    [DisplayName("名称")]
#endif
    private string _name;
    public string NAME
    {
        get { return _name; }
    }

    [SerializeField]
    private int _index;
    public int Index
    {
        get { return _index; }
    }

    [SerializeField,TextArea]
    private string _Desc;
    public string Desc
    {
        get { return _Desc; }
    }

    [SerializeField]
    private Sprite _Icon;
    public Sprite Icon
    {
        get { return _Icon; }
        private set { _Icon = value; }
    }
}
