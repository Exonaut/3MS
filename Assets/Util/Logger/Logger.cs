using UnityEngine;

public class Logger : MonoBehaviour
{
    [Header("Logging")]
    [SerializeField] bool logToConsole = true;
    //[SerializeField] bool logToFile = false;
    [SerializeField] bool logMessages = true;
    [SerializeField] bool logWarnings = true;
    [SerializeField] bool logErrors = true;

    [Header("Settings")]
    [SerializeField] string prefix = "";
    [SerializeField] Color prefixColor = Color.white;

    string _hexColor;

    private static Logger instance;
    private static Logger DefaultLogger
    {
        get
        {
            if (instance == null)
            {
                var i = GameObject.FindGameObjectsWithTag("Default");
                foreach (var item in i)
                {
                    var log = item.GetComponent<Logger>();
                    if (log)
                    {
                        instance = item.GetComponent<Logger>();
                        break;
                    }
                }
            }

            return instance;
        }
        set
        {
            instance = value;
        }
    }

    public static Logger GetDefaultLogger(MonoBehaviour caller)
    {
        DefaultLogger.LogWarning($"Default logger not set for {caller.name}", caller);
        return DefaultLogger;
    }

    private void OnValidate()
    {
        _hexColor = "#" + ColorUtility.ToHtmlStringRGBA(prefixColor);
    }

    public void Log(string message)
    {
        if (logToConsole && logMessages)
        {
            Debug.Log($"<color={_hexColor}>{prefix}:</color> {message}");
        }
    }

    public void Log(string message, Object caller)
    {
        if (logToConsole && logMessages)
        {
            Debug.Log($"<color={_hexColor}>{prefix}:</color> {message}", caller);
        }
    }

    public void LogWarning(string message)
    {
        if (logToConsole && logWarnings)
        {
            Debug.LogWarning($"<color={_hexColor}>{prefix}:</color> {message}");
        }
    }

    public void LogWarning(string message, Object caller)
    {
        if (logToConsole && logWarnings)
        {
            Debug.LogWarning($"<color={_hexColor}>{prefix}:</color> {message}", caller);
        }
    }

    public void LogError(string message)
    {
        if (logToConsole && logErrors)
        {
            Debug.LogError($"<color={_hexColor}>{prefix}:</color> {message}");
        }
    }

    public void LogError(string message, Object caller)
    {
        if (logToConsole && logErrors)
        {
            Debug.LogError($"<color={_hexColor}>{prefix}:</color> {message}", caller);
        }
    }

}
