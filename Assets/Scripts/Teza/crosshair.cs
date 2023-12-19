using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class crosshair : MonoBehaviour
{
    private TezaController tezaController;
    // private Vector2 lookInput;
    [SerializeField] private float crosshairRadius;
    void Start()
    {
        Cursor.visible = false;
        tezaController = GetComponentInParent<TezaController>();
    }
    void Update()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device is Mouse)
            {
                //Debug.Log("Mouse Input Detected");
                //Vector2 mousePosition = Mouse.current.position.ReadValue();
                //Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
                //transform.position = position;
            }
            //else if (device is Gamepad)
            //{   
            //    Debug.Log("Gamepad Input Detected");
            //    lookInput = tezaController.lookInput;
            //                                                                              might fix in future
            //    if (lookInput.magnitude > 0f)
            //    {
            //        lookInput.Normalize();
            //        lookInput *= crosshairRadius;
            //        transform.localPosition = lookInput;
            //    }
            //}
        }
    }
}
