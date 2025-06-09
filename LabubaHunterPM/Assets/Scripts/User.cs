using System;
using UnityEngine;

[Serializable]
public class User
{
    public int TutorialStep
    {
        get => PlayerPrefs.GetInt("TUTORIAL_STEP");
        set => PlayerPrefs.SetInt("TUTORIAL_STEP", value);
    }
    public int CountOpenLevels
    {
        get => PlayerPrefs.GetInt("COUNT_OPEN_LEVELS", 0);
        set => PlayerPrefs.SetInt("COUNT_OPEN_LEVELS", value);
    }
    public int CountMaxWave
    {
        get => PlayerPrefs.GetInt("COUNT_MAX_WAVE", 0);
        set => PlayerPrefs.SetInt("COUNT_MAX_WAVE", value);
    }
    public int Coins
    {
        get => PlayerPrefs.GetInt("COINS");
        set => PlayerPrefs.SetInt("COINS", value);
    }
    public int Crystals
    {
        get => PlayerPrefs.GetInt("CRYSTALS");
        set => PlayerPrefs.SetInt("CRYSTALS", value);
    }
    public int Rating
    {
        get => PlayerPrefs.GetInt("RATING");
        set => PlayerPrefs.SetInt("RATING", value);
    }
    public int PlayerLevelRifleMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_RIFLE_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_RIFLE_MAN", value);
    }
    public int PlayerLevelShotgunMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_SHOTGUN_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_SHOTGUN_MAN", value);
    }
    public int PlayerLevelSniperMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_SNIPER_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_SNIPER_MAN", value);
    }
    public int PlayerLevelPowerLongMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_POWER_LONG_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_POWER_LONG_MAN", value);
    }
    public int PlayerLevelRifleLongPowerMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_RIFLE_LONG_POWER_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_RIFLE_LONG_POWER_MAN", value);
    }
    public int PlayerLevelRevolverMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_REVOLVER_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_REVOLVER_MAN", value);
    }
    public int PlayerLevelM4Man
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_M4_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_M4_MAN", value);
    }
    public int PlayerLevelPowerMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_POWER_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_POWER_MAN", value);
    }
    public int PlayerLevelRifleLongMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_RIFLE_LONG_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_RIFLE_LONG_MAN", value);
    }
    public int PlayerLevelRifleOrangeMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_RIFLE_ORANGE_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_RIFLE_ORANGE_MAN", value);
    }
    public int PlayerLevelM4ReskinMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_M4_RESKIN_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_M4_RESKIN_MAN", value);
    }
    public int PlayerLevelPowerReskinMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_POWER_RESKIN_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_POWER_RESKIN_MAN", value);
    }
    public int PlayerLevelRifleOrangeReskinMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_RIFLE_ORANGE_RESKIN_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_RIFLE_ORANGE_RESKIN_MAN", value);
    }
    public int PlayerLevelRifleReskinMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_RIFLE_RESKIN_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_RIFLE_RESKIN_MAN", value);
    }
    public int PlayerLevelPowerLongReskinMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_POWER_LONG_RESKIN_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_POWER_LONG_RESKIN_MAN", value);
    }
    public int PlayerLevelSniperReskinMan
    {
        get => PlayerPrefs.GetInt("PLAYER_LEVEL_SNIPER_RESKIN_MAN", 1);
        set => PlayerPrefs.SetInt("PLAYER_LEVEL_SNIPER_RESKIN_MAN", value);
    }
    public bool IsRifleManUnblock
    {
        get => PlayerPrefs.GetInt("IS_RIFLE_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_RIFLE_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsShotgunManUnblock
    {
        get => PlayerPrefs.GetInt("IS_SHOTGUN_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_SHOTGUN_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsSniperManUnblock
    {
        get => PlayerPrefs.GetInt("IS_SNIPER_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_SNIPER_MAN_UNBLOCK", value == true ? 1 : 0);
    }

    public bool IsPowerLongManUnblock
    {
        get => PlayerPrefs.GetInt("IS_POWER_LONG_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_POWER_LONG_MAN_UNBLOCK", value == true ? 1 : 0);
    }

    public bool IsRifleLongPowerManUnblock
    {
        get => PlayerPrefs.GetInt("IS_RIFLE_LONG_POWER_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_RIFLE_LONG_POWER_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsRevolverManUnblock
    {
        get => PlayerPrefs.GetInt("IS_REVOLVER_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_REVOLVER_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsM4ManUnblock
    {
        get => PlayerPrefs.GetInt("IS_M4_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_M4_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsPowerManUnblock
    {
        get => PlayerPrefs.GetInt("IS_POWER_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_POWER_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsRifleLongManUnblock
    {
        get => PlayerPrefs.GetInt("IS_RIFLE_LONG_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_RIFLE_LONG_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsRifleOrangeManUnblock
    {
        get => PlayerPrefs.GetInt("IS_RIFLE_ORANGE_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_RIFLE_ORANGE_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsM4ReskinManUnblock
    {
        get => PlayerPrefs.GetInt("IS_M4_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_M4_RESKIN_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsPowerReskinManUnblock
    {
        get => PlayerPrefs.GetInt("IS_POWER_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_POWER_RESKIN_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsRifleOrangeReskinManUnblock
    {
        get => PlayerPrefs.GetInt("IS_RIFLE_ORANGE_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_RIFLE_ORANGE_RESKIN_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsRifleReskinManUnblock
    {
        get => PlayerPrefs.GetInt("IS_RIFLE_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_RIFLE_RESKIN_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsPowerLongReskinManUnblock
    {
        get => PlayerPrefs.GetInt("IS_POWER_LONG_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_POWER_LONG_RESKIN_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public bool IsSniperReskinManUnblock
    {
        get => PlayerPrefs.GetInt("IS_SNIPER_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("IS_SNIPER_RESKIN_MAN_UNBLOCK", value == true ? 1 : 0);
    }
    public int CurrentCharacter
    {
        get => PlayerPrefs.GetInt("CURRENT_CHARACTER", 0);
        set => PlayerPrefs.SetInt("CURRENT_CHARACTER", value);
    }
    public string Language 
    {   
        get => PlayerPrefs.GetString("LANGUAGE"); 
        set => PlayerPrefs.SetString("LANGUAGE", value); 
    }
    public string Platform
    {
        get => PlayerPrefs.GetString("PLATFORM");
        set => PlayerPrefs.SetString("PLATFORM", value);
    }

    public static UserExtern GetCurrentUser()
    {
        UserExtern currentUser = new UserExtern
        {
            TutorialStep = PlayerPrefs.GetInt("TUTORIAL_STEP", 0),
            CountOpenLevels = PlayerPrefs.GetInt("COUNT_OPEN_LEVELS", 0),
            CountMaxWave = PlayerPrefs.GetInt("COUNT_MAX_WAVE", 0),
            Coins = PlayerPrefs.GetInt("COINS", 0),
            Crystals = PlayerPrefs.GetInt("CRYSTALS", 0),
            Rating = PlayerPrefs.GetInt("RATING", 0),
            //PlayerLevelRevolverMan = PlayerPrefs.GetInt("PLAYER_LEVEL_REVOLVER_MAN", 1),
            //PlayerLevelRifleLongPowerMan = PlayerPrefs.GetInt("PLAYER_LEVEL_RIFLE_LONG_POWER_MAN", 1),
            //PlayerLevelRifleMan = PlayerPrefs.GetInt("PLAYER_LEVEL_RIFLE_MAN", 1),
            //PlayerLevelShotgunMan = PlayerPrefs.GetInt("PLAYER_LEVEL_SHOTGUN_MAN", 1),
            //PlayerLevelSniperMan = PlayerPrefs.GetInt("PLAYER_LEVEL_SNIPER_MAN", 1),
            //PlayerLevelPowerLongMan = PlayerPrefs.GetInt("PLAYER_LEVEL_POWER_LONG_MAN", 1),
            //PlayerLevelM4Man = PlayerPrefs.GetInt("PLAYER_LEVEL_M4_MAN", 1),
            //PlayerLevelPowerMan = PlayerPrefs.GetInt("PLAYER_LEVEL_POWER_MAN", 1),
            //PlayerLevelRifleLongMan = PlayerPrefs.GetInt("PLAYER_LEVEL_RIFLE_LONG_MAN", 1),
            //PlayerLevelRifleOrangeMan = PlayerPrefs.GetInt("PLAYER_LEVEL_RIFLE_ORANGE_MAN", 1),
            //PlayerLevelM4ReskinMan = PlayerPrefs.GetInt("PLAYER_LEVEL_M4_RESKIN_MAN", 1),
            IsRevolverManUnblock = true,
            IsRifleManUnblock = PlayerPrefs.GetInt("IS_RIFLE_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsRifleLongPowerUnblock = PlayerPrefs.GetInt("IS_RIFLE_LONG_POWER_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsShotgunManUnblock = PlayerPrefs.GetInt("IS_SHOTGUN_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsSniperManUnblock = PlayerPrefs.GetInt("IS_SNIPER_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsPowerLongManUnblock = PlayerPrefs.GetInt("IS_POWER_LONG_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsM4ManUnblock = PlayerPrefs.GetInt("IS_M4_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsPowerManUnblock = PlayerPrefs.GetInt("IS_POWER_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsRifleLongManUnblock = PlayerPrefs.GetInt("IS_RIFLE_LONG_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsRifleOrangeManUnblock = PlayerPrefs.GetInt("IS_RIFLE_ORANGE_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsM4ReskinManUnblock = PlayerPrefs.GetInt("IS_M4_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsPowerReskinManUnblock = PlayerPrefs.GetInt("IS_POWER_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsRifleOrangeReskinManUnblock = PlayerPrefs.GetInt("IS_RIFLE_ORANGE_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsRifleReskinManUnblock = PlayerPrefs.GetInt("IS_RIFLE_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsPowerLongReskinManUnblock = PlayerPrefs.GetInt("IS_POWER_LONG_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false,
            IsSniperReskinManUnblock = PlayerPrefs.GetInt("IS_SNIPER_RESKIN_MAN_UNBLOCK", 0) == 1 ? true : false
        };
        return currentUser;       
    }

    public static void SetCurrentUser(UserExtern currentUser)
    {
        PlayerPrefs.SetInt("COUNT_OPEN_LEVELS", currentUser.CountOpenLevels);
        PlayerPrefs.SetInt("COUNT_MAX_WAVE", currentUser.CountMaxWave);
        PlayerPrefs.SetInt("TUTORIAL_STEP", currentUser.TutorialStep);
        PlayerPrefs.SetInt("COINS", currentUser.Coins);
        PlayerPrefs.SetInt("CRYSTALS", currentUser.Crystals);
        PlayerPrefs.SetInt("RATING", currentUser.Rating);
        //SetPlayerLevel("PLAYER_LEVEL_REVOLVER_MAN", currentUser.PlayerLevelRevolverMan);
        //SetPlayerLevel("PLAYER_LEVEL_RIFLE_LONG_POWER_MAN", currentUser.PlayerLevelRifleLongPowerMan);
        //SetPlayerLevel("PLAYER_LEVEL_RIFLE_MAN", currentUser.PlayerLevelRifleMan);
        //SetPlayerLevel("PLAYER_LEVEL_SHOTGUN_MAN", currentUser.PlayerLevelShotgunMan);
        //SetPlayerLevel("PLAYER_LEVEL_SNIPER_MAN", currentUser.PlayerLevelSniperMan);
        //SetPlayerLevel("PLAYER_LEVEL_POWER_LONG_MAN", currentUser.PlayerLevelPowerLongMan);
        //SetPlayerLevel("PLAYER_LEVEL_M4_MAN", currentUser.PlayerLevelM4Man);
        //SetPlayerLevel("PLAYER_LEVEL_POWER_MAN", currentUser.PlayerLevelPowerMan);
        //SetPlayerLevel("PLAYER_LEVEL_RIFLE_LONG_MAN", currentUser.PlayerLevelRifleLongMan);
        //SetPlayerLevel("PLAYER_LEVEL_RIFLE_ORANGE_MAN", currentUser.PlayerLevelRifleOrangeMan);
        //SetPlayerLevel("PLAYER_LEVEL_M4_RESKIN_MAN", currentUser.PlayerLevelRifleOrangeMan);
        PlayerPrefs.SetInt("IS_REVOLVER_MAN_UNBLOCK", 1);
        PlayerPrefs.SetInt("IS_RIFLE_MAN_UNBLOCK", currentUser.IsRifleManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_RIFLE_LONG_POWER_MAN_UNBLOCK", currentUser.IsRifleLongPowerUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_SHOTGUN_MAN_UNBLOCK", currentUser.IsShotgunManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_SNIPER_MAN_UNBLOCK", currentUser.IsSniperManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_POWER_LONG_MAN_UNBLOCK", currentUser.IsPowerLongManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_M4_MAN_UNBLOCK", currentUser.IsM4ManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_POWER_MAN_UNBLOCK", currentUser.IsPowerManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_RIFLE_LONG_MAN_UNBLOCK", currentUser.IsRifleLongManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_RIFLE_ORANGE_MAN_UNBLOCK", currentUser.IsRifleOrangeManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_M4_RESKIN_MAN_UNBLOCK", currentUser.IsM4ReskinManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_POWER_RESKIN_MAN_UNBLOCK", currentUser.IsPowerReskinManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_RIFLE_ORANGE_RESKIN_MAN_UNBLOCK", currentUser.IsRifleOrangeReskinManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_RIFLE_RESKIN_MAN_UNBLOCK", currentUser.IsRifleReskinManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_POWER_LONG_RESKIN_MAN_UNBLOCK", currentUser.IsPowerLongReskinManUnblock == true ? 1 : 0);
        PlayerPrefs.SetInt("IS_SNIPER_RESKIN_MAN_UNBLOCK", currentUser.IsSniperReskinManUnblock == true ? 1 : 0);
    }

    private static void SetCountOpenLevels(int countOpenLevels)
    {
        PlayerPrefs.SetInt("COUNT_OPEN_LEVELS", countOpenLevels == 0 ? 1: countOpenLevels);
    }
    private static void SetPlayerLevel(string key, int playerLevel)
    {
        PlayerPrefs.SetInt(key, playerLevel == 0 ? 1 : playerLevel);        
    }

    public int GetPlayerLevel(int index)
    {
        //switch (index)
        //{
        //case 0: return PlayerLevelRevolverMan;          
        //case 1: return PlayerLevelRifleMan;
        //case 2: return PlayerLevelRifleLongPowerMan;
        //case 3: return PlayerLevelShotgunMan;
        //case 4: return PlayerLevelSniperMan;
        //case 5: return PlayerLevelPowerLongMan;
        //case 6: return PlayerLevelM4Man;
        //case 7: return PlayerLevelPowerMan;
        //case 8: return PlayerLevelRifleLongMan;
        //case 9: return PlayerLevelRifleOrangeMan;
        //case 10: return PlayerLevelM4ReskinMan;
        //default: return 1;
        //}
        return 1;
    }
    public void SetPlayerLevel(int index)
    {
        switch (index)
        {
            case 0: PlayerLevelRevolverMan += 1; break;          
            case 1: PlayerLevelRifleMan += 1; break;
            case 2: PlayerLevelRifleLongPowerMan += 1; break;
            case 3: PlayerLevelShotgunMan += 1; break;
            case 4: PlayerLevelSniperMan += 1; break;
            case 5: PlayerLevelPowerLongMan += 1; break;
            case 6: PlayerLevelM4Man += 1; break;
            case 7: PlayerLevelPowerMan += 1; break;
            case 8: PlayerLevelRifleLongMan += 1; break;
            case 9: PlayerLevelRifleOrangeMan += 1; break;
            case 10: PlayerLevelM4ReskinMan += 1; break;
            default: Debug.Log("Character not found"); break;
        }
#if !UNITY_EDITOR && UNITY_WEBGL
		Progress.Instance.Save();
#endif
    }
    public bool GetUnblockStatus(int index)
    {
        switch (index)
        {
            case 0: return IsRevolverManUnblock;      
            case 1: return IsRifleManUnblock;
            case 2: return IsRifleLongPowerManUnblock;
            case 3: return IsShotgunManUnblock;
            case 4: return IsSniperManUnblock;
            case 5: return IsPowerLongManUnblock;
            case 6: return IsM4ManUnblock;
            case 7: return IsPowerManUnblock;
            case 8: return IsRifleLongManUnblock;
            case 9: return IsRifleOrangeManUnblock;
            case 10: return IsM4ReskinManUnblock;
            case 11: return IsPowerReskinManUnblock;
            case 12: return IsRifleOrangeReskinManUnblock;
            case 13: return IsRifleReskinManUnblock;
            case 14: return IsPowerLongReskinManUnblock;
            case 15: return IsSniperReskinManUnblock;
            default: return true;
        }
    }
    public void SetUnblockStatus(int index)
    {
        switch (index)
        {
            case 0: IsRevolverManUnblock = true; break;
            case 1: IsRifleManUnblock = true; break;
            case 2: IsRifleLongPowerManUnblock = true; break;
            case 3: IsShotgunManUnblock = true; break;
            case 4: IsSniperManUnblock = true; break;
            case 5: IsPowerLongManUnblock = true; break;
            case 6: IsM4ManUnblock = true; break;
            case 7: IsPowerManUnblock = true; break;
            case 8: IsRifleLongManUnblock = true; break;
            case 9: IsRifleOrangeManUnblock = true; break;
            case 10: IsM4ReskinManUnblock = true; break;
            case 11: IsPowerReskinManUnblock = true; break;
            case 12: IsRifleOrangeReskinManUnblock = true; break;
            case 13: IsRifleReskinManUnblock = true; break;
            case 14: IsPowerLongReskinManUnblock = true; break;
            case 15: IsSniperReskinManUnblock = true; break;
            default: Debug.Log("Character not found"); break;
        }
#if !UNITY_EDITOR && UNITY_WEBGL
		Progress.Instance.Save();
#endif
    }
}
