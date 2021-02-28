using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
<Skill>
    -> ordered list of Steps
<Step>
    CONSTRUCTIONS
    -> (["add"|"subtract"], "fighters_health", <Numeric>, <DamageType>, <DamageModifier>[], <Condition>, <Number>) (basic damage step)
    -> (["add"|"subtract"], <Status>, <SmallNumber>, <Condition>, <Number>) (applies a status for a number of turns, or cleanses a status)
    [Condition and Number are for only allowing this to happen when Condition is true, and with Number probability]
<Numeric>
    SUBCLASSES
    -> <Number>
    -> <NumericTarget>
    -> <ComboNumeric>
<Number>
    SUBCLASSES
    -> <SmallNumber>
    -> <LargeNumber>
<SmallNumber>
    LEAF
    -> 1 - 5
<LargeNumber>
    LEAF
    -> 6-100
<NumericTarget>
    LEAF
    -> "fighters_health"
    -> "fighters_shield"
    -> "fighters_base_[stat]" (like base fire resistance, before other effects/bonuses are applied)
    -> {should there be a "fighters_current_[stat]"? might overcomplicate the logic, and be too powerful/compound-y}
<ComboNumeric>
    CONSTRUCTIONS
    -> (<NumericTarget>, <Number>) (sums them, like the value of your shield + 5)
<Operation>
    LEAF
    -> "subtract"
    -> "add"
    -> "multiply"
