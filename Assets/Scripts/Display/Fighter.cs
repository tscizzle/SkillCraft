using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    /* Parameters. */
    public bool isPlayer;
    public int fighterId;

    /* References. */
    FightState fightState;
    GameObject characterObj;

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();
        characterObj = transform.Find("Character").gameObject;
    }

    void Start()
    {
        // While we are hard-coding the Fighter objects instead of creating them,
        // dynamically, use this hack which assumes 1 player and 1 enemy.
        if (isPlayer)
        {
            fighterId = fightState.player.fighterId;
        }
        else
        {
            fighterId = fightState.enemies.Values.ToList()[0].fighterId;
        }

        PrefabInstantiator.P.CreateHealthBar(gameObject);
    }

    void Update()
    {

    }

    void OnMouseUpAsButton()
    {
        // TODO: if a skill was selected and it targets a fighter, apply the skill
    }
}
