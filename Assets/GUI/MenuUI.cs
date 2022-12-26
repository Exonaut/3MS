using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class MenuUI : MonoBehaviour
{
    [FoldoutGroup("Dependencies", expanded: true)]
    [FoldoutGroup("Dependencies")][SerializeField, Required] protected Logger logger;

    [FoldoutGroup("Settings", expanded: true)]
    [FoldoutGroup("Settings")][SerializeField] bool startEnabled;
    [FoldoutGroup("Settings")][SerializeField] Color notHoveringColor = Color.white;
    [FoldoutGroup("Settings")][SerializeField] Color hoveringColor = Color.gray;

    [FoldoutGroup("Hooks", expanded: true)]
    [FoldoutGroup("Hooks")][SerializeField] List<EventHook<Object>> showUIHooks;
    [FoldoutGroup("Hooks")][SerializeField] List<EventHook<Object>> hideUIHooks;

    private UIDocument document;

    protected void Start()
    {
        logger = logger == null ? Logger.GetDefaultLogger(this) : logger;
        document = GetComponent<UIDocument>();

        if (!startEnabled) gameObject.SetActive(false);

        // Connect hooks
        showUIHooks.ForEach((hook) =>
        {
            hook.AddListener(ShowUI);
        });

        hideUIHooks.ForEach((hook) =>
        {
            hook.AddListener(HideUI);
        });
    }

    protected void OnEnable()
    {
        // Setup Menu buttons
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Hover effect for buttons
        root?.Query<Button>(className: "button").ForEach((button) =>
        {
            button.RegisterCallback<MouseOverEvent>(OnButtonEnter);
            button.RegisterCallback<MouseLeaveEvent>(OnButtonExit);
        });
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

    private void OnButtonEnter(MouseOverEvent e)
    {
        ((Button)e.target).style.unityBackgroundImageTintColor = hoveringColor;
    }

    private void OnButtonExit(MouseLeaveEvent e)
    {
        ((Button)e.target).style.unityBackgroundImageTintColor = notHoveringColor;
    }
}
