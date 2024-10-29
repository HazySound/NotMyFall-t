using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyMotion : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private ConfigurableJoint cj;

    private Quaternion targetInitialRotation;

    // Start is called before the first frame update
    void Start()
    {
        cj = GetComponent<ConfigurableJoint>();
        targetInitialRotation = target.transform.localRotation;
    }

    private Quaternion copyRotation()
    {
        return Quaternion.Inverse(this.target.localRotation) * this.targetInitialRotation;
    }

    private void FixedUpdate()
    {
        cj.targetRotation = copyRotation();
    }
}
