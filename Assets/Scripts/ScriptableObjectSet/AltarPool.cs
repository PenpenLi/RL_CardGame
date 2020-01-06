using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarPool : ScriptableObject 
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
    private bool _NotNormalPool;
    public bool NotNormalPool
    {
        get { return _NotNormalPool; }
        set { _NotNormalPool = value; }
    }


    public virtual List<string> IncludeIDsList
    {
        get { return null; }
    }

    public bool isHeroPool
    {
        get { return this is HeroAltarPool; }
    }
}
