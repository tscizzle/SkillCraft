using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject fighterObj;

    void Start()
    {

    }

    void Update()
    {
        /* Position the health bar near the fighter. */
        Vector3 fighterPosition = fighterObj.transform.position;
        fighterPosition.y += 2;  // Use the top of the figher.
        Vector3 healthBarPosition = Camera.main.WorldToScreenPoint(fighterPosition);
        if (transform.position != healthBarPosition)
        {
            transform.position = healthBarPosition;
        }
    }
}
