using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏特效管理类，包括特效及可替换框
/// </summary>
public class SLEffectManager : PersistentSingleton<SLEffectManager>
{
    public Transform commonEffectNormalAttack;
    public Transform commonEffectLightning;
    public Transform commonEffectHeal;
    public Transform commonEffectStorm;

    public Sprite[] bagItemBgSps;
    public Sprite[] typeBtnBgSps;
    public void playEnemyBornEffect(Vector3 pos, Transform parent)
    {

    }

    public void playCommonEffectNormalAttack(Vector3 pos)
    {
        //SDGameManager.Instance.audio

    }
    public void playCommonEffectSlash(Vector3 pos)
    {

    }

    public void playCommonEffectCast(Vector3 pos)
    {

    }

    public void playCommonEffectRevive(Vector3 pos)
    {

    }

    public void playCommonEffectLocalBarChartAdd(Vector3 pos,SDConstants.BCType tag = SDConstants.BCType.hp)
    {

    }

    public void playCommonEffectTagState(Vector3 pos, StateTag stateTag = StateTag.Hush)
    {

    }
}
