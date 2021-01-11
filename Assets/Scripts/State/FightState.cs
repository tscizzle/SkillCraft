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
        addFighter(isPlayer: true, maxHealth: playerMaxHealth);
        // TODO: starting with one enemy will not be hard-coded forever.
        addFighter(isPlayer: false, maxHealth: enemyMaxHealth);
    }

    /* Public API. */

    public int addFighter(bool isPlayer, int maxHealth)
    /* Add a fighter (player or enemy) to the fight, with full health. */
    {
        int newFighterId = previousFighterId + 1;

        FighterState newFighter = new FighterState(newFighterId, isPlayer, maxHealth);

        fighters[newFighterId] = newFighter;
        if (isPlayer)
        {
            playerFighterId = newFighterId;
        }

        previousFighterId = newFighterId;

        return newFighterId;
    }

    public void setFighterHealth(int fighterId, int health)
    /* Set a fighter's current health, as in when damaged or healed. */
    {
        fighters[fighterId].setHealth(health);
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

    /* Public API. */

    public void setHealth(int health)
    /* Set the current health, as in when damaged or healed. */
    {
        currentHealth = health;
    }
}
