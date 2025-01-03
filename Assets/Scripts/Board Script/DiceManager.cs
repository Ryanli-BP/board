using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;
    [SerializeField] private Dice DiceToThrow;
    [SerializeField] private int numDice = 1;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float rollForce = 10f;

    private List<Dice> liveDice = new List<Dice>();
    private int remainingDice;  // Tracks remaining dice to finish rolling
    private int totalDiceResult; // Tracks the total sum of dice rolls
    private bool canRollDice = false; // Flag to control dice rolling

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        remainingDice = numDice;  // Initialize remaining dice count
    }


    public int GetNumDice()
    {
        return numDice;
    }

    public void EnableDiceRoll()
    {
        totalDiceResult = 0;
        canRollDice = true;
        UIManager.Instance.SetRollDiceButtonVisibility(true); // Show the roll dice button
    }

    private Vector3 originalGravity;

    public void RollDice()
    {
        if (canRollDice)
        {
            remainingDice = numDice;  // Reset remaining dice count to the total number of dice
            canRollDice = false;
            UIManager.Instance.SetRollDiceButtonVisibility(false);

            originalGravity = Physics.gravity;
            Physics.gravity = new Vector3(0, -20f, 0); // Increase gravity

            // Unsubscribe from previous dice finish events before destroying old dice
            foreach (var die in liveDice)
            {
                die.OnDiceFinishedRolling -= HandleDiceFinishedRolling;
                Destroy(die.gameObject);
            }
            liveDice.Clear();

            // Instantiate and roll new dice
            for (int i = 0; i < numDice; i++)
            {
                Dice diceLive = Instantiate(DiceToThrow, transform.position, transform.rotation);
                liveDice.Add(diceLive);
                diceLive.RollDice(throwForce, rollForce, i);
                diceLive.OnDiceFinishedRolling += HandleDiceFinishedRolling;  // Subscribe to dice finish event
            }
        }

    }

    private void HandleDiceFinishedRolling()
    {
        remainingDice--;

        if (remainingDice <= 0)
        {
            // Reset the gravity to its original value
            Physics.gravity = originalGravity;
            GameManager.Instance.OnDiceRollComplete(); // Directly call the GameManager method
        }
    }

    public void HandleDiceResult(int diceResult)
    {
        totalDiceResult += diceResult;
    }

    public int GetTotalDiceResult()
    {
        return totalDiceResult;
    }
}