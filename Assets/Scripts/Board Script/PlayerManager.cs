using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    [SerializeField] private List<Player> players;

    private int currentPlayerID = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public List<Player> GetPlayerList()
    {
        return players;
    }

    public int getCurrentPlayerID()
    {
        return currentPlayerID;
    }

    public Player GetCurrentPlayer()
    {
        return players[currentPlayerID];
    }

    public Player GetNextPlayer()
    {
        currentPlayerID = (currentPlayerID + 1) % players.Count;
        return GetCurrentPlayer();
    }

    public void SpawnAllPlayersAtHome()
    {
        foreach (var player in players)
        {
            SpawnPlayerAtHome(player);
        }
    }

    public void SpawnPlayerAtHome(Player player)
    {
        Tile homeTile = TileManager.Instance.allTiles.Find(tile => tile.isHome && tile.getPlayerID() == player.getPlayerID());

        if (homeTile != null)
        {
            Vector3 homePosition = homeTile.transform.position;
            homePosition.y += 0.5f; // Adjust Y offset
            player.transform.position = homePosition;
            player.GetComponent<PlayerMovement>().SetCurrentTile(homeTile);
            Debug.Log($"Player {player.getPlayerID()} spawned at their home.");
        }
        else
        {
            Debug.LogError($"No home tile found for player {player.getPlayerID()}!");
        }
    }

    public void StartPlayerMovement(int diceTotal)
    {
        Player currentPlayer = GetCurrentPlayer();
        currentPlayer.GetComponent<PlayerMovement>().MovePlayer(diceTotal);
    }
}