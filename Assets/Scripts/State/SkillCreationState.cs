using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG = SkillGrammar;

public class SkillCreationState : MonoBehaviour
{
    /* Class-wide state. */
    static private int previousListenerId;

    /* State. */
    private int editedSkillId;
    private string editedName;
    private Sprite editedImage;
    public Dictionary<int, SG.Component> componentMap
        = new Dictionary<int, SG.Component>();
    private int editedComponentId = -1;
    public SG.Component editedComponent
    {
        get { return editedComponentId != -1 ? componentMap[editedComponentId] : null; }
    }

    /* State subscribers.
    - Called when certain state changes.
    - Ok to be called at any time, and "too many times".
    */
    // (None yet)
    /* Event subscribers.
    - Called when certain events occur.
    - Should be called only at the appropriate times, the appropriate number of times.
    */
    private Dictionary<int, Action> editedComponentListeners =
        new Dictionary<int, Action>();
    private Dictionary<int, Action> availableComponentsListeners =
        new Dictionary<int, Action>();

    void Awake()
    {

    }

    void Start()
    {
        int stepComponentId = createComponent(typeof(SG.Step));
        setEditedComponent(stepComponentId);

        hardCodeAvailableComponents();
    }

    /* PUBLIC API. */

    public void setEditedSkill(int skillId)
    {
        editedSkillId = skillId;
    }

    public void setEditedName(string name)
    {
        editedName = name;
    }

    public void setEditedImage(Sprite image)
    {
        editedImage = image;
    }

    public void setEditedComponent(int componentId)
    {
        editedComponentId = componentId;

        // Run edited component listeners.
        foreach (Action listener in editedComponentListeners.Values) listener();
    }

    public int createComponent(Type componentType = null, SG.Component component = null)
    /* Create a skill component which can be used in constructing other components and
    ultimately a skill.

    :param Type componentType: One of the SG.Component types (e.g. Step, Status, etc.)
    :param SG.Component component: If this is filled in 

    :return int componentId: Id of the component that was created.
    */
    {
        if (component == null)
            component = (SG.Component)Activator.CreateInstance(componentType);

        componentMap[component.componentId] = component;

        // Run available components listeners.
        foreach (Action listener in availableComponentsListeners.Values) listener();

        return component.componentId;
    }

    public void removeComponent(int componentId)
    /* Delete a component (like if it was created by the user and they cancelled it.)
    
    :param int componentId: Id of the component to remove.
    */
    {
        componentMap.Remove(componentId);
    }

    /* Hooks.

    These allow code like display code to subscribe to state changes, or effects to
    happen in response to events.

    Each register<Something>Listener takes 1 param: an Action (which takes no params, no
    return value) which will run when the relevant state changes, and returns an int
    which can be used to reference that listener, for example to remove it later with
    the corresponding remove<Something>Listener.
    */

    public int addEditedComponentListener(Action listener)
    /*  Relevant state: `editedComponentId` */
    {
        int listenerId = previousListenerId + 1;
        editedComponentListeners.Add(listenerId, listener);
        previousListenerId = listenerId;
        return listenerId;
    }

    public void removeEditedComponentListener(int listenerId)
    /*  Relevant state: `editedComponentId` */
    {
        editedComponentListeners.Remove(listenerId);
    }

    public int addAvailableComponentsListener(Action listener)
    /*  Relevant state: `componentMap` */
    {
        int listenerId = previousListenerId + 1;
        availableComponentsListeners.Add(listenerId, listener);
        previousListenerId = listenerId;
        return listenerId;
    }

    public void removeAvailableComponentsListener(int listenerId)
    /*  Relevant state: `componentMap` */
    {
        availableComponentsListeners.Remove(listenerId);
    }

    private void hardCodeAvailableComponents()
    {
        createComponent(component: new SG.DamageType(SG.DamageType.Type.Physical));
        createComponent(component: new SG.DamageType(SG.DamageType.Type.Water));
        createComponent(
            component: new SG.DamageModifier(SG.DamageModifier.Type.Piercing)
        );
        createComponent(component: new SG.Status(SG.Status.Type.Stunned));
        createComponent(component: new SG.Operation(SG.Operation.Type.Add));
        createComponent(component: new SG.Operation(SG.Operation.Type.Add));
        createComponent(component: new SG.Operation(SG.Operation.Type.Subtract));
        createComponent(component: new SG.Operation(SG.Operation.Type.Multiply));
        createComponent(component: new SG.Condition(SG.Condition.Type.AtFullHeath));
        createComponent(
            component: new SG.NumericTarget(SG.NumericTarget.Type.FireResistance)
        );
        createComponent(
            component: new SG.NumericTarget(SG.NumericTarget.Type.WaterResistance)
        );
        createComponent(component: new SG.SmallNumber(1));
        createComponent(component: new SG.SmallNumber(1.2f));
        createComponent(component: new SG.LargeNumber(6));
        createComponent(component: new SG.LargeNumber(7));
        createComponent(component: new SG.LargeNumber(10));
    }
}
