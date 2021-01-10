using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : MonoBehaviour
/* Connects the fight state (health, statuses, etc.) to the GameObjects the user sees on
    the screen (the health bars, status icons, etc.)
*/
{
    // TODO: these will not be hard-coded forever.
    static int playerMaxHealth = 100;
    static int enemyMaxHealth = 250;

    private FightState fightState = new FightState(playerMaxHealth);

    void Awake()
    {
        // TODO: starting with one enemy will not be hard-coded forever.
        fightState.AddFighter(isPlayer: false, maxHealth: enemyMaxHealth);
    }

    void Update()
    {
        // TODO: check state things and update display things (e.g. check player and
        // enemy healths in state and update health bar display GameObjects)
    }
}

public class FightState
/* The state of a fight (health, statuses, etc.). Doesn't know or care about how it is
    displayed.
*/
{
    /* State. */
    private int previousFighterId = -1;
    private FighterState player;
    private Dictionary<int, FighterState> enemies = new Dictionary<int, FighterState>();

    /* Constructor */
    public FightState(int playerMaxHealth)
    {
        // Create the player state.
        AddFighter(isPlayer: true, maxHealth: playerMaxHealth);
    }

    /* Helpers */

    public int AddFighter(bool isPlayer, int maxHealth)
    /* Add a fighter (player or enemy) to the fight, with full health. */
    {
        // Identify each fighter with a unique id.
        int newFighterId = previousFighterId + 1;

        // Create the new fighter.
        FighterState newFighter = new FighterState(newFighterId, isPlayer, maxHealth);

        // Store the new fighter in the appropriate place.
        if (isPlayer)
        {
            player = newFighter;
        }
        else
        {
            enemies[newFighterId] = newFighter;
        }

        // Remember the id just used.
        previousFighterId = newFighterId;

        return newFighterId;
    }
}

public class FighterState
{
    /* Parameters. */
    private int fighterId;
    private bool isPlayer;
    private bool isEnemy { get { return !isPlayer; } }

    /* State. */
    private int maxHealth;
    private int currentHealth;

    public FighterState(int fighterIdArg, bool isPlayerArg, int startingMaxHealth)
    {
        fighterId = fighterIdArg;
        isPlayer = isPlayerArg;
        maxHealth = startingMaxHealth;
        currentHealth = maxHealth;
    }
}
