using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
using GameDataEditor;
using System.Linq;
using Spine.Unity.AttachmentTools;

public class test_spineChangeMatch : MonoBehaviour
{
    public SkeletonDataAsset DataAsset;
    //public Material sourceMaterial;
    SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;

    [Header("ReplaceSet")]
    public Sprite Img;
    [SpineSlot(dataField:"DataAsset")]
    public string slot;
    Spine.BoneData thisSlotBaseData;
    Spine.BoneData thisSlotBaseData2;
    //[SpineAttachment(dataField: "DataAsset")]
    //public string attachment;
    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spineAnimationState = skeletonAnimation.AnimationState;
        skeleton = skeletonAnimation.Skeleton;
        //
        thisSlotBaseData = skeleton.FindSlot(slot).Bone.Data;
    }

    [ContextMenu("StartToReplace")]
    public void StartReplace()
    {
        Spine.Slot baseWQ = skeleton.FindSlot(slot);
        Spine.Attachment originalAttachment = baseWQ.Attachment;
        Spine.Attachment newAttachment = originalAttachment.GetRemappedClone
            (Img, originalAttachment.GetMaterial());
        baseWQ.Bone.ScaleY = -thisSlotBaseData.ScaleY;
        baseWQ.Bone.Rotation = thisSlotBaseData.Rotation - 45;
        baseWQ.Bone.SetLocalPosition(new Vector2(0, -thisSlotBaseData.Y*2));
        baseWQ.Attachment = newAttachment;
    }

    [ContextMenu("StartToReplace2")]
    public void StartReplace2()
    {
        Spine.Slot baseWQ = skeleton.FindSlot(slot);
        Spine.Attachment originalAttachment = baseWQ.Attachment;
        Spine.Attachment newAttachment = originalAttachment.GetRemappedClone
            (Img, originalAttachment.GetMaterial());
        baseWQ.Attachment = newAttachment;
        //
        Spine.Slot otherWQ = skeleton.FindSlot("R_" +slot);
        Spine.Attachment originalAttachment2 = otherWQ.Attachment;
        Spine.Attachment newAttachment2 = originalAttachment2.GetRemappedClone
            (Img, originalAttachment2.GetMaterial());
        otherWQ.Attachment = newAttachment2;
    } 
}
