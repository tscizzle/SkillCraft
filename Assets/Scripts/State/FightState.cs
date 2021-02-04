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

    /* State subscribers. */
    private Dictionary<int, Action> turnListeners = new Dictionary<int, Action>();
    private Dictionary<int, Action> actionListeners = new Dictionary<int, Action>();
    private Dictionary<int, Action> cuedSkillListeners = new Dictionary<int, Action>();

    void Awake()
    {
        hardCodeFightersAndSkills();

        // Initiate the first fighter's turn.
        currentTurnIdx = fighterTurnOrder.Count - 1;
        goToNextFightersTurn();
    }

    void Start()
    {
        runAllListeners();
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

    public void cueSkill(int skillId)
    /* Prepare a skill to be used. This is what clicking a skill buttons does, so that
    the user can see things like how many actions it will cost, as well as choose the
    target to apply the skill to.

    :param int skillId: Id of the skill to cue up.
    */
    {
        cuedSkillId = skillId;

        // Run cued skill listeners.
        foreach (Action listener in cuedSkillListeners.Values) listener();
    }

    public void uncueSkill()
    /* Set the cued skill back to nothing. */
    {
        cuedSkillId = -1;

        // Run cued skill listeners.
        foreach (Action listener in cuedSkillListeners.Values) listener();
    }

    public string useSkill(int targetFighterId)
    /* Check that the cued skill can be used. If it can, then apply its damage and
    effects to the targeted fighter. If not, display to the user why not.

    :param int targetFighterId: Id of the fighter to apply the skill to.

    :return string reason: If the skill was used, return null. If it was not, return a
        string explaining why not.
    */
    {
        string reason = precheckSkillUse(targetFighterId);
        if (reason != null)
        {
            return reason;
        }

        // Use up actions.
        currentFighter.currentActions -= cuedSkill.actionCost;

        // Calculate damage.
        int damage = cuedSkill.damage;

        // Apply damage to target's shields and health.
        FighterState targetFighter = fighters[targetFighterId];
        int newHealth = Mathf.Max(0, targetFighter.currentHealth - damage);
        targetFighter.setHealth(newHealth);

        // Uncue the used skill.
        uncueSkill();

        // Run action listeners.
        foreach (Action listener in actionListeners.Values) listener();

        return null;
    }

    public void goToNextFightersTurn()
    /* Update the turn index to the next fighter, run any turn-end code for the previous
    fighter and run any turn-start code for the next fighter.
    */
    {
        uncueSkill();

        // Move the turn index up 1, wrapping around to the top of the order once the
        // end of the order is reached.
        currentTurnIdx = (currentTurnIdx + 1) % fighterTurnOrder.Count;

        // Give the next fighter actions for this turn.
        currentFighter.gainActionsToStartTurn();

        // Run turn listeners.
        foreach (Action listener in turnListeners.Values) listener();
        // Run actions listeners.
        foreach (Action listener in actionListeners.Values) listener();
        // Run cued skill listeners.
        foreach (Action listener in cuedSkillListeners.Values) listener();
    }

    /* Hooks.

    These allow code like display code to subscribe to state change events.

    Each register<Something>Listener takes 1 param: an Action (which takes no params, no
    return value) which will run when the relevant state changes, and returns an int
    which can be used to reference that listener, for example to remove it later with
    the corresponding remove<Something>Listener.
    */

    public int addTurnListener(Action listener)
    /*  Relevant state: `currentTurnIdx` */
    {
        int listenerId = previousListenerId + 1;
        turnListeners.Add(listenerId, listener);
        previousListenerId = listenerId;
        return listenerId;
    }

    public void removeTurnListener(int listenerId)
    /*  Relevant state: `currentTurnIdx` */
    {
        turnListeners.Remove(listenerId);
    }

    public int addActionListener(Action listener)
    /*  Relevant state: `currentFighter.currentActions` */
    {
        int listenerId = previousListenerId + 1;
        actionListeners.Add(listenerId, listener);
        previousListenerId = listenerId;
        return listenerId;
    }

    public void removeActionListener(int listenerId)
    /*  Relevant state: `currentFighter.currentActions` */
    {
        actionListeners.Remove(listenerId);
    }

    public int addCuedSkillListener(Action listener)
    /*  Relevant state: `cuedSkill` */
    {
        int listenerId = previousListenerId + 1;
        cuedSkillListeners.Add(listenerId, listener);
        previousListenerId = listenerId;
        return listenerId;
    }

    public void removeCuedSkillListener(int listenerId)
    /*  Relevant state: `cuedSkill` */
    {
        cuedSkillListeners.Remove(listenerId);
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
            return "Not enough actions.";

        bool isTargetingEnemy = currentFighter.fighterId != targetFighterId;
        bool isTargetingSelf = currentFighter.fighterId == targetFighterId;
        if (!cuedSkill.canTargetEnemy && isTargetingEnemy)
            return "Cannot target enemy.";
        else if (!cuedSkill.canTargetSelf && isTargetingSelf)
            return "Cannot target self.";

        return null;
    }

    private void runAllListeners()
    /* For situations when we just want the whole display to update (like at the very
    beginning), run all registered listeners, regardless of what state they are
    subscribed to.
    */
    {
        // Run turn listeners.
        foreach (Action listener in turnListeners.Values) listener();
        // Run actions listeners.
        foreach (Action listener in actionListeners.Values) listener();
        // Run cued skill listeners.
        foreach (Action listener in cuedSkillListeners.Values) listener();
    }

    private void hardCodeFightersAndSkills()
    {
        // Create the player.
        addFighter(isPlayer: true, maxHealth: playerMaxHealth);
        player.addSkill(
            new SkillState("Fireball", "icon_69", actionCost: 2, damage: 10)
        );
        player.addSkill(new SkillState("Punch", "icon_126", damage: 3));
        player.addSkill(
            new SkillState(
                "Enchanted Shield",
                "icon_70",
                canTargetEnemy: false,
                canTargetSelf: true
            )
        );

        // Create the enemies.
        addFighter(isPlayer: false, maxHealth: enemyMaxHealth);
        FighterState enemy = enemies.ToList()[0].Value;
        enemy.addSkill(new SkillState("Kick", "icon_16", damage: 3));

        // Set the turn order.
        fighterTurnOrder.Add(playerFighterId);
        fighterTurnOrder.Add(enemy.fighterId);
    }
}

