# Save/Load System Guide

Complete guide for implementing save and load functionality in your game.

## Overview

The save/load system provides:
- **Save Slots** - Multiple save files (up to 10 slot)
- **Auto-Save** - Automatic checkpoints
- **Quick Save/Load** - Fast access to recent save
- **Player State** - Health, inventory, level, position
- **Entity State** - Save/restore any game entity
- **Game Variables** - Store flags and quest progress
- **Playtime Tracking** - How long player has played

## Quick Start

### Setup Save System

```csharp
var game = new Game();
var saveSystem = new SaveLoadSystem(game);
game.AddSystem(saveSystem);

// Setup callbacks
saveSystem.OnSaveComplete += (success) =>
{
    if (success) Console.WriteLine("Game saved!");
};

saveSystem.OnLoadComplete += (success) =>
{
    if (success) Console.WriteLine("Game loaded!");
};
```

### Create Saveable Player

```csharp
var player = new Entity(1);
player.AddComponent(new Transform3D { Position = new Vector3(0, 1, 0) });
player.AddComponent(new Health(100));

// Make player saveable
var playerSave = new PlayerSaveableComponent { Level = 1 };
playerSave.Inventory["health_potion"] = 5;
player.AddComponent(playerSave);
```

### Save Game

```csharp
// Save to slot 0
saveSystem.SaveGame(0, "My Savegame", entities);
```

### Load Game

```csharp
// Load from slot 0
saveSystem.LoadGame(0, entities);
```

## Core Components

### SaveableComponent

Make any entity saveable.

```csharp
var entity = new Entity(100);
var saveable = new SaveableComponent("enemy_1")
{
    IsSaveable = true,
    SaveData = new Dictionary<string, object>
    {
        { "health", 50f },
        { "is_defeated", false }
    }
};
entity.AddComponent(saveable);
```

**Properties:**
- `SaveId` - Unique identifier for saving
- `IsSaveable` - Whether to include in saves
- `SaveData` - Custom data to persist
- `OnGetState` - Custom save logic callback
- `OnRestoreState` - Custom load logic callback

### PlayerSaveableComponent

Specialized component for player state.

```csharp
var playerSave = new PlayerSaveableComponent
{
    Level = 5,
    Experience = 1000f
};
playerSave.Inventory["sword"] = 1;
playerSave.Inventory["shield"] = 1;
player.AddComponent(playerSave);
```

**Properties:**
- `Level` - Player level
- `Experience` - Experience points
- `Inventory` - Item system (Dictionary<string, int>)

**Methods:**
- `GetState()` - Get player state for saving
- `RestoreState(PlayerState)` - Restore from save

### SaveLoadSystem

Main manager for save/load operations (ISystem).

**Methods:**
- `SaveGame(slot, name, entities)` - Save to slot
- `LoadGame(slot, entities)` - Load from slot
- `GetSaveSlot(slot)` - Get slot information
- `GetAllSaveSlots()` - Get all 10 slots
- `DeleteSaveSlot(slot)` - Delete a save
- `SlotHasSave(slot)` - Check if slot has save
- `FormatPlaytime(seconds)` - Format time for display

**Callbacks:**
- `OnSaveStart` - Save beginning
- `OnSaveComplete(success)` - Save finished
- `OnLoadStart` - Load beginning
- `OnLoadComplete(success)` - Load finished
- `OnSaveMessage(message)` - Log messages

## Game State Structure

```json
{
  "playerState": {
    "health": 85.5,
    "maxHealth": 100,
    "positionX": 10.5,
    "positionY": 1.0,
    "positionZ": 20.3,
    "level": 5,
    "experience": 2500.0,
    "inventory": {
      "health_potion": 5,
      "mana_potion": 3,
      "sword": 1
    }
  },
  "currentLevel": "Forest",
  "entities": [
    {
      "entityId": 100,
      "entityType": "Enemy",
      "positionX": 15.0,
      "positionY": 0.0,
      "positionZ": 25.0,
      "isActive": true,
      "customData": {
        "health": 50.0,
        "is_defeated": false
      }
    }
  ],
  "completedQuests": {
    "kill_rats": true,
    "find_treasure": false
  },
  "gameVariables": {
    "boss_defeated": false,
    "secret_found": true
  },
  "playtimeSeconds": 3600.0,
  "saveTime": "2026-04-02T14:30:00",
  "gameVersion": "1.0.0"
}
```

## Complete Example

