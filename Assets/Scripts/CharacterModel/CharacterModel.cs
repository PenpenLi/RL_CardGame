using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
using GameDataEditor;
using System.Linq;
using Spine.Unity.AttachmentTools;
public class CharacterModel : MonoBehaviour
{
    public CharacterModelController.UDE thisUDE;
    //
    public SDConstants.CharacterModelType modelType;
    public bool isFinalBossModel = false;

    public bool _isDead;
    public bool isEnemy;
    public bool isBoss;
    public CharacterModelController CMC;
    public int hashcode
    {
        get { return CMC.heroHashcode; }
    }
    public SDConstants.CharacterType _Tag
    {
        get { return CMC.CType; }
    }
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
    [SpineAnimation]
    public string current_anim_name;
    public float current_anim_time;
    public SkeletonDataAsset DataAsset;
    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;
    [Header("SpineDetail")]
    [SpineSkin(dataField:"DataAsset")]
    public List<string> AllSkins = new List<string>();
    [SpineAnimation(dataField:"DataAsset")]
    public List<string> AllAnimations = new List<string>();
    [Space(15)]
    public Model_ColorInitialize colorInitialize;
    //
    Spine.BoneData weaponBaseData;
    Spine.BoneData weaponBaseData2;
    [Space(15)]
    public Sprite weaponSprite;
    public Sprite weaponSprite2;
    public virtual void Prepare()
    {
        colorInitialize = GetComponent<Model_ColorInitialize>();
        anim_idle = SDConstants.AnimName_IDLE;
        anim_attack = SDConstants.AnimName_ATTACK;
        anim_cast = SDConstants.AnimName_CAST;
        anim_hurt = SDConstants.AnimName_HURT;
        anim_jump = SDConstants.AnimName_JUMP;
        anim_die = SDConstants.AnimName_DIE;
    }
    public virtual void initCharacterModel()
    {
        Prepare();
        //
        AllSkins = skeleton.Data.Skins.Select(x => x.Name).ToList();
        AllAnimations = skeleton.Data.Animations.Select(x => x.Name).ToList();

        if (AllSkins.Contains(CMC.skinName))
        {
            skeleton.SetSkin(CMC.skinName);
        }
        else
        {
            skeleton.SetSkin("default");
        }

        if(_Tag == SDConstants.CharacterType.Hero)
        {
            HeroCharacterSet();
        }
    }
    void HeroCharacterSet()
    {
        Debug.Log("Hero_Character_Set");
        string w0 = "wuqi";
        string w1 = "R_wuqi";
        weaponBaseData = skeleton.FindSlot(w0).Bone.Data;
        weaponBaseData2 = skeleton.FindSlot(w1).Bone.Data;
        //
        Material M = GetComponent<MeshRenderer>().material;
        if(skeleton.FindSlot(w0).Attachment != null)
            M.shader = skeleton.FindSlot(w0).Attachment.GetMaterial().shader;

        if (CMC.notShowWeaponSlot)
        {
            skeleton.FindSlot(w0).A = 0;
            skeleton.FindSlot(w1).A = 0;
        }
        else
        {
            skeleton.FindSlot(w0).A = 1;
            skeleton.FindSlot(w1).A = 1;
            //
            EquipItem weapon = SDDataManager.Instance.GetEquipItemById(CMC.weaponId);
            if (weapon == null) return;
            List<Sprite> allSprites = Resources.LoadAll<Sprite>("Spine/WeaponVision/").ToList();
            weaponSprite = allSprites.Find(x => x.name == weapon.ID);
            if (weaponSprite == null) return;


            Spine.Slot baseWQ = skeleton.FindSlot(w0);
            string an = baseWQ.Data.AttachmentName;
            Spine.Attachment originalAttachment = baseWQ.Attachment;
            Spine.Attachment newAttachment = originalAttachment.GetRemappedClone(weaponSprite, M);
            if(weapon.WeaponRace.WeaponClass == SDConstants.WeaponClass.Sharp)
            {
                Spine.Slot otherWQ = skeleton.FindSlot(w1);
                string newWSName = weaponSprite.name + "_2";
                weaponSprite2 = allSprites.Find(x => x.name == newWSName);
                if (weaponSprite2 != null)
                {
                    Spine.Attachment oa1 = otherWQ.Attachment;
                    Spine.Attachment newoa1 = oa1.GetRemappedClone(weaponSprite2, M);
                    otherWQ.Attachment = newoa1;
                }
            }else if(weapon.WeaponRace.WeaponClass == SDConstants.WeaponClass.Claymore)
            {
                baseWQ.Bone.ScaleY = -weaponBaseData.ScaleY;
                baseWQ.Bone.Rotation = weaponBaseData.Rotation - 45;
                baseWQ.Bone.SetLocalPosition(new Vector2(0, -weaponBaseData.Y * 2));
            }
            baseWQ.Attachment = newAttachment;
        }
    }


