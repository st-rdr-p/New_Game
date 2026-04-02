# Gameplay Mechanics Guide

Complete gameplay mechanics system including enemy AI, combat, physics, collectibles, hazards, and platforms.

## Systems Overview

### Combat System
Handles damage, health, and entity death.
```csharp
var combatSystem = new CombatSystem(game);
game.RegisterSystem(combatSystem);

// Apply damage to entity
combatSystem.ApplyDamage(entity, 25f, "player");

// Listen to damage events
combatSystem.OnDamageApplied += (target, source, damage) =>
{
    Console.WriteLine($"{target.Id} took {damage} damage!");
};

combatSystem.OnEntityDied += (entity) =>
{
    Console.WriteLine($"{entity.Id} died!");
};
```

### Enemy AI System
State machine with Patrol → Chase → Attack behavior.
```csharp
var enemyAISystem = new EnemyAISystem(game);

// Add to enemy
var enemy = game.CreateEntity("enemy");
enemy.AddComponent(new EnemyAI
{
    DetectionRange = 20f,  // When to start chasing
    AttackRange = 3f,      // When to attack
    PatrolSpeed = 5f,
    ChaseSpeed = 12f,
    AttackDamage = 10f,
    AttackCooldown = 1.5f
});

// Listen to state changes
enemyAISystem.OnStateChanged += (entity, newState) =>
{
    Console.WriteLine($"Enemy entered {newState} state");
};
```

### Collectible System
Rings, coins, health pickups, and power-ups.
```csharp
var collectibleSystem = new CollectibleSystem(game);

// Create a ring
var ring = game.CreateEntity("ring");
ring.AddComponent(new Transform3D { Position = new Vector3(5, 0, 0) });
ring.AddComponent(new Collectible(Collectible.CollectibleType.Ring, 10));

// Create health pickup
var health = game.CreateEntity("health_item");
health.AddComponent(new Collectible(Collectible.CollectibleType.HealthPickup, 25));

// Listen to collection
collectibleSystem.OnCollected += (player, item, type) =>
{
    Console.WriteLine($"Collected {type}!");
};
```

### Hazard System
Spikes, lava, ice, electricity with damage and knockback.
```csharp
var hazardSystem = new HazardSystem(game);

var spike = game.CreateEntity("spike");
spike.AddComponent(new Transform3D { Position = new Vector3(0, 0, -5) });
spike.AddComponent(new Hazard(Hazard.HazardType.Spike, 20f));

hazardSystem.OnEntityHitHazard += (entity, hazard) =>
{
    Console.WriteLine($"{entity.Id} hit a hazard!");
};
```

### Platform System
Moving platforms, conveyor belts, and trampolines.

**Moving Platform**
```csharp
var platform = game.CreateEntity("moving_platform");
platform.AddComponent(new MovingPlatform(
    startPosition: new Vector3(0, 0, 0),
    endPosition: new Vector3(10, 0, 0),
    speed: 2f
));
```

**Conveyor Belt**
```csharp
var conveyor = game.CreateEntity("conveyor");
conveyor.AddComponent(new ConveyorBelt(
    direction: Vector3.Right,
    speed: 8f
));
```

**Trampoline**
```csharp
var trampoline = game.CreateEntity("trampoline");
trampoline.AddComponent(new Trampoline(bounceForce: 40f));
```

### Player Movement System
Handles player input and physics-based movement.
```csharp
var playerInputHandler = new PlayerInputHandler(inputManager);
var playerMovementSystem = new PlayerMovementSystem(game, playerInputHandler);

var player = game.CreateEntity("player");
player.AddComponent(new PlayerController
{
    Speed = 15f,
    JumpForce = 15f,
    MaxSpeed = 25f,
    CanDoubleJump = true,
    Drag = 0.98f
});
```

## Components Reference

### Health
```csharp
var health = new Health(maxHealth: 100);
health.TakeDamage(25);
health.Heal(10);
bool isDead = health.IsDead;
float percent = health.GetHealthPercent();
```

### Rigidbody3D
```csharp
var rb = new Rigidbody3D
{
    Mass = 1.0f,
    UseGravity = true,
    Drag = 0.1f
};
rb.ApplyForce(new Vector3(10, 0, 0));
rb.ApplyImpulse(new Vector3(0, 15, 0)); // Jump
```

