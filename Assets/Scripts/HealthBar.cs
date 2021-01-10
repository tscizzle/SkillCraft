using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // State.
    private int maxHealth = 100;
    private int currentHealth = 80;

    // GameObjects.
    public GameObject fighterObj;
    private GameObject currentHealthObj;
    private GameObject healthTextObj;

    void Awake()
    {
        currentHealthObj = transform.Find("MaxHealth/CurrentHealth").gameObject;
        healthTextObj = transform.Find("HealthText").gameObject;
    }

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

        /* Set the width of the health bar based on how much health is left. */
        float healthPortion = (float)currentHealth / (float)maxHealth;
        currentHealthObj.transform.localScale = new Vector3(healthPortion, 1, 1);

        /* Set the text of the health bar. */
        healthTextObj.GetComponent<Text>().text = $"{currentHealth} / {maxHealth}";
    }
}
