using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VcamSettings : MonoBehaviour
{
    public static VcamSettings instance;

    private CinemachineFreeLook freelook;

    public float FOVValue = 50;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        freelook = GetComponent<CinemachineFreeLook>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        ChangeFOV();
    }

    private void ChangeFOV()
    {
        freelook.m_Lens.FieldOfView = Mathf.Lerp(freelook.m_Lens.FieldOfView, FOVValue, 0.1f * Time.maximumDeltaTime);
    }
}
