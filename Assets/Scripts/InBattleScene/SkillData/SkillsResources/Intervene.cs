using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intervene : SkillFunction
{
    [Space(100)]
    public int FigureToTarget;//+-()%
    public int FigureToSource;//+-()%
    public int LastTime;
    public HSkilInfo HS
    {
        get { return GetComponent<HSkilInfo>(); }
    }
    public override void StartSkill(BattleRoleData source, BattleRoleData target)
    {
        base.StartSkill(source, target);
        IsProcessing = true;
        IsUsed = true;
        CalculateBeforeFunction(source, target);

        StartCoroutine(IEStartSkill(source, target));
    }
    public IEnumerator IEStartSkill(BattleRoleData source, BattleRoleData target)
    {
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_cast, false);
        SLEffectManager.Instance.playCommonEffectCast(source.transform.position);
        yield return new WaitForSeconds(castLastTime);

        Debug.Log(source.name + " add " + FigureToTarget + " intervence " + target.name);
        //target.ThisBasicRoleProperty().DmgReduction += FigureToTarget;
        //source.ThisBasicRoleProperty().DmgReduction += FigureToSource;
        target.initInterveneState(targetValCaused(source,target), LastTime);
        source.initInterveneState(sourceValCaused(source,target), LastTime, false);

        yield return new WaitForSeconds(skillLastTime);
        StartCoroutine(IEWaitForEnd(source));
    }

    public int targetValCaused(BattleRoleData source, BattleRoleData target)
    {
        return Mathf.Min((int)(FigureToTarget * (1 + 0.25f * SkillGrade)),85);
    }
    public int sourceValCaused(BattleRoleData source, BattleRoleData target)
    {
        return Mathf.Min((int)(FigureToSource * (1 - 0.25f * SkillGrade)), 0);
    }

}
