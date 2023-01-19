using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class MenuUI : MonoBehaviour
{
    [FoldoutGroup("Dependencies", expanded: true)]
    [FoldoutGroup("Dependencies")][SerializeField, Required] protected Logger logger;

    [FoldoutGroup("Settings", expanded: true)]
    [FoldoutGroup("Settings")][SerializeField] bool startEnabled;
    [FoldoutGroup("Settings")][SerializeField] Color buttonBaseColor = new Color(0xFF, 0x88, 0xD1, 0xFF);
    [FoldoutGroup("Settings")][SerializeField] Color buttonHoveringColor = new Color(0xFF, 0x00, 0xD3, 0xFF);

    [FoldoutGroup("Hooks", expanded: true)]
    [FoldoutGroup("Hooks")][SerializeField] List<EventHook<Object>> showUIHooks;
    [FoldoutGroup("Hooks")][SerializeField] List<EventHook<Object>> hideUIHooks;
    [FoldoutGroup("Hooks")][SerializeField] List<EventHook<Object>> toggleUIHooks;

    protected UIDocument document;
    protected FocusController focusController;
    protected VisualElement root;
    protected VisualElement hovering;

    protected void Start()
    {
        logger = logger == null ? Logger.GetDefaultLogger(this) : logger;
        document = GetComponent<UIDocument>();
        if (document) document.enabled = true;

        if (!startEnabled) gameObject.SetActive(false);

        // Connect hooks
        showUIHooks?.ForEach((hook) => hook.AddListener(ShowUI));
        hideUIHooks?.ForEach((hook) => hook.AddListener(HideUI));
        toggleUIHooks?.ForEach((hook) => hook.AddListener(ToggleUI));
    }

    protected void Update()
    {
        root?.Query<Button>(className: "button").ForEach((button) =>
        {
            ColorButton(button, buttonBaseColor);
        });

        if (focusController.focusedElement is Button)
        {
            Button button = (Button)focusController.focusedElement;
            ColorButton(button, buttonHoveringColor);
        }

        if (hovering != null)
        {
            hovering.Focus();
        }
    }

    protected void OnEnable()
    {
        // Setup Menu buttons
        root = GetComponent<UIDocument>().rootVisualElement;

        // Hover effect for buttons
        root?.Query<Button>(className: "button").ForEach((button) =>
        {
            button.RegisterCallback<MouseOverEvent>(OnButtonEnter);
            button.RegisterCallback<MouseLeaveEvent>(OnButtonExit);
        });

        var btnReturn = root.Q<Button>("Return");
        if (btnReturn != null) btnReturn.clicked += () => OnReturn();

        focusController = root.focusController;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root?.Query<Button>(className: "button").ForEach((button) =>
        {
            button.UnregisterCallback<MouseOverEvent>(OnButtonEnter);
            button.UnregisterCallback<MouseLeaveEvent>(OnButtonExit);
        });
    }

    void OnDestroy()
    {
        foreach (var hook in showUIHooks)
        {
            hook.RemoveListener(ShowUI);
        }

        foreach (var hook in hideUIHooks)
        {
            hook.RemoveListener(HideUI);
        }
    }

    protected void ShowUI(Object caller = null)
    {
        logger?.Log("UI enabled by " + caller?.name, this);

        gameObject.SetActive(true);
    }

    protected void HideUI(Object caller = null)
    {
        logger?.Log("UI disabled by " + caller?.name, this);

        gameObject.SetActive(false);
    }

    protected void ToggleUI(Object caller = null)
    {
        logger?.Log("UI toggled by " + caller?.name + " to " + !gameObject.activeSelf, this);

        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void OnButtonEnter(MouseOverEvent e)
    {
        hovering = (VisualElement)e.target;
    }

    private void OnButtonExit(MouseLeaveEvent e)
    {
        if (hovering == (VisualElement)e.target) hovering = null;
    }

    private void ColorButton(VisualElement e, Color color)
    {
        e.style.unityBackgroundImageTintColor = color;
    }

    [Hookable] public event Action<MenuUI> onReturn;
    protected void OnReturn()
    {
        logger.Log("OnReturn clicked");
        onReturn?.Invoke(this);

        HideUI(this);
    }
}
