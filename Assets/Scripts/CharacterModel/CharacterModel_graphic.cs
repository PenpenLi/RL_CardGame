using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
using GameDataEditor;
using System.Linq;
using Spine.Unity.AttachmentTools;

public class CharacterModel_graphic : CharacterModel
{
    [Space(25)]
    SkeletonGraphic skeletonAnimation;
    public CharacterModel_graphic()
    {
        thisUDE = CharacterModelController.UDE.Graphic;
    }
    public override void Prepare()
    {
        skeletonAnimation = GetComponent<SkeletonGraphic>();
        if (skeletonAnimation)
        {
            spineAnimationState = skeletonAnimation.AnimationState;
            skeleton = skeletonAnimation.Skeleton;
        }
        base.Prepare();

        //
        spineAnimationState.SetAnimation(0, anim_idle, true);
    }


    public override IEnumerator IEChangeModelAnim(string animName, bool IsLoop)
    {
        var track = spineAnimationState.SetAnimation(0, animName, IsLoop);
        return base.IEChangeModelAnim(animName, IsLoop);
    }
}
