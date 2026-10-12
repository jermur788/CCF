// Minimal UnityEngine.InputSystem surface for type-checking only (never shipped, never run).
// Covers exactly the members the runtime sources use; extend it only when a new use appears.

using UnityEngine;
namespace UnityEngine.InputSystem.Controls
{
    public class ButtonControl
    {
        public bool isPressed => false;
        public bool wasPressedThisFrame => false;
        public bool wasReleasedThisFrame => false;
    }
    public class KeyControl : ButtonControl { }
    public class Vector2Control { public Vector2 ReadValue() => default; }
}
namespace UnityEngine.InputSystem
{
    using UnityEngine.InputSystem.Controls;
    public enum Key { None }
    public class Keyboard
    {
        public static Keyboard current => null;
        public KeyControl aKey { get; } = new KeyControl();
        public KeyControl bKey { get; } = new KeyControl();
        public KeyControl cKey { get; } = new KeyControl();
        public KeyControl dKey { get; } = new KeyControl();
        public KeyControl eKey { get; } = new KeyControl();
        public KeyControl fKey { get; } = new KeyControl();
        public KeyControl gKey { get; } = new KeyControl();
        public KeyControl hKey { get; } = new KeyControl();
        public KeyControl iKey { get; } = new KeyControl();
        public KeyControl jKey { get; } = new KeyControl();
        public KeyControl kKey { get; } = new KeyControl();
        public KeyControl lKey { get; } = new KeyControl();
        public KeyControl mKey { get; } = new KeyControl();
        public KeyControl nKey { get; } = new KeyControl();
        public KeyControl oKey { get; } = new KeyControl();
        public KeyControl pKey { get; } = new KeyControl();
        public KeyControl qKey { get; } = new KeyControl();
        public KeyControl rKey { get; } = new KeyControl();
        public KeyControl sKey { get; } = new KeyControl();
        public KeyControl tKey { get; } = new KeyControl();
        public KeyControl uKey { get; } = new KeyControl();
        public KeyControl vKey { get; } = new KeyControl();
        public KeyControl wKey { get; } = new KeyControl();
        public KeyControl xKey { get; } = new KeyControl();
        public KeyControl yKey { get; } = new KeyControl();
        public KeyControl zKey { get; } = new KeyControl();
        public KeyControl digit0Key { get; } = new KeyControl();
        public KeyControl digit1Key { get; } = new KeyControl();
        public KeyControl digit2Key { get; } = new KeyControl();
        public KeyControl digit3Key { get; } = new KeyControl();
        public KeyControl digit4Key { get; } = new KeyControl();
        public KeyControl digit5Key { get; } = new KeyControl();
        public KeyControl digit6Key { get; } = new KeyControl();
        public KeyControl digit7Key { get; } = new KeyControl();
        public KeyControl digit8Key { get; } = new KeyControl();
        public KeyControl digit9Key { get; } = new KeyControl();
        public KeyControl f1Key { get; } = new KeyControl();
        public KeyControl f2Key { get; } = new KeyControl();
        public KeyControl f3Key { get; } = new KeyControl();
        public KeyControl f4Key { get; } = new KeyControl();
        public KeyControl f5Key { get; } = new KeyControl();
        public KeyControl f6Key { get; } = new KeyControl();
        public KeyControl f7Key { get; } = new KeyControl();
        public KeyControl f8Key { get; } = new KeyControl();
        public KeyControl f9Key { get; } = new KeyControl();
        public KeyControl f10Key { get; } = new KeyControl();
        public KeyControl f11Key { get; } = new KeyControl();
        public KeyControl f12Key { get; } = new KeyControl();
        public KeyControl escapeKey { get; } = new KeyControl();
        public KeyControl tabKey { get; } = new KeyControl();
        public KeyControl spaceKey { get; } = new KeyControl();
        public KeyControl leftShiftKey { get; } = new KeyControl();
        public KeyControl rightShiftKey { get; } = new KeyControl();
        public KeyControl enterKey { get; } = new KeyControl();
        public KeyControl backspaceKey { get; } = new KeyControl();
        public KeyControl leftCtrlKey { get; } = new KeyControl();
        public KeyControl rightCtrlKey { get; } = new KeyControl();
        public KeyControl leftAltKey { get; } = new KeyControl();
        public KeyControl rightAltKey { get; } = new KeyControl();
        public KeyControl upArrowKey { get; } = new KeyControl();
        public KeyControl downArrowKey { get; } = new KeyControl();
        public KeyControl leftArrowKey { get; } = new KeyControl();
        public KeyControl rightArrowKey { get; } = new KeyControl();
        public KeyControl deleteKey { get; } = new KeyControl();
    }
    public class Mouse
    {
        public static Mouse current => null;
        public ButtonControl leftButton { get; } = new ButtonControl();
        public ButtonControl rightButton { get; } = new ButtonControl();
        public ButtonControl middleButton { get; } = new ButtonControl();
        public Vector2Control delta { get; } = new Vector2Control();
        public Vector2Control position { get; } = new Vector2Control();
        public Vector2Control scroll { get; } = new Vector2Control();
    }
}
