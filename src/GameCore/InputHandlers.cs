using System;

namespace GameCore
{
    /// <summary>
    /// Bridges menu system with input manager.
    /// Handles menu navigation and selection via keyboard.
    /// </summary>
    public class MenuInputHandler
    {
        private MenuComponent _currentMenu;
        private InputManager _inputManager;
        private bool _isMenuOpen;

        public event Action<MenuComponent> OnMenuNavigated;
        public event Action<MenuButton> OnMenuItemSelected;

        public MenuInputHandler(InputManager inputManager)
        {
            _inputManager = inputManager ?? throw new ArgumentNullException(nameof(inputManager));
            _isMenuOpen = false;

            // Subscribe to input events
            _inputManager.OnActionPressed += HandleInputAction;
        }

        /// <summary>
        /// Set the current menu to handle input for.
        /// </summary>
        public void SetActiveMenu(MenuComponent menu)
        {
            _currentMenu = menu;
            _isMenuOpen = menu != null;
        }

        /// <summary>
        /// Handle input actions and apply them to menu.
        /// </summary>
        private void HandleInputAction(InputAction action)
        {
            if (_currentMenu == null || !_isMenuOpen)
                return;

            switch (action)
            {
                case InputAction.MenuUp:
                    _currentMenu.NavigateUp();
                    OnMenuNavigated?.Invoke(_currentMenu);
                    break;

                case InputAction.MenuDown:
                    _currentMenu.NavigateDown();
                    OnMenuNavigated?.Invoke(_currentMenu);
                    break;

                case InputAction.MenuLeft:
                    // Could be used for sub-menus or horizontal navigation
                    break;

                case InputAction.MenuRight:
                    // Could be used for sub-menus or horizontal navigation
                    break;

                case InputAction.MenuSelect:
                case InputAction.MenuCancel:
                    var button = _currentMenu.GetCurrentButton();
                    if (button != null)
                    {
                        button.OnSelected?.Invoke();
                        OnMenuItemSelected?.Invoke(button);
                    }
                    break;

                case InputAction.MenuBack:
                    // Handle back navigation in menu stack
                    break;
            }
        }

        /// <summary>
        /// Close the menu.
        /// </summary>
        public void CloseMenu()
        {
            _isMenuOpen = false;
            _currentMenu = null;
        }

        /// <summary>
        /// Check if a menu is currently active.
        /// </summary>
        public bool IsMenuActive() => _isMenuOpen && _currentMenu != null;

        /// <summary>
        /// Update input state (call every frame).
        /// </summary>
        public void Update()
        {
            _inputManager.Update();
        }
    }

    /// <summary>
    /// Bridges dialogue system with input manager.
    /// Handles dialogue text advancement and choice selection via keyboard.
    /// </summary>
    public class DialogueInputHandler
    {
        private DialogueComponent _currentDialogue;
        private InputManager _inputManager;
        private bool _isDialogueActive;

        public event Action OnDialogueAdvanced;
        public event Action<int> OnChoiceSelected;

        public DialogueInputHandler(InputManager inputManager)
        {
            _inputManager = inputManager ?? throw new ArgumentNullException(nameof(inputManager));
            _isDialogueActive = false;

            // Subscribe to input events
            _inputManager.OnActionPressed += HandleInputAction;
        }

        /// <summary>
        /// Set the current dialogue to handle input for.
        /// </summary>
        public void SetActiveDialogue(DialogueComponent dialogue)
        {
            _currentDialogue = dialogue;
            _isDialogueActive = dialogue != null && dialogue.IsDialogueActive;
        }

        /// <summary>
        /// Handle input actions and apply them to dialogue.
        /// </summary>
        private void HandleInputAction(InputAction action)
        {
            if (_currentDialogue == null || !_isDialogueActive)
                return;

            switch (action)
            {
                case InputAction.DialogueNext:
                    // Advance to next dialogue line or show choices
                    _currentDialogue.AdvanceDialogue();
                    OnDialogueAdvanced?.Invoke();
                    break;

                case InputAction.DialogueChoiceUp:
                    _currentDialogue.NavigateChoicesUp();
                    break;

                case InputAction.DialogueChoiceDown:
                    _currentDialogue.NavigateChoicesDown();
                    break;

                case InputAction.DialogueChoiceSelect:
                    var choiceIndex = _currentDialogue.GetSelectedChoiceIndex();
                    _currentDialogue.SelectChoice(choiceIndex);
                    OnChoiceSelected?.Invoke(choiceIndex);
                    break;

                case InputAction.DialogueSkip:
                    _currentDialogue.EndDialogue();
                    _isDialogueActive = false;
                    break;
            }
        }

