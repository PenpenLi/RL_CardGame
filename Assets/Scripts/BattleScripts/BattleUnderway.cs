using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Program
{
    static Player player;
    static Monster monster;

    static Role GetSkillTarget(skill skill, Role self, Role enemy)
    {
        if (skill.Target == SkillTarget.ToFoe)
        {
            return enemy;
        }
        else if (skill.Target == SkillTarget.ToFriend)
        {
            //return self;
        }
        else if (skill.Target == SkillTarget.ToSelf)
        {
            return self;
        }
        else if (skill.Target == SkillTarget.NoAim)
        {

        }
        return null;
    }
    static bool RoleAct(Role self, Role other)
    {
        //状态生效
        self.StateAffect();

        skill s = self.SelectSkill(1);
        other.BeHit_AP(s);
        if (other.hp <= 0 || self.hp <= 0)
        {
            return false;
        }
        return true;
    }
    static void Main(string[] args)
    {
        player = new Player(1,"战士", 100,50,50 ,15, 10, 15, 10, 50,1);
        player.skills.Add(skill.CreateDamageSkill("攻击", SkillTarget.ToFoe, player.AP));
        player.skills.Add(skill.CreateHealSkill("急救", SkillTarget.ToSelf, player.AP));
        State state = new State(StateType.DamageOverTime, 3, 5);
        player.skills.Add(skill.CreateAddStateSkill("割裂", SkillTarget.ToFoe, state));

        monster = new Monster(2,"触手怪", 100,50,50, 10, 10, 10, 10, 50,1);
        monster.skills.Add(skill.CreateDamageSkill("攻击", SkillTarget.ToFoe, monster.AP));
        monster.skills.Add(skill.CreateHealSkill("急救", SkillTarget.ToSelf, monster.AP));
        while (true)
        {
            if (!RoleAct(player, monster)) { break; }
            if (!RoleAct(monster, player)) { break; }
        }
        if (player.hp > 0) { Debug.Log("玩家获胜"); }
        else { Debug.Log("怪物获胜"); };
    }
}