public class FighterState
{
    /* Constants. */
    static private int actionsGainedPerTurn = 4;
    static private int maxStartingActionsPerTurn = 6;

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
        this.currentActions = 0;
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

    public void gainActionsToStartTurn()
    /* At the start of this fighter's turn, grant the default number of actions to add
    to any actions they have left over from the previous turn, but also upper-bound
    their starting actions.
    */
    {
        currentActions = Mathf.Min(
            currentActions + actionsGainedPerTurn, maxStartingActionsPerTurn
        );
    }
}

public class SkillState
{
    /* Class-wide state. */
    static private int previousSkillId = 0;

    /* Parameters */
    public int skillId;
    public string name;
    public string iconName;
    public int actionCost;
    public int cooldown;
    public int damage;
    public bool canTargetEnemy;
    public bool canTargetSelf;

    public SkillState(
        string name,
        string iconName,
        int actionCost = 1,
        int cooldown = 1,
        int damage = 0,
        bool canTargetEnemy = true,
        bool canTargetSelf = false
    )
    {
        this.skillId = previousSkillId + 1;
        this.name = name;
        this.iconName = iconName;
        this.actionCost = actionCost;
        this.cooldown = cooldown;
        this.damage = damage;
        this.canTargetEnemy = canTargetEnemy;
        this.canTargetSelf = canTargetSelf;

        previousSkillId = skillId;
    }
}
