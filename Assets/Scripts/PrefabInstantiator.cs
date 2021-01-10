using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
    // Global var that even a prefab can reference. Will be assigned our 1 instance of
    // PrefabInstantiator.
    public static PrefabInstantiator P;

    public GameObject canvasObj;

    public GameObject healthBarPrefab;

    void Awake()
    {
        // Since there should only be 1 PrefabInstantiator instance, assign this
        // instance to a global var.
        P = this;
        canvasObj = GameObject.Find("Canvas");
    }

    /* PUBLIC API */

    public GameObject CreateHealthBar(GameObject fighterObj)
    /* Create a HealthBar for a Fighter.

    :param GameObject fighterObj:
    
    :returns GameObject healthBarObj:
    */
    {
        GameObject healthBarObj = Instantiate(
            healthBarPrefab, Vector3.zero, Quaternion.identity
        );

        HealthBar healthBar = healthBarObj.GetComponent<HealthBar>();
        healthBar.fighterObj = fighterObj;

        // The health bar is a UI image, so make it a child of Canvas.
        healthBarObj.transform.SetParent(canvasObj.transform);

        return healthBarObj;
    }
}
