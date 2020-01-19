using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttritubeChange : SkillFunction
{
    public AttritubeChange()
    {
        UseState = true;
    }

    public override void StartSkill(BattleRoleData source, BattleRoleData target)
    {
        base.StartSkill(source, target);
        IsProcessing = true;
        IsUsed = true;
        CalculateBeforeFunction(source, target);
        //List<BattleRoleData> list = DealWithAOEAction(source, target,AOEType);
        StartCoroutine(IEStartSkill(source, target));
    }
    public IEnumerator IEStartSkill(BattleRoleData source,BattleRoleData target)
    {
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_cast, false);
        SLEffectManager.Instance.playCommonEffectCast(source.transform.position);
        yield return new WaitForSeconds(castLastTime);

        List<BattleRoleData> list = DealWithAOEAction(source, target,AOEType);
        for (int i = 0; i < list.Count; i++)
        {
            stateWork(source, list[i]);
            if (_standardState.Success)
            {
                Debug.Log("成功为 " + target.name + " 添加状态 " + _standardState.NAME);
            }
        }

        yield return new WaitForSeconds(skillLastTime);
        StartCoroutine(IEWaitForEnd(source));
    }
    public override void EndSkill()
    {
        base.EndSkill();
    }
}
