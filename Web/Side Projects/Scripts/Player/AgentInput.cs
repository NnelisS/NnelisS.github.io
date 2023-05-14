using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AgentInput : MonoBehaviour
{
    [field: SerializeField]
    public UnityEvent<Vector3> OnMovementKeyPressed { get; set; }

    private Vector3 moveInput;

    private void Update()
    {
        GetMovementInput();
    }

    private void GetMovementInput()
    {
        Vector2 movement = new Vector2(moveInput.x, moveInput.y);

        OnMovementKeyPressed?.Invoke(new Vector3(movement.x, 0, movement.y));
    }

    private void OnMove(InputValue Value)
    {
        moveInput = Value.Get<Vector2>();
    }
}
