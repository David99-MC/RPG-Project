using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 offSet;

    [SerializeField] float zoomSpeed = 4f;
    [SerializeField] float zoomMin = 5f;
    [SerializeField] float zoomMax = 15f;

    [SerializeField] float rotateSpeed = 100f;
    
    float currentZoom = 6f;
    float playerPeak = 2f;
    float currentRotation = 0f;

    // Update is called once per frame
    void Update()
    {
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, zoomMin, zoomMax);

        currentRotation -= Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;
    }

    void LateUpdate() {
        transform.position = player.position - offSet * currentZoom;
        transform.LookAt(player, Vector3.up * playerPeak);
        transform.RotateAround(player.position, Vector3.up, currentRotation);
    }
}
