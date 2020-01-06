using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;

public class GoddessProperty : MonoBehaviour
{
    public string ID;
    public string Name;
    public GoddessAttritube AttiList;

    public GoddessInfo CurrentGoddess;
    public float chargeSpeed;
    public int newFatigueAddNum;
    public int mpRegendNum;
    public int heroAttiUp;

    public GoddessSkill GSkill;
    public bool InitGoddess(string id)
    {
        CurrentGoddess = SDDataManager.Instance.getGoddessInfoById(id);
        if (CurrentGoddess == null) return false;
        GDEgoddessData GDE = SDDataManager.Instance.getGDEGoddessDataById(id);
        if (GDE == null) return false;
        //
        AttiList = CurrentGoddess.GoddessAtti;
        AttiList += SDDataManager.Instance.GetGoddessAttiByGDE(GDE.attitube);

        GDERuneData r0 = SDDataManager.Instance.getRuneOwnedByHashcode(GDE.rune0);
        GDERuneData r1 = SDDataManager.Instance.getRuneOwnedByHashcode(GDE.rune1);
        GDERuneData r2 = SDDataManager.Instance.getRuneOwnedByHashcode(GDE.rune2);
        GDERuneData r3 = SDDataManager.Instance.getRuneOwnedByHashcode(GDE.rune3);
        if(r0!=null) AttiList += SDDataManager.Instance.GetGoddessAttiByGDE(r0.attitube);
        if(r1!=null) AttiList += SDDataManager.Instance.GetGoddessAttiByGDE(r1.attitube);
        if(r2!=null) AttiList += SDDataManager.Instance.GetGoddessAttiByGDE(r2.attitube);
        if(r3!=null) AttiList += SDDataManager.Instance.GetGoddessAttiByGDE(r3.attitube);
        //
        AttiList.CheckSelf();
        //
        build_goddess_speed();
        build_goddess_fatigueReduce();
        build_goddess_mpRecover();
        build_goddess_attiUp();
        //
        if (!CurrentGoddess.HaveSkill) return true;
        GoddessSkillInfo Info = CurrentGoddess.SkillInfo;
        Transform P = Instantiate(Info.Prefab) as Transform;
        P.SetParent(this.transform);
        //
        GoddessSkill GS = P.GetComponent<GoddessSkill>();
        GS.BM = FindObjectOfType<BattleManager>();
        GS.TargetIsHero = Info.TargetIsHero;
        GS.AOE = Info.AOE;
        GS.skillGrade = GDE.skillGrade;
        GS.Breed = Info.BREED;
        GS.Kind = Info.KIND;
        GS.UseState = Info.UseState;
        GS.State = Info.State;
        //
        if (Info.SkillTag == GoddessSkillInfo.GSTag.damage)
        {
            GSDamage GSD = P.GetComponent<GSDamage>();
            GSD.DamageData = Info._DamageData;
            GSD.UpBySkillGrade = Info.UpBySkillGrade;
        }
        else if(Info.SkillTag == GoddessSkillInfo.GSTag.heal)
        {
            GSHeal GSH = P.GetComponent<GSHeal>();
            GSH.HealData = Info._HealData;
            GSH.UpBySkillGrade = Info.UpBySkillGrade;
        }
        else if(Info.SkillTag == GoddessSkillInfo.GSTag.addState)
        {
            GSAddState GSA = P.GetComponent<GSAddState>();
        }
        GSkill = GS;
        //
        return true;
    }
    public void build_goddess_speed()
    {
        chargeSpeed = SDConstants.GoddessChargeSpeed;
        int A = AttiList.Agile / SDConstants.GoddessPerAttiVolume;
        float rate = A * 1f / SDConstants.GoddessAttiGradient;
        chargeSpeed *= 1 + rate;
    }
    public void build_goddess_fatigueReduce()
    {
        newFatigueAddNum = SDConstants.fatigueAddNum;
        int S = AttiList.Stamina / SDConstants.GoddessPerAttiVolume;
        float rate = 1 - S * 0.75f / SDConstants.GoddessAttiGradient;
        newFatigueAddNum = (int)(newFatigueAddNum * rate);
    }
    public void build_goddess_mpRecover()
    {
        mpRegendNum = 0;
        int R = AttiList.Recovery / SDConstants.GoddessPerAttiVolume;
        mpRegendNum += R * 5;
    }
    public void build_goddess_attiUp()
    {
        heroAttiUp = 0;
        int L = AttiList.Leader / SDConstants.GoddessPerAttiVolume;
        heroAttiUp += L * 3;
    }
}
