using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Logger logger;

    [Hookable] public Action<Vector2> moveEvent;
    public void OnMove(InputValue moveValue)
    {
        if (moveEvent != null)
        {
            moveEvent.Invoke(moveValue.Get<Vector2>());
        }
    }

    [Hookable] public Action<Vector2> lookEvent;
    public void OnLook(InputValue lookValue)
    {
        if (lookEvent != null && Application.isFocused)
        {
            lookEvent.Invoke(lookValue.Get<Vector2>());
        }
    }

    [Hookable] public Action jumpEvent;
    public void OnJump()
    {
        logger.Log("Jump pressed");
        if (jumpEvent != null)
        {
            jumpEvent.Invoke();
        }
    }

    [Hookable] public Action<bool> sprintEvent;
    public void OnSprintStart()
    {
        logger.Log("Sprint started pressed");
        if (sprintEvent != null)
        {
            sprintEvent.Invoke(true);
        }
    }

    public void OnSprintStop()
    {
        logger.Log("Sprint stopped pressed");
        if (sprintEvent != null)
        {
            sprintEvent.Invoke(false);
        }
    }

    [Hookable] public Action<bool> crouchEvent;
    public void OnCrouchStart()
    {
        logger.Log("Crouch start pressed");
        if (crouchEvent != null)
        {
            crouchEvent.Invoke(true);
        }
    }

    public void OnCrouchStop()
    {
        logger.Log("Crouch stop pressed");
        if (crouchEvent != null)
        {
            crouchEvent.Invoke(false);
        }
    }

    [Hookable] public Action primaryStartEvent;
    public void OnPrimaryStart()
    {
        logger.Log("Primary pressed");
        if (primaryStartEvent != null)
        {
            primaryStartEvent.Invoke();
        }
    }

    [Hookable] public Action primaryStopEvent;
    public void OnPrimaryStop()
    {
        logger.Log("Primary released");
        if (primaryStopEvent != null)
        {
            primaryStopEvent.Invoke();
        }
    }

    [Hookable] public Action interactEvent;
    public void OnInteract()
    {
        logger.Log("Interact pressed");
        if (interactEvent != null)
        {
            interactEvent.Invoke();
        }
    }

    [Hookable] public Action muteEvent;
    public void OnMute()
    {
        logger.Log("Mute pressed");
        if (muteEvent != null)
        {
            muteEvent.Invoke();
        }
    }

    [Hookable] public Action exitEvent;
    public void OnExit()
    {
        logger.Log("Exit pressed");
        if (exitEvent != null)
        {
            exitEvent.Invoke();
        }
    }

    [Hookable] public Action volumeUpEvent;
    public void OnVolumeUp()
    {
        logger.Log("Volume up pressed");
        if (volumeUpEvent != null)
        {
            volumeUpEvent.Invoke();
        }
    }

    [Hookable] public Action volumeDownEvent;
    public void OnVolumeDown()
    {
        logger.Log("Volume down pressed");
        if (volumeDownEvent != null)
        {
            volumeDownEvent.Invoke();
        }
    }

    [Hookable] public Action restartEvent;
    public void OnRestart()
    {
        logger.Log("Restart pressed");
        if (restartEvent != null)
        {
            restartEvent.Invoke();
        }
    }

}
