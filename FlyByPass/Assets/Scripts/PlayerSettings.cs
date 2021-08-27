using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upperpik/Settings")]
public class PlayerSettings : ScriptableObject
{
    public bool isPlaying;
    public float ForwardSpeed;
    public float SideSpeed;
    public float sensitivity;
    public float FlySpeed;
}
