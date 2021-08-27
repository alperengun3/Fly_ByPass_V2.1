using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] private PlayerSettings settings;
    [SerializeField] private Transform followed;
    [SerializeField] private float smoothSpeed;
    [SerializeField] Vector3 offset;
    private Vector3 mousePos;
    private Vector3 firstPos;
    private Vector3 diff;
    [SerializeField] private Camera ortho;
    void FixedUpdate()
    {
        Vector3 desirePosition = followed.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desirePosition, smoothSpeed);
        transform.position = smoothPosition;
        transform.LookAt(followed);
        offset = new Vector3(-diff.x, 20,-40);
    }
}
