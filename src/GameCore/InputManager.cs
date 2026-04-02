using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Enum for all input actions in the game.
    /// </summary>
    public enum InputAction
    {
        None,
        
        // Menu navigation
        MenuUp,
        MenuDown,
        MenuLeft,
        MenuRight,
        MenuSelect,
        MenuCancel,
        MenuBack,
        
        // Dialogue
        DialogueNext,
        DialogueChoiceUp,
        DialogueChoiceDown,
        DialogueChoiceSelect,
        DialogueSkip,
        
        // Player movement
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Jump,
        Dash,
        
        // Game control
        Pause,
        Resume,
        SaveGame,
        LoadGame,
        QuickSave,
        QuickLoad,
        
        // Debug
        DebugToggle,
        DebugReload
    }

    /// <summary>
    /// Handles keyboard/input mapping and state management.
    /// </summary>
    public class InputManager
    {
        private Dictionary<InputAction, List<ConsoleKey>> _keyBindings;
        private Dictionary<InputAction, bool> _actionPressed;
        private Dictionary<InputAction, bool> _actionHeld;
        private ConsoleKey _lastKeyPressed = ConsoleKey.NoName;

        public event Action<InputAction> OnActionPressed;
        public event Action<InputAction> OnActionReleased;

        public InputManager()
        {
            _keyBindings = new Dictionary<InputAction, List<ConsoleKey>>();
            _actionPressed = new Dictionary<InputAction, bool>();
            _actionHeld = new Dictionary<InputAction, bool>();
            
            InitializeDefaultBindings();
        }

        /// <summary>
        /// Set up default key bindings (can be overridden by player settings).
        /// </summary>
        private void InitializeDefaultBindings()
        {
            // Menu navigation
            SetKeyBinding(InputAction.MenuUp, ConsoleKey.UpArrow, ConsoleKey.W);
            SetKeyBinding(InputAction.MenuDown, ConsoleKey.DownArrow, ConsoleKey.S);
            SetKeyBinding(InputAction.MenuLeft, ConsoleKey.LeftArrow, ConsoleKey.A);
            SetKeyBinding(InputAction.MenuRight, ConsoleKey.RightArrow, ConsoleKey.D);
            SetKeyBinding(InputAction.MenuSelect, ConsoleKey.Enter);
            SetKeyBinding(InputAction.MenuCancel, ConsoleKey.Escape);
            SetKeyBinding(InputAction.MenuBack, ConsoleKey.Backspace);

            // Dialogue
            SetKeyBinding(InputAction.DialogueNext, ConsoleKey.Enter, ConsoleKey.Spacebar);
            SetKeyBinding(InputAction.DialogueChoiceUp, ConsoleKey.UpArrow, ConsoleKey.W);
            SetKeyBinding(InputAction.DialogueChoiceDown, ConsoleKey.DownArrow, ConsoleKey.S);
            SetKeyBinding(InputAction.DialogueChoiceSelect, ConsoleKey.Enter);
            SetKeyBinding(InputAction.DialogueSkip, ConsoleKey.Escape);

            // Player movement
            SetKeyBinding(InputAction.MoveUp, ConsoleKey.W, ConsoleKey.UpArrow);
            SetKeyBinding(InputAction.MoveDown, ConsoleKey.S, ConsoleKey.DownArrow);
            SetKeyBinding(InputAction.MoveLeft, ConsoleKey.A, ConsoleKey.LeftArrow);
            SetKeyBinding(InputAction.MoveRight, ConsoleKey.D, ConsoleKey.RightArrow);
            SetKeyBinding(InputAction.Jump, ConsoleKey.Spacebar);
            SetKeyBinding(InputAction.Dash, ConsoleKey.LeftShift);

            // Game control
            SetKeyBinding(InputAction.Pause, ConsoleKey.P, ConsoleKey.Escape);
            SetKeyBinding(InputAction.Resume, ConsoleKey.R, ConsoleKey.Escape);
            SetKeyBinding(InputAction.SaveGame, ConsoleKey.F5);
            SetKeyBinding(InputAction.LoadGame, ConsoleKey.F9);
            SetKeyBinding(InputAction.QuickSave, ConsoleKey.F6);
            SetKeyBinding(InputAction.QuickLoad, ConsoleKey.F10);

            // Debug
            SetKeyBinding(InputAction.DebugToggle, ConsoleKey.F12);
            SetKeyBinding(InputAction.DebugReload, ConsoleKey.F11);
        }

        /// <summary>
        /// Set key binding for an action. Can map multiple keys to same action.
        /// </summary>
        public void SetKeyBinding(InputAction action, params ConsoleKey[] keys)
        {
            if (!_keyBindings.ContainsKey(action))
                _keyBindings[action] = new List<ConsoleKey>();
            
            _keyBindings[action].Clear();
            foreach (var key in keys)
                _keyBindings[action].Add(key);
        }

        /// <summary>
        /// Get current bindings for an action.
        /// </summary>
        public List<ConsoleKey> GetKeyBinding(InputAction action)
        {
            return _keyBindings.ContainsKey(action) ? _keyBindings[action] : new List<ConsoleKey>();
        }

        /// <summary>
        /// Update input state. Call this every frame.
        /// </summary>
        public void Update()
        {
            // Check if a key is currently pressed
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                _lastKeyPressed = keyInfo.Key;

                // Find which action this key maps to
                foreach (var kvp in _keyBindings)
                {
                    if (kvp.Value.Contains(keyInfo.Key))
                    {
                        if (!_actionPressed.ContainsKey(kvp.Key))
                            _actionPressed[kvp.Key] = false;

                        if (!_actionPressed[kvp.Key])
                        {
                            _actionPressed[kvp.Key] = true;
                            _actionHeld[kvp.Key] = true;
                            OnActionPressed?.Invoke(kvp.Key);
                        }
                    }
                }
            }
            else
            {
                // Clear pressed state if no key available
                var pressedActions = new List<InputAction>(_actionPressed.Keys);
                foreach (var action in pressedActions)
                {
                    if (_actionPressed[action])
                    {
                        _actionPressed[action] = false;
                        OnActionReleased?.Invoke(action);
                    }
                }
            }
        }

        /// <summary>
        /// Check if an action was just pressed this frame.
        /// </summary>
        public bool IsActionPressed(InputAction action)
        {
            return _actionPressed.ContainsKey(action) && _actionPressed[action];
        }

        /// <summary>
        /// Check if an action is being held.
        /// </summary>
        public bool IsActionHeld(InputAction action)
        {
            return _actionHeld.ContainsKey(action) && _actionHeld[action];
        }

        /// <summary>
        /// Get the last key pressed (useful for debugging/logging).
        /// </summary>
        public ConsoleKey GetLastKeyPressed() => _lastKeyPressed;

        /// <summary>
        /// Rebind a single key for an action.
        /// </summary>
        public void RemapKey(InputAction action, ConsoleKey oldKey, ConsoleKey newKey)
        {
            if (_keyBindings.ContainsKey(action))
            {
                var index = _keyBindings[action].IndexOf(oldKey);
                if (index >= 0)
                    _keyBindings[action][index] = newKey;
            }
        }

        /// <summary>
        /// Reset all bindings to defaults.
        /// </summary>
        public void ResetToDefaults()
        {
            _keyBindings.Clear();
            _actionPressed.Clear();
            _actionHeld.Clear();
            InitializeDefaultBindings();
        }
    }
}
