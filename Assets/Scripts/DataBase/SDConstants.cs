using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 常量类
/// </summary>
public class SDConstants 
{
    /// <summary>
    /// 我方最大出战角色数
    /// </summary>
    static public int MaxSelfNum = 4;
    /// <summary>
    /// 敌方最大出战角色数
    /// </summary>
    static public int MaxOtherNum = 4;
    /// <summary>
    /// 装备选择页面武器上限
    /// </summary>
    static public int EquipNumPerPage = 16;
    /// <summary>
    /// 背包每页上限
    /// </summary>
    static public int BagMaxNumPerPage = 16;
    /// <summary>
    /// 背包起始格子数
    /// </summary>
    static public int BagStartVolime = 2;
    /// <summary>
    /// 背包最大格子数
    /// </summary>
    public static int BagMaxVolume = 8;
    /// <summary>
    /// 最大队伍数量
    /// </summary>
    public static int MaxBattleTeamNum = 10;

    /// <summary>
    /// 单节包括关卡数
    /// </summary>
    public static int LevelNumPerSection = 5;
    /// <summary>
    /// 出现boss（多节完成）关卡最大公约数
    /// </summary>
    public static int PerBossAppearLevel = 25;
    /// <summary>
    /// 单章包括关卡数
    /// </summary>
    public static int LevelNumPerChapter = 125;

    /// <summary>
    /// 当角色等级达到50级以上时每级所需经验
    /// </summary>
    static public int MaxExpPerLevel = 1250;
    /// <summary>
    /// 角色最小升级用经验值
    /// </summary>
    static public int MinExpPerLevel = 25;
    /// <summary>
    /// 暴击算法内常数
    /// </summary>
    static public int CritConstNumber = 100;
    /// <summary>
    /// 精准闪避算法最小值
    /// </summary>
    static public float AccurEvo_Min = 0.1f;
    /// <summary>
    /// 精准闪避算法内常数
    /// </summary>
    static public int AccurEvo_Const = 25;
    /// <summary>
    /// 最小伤害值
    /// </summary>
    public static int MinDamageCount = 2;
    /// <summary>
    /// 開始拥有特殊加成的品阶
    /// </summary>
    public static int LEVELHaveSpecialBuff = 3;
    /// <summary>
    /// 单个属性最大值常数
    /// </summary>
    public static int RoleAttritubeMaxNum = 50;
    /// <summary>
    /// 疲劳度基础常数
    /// </summary>
    public static int fatigueBasicNum = 50;

    /// <summary>
    /// 装备最大品阶
    /// </summary>
    public static int equipMaxQuality = 5;

    /// <summary>
    /// 材料总类数
    /// </summary>
    public static int materialTypeNum = 100;
    /// <summary>
    /// 玩家存档内容读取间隔
    /// </summary>
    public static float READ_CSV_TIME = 0.1f;
    /// <summary>
    /// 单抽角色消耗钻石数
    /// </summary>
    public static int altarDamondCost = 5;

    /// <summary>
    /// 最大章节数
    /// </summary>
    public static int chapterMaxNum = 30000;
    /// <summary>
    /// 切换昼夜所需小时数
    /// </summary>
    public static int HourToChangeDayNight = 3;
    /// <summary>
    /// 切换昼夜所需回合数
    /// </summary>
    public static int RoundToChangeDayNight = 5;
    /// <summary>
    /// 技能等级上限
    /// </summary>
    public static int SkillMaxGrade = 3;
    /// <summary>
    /// 角色星级上限
    /// </summary>
    public static int UnitMAxStarNum = 5;
    /// <summary>
    /// 最小心形容量
    /// </summary>
    public static int MinHeartVolume = 100;

    /// <summary>
    /// 增强类建筑增强数值
    /// </summary>
    public static float BuffBuildingImproveFigureByLv = 0.1f;
    /// <summary>
    /// 建材和建筑经验换算比
    /// </summary>
    public static int JianCaiConsumeWithGetOneExp = 1;

    public static string HERO_TAG = "HERO";
    public static string ENEMY_TAG = "ENEMY";
    public static string AUTO_TARGET_TAG = "NONE";//无目标或自动技能标签

    /// <summary>
    /// 所有单位可能出现位置编号
    /// </summary>
    public static int[] ALL_UNIT_POS_ARRAY 
        = new int[9] 
        {
            0, 1, 2, 3//基础四角
            , 4, 5//水平居中两角(4-(0,1);5-(2,3))
            , 6, 7//垂直居中两角(6-(0,2);7-(1,3))
            , 8 //中央
        };
    #region 动画名字
    static public string AnimName_IDLE = "idle";
    static public string AnimName_DIE = "die";//0.67f
    static public string AnimName_HURT = "hurt";//0.25f
    static public string AnimName_WALK = "walk";
    static public string AnimName_JUMP = "jump";
    static public string AnimName_ATTACK = "attack01";//0.25 0.33f
    static public string AnimName_ATTACK_ENEMY = "attack";//0.38f
    static public string AnimName_CAST = "cast";//0.5f

    static public float AnimTime_ATTACK = 0.6f;
    static public float AnimTime_CAST = 0.8f;
    #endregion

    public static float HERO_MODEL_RATIO = 0.5f;
    public static float HERO_MODEL_BIG_RATIO = 2.5f;

    /// <summary>
    /// 奖金掉落基础值
    /// </summary>
    static public int goldRatio = 1;

