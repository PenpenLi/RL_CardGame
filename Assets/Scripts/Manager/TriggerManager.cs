using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TriggerStateListner(string name, bool value);
public class TriggerManager : PersistentSingleton<TriggerManager>
{
    #region 标准监听器
    public Dictionary<string, bool> Triggers { get; } = new Dictionary<string, bool>();

    public event TriggerStateListner OnTriggerSetEvent;

    public void SetTrigger(string triggerName, bool value)
    {
        if (!Triggers.ContainsKey(triggerName))
            Triggers.Add(triggerName, value);
        else Triggers[triggerName] = value;
        OnTriggerSetEvent?.Invoke(triggerName, value);
        
    }

    public bool GetTriggerState(string triggerName)
    {
        if (!Triggers.ContainsKey(triggerName)) return false;
        else return Triggers[triggerName];
    }
    #endregion

    #region 物品互动监听器

    public delegate void ItemAmountListener(string ID, int leftAmount);
    public event ItemAmountListener OnGetItemEvent;
    public event ItemAmountListener OnLoseItemEvent;
    public void WhenGetItem(string id)
    {
        OnGetItemEvent?.Invoke(id, SDDataManager.Instance.GetItemAmount(id));
    }
    public void WhenLoseItem(string id)
    {
        OnLoseItemEvent?.Invoke(id, SDDataManager.Instance.GetItemAmount(id));
    }
    #endregion

    #region 战斗监听器
    #region 死亡监听
    public delegate void UnitDeathListener(string id);
    public delegate void HeroDeathExtraListener(int hashcode);

    public event UnitDeathListener OnUnitDeathEvent;
    public event HeroDeathExtraListener OnHeroDeathExtraEvent;
    public void WhenUnitDie(BattleRoleData unit)
    {
        OnUnitDeathEvent?.Invoke(unit.UnitId);
        if (!unit.IsEnemy) OnHeroDeathExtraEvent?.Invoke(unit.unitHashcode);
    }
    #endregion


    #endregion
}

