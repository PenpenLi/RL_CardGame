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

    public DateTime OpenTime;
    void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            GDEDataManager.Init("gde_data");
            SetupDatas();
            //gameObject.AddComponent<ResourceManager>();
        }
    }
    private void Start()
    {
        SetupDatas();
        StartCoroutine(IELoadFileAsynchronously());
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
        RoleAttributeList ral = heroData.BasicRAL;
        //
        return ral.BattleForce;
    }
    public int getHeroCareerById(string id)
    {
        if (id.Contains("FIGHTER")) return (int)Job.Fighter;
        else if (id.Contains("RANGER")) return (int)Job.Ranger;
        else if (id.Contains("PRIEST")) return (int)Job.Priest;
        else if (id.Contains("CASTER")) return (int)Job.Caster;

        return 0;
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
        //id以10w来区别职业，以2w来区分星级
        //每个职业星级有2w个可能性
        int heroCareer = getHeroCareerById(id);
        //0=fighter; 1=ranger; 2=priest; 3=caster;
        int starNum = getHeroLevelById(id);

        ROHeroData dal = new ROHeroData();
        dal.starNum = starNum + starNumUpGradeTimes;

        dal.ID = id;

        int gender = -1;
        string n = "无名";
        List<Dictionary<string, string>> list = ReadHeroFromCSV(heroCareer);
        foreach (Dictionary<string, string> s in list)
        {
            if (s["id"] == id)
            {
                gender = getInteger(s["gender"]);
                n = s["name"];
                dal.quality = getInteger(s["quality"]);
                break;
            }
        }
        dal.gender = gender;
        dal.Name = n;
        dal.BasicRAL = getRALById(dal.ID);
        dal.RALRate = RoleAttributeList.zero;

        int _id = id.Contains("#") ? getInteger(id.Split('#')[1]) : getInteger(id);

        dal.CRI = Rand(dal.starNum, dal.starNum * 2, _id, 1);
        dal.CRIDmg = 100 + Rand(dal.starNum * 2, dal.starNum * 3, _id, 2);
        dal.DmgReduction = Rand(0, dal.starNum, _id, 3) * 5;
        dal.DmgReflection = 0;
        dal.GoldRate = dal.RewardRate = 0;
        dal.BarChartRegendPerTurn = RoleBarChart.zero;
        //
        //下面是分别制定职业及星级的基础数据,基本属性的Rand key一定要一致
        //不然会导致所有相关数据重新随机,加入新的属性时可以添加新的key
        //如果要让属性有相关性,例如攻击力高的人物,必然防御力高,可以用一样的key
        //sol

        //特殊属性，小概率出现极品
        int t = 0; int d = 0;
        if (Rand(0, 100, _id, 11) < 5)
        {
            t += 10;
        }
        if (Rand(0, 100, _id, 12) < 5)
        {
            d += 10;
        }
        if (heroCareer == 0 || heroCareer == 1)
        {
            //dal.RALRate.AT += t;dal.RALRate.AD += d;
        }
        else if (heroCareer == 2 || heroCareer == 3)
        {
            //dal.RALRate.MT += t; dal.RALRate.MD += d;
        }
        return dal;
    }
    public ROHeroData getHeroOriginalDataById(string id)
    {
        int heroCareer = getHeroCareerById(id);
        ROHeroData dal = new ROHeroData();
        dal.starNum = getHeroLevelById(id);

        dal.ID = id;

        int gender = -1;
        List<Dictionary<string, string>> list = ReadHeroFromCSV(heroCareer);
        foreach (Dictionary<string, string> s in list)
        {
            if (s["id"] == id)
            {
                gender = getInteger(s["gender"]);
                dal.Name = s["name"];
                dal.quality = getInteger(s["quality"]);
                break;
            }
        }
        dal.gender = gender;
        dal.BasicRAL = getRALById(dal.ID);
        dal.RALRate = RoleAttributeList.zero;

        int _id = id.Contains("#") ? getInteger(id.Split('#')[1]) : getInteger(id);

        dal.CRI = Rand(dal.starNum, dal.starNum * 2, _id, 1);
        dal.CRIDmg = 100 + Rand(dal.starNum * 2, dal.starNum * 3, _id, 2);
        dal.DmgReduction = Rand(0, dal.starNum, _id, 3) * 5;
        dal.DmgReflection = 0;
        dal.GoldRate = dal.RewardRate = 0;
        dal.BarChartRegendPerTurn = RoleBarChart.zero;
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
    public void addHero(string id)
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

        //skill
        List<GDEASkillData> list = SDDataManager.Instance.addStartSkillsWhenSummoning(hero.id);
        for (int i = 0; i < list.Count; i++)
        {
            hero.skillsOwned.Add(list[i]);
            if (SDDataManager.Instance.getSkillByHeroId(list[i].Id, hero.id).isOmegaSkill)
            {
                hero.skillOmegaId = list[i].Id;
            }
            else
            {
                if (hero.skill0Id > 0)
                {
                    if (SDDataManager.Instance.checkHeroEnableSkill1ById(hero.id))
                    {
                        hero.skill1Id = list[i].Id;
                    }
                }
                else
                {
                    hero.skill0Id = list[i].Id;
                }
            }
        }

        //animImg
        int qual = SDDataManager.Instance.getHeroQualityById(id);
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

        if (qual < 4)
        {
            hero.AnimData.isRare = false;

            hero.AnimData.body = getRandomImgAddressForAnim(nameof(hero.AnimData.body));
            hero.AnimData.eyes = getRandomImgAddressForAnim(nameof(hero.AnimData.eyes));
            hero.AnimData.faceother = getRandomImgAddressForAnim(nameof(hero.AnimData.faceother));
            hero.AnimData.hair = getRandomImgAddressForAnim(nameof(hero.AnimData.hair));
            hero.AnimData.handR = getRandomImgAddressForAnim(nameof(hero.AnimData.handR));
            hero.AnimData.head = getRandomImgAddressForAnim(nameof(hero.AnimData.head));
            hero.AnimData.hips = getRandomImgAddressForAnim(nameof(hero.AnimData.hips));
            hero.AnimData.L_hand_a = getRandomImgAddressForAnim(nameof(hero.AnimData.L_hand_a));
            hero.AnimData.L_hand_b = getRandomImgAddressForAnim(nameof(hero.AnimData.L_hand_b));
            hero.AnimData.L_hand_c = getRandomImgAddressForAnim(nameof(hero.AnimData.L_hand_c));
            hero.AnimData.L_jiao = getRandomImgAddressForAnim(nameof(hero.AnimData.L_jiao));
            hero.AnimData.L_leg_a = getRandomImgAddressForAnim(nameof(hero.AnimData.L_leg_a));
            hero.AnimData.L_leg_b = getRandomImgAddressForAnim(nameof(hero.AnimData.L_leg_b));
            hero.AnimData.liuhai = getRandomImgAddressForAnim(nameof(hero.AnimData.liuhai));
            hero.AnimData.R_leg_a = getRandomImgAddressForAnim(nameof(hero.AnimData.R_leg_a));
            hero.AnimData.R_leg_b = getRandomImgAddressForAnim(nameof(hero.AnimData.R_leg_b));
        }


        Instance.PlayerData.herosOwned.Add(hero);
        Instance.PlayerData.Set_herosOwned();
    }
    public bool checkHeroEnableSkill1ByHashcode(int hashcode)
    {
        GDEHeroData hero = getHeroByHashcode(hashcode);
        return checkHeroEnableSkill1ById(hero.id);
    }
    public bool checkHeroEnableSkill1ById(string id)
    {
        int qual = getHeroQualityById(id);
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
                int maxF = (int)((SDConstants.fatigueBasicNum + lv)
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


    public IEnumerable<GDEHeroData> FindAllHerosById(string id,bool onlyUnlocked = true)
    {
        return PlayerData.herosOwned.FindAll(
            x => x.id == id && (onlyUnlocked ? true : !x.locked)).AsEnumerable();
    }
    #endregion
    #region Hero_Anim_Infor
    public string getRandomImgAddressForAnim(string parent)
    {
        //Sprite[] all_body = Resources.LoadAll<Sprite>("Sprites/AnimImage/" + parent);
        //return all_body[UnityEngine.Random.Range(0, all_body.Length-1)].name;

        SpriteAtlas atlas = Resources.Load<SpriteAtlas>("Sprites/AnimImage/" + parent);
        int all = atlas.spriteCount; int c = Mathf.Max(all.ToString().Length, 2);
        int re = UnityEngine.Random.Range(0, all) + 1;
        string reST = string.Format("{0:D" + c + "}", re);
        return parent + reST;
    }
    public Sprite getSpriteFromAtlas(string atlasAddress, string spriteName)
    {
        //Debug.Log("ATLAS==--=="+atlasAddress + "===---===" + spriteName);
        SpriteAtlas atlas = Resources.Load<SpriteAtlas>("Sprites/AnimImage/" + atlasAddress);
        return atlas.GetSprite(spriteName);
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
        if(Team == null)
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
            
            if(H.hashCode == hashcode)
            {
                H.teamIdBelongTo = teamId;H.TeamOrder = index;
                H.status = 1;
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    public void setHeroTeamPos(int hashcode, int newPos)
    {
        int P = newPos % SDConstants.MaxSelfNum;
        foreach(GDEHeroData H in PlayerData.herosOwned)
        {
            if(H.hashCode == hashcode)
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
        foreach(var a in all)
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
    public GDEHeroData getHeroFromTeamByOrder(string teamId,int order)
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
    #region PropInfor
    public int addProp(string id, int num = 1)
    {
        //bool find = false;
        add_Item(id, num);
        foreach (GDEItemData _m in PlayerData.props)
        {
            if (_m.id == id)
            {
                //find = true;
                _m.num += num;
                PlayerData.Set_props();
                //break;
                return _m.num;
            }
        }
        GDEItemData m = new GDEItemData(GDEItemKeys.Item_MaterialEmpty);
        m.id = id;
        m.num = num;
        m.index = PlayerData.props.Count;
        PlayerData.props.Add(m);
        PlayerData.Set_props();
        return m.num;
    }
    public bool consumeProp(string id, out int residue, int num = 1)
    {
        foreach (GDEItemData m in PlayerData.props)
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
                        PlayerData.props.Remove(m);
                    }
                    PlayerData.Set_props();
                    residue = m.num;
                    return true;
                }
            }
        }
        residue = 0;
        return false;
    }
    public ROPropData getPropDataById(string id)
    {
        ROPropData p = new ROPropData();
        List<Dictionary<string, string>> list = ReadFromCSV("prop");
        foreach (Dictionary<string, string> s in list)
        {
            if (s["id"] == id.ToString())
            {
                p = new ROPropData()
                {
                    name = s["name"],
                    id = id,
                    level = getInteger(s["level"]),
                    rarity = getInteger(s["rarity"]),
                    image = s["image"],
                    mode = getInteger(s["mode"]),
                    itemType = SDConstants.ItemType.Prop,
                    canUseInGame = getInteger(s["canUseInGame"]),
                    des = s["des"],
                    specialStr = s["special"],
                    target = s["target"],
                    buyPrice_gold = getInteger(s["buyPrice-gold"]),
                    buyPrice_damond = getInteger(s["buyPrice-diamond"])
                };
            }
        }
        return p;
    }
    public GDEItemData getPropOwned(string id)
    {
        foreach (GDEItemData p in PlayerData.props)
        {
            if (p.id == id) return p;
        }
        return null;
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
        ROPropData p = getPropDataById(id);
        int level = p.level;
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
    public List<GDEItemData> getMaterials()
    {
        return PlayerData.materials;
    }
    public int getMaterialNum(string materialId)
    {
        int sum = 0;
        foreach (GDEItemData m in PlayerData.materials)
        {
            if (m.id == materialId)
            {
                sum = m.num;
                break;
            }
        }
        return sum;
    }
    public int addMaterial(string id, int num = 1)
    {
        add_Item(id, num);
        foreach (GDEItemData M in PlayerData.materials)
        {
            if (M.id == id)
            {
                M.num += num;
                PlayerData.Set_materials();
                return M.num;
            }
        }
        GDEItemData m = new GDEItemData("Material" + id);
        m.id = id;
        m.num = num;
        PlayerData.materials.Add(m);
        PlayerData.Set_materials();
        return m.num;
    }
    public bool consumeMaterial(string id, out int residue, int num = 1)
    {
        foreach (GDEItemData m in PlayerData.materials)
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
                        PlayerData.materials.Remove(m);
                    }
                    PlayerData.Set_materials();
                    residue = m.num;
                    return true;
                }
            }
        }
        residue = 0;
        return false;
    }
    public string getMaterialNameById(string id)
    {
        string name = "";
        List<Dictionary<string, string>> itemDatas
            = SDDataManager.Instance.ReadFromCSV("material");
        for (int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if (s["id"] == id)
            {
                name = s["name"];
                break;
            }
        }
        return name;
    }
    public string getMaterialTypeById(string id)
    {
        List<Dictionary<string, string>> materials
            = SDDataManager.Instance.ReadFromCSV("material");
        foreach (Dictionary<string, string> m in materials)
        {
            if (m["id"] == id.ToString())
            {
                return m["materialType"];
            }
        }
        return null;
    }
    public int getMaterialLevelById(string id)
    {
        List<Dictionary<string, string>> materials
            = SDDataManager.Instance.ReadFromCSV("material");
        foreach (Dictionary<string, string> m in materials)
        {
            if (m["id"] == id.ToString())
            {
                return getInteger(m["level"]);
            }
        }
        return 0;
    }
    public int getMaterialRarityById(string id)
    {
        List<Dictionary<string, string>> materials
            = SDDataManager.Instance.ReadFromCSV("material");
        foreach (Dictionary<string, string> m in materials)
        {
            if (m["id"] == id.ToString())
            {
                return getInteger(m["rarity"]);
            }
        }
        return 0;
    }
    public int getMaterialFigureById(string id)
    {
        List<Dictionary<string, string>> materials
            = SDDataManager.Instance.ReadFromCSV("material");
        foreach (Dictionary<string, string> m in materials)
        {
            if (m["id"] == id.ToString())
            {
                Debug.Log("使用材料id:" + id + "__输出数据:" + getInteger(m["figure"]));
                return getInteger(m["figure"]);
            }
        }
        return 0;
    }
    public int getMaterialWeightById(string id)
    {
        List<Dictionary<string, string>> materials
            = SDDataManager.Instance.ReadFromCSV("material");
        foreach (Dictionary<string, string> m in materials)
        {
            if (m["id"] == id.ToString())
            {
                return getInteger(m["weight"]);
            }
        }
        return 1;
    }
    public ROMaterialData getMaterialDataById(string id)
    {
        ROMaterialData P = new ROMaterialData();
        List<Dictionary<string, string>> list = ReadFromCSV("material");
        foreach (Dictionary<string, string> M in list)
        {
            if (M["id"] == id.ToString())
            {
                P = new ROMaterialData()
                {
                    name = M["name"],
                    id = id,
                    level = getInteger(M["level"]),
                    rarity = getInteger(M["rarity"]),
                    materialType = M["materialType"],
                    image = M["image"],
                    mode = getInteger(M["mode"]),
                    itemType = SDConstants.ItemType.Material,
                    canUseInGame = getInteger(M["canUseInGame"]),
                    des = M["des"],
                    specialStr = M["special"],
                    figure = getInteger(M["figure"]),
                    buyPrice_gold = getInteger(M["buyPrice-gold"]),
                    buyPrice_damond = getInteger(M["buyPrice-diamond"]),
                    exchangeType = getInteger(M["exchangeType"]),
                    maxStack = getInteger(M["maxStack"]),
                    weight = getInteger(M["weight"]),
                    minLv = getInteger(M["minLv"]),
                    maxLv = getInteger(M["maxLv"]),
                };
                return P;
            }
        }
        List<Dictionary<string, string>> propAll = ReadFromCSV("prop");
        foreach (Dictionary<string, string> M in propAll)
        {
            if (M["id"] == id.ToString())
            {
                P = new ROMaterialData()
                {
                    name = M["name"],
                    id = id,
                    level = getInteger(M["level"]),
                    rarity = getInteger(M["rarity"]),
                    materialType = M["materialType"],
                    image = M["image"],
                    mode = getInteger(M["mode"]),
                    kind = M["kind"],
                    itemType = SDConstants.ItemType.Prop,
                    canUseInGame = getInteger(M["canUseInGame"]),
                    des = M["des"],
                    specialStr = M["special"],
                    figure = 0,
                    range = M["range"],
                    target = M["target"],

                    buyPrice_gold = getInteger(M["buyPrice-gold"]),
                    buyPrice_damond = getInteger(M["buyPrice-diamond"]),

                    exchangeType = 0,
                    maxStack = 0,
                    weight = 0,
                    minLv = 0,
                    maxLv = 99,
                };
                return P;
            }
        }
        return P;
    }
    #region MaterialFunction
    public bool UseChestToGetEquip(ROMaterialData A, out List<string> getEquipIds)
    {
        if (SDDataManager.Instance.consumeMaterial(A.id, out int residue))//成功消耗
        {
            Debug.Log("成功消耗" + A.name + " 剩余" + residue);
            List<ROEquipData> allPossibilities = new List<ROEquipData>();
            List<Dictionary<string, string>> all = SDDataManager.Instance.ReadFromCSV("equip");
            if (A.specialStr == "equip")
            {

            }
            else if (A.specialStr == "weapon")
            {
                all = SDDataManager.Instance.ReadFromCSV("weapon");
            }
            else if (A.specialStr == "jewelry")
            {
                all = SDDataManager.Instance.ReadFromCSV("jewelry");
            }
            //
            if (A.level == 0)
            {
                for (int i = 0; i < all.Count; i++)
                {
                    Dictionary<string, string> e = all[i];
                    if (SDDataManager.Instance.getInteger(e["rarity"]) < 2)
                    {
                        ROEquipData n = new ROEquipData()
                        {
                            id = e["id"],
                            rarity = SDDataManager.Instance.getInteger(e["rarity"]),
                        };
                        allPossibilities.Add(n);
                    }
                }
            }
            else if (A.level == 1)
            {
                for (int i = 0; i < all.Count; i++)
                {
                    Dictionary<string, string> e = all[i];
                    if (SDDataManager.Instance.getInteger(e["rarity"]) > 0
                        && SDDataManager.Instance.getInteger(e["rarity"]) <= 2)
                    {
                        ROEquipData n = new ROEquipData()
                        {
                            id = e["id"],
                            rarity = SDDataManager.Instance.getInteger(e["rarity"]),
                        };
                        allPossibilities.Add(n);
                    }
                }
            }
            else if (A.level == 2)
            {
                for (int i = 0; i < all.Count; i++)
                {
                    Dictionary<string, string> e = all[i];
                    if (SDDataManager.Instance.getInteger(e["rarity"]) >= 2)
                    {
                        ROEquipData n = new ROEquipData()
                        {
                            id = e["id"],
                            rarity = SDDataManager.Instance.getInteger(e["rarity"]),
                        };
                        allPossibilities.Add(n);
                    }
                }
            }
            //
            List<float> AllPs = new List<float>();
            for (int i = 0; i < allPossibilities.Count; i++)
            {
                ROEquipData E = allPossibilities[i];
                if (A.level == 0)
                {
                    if (E.rarity == 0) { AllPs.Add(0.85f); }
                    else if (E.rarity == 1) { AllPs.Add(0.15f); }
                }
                else if (A.level == 1)
                {
                    if (E.rarity == 1) { AllPs.Add(0.6f); }
                    else if (E.rarity == 2) { AllPs.Add(0.4f); }
                }
                else if (A.level == 2)
                {
                    if (E.rarity == 2) { AllPs.Add(0.5f); }
                    else { AllPs.Add(0.5f); }
                }
            }
            List<int> result = RandomIntger.NumListReturn(A.figure, AllPs);
            //
            List<string> show = new List<string>();
            for (int i = 0; i < result.Count; i++)
            {
                SDDataManager.Instance.addEquip(allPossibilities[result[i]].id);
                show.Add(allPossibilities[result[i]].id);
            }
            getEquipIds = show;
            return true;
        }
        getEquipIds = new List<string>();
        return false;
    }
    public bool UseChestToGetEquip(string MaterialId, out List<string> getEquipIds)
    {
        return UseChestToGetEquip(getMaterialDataById(MaterialId), out getEquipIds);
    }
    public bool UseChestToGetProp(ROMaterialData A, out List<GDEItemData> getProps)
    {
        if (consumeMaterial(A.id, out int residue))
        {
            Debug.Log("成功消耗" + A.name + " 剩余" + residue);
            List<ROMaterialData> allPossibilities = new List<ROMaterialData>();
            List<Dictionary<string, string>> all = ReadFromCSV("prop");
            if (A.level == 0)
            {
                for (int i = 0; i < all.Count; i++)
                {
                    Dictionary<string, string> e = all[i];
                    if (getInteger(e["rarity"]) <= 2)
                    {
                        ROMaterialData P = new ROMaterialData()
                        {
                            id = e["id"],
                            rarity = SDDataManager.Instance.getInteger(e["rarity"]),
                        };
                        allPossibilities.Add(P);
                    }
                }
            }
            else if (A.level == 1)
            {
                for (int i = 0; i < all.Count; i++)
                {
                    Dictionary<string, string> e = all[i];
                    if (getInteger(e["rarity"]) <= 3 && getInteger(e["rarity"]) > 0)
                    {
                        ROMaterialData P = new ROMaterialData()
                        {
                            id = e["id"],
                            rarity = SDDataManager.Instance.getInteger(e["rarity"]),
                        };
                        allPossibilities.Add(P);
                    }
                }
            }
            List<float> AllPs = new List<float>();
            for (int i = 0; i < allPossibilities.Count; i++)
            {
                ROMaterialData E = allPossibilities[i];
                if (A.level == 0)
                {
                    if (E.rarity == 0) { AllPs.Add(0.4f); }
                    else if (E.rarity == 1) { AllPs.Add(0.35f); }
                    else if (E.rarity == 2) { AllPs.Add(0.25f); }
                }
                else if (A.level == 1)
                {
                    if (E.rarity == 1) { AllPs.Add(0.35f); }
                    else if (E.rarity == 2) { AllPs.Add(0.35f); }
                    else if (E.rarity == 3) { AllPs.Add(0.3f); }
                }
            }

            #region 道具箱获得道具种类随机
            int F = A.figure;
            float[] allF = new float[Mathf.Max(2, F - 1)];
            for (int i = 0; i < allF.Length; i++)
            {
                allF[i] = i * 0.2f + 0.2f;
            }
            F = RandomIntger.Choose(allF) + 2;
            #endregion
            List<int> result = RandomIntger.NumListReturn(F, AllPs);
            //
            List<GDEItemData> show = new List<GDEItemData>();
            for (int i = 0; i < result.Count; i++)
            {
                GDEItemData D = new GDEItemData(GDEItemKeys.Item_MaterialEmpty)
                {
                    id = allPossibilities[result[i]].id,
                    num = 1,
                };
                #region 每种道具获得数量随机
                int R = allPossibilities[result[i]].rarity;
                int N = Mathf.Max(1, 6 - R);
                D.num = Mathf.Max(1, UnityEngine.Random.Range(0, N));
                #endregion
                addProp(allPossibilities[result[i]].id, D.num);
                show.Add(D);
            }
            getProps = show;
            return true;
        }
        getProps = new List<GDEItemData>();
        return false;
    }
    public bool UseChestToGetProp(string MaterialId, out List<GDEItemData> getProps)
    {
        ROMaterialData D = getMaterialDataById(MaterialId);
        return UseChestToGetProp(D, out getProps);
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
    //

    //
    List<Dictionary<string, string>> dataListResultItem;
    public List<Dictionary<string, string>> DataListResultItem
    {
        get
        {
            if (dataListResultItem == null)
            {
                dataListResultItem = new List<Dictionary<string, string>>();
                //HERO
                for (int i = 0; i < (int)Job.End; i++)
                {
                    List<Dictionary<string, string>> L_ = ReadHeroFromCSV(i);
                    for (int j = 0; j < L_.Count; j++)
                    {
                        dataListResultItem.Add(L_[j]);
                    }
                }
                //EQUIP
                List<Dictionary<string, string>> L = ReadFromCSV("equip");
                for (int i = 0; i < L.Count; i++)
                {
                    dataListResultItem.Add(L[i]);
                }
                List<Dictionary<string, string>> L1 = ReadFromCSV("jewelry");
                for (int i = 0; i < L1.Count; i++)
                {
                    dataListResultItem.Add(L1[i]);
                }
                List<Dictionary<string, string>> L2 = ReadFromCSV("weapon");
                for (int i = 0; i < L2.Count; i++)
                {
                    dataListResultItem.Add(L2[i]);
                }
                //PROP
                List<Dictionary<string, string>> L3 = ReadFromCSV("prop");
                for (int i = 0; i < L3.Count; i++)
                {
                    dataListResultItem.Add(L3[i]);
                }
                //MATERIAL
                List<Dictionary<string, string>> L4 = ReadFromCSV("material");
                for (int i = 0; i < L4.Count; i++)
                {
                    dataListResultItem.Add(L4[i]);
                }
                //GODDESS
                List<Dictionary<string, string>> L5 = ReadFromCSV("goddess");
                for (int i = 0; i < L5.Count; i++)
                {
                    dataListResultItem.Add(L5[i]);
                }
                //BADGE
                List<Dictionary<string, string>> L6 = ReadFromCSV("badge");
                for (int i = 0; i < L6.Count; i++)
                {
                    dataListResultItem.Add(L6[i]);
                }
                //ENEMY
                List<Dictionary<string, string>> L7 = ReadFromCSV("enemy");
                for (int i = 0; i < L7.Count; i++)
                {
                    dataListResultItem.Add(L7[i]);
                }
                List<Dictionary<string, string>> L8 = ReadFromCSV("boss");
                for (int i = 0; i < L8.Count; i++)
                {
                    dataListResultItem.Add(L8[i]);
                }
                //QUEST
                //...
            }
            return dataListResultItem;
        }
    }
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
        else if (filename == "prop")
        {
            if (dataListResult5 == null) { dataListResult5 = ReadDataFromCSV("prop"); }
            return dataListResult5;
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
        else if (filename == "jewelry")
        {
            if (dataListResult8 == null) { dataListResult8 = ReadDataFromCSV("jewelry"); }
            return dataListResult8;
        }
        else if (filename == "weapon")
        {
            if (dataListResult9 == null) { dataListResult9 = ReadDataFromCSV("weapon"); }
            return dataListResult9;
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


        hero0List = ReadDataFromCSV("hero0");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        hero1List = ReadDataFromCSV("hero1");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        hero2List = ReadDataFromCSV("hero2");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        hero3List = ReadDataFromCSV("hero3");
        yield return new WaitForSeconds(SDConstants.READ_CSV_TIME);
        //Debug.Log("Dictionary加载完毕");
    }
    public SDConstants.ItemType getItemTypeById(string id)
    {
        int _id = getInteger(id.Split('#')[1]);
        SDConstants.ItemType flag = (SDConstants.ItemType)(_id % 1000000);
        return flag;
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
    #region HeroData_Infor
    List<Dictionary<string, string>> hero0List;
    List<Dictionary<string, string>> hero1List;
    List<Dictionary<string, string>> hero2List;
    List<Dictionary<string, string>> hero3List;
    List<Dictionary<string, string>> hero4List;

    public List<Dictionary<string, string>> ReadHeroFromCSV(int career)
    {
        if (career == 0)
        {
            if (hero0List == null) hero0List = ReadDataFromCSV("hero0");
            return hero0List;
        }
        else if (career == 1)
        {
            if (hero1List == null) hero1List = ReadDataFromCSV("hero1");
            return hero1List;
        }
        else if (career == 2)
        {
            if (hero2List == null) hero2List = ReadDataFromCSV("hero2");
            return hero2List;
        }
        else if (career == 3)
        {
            if (hero3List == null) hero3List = ReadDataFromCSV("hero3");
            return hero3List;
        }
        else
        {
            return ReadDataFromCSV("hero" + career);
        }
    }
    public int getHeroRaceById(string heroId)
    {
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(heroId));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (s["id"] == heroId)
            {
                return getInteger(s["race"]);
            }
        }
        return 0;
    }
    public int getHeroQualityById(string id)
    {
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(id));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (s["id"] == id)
            {
                return getInteger(s["quality"]);
            }
        }
        return 0;
    }
    public int getHeroGenderById(string id)
    {
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(id));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (s["id"] == id)
            {
                return getInteger(s["gender"]);
            }
        }
        return 0;
    }
    public int getHeroLevelById(string id)
    {
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(id));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (s["id"] == id)
            {
                return getInteger(s["level"]);
            }
        }
        return 0;
    }
    public int getHeroSkeletonById(string id)
    {
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(id));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (s["id"] == id)
            {
                return getInteger(s["skeleton"]);
            }
        }
        return 0;
    }
    public int getHeroStartStarNumById(string id)
    {
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(id));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (s["id"] == id)
            {
                return getInteger(s["gender"]);
            }
        }
        return 0;
    }
    public GDEHeroData getHeroByHashcode(int hashcode)
    {
        return GetHeroOwnedByHashcode(hashcode);
    }
    public Dictionary<string, string> readHeroFromCSVById(string id)
    {
        List<Dictionary<string, string>> all = new List<Dictionary<string, string>>();
        for (int i = 0; i < (int)Job.End; i++)
        {
            List<Dictionary<string, string>> list = SDDataManager.Instance.ReadHeroFromCSV(i);
            for (int k = 0; k < list.Count; k++)
            {
                all.Add(list[k]);
            }
        }
        foreach (Dictionary<string, string> d in all)
        {
            if (d["id"] == id.ToString()) { return d; }
        }
        return null;
    }
    public RoleAttributeList getRALById(string heroId)
    {
        RoleAttributeList RAL = new RoleAttributeList();
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(heroId));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (s["id"] == heroId)
            {
                RAL = RALByDictionary(s);
            }
        }
        return RAL;
    }
    public RoleAttributeList getRALByUpLv(RoleAttributeList basicRAL, int upLv)
    {
        basicRAL.AddPercAll(20 * upLv);
        return basicRAL;
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
    public int getLevelByExp(int exp)
    {
        int lv = 0;
        while (lv < 50)
        {
            if (lv * (lv + 1) / 2 * SDConstants.MinExpPerLevel <= exp)
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
    public int getExpByLevel(int lv)
    {
        if (lv < 50)
        {
            int ral = lv * (lv - 1) / 2 * SDConstants.MinExpPerLevel;
            return ral;
        }
        else
        {
            int extraExp = (lv - 50) * 1250;
            return extraExp + 30625;
        }
    }
    public int getExpLengthByLevel(int lv)
    {
        if (lv < 50) return lv * SDConstants.MinExpPerLevel;
        else return 50 * SDConstants.MinExpPerLevel;
    }
    public void addExpToHeroByHashcode(int hashcode, int exp = 1)
    {
        foreach (GDEHeroData h in PlayerData.herosOwned)
        {
            if (h.hashCode == hashcode)
            {
                int starNum = getHeroLevelById(h.id) + h.starNumUpgradeTimes;
                h.exp = HeroOverflowExp(h.exp + exp, starNum);
                PlayerData.Set_herosOwned();
                break; }
        }

    }
    public bool checkHeroExpIfOverflow(int currentExp, int star)
    {
        int limitedLv = 10;
        if (star == 0) limitedLv = 10;
        else if (star == 1) limitedLv = 20;
        else if (star == 2) limitedLv = 30;
        else if (star == 3) limitedLv = 50;
        else if (star == 4) limitedLv = 70;
        else if (star == 5) limitedLv = 100;
        int limitedExp = getExpByLevel(limitedLv);
        if (currentExp >= limitedExp) return true;
        return false;
    }
    public int HeroOverflowExp(int oldExp, int star)
    {
        int limitedLv = 10;
        if (star == 0) limitedLv = 10;
        else if (star == 1) limitedLv = 20;
        else if (star == 2) limitedLv = 30;
        else if (star == 3) limitedLv = 50;
        else if (star == 4) limitedLv = 70;
        else if (star == 5) limitedLv = 100;
        int limitedExp = getExpByLevel(limitedLv);
        if (oldExp >= limitedExp) return limitedExp;
        return oldExp;
    }

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
    public string getCareerStr(int careerIndex, int raceIndex = 0
        , SDConstants.CharacterType type = SDConstants.CharacterType.Hero)
    {
        string s = "";
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
    #region Goddess_Infor
    public string getGoddessSpriteById(string goddessId)
    {
        List<Dictionary<string, string>> list = ReadFromCSV("goddess");
        foreach (Dictionary<string, string> g in list)
        {
            if (g["id"] == goddessId.ToString())
            {
                return g["image"];
            }
        }
        return null;
    }
    public RoGoddessData getGoddessData(GDEgoddessData goddess)
    {
        string id = goddess.id;
        List<Dictionary<string, string>> list = ReadFromCSV("goddess");
        RoGoddessData data = new RoGoddessData();
        foreach (Dictionary<string, string> d in list)
        {
            if (d["id"] == id.ToString())
            {
                data.name = d["name"];
                data.gender = SDDataManager.Instance.getInteger(d["gender"]);
                data.ID = id;
                data.lv = SDDataManager.Instance.getLevelByExp(goddess.exp);
                data.star = SDDataManager.Instance.getInteger(d["level"]) + goddess.star;
                data.volume = goddess.volume;
                data.quality = getInteger(d["quality"]);
                data.sprite = d["image"];
                data.race = getInteger(d["race"]);
                data.mainEffect = d["mainEffect"];
                data.sideEffect = d["sideEffect"];
                data.passiveEffect = d["passiveEffect"];
                data.teamIdsUsed = goddess.UseTeamId;
                break;
            }
        }
        return data;
    }
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
                    taskData.taskType = (int)(mainTimeType * fatigueRate * 0.8f);
                    PlayerData.TimeTaskList.Add(taskData);
                    PlayerData.Set_TimeTaskList();
                }
                else//太轻松不需要治疗
                {

                }
            }

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
                        addMaterial(task.itemId, num);
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
    //public 
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
            List<GDEEquipmentData> equipsListOrder = new List<GDEEquipmentData>();
            int[] arrProperties = new int[equips.Count];
            int[] arrIndex = new int[equips.Count];
            for (int i = 0; i < arrIndex.Length; i++) { arrIndex[i] = i; }
            List<Dictionary<string, string>> itemsData;
            if (Pos == EquipPosition.Finger)
            {
                itemsData = SDDataManager.Instance.ReadFromCSV("jewelry");
            }
            else if (Pos == EquipPosition.Hand)
            {
                itemsData = SDDataManager.Instance.ReadFromCSV("weapon");
            }
            else
            {
                itemsData = SDDataManager.Instance.ReadFromCSV("equip");
            }
            for (int i = 0; i < equips.Count; i++)
            {
                GDEEquipmentData e = equips[i];
                for (int j = 0; j < itemsData.Count; j++)
                {
                    Dictionary<string, string> s = itemsData[i];
                    if (s["id"] == e.id.ToString())
                    {
                        arrProperties[i] = Instance.getEquipBattleForceByHashCode(e.hashcode);
                        break;
                    }
                }
            }
            for (int i = 0; i < equips.Count; i++)
            {
                for (int j = i + 1; j < equips.Count; j++)
                {
                    if (arrProperties[i] < arrProperties[j])
                    {
                        int tmp = arrProperties[i];
                        arrProperties[i] = arrProperties[j];
                        arrProperties[j] = tmp;
                        int tmpIndex = arrIndex[i];
                        arrIndex[i] = arrIndex[j];
                        arrIndex[j] = tmpIndex;
                    }
                }
            }
            for (int i = 0; i < equips.Count; i++)
            {
                equipsListOrder.Add(equips[arrIndex[i]]);
            }
            return equipsListOrder;
        }
        return outData;
    }
    public List<GDEEquipmentData> GetPosOwnedEquipsByCareer(EquipPosition Pos
        , Job career, bool listOrder = false)
    {
        List<GDEEquipmentData> allEquips = SDDataManager.Instance.getOwnedEquipsByPos(Pos, listOrder);
        if (Pos == EquipPosition.Hand)
        {
            List<GDEEquipmentData> AllEs = new List<GDEEquipmentData>();
            for (int i = 0; i < allEquips.Count; i++)
            {
                string id = allEquips[i].id;
                List<int> careerList = SDDataManager.Instance.getWeaponEnableCareer(id);
                if (careerList.Contains((int)career))
                {
                    AllEs.Add(allEquips[i]);
                }
            }
            return AllEs;
        }
        else
        {
            return allEquips;
        }
    }

    public int getEquipPosById(string id)
    {
        if (id.Contains("HELMET")) return (int)EquipPosition.Head;
        else if (id.Contains("BREASTPLATE")) return (int)EquipPosition.Breast;
        else if (id.Contains("GARDEBRAS")) return (int)EquipPosition.Arm;
        else if (id.Contains("LEGGING")) return (int)EquipPosition.Leg;
        else if (id.Contains("JEWELRY")) return (int)EquipPosition.Finger;
        else if (id.Contains("WEAPON")) return (int)EquipPosition.Hand;
        else return (int)EquipPosition.End;
    }
    public int getEquipRarityById(string id)
    {
        int _id = getInteger(id.Split('#')[1]);
        int p = (_id % 100000) / 10000; return p;
    }
    public int getEquipBaiscBattleForceById(string id)
    {
        int equipPos = SDDataManager.Instance.getEquipPosById(id);
        if (equipPos == (int)EquipPosition.Finger)
        {
            List<Dictionary<string, string>> itemsData
                = SDDataManager.Instance.ReadFromCSV("jewelry");
            for (int i = 0; i < itemsData.Count; i++)
            {
                Dictionary<string, string> s = itemsData[i];
                if (s["id"] == id.ToString())
                {
                    int mainF = OneAttritube.ReadEffectString(s["mainEffect"]).BattleForce;
                    int sideF = OneAttritube.ReadEffectString(s["sideEffect"]).BattleForce;
                    //passiveEffect拥有的battlefore读取
                    //然而现在没有设计
                    return mainF + sideF;
                }
            }
            return 0;
        }
        else if (equipPos == (int)EquipPosition.Hand)
        {
            List<Dictionary<string, string>> itemsData
                = SDDataManager.Instance.ReadFromCSV("weapon");
            for (int i = 0; i < itemsData.Count; i++)
            {
                Dictionary<string, string> s = itemsData[i];
                if (s["id"] == id.ToString())
                {
                    int mainF = OneAttritube.ReadEffectString(s["mainEffect"]).BattleForce;
                    int sideF = OneAttritube.ReadEffectString(s["sideEffect"]).BattleForce;
                    //passiveEffect拥有的battlefore读取
                    //然而现在没有设计
                    return mainF + sideF;
                }
            }
            return 0;
        }
        else
        {
            List<Dictionary<string, string>> itemsData
                = SDDataManager.Instance.ReadFromCSV("equip");
            for (int i = 0; i < itemsData.Count; i++)
            {
                Dictionary<string, string> s = itemsData[i];
                if (s["id"] == id.ToString())
                {
                    RoleAttributeList list = SDDataManager.Instance.RALByDictionary(s);
                    return list.BattleForce;
                }
            }
            return 0;
        }
    }
    public int getEquipBattleForceByHashCode(int itemHashcode)
    {
        GDEEquipmentData equip = SDDataManager.Instance.getEquipmentByHashcode(itemHashcode);
        int basic = SDDataManager.Instance.getEquipBaiscBattleForceById(equip.id);
        int level = SDDataManager.Instance.getLevelByExp(equip.exp);
        int flag = (int)(basic * (1 + level * 0.15f));
        return flag;
    }
    public List<int> getWeaponEnableCareer(string id)
    {
        List<int> careerList = new List<int>();
        List<Dictionary<string, string>> itemsData = SDDataManager.Instance.ReadFromCSV("weapon");
        for (int i = 0; i < itemsData.Count; i++)
        {
            Dictionary<string, string> s = itemsData[i];
            if (s["id"] == id.ToString())
            {
                string l = s["career"];
                string[] _l = l.Split('&');
                for (int j = 0; j < _l.Length; j++)
                {
                    int a = SDDataManager.Instance.getInteger(_l[j]);
                    careerList.Add(a);
                }
                return careerList;
            }
        }
        careerList.Add(0);
        return careerList;
    }
    public string getEquipNameByHashcode(int itemHashcode)
    {
        GDEEquipmentData e = SDDataManager.Instance.getEquipmentByHashcode(itemHashcode);
        string id = e.id;
        if (!string.IsNullOrEmpty(id))
        {
            int equipPos = SDDataManager.Instance.getEquipPosById(id);
            List<Dictionary<string, string>> itemsData;
            if (equipPos == 4)
            {
                itemsData = SDDataManager.Instance.ReadFromCSV("jewelry");
            }
            else if (equipPos == 5)
            {
                itemsData = SDDataManager.Instance.ReadDataFromCSV("weapon");
            }
            else
            {
                itemsData = SDDataManager.Instance.ReadFromCSV("equip");
            }
            foreach (Dictionary<string, string> s in itemsData)
            {
                if (s["id"] == id.ToString())
                {
                    return SDGameManager.T(s["name"]);
                }
            }
        }
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
        GDEEquipmentData e = new GDEEquipmentData(GDEItemKeys.Equipment_EquipEmpty)
        {
            id = id,
            hashcode = Instance.equipNum,
            OwnerHashcode = 0,
            locked = false,
            exp = 0,
            num = 1,
            index = 0,
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


    public ROEquipData getEquipDataById(string id)
    {
        List<Dictionary<string, string>> list = ReadFromCSV("equip");
        foreach (Dictionary<string, string> d in list)
        {
            if (d["id"] == id.ToString())
            {
                ROEquipData ROD = new ROEquipData();
                ROD.name = d["name"];
                ROD.id = id;
                ROD.pos = getInteger(d["pos"]);
                //ROD.quality = getInteger(d["quality"]);
                ROD.rarity = getInteger(d["rarity"]);
                ROD.image = d["image"];
                ROD.type = getInteger(d["type"]);
                ROD.passiveEffect = d["passiveEffect"];

                ROD.RAL = new RoleAttributeList()
                {
                    Hp = getInteger(d["hp"]),
                    Mp = getInteger(d["mp"]),
                    Tp = getInteger(d["tp"]),
                    AT = getInteger(d["at"]),
                    AD = getInteger(d["ad"]),
                    MT = getInteger(d["mt"]),
                    MD = getInteger(d["md"]),
                    Speed = getInteger(d["speed"]),
                    Taunt = getInteger(d["taunt"]),
                    Accur = getInteger(d["accur"]),
                    Evo = getInteger(d["evo"]),
                    Crit = getInteger(d["crit"]),
                    Expect = getInteger(d["expect"]),

                    Bleed_Def = getInteger(d["bleed_def"]),
                    Mind_Def = getInteger(d["mind_def"]),
                    Fire_Def = getInteger(d["fire_def"]),
                    Frost_Def = getInteger(d["frost_def"]),
                    Corrosion_Def = getInteger(d["corrosion_def"]),
                    Hush_Def = getInteger(d["hush_def"]),
                    Dizzy_Def = getInteger(d["dizzy_def"]),
                    Confuse_Def = getInteger(d["confuse_def"]),
                };
                ROD.isArmor = true;
                return ROD;
            }
        }
        List<Dictionary<string, string>> list1 = ReadFromCSV("weapon");
        foreach (Dictionary<string, string> d in list1)
        {
            if (d["id"] == id.ToString())
            {
                ROEquipData ROD = new ROEquipData();
                ROD.name = d["name"];
                ROD.id = id;
                ROD.pos = getInteger(d["pos"]);
                //ROD.quality = getInteger(d["quality"]);
                ROD.rarity = getInteger(d["rarity"]);
                ROD.image = d["image"];
                ROD.type = getInteger(d["type"]);
                ROD.passiveEffect = d["passiveEffect"];

                ROD.mainEffect = d["mainEffect"];
                ROD.sideEffect = d["sideEffect"];
                ROD.isArmor = false;
                return ROD;
            }
        }
        List<Dictionary<string, string>> list2 = ReadFromCSV("jewelry");
        foreach (Dictionary<string, string> d in list2)
        {
            if (d["id"] == id.ToString())
            {
                ROEquipData ROD = new ROEquipData();
                ROD.name = d["name"];
                ROD.id = id;
                ROD.pos = getInteger(d["pos"]);
                //ROD.quality = getInteger(d["quality"]);
                ROD.rarity = getInteger(d["rarity"]);
                ROD.image = d["image"];
                ROD.type = getInteger(d["type"]);
                ROD.passiveEffect = d["passiveEffect"];

                ROD.mainEffect = d["mainEffect"];
                ROD.sideEffect = d["sideEffect"];
                ROD.isArmor = false;
                return ROD;
            }
        }
        return null;
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
        Job _job = (Job)getHeroCareerById(heroId);
        Race _race = (Race)getHeroRaceById(heroId);
        List<OneSkill> all
            = SkillDetailsList.WriteOneSkillList(_job, _race, getHeroQualityById(heroId));

        int skillNum = UnityEngine.Random.Range(2, 4);
        skillNum = Mathf.Min(skillNum, all.Count);
        List<int> finalList = RandomIntger.NumListReturn(skillNum, all.Count);
        List<GDEASkillData> list = new List<GDEASkillData>();
        for (int i = 0; i < finalList.Count; i++)
        {
            GDEASkillData s = new GDEASkillData(GDEItemKeys.ASkill_normalAttack)
            {
                Id = all[i].skillId
                ,
                Lv = 0
            };
            list.Add(s);
        }
        return list;
    }
    public List<GDEASkillData> OwnedSkillsByHero(int hashcode)
    {
        GDEHeroData hero = getHeroByHashcode(hashcode);
        List<GDEASkillData> owns = hero.skillsOwned;
        return owns;
    }
    public List<OneSkill> getAllSkillsByHashcode(int heroHashcode)
    {
        GDEHeroData hero = getHeroByHashcode(heroHashcode);
        ROHeroData rod = getHeroDataByID(hero.id, hero.starNumUpgradeTimes);
        Job _job = (Job)getHeroCareerById(hero.id);
        Race _race = (Race)getHeroRaceById(hero.id);
        List<OneSkill> all
            = SkillDetailsList.WriteOneSkillList(_job, _race, rod.quality);
        for (int i = 0; i < all.Count; i++)
        {
            bool isUsedInOwnedSkill = false;
            foreach (GDEASkillData s in OwnedSkillsByHero(heroHashcode))
            {
                if (s.Id == all[i].skillId)
                {
                    isUsedInOwnedSkill = true;
                    all[i].isUnlocked = true;
                    all[i].lv = s.Lv;
                    break;
                }
            }
            if (!isUsedInOwnedSkill)
            {
                all[i].isUnlocked = false;
                all[i].lv = -1;
            }
        }
        return all;
    }
    public OneSkill getOwnedSkillById(int skillId, int heroHashcode)
    {
        if (skillId == 0) return OneSkill.empty;
        GDEHeroData hero = getHeroByHashcode(heroHashcode);
        ROHeroData rod = getHeroDataByID(hero.id, hero.starNumUpgradeTimes);
        Job _job = (Job)getHeroCareerById(hero.id);
        Race _race = (Race)getHeroRaceById(hero.id);
        List<OneSkill> all
            = SkillDetailsList.WriteOneSkillList(_job, _race, rod.quality);
        OneSkill s = OneSkill.normalAttack;
        foreach (OneSkill Skill in all)
        {
            if (Skill.skillId == skillId) { s = Skill; break; }
        }
        List<GDEASkillData> owns = hero.skillsOwned;
        foreach (GDEASkillData _skill in owns)
        {
            if (_skill.Id == skillId)
            {
                s.lv = _skill.Lv;
                s.isUnlocked = true;
                return s;
            }
        }
        return s;
    }
    public OneSkill getSkillByHeroId(int skillId, string heroId)
    {
        Job _job = (Job)getHeroCareerById(heroId);
        Race _race = (Race)getHeroRaceById(heroId);
        List<OneSkill> all
            = SkillDetailsList.WriteOneSkillList(_job, _race, getHeroQualityById(heroId));
        foreach (OneSkill s in all)
        {
            if (s.skillId == skillId)
            {
                return s;
            }
        }
        return null;
    }
    public int getDeployedSkillId(int skillPos, int heroHashcode)
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
        return 0;
    }
    public bool ifDeployThisSkill(int skillId, int heroHashcode)
    {
        GDEHeroData hero = getHeroByHashcode(heroHashcode);
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
    public void UnDeploySkillById(int skillId, int heroHashcode)
    {
        foreach (GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == heroHashcode)
            {
                if (hero.skill0Id == skillId)
                {
                    hero.skill0Id = 0;
                }
                else if (hero.skill1Id == skillId)
                {
                    hero.skill1Id = 0;
                }
                else if (hero.skillOmegaId == skillId)
                {
                    hero.skillOmegaId = 0;
                }
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    public void changeEquipedSkill(int newSkillId, int skillPos, int heroHashcode)
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
    public bool AddExpToBuilding(string id, int exp = 1)
    {
        foreach (GDEtownBuildingData B in PlayerData.buildingsOwned)
        {
            if (B.id == id)
            {
                B.exp += exp;
                PlayerData.Set_buildingsOwned();
            }
        }
        return false;
    }
    #endregion
    #region Enemy_Infor
    public ROEnemyData getEnemyDataById(string id)
    {        
        List<Dictionary<string, string>> AllEs = ReadFromCSV("enemy");
        foreach(Dictionary<string,string> L in AllEs)
        {
            if(L["id"] == id)
            {
                ROEnemyData ED = new ROEnemyData();
                ED.RAL = RALByDictionary(L);
                ED.name = L["name"];
                ED.id = id;
                ED.race = SDDataManager.Instance.getInteger(L["race"]);
                ED.Class = L["class"];
                ED.gender = SDDataManager.Instance.getInteger(L["gender"]);
                ED.skeleton = SDDataManager.Instance.getInteger(L["skeleton"]);
                ED.quality = SDDataManager.Instance.getInteger(L["quality"]);
                //
                ED.weight = SDDataManager.Instance.getInteger(L["weight"]);
                ED.appearWeight = SDDataManager.Instance.getInteger(L["appearWeight"]);
                ED.dropRate = SDDataManager.Instance.getInteger(L["dropRate"]);
                ED.dropItems = L["dropItems"];
                //
                return ED;
            }
        }
        return null;
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
                if (id.Contains("PROP"))
                {
                    addProp(id, amount);
                }
                else if (id.Contains("MATERIAL"))
                {
                    addMaterial(id, amount);
                }
            }
        }
        else
        {
            addMaterial(id, amount);
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
                    if (id.Contains("PROP"))
                    {
                        consumeProp(id, out int left, amount);
                        return true;
                    }
                    else if (id.Contains("MATERIAL"))
                    {
                        consumeMaterial(id, out int left, amount);
                        return true;
                    }
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

    #endregion
    public SDConstants.AOEType AOE_TYPE(string data)
    {
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
    }
    /// <summary>
    /// 生成受百分比函数影响的数据
    /// </summary>
    /// <param name="basicData">源数据</param>
    /// <param name="pc">百分比函数(+-()%)</param>
    /// <returns></returns>
    public int FigureAByPc(int basicData, int pc)
    {
        return (int)(basicData * AllRandomSetClass.SimplePercentToDecimal(100 + pc));
    }

    public int getNumFromId(string id)
    {
        if (id.Contains("#")) return getInteger(id.Split('#')[1]);
        else return getInteger(id);
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
}

