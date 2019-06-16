using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    DamageOverTime,
    HealOverTime,

}
[System.Serializable]
public class State
{
    public StateType type;
    public int time;
    public int data;
    public State(StateType _type, int _time, int _data)
    {
        type = _type;
        time = _time;
        data = _data;
    }
    public State Copy()
    {
        return new State(type, time, data);
    }
}
public enum SkillTarget
{
    NoAim = 0,
    ToFoe,
    ToAllFoes,
    ToFriend,
    ToAllFriends,
    ToSelf,
}
public enum SkillType
{
    Damage,
    Heal,
    AddState,
    MPRegen,
    TPRegen,
}
[System.Serializable]
public class skill
{
    /// <summary>
    /// 技能ID
    /// </summary>
    int skillID;
    /// <summary>
    /// 技能名称
    /// </summary>
    public string skillName;
    /// <summary>
    /// 技能类型
    /// </summary>
    SkillType skilltype;
    public SkillType Type
    {
        get { return skilltype; }
    }
    /// <summary>
    /// 技能目标
    /// </summary>
    SkillTarget _skillTarget;
    public SkillTarget Target
    {
        get { return _skillTarget; }
        set { _skillTarget = value; }
    }

    State _state;
    public State state
    {
        get { return _state; }
        //set { _state = value; }
    }

    /// <summary>
    /// 技能暴击倍率
    /// </summary>
    public int multiply = 1;
    /// <summary>
    /// 技能伤害
    /// </summary>
    int _skilldamage = 0;
    public int skillDamage
    {
        get { return _skilldamage * multiply; }
        private set { _skilldamage = value; }
    }
    /// <summary>
    /// 技能治疗
    /// </summary>
    int _skillheal = 0;
    public int skillHeal
    {
        get { return _skillheal * multiply; }
        set { _skillheal = value; }
    }
    /// <summary>
    /// 技能构造
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="Target">类型</param>
    /// <param name="damage">伤害</param>
    //public skill()

    public void SetDamageMutiply(int multi)
    {
        multiply = multi;
    }

    //用于创建技能的辅助方法
    static public skill CreateDamageSkill(string n, SkillTarget t, int damage)
    {
        skill s = new skill();
        s.skillName = n;
        s.skilltype = SkillType.Damage;
        s._skillTarget = t;
        s.skillDamage = damage;
        return s;
    }
    static public skill CreateHealSkill(string n, SkillTarget t, int heal)
    {
        skill s = new skill();
        s.skillName = n;
        s.skilltype = SkillType.Heal;
        s._skillTarget = t;
        s.skillHeal = heal;
        return s;
    }
    static public skill CreateAddStateSkill(string n, SkillTarget t, State _state)
    {
        skill s = new skill();
        s.skillName = n;
        s.skilltype = SkillType.AddState;
        s._skillTarget = t;
        s._state = _state;
        return s;
    }
}


