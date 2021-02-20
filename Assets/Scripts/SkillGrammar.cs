using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
<Skill>
    -> ordered list of Steps
<Step>
    -> (["add"|"subtract"], "fighters_health", <Numeric>, <DamageType>, <DamageModifier>[], <Condition>, <Number>) (basic damage step)
    -> (["add"|"subtract"], <Status>, <SmallNumber>, <Condition>, <Number>) (applies a status for a number of turns, or cleanses a status)
    [Condition and Number are for only allowing this to happen when Condition is true, and with Number probability]
<Numeric>
    -> <Number>
    -> <NumericTarget>
    -> <ComboNumeric>
<Number>
    -> <SmallNumber>
    -> <LargeNumber>
<SmallNumber>
    -> 1 - 5
<LargeNumber>
    -> 6-100
<NumericTarget>
    -> "fighters_health"
    -> "fighters_shield"
    -> "fighters_base_[stat]" (like base fire resistance, before other effects/bonuses are applied)
    -> {should there be a "fighters_current_[stat]"? might overcomplicate the logic, and be too powerful/compound-y}
<ComboNumeric>
    -> (<NumericTarget>, <Number>) (sums them, like the value of your shield + 5)
<Operation>
    -> "subtract"
    -> "add"
    -> "multiply"
<Status>
    -> (["add"|"subtract"], <NumericTarget>, <Numeric>) (e.g. subtract from base fire resistance 10%)
    -> (<Operation>, <NumericTarget>, <SmallNumber>) (e.g. multiply base fire resistance by 4)
    -> "stunned" (to take away enemy turn/actions)
    -> "invulnerable" (can't lose health)
    -> "invisible" (can't be the specific target of a skill, though attacks that hit many fighters can still hit)
    -> {any other hard-coded things, can be creative, take away actions, etc.}
<DamageType>
    -> "physical"
    -> "fire"
    -> "water"
    -> "earth"
    -> "air"
<DamageModifier>
    -> "piercing"
    -> "hits_all_enemies"
    -> "hits_all_fighters"
<Condition>
    -> "below_half_health"
    -> "at_full_health"
    -> "no_shield"
    -> "multiple_enemies"
    -> <Status> (when the targeted fighter has that status)
*/

namespace SkillGrammar
{
    public class Skill
    {
        public List<Step> steps;

        public Skill(List<Step> steps)
        {
            this.steps = steps;
        }
    }

    public class Component
    /* Anything that can be displayed as a bubble in the component playground. */
    {

    }

    public class Step : Component
    {
        public Operation operation;
        public NumericTarget numericTarget;
        public Numeric damage;
        public DamageType damageType;
        public DamageModifier damageModifier;
        public Status status;
        public SmallNumber turns;
        public Condition condition;
        public Number chance;

        public Step(
            Operation operation,
            NumericTarget numericTarget,
            Numeric damage,
            DamageType damageType,
            DamageModifier damageModifier,
            Condition condition,
            Number chance
        )
        {
            if (
                operation.type != Operation.Type.Add
                && operation.type != Operation.Type.Subtract
            )
            {
                throw new SkillGrammarException(
                    $"A Step's Operation must be {Operation.Type.Add}"
                    + $" or {Operation.Type.Add}."
                );
            }
            if (numericTarget.type != NumericTarget.Type.Health)
            {
                throw new SkillGrammarException(
                    $"A Step's NumericTarget must be Type {NumericTarget.Type.Health}."
                );
            }

            this.operation = operation;
            this.numericTarget = numericTarget;
            this.damage = damage;
            this.damageType = damageType;
            this.damageModifier = damageModifier;
            this.condition = condition;
            this.chance = chance;
        }

        public Step(
            Operation operation,
            Status status,
            SmallNumber turns,
            Condition condition,
            Number chance
        )
        {
            if (
                operation.type != Operation.Type.Add
                && operation.type != Operation.Type.Subtract
            )
            {
                throw new SkillGrammarException(
                    $"A Step's Operation must be {Operation.Type.Add}"
                    + $" or {Operation.Type.Add}."
                );
            }

            this.operation = operation;
            this.status = status;
            this.turns = turns;
            this.condition = condition;
            this.chance = chance;
        }
    }

    public class Numeric : Component { }

    public class Number : Numeric
    {
        static public float minVal;
        static public float maxVal;

        public float value;

        public Number(float value)
        {
            if (value < minVal || value >= maxVal)
            {
                throw new SkillGrammarException(
                    $"{value} is not between {minVal} and {maxVal}"
                );
            }
            this.value = value;
        }
    }

    public class SmallNumber : Number
    {
        new static public float minVal = 1;
        new static public float maxVal = 6;

        public SmallNumber(float value) : base(value) { }
    }

    public class LargeNumber : Number
    {
        new static public float minVal = 6;
        new static public float maxVal = 100;

        public LargeNumber(float value) : base(value) { }
    }

    public class NumericTarget : Numeric
    {
        public enum Type
        {
            Health,
            PhysicalShield,
            MagicShield,
            FireResistance,
            EarthResistance,
            WaterResistance,
            AirResistance
        }
        public Type type;

        public NumericTarget(Type type)
        {
            this.type = type;
        }
    }

    public class ComboNumeric : Numeric
    {
        public Number number;
        public NumericTarget numericTarget;

        public ComboNumeric(Number number, NumericTarget numericTarget)
        {
            this.number = number;
            this.numericTarget = numericTarget;
        }
    }

    public class Condition : Component
    {
        public enum Type
        {
            BelowHalfHealth, AtFullHeath, NoShield, MultipleEnemies, HasStatus
        }
        public Type type;
        public Status status;

        public Condition(Type type)
        {
            this.type = type;
        }
    }

    public class Status : Component
    {
        public enum Type { Stunned, Invulnerable, Invisible }
        public Operation operation;
        public NumericTarget numericTarget;
        public Numeric anyAmount;
        public SmallNumber smallAmount;
        public Type type;

        public Status(
            Operation operation, NumericTarget numericTarget, Numeric anyAmount
        )
        {
            if (
                operation.type != Operation.Type.Add
                && operation.type != Operation.Type.Subtract
            )
            {
                throw new SkillGrammarException(
                    $"A Status's Operation with anyAmount must be " +
                    $"{Operation.Type.Add} or {Operation.Type.Subtract}."
                );
            }

            this.operation = operation;
            this.numericTarget = numericTarget;
            this.anyAmount = anyAmount;
        }

        public Status(
            Operation operation, NumericTarget numericTarget, SmallNumber smallAmount
        )
        {
            this.operation = operation;
            this.numericTarget = numericTarget;
            this.smallAmount = smallAmount;
        }

        public Status(Type type)
        {
            this.type = type;
        }
    }

    public class Operation : Component
    {
        public enum Type { Add, Subtract, Multiply }
        public Type type;

        public Operation(Type type)
        {
            this.type = type;
        }
    }

    public class DamageType : Component
    {
        public enum Type { Physical, Fire, Earth, Water, Air }
        public Type type;

        public DamageType(Type type)
        {
            this.type = type;
        }
    }

    public class DamageModifier : Component
    {
        public enum Type { Piercing, HitsAllEnemies, HitsAllFighters }
        public Type type;

        public DamageModifier(Type type)
        {
            this.type = type;
        }
    }

    public class SkillGrammarException : Exception
    {
        public SkillGrammarException(string message) : base(message) { }
    }
}
