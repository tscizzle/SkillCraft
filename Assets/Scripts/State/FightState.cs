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

    void Awake()
    {
        // Create the player.
        addFighter(isPlayer: true, maxHealth: playerMaxHealth);
        player.addSkill(new SkillState("Fireball", "icon_69", damage: 10));
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

    /* Public API. */

    public int addFighter(bool isPlayer, int maxHealth)
    /* Add a fighter (player or enemy) to the fight, with full health. */
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
    /* Apply the damage and effects of the cued skill to the targeted fighter. */
    {
        // Calculate the damage done by this skill.
        int damage = cuedSkill.damage;

        // Subtract that much health from the target, without going below 0.
        FighterState targetFighter = fighters[targetFighterId];
        int newHealth = Mathf.Max(0, targetFighter.currentHealth - damage);
        targetFighter.setHealth(newHealth);

        // Uncue the skill that was just used.
        cuedSkillId = -1;
    }
}

public class FighterState
{
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
    public int currentActions = 4;

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
        this.skills = skills != null ? skills : new Dictionary<int, SkillState>();

        previousFighterId = fighterId;
    }

    /* Public API. */

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
    public int damage;

    public SkillState(string name, string iconName, int damage = 0)
    {
        this.skillId = previousSkillId + 1;
        this.name = name;
        this.iconName = iconName;
        this.damage = damage;

        previousSkillId = skillId;
    }
}
