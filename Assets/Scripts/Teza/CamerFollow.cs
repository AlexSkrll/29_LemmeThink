using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerFollow : MonoBehaviour
{
    public Transform Teza;
    private Vector3 offset = new Vector3(0f, 0f, -10f);

    // Update is called once per frame
    void Update()
    {
        transform.position = Teza.position + offset;
    }
}
