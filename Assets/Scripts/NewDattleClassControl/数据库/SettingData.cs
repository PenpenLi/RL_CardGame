using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingData : MonoBehaviour
{
    #region IsAutoBattle
    static string IsAutoBattleKey = "IsAutoBattle";
    bool _autoBattle;
    public bool IsAutoBattle
    {
        get{ return  _autoBattle; }
        set
        {
            if(_autoBattle != value)
            {
                _autoBattle = value;
                Messenger<bool>.Broadcast(IsAutoBattleKey, _autoBattle);
            }
        }
    }
    void OnAutoBattleKeyChanged(bool Back)
    {

    }
    #endregion
    #region IsMusicOn
    static string IsMusicOnKey = "IsMusicOn";
    bool _MusicOn;
    public bool IsMusicOn
    {
        get { return _MusicOn; }
        set
        {
            if(_MusicOn != value)
            {
                _MusicOn = value;
                Messenger<bool>.Broadcast(IsMusicOnKey, _MusicOn);
            }
        }
    }
    void OnMusicOnKeyChanged(bool Back)
    {

    }
    #endregion

    public void SaveToMessenger()
    {
        Messenger.AddListener(SDConstants.SchemaKey, AddCallBack);
        //
        Messenger<bool>.AddListener(IsAutoBattleKey, OnAutoBattleKeyChanged);
        Messenger<bool>.AddListener(IsAutoBattleKey, OnMusicOnKeyChanged);
    }
    void AddCallBack()
    {
        Debug.Log("setting类监控开启");
    }

}


public class EventListener
{
    public delegate void OnBoolChangeDelegate(bool newVal);
    public event OnBoolChangeDelegate OnVariableChange;//事件
    private bool m_boolean = false;
    public bool Boolean
    {
        get { return m_boolean; }
        set
        {
            if (m_boolean == value) return;
            OnVariableChange?.Invoke(!m_boolean);
            m_boolean = value;
        }
    }
}