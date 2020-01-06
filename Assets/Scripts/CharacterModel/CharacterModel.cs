using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
using GameDataEditor;

public class CharacterModel : MonoBehaviour
{
    public SDConstants.CharacterModelType modelType;
    public bool isFinalBossModel = false;

    public bool _isDead;
    public bool isEnemy;
    public bool isBoss;
    //public GDEAnimData AnimData;
    public CharacterModelController CMC;
    public int hashcode
    {
        get { return CMC.heroHashcode; }
    }
    public SDConstants.CharacterType _Tag;
    public SDConstants.EstateType estate_type = SDConstants.EstateType.Idle;
    public bool facingLeft;
    //public bool isDead;
    [Range(-1f, 1f)]
    public float currentSpeed;

    public System.Action ActionEvent;//let other scripts know when thisUnit is Acting
    //public AnimationReferenceAsset run, idle, action, die;
    [SpineAnimation]
    public string anim_idle;
    [SpineAnimation]
    public string anim_attack;
    [SpineAnimation]
    public string anim_hurt;
    [SpineAnimation]
    public string anim_cast;
    [SpineAnimation]
    public string anim_die;
    [SpineAnimation]
    public string anim_jump;
    [SpineAnimation]
    public string anim_fade;
    [Space(25)]
    [SpineAnimation] public string current_anim_name;
    SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;
    [Space(15)]
    public List<Chara_atlasRegionAttacher> ARAList = new List<Chara_atlasRegionAttacher>();
    public Model_ColorInitialize colorInitialize;
    public int SkeletonIndex;
    private void OnEnable()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (skeletonAnimation)
        {
            spineAnimationState = skeletonAnimation.AnimationState;
            skeleton = skeletonAnimation.Skeleton;
        }
    }
    public virtual void initCharacterModel(int _skeletonIndex)
    {
        SkeletonIndex = _skeletonIndex;
        //mixAndMatch = GetComponent<Chara_mixAndMatch>();
        colorInitialize = GetComponent<Model_ColorInitialize>();
        anim_idle = "animation";
        anim_attack = "hit";
        anim_cast = "hit";
        anim_hurt = "hurt";
        anim_jump = "jump";
        ChangeModelAnim(anim_idle, true);
        if (FindObjectOfType<HomeScene>())
        {
            GetComponent<MeshRenderer>().sortingLayerName = "UI";
            GetComponent<MeshRenderer>().sortingOrder = 1;
        }
        else
        {
            GetComponent<MeshRenderer>().sortingLayerName = "Role";
        }
        //mixAndMatch.initThisMatch();
        ReadAnimImgList.initCharaModelByGDE(this);
        StartCoroutine(applyARA());
    }
    IEnumerator applyARA()
    {
        yield return new WaitForSeconds(0.05f);
        for(int i = 0; i < ARAList.Count; i++)
        {
            ARAList[i].CM = this;
            ARAList[i].StartToApply(skeletonAnimation);
            yield return null;
        }
    }


    public void testBtn()
    {
        ChangeModelAnim(anim_attack);
    }
    public void testBtn1()
    {
        ChangeModelAnim(anim_hurt);
    }
    public void ChangeModelAnim(string TriggerName, bool IsLoop = false)
    {
        if (_isDead) return;
        if (TriggerName == null || TriggerName == "") return;
        if (isFinalBossModel)
        {
            if (TriggerName == anim_hurt
                            || TriggerName == anim_attack
                            || TriggerName == anim_die)
            {
                transform.DOShakePosition(0.2f, new Vector3(1, 1, 1));
            }
        }
        if (_Tag == SDConstants.CharacterType.Hero
            || _Tag == SDConstants.CharacterType.Enemy)
        {
            current_anim_name = TriggerName;
            StartCoroutine(IEChangeModelAnim(TriggerName, IsLoop));

        }
    }
    public IEnumerator IEChangeModelAnim(string animName,bool IsLoop)
    {
        var track = spineAnimationState.SetAnimation(0, animName, IsLoop);
        //var empty = skeletonAnimation.state.AddEmptyAnimation(1, 0.5f, 0.1f);
        if (!IsLoop)
        {
            spineAnimationState.AddAnimation(0, anim_idle, true, 0);
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void SetReplaceImgState(bool state)
    {

    }


 
}
