using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using I2.Loc;
using GameDataEditor;
using System;
using System.Net;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.U2D;
using Spine.Unity;
using Spine;

public class CreateConfig : MonoBehaviour
{
    /*
    [MenuItem("Tools/CreateConfig")]
    private static void Create()
    {
        CreateArmorConfig();
    }
    private static void CreateArmorConfig()
    {
        List<Dictionary<string, string>> xxListResult;




        #region Equip
        xxListResult = CreateConfig.ReadVSC("equip");
        ArmorRank[] armor_ranks = Resources.LoadAll<ArmorRank>("");
        WeaponRace[] waepon_races = Resources.LoadAll<WeaponRace>("");
        List<SpriteAtlas> allAtlas = Resources.LoadAll<SpriteAtlas>("Sprites/atlas").ToList();
        for (int i = 0; i < xxListResult.Count; i++)
        {
            Dictionary<string, string> Dic = xxListResult[i];
            EquipItem ei = ScriptableObject.CreateInstance<EquipItem>();
            ei.initData(Dic["id"], Dic["name"], Dic["desc"], CreateConfig.StringToInteger(Dic["level"])
                , false, true, true, true, false, Dic["specialStr"]);
            ei.ResistStr = Dic["resistStr"];
            //
            int rankType = CreateConfig.StringToInteger(Dic["type"]);
            foreach (ArmorRank rank in armor_ranks)
            {
                if (rank.Index == rankType)
                {
                    ei.ArmorRank = rank; break;
                }
            }
            if (!string.IsNullOrEmpty(Dic["suit"]))
            {
                ei.SuitBelong = true; ei.SuitId = Dic["suit"];
            }
            //
            ei.RAL = RALBySpecialStr(RoleAttributeList.zero, Dic["specialStr"]);
            ei.RAL = RALByResistStr(ei.RAL, Dic["resistStr"]);
            ei.PassiveEffect = Dic["passiveEffect"];
            //
            string _class = Dic["class"];
            EquipPosition pos = ROHelp.EQUIP_POS(_class);
            ei.EquipPos = pos;
            if (pos == EquipPosition.Hand)
            {
                string weaponClass = Dic["weaponClass"].ToLower();
                foreach (WeaponRace r in waepon_races)
                {
                    if (weaponClass == r.NAME.ToLower())
                    {
                        ei.WeaponRace = r; break;
                    }
                }
            }


            //
            AssetDatabase.CreateAsset(ei, "Assets/Resources/ScriptableObjects/items/Equips/" 
                + (int)ei.EquipPos+"_"
                +ei.LEVEL + "_" + ei.NAME + ".asset");
        }
        #endregion
        return;
        xxListResult = ReadVSC("hero");
        HeroRace[] heroRaces = Resources.LoadAll<HeroRace>("");
        RoleCareer[] careers = Resources.LoadAll<RoleCareer>("");
        for (int i = 0; i < xxListResult.Count; i++)
        {
            Dictionary<string, string> Dic = xxListResult[i];
            HeroInfo mi = ScriptableObject.CreateInstance<HeroInfo>();
            CharacterSex sex = (CharacterSex)(StringToInteger(Dic["gender"]));
            mi.initData(Dic["id"], Dic["name"], Dic["desc"], sex, "");
            int Race = StringToInteger(Dic["race"]);
            foreach (HeroRace race in heroRaces)
            {
                if (race.Index == Race){ mi.Race = race; break;}
            }
            int career = StringToInteger(Dic["career"]);
            foreach (RoleCareer c in careers)
            {
                if (c.Index == career) { mi.Career = c; break; }
            }
            mi.InitRAL(RALByDictionary(Dic));
            mi.WeaponRaceList = GetWeaponTypeList(Dic["weaponClass"]);
            mi.SpecialStr = Dic["specialStr"];
            mi.PersonalSkillList = getSkillsByString(Dic["skill"]);
            //
            AssetDatabase.CreateAsset(mi, "Assets/Resources/ScriptableObjects/heroes/" 
                + career + "_"
                + mi.ID.Substring(mi.ID.Length - 3) + "_"+mi.Name + ".asset");
        }

        return;

        #region enemy
        xxListResult = CreateConfig.ReadVSC("enemy");
        EnemyRank[] eRanks = Resources.LoadAll<EnemyRank>("");
        for (int i = 0; i < xxListResult.Count; i++)
        {
            Dictionary<string, string> Dic = xxListResult[i];
            EnemyInfo mi = ScriptableObject.CreateInstance<EnemyInfo>();
            CharacterSex sex = (CharacterSex)(StringToInteger(Dic["gender"]));
            mi.initData(Dic["id"], Dic["name"], Dic["desc"], sex, "");
            mi.Race = EnemyRaces(Dic["race"]);

            int ranktype = StringToInteger(Dic["rank"]);
            foreach (EnemyRank rank in eRanks)
            {
                if (rank.Index == ranktype) { mi.EnemyRank = rank; break; }
            }
            mi.RAL = RALByDictionary(Dic);
            mi.weight = StringToInteger(Dic["weight"]);
            mi.dropPercent = StringToInteger(Dic["dropRate"]);
            //
            AssetDatabase.CreateAsset(mi, "Assets/Resources/ScriptableObjects/enemies/"
                + mi.ID.Split('#')[1] + mi.Name + ".asset");
        }
        #endregion

        return;

        return;
        #region Consumable 
        xxListResult = CreateConfig.ReadVSC("material");
        for (int i = 0; i < xxListResult.Count; i++)
        {
            Dictionary<string, string> Dic = xxListResult[i];
            SDConstants.MaterialType MT = CreateConfig.getMTypeByString(Dic["materialType"]);
            consumableItem mi = ScriptableObject.CreateInstance<consumableItem>();
            mi.initData(Dic["id"], Dic["name"], Dic["desc"], CreateConfig.StringToInteger(Dic["level"])
                , false, true, true, true, false, Dic["specialStr"], SDConstants.ItemType.Consumable);
            mi.MaterialType = MT;
            mi.buyPrice_coin = StringToInteger(Dic["buyPrice_coin"]);
            mi.buyPrice_diamond = StringToInteger(Dic["buyPrice_diamond"]);
            mi.minLv = StringToInteger(Dic["minLv"]);
            mi.maxLv = StringToInteger(Dic["maxLv"]);
            mi.exchangeType = StringToInteger(Dic["exchangeType"]);
            mi.weight = StringToInteger(Dic["weight"]);
            mi.ConsumableType = (SDConstants.ConsumableType)StringToInteger(Dic["consumableType"]);
            if (Dic.ContainsKey("range"))
            {
                mi.AOE = ROHelp.AOE_TYPE(Dic["range"]);
            }
            if (Dic.ContainsKey("target"))
            {
                mi.AIM = Dic["target"].ToUpper();
            }
            //
            AssetDatabase.CreateAsset(mi, "Assets/Resources/ScriptableObjects/items/Consumables/"
                + mi.ID.Substring(mi.ID.Length - 3) + mi.NAME + ".asset");
        }
        #endregion

    }
    public static List<WeaponRace> GetWeaponTypeList(string L)
    {
        List<WeaponRace> results = new List<WeaponRace>();
        WeaponRace[] weaponRaces = Resources.LoadAll<WeaponRace>("");
        string[] s = L.Split(',');
        for(int i = 0; i < s.Length; i++)
        {
            int index = StringToInteger(s[i]);
            foreach(var r in weaponRaces)
            {
                if(r.Index == index)
                {
                    results.Add(r);break;
                }
            }
        }
        return results;
    }
    public static List<skillInfo> getSkillsByString(string s)
    {
        List<skillInfo> results = new List<skillInfo>();
        string[] ss = s.Split('|');
        skillInfo[] all = Resources.LoadAll<skillInfo>("");
        for(int i = 0; i < ss.Length; i++)
        {
            if (!ss[i].Contains("@"))
            {
                ss[i] = "@" + ss[i];
            }
            foreach(skillInfo info in all)
            {
                if(info.ID == ss[i])
                {
                    results.Add(info);break;
                }
            }
        }
        return results;
    }
    public static List<Dictionary<string,string>> ReadVSC(string csvName)
    {
        List<Dictionary<string, string>> xxListResult
            = new List<Dictionary<string, string>>();
        TextAsset chadata = Resources.Load("Data/" + csvName) as TextAsset;
        List<string[]> cDatas = LocalizationReader.ReadCSV(chadata.text);
        xxListResult = ROHelp.ConvertCsvListToDictWithAttritubes(cDatas);

        return xxListResult;
    }
    public static int StringToInteger(string s)
    {
        int tmp = 0;
        if (s != null && s != "")
        {
            if (int.TryParse(s, out _)) { tmp = int.Parse(s); }
        }
        return tmp;
    }
    public static SDConstants.MaterialType getMTypeByString(string s)
    {
        for(int i = 0; i < (int)SDConstants.MaterialType.end; i++)
        {
            if (s == ((SDConstants.MaterialType)i).ToString()) return (SDConstants.MaterialType)i;
        }
        return SDConstants.MaterialType.end;
    }
    public static RoleAttributeList RALByDictionary(Dictionary<string, string> s)
    {
        RoleAttributeList RAL = new RoleAttributeList();
        RAL.Hp = StringToInteger(s["hp"]);
        RAL.Mp = StringToInteger(s["mp"]);
        RAL.Tp = StringToInteger(s["tp"]);
        RAL.AT = StringToInteger(s["at"]);
        RAL.AD = StringToInteger(s["ad"]);
        RAL.MT = StringToInteger(s["mt"]);
        RAL.MD = StringToInteger(s["md"]);
        RAL.Speed = StringToInteger(s["speed"]);
        RAL.Taunt = StringToInteger(s["taunt"]);
        RAL.Accur = StringToInteger(s["accur"]);
        RAL.Evo = StringToInteger(s["evo"]);
        RAL.Crit = StringToInteger(s["crit"]);
        RAL.Expect = StringToInteger(s["expect"]);

        RAL.Bleed_Def = StringToInteger(s["bleed_def"]);
        RAL.Mind_Def = StringToInteger(s["mind_def"]);
        RAL.Fire_Def = StringToInteger(s["fire_def"]);
        RAL.Frost_Def = StringToInteger(s["frost_def"]);
        RAL.Corrosion_Def = StringToInteger(s["corrosion_def"]);
        RAL.Hush_Def = StringToInteger(s["hush_def"]);
        RAL.Dizzy_Def = StringToInteger(s["dizzy_def"]);
        RAL.Confuse_Def = StringToInteger(s["confuse_def"]);
        return RAL;
    }
    public static RoleAttributeList RALBySpecialStr(RoleAttributeList ral,string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            RoleAttributeList RAL = ral;
            string[] allSs = str.Split('|');
            for(int i = 0; i < allSs.Length; i++)
            {
                string[] Ss = allSs[i].Split(':');
                RAL.Set(StringToInteger(Ss[1]), ROHelp.AD_TAG(Ss[0]));
            }
            return RAL;
        }
        return ral;
    }
    public static RoleAttributeList RALByResistStr(RoleAttributeList ral,string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            RoleAttributeList RAL = ral;
            string[] allSs = str.Split('|');
            for (int i = 0; i < allSs.Length; i++)
            {
                string[] Ss = allSs[i].Split(':');
                RAL.Set(StringToInteger(Ss[1]), ROHelp.STATE_TAG(Ss[0]));
            }
            return RAL;
        }
        return ral;
    }
    public static List<EnemyRace> EnemyRaces(string s)
    {
        List<EnemyRace> results = new List<EnemyRace>();
        EnemyRace[] all = Resources.LoadAll<EnemyRace>("");
        string[] races = s.Split(',');
        for(int i = 0; i < races.Length; i++)
        {
            int r = StringToInteger(races[i]);
            foreach(EnemyRace race in all)
            {
                if(race.Index == r)
                {
                    results.Add(race);break;
                }
            }
        }
        return results;
    }

    [MenuItem("Tools/CreatePool")]
    private static void CreatePool()
    {
        CreatePools();
    }
    private static void CreatePools()
    {
        HeroAltarPool P = ScriptableObject.CreateInstance<HeroAltarPool>();
        HeroInfo[] all = Resources.LoadAll<HeroInfo>
            ("ScriptableObjects/heroes");
        List<HeroInfo> list = all.ToList();
        P.ID = "POOL_H#000";
        P.Name = "全战士卡池";
        P.HeroList = list.FindAll(x=> 
        {
            if (x.Name.Contains("无名") && !x.Name.Contains("("))
            {
                return false;
            }
            else if (x.Career.Career == Job.Fighter)
            {
                if (x.ID == "H_FIGHTER#000050") return false;
                else return true;
            }
            else return false;
        });
        P.HeroList = P.HeroList.FindAll(x=>
        {
            return (x.Sex != CharacterSex.Unknown);
        });
        AssetDatabase.CreateAsset(P, "Assets/Resources/ScriptableObjects/pools/"
                + "AllHeroPool" + ".asset");
    }

    public static int SS_index;
    [MenuItem("Tools/CaptureScreenShot")]
    public static void CaptureScreenShot()
    {
        UnityEngine.ScreenCapture.CaptureScreenshot("ScreenShot"+SS_index+".png", 0);
        SS_index++;
    }

    [MenuItem("Tools/BuildHeroSkeletons")]
    public static void BuildHeroSpines()
    {
        SkeletonDataAsset SDA = Resources.Load<SkeletonDataAsset>("Spine/role01");
        List<HeroInfo> all = Resources.LoadAll<HeroInfo>("ScriptableObjects/heroes").ToList()
            .FindAll(x=>x.Name.Contains("无名"));
        foreach(HeroInfo h in all)
        {
            h.UseSpineData = true;
            h.SpineData.SkeletonData = SDA;
        }
    }
    */
}
