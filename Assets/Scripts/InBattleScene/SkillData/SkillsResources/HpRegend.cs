using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpRegend : SkillFunction
{
    public int lastTime;
    protected SDConstants.BCType _stateTag = SDConstants.BCType.hp;
    public override void StartSkill(BattleRoleData source, BattleRoleData target)
    {
        base.StartSkill(source, target);
        if (IsProcessing) return;
        IsProcessing = true;
        IsUsed = true;
        CalculateBeforeFunction(source, target);

        List<BattleRoleData> list = DealWithAOEAction(source, target,AOEType);
        for(int i = 0; i < list.Count; i++)
        {
            StartCoroutine(IEStartSkill(source, list[i], NumberData.Build(ValCaused(source, list[i])), lastTime));
        }
    }
    public void PropStartSkill(BattleRoleData source, BattleRoleData target, NumberData val
    , int lastTime, SDConstants.AOEType AOEType = SDConstants.AOEType.None)
    {
        base.StartSkill(source, target);
        IsProcessing = true;
        IsUsed = true;

        if (GetComponent<HSkilInfo>())
        {
            GetComponent<HSkilInfo>().AOEType = AOEType;
            List<BattleRoleData> list = DealWithAOEAction(source, target,AOEType);
            for (int i = 0; i < list.Count; i++)
            {
                StartCoroutine(IEStartSkill(source, list[i], val, lastTime));
            }
        }
        else
        {
            StartCoroutine(IEStartSkill(source, target, val, lastTime));
        }
    }
    public IEnumerator IEStartSkill(BattleRoleData source, BattleRoleData target
        , NumberData _val,int lastTime)
    {
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_cast, false);
        SLEffectManager.Instance.playCommonEffectCast(source.transform.position);
        yield return new WaitForSeconds(castLastTime);

        SLEffectManager.Instance.playCommonEffectLocalBarChartAdd(target.transform.position);
        yield return new WaitForSeconds(effectLastTime);

        source.unit_character_model.CurrentCharacterModel 
            .ChangeModelAnim(source.unit_character_model.CurrentCharacterModel.anim_idle, true);
        int val = _val.DATA;
        if(_val.dataTag == NumberData.DataType.percent)
        {
            val = (int)(target.HpController.MaxHp * _val.DECIMAL);
        }
        //target.ControlRegendVisual(_stateTag, lastTime,val);
        OneStateController state = new OneStateController()
        {
            ID = name + "#" + _stateTag.ToString().ToUpper(),
            NAME = name,
            BarChart = new RoleBarChart() { HP = val, },
            StateTag = StateTag.End,
            LastTime = lastTime,
        };
        if (target.AddStandardState(state))
        {
            Debug.Log(source.name + " heal cause " + val + " to " + target.name);
        }

        yield return new WaitForSeconds(skillLastTime);
        StartCoroutine(IEWaitForShortEnd(source));
    }

    public int ValCaused(BattleRoleData source, BattleRoleData target)
    {
        HSExportDmgModifity S = GetComponent<HSExportDmgModifity>();
        if (S)
        {
            //int level = source.ThisBasicRoleProperty().LEVEL;
            return S.AllResult(source, target, AttributeData.MT, SkillGrade);
        }
        return source.ThisBasicRoleProperty().ReadRA(AttributeData.MT)/10;
    }
}
