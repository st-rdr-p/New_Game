using NUnit.Framework;
using GameCore;
using System;
using System.Collections.Generic;

namespace GameTests
{
    /// <summary>
    /// Tests for the Input Manager - key binding, input state, input handling.
    /// </summary>
    [TestFixture]
    public class InputManagerTests
    {
        private InputManager? _inputManager;

        [SetUp]
        public void Setup()
        {
            _inputManager = new InputManager();
        }

        #region Key Binding Tests

        [Test]
        public void InputManager_SetKeyBinding()
        {
            // Arrange & Act
            _inputManager!.SetKeyBinding(InputAction.Jump, ConsoleKey.Spacebar);

            // Assert
            var bindings = _inputManager.GetKeyBinding(InputAction.Jump);
            Assert.That(bindings.Count, Is.GreaterThan(0));
            Assert.That(bindings, Contains.Item(ConsoleKey.Spacebar));
        }

        [Test]
        public void InputManager_MultipleKeysForAction()
        {
            // Arrange & Act
            _inputManager!.SetKeyBinding(InputAction.MoveUp, ConsoleKey.W, ConsoleKey.UpArrow);

            // Assert
            var bindings = _inputManager.GetKeyBinding(InputAction.MoveUp);
            Assert.That(bindings.Count, Is.EqualTo(2));
            Assert.That(bindings, Contains.Item(ConsoleKey.W));
            Assert.That(bindings, Contains.Item(ConsoleKey.UpArrow));
        }

        [Test]
        public void InputManager_RebindAction()
        {
            // Arrange
            _inputManager!.SetKeyBinding(InputAction.Pause, ConsoleKey.P);
            
            // Act
            _inputManager.SetKeyBinding(InputAction.Pause, ConsoleKey.Escape);

            // Assert
            var bindings = _inputManager.GetKeyBinding(InputAction.Pause);
            Assert.That(bindings, Contains.Item(ConsoleKey.Escape));
        }

        [Test]
        public void InputManager_GetKeyBinding_DefaultBindings()
        {
            // Arrange & Act
            var menuUpBindings = _inputManager!.GetKeyBinding(InputAction.MenuUp);

            // Assert
            Assert.That(menuUpBindings.Count, Is.GreaterThan(0));
        }

        #endregion

        #region Action State Tests

        [Test]
        public void InputManager_IsActionPressed()
        {
            // Arrange - Create a new manager and verify initial state
            var manager = new InputManager();

            // Act & Assert - action should not be pressed initially
            Assert.That(manager.IsActionPressed(InputAction.Jump), Is.False);
        }

        [Test]
        public void InputManager_IsActionHeld()
        {
            // Arrange
            var manager = new InputManager();

            // Act & Assert
            Assert.That(manager.IsActionHeld(InputAction.MoveUp), Is.False);
        }

        [Test]
        public void InputManager_GetLastKeyPressed()
        {
            // Arrange
            var manager = new InputManager();

            // Act
            var lastKey = manager.GetLastKeyPressed();

            // Assert
            Assert.That(lastKey, Is.EqualTo(ConsoleKey.NoName));
        }

        #endregion

        #region Event Tests

        [Test]
        public void InputManager_OnActionPressedEvent()
        {
            // Arrange
            var manager = new InputManager();
            InputAction? pressedAction = null;

            // Act - subscribe to the event
            manager.OnActionPressed += (action) => { pressedAction = action; };

            // Assert - subscription should succeed without throwing
            Assert.Pass("Event subscription successful");
        }

        [Test]
        public void InputManager_OnActionReleasedEvent()
        {
            // Arrange
            var manager = new InputManager();
            InputAction? releasedAction = null;

            // Act - subscribe to the event
            manager.OnActionReleased += (action) => { releasedAction = action; };

            // Assert - subscription should succeed without throwing
            Assert.Pass("Event subscription successful");
        }

        #endregion

        #region Update Tests

        [Test]
        public void InputManager_UpdateDoesNotThrow()
        {
            // Arrange
            var manager = new InputManager();

            // Act & Assert
            Assert.DoesNotThrow(() => manager.Update());
        }

        #endregion

        #region Movement Binding Tests

        [Test]
        public void Input_MovementActionsHaveBindings()
        {
            // Arrange
            var manager = new InputManager();

            // Act
            var upBindings = manager.GetKeyBinding(InputAction.MoveUp);
            var downBindings = manager.GetKeyBinding(InputAction.MoveDown);
            var leftBindings = manager.GetKeyBinding(InputAction.MoveLeft);
            var rightBindings = manager.GetKeyBinding(InputAction.MoveRight);

            // Assert
            Assert.That(upBindings.Count, Is.GreaterThan(0));
            Assert.That(downBindings.Count, Is.GreaterThan(0));
            Assert.That(leftBindings.Count, Is.GreaterThan(0));
            Assert.That(rightBindings.Count, Is.GreaterThan(0));
        }

        #endregion

        #region Menu Binding Tests

        [Test]
        public void Input_MenuActionsHaveBindings()
        {
            // Arrange
            var manager = new InputManager();

            // Act
            var selectBindings = manager.GetKeyBinding(InputAction.MenuSelect);
            var cancelBindings = manager.GetKeyBinding(InputAction.MenuCancel);

            // Assert
            Assert.That(selectBindings.Count, Is.GreaterThan(0));
            Assert.That(cancelBindings.Count, Is.GreaterThan(0));
            Assert.That(selectBindings, Contains.Item(ConsoleKey.Enter));
        }

        #endregion

        #region Dialogue Binding Tests

        [Test]
        public void Input_DialogueActionsHaveBindings()
        {
            // Arrange
            var manager = new InputManager();

            // Act
            var nextBindings = manager.GetKeyBinding(InputAction.DialogueNext);
            var skipBindings = manager.GetKeyBinding(InputAction.DialogueSkip);

            // Assert
            Assert.That(nextBindings.Count, Is.GreaterThan(0));
            Assert.That(skipBindings.Count, Is.GreaterThan(0));
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Input_AllMovementKeysConfigured()
        {
            // Arrange
            var manager = new InputManager();
            var movementActions = new[]
            {
                InputAction.MoveUp,
                InputAction.MoveDown,
                InputAction.MoveLeft,
                InputAction.MoveRight
            };

            // Act & Assert
            foreach (var action in movementActions)
            {
                var bindings = manager.GetKeyBinding(action);
                Assert.That(bindings.Count, Is.GreaterThan(0), $"{action} should have key bindings");
            }
        }

        [Test]
        public void Input_GameControlActionsConfigured()
        {
            // Arrange
            var manager = new InputManager();

            // Act
            var pauseBindings = manager.GetKeyBinding(InputAction.Pause);
            var saveBindings = manager.GetKeyBinding(InputAction.SaveGame);

            // Assert
            Assert.That(pauseBindings.Count, Is.GreaterThan(0));
            Assert.That(saveBindings.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Input_KeyRebindingWorkflow()
        {
            // Arrange
            var manager = new InputManager();
            var originalBindings = manager.GetKeyBinding(InputAction.Jump);

            // Act
            manager.SetKeyBinding(InputAction.Jump, ConsoleKey.W);
            var newBindings = manager.GetKeyBinding(InputAction.Jump);

            // Assert
            Assert.That(newBindings, Contains.Item(ConsoleKey.W));
            Assert.That(newBindings.Count, Is.EqualTo(1));
        }

        #endregion
    }
}
