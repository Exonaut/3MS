using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

[RequireComponent(typeof(AudioSource))]
public class MenuUI : MonoBehaviour
{
    [FoldoutGroup("Dependencies", expanded: true)]
    [FoldoutGroup("Dependencies")][SerializeField, Required] protected Logger logger;

    [FoldoutGroup("Settings", expanded: true)]
    [FoldoutGroup("Settings")][SerializeField] bool startEnabled;

    [FoldoutGroup("Audio", expanded: true)]
    [FoldoutGroup("Audio")][SerializeField] AudioClip buttonSelectedClip;
    [FoldoutGroup("Audio")][SerializeField] AudioClip buttonClickedClip;

    [FoldoutGroup("Hooks", expanded: true)]
    [FoldoutGroup("Hooks")][SerializeField] List<EventHook<Object>> showUIHooks;
    [FoldoutGroup("Hooks")][SerializeField] List<EventHook<Object>> hideUIHooks;
    [FoldoutGroup("Hooks")][SerializeField] List<EventHook<Object>> toggleUIHooks;

    Color buttonBaseColor = new Color(1f, 0.533f, 0.82f, 1f);
    Color buttonHoveringColor = new Color(1f, 0.882f, 0.945f, 255f);

    protected UIDocument document;
    protected FocusController focusController;
    protected VisualElement root;
    protected AudioSource audioSource;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        if (!startEnabled) gameObject.SetActive(false);

        logger = logger == null ? Logger.GetDefaultLogger(this) : logger;
        document = GetComponent<UIDocument>();
        if (document) document.enabled = true;

        // Connect hooks
        showUIHooks?.ForEach((hook) => hook.AddListener(ShowUI));
        hideUIHooks?.ForEach((hook) => hook.AddListener(HideUI));
        toggleUIHooks?.ForEach((hook) => hook.AddListener(ToggleUI));

        audioSource = GetComponent<AudioSource>();
    }

    protected void Initialize()
    {
        // Setup Menu buttons
        root = GetComponent<UIDocument>().rootVisualElement;

        // Hover effect for buttons
        root.Query<Button>(className: "button").ForEach((button) =>
        {
            button.RegisterCallback<MouseOverEvent>(OnButtonEnter);
            button.RegisterCallback<MouseLeaveEvent>(OnButtonExit);
            button.clicked += () => PlayButtonClickedSound();
        });

        var btnReturn = root.Q<Button>("Return");
        if (btnReturn != null) btnReturn.clicked += () => OnReturn();

        focusController = root.focusController;
    }

    protected void Update()
    {
        root?.Query<Button>(className: "button").ForEach((button) =>
        {
            ColorButton(button, buttonBaseColor);
        });

        if (focusController != null && focusController.focusedElement is Button)
        {
            Button button = (Button)focusController.focusedElement;
            ColorButton(button, buttonHoveringColor);
        }
    }

    protected void OnEnable()
    {
        StartCoroutine(InitializeOnNextFrame());
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

    public void ShowUI(Object caller = null)
    {
        logger?.Log("UI enabled by " + caller?.name, this);

        gameObject.SetActive(true);
    }

    public void HideUI(Object caller = null)
    {
        logger?.Log("UI disabled by " + caller?.name, this);

        gameObject.SetActive(false);
    }

    public void ToggleUI(Object caller = null)
    {
        logger?.Log("UI toggled by " + caller?.name + " to " + !gameObject.activeSelf, this);

        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void OnButtonEnter(MouseOverEvent e)
    {
        ((VisualElement)e.target).Focus();
        audioSource?.PlayOneShot(buttonSelectedClip);
    }

    private void OnButtonExit(MouseLeaveEvent e)
    {
        //
    }

    private void ColorButton(VisualElement e, Color color)
    {
        e.style.unityBackgroundImageTintColor = color;
    }

    private void PlayButtonClickedSound()
    {
        audioSource.PlayOneShot(buttonClickedClip);
    }

    [Hookable] public event Action<MenuUI> onReturn;
    protected void OnReturn()
    {
        logger.Log("OnReturn clicked");
        onReturn?.Invoke(this);

        HideUI(this);
    }

    protected IEnumerator InitializeOnNextFrame()
    {
        yield return new WaitForEndOfFrame();
        this.Initialize();
    }
}
