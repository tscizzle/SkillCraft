using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    PrefabInstantiator prefabInstantiator;
    GameObject characterObj;

    void Awake()
    {
        GameObject prefabInstantiatorObj = GameObject.Find("PrefabInstantiator");
        prefabInstantiator = prefabInstantiatorObj.GetComponent<PrefabInstantiator>();
        characterObj = transform.Find("Character").gameObject;
    }

    void Start()
    {
        PrefabInstantiator.P.CreateHealthBar(gameObject);
    }

    void Update()
    {

    }
}
