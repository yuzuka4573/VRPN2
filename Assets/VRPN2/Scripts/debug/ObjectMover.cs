using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    Transform target;

    private void Start()
    {
        target = gameObject.transform;
    }
/// <summary>
/// Camera control with AWSD
/// </summary>
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            if (Input.GetKey(KeyCode.A)) {
                target.Translate(-0.1f, 0, 0);
            }
            if (Input.GetKey(KeyCode.W))
            {
                target.Translate(0, -0.1f, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                target.Translate(0, 0.1f, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                target.Translate(0.1f, 0, 0);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                target.localScale = new Vector3(target.localScale.x - 0.1f, target.localScale.y - 0.1f, target.localScale.z-0.1f);
            }
            if (Input.GetKey(KeyCode.E))
            {
                target.localScale = new Vector3(target.localScale.x + 0.1f, target.localScale.y + 0.1f, target.localScale.z + 0.1f);
            }

        }
    }
}
