using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using GameDataEditor;
using System;
using System.Net;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.U2D;
using Spine.Unity;
using Spine;

/// <summary>
/// 数据管理类，包括游戏内所有记录数据
/// </summary>
public class SDDataManager : PersistentSingleton<SDDataManager>
{
    public GDEPlayerData PlayerData;
    public GDESettingData SettingData;
    public GDEResidentMovementData ResidentMovementData;

    public int heroNum = 0;
    public int equipNum = 0;
    public int slaveNum = 0;
    public int runeNum = 0;
    public List<string> AllStrs;
    public List<string> AllStrs2;

    public DateTime OpenTime;
    public override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            GDEDataManager.Init("gde_data");
            SetupDatas();
            gameObject.AddComponent<ResourceManager>();
        }
    }
    private void Start()
    {
        SetupDatas();
        //StartCoroutine(IELoadFileAsynchronously());
    }
    #region Infor

    #region GameChapterInfor
    public void addUnlockedChapter()
    {
        //
    }
    #endregion
    #region HeroInfor
    public void SetupDatas()
    {
        PlayerData = new GDEPlayerData(GDEItemKeys.Player_CurrentPlayer);
        SettingData = new GDESettingData(GDEItemKeys.Setting_Setting);
        PlayerData.achievementData = new GDEAchievementData(GDEItemKeys.Achievement_newAchievement);
        ResidentMovementData
            = new GDEResidentMovementData(GDEItemKeys.ResidentMovement_EmptyResidentMovement);

        if (PlayerData.herosOwned != null)
        {

        }
        if (PlayerData.heroesTeam != null)
        {
            if (PlayerData.heroesTeam.Count == 0)
            {
                for (int i = 0; i < SDConstants.MaxSelfNum; i++)
                {
                    GDEunitTeamData team = new GDEunitTeamData(GDEItemKeys.unitTeam_emptyHeroTeam)
                    {
                        id = string.Empty,
                        goddess = string.Empty,
                        badge = 0,
                    };
                    PlayerData.heroesTeam.Add(team);
                }
                PlayerData.Set_heroesTeam();
            }
            //if(PlayerData.)
        }
        OpenTime = DateTime.Now;


        //setupUnlockNum();
        //setupDecorations();
        //setupEmployers();
        //setupGoods();
        //setupNpcs();
        //setupRelics();
        //setupJobs();
        //setupAchievement();
        //checkCareerUnlocked();
        //if(SDDataManager.Instance.SettingData.)

    }
    public int getTempleByType(Job job, AttributeData templeType)
    {
        return getTempleByJob(job).AllAttributeData[(int)templeType];
    }
    public RoleAttributeList getTempleByJob(Job job)
    {
        RoleAttributeList RL = new RoleAttributeList();
        if (job == Job.Fighter)
        {
            RL_ChangeByList(RL, PlayerData.temple_fighter);
            return RL;
        }
        else if (job == Job.Ranger)
        {
            RL_ChangeByList(RL, PlayerData.temple_ranger);
            return RL;
        }
        else if (job == Job.Priest)
        {
            RL_ChangeByList(RL, PlayerData.temple_priest);
            return RL;
        }
        else if (job == Job.Caster)
        {
            RL_ChangeByList(RL, PlayerData.temple_caster);
            return RL;
        }
        return RL;
    }
    void RL_ChangeByList(RoleAttributeList RL, List<int> change, bool IsForAD = true)
    {
        if (IsForAD)
        {
            for (int i = 0; i < (int)AttributeData.End; i++)
            {
                if (i < change.Count)
                {
                    RL.AllAttributeData[i] += change[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < (int)StateTag.End; i++)
            {
                if (i < change.Count)
                {
                    RL.AllResistData[i] += change[i];
                }
            }
        }
    }
    public string getHeroIdByHashcode(int Hashcode)
    {
        foreach (GDEHeroData item in PlayerData.herosOwned)
        {
            if (item.hashCode == Hashcode)
            {
                return item.id;
            }
        }
        return string.Empty;
    }
    public int getHeroOriginalBattleForceByHashCode(int Hashcode)
    {
        GDEHeroData hero = getHeroByHashcode(Hashcode);
        //
        ROHeroData heroData = getHeroOriginalDataById(hero.id);
        RoleAttributeList ral = heroData.ExportRAL;
        //
        return ral.BattleForce;
    }
    public GDEHeroData GetHeroOwnedByHashcode(int hashCode)
    {
        foreach (GDEHeroData h in PlayerData.herosOwned)
        {
            if (h.hashCode == hashCode)
            {
                return h;
            }
        }
        return null;
    }
    public ROHeroData getHeroDataByID(string id, int starNumUpGradeTimes)
    {
        HeroInfo info = getHeroInfoById(id);
        ROHeroData dal = new ROHeroData();
        dal.Info = info;
        dal.starNumUpGradeTimes = starNumUpGradeTimes;
        dal.CRIDmg = 125 + 25 * dal.starNum;
        dal.DmgReduction = 0;
        dal.DmgReflection = 0;
        dal.GoldRate = dal.RewardRate = 0;
        dal.BarChartRegendPerTurn = RoleBarChart.zero;
        return dal;
    }
    public ROHeroData getHeroOriginalDataById(string id)
    {
        HeroInfo info = getHeroInfoById(id);
        ROHeroData dal = new ROHeroData();
        dal.Info = info;
        dal.RALRate = RoleAttributeList.zero;
        return dal;
    }
    public int getHeroStatus(int hashcode)
    {
        int status = 0;
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == hashcode)
            {
                status = hero.status;
                break;
            }
        }
        return status;
    }
    public void setHeroStatus(int hashcode, int aimStatus)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == hashcode)
            {
                hero.status = aimStatus;
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    public int getHeroPosInTeamByHashcode(int hashcode)
    {
        int val = 0;
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == hashcode)
            {
                val = hero.teamPos;
                break;
            }
        }
        return val;
    }
    public int getHeroStarNumByHashcode(int hashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == hashcode)
            {
                string id = hero.id;
                int level = getHeroLevelById(id);
                int starupgrade = hero.starNumUpgradeTimes;
                return level + starupgrade;
            }
        }
        return 0;
    }
    public void consumeHero(int hashcode)
    {
        foreach (GDEHeroData h in PlayerData.herosOwned)
        {
            if (!h.locked && h.hashCode == hashcode)
            {
                consume_Item(h.id, out int left);
                PlayerData.herosOwned.Remove(h);
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    public int addHero(string id)
    {
        Instance.heroNum++;
        //
        add_Item(id);
        //
        GDEHeroData hero = new GDEHeroData(GDEItemKeys.Hero_BasicHero)
        {
            id = id
,
            hashCode = Instance.heroNum
,
            status = 0
,
            starNumUpgradeTimes = 1
,
            exp = 0
        };

//ral
    RoleAttributeList ral = RoleAttributeList.RandomSet
        (
        new ScopeInt(-15, 15)//三项Barchart
        , new ScopeInt(-5, 5)//四项攻防
        , new ScopeInt(-1, 1)//其他
        , new ScopeInt(-10, 10)//抗性
        );
    hero.RoleAttritubeList = ral.TurnIntoGDEData;
//skill
        List<GDEASkillData> list = addStartSkillsWhenSummoning(hero.id);
        for(int i = 0; i < list.Count; i++)
        {
            hero.skillsOwned.Add(list[i]);
            hero.Set_skillsOwned();
        }

        AllStrs = hero.skillsOwned.Select(x =>
        {
            return x.Id + "___" + x.Lv;
        }).ToList();

        //直接将已解锁技能装配上
        List<string> enables = list.FindAll(x => x.Lv >= 0).Select(x => x.Id).ToList();
    for (int i = 0; i < enables.Count; i++)
    {
        if (getSkillByHeroId(enables[i], hero.id).isOmegaSkill)
        {
            hero.skillOmegaId = enables[i];
        }
        else
        {
            if(!string.IsNullOrEmpty (hero.skill0Id))
            {
                if (checkHeroEnableSkill1ById(hero.id))
                {
                    hero.skill1Id = enables[i];
                }
            }
            else
            {
                hero.skill0Id = enables[i];
            }
        }
    }

//animImg
    int level = getHeroLevelById(id);

    hero.AnimData = new GDEAnimData(GDEItemKeys.Anim_EmptyAnim)
{
isRare = true,
body = string.Empty,
eyes = string.Empty,
faceother = string.Empty,
hair = string.Empty,
handR = string.Empty,
head = string.Empty,
hips = string.Empty,
L_hand_a = string.Empty,
L_hand_b = string.Empty,
L_hand_c = string.Empty,
L_jiao = string.Empty,
L_leg_a = string.Empty,
L_leg_b = string.Empty,
liuhai = string.Empty,
R_leg_a = string.Empty,
R_leg_b = string.Empty,
};

    if (level < 2)
{
hero.AnimData.isRare = false;
int skeleton = getHeroSkeletonById(id);
hero.AnimData.skeletonIndex = skeleton;
hero.AnimData.body = getRandomImgAddressForAnim(nameof(hero.AnimData.body), skeleton);
hero.AnimData.eyes = getRandomImgAddressForAnim(nameof(hero.AnimData.eyes), skeleton);
hero.AnimData.faceother = getRandomImgAddressForAnim(nameof(hero.AnimData.faceother), skeleton);
hero.AnimData.hair = getRandomImgAddressForAnim(nameof(hero.AnimData.hair) + 1, skeleton);
hero.AnimData.handR = getRandomImgAddressForAnim(nameof(hero.AnimData.handR), skeleton);
hero.AnimData.head = getRandomImgAddressForAnim(nameof(hero.AnimData.head), skeleton);
hero.AnimData.hips = getRandomImgAddressForAnim(nameof(hero.AnimData.hips), skeleton);
hero.AnimData.L_hand_a = getRandomImgAddressForAnim(nameof(hero.AnimData.L_hand_a), skeleton);
hero.AnimData.L_hand_b = getRandomImgAddressForAnim(nameof(hero.AnimData.L_hand_b), skeleton);
hero.AnimData.L_hand_c = getRandomImgAddressForAnim(nameof(hero.AnimData.L_hand_c), skeleton);
hero.AnimData.L_jiao = getRandomImgAddressForAnim(nameof(hero.AnimData.L_jiao), skeleton);
hero.AnimData.L_leg_a = getRandomImgAddressForAnim(nameof(hero.AnimData.L_leg_a), skeleton);
hero.AnimData.L_leg_b = getRandomImgAddressForAnim(nameof(hero.AnimData.L_leg_b), skeleton);
hero.AnimData.liuhai = getRandomImgAddressForAnim(nameof(hero.AnimData.liuhai), skeleton);
hero.AnimData.R_leg_a = getRandomImgAddressForAnim(nameof(hero.AnimData.R_leg_a), skeleton);
hero.AnimData.R_leg_b = getRandomImgAddressForAnim(nameof(hero.AnimData.R_leg_b), skeleton);
}

        Instance.PlayerData.herosOwned.Add(hero);
        Instance.PlayerData.Set_herosOwned();



        return hero.hashCode;
    }
    public void addHeroByConsumeHero(string costId)
    {
        HeroInfo costHero = getHeroInfoById(costId);
        if (costHero.LEVEL > 0) return;
        List<HeroInfo> all = getHeroInfoList;
        all = all.FindAll(x => x.LEVEL > 0 && x.Race.Race == costHero.Race.Race);
        //HeroInfo target = all[UnityEngine.Random.Range(0, all.Count)];
        float[] ra = new float[] { 0.6f, 0.3f, 0.1f };
        int le = RandomIntger.Choose(ra)+1;
        all = all.FindAll(x => x.LEVEL == le);
        HeroInfo target = all[UnityEngine.Random.Range(0, all.Count)];
        //return target.ID;
        int hc = addHero(target.ID);
        RoleAttributeList ral = RoleAttributeList.GDEToRAL
            (getHeroByHashcode(hc).RoleAttritubeList);


    }
    public bool checkHeroEnableSkill1ByHashcode(int hashcode)
    {
        GDEHeroData hero = getHeroByHashcode(hashcode);
        return checkHeroEnableSkill1ById(hero.id);
    }
    public bool checkHeroEnableSkill1ById(string id)
    {
        int qual = getHeroLevelById(id);
        if (qual < 2) return false;
        else return true;
    }
    #region 根据ID+KEY来产生随机数,一样的种子会出现必然一致的结果
    public static int Rand(int a, int b, int id, int key)
    {
        UnityEngine.Random.State originalSeed = UnityEngine.Random.state;
        UnityEngine.Random.InitState(id + key);
        int fin = UnityEngine.Random.Range(a, b);
        UnityEngine.Random.state = originalSeed;
        return fin;
    }
    //JACK:根据ID+KEY来产生随机数,一样的种子会出现必然一致的结果
    //key用来对ID人物的不同属性区分
    //并且防止以后加入新的属性,其他属性会重新随机造成的问题,特别是名字,如果再次随机,会造成人物名字全部不一致
    //所以名字需要跟随ID非常固定
    static public float Rand(float a, float b, int id, int key)
    {
        UnityEngine.Random.State originalSeed = UnityEngine.Random.state;
        UnityEngine.Random.InitState(id + key);
        float fin = UnityEngine.Random.Range(a, b);
        UnityEngine.Random.state = originalSeed;
        return fin;
    }
    #endregion
    public void addHeroFatigue(int figure, int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                hero.Fatigue += figure;
                if (hero.Fatigue < 0) hero.Fatigue = 0;
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    public int getHeroFatigue(int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                return hero.Fatigue;
            }
        }
        return 0;
    }
    public int getHeroMaxFatigue(int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                int lv = hero.lv;
                ROHeroData h = getHeroDataByID(hero.id, hero.starNumUpgradeTimes);
                int maxF = (int)((SDConstants.fatigueBasicNum + lv * 2)
                    * Mathf.Max(h.quality * 1f / 2, 1));
                return maxF;
            }
        }
        return SDConstants.fatigueBasicNum;
    }
    public bool checkHeroFatigueTooHigh(int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                int f = hero.Fatigue;
                int maxF = getHeroMaxFatigue(heroHashcode);
                if (f >= maxF)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public float getHeroFatigueRate(int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                int f = hero.Fatigue;
                int maxF = getHeroMaxFatigue(heroHashcode);
                return Mathf.Min(1, f * 1f / maxF);
            }
        }
        return 0f;
    }
    public void setHeroFatigue(int figure, int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                hero.Fatigue = figure;
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    public IEnumerable<GDEHeroData> FindAllHerosById(string id, bool onlyUnlocked = true)
    {
        return PlayerData.herosOwned.FindAll(
            x => x.id == id && (onlyUnlocked ? true : !x.locked)).AsEnumerable();
    }
    public bool checkHeroCanImprove(int hashcode, SDConstants.MaterialType mtype)
    {
        GDEHeroData data = getHeroByHashcode(hashcode);
        ROHeroData ro = getHeroOriginalDataById(data.id);
        if (mtype == SDConstants.MaterialType.exp)
        {
            int lv = getLevelByExp(data.exp);
            int limitLv = heroMaxLvByStar(data.starNumUpgradeTimes + ro.starNum);
            return lv < limitLv;
        }
        else if(mtype == SDConstants.MaterialType.star)
        {
            return data.starNumUpgradeTimes + ro.starNum < SDConstants.UnitMAxStarNum;
        }
        else if(mtype == SDConstants.MaterialType.skill)
        {
            List<GDEASkillData> all = data.skillsOwned;
            return all.FindAll(a => a.Lv < SDConstants.SkillMaxGrade).Count > 0;
        }
        else { return false; }
    }
    public List<HeroInfo> getHeroInfoList
    {
        get
        {
            List<HeroInfo> results = new List<HeroInfo>();
            HeroInfo[] all = Resources.LoadAll<HeroInfo>("ScriptableObjects/heroes");
            for (int i = 0; i < all.Length; i++)
            {
                results.Add(all[i]);
            }
            return results;
        }
    }
    public HeroInfo getHeroInfoById(string id)
    {
        HeroInfo[] all = Resources.LoadAll<HeroInfo>("ScriptableObjects/heroes");
        foreach (HeroInfo info in all)
        {
            if (info.ID == id) return info;
        }
        return null;
    }
    public bool CheckHaveHeroById(string id)
    {
        return PlayerData.herosOwned.Exists(x => x.id == id);
    }
    public Job getHeroCareerById(string id)
    {
        List<HeroInfo> list = getHeroInfoList;
        foreach (HeroInfo info in list)
        {
            if (info.ID == id) return info.Career.Career;
        }
        return Job.End;
    }
    public Race getHeroRaceById(string id)
    {
        List<HeroInfo> list = getHeroInfoList;
        foreach (HeroInfo info in list)
        {
            if (info.ID == id) return info.Race.Race;
        }
        return Race.End;
    }
    public CharacterSex getHeroGenderById(string id)
    {
        List<HeroInfo> list = getHeroInfoList;
        foreach (HeroInfo info in list)
        {
            if (info.ID == id) return info.Sex;
        }
        return CharacterSex.Unknown;
    }
    public int getHeroLevelById(string id)
    {
        //List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(id));
        List<HeroInfo> list = getHeroInfoList;
        foreach (HeroInfo info in list)
        {
            if (info.ID == id) return info.LEVEL;
        }
        return 0;
    }
    public int getHeroSkeletonById(string id)
    {
        List<HeroInfo> list = getHeroInfoList;
        foreach (HeroInfo info in list)
        {
            if (info.ID == id) return info.Skeleton;
        }
        return 0;
    }
    public GDEHeroData getHeroByHashcode(int hashcode)
    {
        return GetHeroOwnedByHashcode(hashcode);
    }
    public RoleAttributeList RALByDictionary(Dictionary<string, string> s)
    {
        RoleAttributeList RAL = new RoleAttributeList();
        RAL.Hp = getInteger(s["hp"]);
        RAL.Mp = getInteger(s["mp"]);
        RAL.Tp = getInteger(s["tp"]);
        RAL.AT = getInteger(s["at"]);
        RAL.AD = getInteger(s["ad"]);
        RAL.MT = getInteger(s["mt"]);
        RAL.MD = getInteger(s["md"]);
        RAL.Speed = getInteger(s["speed"]);
        RAL.Taunt = getInteger(s["taunt"]);
        RAL.Accur = getInteger(s["accur"]);
        RAL.Evo = getInteger(s["evo"]);
        RAL.Crit = getInteger(s["crit"]);
        RAL.Expect = getInteger(s["expect"]);

        RAL.Bleed_Def = getInteger(s["bleed_def"]);
        RAL.Mind_Def = getInteger(s["mind_def"]);
        RAL.Fire_Def = getInteger(s["fire_def"]);
        RAL.Frost_Def = getInteger(s["frost_def"]);
        RAL.Corrosion_Def = getInteger(s["corrosion_def"]);
        RAL.Hush_Def = getInteger(s["hush_def"]);
        RAL.Dizzy_Def = getInteger(s["dizzy_def"]);
        RAL.Confuse_Def = getInteger(s["confuse_def"]);
        return RAL;
    }
    public int getRoleRAMaxNumPerLv(AttributeData tag, int lv)
    {
        int up = lv / 10 + 1;
        int basicRal = SDConstants.RoleAttritubeMaxNum;
        if (tag == AttributeData.Hp) return (int)(basicRal * 5 * up);
        else if (tag == AttributeData.Mp) return (int)(basicRal * 3.5f * up);
        else if (tag == AttributeData.Tp) return (int)(basicRal * 2.5f * up);
        else if (tag == AttributeData.AD) return (int)(basicRal * 1f * up);
        else if (tag == AttributeData.AT) return (int)(basicRal * 1f * up);
        else if (tag == AttributeData.MD) return (int)(basicRal * 1f * up);
        else if (tag == AttributeData.MT) return (int)(basicRal * 1f * up);
        else if (tag == AttributeData.Speed) return (int)(basicRal * 0.25f * up);
        else if (tag == AttributeData.Accur) return (int)(basicRal * 1f * up);
        else if (tag == AttributeData.Evo) return (int)(basicRal * 1f * up);
        else if (tag == AttributeData.Crit) return (int)(basicRal * 0.25f * up);
        else if (tag == AttributeData.Expect) return (int)(basicRal * 0.5f * up);
        else if (tag == AttributeData.Taunt) return (int)(basicRal * 1f * up);
        else return basicRal * 1 * up;
    }
    public int getRoleSRMaxNumPerLv(int lv)
    {
        int up = lv / 10 + 1;
        int basicRal = SDConstants.RoleAttritubeMaxNum;

        return (int)(basicRal * 0.5f * up);
    }
    public void dressEquipment(int heroHashcode, int itemHashcode, bool isSecondJewelry = false)
    {
        int oldEquipHashcode = 0;
        foreach (GDEHeroData hero in SDDataManager.Instance.PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                GDEEquipmentData equip
                    = SDDataManager.Instance.getEquipmentByHashcode(itemHashcode);
                equip.OwnerHashcode = heroHashcode;
                #region add equip
                int pos = SDDataManager.Instance.getEquipPosById(equip.id);
                if (pos == (int)EquipPosition.Head)
                {
                    oldEquipHashcode
                        = hero.equipHelmet != null ? hero.equipHelmet.hashcode : 0;
                    hero.equipHelmet = equip;
                }
                else if (pos == (int)EquipPosition.Breast)
                {
                    oldEquipHashcode
                        = hero.equipBreastplate != null ? hero.equipBreastplate.hashcode : 0;
                    hero.equipBreastplate = equip;
                }
                else if (pos == (int)EquipPosition.Arm)
                {
                    oldEquipHashcode
                        = hero.equipGardebras != null ? hero.equipGardebras.hashcode : 0;
                    hero.equipGardebras = equip;
                }
                else if (pos == (int)EquipPosition.Leg)
                {
                    oldEquipHashcode
                        = hero.equipLegging != null ? hero.equipLegging.hashcode : 0;
                    hero.equipLegging = equip;
                }
                else if (pos == (int)EquipPosition.Finger)
                {
                    if (!isSecondJewelry)
                    {
                        oldEquipHashcode
                            = hero.jewelry0 != null ? hero.jewelry0.hashcode : 0;
                        hero.jewelry0 = equip;
                    }
                    else
                    {
                        oldEquipHashcode
                            = hero.jewelry1 != null ? hero.jewelry1.hashcode : 0;
                        hero.jewelry1 = equip;
                    }
                }
                else if (pos == (int)EquipPosition.Hand)
                {
                    oldEquipHashcode
                        = hero.equipWeapon != null ? hero.equipWeapon.hashcode : 0;
                    hero.equipWeapon = equip;
                }
                #endregion
                break;
            }
        }
        foreach (GDEEquipmentData e in SDDataManager.Instance.PlayerData.equipsOwned)
        {
            if (e.hashcode == itemHashcode)
            {
                e.OwnerHashcode = heroHashcode;
            }
            if (e.hashcode == oldEquipHashcode && oldEquipHashcode > 0)
            {
                e.OwnerHashcode = 0;
            }
        }
        SDDataManager.Instance.PlayerData.Set_equipsOwned();
    }
    public void disrobeEquipment(int heroHashcode, EquipPosition pos, bool isSecondJPos = false)
    {
        int equipHashcode = 0;
        foreach (GDEHeroData hero in SDDataManager.Instance.PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                if (pos == EquipPosition.Head)
                {
                    equipHashcode = hero.equipHelmet.hashcode;
                    hero.equipHelmet = Instance.equipEmpty();
                }
                else if (pos == EquipPosition.Breast)
                {
                    equipHashcode = hero.equipBreastplate.hashcode;
                    hero.equipBreastplate = Instance.equipEmpty();
                }
                else if (pos == EquipPosition.Arm)
                {
                    equipHashcode = hero.equipGardebras.hashcode;
                    hero.equipGardebras = Instance.equipEmpty();
                }
                else if (pos == EquipPosition.Leg)
                {
                    equipHashcode = hero.equipLegging.hashcode;
                    hero.equipLegging = Instance.equipEmpty();
                }
                else if (pos == EquipPosition.Finger)
                {
                    if (!isSecondJPos)
                    {
                        equipHashcode = hero.jewelry0.hashcode;
                        hero.jewelry0 = Instance.equipEmpty();
                    }
                    else
                    {
                        equipHashcode = hero.jewelry1.hashcode;
                        hero.jewelry1 = Instance.equipEmpty();
                    }
                }
                else if (pos == EquipPosition.Hand)
                {
                    equipHashcode = hero.equipWeapon.hashcode;
                    hero.equipWeapon = Instance.equipEmpty();
                }
                break;
            }
        }
        foreach (GDEEquipmentData e in SDDataManager.Instance.PlayerData.equipsOwned)
        {
            if (e.hashcode == equipHashcode)
            {
                e.OwnerHashcode = 0;
                break;
            }
        }
        SDDataManager.Instance.PlayerData.Set_herosOwned();
        GDEEquipmentData _e
            = SDDataManager.Instance.getHeroEquipmentByPos(heroHashcode, pos, isSecondJPos);
    }
    #region Level-&&-Exp-=>-Caculate
    public int getLevelByExp(int exp)
    {
        int lv = 0;
        int V = 0;
        while (lv < SDConstants.MaxIncreasingExpLevel)
        {
            V += ExpBulkPerLevel(lv);
            if (V <= exp)
            {
                lv++;
            }
            else
            {
                break;
            }
        }
        return lv;
    }
    public int ExpBulkPerLevel(int lv)
    {
        if (lv < 50) return lv * SDConstants.MinExpPerLevel;
        else return 50 * SDConstants.MinExpPerLevel;
    }
    public int getMinExpReachLevel(int lv)
    {
        int V = 0;
        for (int i = 1; i < lv; i++)
        {
            V += ExpBulkPerLevel(i);
        }
        return V;
    }
    public float getExpRateByExp(int exp)
    {
        int currentLv = getLevelByExp(exp);
        int expOld = getMinExpReachLevel(currentLv);
        int expLength = ExpBulkPerLevel(currentLv);
        return (exp - expOld) * 1f / expLength;
    }
    #endregion
    public void addExpToHeroByHashcode(int hashcode, int exp = 1)
    {
        foreach (GDEHeroData h in PlayerData.herosOwned)
        {
            if (h.hashCode == hashcode)
            {
                int starNum = getHeroLevelById(h.id) + h.starNumUpgradeTimes;
                h.exp = HeroOverflowExp(h.exp + exp, starNum);
                PlayerData.Set_herosOwned();
                break;
            }
        }

    }
    public int heroMaxLvByStar(int star)
    {
        int limitedLv = 10;
        if (star == 0) limitedLv = 10;
        else if (star == 1) limitedLv = 20;
        else if (star == 2) limitedLv = 30;
        else if (star == 3) limitedLv = 50;
        else if (star == 4) limitedLv = 70;
        else if (star == 5) limitedLv = 100;
        return limitedLv;
    }
    public bool checkHeroExpIfOverflow(int currentExp, int star)
    {
        int limitedLv = heroMaxLvByStar(star);
        int limitedExp = getMinExpReachLevel(limitedLv);
        if (currentExp >= limitedExp) return true;
        return false;
    }
    public int HeroOverflowExp(int oldExp, int star)
    {
        int limitedLv = heroMaxLvByStar(star);
        int limitedExp = getMinExpReachLevel(limitedLv);
        if (oldExp >= limitedExp) return limitedExp;
        return oldExp;
    }
    /// <summary>
    /// likability
    /// </summary>
    /// <param name="hashcode"></param>
    /// <param name="likability"></param>
    public void addLikabilityToHeroByHashcode(int hashcode, int likability = 1)
    {
        foreach (GDEHeroData h in PlayerData.herosOwned)
        {
            if (h.hashCode == hashcode)
            {
                h.likability += likability; break;
            }
        }
    }
    public int getLikeByLikability(int L, out float RateToNext)
    {
        if (L >= SDConstants.MinHeartVolume * 8.5f)
        {
            RateToNext = 0; return 3;
        }
        if (L >= SDConstants.MinHeartVolume * 3.5f)
        {
            RateToNext = (L - SDConstants.MinHeartVolume * 3.5f) * 1f
                / (SDConstants.MinHeartVolume * 5f);
            return 2;
        }
        if (L >= SDConstants.MinHeartVolume)
        {
            RateToNext = (L - SDConstants.MinHeartVolume) * 1f
                / (SDConstants.MinHeartVolume * 2.5f);
            return 1;
        }
        RateToNext = L * 1f / SDConstants.MinHeartVolume;
        return 0;
    }
    public string getCareerStr(Job career, int raceIndex = 0
        , SDConstants.CharacterType type = SDConstants.CharacterType.Hero)
    {
        string s = "";
        int careerIndex = (int)career;
        if (type == SDConstants.CharacterType.Hero)
        {
            if (careerIndex == 0) s = SDGameManager.T("fighter");
            else if (careerIndex == 1) s = SDGameManager.T("ranger");
            else if (careerIndex == 2) s = SDGameManager.T("priest");
            else if (careerIndex == 3) s = SDGameManager.T("caster");
        }
        else if (type == SDConstants.CharacterType.Goddess)
        {

        }
        else if (type == SDConstants.CharacterType.Enemy)
        {
            if (raceIndex == 0)
            {

            }
        }
        return s;
    }
    public string getRaceStr(int raceIndex, SDConstants.CharacterType type)
    {
        string s = "";
        if (type == SDConstants.CharacterType.Hero)
        {
            if (raceIndex == 0) s = SDGameManager.T("human");
            else if (raceIndex == 1) s = SDGameManager.T("elf");
            else if (raceIndex == 2) s = SDGameManager.T("dragonborn");
        }
        else if (type == SDConstants.CharacterType.Goddess)
        {

        }
        else if (type == SDConstants.CharacterType.Enemy)
        {
            if (raceIndex == 0) s = SDGameManager.T("elemental");
            else if (raceIndex == 1) s = SDGameManager.T("goblin");
            else if (raceIndex == 2) s = SDGameManager.T("orc");
            else if (raceIndex == 3) s = SDGameManager.T("beast");
        }
        return s;
    }
    public bool getHeroIfLocked(int hashcode)
    {
        GDEHeroData h = getHeroByHashcode(hashcode);
        if (h != null)
        {
            return h.locked;
        }
        return true;
    }
    public int getHeroExpPrice(int hashcode)
    {
        GDEHeroData h = getHeroByHashcode(hashcode);
        int _exp = h.exp;
        string id = h.id;
        ROHeroData dal = getHeroDataByID(id, h.starNumUpgradeTimes);
        int baseExp = (int)(25 * (1 + 0.2f * dal.quality + 0.2f * dal.starNum));
        return baseExp + _exp;
    }
    #endregion
    #region Hero_Anim_Infor
    public string getRandomImgAddressForAnim(string parent, int skeletonIndex = 0)
    {
        //Sprite[] all_body = Resources.LoadAll<Sprite>("Sprites/AnimImage/" + parent);
        //return all_body[UnityEngine.Random.Range(0, all_body.Length-1)].name;
        HeroAnimImgList hail = null;
        HeroAnimImgList[] allHAIL = Resources.LoadAll<HeroAnimImgList>("ScriptableObjects");
        for (int i = 0; i < allHAIL.Length; i++)
        {
            if (allHAIL[i].Skeleton == skeletonIndex)
            {
                hail = allHAIL[i];
                break;
            }
        }
        if (hail == null) return string.Empty;

        HeroAnimImgList.SlotRegionPairList list = null;
        foreach (var L in hail.AllEnableList)
        {
            if (L.slot == parent)
            {
                list = L; break;
            }
        }
        if (list == null) return string.Empty;
        //
        int count = list.AllRegionList.Count;
        int selectedIndex = UnityEngine.Random.Range(0, count);
        return list.AllRegionList[selectedIndex].Region;
    }
    public Sprite getSpriteFromAtlas(string atlasAddress, string spriteName)
    {
        //Debug.Log("ATLAS==--=="+atlasAddress + "===---===" + spriteName);
        SpriteAtlas atlas = Resources.Load<SpriteAtlas>("Sprites/AnimImage/" + atlasAddress);
        return atlas.GetSprite(spriteName);
    }
    public HeroAnimImgList.SlotRegionPair GetPairBySlotAndRegion(int skeletonIndex, string slot, string region)
    {
        HeroAnimImgList hail = null;
        HeroAnimImgList[] allHAIL = Resources.LoadAll<HeroAnimImgList>("ScriptableObjects/heroAnimImgList");
        for (int i = 0; i < allHAIL.Length; i++)
        {
            if (allHAIL[i].Skeleton == skeletonIndex)
            {
                hail = allHAIL[i]; break;
            }
        }
        if (hail == null) return null;
        HeroAnimImgList.SlotRegionPairList list = hail.AllEnableList.Find(x => x.slot == slot);
        if (list == null) return null;
        return list.AllRegionList.Find(x => x.region == region);
    }
    #endregion
    #region HeroTeamInfor
    public GDEunitTeamData getHeroTeamByTeamId(string id)
    {
        foreach (GDEunitTeamData t in PlayerData.heroesTeam)
        {
            if (t.id == id) return t;
        }
        return null;
    }
    public bool checkHeroOwned(string heroId)
    {
        bool flag = false;
        foreach (GDEHeroData item in PlayerData.herosOwned)
        {
            if (item.id == heroId)
            {
                flag = true; break;
            }
        }
        return flag;
    }
    public void setHeroTeam(string teamId, int index, int hashcode)
    {
        GDEunitTeamData Team = getHeroTeamByTeamId(teamId);
        if (Team == null)
        {
            Team = new GDEunitTeamData(GDEItemKeys.unitTeam_emptyHeroTeam);
            Team.id = teamId;
            PlayerData.heroesTeam.Add(Team);
            PlayerData.Set_heroesTeam();
        }

        foreach (GDEHeroData H in PlayerData.herosOwned)
        {
            if (H.teamIdBelongTo == teamId && H.TeamOrder == index)
            {
                H.teamIdBelongTo = string.Empty;
                H.TeamOrder = 0;
                H.status = 0;
                PlayerData.Set_herosOwned();
                break;
            }
        }

        foreach (GDEHeroData H in PlayerData.herosOwned)
        {

            if (H.hashCode == hashcode)
            {
                H.teamIdBelongTo = teamId; H.TeamOrder = index;
                H.status = 1;
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    public void setHeroTeamPos(int hashcode, int newPos)
    {
        int P = newPos % SDConstants.MaxSelfNum;
        foreach (GDEHeroData H in PlayerData.herosOwned)
        {
            if (H.hashCode == hashcode)
            {
                H.teamPos = P;
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    public List<GDEunitTeamData> getHeroGroup() { return PlayerData.heroesTeam; }
    public void removeFromTeam(int hashcode)
    {
        foreach (GDEHeroData item in PlayerData.herosOwned)
        {
            if (item.hashCode == hashcode)
            {
                if (item.status == 1)//角色在战斗组
                {
                    removeHeroFromBattleTeam(hashcode);
                }
                else if (item.status == 2)
                {

                }
                else if (item.status == 3)
                {

                }
                else if (item.status == 4)
                {

                }
                break;
            }
        }
    }
    public void removeHeroFromBattleTeam(int hashcode)
    {

        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == hashcode)
            {
                hero.teamIdBelongTo = string.Empty;
                hero.status = 0; break;
            }
        }
    }
    public List<int> getHerosHashcodeFromTeam(string id)
    {
        List<int> list = new List<int>();
        IEnumerable<GDEHeroData> all
            = PlayerData.herosOwned.FindAll(x => x.teamIdBelongTo == id).AsEnumerable();
        foreach (var a in all)
        {
            list.Add(a.hashCode);
        }
        return list;
    }
    public List<GDEHeroData> getHerosFromTeam(string id)
    {
        List<GDEHeroData> list = new List<GDEHeroData>();
        IEnumerable<GDEHeroData> all
            = PlayerData.herosOwned.FindAll(x => x.teamIdBelongTo == id).AsEnumerable();
        foreach (var a in all)
        {
            list.Add(a);
        }
        return list;
    }
    public GDEHeroData getHeroFromTeamByOrder(string teamId, int order)
    {
        List<GDEHeroData> all = getHerosFromTeam(teamId);
        return all.Find(x => x.TeamOrder == order);
    }


    public void setTeamName(string teamId, string new_name)
    {
        foreach (GDEunitTeamData t in PlayerData.heroesTeam)
        {
            if (t.id == teamId)
            {
                t.teamName = new_name;
                PlayerData.Set_heroesTeam();
                break;
            }
        }
    }
    public void setTeamGoddess(string teamId, string goddessId)
    {
        foreach (GDEunitTeamData t in PlayerData.heroesTeam)
        {
            if (t.id == teamId)
            {
                t.goddess = goddessId;
                PlayerData.Set_heroesTeam();
                break;
            }
        }
    }
    #endregion
    #region ConsumableInfor
    public int addConsumable(string id, int num = 1)
    {
        add_Item(id, num);
        foreach (GDEItemData M in PlayerData.consumables)
        {
            if (M.id == id)
            {
                M.num += num;
                PlayerData.Set_consumables();
                return M.num;
            }
        }
        consumableItem[] allPs = Resources.LoadAll<consumableItem>
            ("ScriptableObjects/Items/Consumables");
        if (allPs.Select(x => x.ID == id).ToList().Count <= 0)
        {
            Debug.Log("不存在该道具"); return 0;
        }

        GDEItemData m = new GDEItemData(GDEItemKeys.Item_MaterialEmpty);
        m.id = id;
        m.num = num;
        PlayerData.consumables.Add(m);
        PlayerData.Set_consumables();
        return m.num;
    }
    public int getConsumableNum(string id)
    {
        GDEItemData item = PlayerData.consumables.Find(x => x.id == id);
        if (item != null)
        {
            return item.num;
        }
        return 0;
    }
    public List<GDEItemData> getConsumablesOwned
    {
        get { return PlayerData.consumables; }
    }
    public bool consumeConsumable(string id, out int residue, int num = 1)
    {
        foreach (GDEItemData m in PlayerData.consumables)
        {
            if (m.id == id)
            {
                if (m.num < num)
                {
                    residue = m.num;
                    return false;
                }
                else
                {
                    m.num -= num;
                    consume_Item(id, out int left, num);
                    if (m.num <= 0)
                    {
                        PlayerData.consumables.Remove(m);
                    }
                    PlayerData.Set_consumables();
                    residue = m.num;
                    return true;
                }
            }
        }
        residue = 0;
        return false;
    }
    public List<consumableItem> AllConsumableList
    {
        get 
        {
            consumableItem[] all = Resources.LoadAll<consumableItem>
                ("ScriptableObjects/Items/Consumables");
            return all.ToList();
        }
    }
    public consumableItem getConsumableItemById(string id)
    {
        consumableItem[] all = Resources.LoadAll<consumableItem>
            ("ScriptableObjects/Items/Consumables");
        foreach (consumableItem item in all)
        {
            if (item.ID == id) return item;
        }
        return null;
    }
    /// <summary>
    /// getConsumableById替补
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public consumableItem getConsumableById(string id)
    {
        return getConsumableItemById(id);
    }
    public GDEItemData getConsumeableDataById(string id)
    {
        foreach (GDEItemData data in getConsumablesOwned)
        {
            if (data.id == id) return data;
        }
        return null;
    }
    public string getConsumableStrById(string id)
    {
        consumableItem item = getConsumableItemById(id);
        if (item != null) return item.SpecialStr;
        return string.Empty;
    }

    public bool checkIfHaveOpKey(SDConstants.MaterialType type, out string keyId)
    {
        consumableItem key = AllConsumableList.Find
            (x =>
            {
                string s = x.SpecialStr.ToLower();
                string t = type.ToString().ToLower();
                return s.Contains(t) && x.MaterialType == SDConstants.MaterialType.key;
            });
        if (key)
        {
            string ID = key.ID;
            keyId = ID;
            if (PlayerData.consumables.Exists(x => x.id == ID)) return true;
        }else keyId = string.Empty;
        return false;
    }
    #endregion
    #region PropInfor
    public List<GDEItemData> getPropsOwned
    {
        get
        {
            return getConsumablesOwned.FindAll(x =>
            {
                consumableItem item = getConsumableItemById(x.id);
                if (item == null) return false;
                return item.isProp;
            });
        }
    }
    public bool checkIfPropIsTaken(string id)
    {
        foreach (GDEItemData p in PlayerData.propsTeam)
        {
            if (p.id == id) return true;
        }
        return false;
    }
    public int propTakenVolume(string id)
    {
        consumableItem p = getConsumableById(id);
        if (!p.isProp) return 0;
        int level = p.LEVEL;
        if (level == 0) return 10;
        else if (level == 1) return 5;
        else if (level == 2) return 3;
        else if (level == 3) return 3;
        else if (level == 4) return 1;
        else if (level == 5) return 1;
        return 1;
    }
    public void unlockNewPropTeamSlot(int unlockNum = 1)
    {
        if (PlayerData.propsTeam.Count < SDConstants.BagMaxVolume)
        {
            for (int i = 0; i < unlockNum; i++)
            {
                GDEItemData D = new GDEItemData(GDEItemKeys.Item_MaterialEmpty)
                {
                    id = string.Empty,
                    num = 0,
                };
                D.index = PlayerData.propsTeam.Count;
                PlayerData.propsTeam.Add(D);
                PlayerData.Set_propsTeam();
            }
        }
    }
    #endregion
    #region MaterialInfor
    public List<consumableItem> allRaws
    {
        get
        {
            consumableItem[] all = Resources.LoadAll<consumableItem>
                ("ScriptableObjects/Items/Consumables");
            List<consumableItem> results = new List<consumableItem>();
            for(int i = 0; i < all.Length; i++)
            {
                if(all[i].MaterialType == SDConstants.MaterialType.raw)
                {
                    results.Add(all[i]);
                }
            }
            return results;
        }
    }
    public List<GDEItemData> getMaterialsOwned
    {
        get {
            return getConsumablesOwned.FindAll(x =>
                {
                    consumableItem item = getConsumableItemById(x.id);
                    if (item == null) return false;
                    return !item.isProp;
                });
        }
    }
    public string getMaterialNameById(string id)
    {
        List<consumableItem> list = AllConsumableList;
        foreach(consumableItem item in list)
        {
            if (item.ID == id) return item.NAME;
        }
        return string.Empty;
    }
    public SDConstants.MaterialType getMaterialTypeById(string id)
    {
        consumableItem item = getConsumableById(id);
        if (item)
        {
            return item.MaterialType;
        }
        return SDConstants.MaterialType.end;
    }
    public int getMaterialLevelById(string id)
    {
        List<consumableItem> list = AllConsumableList;
        foreach (var m in list)
        {
            if (m.ID == id)
            {
                return m.LEVEL;
            }
        }
        return 0;
    }
    public int getMaterialWeightById(string id)
    {
        if(AllConsumableList.Exists(x=>x.ID == id))
        {
            consumableItem item = getConsumableItemById(id);
            return 5 - item.LEVEL;
        }
        return 1;
    }


    public bool checkConsumableIfProp(string id)
    {
        consumableItem item = getConsumableById(id);
        if (item)
        {
            return item.isProp;
        }
        return false;
    }
    #region MaterialFunction
    public int getFigureFromMaterial(consumableItem item)
    {
        if (item.MaterialType == SDConstants.MaterialType.exp
            || item.MaterialType == SDConstants.MaterialType.likability
            || item.MaterialType == SDConstants.MaterialType.equip_exp)
        {
            return getInteger(item.SpecialStr);
        }
        return 0;
    }
    public int getFigureFromMaterial(string itemId)
    {
        consumableItem item = getConsumableById(itemId);
        return getFigureFromMaterial(item);
    }
    public string getMaterialSpecialStr(string id)
    {
        consumableItem item = getConsumableById(id);
        if (item) return item.SpecialStr;
        return string.Empty;
    }
    #endregion
    #endregion

    #region 基础货币信息
    #region Gold_Infor
    public int AddCoin(int val)
    {
        int number = PlayerData.coin;
        PlayerData.coin = number + val;
        return PlayerData.coin;
    }
    public bool ConsumeCoin(int val)
    {
        int number = PlayerData.coin;
        if (number >= val)
        {
            PlayerData.coin = number - val;
            return true;
        }
        else
        {
            Debug.Log("操作无法执行，金币数量不足");
            return false;
        }
    }
    public int GetCoin() { return PlayerData.coin; }

    public int getGoldPerc()
    {
        return getGoldPercOrigin();
    }
    public int getGoldPercOrigin() { return PlayerData.addGoldPerc; }
    public void addGoldPerc(int val) { PlayerData.addGoldPerc += val; }
    #endregion
    #region Damond_Infor
    public int AddDamond(int val)
    {
        int num = PlayerData.damond;
        PlayerData.damond = num + val;
        return PlayerData.damond;
    }
    public bool ConsumeDamond(int val)
    {
        int num = PlayerData.damond;
        if (num >= val)
        {
            PlayerData.damond = num-val;
            return true;
        }
        else
        {
            Debug.Log("操作无法执行，钻石数量不足");
            return false;
        }
    }
    public int GetDamond() { return PlayerData.damond; }
    #endregion
    #region JianCai_Infor
    public int AddJiancai(int val)
    {
        int num = PlayerData.JianCai;
        PlayerData.JianCai = num + val;
        return PlayerData.JianCai;
    }
    public bool ConsumeJiancai(int val)
    {
        int num = PlayerData.JianCai;
        if (num >= val)
        {
            PlayerData.JianCai = num - val;
            return true;
        }
        else
        {
            Debug.Log("操作无法执行，建筑升级材料不足");
            return false;
        }
    }
    public int getJiancai() { return PlayerData.JianCai; }
    #endregion
    #endregion
    #region LevelInfor
    public void SetNewBestLevel(int val)
    {
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            if (val > PlayerData.newBestLevel)
            {
                PlayerData.newBestLevel = val;
            }
        }
    }
    public int GetNewBestLevel()
    {
        return PlayerData.newBestLevel;
    }
    public int GetMaxPassSection()
    {
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            return PlayerData.maxPassSection;
        }
        else if (SDGameManager.Instance.gameType == SDConstants.GameType.Hut)
        {

        }
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Dungeon)
        {
            return PlayerData.maxDurgeonPassLevel;
        }
        return PlayerData.maxPassSection;
    }
    public int getDimension() { return PlayerData.dimension; }
    #endregion
    #region DataListResult
    public void ResetDatas()
    {
        PlayerPrefs.DeleteAll();
        GDEDataManager.ClearSaved();
    }
    #region 现场索引后内容存储区域
    List<Dictionary<string, string>> dataListResult0;
    List<Dictionary<string, string>> dataListResult1;
    List<Dictionary<string, string>> dataListResult2;
    List<Dictionary<string, string>> dataListResult3;
    List<Dictionary<string, string>> dataListResult4;
    List<Dictionary<string, string>> dataListResult5;
    List<Dictionary<string, string>> dataListResult6;
    List<Dictionary<string, string>> dataListResult7;
    List<Dictionary<string, string>> dataListResult8;
    List<Dictionary<string, string>> dataListResult9;
    List<Dictionary<string, string>> dataListResult10;
    List<Dictionary<string, string>> dataListResult11;
    List<Dictionary<string, string>> dataListResult12;
    List<Dictionary<string, string>> dataListResult13;
    List<Dictionary<string, string>> dataListResult14;
    List<Dictionary<string, string>> dataListResult15;
    List<Dictionary<string, string>> dataListResult16;
    List<Dictionary<string, string>> dataListResult17;
    List<Dictionary<string, string>> dataListResult18;
    List<Dictionary<string, string>> dataListResult20;
    List<Dictionary<string, string>> dataListResult21;
    List<Dictionary<string, string>> dataListResult22;
    List<Dictionary<string, string>> dataListResult23;
    List<Dictionary<string, string>> dataListResult24;
    List<Dictionary<string, string>> dataListResult25;
    List<Dictionary<string, string>> dataListResult26;
    List<Dictionary<string, string>> dataListResult27;
    List<Dictionary<string, string>> dataListResult28;
    List<Dictionary<string, string>> dataListResult29;
    
    #endregion
    public List<Dictionary<string, string>> ReadFromCSV(string filename)
    {
        if (filename == "achievement")
        {
            if (dataListResult0 == null) { dataListResult0 = ReadDataFromCSV("achievement"); }
            return dataListResult0;
        } else if (filename == "equip")
        {
            if (dataListResult1 == null) { dataListResult1 = ReadDataFromCSV("equip"); };
            return dataListResult1;
        }
        else if (filename == "decoration")
        {
            if (dataListResult2 == null) { dataListResult2 = ReadDataFromCSV("decoration"); }
            return dataListResult2;
        }
        else if (filename == "enemy")
        {
            if (dataListResult3 == null) { dataListResult3 = ReadDataFromCSV("enemy"); }
            return dataListResult3;
        }
        else if (filename == "material")
        {
            if (dataListResult4 == null) { dataListResult4 = ReadDataFromCSV("material"); }
            return dataListResult4;
        }
        else if (filename == "heroLvUp")
        {
            if (dataListResult6 == null) { dataListResult6 = ReadDataFromCSV("heroLvUp"); }
            return dataListResult6;
        }
        else if (filename == "nameBefore")
        {
            if (dataListResult7 == null) { dataListResult7 = ReadDataFromCSV("nameBefore"); }
            return dataListResult7;
        }
        else if (filename == "goddess")
        {
            if (dataListResult10 == null) { dataListResult10 = ReadDataFromCSV("goddess"); }
            return dataListResult10;
        }
        else if (filename == "badge")
        {
            if (dataListResult11 == null) { dataListResult11 = ReadDataFromCSV("badge"); }
            return dataListResult11;
        }
        else if (filename == "task_time")
        {
            if (dataListResult12 == null) { dataListResult12 = ReadDataFromCSV("task_time"); }
            return dataListResult12;
        }
        else if (filename == "task_plot")
        {
            if (dataListResult13 == null) { dataListResult13 = ReadDataFromCSV("task_plot"); }
            return dataListResult13;
        }
        else
        {
            return ReadDataFromCSV(filename);
        }
    }
    public List<Dictionary<string, string>> ReadDataFromCSV(string fileName)
    {
        List<Dictionary<string, string>> xxListResult
            = new List<Dictionary<string, string>>();
        TextAsset chadata = Resources.Load("Data/" + fileName) as TextAsset;
        List<string[]> cDatas = LocalizationReader.ReadCSV(chadata.text);
        xxListResult = ROHelp.ConvertCsvListToDictWithAttritubes(cDatas);
        return xxListResult;
    }
    public int getInteger(string s)
    {
        int tmp = 0;
        if (s != null && s != "")
        {
            if (int.TryParse(s, out _)) { tmp = int.Parse(s); }
        }
        return tmp;
    }
    public IEnumerator IELoadFileAsynchronously()
    {
        dataListResult0 = ReadDataFromCSV("achievement");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        dataListResult1 = ReadDataFromCSV("equip");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        //dataListResult2 = ReadDataFromCSV("decoration");
        //yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        dataListResult3 = ReadDataFromCSV("enemy");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        dataListResult4 = ReadDataFromCSV("material");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        dataListResult5 = ReadDataFromCSV("prop");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        //dataListResult6 = ReadDataFromCSV("heroLvUp");
        //yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        dataListResult7 = ReadDataFromCSV("nameBefore");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        dataListResult8 = ReadDataFromCSV("jewelry");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        dataListResult9 = ReadDataFromCSV("weapon");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
    }
    public SDConstants.ItemType getItemTypeById(string id)
    {
        string Sign = id.ToCharArray()[0].ToString().ToUpper();
        if (Sign == "M") return SDConstants.ItemType.Consumable;
        else if (Sign == "D") return SDConstants.ItemType.Enemy;
        else if (Sign == "E") return SDConstants.ItemType.Equip;
        else if (Sign == "H") return SDConstants.ItemType.Hero;
        else if (Sign == "G") return SDConstants.ItemType.Goddess;
        else if (Sign == "R") return SDConstants.ItemType.Rune;
        else if (Sign == "N") return SDConstants.ItemType.NPC;

        return SDConstants.ItemType.End;
    }
    #endregion
    #region HeroAlarPoolInfor
    public List<HeroAltarPool> GetAllHeroAltarPoolList
    {
        get
        {
            return Resources.LoadAll<HeroAltarPool>("ScriptableObjects/pools").ToList();
        }
    }
    public HeroAltarPool GetHeroAltarPoolById(string id)
    {
        return GetAllHeroAltarPoolList.Find(x => x.ID == id);
    }
    public List<GDEHeroAltarPoolData> GetAllHeroPool()
    {
        return PlayerData.AltarPoolList;
    }
    public GDEHeroAltarPoolData GetHeroPoolDataById(string id)
    {
        return GetAllHeroPool().Find(x => x.ID == id);
    }
    public HeroInfo AltarInOnePool(float[] Possibilities, string poolId, bool MustS = false)
    {
        int L = RandomIntger.Choose(Possibilities);
        GDEHeroAltarPoolData PoolData = GetHeroPoolDataById(poolId);
        PoolData.AltarTimes++;
        PlayerData.Set_AltarPoolList();
        if(PoolData.AltarTimes>= 10)
        {
            if (PoolData.GetSNum == 0) L = 3;
        }
        HeroAltarPool Pool = GetHeroAltarPoolById(poolId);
        List<int> LEVELList = Pool.HeroList.Select(x => x.LEVEL).ToList();
        LEVELList.Sort();
        if (MustS) L = LEVELList.Max();
        if (!LEVELList.Contains(L))
        {
            if (L < LEVELList.Count)
            {
                L = LEVELList[L];
            }
            else 
            {
                L = LEVELList[UnityEngine.Random.Range(0, LEVELList.Count)];
            } 
        }
        if (L >= 3)
        {
            PoolData.GetSNum++;
            PlayerData.Set_AltarPoolList();
        }
        List<HeroInfo> list = Pool.HeroList.FindAll(x => x.LEVEL == L);
        List<HeroInfo> _list = Pool.HeroesUsingSpecialPossibility.FindAll(x => x.LEVEL == L);
        if (_list.Count > 0)
        {
            float R = UnityEngine.Random.Range(0, 1f);
            if (R < 0.5f)
            {
                return _list[UnityEngine.Random.Range(0, _list.Count)];
            }
        }
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
    #endregion
    #region Goddess_Infor
    public GDEgoddessData getGDEGoddessDataById(string goddessId)
    {
        foreach (GDEgoddessData g in PlayerData.goddessOwned)
        {
            if (g.id == goddessId)
            {
                return g;
            }
        }
        return null;
    }
    public GoddessInfo getGoddessInfoById(string id)
    {
        GoddessInfo[] All = Resources.LoadAll<GoddessInfo>("ScriptableObjects/生物列表");
        for(int i = 0; i < All.Length; i++)
        {
            if (All[i].ID == id) return All[i];
        }
        return null;
    }
    public bool addGoddess(GoddessInfo info)
    {
        foreach (GDEgoddessData g in PlayerData.goddessOwned)
        {
            if (g.id == info.ID)
            {
                g.attitube.agile = info.GoddessAtti.Agile;
                g.attitube.stamina = info.GoddessAtti.Stamina;
                g.attitube.recovery = info.GoddessAtti.Recovery;
                g.attitube.leader = info.GoddessAtti.Leader;
                g.index = info.Index;
                PlayerData.Set_goddessOwned();
                return false;
            }
        }
        //
        GDEgoddessData data = new GDEgoddessData(GDEItemKeys.goddess_baseGoddess)
        {
            id = info.ID,
            star = 0,
            volume = 0,
            exp = 0,
            rune0 = 0,
            rune1 = 0,
            rune2 = 0,
            rune3 = 0,
            index = info.Index,
            UseTeamId = new List<int>(),
        };
        data.attitube.agile = info.GoddessAtti.Agile;
        data.attitube.stamina = info.GoddessAtti.Stamina;
        data.attitube.recovery = info.GoddessAtti.Recovery;
        data.attitube.leader = info.GoddessAtti.Leader;
        PlayerData.goddessOwned.Add(data);
        PlayerData.Set_goddessOwned();
        return true;
    }
    public void addExpToGoddess(string goddessId,int exp)
    {
        foreach(GDEgoddessData goddess in PlayerData.goddessOwned)
        {
            if(goddess.id == goddessId)
            {
                goddess.exp += exp;
                PlayerData.Set_goddessOwned();
                break;
            }
        }
    }

    #region Integrity-&&-Volume-=>-Caculate
    public int getIntegrityByVolume(int volume, int quality)
    {
        int integrity = 0;
        int V = 0;
        while(integrity < SDConstants.goddessMaxIntegrity)
        {
            V += VolumeBulkPerIntegrity(integrity, quality);
            if (V < volume)
            {
                integrity++;
            }
            else
            {
                break;
            }
        }
        return integrity;
    }
    public int VolumeBulkPerIntegrity(int integrity, int quality)
    {
        int result = SDConstants.MinVolumePerIntegrity * (integrity + 1) * (5 + quality * 2);
        return result;
    }
    public int getMinVolumeReachIntegrity(int integrity,int quality)
    {
        int V = 0;
        for(int i = 0; i < integrity; i++)
        {
            V += VolumeBulkPerIntegrity(i,quality);
        }
        return V;
    }
    public float getRateAppraochIntegrity(int volume, int quality)
    {
        int integrity = getIntegrityByVolume(volume, quality);

        int vOld = 0; 
        vOld = getMinVolumeReachIntegrity(integrity, quality);
        int minV = VolumeBulkPerIntegrity(integrity, quality);
        return (volume - vOld) * 1f / minV;
    }
    #endregion

    #endregion
    #region TimeTask_Infor
    public bool haveTimeTaskByTaskId(string taskId, out GDEtimeTaskData task)
    {
        task = PlayerData.TimeTaskList.Find(x => x.taskId == taskId);
        return task != null;
    }
    public void AddTimeTask(SDConstants.timeTaskType taskType,int Hashcode
        ,string itemId
        ,string taskId = null)
    {
        string TaskId = taskId;
        GDEtimeTaskData taskData;

        if (string.IsNullOrEmpty(TaskId))
        {
            string taskNB = "TT_" + taskType.ToString();
            List<GDEtimeTaskData> allFix = PlayerData.TimeTaskList.FindAll(x => x.taskId.Contains(taskNB));
            TaskId = taskNB + "#" + string.Format("{0:D2}", allFix.Count);
        }
        else if (haveTimeTaskByTaskId(TaskId, out taskData))
        {
            if (!string.IsNullOrEmpty(taskData.startTime))
            {
                DateTime starttime = Convert.ToDateTime(taskData.startTime);
                TimeSpan interval = DateTime.Now - starttime;
                if (interval.Seconds < taskData.timeType * 60)
                {
                    Debug.Log("已存在该任务且未完成，添加失败");
                    return;
                }
            }
        }
        //
        taskData = new GDEtimeTaskData(GDEItemKeys.timeTask_emptyTimeTask)
        {
            taskId = taskId,
            itemHashcode = Hashcode,
            itemId = itemId,
            startTime = DateTime.Now.ToString(),
        };
        if(taskType == SDConstants.timeTaskType.HOSP)
        {
            int maxF = getHeroMaxFatigue(taskData.itemHashcode);
            GDEHeroData hero = getHeroByHashcode(Hashcode);
            ROHeroData _hero = getHeroOriginalDataById(hero.id);
            int mainTimeType = (hero.starNumUpgradeTimes + _hero.starNum + _hero.quality) 
                * 2 + 10;

            int status = hero.status;
            if(status == 2)
            {
                setHeroStatus(Hashcode, 3);
                taskData.oldData = status;
                taskData.timeType = mainTimeType;
                PlayerData.TimeTaskList.Add(taskData);
                PlayerData.Set_TimeTaskList();
            }
            else if(status != 3)
            {
                float fatigueRate = getHeroFatigueRate(Hashcode);
                if (fatigueRate > 0.1f)//存在明显疲劳
                {
                    setHeroStatus(Hashcode, 3);
                    taskData.oldData = status;
                    taskData.timeType = (int)(mainTimeType * fatigueRate * 0.8f);
                    PlayerData.TimeTaskList.Add(taskData);
                    PlayerData.Set_TimeTaskList();
                }
                else//太轻松不需要治疗
                {

                }
            }
        }
        else if(taskType == SDConstants.timeTaskType.FACT)
        {
            consumableItem M = getConsumableById(itemId);
            SDConstants.MaterialType mt = M.MaterialType;
            int mainTimeType = (M.LEVEL + 1) * 2;
            GDENPCData slave = SDDataManager.Instance.GetNPCOwned(Hashcode);
            if (slave!=null)
            {
                int lv = getLevelByExp(slave.exp);
                int like = getLikeByLikability(slave.likability,out float rate);
                mainTimeType = Mathf.Max(2, mainTimeType - lv);
                if (mt == SDConstants.MaterialType.exp)
                {
                    taskData.oldData = slave.workPower0;
                }
                else if(mt == SDConstants.MaterialType.equip_exp)
                {
                    taskData.oldData = slave.workPower1;
                }
                else
                {
                    taskData.oldData = slave.workPower2;
                }
            }
            else
            {
                taskData.oldData = 0;
            }
            taskData.timeType = mainTimeType;
            PlayerData.TimeTaskList.Add(taskData);
            PlayerData.Set_TimeTaskList();
        }
    }
    public bool AbandonTimeTask(string taskId)
    {
        GDEtimeTaskData task = PlayerData.TimeTaskList.Find(x => x.taskId == taskId);
        if (task!=null && DateTime.TryParse(task.startTime, out DateTime starttime))
        {
            if (taskId.Contains("TT_HOSP"))
            {
                TimeSpan span = DateTime.Now - starttime;
                if (span.Minutes * 1f / task.timeType < 0.2f)
                {
                    //确认没能进行足够时间来进行恢复，timetask未完成
                    foreach (GDEtimeTaskData T in PlayerData.TimeTaskList)
                    {
                        if (T.taskId == taskId)
                        {
                            PlayerData.TimeTaskList.Remove(T);
                            PlayerData.Set_TimeTaskList();
                            setHeroStatus(task.itemHashcode, task.oldData);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    public bool FinishTimeTask(string taskId)
    {
        GDEtimeTaskData task = PlayerData.TimeTaskList.Find(x => x.taskId == taskId);
        if (task != null && DateTime.TryParse(task.startTime, out DateTime starttime))
        {
            TimeSpan span = DateTime.Now - starttime;
            if (taskId.Contains("TT_HOSP"))
            {
                bool flag = false;
                if (task.oldData != 2)//该角色之前并未遭受重创
                {
                    float s = span.Minutes * 1f / task.timeType;
                    if (s >= 0.2f)
                    {
                        //确认已进行恢复最小时间
                        int getHeroF = getHeroFatigue(task.itemHashcode);
                        setHeroFatigue((int)(getHeroF * (1f-s)), task.itemHashcode);
                        flag = true;
                    }
                }
                else//该角色之前遭受重创
                {
                    float s = span.Minutes * 1f / task.timeType;
                    if (s >= 1)
                    {
                        //重创角色只能使用timetype时间
                        setHeroFatigue(0, task.itemHashcode);
                        flag = true;
                    }
                }
                if (flag)
                {
                    foreach (GDEtimeTaskData T in PlayerData.TimeTaskList)
                    {
                        if (T.taskId == taskId)
                        {
                            PlayerData.TimeTaskList.Remove(T);
                            PlayerData.Set_TimeTaskList();
                            setHeroStatus(task.itemHashcode, 0);
                            return true;
                        }
                    }
                }
            }
            else if (taskId.Contains("TT_FACT"))
            {
                foreach (GDEtimeTaskData T in PlayerData.TimeTaskList)
                {
                    if (T.taskId == taskId)
                    {
                        TimeSpan leaveT = TimeSpan.FromMinutes(span.Minutes % task.timeType);
                        int num = span.Minutes / task.timeType;
                        addConsumable(task.itemId, num);
                        T.startTime = (DateTime.Now - leaveT).ToString();
                        PlayerData.Set_TimeTaskList();
                        string MN = getMaterialNameById(task.itemId);
                        PopoutController.CreatePopoutMessage("获得 " + MN + " X " + num, 50);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public GDEtimeTaskData GetTimeTaskById(string taskId)
    {
        return PlayerData.TimeTaskList.Find(x => x.taskId == taskId);
    }
    public bool ChangeNPCInFactoryAssemblyLine(int NPCHashcode,string taskId)
    {
        foreach(GDEtimeTaskData T in PlayerData.TimeTaskList)
        {
            if(T.taskId == taskId && T.itemHashcode != NPCHashcode)
            {
                T.itemHashcode = NPCHashcode;
                PlayerData.Set_TimeTaskList();
                return true;
            }
        }
        return false;
    }
    #endregion
    #region Task_Info

    #endregion
    #region Achievement_Infor
    public GDEAchievementData getAchievementData()
    {
        return PlayerData.achievementData;
    }
    public void addAchievementDataByType(string type, int num = 1)
    {
        GDEAchievementData data = PlayerData.achievementData;
        if (type == "login")
        {
            data.login += num;
        }
        else if(type == "kill_fodder")
        {
            data.killFodder += num;
        }
        else if (type == "kill_normal")
        {
            data.killNormalEnemy += num;
        }
        else if(type == "kill_elite")
        {
            data.killElite += num;
        }
        else if (type == "kill_boss")
        {
            data.killBoss += num;
        }
        else if(type == "kill_god")
        {
            data.killGod += num;
        }
        else if (type == "forgeEquip")
        {
            data.forgeEquip += num;
        }
        else if (type == "forgeProp")
        {
            data.forgeProp += num;
        }
        else if (type == "useProp")
        {
            data.useProp += num;
        }
        else if (type == "ownHero")
        {
            data.ownHero += num;
        }
        else if (type == "ownHeroFightForce")
        {
            if (num >= data.ownHeroFightForce)
            {
                data.ownHeroFightForce = num;
            }
        }
        else if (type == "ownEquip")
        {
            data.ownEquip += num;
        }
        else if (type == "finishTarget")
        {
            data.finishTarget += num;
        }
        else if (type == "earnGold")
        {
            if (num <= int.MaxValue - data.earnCoin)
            {
                data.earnCoin += num;
            }
        }
        else if (type == "consumeCoin")
        {
            data.consumeCoin += num;
        }
        else if (type == "passedNum_level")
        {
            data.passedNum_level += num;
        }
        else if(type == "heroDie")
        {
            data.heroDie += num;
        }
    }
    public List<GDEItemData> GetAllEnemiesPlayerSaw
    {
        get { return PlayerData.achievementData.EnemiesGet; }
    }
    public void AddKillingDataToAchievement(string enemyId)
    {
        List<GDEItemData> All = GetAllEnemiesPlayerSaw;
        if(All.Exists(x=>x.id == enemyId))
        {
            foreach(GDEItemData d in PlayerData.achievementData.EnemiesGet)
            {
                if(d.id == enemyId) 
                {
                    d.num++;
                    PlayerData.achievementData.Set_EnemiesGet();
                    break; 
                }
            }
        }
        else
        {
            GDEItemData newE = new GDEItemData(GDEItemKeys.Item_MaterialEmpty)
            {
                id = enemyId,num=1,index = 0,
            };
            PlayerData.achievementData.EnemiesGet.Add(newE);
            PlayerData.achievementData.Set_EnemiesGet();
        }
    }
    #endregion
    #region Equipments_Infor
    public GDEEquipmentData getHeroEquipHelmet(int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                if (hero.equipHelmet != null)
                {
                    bool exit = false;
                    foreach (GDEEquipmentData a in PlayerData.equipsOwned)
                    {
                        if (a.hashcode == hero.equipHelmet.hashcode)
                        {
                            exit = true;
                        }
                    }
                    if (exit)
                    {
                        return hero.equipHelmet;
                    }
                    else
                    {
                        hero.equipHelmet.id = string.Empty;
                        hero.equipHelmet.equipType = 0;
                        hero.equipHelmet.exp = 0;
                        hero.equipHelmet.equipBattleForce = 0;
                        hero.equipHelmet.hashcode = 0;
                        hero.equipHelmet.num = 0;
                        return hero.equipHelmet;
                    }
                }
            }
        }
        return null;
    }
    public GDEEquipmentData getHeroEquipBreastplate(int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                if (hero.equipBreastplate != null)
                {
                    bool exit = false;
                    foreach (GDEEquipmentData a in PlayerData.equipsOwned)
                    {
                        if (a.hashcode == hero.equipBreastplate.hashcode)
                        {
                            exit = true;
                        }
                    }
                    if (exit)
                    {
                        return hero.equipBreastplate;
                    }
                    else
                    {
                        hero.equipBreastplate.id = string.Empty;
                        hero.equipBreastplate.equipType = 0;
                        hero.equipBreastplate.exp = 0;
                        hero.equipBreastplate.equipBattleForce = 0;
                        hero.equipBreastplate.hashcode = 0;
                        hero.equipBreastplate.num = 0;
                        return hero.equipBreastplate;
                    }
                }
            }
        }
        return null;
    }
    public GDEEquipmentData getHeroEquipGardebras(int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                if (hero.equipGardebras != null)
                {
                    bool exit = false;
                    foreach (GDEEquipmentData a in PlayerData.equipsOwned)
                    {
                        if (a.hashcode == hero.equipGardebras.hashcode)
                        {
                            exit = true;
                        }
                    }
                    if (exit)
                    {
                        return hero.equipGardebras;
                    }
                    else
                    {
                        hero.equipGardebras.id = string.Empty;
                        hero.equipGardebras.equipType = 0;
                        hero.equipGardebras.exp = 0;
                        hero.equipGardebras.equipBattleForce = 0;
                        hero.equipGardebras.hashcode = 0;
                        hero.equipGardebras.num = 0;
                        return hero.equipGardebras;
                    }
                }
            }
        }
        return null;
    }
    public GDEEquipmentData getHeroEquipLegging(int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                if (hero.equipLegging != null)
                {
                    bool exit = false;
                    foreach (GDEEquipmentData a in PlayerData.equipsOwned)
                    {
                        if (a.hashcode == hero.equipLegging.hashcode)
                        {
                            exit = true;
                        }
                    }
                    if (exit)
                    {
                        return hero.equipLegging;
                    }
                    else
                    {
                        hero.equipLegging.id = string.Empty;
                        hero.equipLegging.equipType = 0;
                        hero.equipLegging.exp = 0;
                        hero.equipLegging.equipBattleForce = 0;
                        hero.equipLegging.hashcode = 0;
                        hero.equipLegging.num = 0;
                        return hero.equipLegging;
                    }
                }
            }
        }
        return null;
    }
    public GDEEquipmentData getHeroEquipJewelry(int heroHashcode, bool isSecondJewelry = false)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                if (!isSecondJewelry)
                {
                    if (hero.jewelry0 != null)
                    {
                        bool exit = false;
                        foreach (GDEEquipmentData a in PlayerData.equipsOwned)
                        {
                            if (a.hashcode == hero.jewelry0.hashcode)
                            {
                                exit = true;
                            }
                        }
                        if (exit)
                        {
                            return hero.jewelry0;
                        }
                        else
                        {
                            hero.jewelry0.id = string.Empty;
                            hero.jewelry0.equipType = 0;
                            hero.jewelry0.exp = 0;
                            hero.jewelry0.equipBattleForce = 0;
                            hero.jewelry0.hashcode = 0;
                            hero.jewelry0.num = 0;
                            return hero.jewelry0;
                        }
                    }
                }
                else
                {
                    if (hero.jewelry1 != null)
                    {
                        bool exit = false;
                        foreach (GDEEquipmentData a in PlayerData.equipsOwned)
                        {
                            if (a.hashcode == hero.jewelry1.hashcode)
                            {
                                exit = true;
                            }
                        }
                        if (exit)
                        {
                            return hero.jewelry1;
                        }
                        else
                        {
                            hero.jewelry1.id = string.Empty;
                            hero.jewelry1.equipType = 0;
                            hero.jewelry1.exp = 0;
                            hero.jewelry1.equipBattleForce = 0;
                            hero.jewelry1.hashcode = 0;
                            hero.jewelry1.num = 0;
                            return hero.jewelry1;
                        }
                    }
                }
            }
        }
        return null;
    }
    public GDEEquipmentData getHeroWeapon(int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                if (hero.equipWeapon != null)
                {
                    bool exit = false;
                    foreach (GDEEquipmentData a in PlayerData.equipsOwned)
                    {
                        if (a.hashcode == hero.equipWeapon.hashcode)
                        {
                            exit = true;
                        }
                    }
                    if (exit)
                    {
                        return hero.equipWeapon;
                    }
                    else
                    {
                        hero.equipWeapon.id = string.Empty;
                        hero.equipWeapon.equipType = 0;
                        hero.equipWeapon.exp = 0;
                        hero.equipWeapon.equipBattleForce = 0;
                        hero.equipWeapon.hashcode = 0;
                        hero.equipWeapon.num = 0;
                        return hero.equipWeapon;
                    }
                }
            }
        }
        return null;
    }
    public GDEEquipmentData getHeroEquipmentByPos
        (int heroHashcode, EquipPosition pos, bool isSecondJPos = false)
    {
        switch (pos)
        {
            case EquipPosition.Head: return Instance.getHeroEquipHelmet(heroHashcode);
            case EquipPosition.Breast: return Instance.getHeroEquipBreastplate(heroHashcode);
            case EquipPosition.Arm: return Instance.getHeroEquipGardebras(heroHashcode);
            case EquipPosition.Leg: return Instance.getHeroEquipLegging(heroHashcode);
            case EquipPosition.Finger:
                if (!isSecondJPos) return Instance.getHeroEquipJewelry(heroHashcode);
                else return Instance.getHeroEquipJewelry(heroHashcode, true);
            case EquipPosition.Hand:
                return Instance.getHeroWeapon(heroHashcode);
            default: return null;
        }
    }
    public GDEEquipmentData equipEmpty()
    {
        GDEEquipmentData e = new GDEEquipmentData(GDEItemKeys.Equipment_EquipEmpty);
        e.hashcode = e.OwnerHashcode = e.equipType = e.equipBattleForce = e.index
            = e.num = e.exp = 0;
        e.id = string.Empty;
        return e;
    }
    public GDEEquipmentData getEquipmentByHashcode(int itemHashcode)
    {
        List<GDEEquipmentData> all = SDDataManager.Instance.PlayerData.equipsOwned;
        foreach (GDEEquipmentData e in all)
        {
            if (e.hashcode == itemHashcode)
            {
                return e;
            }
        }
        return null;
    }
    public RoleAttributeList EquipRALByDictionary(Dictionary<string, string> s)
    {
        RoleAttributeList ral = new RoleAttributeList();
        string mainE = s["mainEffect"]; string sideE = s["sideEffect"];
        ral.Add(OneAttritube.ReadEffectString(mainE));//添加主属性
        ral.Add(OneAttritube.ReadEffectString(sideE));//添加副属性
        return ral;
    }
    public List<GDEEquipmentData> getAllOwnedEquips()
    {
        return PlayerData.equipsOwned;
    }
    public List<GDEEquipmentData> getOwnedEquipsByPos(EquipPosition Pos, bool listOrder = false)
    {
        List<GDEEquipmentData> equips = SDDataManager.Instance.PlayerData.equipsOwned;
        List<GDEEquipmentData> outData = new List<GDEEquipmentData>();
        for (int i = 0; i < equips.Count; i++)
        {
            GDEEquipmentData e = equips[i];
            if (SDDataManager.Instance.getEquipPosById(e.id) == (int)Pos)
            {
                outData.Add(e);
            }
        }
        if (listOrder)
        {
            List<GDEEquipmentData> equipsListOrder = outData;
            equipsListOrder.Sort((x,y)=> 
            {
                int l_x = getEquipRarityById(x.id);
                int l_y = getEquipRarityById(y.id);
                return -l_x.CompareTo(l_y);
            });
            return equipsListOrder;
        }
        return outData;
    }
    public List<GDEEquipmentData> GetPosOwnedEquipsByCareer(EquipPosition Pos
        ,string heroId, bool listOrder = false)
    {
        List<GDEEquipmentData> allEquips = SDDataManager.Instance.getOwnedEquipsByPos(Pos, listOrder);
        if (Pos == EquipPosition.Hand)
        {
            List<WeaponRace> heroCanUse = getHeroInfoById(heroId).WeaponRaceList;
            List<GDEEquipmentData> AllEs = allEquips.FindAll
                (x => 
                {
                    if (string.IsNullOrEmpty(heroId)) return true;
                    WeaponRace race = GetEquipItemById(x.id).WeaponRace;
                    return heroCanUse.Contains(race);
                });
            return AllEs;
        }
        else
        {
            return allEquips;
        }
    }

    public List<EquipItem> AllEquipList
    {
        get
        {
            EquipItem[] all = Resources.LoadAll<EquipItem>
                ("ScriptableObjects/items/Equips");
            List<EquipItem> results = new List<EquipItem>();
            for(int i = 0; i < all.Length; i++)
            {
                results.Add(all[i]);
            }
            return results;
        }
    }
    public EquipItem GetEquipItemById(string id)
    {
        List<EquipItem> all = AllEquipList;
        foreach(EquipItem item in all)
        {
            if (item.ID == id) return item;
        }
        return null;
    }
    public int getEquipPosById(string id)
    {
        List<EquipItem> all = AllEquipList;
        foreach(EquipItem item in all)
        {
            if(item.ID == id)
            {
                return (int)item.EquipPos;
            }
        }
        return (int)EquipPosition.End;
    }
    /// <summary>
    /// same as LEVEL for equip
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int getEquipRarityById(string id)
    {
        List<EquipItem> all = AllEquipList;
        foreach (EquipItem item in all)
        {
            if (item.ID == id)
            {
                return item.LEVEL;
            }
        }
        return 0;
    }
    public int getEquipBaiscBattleForceById(string id)
    {
        EquipItem item = GetEquipItemById(id);
        return item.BattleForce;
    }
    public int getEquipBattleForceByHashCode(int itemHashcode)
    {
        GDEEquipmentData equip = SDDataManager.Instance.getEquipmentByHashcode(itemHashcode);
        int basic = SDDataManager.Instance.getEquipBaiscBattleForceById(equip.id);
        int level = SDDataManager.Instance.getLevelByExp(equip.exp);
        int flag = (int)(basic * (1 + level * 0.15f));
        return flag;
    }
    public string getEquipNameByHashcode(int itemHashcode)
    {
        GDEEquipmentData e = SDDataManager.Instance.getEquipmentByHashcode(itemHashcode);
        string id = e.id;
        EquipItem item = GetEquipItemById(id);
        if (item) return item.NAME;
        return SDGameManager.T("无此装备");
    }
    public bool checkEquipFixIfSuccess(int hashcode)
    {
        foreach (GDEEquipmentData e in PlayerData.equipsOwned)
        {
            if (e.hashcode == hashcode)
            {
                int quality = e.quality;
                if (quality >= SDConstants.equipMaxQuality) return false;
                float rate = Mathf.Min(1
                    , (SDConstants.equipMaxQuality - quality - 1) * 0.2f + 0.25f);
                if (UnityEngine.Random.Range(0, 1) < rate)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public int getEquipQualityByHashcode(int hashcode)
    {
        foreach (GDEEquipmentData e in PlayerData.equipsOwned)
        {
            if (e.hashcode == hashcode)
            {
                return e.quality;
            }
        }
        return 0;
    }
    public float getPossibleLvupByTargetLevel(int targetLevel,out int visualRate)
    {
        float m = 1 - targetLevel * targetLevel * 0.01f;
        visualRate = (int)(((m * 100) / 10) * 10);
        if (targetLevel <= 5) { m += 0.15f; }
        else { m -= 0.25f; }
        return m;
    }

    public void addExpToEquipByHashcode(int hashcode, int figure)
    {
        foreach (GDEEquipmentData equip in PlayerData.equipsOwned)
        {
            if (equip.hashcode == hashcode)
            {
                equip.exp += figure;
                PlayerData.Set_equipsOwned();
                break;
            }
        }
    }


    public void addEquip(GDEEquipmentData equip)
    {
        Instance.equipNum++;
        //
        add_Item(equip.id);
        //
        equip.hashcode = Instance.equipNum;
        equip.OwnerHashcode = 0;
        Instance.PlayerData.equipsOwned.Add(equip);
        Instance.PlayerData.Set_equipsOwned();
    }
    public void addEquip(string id)
    {
        Instance.equipNum++;
        //
        add_Item(id);
        //
        int level = GetEquipItemById(id).LEVEL;
        GDEEquipmentData e = new GDEEquipmentData(GDEItemKeys.Equipment_EquipEmpty)
        {
            id = id,
            hashcode = Instance.equipNum,
            OwnerHashcode = 0,
            locked = level<2?false:true,
            exp = 0,
            num = 1,
            index = 0,
            quality = UnityEngine.Random.Range(0, SDConstants.equipMaxQuality),
            initialQuality = UnityEngine.Random.Range(0, 1),
        };
        Instance.PlayerData.equipsOwned.Add(e);
        Instance.PlayerData.Set_equipsOwned();
    }
    public bool consumeEquip(int hashcode)
    {
        foreach (GDEEquipmentData E in PlayerData.equipsOwned)
        {
            if (E.hashcode == hashcode && !E.locked)
            {
                PlayerData.equipsOwned.Remove(E);
                PlayerData.Set_equipsOwned();
                consume_Item(E.id, out int leftAmount);
                return true;
            }
        }
        Debug.Log("对象(装备#" + hashcode + ")不存在或已被锁定");
        return false;
    }
    public IEnumerable<GDEEquipmentData> FindAllArmorsById(string id, bool onlyUnlocked = true)
    {
        return PlayerData.equipsOwned.FindAll(x => x.id == id
        && (onlyUnlocked ? true : !x.locked)).AsEnumerable();
    }
    #endregion
    #region residentMovementInfor
    public RoleAttributeList BuffFromRace(RoleAttributeList basic, Race r)
    {
        RoleAttributeList ral = RoleAttributeList.zero;
        //人类
        if (r == Race.Human)
        {

        }
        //精灵在夜间战斗力上升，白天速度降低
        else if (r == Race.Elf)
        {
            if (ResidentMovementData.CurrentDayNightId == 1)//夜间
            {
                ral.AT = basic.read(AttributeData.AT, 5);
                ral.MT = basic.read(AttributeData.MT, 5);
                ral.Speed = basic.read(AttributeData.Speed, 5);
                ral.Accur = basic.read(AttributeData.Accur, 5);
            }
            else
            {
                ral.Speed = -basic.read(AttributeData.Speed, 5);
            }
        }
        //龙裔基础能力周期性增强
        else if (r == Race.Dragonborn)
        {
            ActionBarManager abm = FindObjectOfType<ActionBarManager>();
            if (abm)
            {
                int flag = abm.CurrentRoundNum % 4;
                flag = flag <= 2 ? flag : 4 - flag;
                ral.AT = basic.read(AttributeData.AT, flag);
                ral.AD = basic.read(AttributeData.AD, flag);
                ral.MT = basic.read(AttributeData.MT, flag);
                ral.MD = basic.read(AttributeData.MD, flag);
            }
        }

        return ral;
    }
    public RoleAttributeList BuffFromDaynight(RoleAttributeList basic)
    {
        RoleAttributeList ral = RoleAttributeList.zero;
        if (ResidentMovementData.CurrentDayNightId == 1)//夜间
        {
            ral.Accur = -basic.read(AttributeData.Accur, 5);
        }
        else
        {

        }
        return ral;
    }
    #endregion
    #region Skill_Infor
    public List<GDEASkillData> addStartSkillsWhenSummoning(string heroId)
    {
        List<OneSkill> all
            = SkillDetailsList.WriteOneSkillList(heroId);

        int skillNum = UnityEngine.Random.Range(2, 3);
        skillNum = Mathf.Min(skillNum, all.Count);

        all = ROHelp.RandomList(all);
        List<GDEASkillData> list = new List<GDEASkillData>();

        for (int i = 0; i < all.Count; i++)
        {
            bool flag = i < skillNum;
            all[i].lv = flag ? 0 : -1;
        }

        List<OneSkill> _all = all.FindAll(x => !x.islocked);
        Debug.Log("EXIT_OMEGASKILL: " + all.Exists(x => x.isOmegaSkill));
        if (!_all.Exists(x => x.isOmegaSkill))
        {
            List<OneSkill> fa = all.FindAll(x => x.isOmegaSkill);
            List<string> l =fa.Select(x => x.skillId).ToList();
            string l0 = l[UnityEngine.Random.Range(0, l.Count)];
            all.Find(x => x.skillId == l0).lv = 0;
        }
        if (!_all.Exists(x => !x.isOmegaSkill))
        {
            List<OneSkill> fa = all.FindAll(x => !x.isOmegaSkill);
            List<string> l =fa.Select(x => x.skillId).ToList();
            string l0 = l[UnityEngine.Random.Range(0, l.Count)];
            all.Find(x => x.skillId == l0).lv = 0;
        }

        all.Sort((x, y) => x.index.CompareTo(y.index));
        all.Sort((x, y) => x.isOmegaSkill.CompareTo(y.isOmegaSkill));
        all.Sort((x, y) => x.lv.CompareTo(y.lv));
        for(int i = 0; i < all.Count; i++)
        {
            GDEASkillData s = new GDEASkillData(GDEItemKeys.ASkill_normalAttack)
            {
                Id = all[i].skillId
    ,
                Lv = all[i].lv,
            };
            list.Add(s);
        }
        return list;
    }
    /// <summary>
    /// 英雄所有已经解锁的技能
    /// </summary>
    /// <param name="hashcode"></param>
    /// <returns></returns>
    public List<GDEASkillData> OwnedSkillsByHero(int hashcode)
    {
        GDEHeroData hero = getHeroByHashcode(hashcode);
        List<GDEASkillData> owns = hero.skillsOwned.FindAll(x=>x.Lv>=0);
        return owns;
    }
    public List<OneSkill> getAllSkillsByHashcode(int heroHashcode)
    {
        GDEHeroData hero = getHeroByHashcode(heroHashcode);
        List<OneSkill> all = SkillDetailsList.WriteOneSkillList
            (hero.id);
        List<GDEASkillData> ownedSkills = hero.skillsOwned;

        AllStrs2 = ownedSkills.Select(x =>
        {
            return x.Id + "___" + x.Lv;
        }).ToList();

        foreach(OneSkill s in all) { s.lv = -1; }
        for(int i = 0; i < ownedSkills.Count; i++)
        {
            string id = ownedSkills[i].Id;
            if(all.Exists(x=>x.skillId == id))
            {
                all.Find(x => x.skillId == id).lv = ownedSkills[i].Lv;
            }
        }


        return all;
    }
    public OneSkill getOwnedSkillById(string skillId, int heroHashcode)
    {
        if (string.IsNullOrEmpty(skillId)) return null;
        GDEHeroData hero = getHeroByHashcode(heroHashcode);
        List<OneSkill> all
            = SkillDetailsList.WriteOneSkillList(hero.id);
        OneSkill s = OneSkill.normalAttack;
        s = all.Find(x => x.skillId == skillId);
        List<GDEASkillData> owns = hero.skillsOwned;
        s.lv = owns.Find(x => x.Id == skillId).Lv;
        return s;
    }
    public OneSkill getSkillByHeroId(string skillId, string heroId)
    {
        HeroInfo info = getHeroInfoById(heroId);
        List<OneSkill> all
            = SkillDetailsList.WriteOneSkillList(heroId);
        foreach (OneSkill s in all)
        {
            if (s.skillId == skillId)
            {
                return s;
            }
        }
        return null;
    }
    public string getDeployedSkillId(int skillPos, int heroHashcode)
    {
        GDEHeroData hero = getHeroByHashcode(heroHashcode);
        if (hero != null)
        {
            if (skillPos == 0)
            {
                return hero.skill0Id;
            }
            else if (skillPos == 1) { return hero.skill1Id; }
            else if (skillPos == 2) { return hero.skillOmegaId; }
        }
        return string.Empty;
    }
    public bool ifDeployThisSkill(string skillId, int heroHashcode)
    {
        GDEHeroData hero = getHeroByHashcode(heroHashcode);
        OneSkill skill = getOwnedSkillById(skillId, heroHashcode);
        if (skill.lv < 0) return false;
        if (hero.skill0Id == skillId
            || hero.skillOmegaId == skillId)
        {
            return true;
        }
        if (hero.skill1Id == skillId && checkHeroEnableSkill1ByHashcode(heroHashcode))
        {
            return true;
        }
        return false;
    }
    public void UnDeploySkillById(string skillId, int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                if (hero.skill0Id == skillId)
                {
                    hero.skill0Id = string.Empty;
                }
                else if (hero.skill1Id == skillId)
                {
                    hero.skill1Id = string.Empty;
                }
                else if (hero.skillOmegaId == skillId)
                {
                    hero.skillOmegaId = string.Empty;
                }
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    public void changeEquipedSkill(string newSkillId, int skillPos, int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                if (skillPos == 0)
                {
                    hero.skill0Id = newSkillId;
                }
                else if (skillPos == 1 && checkHeroEnableSkill1ByHashcode(heroHashcode))
                {
                    hero.skill1Id = newSkillId;
                }
                else if (skillPos == 2)
                {
                    hero.skillOmegaId = newSkillId;
                }
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    #endregion
    #region Building_Infor
    public bool LvUpBuilding(string id, int level = 1)
    {
        foreach (GDEtownBuildingData B in PlayerData.buildingsOwned)
        {
            if (B.id == id)
            {
                B.level += level;
                PlayerData.Set_buildingsOwned();
            }
        }
        return false;
    }
    #endregion
    #region Enemy_Infor
    public ROEnemyData getEnemyDataById(string id)
    {
        ROEnemyData d = new ROEnemyData();
        EnemyInfo enemy = SDDataManager.Instance.getEnemyInfoById(id);
        if (enemy)
        {
            d.Info = enemy;
            d.CRIDmg = 175;
            d.DmgReduction = 0;
            d.DmgReflection = 0;
            d.dropCoins = 5 * enemy.EnemyRank.Index + 5;
            return d;
        }
        return null;
    }
    public List<EnemyInfo> AllEnemyList
    {
        get 
        {
            List<EnemyInfo> results = new List<EnemyInfo>();
            EnemyInfo[] all = Resources.LoadAll<EnemyInfo>
                ("ScriptableObjects/enemies");
            for(int i = 0; i < all.Length; i++)
            {
                results.Add(all[i]);
            }
            return results;
        }
    }
    public EnemyInfo getEnemyInfoById(string id)
    {
        return AllEnemyList.Find(x => x.ID == id);
    }
    public EnemyRank getEnemyRankByIndex(int race)
    {
        EnemyRank[] allranks = Resources.LoadAll<EnemyRank>("");
        foreach (EnemyRank rank in allranks)
        {
            if (rank.Index == race) return rank;
        }
        return null;
    }
    #endregion
    #region Rune_Infor
    public List<RuneItem> AllRuneList
    {
        get
        {
            RuneItem[] all = Resources.LoadAll<RuneItem>("ScriptableObjects/Items/Runes");
            return all.ToList();
        }
    }
    public RuneItem getRuneItemById(string id)
    {
        return AllRuneList.Find(x => x.ID == id);
    }
    public List<GDERuneData> getAllRunesOwned
    {
        get 
        {
            List<GDERuneData> list = PlayerData.RunesOwned;
            bool flag = list.GroupBy(x => x.Hashcode).Where(x => x.Count() > 1).Count() > 0;
            if (flag)
            {
                for(int i = 0; i < list.Count; i++)
                {
                    list[i].Hashcode = i + 1;
                }
                PlayerData.RunesOwned = list;
                PlayerData.Set_RunesOwned();
            }
            return PlayerData.RunesOwned; 
        }
    }
    public GDERuneData getRuneOwnedByHashcode(int hashcode)
    {
        return PlayerData.RunesOwned.Find(x => x.Hashcode == hashcode);
    }
    public bool getRuneEquippedByPosAndGoddess(int pos, string ownerId, out GDERuneData data)
    {
        GDEgoddessData goddess = getGDEGoddessDataById(ownerId);
        if(pos == 0)
        {
            GDERuneData _data = getRuneOwnedByHashcode(goddess.rune0);
            if (_data != null)
            {
                data = _data;
                return true;
            }
        }
        else if(pos == 1)
        {
            GDERuneData _data = getRuneOwnedByHashcode(goddess.rune1);
            if (_data != null)
            {
                data = _data;
                return true;
            }
        }
        else if(pos == 2)
        {
            GDERuneData _data = getRuneOwnedByHashcode(goddess.rune2);
            if (_data != null)
            {
                data = _data;
                return true;
            }
        }
        else if(pos == 3)
        {
            GDERuneData _data = getRuneOwnedByHashcode(goddess.rune3);
            if (_data != null)
            {
                data = _data;
                return true;
            }
        }
        data = null;return false;
    }
    public bool checkRuneEquippedByGoddess(int hashcode,string ownerId,out int pos)
    {
        GDEgoddessData goddess = getGDEGoddessDataById(ownerId);
        if (goddess == null)
        {
            pos = 0;return false;
        }
        if (goddess.rune0 == hashcode)
        {
            pos = 0; return true;
        }
        else if (goddess.rune1 == hashcode)
        {
            pos = 1; return true;
        }
        else if (goddess.rune2 == hashcode)
        {
            pos = 2; return true;
        }
        else if(goddess.rune3 == hashcode)
        {
            pos = 3;return true;
        }
        pos = 0;
        return false;
    }
    public void AddRune(string runeId)
    {
        Instance.runeNum++;
        add_Item(runeId);
        RuneItem item = getRuneItemById(runeId);
        if (item)
        {
            GDERuneData _rune = new GDERuneData(GDEItemKeys.Rune_RuneEmpty)
            {
                id = runeId,
                Hashcode = Instance.runeNum,
                posInOwner = 0,
                ownerId = string.Empty,
                quality = item.Quality,
                level = 0,
                initalQuality = 0,
                locked = false,
                //
                attitube = new GDEgoddessAttiData
                    (GDEItemKeys.goddessAtti_emptyGAtti)
                {
                    stamina = UnityEngine.Random.Range(0, 2) + item.Atti.Stamina,
                    agile = UnityEngine.Random.Range(0, 2) + item.Atti.Agile,
                    recovery = UnityEngine.Random.Range(0, 2) + item.Atti.Recovery,
                    leader = UnityEngine.Random.Range(0, 2) + item.Atti.Leader,
                }
            };
            PlayerData.RunesOwned.Add(_rune);
            PlayerData.Set_RunesOwned();
        }
    }
    public bool ConsumeRune(int hashcode)
    {
        foreach(GDERuneData rune in PlayerData.RunesOwned)
        {
            if(rune.Hashcode == hashcode)
            {
                PlayerData.RunesOwned.Remove(rune);
                PlayerData.Set_RunesOwned();
                consume_Item(rune.id, out int leftamount);
                return true;
            }
        }
        return false;
    }
    public void addRuneToGoddessSlot(int runeHashcode,string goddessId, int pos)
    {
        GDERuneData rune = getRuneOwnedByHashcode(runeHashcode);
        int oldRune = 0;
        if (rune != null)
        {
            //从原始所有者上卸下
            if (PlayerData.goddessOwned.Exists(x => x.id == rune.ownerId))
            {
                foreach (GDEgoddessData g in PlayerData.goddessOwned)
                {
                    if (g.id == rune.ownerId)
                    {
                        if (g.rune0 == runeHashcode) { g.rune0 = 0; }
                        if (g.rune1 == runeHashcode) { g.rune1 = 0; }
                        if (g.rune2 == runeHashcode) { g.rune2 = 0; }
                        if (g.rune3 == runeHashcode) { g.rune3 = 0; }
                        PlayerData.Set_goddessOwned();
                        break;
                    }
                }
            }
            //装至目标身上
            foreach (GDEgoddessData goddess in PlayerData.goddessOwned)
            {
                if(goddess.id == goddessId)
                {
                    if(pos == 0)
                    {
                        oldRune = goddess.rune0;
                        goddess.rune0 = runeHashcode;
                    }
                    else if(pos == 1)
                    {
                        oldRune = goddess.rune1;
                        goddess.rune1 = runeHashcode;
                    }
                    else if(pos == 2)
                    {
                        oldRune = goddess.rune2;
                        goddess.rune2 = runeHashcode;
                    }
                    else if(pos == 3)
                    {
                        oldRune = goddess.rune3;
                        goddess.rune3 = runeHashcode;
                    }
                    PlayerData.Set_goddessOwned();
                    break;
                }
            }
            //修改原装备信息
            foreach(GDERuneData R in PlayerData.RunesOwned)
            {
                if(R.Hashcode == oldRune)
                {
                    R.ownerId = string.Empty;
                    PlayerData.Set_RunesOwned();
                    break;
                }
            }
            //修改新装备信息
            foreach (GDERuneData _r in PlayerData.RunesOwned)
            {
                if (_r.Hashcode == runeHashcode)
                {
                    _r.ownerId = goddessId;
                    break;
                }
            }
        }
    }
    public bool checkRuneStatus(int hashcode)
    {
        GDERuneData rune = getRuneOwnedByHashcode(hashcode);
        if (rune!=null && !string.IsNullOrEmpty(rune.ownerId))
        {
            if(PlayerData.goddessOwned.Exists(x=>x.id == rune.ownerId))
            {
                return true;
            }
        }
        return false;
    }
    public bool lvUpRune(int hashcode, int levelUp = 1)
    {
        GDERuneData rune = getRuneOwnedByHashcode(hashcode);
        if (rune.level + levelUp > SDConstants.RuneMaxLevel) return false;
        RuneItem r = getRuneItemById(rune.id);
        int coinWill = getCoinWillImproveCost(rune.level,rune.quality,levelUp);
        if (PlayerData.coin < coinWill) return false;
        //
        foreach(GDERuneData R in PlayerData.RunesOwned)
        {
            if(R.Hashcode == hashcode)
            {
                R.level += levelUp;
                R.attitube = chooseAttiElementToImprove(r,R, levelUp);
                PlayerData.coin -= coinWill;
                PlayerData.Set_RunesOwned();
                return true;
            }
        }
        return false;
    }
    GDEgoddessAttiData chooseAttiElementToImprove(RuneItem r,GDERuneData R,int levelUp)
    {
        GDEgoddessAttiData GA = R.attitube;
        int up = (int)((R.quality + 1) * levelUp);
        if (r.AttiType == GoddessAttiType.agile) GA.agile += up;
        else if (r.AttiType == GoddessAttiType.stamina) GA.stamina += up;
        else if (r.AttiType == GoddessAttiType.recovery) GA.recovery += up;
        else if (r.AttiType == GoddessAttiType.leader) GA.leader += up;
        return GA;
    }
    public int getCoinWillImproveCost(int level, int quality, int levelUp = 1)
    {
        int result = 0;
        for(int i = 0; i < levelUp; i++)
        {
            result += (level + i + 1) 
                * SDConstants.BaseCoinSillImproveCost * (quality + 1);
        }
        return result;
    }
    public bool CheckIfCanComposeToCreateNewRune(GDERuneData rune0,GDERuneData rune1,GDERuneData rune2
        ,out string newRuneId)
    {
        if (rune0 != null && rune1 != null && rune2 != null)
        {
            int[] quals = new int[] { rune0.quality, rune1.quality, rune2.quality };
            int wholeQ = rune0.quality + rune1.quality + rune2.quality;
            RuneItem[] all = Resources.LoadAll<RuneItem>("ScriptableObjects/Items/Runes");
            List<RuneItem> rewards = new List<RuneItem>();
            if(quals.Min() == 0)
            {
                //0---1---2
                int d = 1;
                float r = SDConstants.composeBasicFigure;
                for(int i = 0; i < wholeQ; i++)
                {
                    r *= SDConstants.composeChangeFigure;
                }
                if(UnityEngine.Random.Range(0,1f) >= r)
                {
                    d = 2;
                }
                //
                for (int i = 0; i < all.Length; i++)
                {
                    if (all[i].Quality < d)
                    {
                        rewards.Add(all[i]);
                    }
                }
            }
            else if(quals.Min() == 1)
            {
                //3---4---5
                int d = 1;
                float r = SDConstants.composeBasicFigure;
                for (int i = 0; i < wholeQ-3; i++)
                {
                    r *= SDConstants.composeChangeFigure;
                }
                if (UnityEngine.Random.Range(0, 1f) >= r)
                {
                    d = 2;
                }
                //
                for (int i = 0; i < all.Length; i++)
                {
                    if (all[i].Quality >= d)
                    {
                        rewards.Add(all[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < all.Length; i++)
                {
                    if (all[i].Quality > 1)
                    {
                        rewards.Add(all[i]);
                    }
                }
            }
            RuneItem RI = rewards[UnityEngine.Random.Range(0, rewards.Count)];

            newRuneId = RI.ID;
            return true;
        }
        else
        {
            newRuneId = string.Empty;return false;
        }
    }
    public GoddessAttritube GetGoddessAttiByGDE(GDEgoddessAttiData atti)
    {
        if (atti == null) return GoddessAttritube.zero;
        GoddessAttritube GA = new GoddessAttritube
            (atti.agile,atti.stamina,atti.recovery,atti.leader);
        return GA;
    }
    #endregion
    #region Item_Infor(全物品记录)
    private int add_Item(string id, int amount = 1)
    {
        foreach (GDEItemData I in PlayerData.ItemsOwned)
        {
            if (I.id == id)
            {
                I.num += amount;
                PlayerData.Set_ItemsOwned();
                //
                TriggerManager.Instance.WhenGetItem(id);
                //
                return I.num;
            }
        }
        GDEItemData D = new GDEItemData(GDEItemKeys.Item_MaterialEmpty)
        {
            id = id
            ,
            num = amount
        };
        PlayerData.ItemsOwned.Add(D);
        PlayerData.Set_ItemsOwned();
        //
        TriggerManager.Instance.WhenGetItem(id);
        //
        return D.num;
    }
    private bool consume_Item(string id, out int leftAmonut, int amount = 1)
    {
        foreach (GDEItemData D in PlayerData.ItemsOwned)
        {
            if (D.id == id)
            {
                bool flag;
                if (D.num < amount)
                {
                    D.num = 0;
                    leftAmonut = 0;
                    flag = false;
                }
                else
                {
                    D.num -= amount;
                    leftAmonut = D.num;
                    flag = true;
                }
                if (D.num <= 0)
                {
                    PlayerData.ItemsOwned.Remove(D);
                }
                PlayerData.Set_ItemsOwned();
                //
                if(flag) TriggerManager.Instance.WhenLoseItem(id);
                //
                return flag;
            }
        }
        leftAmonut = 0;
        return false;
    }

    public void AddItem(string id , int amount = 1)
    {
        if (id.Contains("_"))
        {
            string Sign = id.Split('_')[0];
            if (Sign == "H")
            {
                addHero(id);
            }
            else if (Sign == "A")
            {
                addEquip(id);
            }
            else if (Sign == "M")
            {
                addConsumable(id, amount);
            }
        }
        else
        {
            addConsumable(id, amount);
        }
    }

    public bool LoseItem(string id, int amount = 1)
    {
        if (CheckIfHaveItemById(id) && amount > 0)
        {
            if (id.Contains("_"))
            {
                string Sign = id.Split('_')[0];
                if (Sign == "H")
                {
                    for(int i = 0; i < amount; i++)
                    {
                        foreach (var HD in FindAllHerosById(id))
                        {
                            consumeHero(HD.hashCode);break;
                        }
                    }
                    return true;
                }
                else if(Sign == "A")
                {
                    for (int i = 0; i < amount; i++)
                    {
                        foreach (var ED in FindAllArmorsById(id))
                        {
                            consumeEquip(ED.hashcode); break;
                        }
                    }
                    return true; 
                }
                else if(Sign == "M")
                {
                    consumeConsumable(id, out int left, amount);
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckIfHaveItemById(string id)
    {
        foreach (GDEItemData I in PlayerData.ItemsOwned)
        {
            if (I.id == id)
            {
                return true;
            }
        }
        return false;
    }
    public int GetItemAmount(string id)
    {
        foreach (GDEItemData I in PlayerData.ItemsOwned)
        {
            if (I.id == id)
            {
                return I.num;
            }
        }
        return 0;
    }


    public int HeroBattleForce(int hashcode)
    {
        int h = getHeroOriginalBattleForceByHashCode(hashcode);
        for(int i = 0; i < (int)EquipPosition.End; i++)
        {
            h += getEquipBattleForceByHashCode(hashcode);
        }
        return h;
    }
    #endregion
    #region NPC_Infor
    public void AddSlave(string enemyId)
    {
        slaveNum++;
        GDENPCData slave = new GDENPCData(GDEItemKeys.NPC_noone)
        {
            id = enemyId,
            hashcode = slaveNum,
            workingInBuliding=true,
            ShowInBag=true,
            exp=0,
            likability=0,
        };
        PlayerData.NPCList.Add(slave);
        PlayerData.Set_NPCList();
    }
    public GDENPCData GetNPCOwned(int hashcode)
    {
        return PlayerData.NPCList.Find(x => x.hashcode == hashcode);
    }
    #endregion


    public int getNumFromId(string id)
    {
        if (id.Contains("#")) return getInteger(id.Split('#')[1]);
        else return getInteger(id);
    }
    public string rarityString(int quality)
    {
        string n = SDGameManager.T("N");
        if (quality == 1) { n = SDGameManager.T("R"); }
        else if (quality == 2) { n = SDGameManager.T("SR"); }
        else if (quality == 3) { n = SDGameManager.T("SSR"); }
        return n;
    }
    #endregion
}


public class ROHelp
{
    public static List<Dictionary<string,string>> ConvertCsvListToDictWithAttritubes
        (List<string[]> list)
    {
        List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
        if (list.Count <= 1) return results;
        string[] atrs = list[0];//0处存放索引，1处开始存放内容
        for(int i = 1; i < list.Count; i++)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            string[] sing = list[i];
            for(int k = 0; k < atrs.Length; k++)
            {
                d[atrs[k]] = sing[k];
            }
            results.Add(d);
        }
        return results;
    }
    public static SDConstants.AOEType AOE_TYPE(string data)
    {
        for(int i = 0; i < (int)SDConstants.AOEType.End; i++)
        {
            if(data.ToLower() == ((SDConstants.AOEType)i).ToString().ToLower())
            {
                return (SDConstants.AOEType)i;
            }
        }
        return SDConstants.AOEType.None;
        /*
        if (data == "none") return SDConstants.AOEType.None;
        else if (data == "horizontal") return SDConstants.AOEType.Horizontal;
        else if (data == "horizontal1") return SDConstants.AOEType.Horizontal1;
        else if (data == "horizontal2") return SDConstants.AOEType.Horizontal2;
        else if (data == "vertical") return SDConstants.AOEType.Vertical;
        else if (data == "vertical1") return SDConstants.AOEType.Vertical1;
        else if (data == "vertical2") return SDConstants.AOEType.Vertical2;
        else if (data == "random1") return SDConstants.AOEType.Random1;
        else if (data == "random2") return SDConstants.AOEType.Random2;
        else if (data == "random3") return SDConstants.AOEType.Random3;
        else if (data == "continuous2") return SDConstants.AOEType.Continuous2;
        else if (data == "continuous3") return SDConstants.AOEType.Continuous3;
        else if (data == "all") return SDConstants.AOEType.All;
        return SDConstants.AOEType.None;
        */
    }
    public static StateTag STATE_TAG(string s)
    {
        for (int i = 0; i < (int)StateTag.End; i++)
        {
            if (s.ToLower() == ((StateTag)i).ToString().ToLower())
            {
                return (StateTag)i;
            }
        }
        return StateTag.End;
    }
    public static AttributeData AD_TAG(string s)
    {
        for (int i = 0; i < (int)AttributeData.End; i++)
        {
            if (s.ToLower() == ((AttributeData)i).ToString().ToLower())
            {
                return (AttributeData)i;
            }
        }
        return AttributeData.End;
    }
    public static bool CheckStringIsADElseST(string s,out AttributeData ad,out StateTag st)
    {
        ad = AD_TAG(s);st = STATE_TAG(s);
        if (ad == AttributeData.End && st != StateTag.End)
        {
            return false;
        }
        else return true;
    }
    public static EquipPosition EQUIP_POS(string s)
    {
        for(int i = 0; i < (int)EquipPosition.End; i++)
        {
            if(s.ToLower() == ((EquipPosition)i).ToString().ToLower())
            {
                return (EquipPosition)i;
            }
        }
        return EquipPosition.End;
    }
    public static Job getJobByString(string s)
    {
        for (int i = 0; i < (int)Job.End; i++)
        {
            if (s.ToLower() == ((Job)i).ToString().ToLower()) return (Job)i;
        }
        return Job.End;
    }

    public static List<T> RandomList<T>(List<T> originalList)
    {
        int length = originalList.Count;
        List<T> oldList = originalList;
        List<T> list = new List<T>();
        while (list.Count < length)
        {
            int a = UnityEngine.Random.Range(0, oldList.Count);
            if (!list.Contains(oldList[a]))
            {
                list.Add(oldList[a]);
                oldList.RemoveAt(a);
            }
        }
        return list;
    }
}

