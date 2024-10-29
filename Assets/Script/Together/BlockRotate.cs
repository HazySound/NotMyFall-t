using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRotate : MonoBehaviour
{
    private Vector3 newRot = new Vector3();
    private Vector3 newPos = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        newRot.z = gameObject.transform.rotation.z;
        gameObject.transform.rotation = Quaternion.Euler(newRot);

        newPos.x = gameObject.transform.position.x;
        newPos.y = gameObject.transform.position.y;
        gameObject.transform.position = newPos;
    }
}