    #region extra

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
        current_anim_time = 0;
        if (_Tag == SDConstants.CharacterType.Hero
            || _Tag == SDConstants.CharacterType.Enemy)
        {
            string AnimName = TriggerName;
            if (_Tag == SDConstants.CharacterType.Hero)
            {
                HeroInfo info = SDDataManager.Instance.getHeroInfoById(CMC.id);
                Job career = info.Career.Career;
                List<string> fixedAnimNames = AllAnimations.FindAll
                    (x => x.Contains(TriggerName));
                AnimName = fixedAnimNames.Find(x => x.Contains(TriggerName));
                //
                if(TriggerName == anim_attack)
                {
                    if (career == Job.Ranger)
                    {
                        AnimName = fixedAnimNames.Find(x => x.Contains("2"));
                    }
                    else
                    {
                        AnimName = fixedAnimNames.Find(x => x.Contains("1"));
                        if (AnimName == null)
                        {
                            AnimName = "attect-01";
                        }
                    }
                    current_anim_time = SDConstants.AnimTime_ATTACK;
                }
                if(TriggerName == anim_cast)
                {
                    if (career == Job.Priest)
                    {
                        AnimName = fixedAnimNames.Find(x => x.Contains("3"));
                    }
                    else
                    {
                        AnimName = fixedAnimNames.Find(x => x.Contains("2"));
                    }
                    BattleRoleData source = GetComponentInParent<BattleRoleData>();
                    BattleManager BM = source.BM;
                    SkillFunction skill = BM._currentSkill;
                    SDConstants.AOEType aoe = skill.AOEType;
                    if (skill.IsOmega)
                    {
                        AnimName = fixedAnimNames.Find(x => x.Contains("4"));
                    }
                    current_anim_time = SDConstants.AnimTime_CAST;
                }
                if(TriggerName == anim_jump)
                {
                    AnimName = AllAnimations.Find(x => x.Contains("cast") && x.Contains("1"));
                }
                if(TriggerName == anim_die)
                {
                    current_anim_time = SDConstants.AnimTime_DIE;
                }
            }
            else
            {
                AnimName = AllAnimations.Find(x => x.Contains(TriggerName));
                if (AnimName == null) return;

            }
            current_anim_name = AnimName;
            StartCoroutine(IEChangeModelAnim(AnimName, IsLoop));
        }
    }
    public virtual IEnumerator IEChangeModelAnim(string animName,bool IsLoop)
    {
        //var track = skeletonAnimation.state.SetAnimation(0, animName, IsLoop);
        //var empty = skeletonAnimation.state.AddEmptyAnimation(1, 0.5f, 0.1f);
        yield return new WaitForSeconds(current_anim_time);
        if (!IsLoop)
        {
            spineAnimationState.AddAnimation(0, anim_idle, true, 0);
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void SetReplaceImgState(bool state)
    {

    }

    [ContextMenu("StartAnim")]
    public void Test_ChangeModeAnim()
    {
        ChangeModelAnim(current_anim_name, false);
    }
#endregion

}