### PlayerController
```csharp
var controller = new PlayerController
{
    Speed = 15f,
    JumpForce = 15f,
    DashForce = 40f,
    DashCooldown = 1f
};
controller.PerformDash(direction);
```

### EnemyAI
Properties:
- `DetectionRange` - Distance to start chasing
- `AttackRange` - Distance to start attacking
- `PatrolSpeed` - Walk speed when patrolling
- `ChaseSpeed` - Run speed when chasing
- `AttackDamage` - Damage per attack
- `AttackCooldown` - Seconds between attacks
- `CurrentState` - Patrol/Chase/Attack/Dead

### PowerUpComponent
```csharp
var powerUp = new PowerUpComponent();
powerUp.ApplyPowerUp("SpeedBoost", duration: 10f);

if (powerUp.IsActive)
{
    // Apply effect
}
```

## Complete Scene Setup

```csharp
var game = new Game();
var inputManager = new InputManager();

// Create all systems
var combatSystem = new CombatSystem(game);
var enemyAISystem = new EnemyAISystem(game);
var collectibleSystem = new CollectibleSystem(game);
var hazardSystem = new HazardSystem(game);
var platformSystem = new PlatformSystem(game);
var playerInputHandler = new PlayerInputHandler(inputManager);
var playerMovementSystem = new PlayerMovementSystem(game, playerInputHandler);

game.RegisterSystem(combatSystem);
game.RegisterSystem(enemyAISystem);
game.RegisterSystem(collectibleSystem);
game.RegisterSystem(hazardSystem);
game.RegisterSystem(platformSystem);
game.RegisterSystem(playerMovementSystem);

// Create player
var player = game.CreateEntity("player");
player.AddComponent(new Tag("Player"));
player.AddComponent(new Transform3D());
player.AddComponent(new Rigidbody3D());
player.AddComponent(new Health(100));
player.AddComponent(new PlayerController());

// Create enemy
var enemy = game.CreateEntity("enemy");
enemy.AddComponent(new Transform3D { Position = new Vector3(15, 0, 0) });
enemy.AddComponent(new Health(50));
enemy.AddComponent(new EnemyAI());

// Create environment
for (int i = 0; i < 5; i++)
{
    var ring = game.CreateEntity($"ring_{i}");
    ring.AddComponent(new Transform3D { Position = new Vector3(5 + i * 2, 0, 0) });
    ring.AddComponent(new Collectible(Collectible.CollectibleType.Ring, 10));
}

// Game loop
while (gameRunning)
{
    float deltaTime = 0.016f; // 60fps
    
    combatSystem.Update(deltaTime);
    enemyAISystem.Update(deltaTime);
    collectibleSystem.Update(deltaTime);
    hazardSystem.Update(deltaTime);
    platformSystem.Update(deltaTime);
    playerMovementSystem.Update(deltaTime);
    
    // Render...
}
```

## Enemy AI State Transitions

```
       Patrol (outside range)
          ↓     ↑
      (detect)  (lose)
          ↓     ↑
       Chase (in detection range)
          ↓     ↑
      (close)   (far)
          ↓     ↑
       Attack (in attack range)
          │
        (die)
          ↓
        Dead
```

## Physics Integration

The system supports:
- Gravity and falling
- Velocity and acceleration
- Mass-based force application
- Drag for air resistance
- Platform carrying (stays on top of moving platforms)
- Knockback from damage
- Bouncing from trampolines

## Performance Tips

1. Use pooling for frequently created objects (projectiles, particles)
2. Cache component references where possible
3. Only check collisions for nearby entities
4. Use simple distance checks instead of physics colliders
5. Batch damage checks per frame rather than per hit

## Extending the System

Add custom hazard types:
```csharp
public enum HazardType
{
    Spike,
    Lava,
    Ice,        // New!
    Electricity // New!
}
```

Add custom power-ups:
```csharp
public enum PowerUpType
{
    SpeedBoost,
    Invincibility,
    Shield,        // New!
    DoubleJump     // New!
}
```

Add custom collectible types:
```csharp
public enum CollectibleType
{
    Ring,
    Coin,
    HealthPickup,
    PowerUp,
    Key,           // New!
    Treasure       // New!
}
```
