using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FightState : MonoBehaviour
/* The state of a fight (health, statuses, etc.). Doesn't know or care about how it is
    displayed.
*/
{
    // TODO: these hard-codeds will be replaced eventually.
    static int playerMaxHealth = 100;
    static int enemyMaxHealth = 250;

    /* Class-wide state. */
    static private int previousListenerId = 0;

    /* State. */
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
    private int currentTurnIdx = 0;
    private List<int> fighterTurnOrder = new List<int>();
    public FighterState currentFighter
    {
        get { return fighters[fighterTurnOrder[currentTurnIdx]]; }
    }
    [System.NonSerialized]
    public int cuedSkillId = -1;
    public SkillState cuedSkill
    {
        get { return cuedSkillId != -1 ? currentFighter.skills[cuedSkillId] : null; }
    }

    /* Subscribers. */
    private Dictionary<int, Action> turnChangeListeners = new Dictionary<int, Action>();

    void Awake()
    {
        // Create the player.
        addFighter(isPlayer: true, maxHealth: playerMaxHealth);
        player.addSkill(
            new SkillState("Fireball", "icon_69", actionCost: 2, damage: 10)
        );
        player.addSkill(new SkillState("Punch", "icon_126", damage: 3));
        player.addSkill(new SkillState("Enchanted Shield", "icon_70"));

        // Create the enemies.
        addFighter(isPlayer: false, maxHealth: enemyMaxHealth);
        FighterState enemy = enemies.ToList()[0].Value;
        enemy.addSkill(new SkillState("Punch", "icon_126"));

        // Set the turn order.
        fighterTurnOrder.Add(playerFighterId);
        fighterTurnOrder.Add(enemy.fighterId);
    }

    /* PUBLIC API. */

    public int addFighter(bool isPlayer, int maxHealth)
    /* Add a fighter (player or enemy) to the fight, with full health.
    
    :param bool isPlayer: Whether the fighter being added is the user's player or not.
    :param int maxHealth: Max health this fighter can have.

    :return int fighterId: fighterId of the created fighter.
    */
    {
        FighterState newFighter = new FighterState(isPlayer, maxHealth);

        fighters[newFighter.fighterId] = newFighter;

        if (isPlayer)
        {
            playerFighterId = newFighter.fighterId;
        }

        return newFighter.fighterId;
    }

    public void useSkill(int targetFighterId)
    /* Check that the cued skill can be used. If it can, then apply its damage and
    effects to the targeted fighter. If not, display to the user why not.

    :param int targetFighterId: Id of the fighter to apply the skill to.
    */
    {
        string reason = precheckSkillUse(targetFighterId);
        if (reason != null)
        {
            // TODO: display reason, like "Not enough actions."
            return;
        }

        // Subtract the required actions for this skill from the current fighter.
        currentFighter.currentActions -= cuedSkill.actionCost;

        // Calculate the damage done by this skill.
        int damage = cuedSkill.damage;

        // Subtract that much health from the target, without going below 0.
        FighterState targetFighter = fighters[targetFighterId];
        int newHealth = Mathf.Max(0, targetFighter.currentHealth - damage);
        targetFighter.setHealth(newHealth);

        // Uncue the skill that was just used.
        cuedSkillId = -1;
    }

    public void goToNextFightersTurn()
    /* Update the turn index to the next fighter, run any turn-end code for the previous
    fighter and run any turn-start code for the next fighter.
    */
    {
        // Move the turn index up 1, wrapping around to the top of the order once the
        // end of the order is reached.
        currentTurnIdx = (currentTurnIdx + 1) % fighterTurnOrder.Count;

        foreach (Action listener in turnChangeListeners.Values)
        {
            listener();
        }
    }

    /* Hooks (for display to subscribe to state change events). */

    public int addTurnChangeListener(Action listener)
    /* Register a function to run whenever the fighter turn changes. Allows display code
    to update the display only when the relevent state changes instead of every frame.

    :param Action listener: Function (no params, no return)

    :return int listenerId: Key to use to unregister this listener with
        removeTurnChangeListener.
    */
    {
        int listenerId = previousListenerId + 1;
        turnChangeListeners.Add(listenerId, listener);
        previousListenerId = listenerId;
        return listenerId;
    }

    public void removeTurnChangeListener(int listenerId)
    /* Remove a previously registered listener of the fighter turn.
    
    :param int listenerId: Id returned when the listener was registered with
        addTurnChangeListener.
    */
    {
        turnChangeListeners.Remove(listenerId);
    }

    /* Helpers. */

    private string precheckSkillUse(int targetFighterId)
    /* Check that the cued skill is able to be used given the current state and the
    targeted fighter. If not, give the reason.
    
    :param int targetFighterId: Id of the fighter to apply the skill to.

    :return string reason: If the skill can be used, return null. If it cannot, return a
        string explaining why not.
    */
    {
        if (cuedSkill.actionCost > currentFighter.currentActions)
        {
            return "Not enough actions.";
        }

        return null;
    }
}

public class FighterState
{
    /* Constants. */
    static private int startingActionsPerTurn = 4;

    /* Class-wide state. */
    static private int previousFighterId = 0;

    /* Parameters. */
    public int fighterId;
    public bool isPlayer;
    public bool isEnemy { get { return !isPlayer; } }
    public Dictionary<int, SkillState> skills;

    /* State. */
    public int maxHealth;
    public int currentHealth;
    public int currentActions;

    public FighterState(
        bool isPlayer,
        int startingMaxHealth,
        Dictionary<int, SkillState> skills = null
    )
    {
        this.fighterId = previousFighterId + 1;
        this.isPlayer = isPlayer;
        this.maxHealth = startingMaxHealth;
        this.currentHealth = maxHealth;
        this.currentActions = startingActionsPerTurn;
        this.skills = skills != null ? skills : new Dictionary<int, SkillState>();

        previousFighterId = fighterId;
    }

    /* PUBLIC API. */

    public void setHealth(int health)
    /* Set the current health, as in when damaged or healed. */
    {
        currentHealth = health;
    }

    public void addSkill(SkillState skill)
    /* Add a skill to a fighter. */
    {
        skills[skill.skillId] = skill;
    }
}

public class SkillState
{
    /* Class-wide state. */
    static private int previousSkillId = 0;

    /* Parameters */
    public int skillId;
    private string name;
    public string iconName;
    public int actionCost;
    public int damage;

    public SkillState(string name, string iconName, int actionCost = 1, int damage = 0)
    {
        this.skillId = previousSkillId + 1;
        this.name = name;
        this.iconName = iconName;
        this.actionCost = actionCost;
        this.damage = damage;

        previousSkillId = skillId;
    }
}
