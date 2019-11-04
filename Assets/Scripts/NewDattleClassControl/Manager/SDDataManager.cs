using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using GameDataEditor;
using System;
using System.Net;
using UnityEngine.UI;

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
    public void addUnlockedChapter()
    {
        //
    }
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
                for(int i = 0; i < SDConstants.MaxSelfNum; i++)
                {
                    GDEunitTeamData team = new GDEunitTeamData(GDEItemKeys.unitTeam_emptyHeroTeam)
                    {
                        id = 0,
                        goddess = 0,
                        badge = 0,
                        heroes = new List<int>()
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
        else if(job == Job.Ranger)
        {
            RL_ChangeByList(RL, PlayerData.temple_ranger);
            return RL;
        }
        else if(job == Job.Priest)
        {
            RL_ChangeByList(RL, PlayerData.temple_priest);
            return RL;
        }
        else if(job == Job.Caster)
        {
            RL_ChangeByList(RL, PlayerData.temple_caster);
            return RL;
        }
        return RL;
    }
    void RL_ChangeByList(RoleAttributeList RL , List<int> change, bool IsForAD = true)
    {
        if (IsForAD)
        {
            for(int i = 0; i < (int)AttributeData.End; i++)
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

    #region HeroInfor
    public int getHeroIdByHashcode(int Hashcode)
    {
        foreach(GDEHeroData item in PlayerData.herosOwned)
        {
            if (item.hashCode == Hashcode)
            {
                return item.id;
            }
        }
        return 0;
    }
    public int getBattleForceByHashCode(int Hashcode)
    {
        int battleForce = 0;

        return battleForce;
    }
    /// <summary>
    /// 以10w来区分职业
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int getHeroCareerById(int id)
    {
        int HeroCareer = id / 100000;
        return HeroCareer;
    }
    public GDEHeroData GetHeroOwnedByHashcode(int hashCode)
    {
        foreach(GDEHeroData h in PlayerData.herosOwned)
        {
            if(h.hashCode == hashCode)
            {
                return h;
            }
        }
        return null;
    }
    public ROHeroData getHeroDataByID(int id, int starNumUpGradeTimes)
    {
        //id以10w来区别职业，以2w来区分星级
        //每个职业星级有2w个可能性
        int heroCareer = getHeroCareerById(id);
        //0=fighter; 1=ranger; 2=priest; 3=caster;
        int starNum = id % 100000 / 20000 + getHeroLevelById(id);

        ROHeroData dal = new ROHeroData();
        dal.starNum = starNum + starNumUpGradeTimes;

        dal.ID = id;

        int gender = -1;
        string n = "无名";
        List<Dictionary<string, string>> list = ReadHeroFromCSV(heroCareer);
        foreach (Dictionary<string, string> s in list)
        {
            if (getInteger(s["id"]) == id)
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

        dal.CRI = Rand(dal.starNum, dal.starNum * 2, id, 1);
        dal.CRIDmg = 100 + Rand(dal.starNum * 2, dal.starNum * 3, id, 2);
        dal.DmgReduction = Rand(0, dal.starNum, id, 3) * 5;
        dal.DmgReflection = 0;
        dal.GoldRate = dal.RewardRate = 0;
        dal.BarChartRegendPerTurn = RoleBarChart.zero;
        //
        //下面是分别制定职业及星级的基础数据,基本属性的Rand key一定要一致
        //不然会导致所有相关数据重新随机,加入新的属性时可以添加新的key
        //如果要让属性有相关性,例如攻击力高的人物,必然防御力高,可以用一样的key
        //sol

        //特殊属性，小概率出现极品
        int t = 0;int d = 0;
        if ( Rand(0, 100, id, 11) < 5)
        {
            t += 10;
        }
        if ( Rand(0, 100, id, 12) < 5)
        {
            d += 10;
        }
        if (heroCareer == 0 || heroCareer == 1)
        {
            //dal.RALRate.AT += t;dal.RALRate.AD += d;
        }
        else if(heroCareer == 2 || heroCareer == 3)
        {
            //dal.RALRate.MT += t; dal.RALRate.MD += d;
        }
        return dal;
    }
    public int getHeroStatus(int hashcode)
    {
        int status = 0;
        foreach(GDEHeroData hero in PlayerData.herosOwned)
        {
            if(hero.hashCode == hashcode)
            {
                status = hero.status;
                break;
            }
        }
        return status;
    }
    public void setHeroStatus(int hashcode,int aimStatus)
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
        foreach(GDEHeroData hero in PlayerData.herosOwned)
        {
            if(hero.hashCode == hashcode)
            {
                int id = hero.id;
                int level = getHeroLevelById(id);
                int starupgrade = hero.starNumUpgradeTimes;
                return id % 100000 / 20000 + level + starupgrade;
            }
        }
        return 0;
    }
    public void consumeHero(int hashcode)
    {
        foreach(GDEHeroData h in PlayerData.herosOwned)
        {
            if(!h.locked && h.hashCode == hashcode)
            {
                PlayerData.herosOwned.Remove(h);
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    public bool checkHeroEnableSkill1ByHashcode(int hashcode)
    {
        GDEHeroData hero = getHeroByHashcode(hashcode);
        return checkHeroEnableSkill1ById(hero.id);
    }
    public bool checkHeroEnableSkill1ById(int id)
    {
        int qual = getHeroQualityById(id);
        if (qual < 2) return false;
        else return true;
    }
    #region 根据ID+KEY来产生随机数,一样的种子会出现必然一致的结果
    public static int Rand(int a,int b,int id, int key)
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

    #region HeroTeamInfor
    public GDEunitTeamData getHeroTeamByTeamId(int id)
    {
        foreach(GDEunitTeamData t in PlayerData.heroesTeam)
        {
            if (t.id == id) return t;
        }
        return null;
    }
    public bool checkHeroOwned(int heroId)
    {
        bool flag = false;
        foreach(GDEHeroData item in PlayerData.herosOwned)
        {
            if(item.id == heroId)
            {
                flag = true;break;
            }
        }
        return flag;
    }
    public void setHeroInTeam(int teamId,int index,int hashcode)
    {

    }
    public void setHeroTeam(int teamId,int index, int hashcode)
    {
        GDEunitTeamData Team = getHeroTeamByTeamId(teamId);
        if (Team != null 
            && Team.heroes[index] != 0
            && Team.heroes[index] != hashcode)
        {
            int _hashcode = Team.heroes[index];
            foreach (GDEHeroData hero in PlayerData.herosOwned)
            {
                if (hero.hashCode == _hashcode)
                {
                    hero.status = 0;
                    break;
                }
            }
        }
        foreach(GDEunitTeamData T in PlayerData.heroesTeam)
        {
            if(T.id == teamId)
            {
                T.heroes[index] = hashcode;
            }
        }
        foreach(GDEHeroData hero in PlayerData.herosOwned)
        {
            if(hero.hashCode == hashcode)
            {
                hero.status = 1;
                break;
            }
        }
        PlayerData.Set_herosOwned();
        PlayerData.Set_heroesTeam();
    }
    public List<GDEunitTeamData> getHeroGroup() { return PlayerData.heroesTeam; }
    public void removeFromTeam(int hashcode)
    {
        foreach(GDEHeroData item in PlayerData.herosOwned)
        {
            if(item.hashCode == hashcode)
            {
                if(item.status == 1)//角色在战斗组
                {
                    removeHeroFromBattleTeam(hashcode);
                }
                else if(item.status == 2)
                {

                }
                else if(item.status == 3)
                {

                }
                else if(item.status == 4)
                {

                }
                break;
            }
        }
    }
    public void removeHeroFromBattleTeam(int hashcode)
    {
        for(int i = 0; i < PlayerData.heroesTeam.Count; i++)
        {
            for(int j = 0; j < PlayerData.heroesTeam[i].heroes.Count; j++)
            {
                if (PlayerData.heroesTeam[i].heroes[j] == hashcode)
                {
                    PlayerData.heroesTeam[i].heroes[j] = 0;
                    PlayerData.Set_heroesTeam();
                    break;
                }
            }

        }
        foreach(GDEHeroData hero in PlayerData.herosOwned)
        {
            if (hero.hashCode == hashcode)
            {
                hero.teamIdBelongTo = 0;
                hero.status = 0;break;
            }
        }
    }
    #endregion
    #endregion
    #region MaterialInfor
    public List<GDEAMaterialData> getMaterials()
    {
        return PlayerData.materials;
    }
    public int getMaterialNum(int materialId)
    {
        int sum = 0;
        foreach(GDEAMaterialData m in PlayerData.materials)
        {
            if(m.id == materialId)
            {
                sum = m.num;
                break;
            }
        }
        return sum;
    }
    public void addMaterial(int id, int num = 1)
    {
        bool find = false;
        foreach(GDEAMaterialData m in PlayerData.materials)
        {
            if(m.id == id)
            {
                find = true;
                m.num += num;
                PlayerData.Set_materials();
                break;
            }
        }
        if (!find)
        {
            GDEAMaterialData m = new GDEAMaterialData("Material" + id);
            m.id = id;
            m.num = num;
            PlayerData.materials.Add(m);
            PlayerData.Set_materials();
        }
    }
    public void consumeMaterial(int id, int num = 1)
    {
        foreach(GDEAMaterialData m in PlayerData.materials)
        {
            if(m.id == id)
            {
                m.num -= num;
                if(m.num <= 0)
                {
                    PlayerData.materials.Remove(m);
                }
                PlayerData.Set_materials();
                break;
            }
        }
    }
    public string getMaterialNameById(int id)
    {
        string name = "";
        List<Dictionary<string, string>> itemDatas 
            = SDDataManager.Instance.ReadFromCSV("material");
        for(int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            if(getInteger(s["id"]) == id)
            {
                name = s["name"];
                break;
            }
        }
        return name;
    }
    public string getMaterialTypeById(int id)
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
    public int getMaterialLevelById(int id)
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
    public int getMaterialRarityById(int id)
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

    public int getMaterialFigureById(int id)
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
    #endregion
    #region GoldInfor
    public void AddCoin(int val)
    {
        int number = PlayerData.n_coin;
        PlayerData.n_coin = number + val;

    }
    public int getGoldPerc()
    {
        return getGoldPercOrigin();
    }
    public int getGoldPercOrigin() { return PlayerData.addGoldPerc; }
    public void addGoldPerc(int val) { PlayerData.addGoldPerc += val; }
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
    public int GetMaxPassLevel()
    {
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            return PlayerData.maxPassLevel;
        }
        else if (SDGameManager.Instance.gameType == SDConstants.GameType.Hut)
        {

        }
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Dungeon)
        {
            return PlayerData.maxDurgeonPassLevel;
        }
        return PlayerData.maxPassLevel;
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
    public List<Dictionary<string,string>> ReadFromCSV(string filename)
    {
        if (filename == "achievement")
        {
            if (dataListResult0 == null) { dataListResult0 = ReadDataFromCSV("achievement"); }
            return dataListResult0;
        }else if (filename == "equip")
        {
            if (dataListResult1 == null) { dataListResult1 = ReadDataFromCSV("equip"); };
            return dataListResult1;
        }
        else if(filename == "decoration")
        {
            if(dataListResult2 == null) { dataListResult2 = ReadDataFromCSV("decoration"); }
            return dataListResult2;
        }
        else if(filename == "enemy")
        {
            if(dataListResult3 == null) { dataListResult3 = ReadDataFromCSV("enemy"); }
            return dataListResult3;
        }
        else if (filename == "material")
        {
            if(dataListResult4 == null) { dataListResult4 = ReadDataFromCSV("material"); }
            return dataListResult4;
        }
        else if(filename == "prop")
        {
            if(dataListResult5 == null) { dataListResult5 = ReadDataFromCSV("prop"); }
            return dataListResult5;
        }        
        else if(filename == "heroLvUp")
        {
            if(dataListResult6 == null) { dataListResult6 = ReadDataFromCSV("heroLvUp"); }
            return dataListResult6;
        }
        else if(filename == "nameBefore")
        {
            if(dataListResult7 == null) { dataListResult7 = ReadDataFromCSV("nameBefore"); }
            return dataListResult7;
        }
        else if(filename == "jewelry")
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
        else
        {
            return ReadDataFromCSV(filename);
        }
    }
    public List<Dictionary<string,string>> ReadDataFromCSV(string fileName)
    {
        List<Dictionary<string, string>> xxListResult;
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
    public int getHeroRaceById(int heroId)
    {
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(heroId));
        for( int i = 0;i< list.Count;i++)
        {
            Dictionary<string, string> s = list[i];
            if(getInteger(s["id"]) == heroId)
            {
                return getInteger(s["race"]);
            }
        }
        return 0;
    }
    public int getHeroQualityById(int id)
    {
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(id));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (getInteger(s["id"]) == id)
            {
                return getInteger(s["quality"]);
            }
        }
        return 0;
    }
    public int getHeroGenderById(int id)
    {
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(id));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (getInteger(s["id"]) == id)
            {
                return getInteger(s["gender"]);
            }
        }
        return 0;
    }
    public int getHeroLevelById(int id)
    {
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(id));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (getInteger(s["id"]) == id)
            {
                return getInteger(s["level"]);
            }
        }
        return 0;
    }
    public int getHeroStartStarNumById(int id)
    {
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(id));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (getInteger(s["id"]) == id)
            {
                return getInteger(s["gender"]);
            }
        }
        return 0;
    }
    public GDEHeroData getHeroByHashcode(int hashcode)
    {
        foreach(GDEHeroData hero in PlayerData.herosOwned)
        {
            if(hero.hashCode == hashcode)
            {
                return hero;
            }
        }
        return null;
    }
    public Dictionary<string,string> readHeroFromCSVById(int id)
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
        foreach(Dictionary<string,string> d in all)
        {
            if(d["id"] == id.ToString()) { return d; }
        }
        return null;
    }
    public RoleAttributeList getRALById(int heroId)
    {
        RoleAttributeList RAL = new RoleAttributeList();
        List<Dictionary<string, string>> list = ReadHeroFromCSV(getHeroCareerById(heroId));
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> s = list[i];
            if (getInteger(s["id"]) == heroId)
            {
                RAL = RALByDictionary(s);
            }
        }
        return RAL;
    }
    public RoleAttributeList getRALByUpLv(RoleAttributeList basicRAL,int upLv)
    {
        basicRAL.AddPercAll(20 * upLv);
        return basicRAL;
    }
    public RoleAttributeList RALByDictionary(Dictionary<string,string> s)
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
        RAL.Nature_Def = getInteger(s["nature_def"]);
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

    public void dressEquipment(int heroHashcode, int itemHashcode, bool isSecondJewelry=false)
    {
        int oldEquipHashcode = 0;
        foreach (GDEHeroData hero in SDDataManager.Instance.PlayerData.herosOwned)
        {
            if(hero.hashCode == heroHashcode)
            {
                GDEEquipmentData equip 
                    = SDDataManager.Instance.getEquipmentByHashcode(itemHashcode);
                equip.OwnerHashcode = heroHashcode;
                #region add equip
                int pos = SDDataManager.Instance.getEquipPosById(equip.equipId);
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
        foreach(GDEEquipmentData e in SDDataManager.Instance.PlayerData.equipsOwned)
        {
            if (e.hashcode == itemHashcode)
            {
                e.OwnerHashcode = heroHashcode;
            }
            if(e.hashcode == oldEquipHashcode && oldEquipHashcode > 0)
            {
                e.OwnerHashcode = 0;
            }
        }
        SDDataManager.Instance.PlayerData.Set_equipsOwned();
    }
    public void disrobeEquipment(int heroHashcode, EquipPosition pos, bool isSecondJPos = false)
    {
        int equipHashcode = 0;
        foreach(GDEHeroData hero in SDDataManager.Instance.PlayerData.herosOwned)
        {
            if(hero.hashCode == heroHashcode)
            {
                if(pos == EquipPosition.Head)
                {
                    equipHashcode = hero.equipHelmet.hashcode;
                    hero.equipHelmet = Instance.equipEmpty();
                }
                else if(pos == EquipPosition.Breast)
                {
                    equipHashcode = hero.equipBreastplate.hashcode;
                    hero.equipBreastplate = Instance.equipEmpty();
                }
                else if(pos == EquipPosition.Arm)
                {
                    equipHashcode = hero.equipGardebras.hashcode;
                    hero.equipGardebras = Instance.equipEmpty();
                }
                else if(pos == EquipPosition.Leg)
                {
                    equipHashcode = hero.equipLegging.hashcode;
                    hero.equipLegging = Instance.equipEmpty();
                }
                else if(pos == EquipPosition.Finger)
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
                else if(pos == EquipPosition.Hand)
                {
                    equipHashcode = hero.equipWeapon.hashcode;
                    hero.equipWeapon = Instance.equipEmpty();
                }
                break;
            }
        }
        foreach(GDEEquipmentData e in SDDataManager.Instance.PlayerData.equipsOwned)
        {
            if(e.hashcode == equipHashcode)
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
            if (lv * (lv + 1) / 2 * SDConstants.MinExpPerLevel < exp)
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
        int ral = lv * (lv - 1) / 2 * SDConstants.MinExpPerLevel;
        return ral;
    }
    public void addExpToHeroByHashcode(int hashcode,int exp = 1)
    {
        foreach(GDEHeroData h in PlayerData.herosOwned)
        {
            if (h.hashCode == hashcode) 
            { h.exp += exp;break; }
        }
        PlayerData.Set_herosOwned();
    }
    public void addLikabilityToHeroByHashcode(int hashcode, int likability = 1)
    {
        foreach(GDEHeroData h in PlayerData.herosOwned)
        {
            if(h.hashCode == hashcode)
            {
                h.likability += likability;break;
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
        else if (type == SDConstants.CharacterType.Godness)
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
        else if (type == SDConstants.CharacterType.Godness)
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
        if (h!=null)
        {
            return h.locked;
        }
        return true;
    }
    public int getHeroExpPrice(int hashcode)
    {
        GDEHeroData h = getHeroByHashcode(hashcode);
        int _exp = h.exp;
        int id = h.id;
        ROHeroData dal = getHeroDataByID(id, h.starNumUpgradeTimes);
        int baseExp = (int)(25 * (1 + 0.2f * dal.quality + 0.2f * dal.starNum));
        return baseExp + _exp;
    }
    #endregion
    #region Goddess_Infor
    public string getGoddessSpriteById(int goddessId)
    {
        List<Dictionary<string, string>> list = ReadFromCSV("goddess");
        foreach(Dictionary<string,string> g in list)
        {
            if(g["id"] == goddessId.ToString())
            {
                return g["image"];
            }
        }
        return null;
    }


    #endregion
    #region TimeTask_Infor
    public GDEtimeTaskData getTimeTaskByCharaAndType(int chara,int taskType)
    {
        foreach(GDEtimeTaskData task in PlayerData.TimeTaskList)
        {
            if(task.itemChara == chara && task.taskType == taskType)
            {
                return task;
            }
        }
        return null;
    }
    public GDEtimeTaskData getTimeTaskByTaskId(int taskId)
    {
        foreach(GDEtimeTaskData task in PlayerData.TimeTaskList)
        {
            if (task.taskId == taskId) return task;
        }
        return null;
    }
    #endregion

    #region HeroEquipments_Infor
    public GDEEquipmentData getHeroEquipHelmet(int heroHashcode)
    {
        foreach(GDEHeroData hero in PlayerData.herosOwned)
        {
            if(hero.hashCode == heroHashcode)
            {
                if (hero.equipHelmet != null)
                {
                    bool exit = false;
                    foreach(GDEEquipmentData a in PlayerData.equipsOwned)
                    {
                        if(a.hashcode == hero.equipHelmet.hashcode)
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
                        hero.equipHelmet.equipId = 0;
                        hero.equipHelmet.equipLv = 0;
                        hero.equipHelmet.equipType = 0;
                        hero.equipHelmet.upLv = 0;
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
                        hero.equipBreastplate.equipId = 0;
                        hero.equipBreastplate.equipLv = 0;
                        hero.equipBreastplate.equipType = 0;
                        hero.equipBreastplate.upLv = 0;
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
                        hero.equipGardebras.equipId = 0;
                        hero.equipGardebras.equipLv = 0;
                        hero.equipGardebras.equipType = 0;
                        hero.equipGardebras.upLv = 0;
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
                        hero.equipLegging.equipId = 0;
                        hero.equipLegging.equipLv = 0;
                        hero.equipLegging.equipType = 0;
                        hero.equipLegging.upLv = 0;
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
    public GDEEquipmentData getHeroEquipJewelry(int heroHashcode,bool isSecondJewelry = false)
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
                            hero.jewelry0.equipId = 0;
                            hero.jewelry0.equipLv = 0;
                            hero.jewelry0.equipType = 0;
                            hero.jewelry0.upLv = 0;
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
                            hero.jewelry1.equipId = 0;
                            hero.jewelry1.equipLv = 0;
                            hero.jewelry1.equipType = 0;
                            hero.jewelry1.upLv = 0;
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
                        hero.equipWeapon.equipId = 0;
                        hero.equipWeapon.equipLv = 0;
                        hero.equipWeapon.equipType = 0;
                        hero.equipWeapon.upLv = 0;
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
        (int heroHashcode,EquipPosition pos,bool isSecondJPos = false)
    {
        switch (pos)
        {
            case EquipPosition.Head:return Instance.getHeroEquipHelmet(heroHashcode);
            case EquipPosition.Breast:return Instance.getHeroEquipBreastplate(heroHashcode);
            case EquipPosition.Arm:return Instance.getHeroEquipGardebras(heroHashcode);
            case EquipPosition.Leg:return Instance.getHeroEquipLegging(heroHashcode);
            case EquipPosition.Finger:
                if (!isSecondJPos) return Instance.getHeroEquipJewelry(heroHashcode);
                else return Instance.getHeroEquipJewelry(heroHashcode, true);
            case EquipPosition.Hand:
                return Instance.getHeroWeapon(heroHashcode);
            default:return null;
        }
    }
    public GDEEquipmentData equipEmpty()
    {
        GDEEquipmentData e = new GDEEquipmentData(GDEItemKeys.Equipment_EquipEmpty);
        e.hashcode = e.equipId = e.OwnerHashcode
            = e.equipLv = e.equipType = e.equipBattleForce = e.index
            = e.num = e.upLv = 0;
        return e;
    }
    public GDEEquipmentData getEquipmentByHashcode(int itemHashcode)
    {
        List<GDEEquipmentData> all = SDDataManager.Instance.PlayerData.equipsOwned;
        foreach(GDEEquipmentData e in all)
        {
            if(e.hashcode == itemHashcode)
            {
                return e;
            }
        }
        return null;
    }
    public RoleAttributeList EquipRALByDictionary(Dictionary<string, string> s)
    {
        RoleAttributeList ral = new RoleAttributeList();
        string mainE = s["mainEffect"];string sideE = s["sideEffect"];
        ral.Add(OneAttritube.ReadEffectString(mainE));//添加主属性
        ral.Add(OneAttritube.ReadEffectString(sideE));//添加副属性
        return ral;
    }
    public List<GDEEquipmentData> getAllOwnedEquips()
    {
        return PlayerData.equipsOwned;
    }
    public List<GDEEquipmentData> getOwnedEquipsByPos(EquipPosition Pos ,bool listOrder = false)
    {
        List<GDEEquipmentData> equips = SDDataManager.Instance.PlayerData.equipsOwned;
        List<GDEEquipmentData> outData = new List<GDEEquipmentData>();
        for(int i = 0; i < equips.Count; i++)
        {
            GDEEquipmentData e = equips[i];
            if (SDDataManager.Instance.getEquipPosById(e.equipId) == (int)Pos)
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
            if(Pos == EquipPosition.Finger)
            {
                itemsData = SDDataManager.Instance.ReadFromCSV("jewelry");
            }
            else if(Pos == EquipPosition.Hand)
            {
                itemsData = SDDataManager.Instance.ReadFromCSV("weapon");
            }
            else
            {
                itemsData = SDDataManager.Instance.ReadFromCSV("equip");
            }
            for(int i = 0; i < equips.Count; i++)
            {
                GDEEquipmentData e = equips[i];
                for(int j = 0; j < itemsData.Count; j++)
                {
                    Dictionary<string, string> s = itemsData[i];
                    if(s["id"] == e.equipId.ToString())
                    {
                        arrProperties[i] = Instance.getEquipBattleForceByHashCode(e.hashcode);
                        break;
                    }
                }
            }
            for(int i = 0; i < equips.Count; i++)
            {
                for(int j = i + 1; j < equips.Count; j++)
                {
                    if(arrProperties[i] < arrProperties[j])
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
            for(int i = 0; i < equips.Count; i++)
            {
                equipsListOrder.Add(equips[arrIndex[i]]);
            }
            return equipsListOrder;
        }
        return outData;
    }
    public List<GDEEquipmentData> GetPosOwnedEquipsByCareer(EquipPosition Pos
        , Job career,bool listOrder = false)
    {
        List<GDEEquipmentData> allEquips = SDDataManager.Instance.getOwnedEquipsByPos(Pos,listOrder);
        if(Pos == EquipPosition.Hand)
        {
            List<GDEEquipmentData> AllEs = new List<GDEEquipmentData>();
            for(int i = 0; i < allEquips.Count; i++)
            {
                int id = allEquips[i].equipId;
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

    public int getEquipPosById(int id)
    {
        int p = (id%1000000) / 100000;return p;
    }
    public int getEquipRarityById(int id)
    {
        int p = (id % 100000) / 10000;return p;
    }
    public int getEquipBaiscBattleForceById(int id)
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
                    int mainF = OneAttritube.ReadEffectString(s["mainEffect"])
                        .exportBattleForce();
                    int sideF = OneAttritube.ReadEffectString(s["sideEffect"])
                        .exportBattleForce();
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
                    int mainF = OneAttritube.ReadEffectString(s["mainEffect"])
                        .exportBattleForce();
                    int sideF = OneAttritube.ReadEffectString(s["sideEffect"])
                        .exportBattleForce();
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
                    return list.exportBattleForce();
                }
            }
            return 0;
        }
    }
    public int getEquipBattleForceByHashCode(int itemHashcode)
    {
        GDEEquipmentData equip = SDDataManager.Instance.getEquipmentByHashcode(itemHashcode);
        int basic = SDDataManager.Instance.getEquipBaiscBattleForceById(equip.equipId);
        int flag = basic;



        return flag;
    }
    public List<int> getWeaponEnableCareer(int id)
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
        int id = e!=null?e.equipId:0;
        if (id > 0)
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
    #endregion
    #region residentMovementInfor
    public RoleAttributeList BuffFromRace(RoleAttributeList basic , Race r)
    {
        RoleAttributeList ral = RoleAttributeList.zero;
        //人类
        if(r == Race.Human)
        {

        }
        //精灵在夜间战斗力上升，白天速度降低
        else if(r == Race.Elf)
        {
            if (ResidentMovementData.CurrentDayNightId == 1)//夜间
            {
                ral.AT = basic.read(AttributeData.AT , 5);
                ral.MT = basic.read(AttributeData.MT , 5);
                ral.Speed = basic.read(AttributeData.Speed , 5);
                ral.Accur = basic.read(AttributeData.Accur, 5);
            }
            else
            {
                ral.Speed = - basic.read(AttributeData.Speed, 5);
            }
        }
        //龙裔基础能力周期性增强
        else if(r == Race.Dragonborn)
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
    public List<GDEASkillData> addStartSkillsWhenSummoning(int heroId)
    {
        Job _job = (Job)getHeroCareerById(heroId);
        Race _race = (Race)getHeroRaceById(heroId);
        List<OneSkill> all
            = SkillDetailsList.WriteOneSkillList(_job, _race, getHeroQualityById(heroId));

        int skillNum = UnityEngine.Random.Range(2, 4);
        skillNum = Mathf.Min(skillNum, all.Count);
        List<int> finalList = RandomIntger.NumListReturn(skillNum, all.Count);
        List<GDEASkillData> list = new List<GDEASkillData>();
        for(int i = 0; i < finalList.Count; i++)
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
        for(int i = 0; i < all.Count; i++)
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
            if(!isUsedInOwnedSkill)
            { 
                all[i].isUnlocked = false;
                all[i].lv = -1;
            }
        }
        return all;
    }
    public OneSkill getOwnedSkillById(int skillId ,int heroHashcode)
    {
        GDEHeroData hero = getHeroByHashcode(heroHashcode);
        ROHeroData rod = getHeroDataByID(hero.id, hero.starNumUpgradeTimes);
        Job _job = (Job)getHeroCareerById(hero.id);
        Race _race = (Race)getHeroRaceById(hero.id);
        List<OneSkill> all 
            = SkillDetailsList.WriteOneSkillList( _job, _race, rod.quality);
        OneSkill s = OneSkill.normalAttack;
        foreach(OneSkill Skill in all)
        {
            if(Skill.skillId == skillId) { s = Skill; break; }
        }
        List<GDEASkillData> owns = hero.skillsOwned;
        foreach(GDEASkillData _skill in owns)
        {
            if(_skill.Id == skillId)
            {
                s.lv = _skill.Lv;
                s.isUnlocked = true;
                break;
            }
        }
        return s;
    } 
    public OneSkill getSkillByHeroId(int skillId,int heroId)
    {
        Job _job = (Job)getHeroCareerById(heroId);
        Race _race = (Race)getHeroRaceById(heroId);
        List<OneSkill> all
            = SkillDetailsList.WriteOneSkillList(_job, _race, getHeroQualityById(heroId));
        foreach(OneSkill s in all)
        {
            if(s.skillId == skillId)
            {
                return s;
            }
        }
        return null;
    }
    public int getDeployedSkillId(int skillPos, int heroHashcode)
    {
        GDEHeroData hero = getHeroByHashcode(heroHashcode);
        if (hero!=null)
        {
            if ( skillPos == 0 )
            {
                return hero.skill0Id;
            }
            else if ( skillPos == 1 ) { return hero.skill1Id; }
            else if( skillPos == 2 ) { return hero.skillOmegaId; }
        }
        return 0;
    }
    public bool ifDeployThisSkill(int skillId, int heroHashcode)
    {
        GDEHeroData hero = getHeroByHashcode(heroHashcode);
        if (hero.skill0Id == skillId
            || hero.skill1Id == skillId
            || hero.skillOmegaId == skillId)
        {
            Debug.Log("s0: " + hero.skill0Id + "|s1: " + hero.skill1Id + "|so: " + hero.skillOmegaId + "|--: " + skillId);
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
    public void changeEquipedSkill(int newSkillId,int skillPos,int heroHashcode)
    {
        foreach(GDEHeroData hero in PlayerData.herosOwned)
        {
            if(hero.hashCode == heroHashcode)
            {
                if(skillPos == 0)
                {
                    hero.skill0Id = newSkillId;
                }
                else if(skillPos == 1 && checkHeroEnableSkill1ByHashcode(heroHashcode))
                {
                    hero.skill1Id = newSkillId;
                }
                else if(skillPos == 2)
                {
                    hero.skillOmegaId = newSkillId;
                }
                PlayerData.Set_herosOwned();
                break;
            }
        }
    }
    #endregion
    public int getBForceFromRAL(RoleAttributeList ral)
    {
        int flag = 0;
        flag += (int)(ral.Hp * 1f / 5f)
            + (int)(ral.Mp * 1f / 3.5f)
            + (int)(ral.Tp * 1f / 2.5f)
            + ral.AD + ral.AT + ral.MD + ral.MT
            + (int)(ral.Speed * 4f)
            + ral.Accur + ral.Evo
            + (int)(ral.Crit * 4f);
            //+ (int)(ral.Expect * 0.5f)
            //+ (int)(ral.Taunt * 4f);
        return flag;
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