<Status>
    LEAF
    -> "stunned" (to take away enemy turn/actions)
    -> "invulnerable" (can't lose health)
    -> "invisible" (can't be the specific target of a skill, though attacks that hit many fighters can still hit)
    -> {any other hard-coded things, can be creative, take away actions, etc.}
    -> (["add"|"subtract"], <NumericTarget>, <Number>) (e.g. subtract from base fire resistance 10%)
    -> (<Operation>, <NumericTarget>, <SmallNumber>) (e.g. multiply base fire resistance by 4, NEED CONSTRAINT HERE e.g. can't multiply health)
<DamageType>
    LEAF
    -> "physical"
    -> "fire"
    -> "water"
    -> "earth"
    -> "air"
<DamageModifier>
    LEAF
    -> "piercing"
    -> "hits_all_enemies"
    -> "hits_all_fighters"
<Condition>
    LEAF
    -> "below_half_health"
    -> "at_full_health"
    -> "no_shield"
    -> "multiple_enemies"
    -> <Status> (when the targeted fighter has that status)

TODO: should Status and Condition use CONSTRUCTIONS or SUBCLASSES instead of LEAF, and the hard-coded standalones in each should be their
        own new component type?
TODO: is LEAF just signified by having `constructionOptions` be `null`? and SUBCLASSES isn't its own concept, it just works?
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

    public abstract class Component
    /* Anything that can be displayed as a bubble in the component playground. */
    {
        /* Class-wide state. */
        static public int previousComponentId = 0;

        /* Parameters. */
        public int componentId;
        // The options of how to construct this component, keyed by a name for it.
        public virtual Dictionary<string, List<Type>> constructionOptions
        {
            get;
        }

        /* State. */
        public string selectedConstruction;

        public Component()
        {
            this.componentId = previousComponentId + 1;

            previousComponentId = this.componentId;
        }

        /* PUBLIC API. */

        public abstract string getName();
    }

    public class Step : Component
    {
        public override Dictionary<string, List<Type>> constructionOptions
        {
            get
            {
                return new Dictionary<string, List<Type>>()
                {
                    { "Damage", new List<Type>()
                        {
                            typeof(Operation), // Add or subtract...
                            typeof(NumericTarget), // from "fighters_health"...
                            typeof(Numeric), // this much damage...
                            typeof(DamageType), // of this type...
                            typeof(List<DamageModifier>), // with these modifiers...
                            typeof(Condition), // if this condition is true...
                            typeof(Number), // with this probability of success.
                        }
                    },
                    { "Status", new List<Type>()
                        {
                            typeof(Operation), // Add or subtract...
                            typeof(Status), // this status...
                            typeof(SmallNumber), // this many turns...
                            typeof(Condition), // if this condition is true...
                            typeof(Number), // with this probability of success.
                        }
                    }
                };
            }
        }

        private Operation _operation;
        public Operation operation
        {
            get { return _operation; }
            set
            {
                Operation.Type add = Operation.Type.Add;
                Operation.Type subtract = Operation.Type.Subtract;
                if (value.type != add && value.type != subtract)
                {
                    throw new SkillGrammarException(
                        $"A Step's Operation must be {add} or {subtract}."
                    );
                }
                _operation = value;
            }
        }
        private NumericTarget _numericTarget;
        public NumericTarget numericTarget
        {
            get { return _numericTarget; }
            set
            {
                NumericTarget.Type health = NumericTarget.Type.Health;
                if (selectedConstruction == "Damage" && value.type != health)
                {
                    throw new SkillGrammarException(
                        $"A Damage Step's NumericTarget must be Type {health}."
                    );
                }
                _numericTarget = value;
            }
        }
        public Numeric damage;
        public DamageType damageType;
        public List<DamageModifier> damageModifiers;
        public Status status;
        public SmallNumber turns;
        public Condition condition;
        public Number chance = new LargeNumber(100);

        public override string getName()
        {
            return "Step";
        }
    }

    public abstract class Numeric : Component { }

    public class Number : Numeric
    {
        public virtual float minVal { get; }
        public virtual float maxVal { get; }

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

        public override string getName()
        {
            return $"{value}";
        }
    }

    public class SmallNumber : Number
    {
        public override float minVal { get { return 1; } }
        public override float maxVal { get { return 6; } }

        public SmallNumber(float value) : base(value) { }

        public override string getName()
        {
            return $"{value} (small)";
        }
    }

    public class LargeNumber : Number
    {
        public override float minVal { get { return 6; } }
        public override float maxVal { get { return 101; } }

        public LargeNumber(float value) : base(value) { }

        public override string getName()
        {
            return $"{value} (large)";
        }
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

        public override string getName()
        {
            return $"{type}";
        }
    }

    public class ComboNumeric : Numeric
    {
        public override Dictionary<string, List<Type>> constructionOptions
        {
            get
            {
                return new Dictionary<string, List<Type>>()
                {
                    { "Sum", new List<Type>()
                        {
                            typeof(Number), // Add this number...
                            typeof(NumericTarget), // to this numeric target.
                        }
                    }
                };
            }
        }

        public Number number;
        public NumericTarget numericTarget;

        public override string getName()
        {
            return $"{numericTarget.getName()} + {number.value}";
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

        public Condition(Status status)
        {
            this.type = Type.HasStatus;
        }

        public override string getName()
        {
            if (status != null)
                return $"if {type} {status.getName()}";
            else
                return $"if {type}";
        }
    }

    public class Status : Component
    {
        public enum Type { Stunned, Invulnerable, Invisible, Custom }
        public Operation operation;
        public NumericTarget numericTarget;
        public Number anyAmount;
        public SmallNumber smallAmount;
        public Type type;

        public Status(
            Operation operation, NumericTarget numericTarget, Number anyAmount
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
            this.type = Type.Custom;
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

        public override string getName()
        {
            if (type == Type.Custom)
            {
                Number amount = smallAmount != null ? smallAmount : anyAmount;
                return $"{type} status: {operation} {amount.value} to {numericTarget}";
            }
            else
            {
                return $"{type}";
            }
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

        public override string getName()
        {
            return $"{type}";
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

        public override string getName()
        {
            return $"{type}";
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

        public override string getName()
        {
            return $"{type}";
        }
    }

    public class SkillGrammarException : Exception
    {
        public SkillGrammarException(string message) : base(message) { }
    }
}
