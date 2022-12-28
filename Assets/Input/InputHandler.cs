using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Logger logger;

    [Hookable] public Action<Vector2> onMove;
    public void OnMove(InputValue moveValue)
    {
        onMove?.Invoke(moveValue.Get<Vector2>());
    }

    [Hookable] public Action<Vector2> onLook;
    public void OnLook(InputValue lookValue)
    {
        onLook?.Invoke(lookValue.Get<Vector2>());
    }

    [Hookable] public Action<bool> onJump;
    [Hookable] public Action<InputHandler> onJumpStart;
    public void OnJumpStart()
    {
        logger.Log("Jump pressed", this);

        onJump?.Invoke(true);
        onJumpStart?.Invoke(this);
    }

    [Hookable] public Action<InputHandler> onJumpStop;
    public void OnJumpStop()
    {
        logger.Log("Jump released", this);

        onJump?.Invoke(false);
        onJumpStop?.Invoke(this);
    }

    [Hookable] public Action<bool> onSprint;
    [Hookable] public Action<InputHandler> onSprintStart;
    public void OnSprintStart()
    {
        logger.Log("Sprint pressed", this);

        onSprint?.Invoke(true);
        onSprintStart?.Invoke(this);
    }

    [Hookable] public Action<InputHandler> onSprintStop;
    public void OnSprintStop()
    {
        logger.Log("Sprint released", this);

        onSprint?.Invoke(false);
        onSprintStop?.Invoke(this);
    }

    [Hookable] public Action<bool> onCrouch;
    [Hookable] public Action<InputHandler> onCrouchStart;
    public void OnCrouchStart()
    {
        logger.Log("Crouch pressed", this);

        onCrouch?.Invoke(true);
        onCrouchStart?.Invoke(this);
    }

    [Hookable] public Action<InputHandler> onCrouchStop;
    public void OnCrouchStop()
    {
        logger.Log("Crouch released", this);

        onCrouch?.Invoke(false);
        onCrouchStop?.Invoke(this);
    }

    [Hookable] public Action<bool> onPrimary;
    [Hookable] public Action<InputHandler> onPrimaryStart;
    public void OnPrimaryStart()
    {
        logger.Log("Primary pressed", this);

        onPrimary?.Invoke(true);
        onPrimaryStart?.Invoke(this);
    }

    [Hookable] public Action<InputHandler> onPrimaryStop;
    public void OnPrimaryStop()
    {
        logger.Log("Primary released", this);

        onPrimary?.Invoke(false);
        onPrimaryStop?.Invoke(this);
    }

    [Hookable] public Action<InputHandler> onInteract;
    public void OnInteract()
    {
        logger.Log("Interact pressed", this);

        onInteract?.Invoke(this);
    }

    [Hookable] public Action<InputHandler> onPause;
    public void OnPause()
    {
        logger.Log("Pause pressed", this);

        onPause?.Invoke(this);
    }

}
