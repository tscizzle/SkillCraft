using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    /* References. */
    private FightState fightState;

    /* State. */
    private bool isTakingTurn = false;
    // Map skillId to how many times the enemy has used that skill.
    private Dictionary<int, int> skillsUsedCounts = new Dictionary<int, int>();

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();
    }

    void Update()
    {
        if (fightState.currentFighter.isEnemy && !isTakingTurn)
            StartCoroutine(takeEnemyTurn());
    }

    /* Helpers. */

    private IEnumerator takeEnemyTurn()
    /* Perform a series of skills until no longer possible, and then end the turn. */
    {
        isTakingTurn = true;

        // Pause.
        yield return new WaitForSeconds(2.5f);

        // One iteration of this OUTER loop corresponds to one successful skill used.
        // After each one, rerun the skill-choosing algorithm of orderSkillsToUse.
        while (true)
        {
            bool didUseSkill = false;

            List<SkillState> skillsToUse = orderSkillsToUse();
            // One iteration of this INNER loop corresponds to one attempted skill use.
            // We break from this loop once we successfully use a skill.
            foreach (SkillState skill in skillsToUse)
            {
                // Cue the skill.
                fightState.cueSkill(skill.skillId);
                // Pause.
                yield return new WaitForSeconds(2.5f);

                // Target either the player or the enemy's self, based on the skill.
                FighterState targetFighter = skill.canTargetSelf
                    ? fightState.currentFighter
                    : fightState.player;
                int targetFighterId = targetFighter.fighterId;
                // Attempt to use the skill.
                string failureReason = fightState.useSkill(targetFighterId);

                // If successful, pause, then break from this INNER loop.
                if (failureReason == null)
                {
                    // Indicate that this skill was successfully used.
                    skillsUsedCounts[skill.skillId] =
                        getSkillUsedCount(skill.skillId) + 1;
                    didUseSkill = true;
                    // Pause.
                    yield return new WaitForSeconds(2.5f);

                    // Exit ths INNER loop so the OUTER loop can continue and
                    // potentially use more skills.
                    break;
                }
            }

            // If no skill succeeded, didUseSkill is still false, so we break from the
            // OUTER loop so we may end this turn.
            if (!didUseSkill)
                break;
        }

        // End the turn.
        fightState.goToNextFightersTurn();

        isTakingTurn = false;
    }

    private List<SkillState> orderSkillsToUse()
    /* Choose which skill to use first, based on which skills are even able to be cued,
    and how much each skill has been used so far.

    :return List<SkillState>: In case the chosen skill is not able to be used for some
        reason, return a whole list (ordered by the same rules for choosing the first
        skill) so the fighter can try others until it finds a skill that can be used.
    */
    {
        Dictionary<int, SkillState> cueableSkills = fightState.cueableSkills;

        List<SkillState> skillsOrderedByUsage = cueableSkills
            .OrderBy(kvp => getSkillUsedCount(kvp.Key))
            .Select(kvp => cueableSkills[kvp.Key])
            .ToList();

        return skillsOrderedByUsage;
    }

    private int getSkillUsedCount(int skillId)
    /* Helper for accessing skillsUsedCounts but defaulting to 0 if the skill is not in
    there yet.

    :param int skillId: Skill you want the usage count for.

    :return int count:
    */
    {
        if (!skillsUsedCounts.ContainsKey(skillId))
        {
            skillsUsedCounts[skillId] = 0;
        }
        return skillsUsedCounts[skillId];
    }
}
