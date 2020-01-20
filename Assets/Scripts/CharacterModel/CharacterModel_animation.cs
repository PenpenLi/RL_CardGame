using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
using GameDataEditor;
using System.Linq;
using Spine.Unity.AttachmentTools;
public class CharacterModel_animation : CharacterModel
{
    [Space(25)]
    SkeletonAnimation skeletonAnimation;
    public CharacterModel_animation()
    {
        thisUDE = CharacterModelController.UDE.Animation;
    }
    public override void Prepare()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (skeletonAnimation)
        {
            spineAnimationState = skeletonAnimation.AnimationState;
            skeleton = skeletonAnimation.Skeleton;
        }
        base.Prepare();

        //
        skeletonAnimation.state.SetAnimation(0, anim_idle, true);
        if (FindObjectOfType<HomeScene>())
        {
            GetComponent<MeshRenderer>().sortingLayerName = "UI";
            Canvas c = GetComponentInParent<Canvas>();
            if (c)
            {
                GetComponent<MeshRenderer>().sortingOrder = c.sortingOrder;
            }
            else GetComponent<MeshRenderer>().sortingOrder = 0;
        }
        else
        {
            GetComponent<MeshRenderer>().sortingLayerName = "Role";
        }
    }

    public override IEnumerator IEChangeModelAnim(string animName, bool IsLoop)
    {
        var track = skeletonAnimation.state.SetAnimation(0, animName, IsLoop);
        return base.IEChangeModelAnim(animName, IsLoop);
    }
}
