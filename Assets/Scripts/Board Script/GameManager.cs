using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameState currentState;
    public static event Action<GameState> OnGameStateChanged;

    private int totalDiceResult = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ChangeState(GameState.GameSetup);
    }

    private void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.GameSetup:
                SetupGame();
                break;

            case GameState.PlayerTurnStart:
                StartPlayerTurn();
                break;

            case GameState.PlayerRollingDice:
                EnableDiceRoll();
                break;

            case GameState.PlayerMoving:
                break;

            case GameState.PlayerTurnEnd:
                EndPlayerTurn();
                break;

            case GameState.GameEnd:
                EndGame();
                break;
        }

        OnGameStateChanged?.Invoke(currentState);
    }

    private void SetupGame()
    {
        PlayerManager.Instance.SpawnAllPlayersAtHome();
        ChangeState(GameState.PlayerTurnStart);
    }

    private void StartPlayerTurn()
    {
        ChangeState(GameState.PlayerRollingDice);
    }

    private void EnableDiceRoll()
    {
        totalDiceResult = 0; // Reset total dice result
        DiceManager.Instance.EnableDiceRoll();
        DiceManager.OnAllDiceFinished += OnDiceRollComplete;
    }

    public void HandleDiceResult(int diceResult)
    {
        totalDiceResult += diceResult;
    }

    private void OnDiceRollComplete()
    {
        DiceManager.OnAllDiceFinished -= OnDiceRollComplete;
        UIManager.Instance.DisplayTotalResult(totalDiceResult);
    }

    private void StartPlayerMovement(int diceTotal)
    {
        // Implement player movement start logic here
    }

    private void EndPlayerTurn()
    {
        // Implement player turn end logic here
    }

    private void EndGame()
    {
        Debug.Log("Game Over! Implement end-game logic here.");
    }
}

public enum GameState
{
    GameSetup,
    PlayerTurnStart,
    PlayerRollingDice,
    PlayerMoving,
    PlayerTurnEnd,
    GameEnd
}