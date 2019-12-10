using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using Spine;
public class CharaAnimImgController : MonoBehaviour,IHasSkeletonDataAsset
{
    public SkeletonDataAsset skeletonDataAsset;
    SkeletonDataAsset IHasSkeletonDataAsset.SkeletonDataAsset 
    { get { return this.skeletonDataAsset; } }

    public Material sourceMaterial;
    public bool applyPMA = true;
    public List<AnimImgHook> pables = new List<AnimImgHook>();

    public Dictionary<ChangeAnimImgAsset, Attachment> cachedAttachments 
        = new Dictionary<ChangeAnimImgAsset, Attachment>();

    [System.Serializable]
    public class AnimImgHook
    {
        [SpineSlot]
        public string slot;
        [SpineSkin]
        public string templateSkin;
        [SpineAttachment(skinField: "templateSkin")]
        public string templateAttachment;
    }


    public SkeletonAnimation skeletonAnimation
    {
        get { return GetComponent<SkeletonAnimation>(); }
    }
    [SpineSkin]
    public string templateSkinName;
    Spine.Skin functionSkin;
    Spine.Skin collectedSkin;

    public Material runtimeMaterial;
    public Texture2D runtimeAtlas;
    //
    #region AnimImgChangeVision
    private void Start()
    {
        functionSkin = new Skin("function");
        var templateskin = skeletonAnimation.Skeleton.Data.FindSkin(templateSkinName);
        if(templateskin != null)
        {
            functionSkin.AddAttachments(templateskin);
        }
        skeletonAnimation.Skeleton.Skin = functionSkin;
        RefreshSkeletionAttachments();
    }

    public void EquipOnVision(int slotIndex,string attachmentName,Attachment attachment)
    {
        functionSkin.SetAttachment(slotIndex, attachmentName, attachment);
        skeletonAnimation.Skeleton.SetSkin(functionSkin);
        RefreshSkeletionAttachments();
    }
    public void OptimizeSkin()
    {
        // 1. Collect all the attachments of all active skins.
        collectedSkin = collectedSkin ?? new Skin("Collected skin");
        collectedSkin.Clear();
        collectedSkin.AddAttachments(skeletonAnimation.Skeleton.Data.DefaultSkin);
        collectedSkin.AddAttachments(functionSkin);

        // 2. Create a repacked skin.
        var repackedSkin = collectedSkin.GetRepackedSkin("Repacked skin", skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial, out runtimeMaterial, out runtimeAtlas);
        collectedSkin.Clear();

        // 3. Use the repacked skin.
        skeletonAnimation.Skeleton.Skin = repackedSkin;
        RefreshSkeletionAttachments();
    }
    void RefreshSkeletionAttachments()
    {
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();
        skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);// skeletonAnimation.Update(0);
    }
    #endregion


    public void getDataFromCAIAsset(ChangeAnimImgAsset asset)
    {
        AnimImgHook hook = pables.Find(x => x.slot == asset.targetSlot);
        if (hook == null) return;
        var skeletonData = skeletonDataAsset.GetSkeletonData(true);
        int slotIndex = skeletonData.FindSlotIndex(hook.slot);
        var attachment = GenerateAttachmentFromEquipAsset
            (asset, slotIndex, hook.templateSkin, hook.templateAttachment);
        EquipOnVision(slotIndex,hook.templateAttachment,attachment);
    }
    Attachment GenerateAttachmentFromEquipAsset(ChangeAnimImgAsset asset,int slotindex
        ,string templateSkinName,string templateAttachmentName)
    {
        Attachment attachment;
        cachedAttachments.TryGetValue(asset, out attachment);
        if(attachment== null)
        {
            var skeletonData = skeletonDataAsset.GetSkeletonData(true);
            var templateSkin = skeletonData.FindSkin(templateSkinName);
            Attachment templateAttachment 
                = templateSkin.GetAttachment(slotindex, templateAttachmentName);
            attachment = templateAttachment.GetRemappedClone(asset.sprite, sourceMaterial, premultiplyAlpha: this.applyPMA);

            cachedAttachments.Add(asset, attachment);
        }
        return attachment;
    }

    public void Done()
    {
        OptimizeSkin();
    }
}


[System.Serializable]
public class ChangeAnimImgAsset
{
    public Sprite sprite;
    [SpineSlot]
    public string targetSlot;
    public ChangeAnimImgAsset(string _targetSlot, string spriteName)
    {
        targetSlot = _targetSlot;
        sprite = SDDataManager.Instance.getSpriteFromAtlas(targetSlot, spriteName);
    }
}
