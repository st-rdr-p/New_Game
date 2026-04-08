using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Menu button component for interactive menu selections.
    /// </summary>
    public class MenuButton
    {
        /// <summary>Display text for this button.</summary>
        public string Text { get; set; } = "";

        /// <summary>Whether this button is currently highlighted/selected.</summary>
        public bool IsSelected { get; set; } = false;

        /// <summary>Callback when button is pressed.</summary>
        public Action OnPressed { get; set; } = () => { };

        /// <summary>Optional callback when button is selected (before pressing).</summary>
        public Action OnSelected { get; set; } = () => { };

        public MenuButton(string text, Action? onPressed = null)
        {
            Text = text;
            OnPressed = onPressed ?? (() => { });
        }
    }

    /// <summary>
    /// Menu component for displaying menus (main menu, pause menu, settings, etc).
    /// </summary>
    public class MenuComponent : Component
    {
        /// <summary>Menu types.</summary>
        public enum MenuType { MainMenu, PauseMenu, Settings, ConfirmDialog, CustomMenu }

        /// <summary>Type of this menu.</summary>
        public MenuType Type { get; set; }

        /// <summary>Title of the menu.</summary>
        public string Title { get; set; } = "";

        /// <summary>All buttons in this menu.</summary>
        public List<MenuButton> Buttons { get; set; } = new();

        /// <summary>Currently selected button index.</summary>
        public int SelectedButtonIndex { get; private set; } = 0;

        /// <summary>Whether this menu is currently active/displayed.</summary>
        public bool IsActive { get; set; } = false;

        /// <summary>Callback when menu is opened.</summary>
        public Action OnMenuOpen { get; set; } = () => { };

        /// <summary>Callback when menu is closed.</summary>
        public Action OnMenuClose { get; set; } = () => { };

        /// <summary>Callback when button is selected.</summary>
        public Action<int> OnButtonSelected { get; set; } = (_) => { };

        /// <summary>Callback when button is pressed.</summary>
        public Action<int> OnButtonPressed { get; set; } = (_) => { };

        /// <summary>Whether to pause game while menu is active.</summary>
        public bool PausesGame { get; set; } = true;

        /// <summary>Whether to allow menu input (keyboard navigation).</summary>
        public bool AllowInput { get; set; } = true;

        public MenuComponent(MenuType type, string title)
        {
            Type = type;
            Title = title;
        }

        /// <summary>Add a button to this menu.</summary>
        public void AddButton(MenuButton button)
        {
            Buttons.Add(button);
            if (Buttons.Count == 1)
            {
                SelectButton(0);
            }
        }

        /// <summary>Select a button by index.</summary>
        public void SelectButton(int index)
        {
            if (index < 0 || index >= Buttons.Count)
                return;

            // Deselect previous
            if (SelectedButtonIndex >= 0 && SelectedButtonIndex < Buttons.Count)
            {
                Buttons[SelectedButtonIndex].IsSelected = false;
            }

            // Select new
            SelectedButtonIndex = index;
            Buttons[SelectedButtonIndex].IsSelected = true;
            Buttons[SelectedButtonIndex].OnSelected?.Invoke();
            OnButtonSelected?.Invoke(index);
        }

        /// <summary>Move selection up.</summary>
        public void SelectPrevious()
        {
            int newIndex = SelectedButtonIndex - 1;
            if (newIndex < 0)
                newIndex = Buttons.Count - 1; // Wrap around
            SelectButton(newIndex);
        }

        /// <summary>Move selection down.</summary>
        public void SelectNext()
        {
            int newIndex = SelectedButtonIndex + 1;
            if (newIndex >= Buttons.Count)
                newIndex = 0; // Wrap around
            SelectButton(newIndex);
        }

        /// <summary>Press the currently selected button.</summary>
        public void PressSelectedButton()
        {
            if (SelectedButtonIndex >= 0 && SelectedButtonIndex < Buttons.Count)
            {
                var button = Buttons[SelectedButtonIndex];
                button.OnPressed?.Invoke();
                OnButtonPressed?.Invoke(SelectedButtonIndex);
            }
        }

        /// <summary>Open the menu.</summary>
        public void Open()
        {
            IsActive = true;
            OnMenuOpen?.Invoke();
        }

        /// <summary>Close the menu.</summary>
        public void Close()
        {
            IsActive = false;
            OnMenuClose?.Invoke();
        }

        /// <summary>Get the currently selected button.</summary>
        public MenuButton GetSelectedButton()
        {
            if (SelectedButtonIndex >= 0 && SelectedButtonIndex < Buttons.Count)
                return Buttons[SelectedButtonIndex];
            return null;
        }
    }

    /// <summary>
    /// Confirmation dialog component (Yes/No prompts).
    /// </summary>
    public class ConfirmDialog : Component
    {
        /// <summary>Prompt text.</summary>
        public string Prompt { get; set; }

        /// <summary>Callback if user selects "Yes".</summary>
        public Action OnConfirm { get; set; }

        /// <summary>Callback if user selects "No".</summary>
        public Action OnCancel { get; set; }

        /// <summary>Whether dialog is active.</summary>
        public bool IsActive { get; set; } = false;

        public ConfirmDialog(string prompt)
        {
            Prompt = prompt;
        }

        /// <summary>Show the dialog.</summary>
        public void Show()
        {
            IsActive = true;
        }

        /// <summary>Hide the dialog.</summary>
        public void Hide()
        {
            IsActive = false;
        }

        /// <summary>Confirm the action.</summary>
        public void Confirm()
        {
            OnConfirm?.Invoke();
            Hide();
        }

        /// <summary>Cancel the action.</summary>
        public void Cancel()
        {
            OnCancel?.Invoke();
            Hide();
        }
    }
}
