using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIData", menuName = "AIStats", order = 0)]
public class AISO : ScriptableObject
{
    [Header("Speed Settings")]
    public float RoamSpeed = 2;
    public float MinRoamRange = 5;
    public float MaxRoamRange = 25;
    public float AttackSpeed = 3;

    [Space(20)]
    [Header("Vision Cone Settings")]
    [Range(0, 100)] public float ViewRadius = 25;
    [Range(0, 360)] public float ViewAngle = 90;

    [Space(10)]
    [Header("Layer Settings")]
    public LayerMask LookForLayer;
    public LayerMask ObstacleLayer;
}