```csharp
public class GameWithSaving : Game
{
    private SaveLoadSystem _saveSystem;
    private MenuSystem _menuSystem;
    private Entity _player;

    protected override void OnInitialize()
    {
        // Create systems
        _saveSystem = new SaveLoadSystem(this);
        _menuSystem = new MenuSystem(this);
        
        AddSystem(_saveSystem);
        AddSystem(_menuSystem);

        // Setup callbacks
        SetupSaveCallbacks();

        // Create player
        _player = CreateSaveablePlayer();
        AddEntity(_player);

        // Create some enemies
        AddEntity(CreateSaveableEnemy(100, new Vector3(10, 0, 10)));
        AddEntity(CreateSaveableEnemy(101, new Vector3(-10, 0, -10)));

        // Setup menus
        CreateSaveMenu(_menuSystem);
        CreateLoadMenu(_menuSystem);

        Console.WriteLine("Game ready!");
    }

    private void SetupSaveCallbacks()
    {
        _saveSystem.OnSaveMessage += (msg) => Console.WriteLine($"[SAVE] {msg}");
        _saveSystem.OnSaveComplete += (success) =>
        {
            if (success)
                Console.WriteLine("✓ Save successful");
            else
                Console.WriteLine("✗ Save failed");
        };

        _saveSystem.OnLoadMessage += (msg) => Console.WriteLine($"[LOAD] {msg}");
        _saveSystem.OnLoadComplete += (success) =>
        {
            if (success)
                Console.WriteLine("✓ Load successful");
            else
                Console.WriteLine("✗ Load failed");
        };
    }

    private Entity CreateSaveablePlayer()
    {
        var player = new Entity(1);
        player.AddComponent(new Transform3D { Position = Vector3.Zero });
        player.AddComponent(new Tag("Player"));
        player.AddComponent(new Health(100));

        var playerSave = new PlayerSaveableComponent { Level = 1 };
        playerSave.Inventory["health_potion"] = 5;
        player.AddComponent(playerSave);

        return player;
    }

    private Entity CreateSaveableEnemy(int id, Vector3 pos)
    {
        var enemy = new Entity(id);
        enemy.AddComponent(new Transform3D { Position = pos });
        enemy.AddComponent(new Tag("Enemy"));
        enemy.AddComponent(new Health(50));
        enemy.AddComponent(new EnemyAI());

        var saveable = new SaveableComponent($"enemy_{id}");
        enemy.AddComponent(saveable);

        return enemy;
    }

    private void CreateSaveMenu(MenuSystem menuSystem)
    {
        var menu = new MenuComponent(MenuComponent.MenuType.CustomMenu, "Save Game");

        for (int i = 0; i < 10; i++)
        {
            var slot = _saveSystem.GetSaveSlot(i);
            string text = slot.IsValid 
                ? $"Slot {i}: {slot.SaveName} ({slot.SaveTime:MM/dd})"
                : $"Slot {i}: <Empty>";

            int slotIndex = i;
            menu.AddButton(new MenuButton(text, () =>
            {
                _saveSystem.SaveGame(slotIndex, $"Save {slotIndex}", GetEntities());
            }));
        }

        menu.AddButton(new MenuButton("Back", () => menuSystem.CloseCurrentMenu()));
        menuSystem.RegisterMenu("save_menu", menu);
    }

    private void CreateLoadMenu(MenuSystem menuSystem)
    {
        var menu = new MenuComponent(MenuComponent.MenuType.CustomMenu, "Load Game");

        for (int i = 0; i < 10; i++)
        {
            var slot = _saveSystem.GetSaveSlot(i);
            string text = slot.IsValid 
                ? $"Slot {i}: {slot.SaveName} ({slot.SaveTime:MM/dd})"
                : $"Slot {i}: <Empty>";

            int slotIndex = i;
            menu.AddButton(new MenuButton(text, () =>
            {
                if (slot.IsValid)
                {
                    _saveSystem.LoadGame(slotIndex, GetEntities());
                }
            }));
        }

        menu.AddButton(new MenuButton("Back", () => menuSystem.CloseCurrentMenu()));
        menuSystem.RegisterMenu("load_menu", menu);
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // Handle save/load input
        // if (Input.GetKeyDown(KeyCode.F5))
        //     _saveSystem.SaveGame(0, "QuickSave", GetEntities());
        // 
        // if (Input.GetKeyDown(KeyCode.F9))
        //     _saveSystem.LoadGame(0, GetEntities());
    }

    private IEnumerable<Entity> GetEntities()
    {
        // Return game's entity list
        return new List<Entity> { _player };
    }
}
```

## Save Slots

### Get Slot Information

```csharp
// Get info for slot 0
var slot = saveSystem.GetSaveSlot(0);

Console.WriteLine($"Save Name: {slot.SaveName}");
Console.WriteLine($"Save Time: {slot.SaveTime}");
Console.WriteLine($"Playtime: {saveSystem.FormatPlaytime(slot.PlaytimeSeconds)}");
Console.WriteLine($"Level: {slot.CurrentLevel}");
Console.WriteLine($"Progress: {slot.ProgressPercent}%");
```

