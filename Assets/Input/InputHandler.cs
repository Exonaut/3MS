using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public class InputHandler : MonoBehaviour
{
    [FoldoutGroup("Dependencies", expanded: true)]
    [FoldoutGroup("Dependencies")][SerializeField, Required] private Logger logger;

    [FoldoutGroup("Hooks")][SerializeField] List<EventHook<Object>> selectPlayerLayout;
    [FoldoutGroup("Hooks")][SerializeField] List<EventHook<Object>> selectUILayout;

    PlayerInput playerInput;

    private void Update()
    {
        if (playerInput.currentActionMap.name == "UI")
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    private void OnEnable()
    {
        playerInput = GetComponent<PlayerInput>();

        selectPlayerLayout?.ForEach((hook) => hook.AddListener(SelectPlayerLayout));
        selectUILayout?.ForEach((hook) => hook.AddListener(SelectUILayout));
    }

    private void OnDisable()
    {
        selectPlayerLayout?.ForEach((hook) => hook.RemoveListener(SelectPlayerLayout));
        selectUILayout?.ForEach((hook) => hook.RemoveListener(SelectUILayout));
    }

    public void SelectPlayerLayout(Object caller)
    {
        logger.Log("PlayerLayout selected", this);

        playerInput?.SwitchCurrentActionMap("Player");

        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void SelectUILayout(Object caller)
    {
        logger.Log("UILayout selected", this);

        playerInput?.SwitchCurrentActionMap("UI");

        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
    }

    [Hookable] public event Action<Vector2> onMove;
    public void OnMove(InputValue moveValue)
    {
        onMove?.Invoke(moveValue.Get<Vector2>());
    }

    [Hookable] public event Action<Vector2> onLook;
    public void OnLook(InputValue lookValue)
    {
        onLook?.Invoke(lookValue.Get<Vector2>());
    }

    [Hookable] public event Action<bool> onJump;
    [Hookable] public event Action<InputHandler> onJumpStart;
    public void OnJumpStart()
    {
        logger.Log("Jump pressed", this);

        onJump?.Invoke(true);
        onJumpStart?.Invoke(this);
    }

    [Hookable] public event Action<InputHandler> onJumpStop;
    public void OnJumpStop()
    {
        logger.Log("Jump released", this);

        onJump?.Invoke(false);
        onJumpStop?.Invoke(this);
    }

    [Hookable] public event Action<bool> onSprint;
    [Hookable] public event Action<InputHandler> onSprintStart;
    public void OnSprintStart()
    {
        logger.Log("Sprint pressed", this);

        onSprint?.Invoke(true);
        onSprintStart?.Invoke(this);
    }

    [Hookable] public event Action<InputHandler> onSprintStop;
    public void OnSprintStop()
    {
        logger.Log("Sprint released", this);

        onSprint?.Invoke(false);
        onSprintStop?.Invoke(this);
    }

    [Hookable] public event Action<bool> onCrouch;
    [Hookable] public event Action<InputHandler> onCrouchStart;
    public void OnCrouchStart()
    {
        logger.Log("Crouch pressed", this);

        onCrouch?.Invoke(true);
        onCrouchStart?.Invoke(this);
    }

    [Hookable] public event Action<InputHandler> onCrouchStop;
    public void OnCrouchStop()
    {
        logger.Log("Crouch released", this);

        onCrouch?.Invoke(false);
        onCrouchStop?.Invoke(this);
    }

    [Hookable] public event Action<bool> onPrimary;
    [Hookable] public event Action<InputHandler> onPrimaryStart;
    public void OnPrimaryStart()
    {
        logger.Log("Primary pressed", this);

        onPrimary?.Invoke(true);
        onPrimaryStart?.Invoke(this);
    }

    [Hookable] public event Action<InputHandler> onPrimaryStop;
    public void OnPrimaryStop()
    {
        logger.Log("Primary released", this);

        onPrimary?.Invoke(false);
        onPrimaryStop?.Invoke(this);
    }

    [Hookable] public event Action<InputHandler> onInteract;
    public void OnInteract()
    {
        logger.Log("Interact pressed", this);

        onInteract?.Invoke(this);
    }

    [Hookable] public event Action<InputHandler> onPause;
    public void OnPause()
    {
        logger.Log("Pause pressed", this);

        onPause?.Invoke(this);
        SelectUILayout(this);
    }

    [Hookable] public event Action<InputHandler> onUICancle;
    public void OnCancle()
    {
        logger.Log("Cancle pressed", this);

        onUICancle?.Invoke(this);
    }

}
