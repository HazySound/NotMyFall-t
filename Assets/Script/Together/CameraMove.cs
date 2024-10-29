using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private GameObject Target;
    private Vector3 newPos = new Vector3();

    private float x, y;
    // Start is called before the first frame update
    void Start()
    {
        newPos.x = Target.transform.position.x;
        newPos.y = Target.transform.position.y;
        newPos.z = gameObject.transform.position.z;

        gameObject.transform.position = newPos;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        x = Target.transform.position.x;
        y = Target.transform.position.y;

        newPos.x = x;
        newPos.y = y > 1.3f ? y : 1.3f;

        gameObject.transform.position = newPos;
    }
}
