using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

[System.Serializable]
public class BattleRoleManager : MonoBehaviour
{
    //角色存储为json

    /// <summary>
    /// 对应玩家操纵小队
    /// </summary>
    RoleEntry _RE_Player = new RoleEntry();
    RoleEntry RE_Player
    {
        get { return _RE_Player; }
        set { _RE_Player = value; }
    }
    [HideInInspector]
    public List<Role> PlayersList = new List<Role>();

    /// <summary>
    /// 对应怪物小队
    /// </summary>
    RoleEntry _RE_Monster = new RoleEntry();
    RoleEntry RE_Monster
    {
        get { return _RE_Monster; }
        set { _RE_Monster = value; }
    }
    [HideInInspector]
    public List<Role> MonstersList = new List<Role>();

    public string RoleListPath(bool IsPlayers)
    {
        if (IsPlayers) { return "Jsons/AllBattlePlayers"; }
        else { return "Jsons/AllBattleMonsters"; }
    }

    /// <summary>
    /// 脚本自身设置
    /// </summary>
    private static BattleRoleManager _instance;
    public static BattleRoleManager instance
    {
        get
        {
            if (_instance == null) _instance = new BattleRoleManager();
            return _instance;
        }
    }
    private void Start()
    {
        CheckRoleDataStart();
    }
    void CheckRoleDataStart()
    {
        if (LoadFromFile(true) != null)
        {
            RE_Player = LoadFromFile(true);
            //Debug.Log(RE_Player.RoleArraies.Length);
        }
        if (LoadFromFile(false) != null)
        {
            RE_Monster = LoadFromFile(false);
            //Debug.Log(RE_Monster.RoleArraies.Length);
        }
    }

    RoleEntry LoadFromFile(bool isplayer)
    {
        if(!File.Exists(Application.dataPath + "/Resources/" + RoleListPath(isplayer))) { return null; }
        StreamReader sr = new StreamReader(Application.dataPath + "/Resources/" + RoleListPath(isplayer));
        if(sr == null) { return null; }
        string json = sr.ReadToEnd();
        //Debug.Log(json);
        if (json.Length>0)
        {
            return JsonUtility.FromJson<RoleEntry>(json);
        }
        return null;
    }

    public void ReadData()
    {
        Debug.Log("Load");

        ReadFromText(true, RE_Player, PlayersList);

        ReadFromText(false, RE_Monster, MonstersList);

        //传输信息至战斗界面
        GetComponent<SimpleBattleScript>().SimpleSetRoles();
        GetComponent<SimpleBattleScript>().StartBattleMenu = true;
    }
    void ReadFromText(bool isPlayer,RoleEntry content,List<Role> AimList)
    {
        AimList.Clear();
        if (LoadFromFile(isPlayer) != null)
        {
            content = LoadFromFile(isPlayer);
        }
        else
        {
            Debug.Log("未能读取Json数据");
        }
        //AimList.Clear();
        for(int i = 0; i < content.RoleArraies.Length; i++)
        {
            AimList.Add(content.RoleArraies[i]);
            Debug.Log(AimList[i].name+" "+ AimList[i].hp+" "+ AimList[i].sp+" "+ AimList[i].MaxTp+" "+ AimList[i].Speed);
        }
    }

    public void SaveData()
    {
        Debug.Log("Save");
        //玩家小队
        Role[] a = new Role[3];
        a[0] = new Role(1, "流浪战士保济丸", 200, 50, 100, 25, 25, 10, 10, 25, 1);
        a[1] = new Role(2, "赏金刺客青草膏", 100, 50, 100, 25, 15, 10, 10, 50, 1);
        a[2] = new Role(3, "见习巫师感冒灵", 125, 100, 100, 5, 5, 25, 20, 25, 1);
        RE_Player.RoleArraies = a;
        WriteToText(true, RE_Player);
        //怪物小队
        Role[] b = new Role[3];
        b[0] = new Role(-1, "哥布林", 125, 50, 100, 15, 10, 5, 5, 55, 1);
        b[1] = new Role(-2, "哥布林术士", 150, 150, 100, 10, 10, 15, 15, 55, 1);
        b[2] = new Role(-3, "兽人", 400, 50, 100, 30, 40, 5, 5, 20, 1);
        RE_Monster.RoleArraies = b;
        WriteToText(false, RE_Monster);
    }
    void WriteToText(bool isPlayers,RoleEntry content)
    {
        //TextAsset ta = Resources.Load<TextAsset>(RoleListPath(isPlayers));
        string s = JsonUtility.ToJson(content);
        Debug.Log(s);
        if(!File.Exists(Application.dataPath + "/Resources/" + RoleListPath(isPlayers)))
        {
            Debug.Log("地址不存在");
        }
        else
        {
            File.WriteAllText(Application.dataPath + "/Resources/" + RoleListPath(isPlayers), s, Encoding.UTF8);
            StreamReader sr = new StreamReader(Application.dataPath + "/Resources/" + RoleListPath(isPlayers));
            string json = sr.ReadToEnd();
            Debug.Log(json);
        }
        
    }

}
