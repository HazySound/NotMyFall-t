using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainBasket : MonoBehaviour
{
    private BoxCollider floor;
    private void OnCollisionEnter(Collision collision)
    {
        floor.isTrigger = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        floor = GetComponentInChildren<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
