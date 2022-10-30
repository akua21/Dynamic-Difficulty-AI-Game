using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviourEnum { player, bot, botInputs, botGenetic };
public enum CharacterState { idle, attack, block, dash, hitted, recoil };
public enum BotMovementState { idle, follow, wander };

public enum GameMode { none, genetic, classical, manyEnemies};

public class GameInputs
{
    public int PlayerHP { get; set; }
    public int BotHP { get; set; }
    public float DistanceToPlayer { get; set; }
    public Vector2 DirectionToPlayer { get; set; }
    public CharacterState PlayerState { get; set; }
    public CharacterState BotState { get; set; }
    public float PlayerRotation { get; set; }
    public float BotRotation { get; set; }

    public GameInputs()
    {
        PlayerHP = 0;
        BotHP = 0;
        DistanceToPlayer = 0f;
        DirectionToPlayer = new Vector2(0, 0);
        PlayerState = CharacterState.idle;
        BotState = CharacterState.idle;
        PlayerRotation = 0f;
        BotRotation = 0f;
    }

    public GameInputs(
        int playerHP,
        int botHP,
        float distanceToPlayer,
        Vector2 directionToPlayer,
        CharacterState playerState,
        CharacterState botState,
        float playerRotation,
        float botRotation
    )
    {
        PlayerHP = playerHP;
        BotHP = botHP;
        DistanceToPlayer = distanceToPlayer;
        DirectionToPlayer = directionToPlayer;
        PlayerState = playerState;
        BotState = botState;
        PlayerRotation = playerRotation;
        BotRotation = botRotation;
    }

}

public class MatchController : MonoBehaviour
{

    public static Character Player;
    public static Character Bot;

    private static int defaultSpeed;
    private static int defaultAtackStrength;
    private static int defaultBlockStrength;
    private static float defaultDashStrength;
    private static float defaultStaminaRecoverRate;


    public static int PlayerLives;
    public static int BotLives;

    public static GameMode CurrentGameMode;

    private static int _currentDifficulty;
    public static int CurrentDifficulty {
        get {
            return _currentDifficulty;
        }
        set {
            Debug.Log(CurrentGameMode);
            switch (CurrentGameMode)
            {
                case GameMode.genetic:
                    GeneticDifficultyControl(value);
                    break;
                case GameMode.classical:
                    ClassicalDifficultyControl(value);
                    break;
                case GameMode.manyEnemies:
                    break;
                default:
                    break;
            }
        }
    } 
    public static float[] CurrentGenes;

    public static readonly float[] BOTEASY = new float[8] {
        0.07f,
        0.03f,
        0.10f,
        0.10f,
        0.02f,
        0.02f,
        0.02f,
        0.01f
    };

    public static readonly float[] BOTMEDIUM = new float[8] {
        0.09f,
        0.03f,
        0.08f,
        0.12f,
        0.04f,
        0.04f,
        0.04f,
        0.02f
    };

    public static readonly float[] BOTHARD = new float[8] {
        0.12f,
        0.02f,
        0.05f,
        0.15f,
        0.06f,
        0.06f,
        0.06f,
        0.02f
    };

    public static bool WarmUp;

    public static float MatchTime;
    public static bool PlayerWon;
    public static int WinnerHP;

    public static string PlayerID;

    public static int NumberLives;

    public static void SetPlayer(Character player)
    {
        Player = player;
    }

    public static void SetBot(Character bot)
    {
        Bot = bot;
        defaultSpeed = Bot.Speed;
        defaultAtackStrength = Bot.CharacterWeapon.Strength;
        defaultBlockStrength = Bot.CharacterShield.Strength;
        defaultDashStrength = Bot.DashStrength;
        defaultStaminaRecoverRate = Bot.StaminaRecoverRate;
    }

    public static void GeneticDifficultyControl(int difficulty)
    {
        _currentDifficulty = Mathf.Clamp(difficulty, -1, 1);
        if (WarmUp)
        {
            UpdateGenesFromDifficulty(_currentDifficulty);
        }
    }

    public static void ClassicalDifficultyControl(int difficulty)
    {
        UpdateGenesFromDifficulty(0);

        Debug.Log(difficulty);

        int speedMultiplier = 10;
        int attackStrengthMultiplier = 5;
        int blockStrengthMultiplier = 5;
        float dashStrengthMultiplier = 10.0f;
        float staminaRecoverRateMultiplier = -0.05f;


        if (Bot != null)
        {
            Bot.Speed = defaultSpeed + difficulty * speedMultiplier;
            Bot.CharacterWeapon.Strength = defaultAtackStrength + difficulty * attackStrengthMultiplier;
            Bot.CharacterShield.Strength = defaultBlockStrength + difficulty * blockStrengthMultiplier;
            Bot.DashStrength = defaultDashStrength + difficulty * dashStrengthMultiplier;
            Bot.StaminaRecoverRate = defaultStaminaRecoverRate + difficulty * staminaRecoverRateMultiplier;
        }

        _currentDifficulty = Mathf.Clamp(difficulty, -10, 10);;

    }

    private static void UpdateGenesFromDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case -1:
                CurrentGenes = BOTEASY;
                break;
            case 0:
                CurrentGenes = BOTMEDIUM;
                break;
            case 1:
                CurrentGenes = BOTHARD;
                break;
            default:
                break;
        }
    }

    public static void UpdateGenes(float[] genes)
    {
        CurrentGenes = genes;
    }

    public static void UpdateWarmUp(bool warmUp)
    {
        WarmUp = warmUp;
    }

    

    public static void SetTrainingInfo(Character player, float matchTime)
    {
        PlayerWon = player.HP != 0;
        MatchTime = matchTime;

        if (PlayerWon)
        {
            WinnerHP = player.HP;
        }
        else
        {
            WinnerHP = player.OtherCharacter.HP;
        }
    }


    void Update()
    {
        PlayerLives = Player.CurrentLives;
        BotLives = Bot.CurrentLives;
    }
}