    #region 技能效果文字颜色
    static public Color32 color_grey = new Color32(125, 125, 125, 255);//消耗
    static public Color32 color_orange = new Color32(255, 166, 0, 255);//暴击
    static public Color32 color_blue_dark = new Color32(6, 142, 255, 255);//回蓝
    static public Color32 color_green = new Color32(24, 243, 0, 255);//回血
    static public Color32 color_blue_light = new Color32(5, 234, 247, 255);//闪避
    static public Color32 color_red = new Color32(231, 0, 0, 255);//普通掉血
    static public Color32 color_red_dark = new Color32(180, 25, 25, 255);//失误
    #endregion
    #region 视觉设计类型
    public enum CharacterModelType
    {
        CharacterModelType0,
        CharacterModelType1,
        CharacterModelType2,
        CharacterModelType3,
        CharacterModelType4,
        CharacterModelType5,
        CharacterModelType6,
        CharacterModelType7,
        CharacterModelType8,
        CharacterModelType9,
        CharacterModelType10,
        CharacterModelType11,
        CharacterModelType12,
        CharacterModelType13,
        CharacterModelType14,
        CharacterModelType15,

    }
    public enum CharacterAnimType
    {
        Fighter=0,
        Ranger=1,
        Priest=2,
        Caster=3,
        Enemy=4,
    }
    #endregion


    #region Enum列表
    /// <summary>
    /// 渠道类型
    /// </summary>
    public enum ChannelType
    {
        IOS,
        OFFICIAL,
        TAPTAP,
        GP,
        ADSONLY,
        SHANJI,
    };
    /// <summary>
    /// 分享类型
    /// </summary>
    public enum ShareType
    {
        ALL,
        WeiBo,
        WechatMoments
    };

    /// <summary>
    /// 角色类型
    /// </summary>
    public enum CharacterType
    {
        Hero,
        Enemy,
        Goddess,
        NPC,
        End,
    };

    /// <summary>
    /// 英雄选择类型
    /// </summary>
    public enum HeroSelectType
    {
        All=0,
        Battle,
        Hospital,
             Dispatch
            ,Mission
            ,Recruit
           
            ,UseProp
            ,Train
            ,TrainConsume
            , Promote
            , PromoteConsume
            ,StarUp
            ,Wake
            ,Replace
            
            
    }
    /// <summary>
    /// 角色状态类型
    /// </summary>
    public enum EstateType
    {
        Idle,
        Action,
        Hurt,
        Die
    };
    /// <summary>
    /// 背包类型
    /// </summary>
    public enum BagItemType
    {
        Helmet
        ,Breastplate
        ,Gardebras
        ,Legging
        ,Jewelry
        ,Weapon
            ,Prop
            ,Material
            ,Hero
            ,Keys
    }
    /// <summary>
    /// 通用选择栏类型
    /// </summary>
    public enum ItemType
    {
        Hero=0
            ,
        Equip=1
            ,
        Prop=2
            ,
        Material=3
            ,
        Goddess=4
            ,
        Badge=5
            ,
        Enemy=6
            ,
        Quest=7
            ,
        NPC=8
            ,
        End
    }
    public enum GameType
    {
        Normal,
        Hell,
        Dungeon,
        Hut,
        DimensionBoss,
    }

    public enum deployType
    {
        skill,
        goddess,
        badge,
    }

    public enum timeTaskType
    {
        HOSP,
        FACT,
        CHALL,
    }
    #endregion
    #region 物品Enum列表
    public enum ArmorType
    {
        none,
        light,
        middle,
        heavy,
        end,
    }
    public enum WeaponType
    {
        Sharp=0
            ,Longhandle=1
            ,Blunt=2
            ,assist=3
            ,Enhance=4
    }
    public enum JewelryType
    {
        JT0,JT1,JT2,
    }
    /// <summary>
    /// 材料类型
    /// </summary>
    public enum MaterialType
    {
        exp=0
            ,
        star=1
            ,
        skill=2
            ,
        likability = 3
            ,
        money=4
            ,
        equip_exp=5
            ,
        equip_fix=6
            ,
        equip_reap = 7
            ,
        prop_reap=8
            ,
        agentia
            ,
        magic
            ,
        anqi
            ,
        all
    }
    /// <summary>
    /// 升级用材料道具
    /// </summary>
    public enum StockType
    {
        material
            ,
        hero
            ,
        prop
            ,
        equip
            ,
        all
    }
    public enum StockUseType
    {
        work,detail,sell,
    }
    #endregion
    #region 战斗类enum
    public enum BCType { hp, mp, tp, end }
    public enum AddMpTpType
    {
        Normal,
        PreferMp,
        PreferTp,
        LowMp,
        LowTp,
        PreferBoth,
        LowBoth,
        YearnMp,YearnTp,YearnBoth,
        End,
    }
    public enum AOEType
    {
        None,
        Horizontal,
        Horizontal1, Horizontal2,
        Vertical,
        Vertical1, Vertical2,
        Random1,
        Random2,
        Random3,
        Continuous2,
        Continuous3,
        All,
        End,
    }
    public enum EnemyType
    {
        /// <summary>
        /// 炮灰
        /// </summary>
        fodder
            ,

        /// <summary>
        /// 普通敌人
        /// </summary>
        normal,

        /// <summary>
        /// 精英
        /// </summary>
        elite,

        /// <summary>
        /// 首领
        /// </summary>
        boss,

        /// <summary>
        /// 高阶首领
        /// </summary>
        god,
        end,
    }
    public enum RestrainType
    {
        R0,R1,R2,
    }
    #endregion
    #region 设置类常量
    public const string SchemaKey = "_schema";
    #endregion
}