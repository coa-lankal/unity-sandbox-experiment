using UnityEditor;
using UnityEngine;
using System.Reflection;

public class InspectorLockShortcut
{
    private const string MENU_PATH = "Window/Toggle Inspector Lock %q";
    private static bool _isLocked = false;

    [MenuItem(MENU_PATH, priority = 210)]
    private static void ToggleInspectorLock()
    {
        EditorWindow inspectorWindow = GetInspectorWindow();
        if (inspectorWindow == null) return;

        ToggleLockState(inspectorWindow);
        RefreshLockState();
    }

    [MenuItem(MENU_PATH, validate = true)]
    private static bool ValidateToggleMenu()
    {
        RefreshLockState();
        Menu.SetChecked(MENU_PATH, _isLocked);
        return true;
    }

    private static void RefreshLockState()
    {
        EditorWindow inspector = GetInspectorWindow();
        if (inspector != null)
        {
            PropertyInfo lockProperty = GetLockProperty();
            if (lockProperty != null)
            {
                _isLocked = (bool)lockProperty.GetValue(inspector);
            }
        }
    }

    private static EditorWindow GetInspectorWindow()
    {
        System.Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        return inspectorType != null ? EditorWindow.GetWindow(inspectorType) : null;
    }

    private static PropertyInfo GetLockProperty()
    {
        System.Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        return inspectorType?.GetProperty("isLocked", BindingFlags.Public | BindingFlags.Instance);
    }

    private static void ToggleLockState(EditorWindow inspector)
    {
        PropertyInfo lockProperty = GetLockProperty();
        if (lockProperty == null) return;

        bool currentState = (bool)lockProperty.GetValue(inspector);
        lockProperty.SetValue(inspector, !currentState);
        inspector.Repaint(); // 强制刷新显示
    }
}