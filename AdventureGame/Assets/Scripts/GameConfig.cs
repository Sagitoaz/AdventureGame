using UnityEngine;

public class GameConfig
{
    // Input
    public static string HORIZONTAL_INPUT = "Horizontal";
    // Animator
    public static string MOVING_STATE = "MOVING";
    public static string IS_ATTACK = "IS_ATK";
    public static string ATTACK_STATE = "ATK_INDEX";
    public static string ATTACK_QUEUE = "COMBO_QUEUE";
    public static string JUMP_TRIGGER = "JUMP";
    public static string FALLING_STATE = "FALL";
    public static string IS_GROUND = "IS_GROUND";
    public static string AIR_ATK_TRIGGER = "AIR_ATK";
    public static string DASH_ATK_TRIGGER = "DASH_ATK";
    public static string DEF_TRIGGER = "DEF";
    public static string U_ATK_TRIGGER = "U_ATK";
    public static string K_U_ATK_TRIGGER = "K_U_ATK";
    public static string ULTIMATE_TRIGGER = "ULTIMATE";
    public static string HIT_TRIGGER = "HIT";
    public static string DEAD_TRIGGER = "DEAD";

    public static string BOSS_CHASE_BOOL = "Chase";

    public static string BOSS_DASH_ATK_TRIGGER = "DashAtk";

    public static string BOSS_ATKJJ1_TRIGGER = "AtkJJ1";

    public static string BOSS_ATKJJ2_BOOL = "AtkJJ2";

    public static string BOSS_ATKJK1_TRIGGER = "AtkJK1";

    public static string BOSS_ATKJK2_BOOL = "AtkJK2";

    public static string BOSS_ATKJK3_BOOL = "AtkJK3";

    public static string BOSS_KICK_TRIGGER = "Kick";

    public static string BOSS_HURT_TRIGGER = "BeHurt";

    public static string BOSS_DIE_TRIGGER = "Die";
}