        /// <summary>
        /// Close the dialogue.
        /// </summary>
        public void CloseDialogue()
        {
            _isDialogueActive = false;
            _currentDialogue = null;
        }

        /// <summary>
        /// Check if dialogue is currently active.
        /// </summary>
        public bool IsDialogueActive() => _isDialogueActive && _currentDialogue != null;

        /// <summary>
        /// Update input state (call every frame).
        /// </summary>
        public void Update()
        {
            _inputManager.Update();
        }
    }

    /// <summary>
    /// Handles save/load game input bindings.
    /// </summary>
    public class SaveLoadInputHandler
    {
        private InputManager _inputManager;
        private SaveLoadSystem _saveLoadSystem;

        public event Action<int> OnSaveRequested;
        public event Action<int> OnLoadRequested;

        public SaveLoadInputHandler(InputManager inputManager, SaveLoadSystem saveLoadSystem)
        {
            _inputManager = inputManager ?? throw new ArgumentNullException(nameof(inputManager));
            _saveLoadSystem = saveLoadSystem ?? throw new ArgumentNullException(nameof(saveLoadSystem));

            _inputManager.OnActionPressed += HandleInputAction;
        }

        /// <summary>
        /// Handle save/load input actions.
        /// </summary>
        private void HandleInputAction(InputAction action)
        {
            switch (action)
            {
                case InputAction.SaveGame:
                    OnSaveRequested?.Invoke(0); // Save to slot 0
                    break;

                case InputAction.LoadGame:
                    OnLoadRequested?.Invoke(0); // Load from slot 0
                    break;

                case InputAction.QuickSave:
                    OnSaveRequested?.Invoke(9); // Quick save to slot 9
                    break;

                case InputAction.QuickLoad:
                    OnLoadRequested?.Invoke(9); // Quick load from slot 9
                    break;
            }
        }

        /// <summary>
        /// Update input state (call every frame).
        /// </summary>
        public void Update()
        {
            _inputManager.Update();
        }
    }

    /// <summary>
    /// Handles player movement input.
    /// </summary>
    public class PlayerInputHandler
    {
        private InputManager _inputManager;

        public event Action<Vector3> OnMovementInput;
        public event Action OnJumpPressed;
        public event Action OnDashPressed;

        public PlayerInputHandler(InputManager inputManager)
        {
            _inputManager = inputManager ?? throw new ArgumentNullException(nameof(inputManager));
            _inputManager.OnActionPressed += HandleInputAction;
        }

        /// <summary>
        /// Handle player action input.
        /// </summary>
        private void HandleInputAction(InputAction action)
        {
            switch (action)
            {
                case InputAction.Jump:
                    OnJumpPressed?.Invoke();
                    break;

                case InputAction.Dash:
                    OnDashPressed?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Get current movement direction based on held keys.
        /// </summary>
        public Vector3 GetMovementInput()
        {
            var direction = Vector3.Zero;

            if (_inputManager.IsActionHeld(InputAction.MoveUp))
                direction.Y += 1;
            if (_inputManager.IsActionHeld(InputAction.MoveDown))
                direction.Y -= 1;
            if (_inputManager.IsActionHeld(InputAction.MoveLeft))
                direction.X -= 1;
            if (_inputManager.IsActionHeld(InputAction.MoveRight))
                direction.X += 1;

            return direction.Magnitude > 0 ? direction.Normalized : Vector3.Zero;
        }

        /// <summary>
        /// Update input state (call every frame).
        /// </summary>
        public void Update()
        {
            _inputManager.Update();
            OnMovementInput?.Invoke(GetMovementInput());
        }
    }
}
