using Synty.AnimationBaseLocomotion.Samples.InputSystem;
using UnityEngine;

public class InputReaderExtension : MonoBehaviour
{
    public bool isJumpHeld { get; private set; }

    private Controls _controls;

    private void OnEnable()
    {
        _controls = new Controls();
        _controls.Player.Jump.performed += ctx => isJumpHeld = true;
        _controls.Player.Jump.canceled += ctx => isJumpHeld = false;
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
    }
}
