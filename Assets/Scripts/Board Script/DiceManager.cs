using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DiceManager : MonoBehaviour
{

    public static DiceManager Instance;
    [SerializeField] private Dice DiceToThrow;
    [SerializeField] private int numDice = 3;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float rollForce = 10f;

    private List<Dice> liveDice = new List<Dice>();
    private int remainingDice;  // Tracks remaining dice to finish rolling

    public static UnityAction OnAllDiceFinished;  // Event triggered when all dice finish rolling

    private InputAction rollDiceAction; // New InputAction for rolling dice
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

        // Get and enable the InputAction for rolling the dice
        rollDiceAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
        rollDiceAction.Enable();
        rollDiceAction.performed += OnRollDicePerformed;
    }

    private void OnDisable()
    {
        // Disable the InputAction and unsubscribe from the event
        rollDiceAction.performed -= OnRollDicePerformed;
        rollDiceAction.Disable();
    }

    private void OnRollDicePerformed(InputAction.CallbackContext context)
    {
        if (canRollDice)
        {
            RollDice(); // Trigger the dice roll when the action is performed
        }
    }

    public void EnableDiceRoll()
    {
        canRollDice = true; // Allow dice rolling
    }

    private void RollDice()
    {
        remainingDice = numDice;  // Reset remaining dice count to the total number of dice
        canRollDice = false; // Disable further dice rolls until enabled again

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

    private void HandleDiceFinishedRolling()
    {
        remainingDice--;

        if (remainingDice <= 0)
        {
            OnAllDiceFinished?.Invoke();
        }
    }
}