### List All Slots

```csharp
var slots = saveSystem.GetAllSaveSlots();

foreach (var slot in slots)
{
    if (slot.IsValid)
    {
        Console.WriteLine($"Slot {slot.SlotIndex}: {slot.SaveName}");
    }
}
```

### Delete Save

```csharp
saveSystem.DeleteSaveSlot(2);
Console.WriteLine("Save deleted");
```

### Check If Slot Has Save

```csharp
if (saveSystem.SlotHasSave(0))
{
    Console.WriteLine("Slot 0 has a save");
}
```

## Custom Save Data

### Save Entity-Specific Data

```csharp
var npc = new Entity(50);
npc.AddComponent(new Transform3D { Position = new Vector3(5, 0, 0) });

var saveable = new SaveableComponent("merchant_npc")
{
    SaveData = new Dictionary<string, object>
    {
        { "talked_times", 3 },
        { "quest_given", true },
        { "inventory_open", false }
    }
};
npc.AddComponent(saveable);

// Later, retrieve the data
var savedData = saveable.SaveData;
int talkCount = (int)savedData["talked_times"];
```

### Custom Save Callbacks

```csharp
var entity = new Entity(100);
var saveable = new SaveableComponent("custom_entity");

// Custom get state logic
saveable.OnGetState += () =>
{
    var state = new EntityState { EntityId = entity.Id };
    
    // Custom data capture
    if (entity.TryGetComponent<Health>(out var health))
    {
        state.CustomData["current_health"] = health.CurrentHealth;
        state.CustomData["max_health"] = health.MaxHealth;
    }
    
    return state;
};

// Custom restore logic
saveable.OnRestoreState += (state) =>
{
    if (state.CustomData.TryGetValue("current_health", out var health))
    {
        // Apply custom restoration
    }
};

entity.AddComponent(saveable);
```

## Quick Save/Load

### Simple Quick Save

```csharp
// Save to slot 0
saveSystem.SaveGame(0, "QuickSave", entities);

// Load from slot 0
saveSystem.LoadGame(0, entities);
```

### Auto-Save

```csharp
public class AutoSaveManager
{
    private float _autoSaveInterval = 300f; // 5 minutes
    private float _timeSinceLastSave = 0f;

    public void Update(float deltaTime, SaveLoadSystem saveSystem, IEnumerable<Entity> entities)
    {
        _timeSinceLastSave += deltaTime;

        if (_timeSinceLastSave >= _autoSaveInterval)
        {
            saveSystem.SaveGame(9, "AutoSave", entities);
            _timeSinceLastSave = 0;
        }
    }
}
```

## Save Menu Integration

```csharp
// Create save slot selection menu
var menu = new MenuComponent(MenuComponent.MenuType.CustomMenu, "Save Game");

for (int i = 0; i < 10; i++)
{
    var slot = saveSystem.GetSaveSlot(i);
    string buttonText = slot.IsValid 
        ? $"Slot {i}: {slot.SaveName} - {slot.SaveTime:MM/dd HH:mm}"
        : $"Slot {i}: <Empty>";

    int slotIndex = i;
    menu.AddButton(new MenuButton(buttonText, () =>
    {
        saveSystem.SaveGame(slotIndex, $"Save {DateTime.Now:HH:mm}", entities);
    }));
}

menu.AddButton(new MenuButton("Cancel", () => menuSystem.CloseCurrentMenu()));
menuSystem.OpenMenu(menu);
```

## Best Practices

1. **Make entities saveable early** - Add SaveableComponent when creating
2. **Use meaningful save IDs** - e.g., "npc_merchant", "enemy_boss_1"
3. **Track playtime** - Monitor for achievements
4. **Store game state** - Flags, quest progress, etc.
5. **Auto-save frequently** - Don't lose too much progress
6. **Validate saves** - Check version before loading
7. **Clear old data** - Don't keep outdated saves
8. **Test edge cases** - Load with missing entities, corrupt files, etc.

## Troubleshooting

### Save Not Working?
- Check SaveDirectory exists
- Verify file permissions
- Check disk space available
- Validate entity components

### Load Fails?
- Verify save file exists
- Check game version matches
- Ensure entities exist before loading
- Check JSON format

### Data Not Persisting?
- Verify SaveableComponent attached
- Check CustomData dictionary
- Confirm OnGetState implementation
- Validate JSON serialization

---

See **SaveLoadExamples.cs** for complete working examples!
