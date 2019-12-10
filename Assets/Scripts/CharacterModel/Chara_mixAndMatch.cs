using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Spine.Unity.AttachmentTools;
using GameDataEditor;

public class Chara_mixAndMatch : MonoBehaviour
{
    #region Inspector
    [SpineSkin]
    public string templateAttachmentsSkin = "default";
    // This will be used as the basis for shader and material property settings.
    public Material sourceMaterial; 
    [System.Serializable]
    public class SlotRegionPair
    {
        public Sprite newSprite;
        [SpineSlot] public string Slot;
        [SpineAttachment(slotField: "Slot", skinField: "baseSkinName")] public string Key;
    }
    public List<SlotRegionPair> AllReplacePairs = new List<SlotRegionPair>();

    [Header("Runtime Repack")]
    public bool repack = true;
    public BoundingBoxFollower bbFollower;
    [Header("Do not assign")]
    public Texture2D runtimeAtlas;
    public Material runtimeMaterial;
    #endregion
    Skin customSkin;
    void OnValidate()
    {
        if (sourceMaterial == null)
        {
            var skeletonAnimation = GetComponent<SkeletonAnimation>();
            if (skeletonAnimation != null)
                sourceMaterial = skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
        }
    }
    private void Start()
    {
        Apply();
    }
    void Apply()
    {
        var skeletonAnimation = GetComponent<SkeletonAnimation>();
        var skeleton = skeletonAnimation.Skeleton;

        // STEP 0: PREPARE SKINS
        // Let's prepare a new skin to be our custom skin with equips/customizations. 
        //We get a clone so our original skins are unaffected.
        customSkin = customSkin ?? new Skin("custom skin");
        // This requires that all customizations are done with skin placeholders defined in Spine.
        //customSkin = customSkin ?? skeleton.UnshareSkin(true, false, skeletonAnimation.AnimationState); // use this if you are not customizing on the default skin.
        var templateSkin = skeleton.Data.FindSkin(templateAttachmentsSkin);

        // STEP 1: "EQUIP" ITEMS USING SPRITES
        // STEP 1.1 Find the original/template attachment.
        // Step 1.2 Get a clone of the original/template attachment.
        // Step 1.3 Apply the Sprite image to the clone.
        // Step 1.4 Add the remapped clone to the new custom skin.

        for (int i = 0; i < AllReplacePairs.Count; i++)
        {
            SlotRegionPair Pair = AllReplacePairs[i];
            int slotIndex = skeleton.FindSlotIndex(Pair.Slot);
            // You can access GetAttachment and SetAttachment via string
            //, but caching the slotIndex is faster.
            Attachment templateAttachment = templateSkin.GetAttachment(slotIndex, Pair.Key);// STEP 1.1
            Attachment newAttachment = templateAttachment.GetRemappedClone
                (Pair.newSprite, sourceMaterial);// STEP 1.2 - 1.3


            if (newAttachment != null)
                customSkin.SetAttachment(slotIndex, Pair.Key, newAttachment);//STEP 1.4
        }

        if (repack)
        {
            var repackedSkin = new Skin("repacked skin");
            repackedSkin.AddAttachments(skeleton.Data.DefaultSkin); // Include the "default" skin. (everything outside of skin placeholders)
            repackedSkin.AddAttachments(customSkin); // Include your new custom skin.
            repackedSkin = repackedSkin.GetRepackedSkin("repacked skin", sourceMaterial, out runtimeMaterial, out runtimeAtlas); // Pack all the items in the skin.
            skeleton.SetSkin(repackedSkin); // Assign the repacked skin to your Skeleton.
            if (bbFollower != null) bbFollower.Initialize(true);
        }
        else
        {
            skeleton.SetSkin(customSkin); // Just use the custom skin directly.
        }

        skeleton.SetSlotsToSetupPose(); // Use the pose from setup pose.
        skeletonAnimation.Update(0); // Use the pose in the currently active animation.

        Resources.UnloadUnusedAssets();
    }

    public void initThisMatch()
    {
        var skeletonAnimation = GetComponent<SkeletonAnimation>();
        var all = skeletonAnimation.skeleton.Data.Animations;

        //SlotRegionPair Pair = new SlotRegionPair();
        //Pair.newSprite = Resources.Load<Sprite>("Textures/timg");
        //Pair.Slot = "wuqi";
        //Pair.Key = "wuqi";
        //AllReplacePairs.Add(Pair);

        //
        readHeroAnimImg();


        //
        Apply();
    }


    public void readHeroAnimImg()
    {
        int hc = GetComponent<CharacterModel>().hashcode;
        GDEAnimData AD = SDDataManager.Instance.getHeroByHashcode(hc)?.AnimData;
        Debug.Log("BUildCharaModel" + AD.isRare);
        if (AD != null && !AD.isRare)
        {
            Debug.Log("BUildCharaModel");
            if (!string.IsNullOrEmpty(AD.body))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.body), AD.body);
                Pair.Slot = Pair.Key = nameof(AD.body);
                AllReplacePairs.Add(Pair);
            }            
            if (!string.IsNullOrEmpty(AD.eyes))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.eyes), AD.eyes);
                Pair.Slot = Pair.Key = nameof(AD.eyes)+1;
                AllReplacePairs.Add(Pair);
            }          
            if (!string.IsNullOrEmpty(AD.handR))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.handR), AD.handR);
                Pair.Slot = Pair.Key = nameof(AD.handR);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.hair))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.hair), AD.hair);
                Pair.Slot = Pair.Key = nameof(AD.hair) + 1;
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.hips))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.hips), AD.hips);
                Pair.Slot = Pair.Key = nameof(AD.hips);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.L_hand_a))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.L_hand_a), AD.L_hand_a);
                Pair.Slot = Pair.Key = nameof(AD.L_hand_a);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.L_hand_b))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.L_hand_b), AD.L_hand_b);
                Pair.Slot = Pair.Key = nameof(AD.L_hand_b);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.L_hand_c))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.L_hand_c), AD.L_hand_c); ;
                Pair.Slot = Pair.Key = nameof(AD.L_hand_c);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.L_jiao))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.L_jiao), AD.L_jiao);
                Pair.Slot = Pair.Key = nameof(AD.L_jiao);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.L_leg_a))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.L_leg_a), AD.L_leg_a);
                Pair.Slot = Pair.Key = nameof(AD.L_leg_a);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.L_leg_b))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.L_leg_b), AD.L_leg_b);
                Pair.Slot = Pair.Key = nameof(AD.L_leg_b);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.liuhai))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.liuhai), AD.liuhai);
                Pair.Slot = Pair.Key = nameof(AD.liuhai);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.R_leg_a))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.R_leg_a), AD.R_leg_a);
                Pair.Slot = Pair.Key = nameof(AD.R_leg_a);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.R_leg_b))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.R_leg_b), AD.R_leg_b);
                Pair.Slot = Pair.Key = nameof(AD.R_leg_b);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.head))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.head), AD.head);
                Pair.Slot = Pair.Key = nameof(AD.head);
                AllReplacePairs.Add(Pair);
            }
            if (!string.IsNullOrEmpty(AD.faceother))
            {
                SlotRegionPair Pair = new SlotRegionPair();
                Pair.newSprite = SDDataManager.Instance.getSpriteFromAtlas(nameof(AD.faceother), AD.faceother);
                Pair.Slot = "face1"; 
                Pair.Key = nameof(AD.faceother);
                AllReplacePairs.Add(Pair);
            }
        }
    }
}