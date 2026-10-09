using UnityEngine;
using UnityEngine.InputSystem;

// Shared actions work with OpenXR controllers and the editor keyboard.
public static class SurvivalInput
{
    public const string UseHint = "[A (VR) / E]";
    public const string FillHint = "[B (VR) / Y]";

    private static InputAction use;
    private static InputAction fill;

    // Register both actions before scene interaction and the XR session begin.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        _ = UsePressed;
        _ = FillPressed;
    }

    public static bool UsePressed => ReadPressed(ref use, "SurvivalUse",
        "<XRController>{RightHand}/primaryButton", "<Keyboard>/e");

    public static bool FillPressed => ReadPressed(ref fill, "SurvivalFill",
        "<XRController>{RightHand}/secondaryButton", "<Keyboard>/y");

    private static bool ReadPressed(ref InputAction action, string name,
        string controllerBinding, string keyboardBinding)
    {
        if (action == null)
        {
            action = new InputAction(name, InputActionType.Button);
            action.AddBinding(controllerBinding);
            action.AddBinding(keyboardBinding);
            action.Enable();
        }
        return action.WasPressedThisFrame();
    }

    // Also reset when entering Play Mode with domain reload disabled.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        use?.Dispose();
        fill?.Dispose();
        use = null;
        fill = null;
    }
}