public class RoleEntry
{
    public Role[] RoleArraies;
}
[System.Serializable]
public class Role
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public int ID;
    /// <summary>
    /// 角色名称
    /// </summary>
    public string name;
    /// <summary>
    /// 生命
    /// </summary>
    public int hp;
    /// <summary>
    /// 最大血量
    /// </summary>
    public int MaxHp;
    /// <summary>
    /// 魔法值
    /// </summary>
    public int sp;
    /// <summary>
    /// 最大魔力值
    /// </summary>
    public int MaxSp;
    /// <summary>
    /// 怒气值
    /// </summary>
    public int tp;
    /// <summary>
    /// 最大怒气值
    /// </summary>
    public int MaxTp;
    /// <summary>
    /// 物攻
    /// </summary>
    public int AP;
    /// <summary>
    /// 物防
    /// </summary>
    public int AD;
    /// <summary>
    /// 魔攻
    /// </summary>
    public int MP;
    /// <summary>
    /// 魔防
    /// </summary>
    public int MD;
    /// <summary>
    /// 速度(计算时处以100)
    /// </summary>
    public int Speed;
    /// <summary>
    /// 受击概率
    /// </summary>
    public float TauntRate;
    /// <summary>
    /// 技能列表
    /// </summary>
    public List<skill> skills = new List<skill>();
    /// <summary>
    /// 状态列表
    /// </summary>
    public List<State> states = new List<State>();
    /// <summary>
    /// 角色构造
    /// </summary>
    /// <param name="_hp">生命</param>
    /// <param name="_ap">物攻</param>
    /// <param name="_ad">物防</param>
    /// <param name="_mp">魔攻</param>
    /// <param name="_md">魔防</param>
    public Role(int _id,string n, int _hp,int _sp,int _tp, int _ap, int _ad, int _mp, int _md, int _speed,float taunt)
    {
        ID = _id;
        name = n;
        hp = _hp;
        MaxHp = _hp;
        sp = _sp;
        MaxSp = _sp;
        tp = 0;
        MaxTp = _tp;
        AP = _ap;
        AD = _ad;
        MP = _mp;
        MD = _md;
        Speed = _speed;
        TauntRate = taunt;
    }
    /// <summary>
    /// 基类函数
    /// </summary>
    /// <returns></returns>
    virtual public skill SelectSkill(int n)
    {
        if (n < 0 || n > skills.Count)
        {
            Debug.Log("超出技能列表范围范围");
            return skills[0];
        }
        return skills[Mathf.Min(skills.Count-1,n)];
    }
    public virtual void BeHit_AP(skill skill)
    {
        int cost_hp = 0;
        switch (skill.Type)
        {
            case SkillType.Damage:
                cost_hp = Mathf.Max(1, skill.skillDamage * 2 - AD);
                ChangeHp(-cost_hp);
                break;
            case SkillType.Heal:
                cost_hp = skill.skillHeal;
                ChangeHp(cost_hp);
                break;
            case SkillType.AddState:
                AddState(skill.state);
                break;
        }

        //Debug.Log("错误：不能调用skill的基类");
    }

    private void ChangeHp(int hp)
    {
        this.hp = Mathf.Min(MaxHp, this.hp + hp);
        if (this.hp < 0) this.hp = 0;
    }


    public virtual bool AddState(State state)
    {
        states.Add(state.Copy());
        return true;
    }
    public virtual bool StateAffect()
    {
        for(int i=0;i<states.Count;i++)
        {
            if (states[i].time <= 0)
            {
                Debug.Log("状态失效");
                continue;
            }
            if (states[i].type == StateType.DamageOverTime)
            {
                Debug.Log("DOT生效");
                ChangeHp(-states[i].data);
            }
            else if (states[i].type == StateType.HealOverTime)
            {
                Debug.Log("HOT生效");
                ChangeHp(states[i].data);
            }
            states[i].time--;
        }
        //遍历删除已经失效的状态。从列表删除元素时，注意从后往前删。

        return true;
    }
}

public class Player : Role
{
    public Player(int _id,string n, int _hp,int _sp,int _tp, int _ap, int _ad, int _mp, int _md, int _speed,float taunt) :
        base(_id,n, _hp,_sp,_tp, _ap, _ad, _mp, _md, _speed, taunt)
    {

    }
    /// <summary>
    /// 派生函数
    /// </summary>
    /// <param name="n">当前选择的技能</param>
    /// <returns></returns>
    public override skill SelectSkill(int n)
    {
        return base.SelectSkill(n);
    }
    public override void BeHit_AP(skill skill)
    {
        base.BeHit_AP(skill);
        //玩家特殊技能
        if (hp * 1.0f / MaxHp <= 0.5f)
        {
            Debug.Log("激怒！提升伤害");
            foreach (var j in skills)
            {
                j.SetDamageMutiply(3);
            }
        }
    }
}
public class Monster : Role
{
    public Monster(int _id,string n, int _hp,int _sp,int _tp, int _ap, int _ad, int _mp, int _md, int _speed,float taunt) :
        base(_id,n, _hp,_sp,_tp, _ap, _ad, _mp, _md, _speed, taunt)
    {

    }
    /// <summary>
    /// 派生函数
    /// </summary>
    /// <returns></returns>
    public override skill SelectSkill(int n)
    {
        return skills[Random.Range(0, skills.Count)];
    }
    public override void BeHit_AP(skill skill)
    {
        base.BeHit_AP(skill);
    }
}


