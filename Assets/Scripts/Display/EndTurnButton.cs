using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndTurnButton : MonoBehaviour, IPointerClickHandler
{

    // TODO: add an on-click which increments fightState.currentTurnIdx by 1, mod the number of fighters
    private FightState fightState;

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();
    }

    public void OnPointerClick(PointerEventData ped)
    {
        fightState.goToNextFightersTurn();
    }
}
