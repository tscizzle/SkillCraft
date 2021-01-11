using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FightState : MonoBehaviour
/* The state of a fight (health, statuses, etc.). Doesn't know or care about how it is
    displayed.
*/
{
    // TODO: these will not be hard-coded forever.
    static int playerMaxHealth = 100;
    static int enemyMaxHealth = 250;

    /* State. */
    private int previousFighterId = -1;
    public Dictionary<int, FighterState> fighters = new Dictionary<int, FighterState>();
    private int playerFighterId;
    public FighterState player { get { return fighters[playerFighterId]; } }
    public Dictionary<int, FighterState> enemies
    {
        get
        {
            return fighters
                .Where(p => p.Value.isEnemy)
                .ToDictionary(p => p.Key, p => p.Value);
        }
    }

    void Awake()
    {
        // Create the player state.
        AddFighter(isPlayer: true, maxHealth: playerMaxHealth);
        // TODO: starting with one enemy will not be hard-coded forever.
        AddFighter(isPlayer: false, maxHealth: enemyMaxHealth);
    }

    /* Public API. */

    public int AddFighter(bool isPlayer, int maxHealth)
    /* Add a fighter (player or enemy) to the fight, with full health. */
    {
        // Identify each fighter with a unique id.
        int newFighterId = previousFighterId + 1;

        // Create the new fighter.
        FighterState newFighter = new FighterState(newFighterId, isPlayer, maxHealth);

        // Store the fighter.
        fighters[newFighterId] = newFighter;

        // If this is the player, store the id for convenient access.
        if (isPlayer)
        {
            playerFighterId = newFighterId;
        }

        // Remember the id just used.
        previousFighterId = newFighterId;

        return newFighterId;
    }
}

public class FighterState
{
    /* Parameters. */
    public int fighterId;
    public bool isPlayer;
    public bool isEnemy { get { return !isPlayer; } }

    /* State. */
    public int maxHealth;
    public int currentHealth;

    public FighterState(int fighterIdArg, bool isPlayerArg, int startingMaxHealth)
    {
        fighterId = fighterIdArg;
        isPlayer = isPlayerArg;
        maxHealth = startingMaxHealth;
        currentHealth = maxHealth;
    }
}
