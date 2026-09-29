using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.UI;

[DisallowMultipleComponent]
public class SurvivalGameOver : MonoBehaviour
{
    private PlayerStats stats;
    private GameObject screen;
    private bool shown;
    private bool restarting;
    private CursorLockMode previousLock;
    private bool previousCursorVisible;
    private BaseInputModule[] previousModules;
    private bool[] previousModuleStates;
    private BaseInputModule addedInputModule;

    private void Awake() => stats = GetComponent<PlayerStats>();

    private void Update()
    {
        if (stats == null || !stats.IsDead) return;
        if (!shown) Show();
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            Restart();
    }

    private void Show()
    {
        shown = true;
        previousLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        screen = new GameObject("GameOverScreen", typeof(RectTransform), typeof(Canvas));
        var canvas = screen.GetComponent<Canvas>();
        canvas.sortingOrder = 1000;
        Camera camera = Camera.main;
        if (XRSettings.isDeviceActive && camera != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = camera;
            screen.transform.SetParent(camera.transform, false);
            screen.transform.localPosition = new Vector3(0f, 0f, 1.5f);
            screen.transform.localScale = Vector3.one * 0.002f;
            ((RectTransform)screen.transform).sizeDelta = new Vector2(800f, 500f);
            screen.AddComponent<TrackedDeviceGraphicRaycaster>();
        }
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = screen.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
        }
        screen.AddComponent<GraphicRaycaster>();
        if (EventSystem.current == null)
        {
            var events = new GameObject("GameOverEventSystem", typeof(EventSystem));
            events.transform.SetParent(screen.transform);
        }
        // The scene still has a legacy input module. Use an input-system module
        // for the restart button, and restore the prior modules on teardown.
        var eventSystem = EventSystem.current;
        previousModules = eventSystem.GetComponents<BaseInputModule>();
        previousModuleStates = new bool[previousModules.Length];
        for (int i = 0; i < previousModules.Length; i++)
        {
            previousModuleStates[i] = previousModules[i].enabled;
            previousModules[i].enabled = false;
        }
        BaseInputModule inputModule = XRSettings.isDeviceActive
            ? (BaseInputModule)eventSystem.GetComponent<XRUIInputModule>()
            : eventSystem.GetComponent<InputSystemUIInputModule>();
        if (inputModule == null)
        {
            inputModule = XRSettings.isDeviceActive
                ? (BaseInputModule)eventSystem.gameObject.AddComponent<XRUIInputModule>()
                : eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            addedInputModule = inputModule;
        }
        inputModule.enabled = true;

        var background = new GameObject("Backdrop", typeof(RectTransform), typeof(Image));
        background.transform.SetParent(screen.transform, false);
        var backdropRect = (RectTransform)background.transform;
        backdropRect.anchorMin = Vector2.zero;
        backdropRect.anchorMax = Vector2.one;
        backdropRect.offsetMin = backdropRect.offsetMax = Vector2.zero;
        background.GetComponent<Image>().color = new Color(0.025f, 0.035f, 0.05f, 0.94f);

        AddText("Title", "GAME OVER", 64f, new Vector2(0f, 65f), new Vector2(700f, 100f));
        var buttonObject = new GameObject("Restart", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(screen.transform, false);
        var buttonRect = (RectTransform)buttonObject.transform;
        buttonRect.sizeDelta = new Vector2(280f, 70f);
        buttonRect.anchoredPosition = new Vector2(0f, -45f);
        buttonObject.GetComponent<Image>().color = new Color(0.1f, 0.48f, 0.52f);
        var button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();
        button.onClick.AddListener(Restart);
        var label = AddText("RestartLabel", "CHOI LAI", 29f, Vector2.zero, new Vector2(280f, 70f));
        label.transform.SetParent(buttonObject.transform, false);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(buttonObject);
    }

    private TMP_Text AddText(string objectName, string message, float size, Vector2 position, Vector2 bounds)
    {
        var obj = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(screen.transform, false);
        var text = obj.GetComponent<TextMeshProUGUI>();
        text.text = message;
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        text.rectTransform.sizeDelta = bounds;
        text.rectTransform.anchoredPosition = position;
        return text;
    }

    public void Restart()
    {
        if (!shown || restarting) return;
        var scene = SceneManager.GetActiveScene();
        restarting = true;
        Time.timeScale = 1f;
        RestoreCursor();
        try
        {
#if UNITY_EDITOR
            // Also works for test scenes not yet included in Build Settings.
            if (scene.buildIndex < 0)
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                    scene.path, new LoadSceneParameters(LoadSceneMode.Single));
            else
#endif
                SceneManager.LoadScene(scene.path, LoadSceneMode.Single);
        }
        catch (System.Exception exception)
        {
            restarting = false;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.LogException(exception, this);
        }
    }

    private void RestoreCursor()
    {
        Cursor.lockState = previousLock;
        Cursor.visible = previousCursorVisible;
    }

    private void OnDestroy()
    {
        if (addedInputModule != null) Destroy(addedInputModule);
        if (previousModules != null)
            for (int i = 0; i < previousModules.Length; i++)
                if (previousModules[i] != null) previousModules[i].enabled = previousModuleStates[i];
        if (screen != null) Destroy(screen);
        if (!shown) return;
        Time.timeScale = 1f;
        RestoreCursor();
    }
}
