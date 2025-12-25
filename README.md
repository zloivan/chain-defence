# Chain Defence

A Unity-based tower defense game with a unique match-3 puzzle mechanic. Defend your base by creating chains of matching colored balls to spawn towers that attack incoming enemy waves.

## Preview
![Gameplay](docs/gameplay.gif)
## 🎮 Game Concept

Chain Defence combines tower defense strategy with match-3 puzzle gameplay. Players draw chains by connecting 3 or more balls of the same color on a grid. When a valid chain is created, those balls are destroyed and towers are spawned to defend against waves of enemies approaching your base.

## ✨ Key Features

- **Chain-Based Tower Spawning**: Connect matching colored balls to create chains and spawn defensive towers
- **Multiple Tower Types**: Different tower types with unique attack patterns:
  - Single Target towers
  - AOE (Area of Effect) towers
  - Slow towers that reduce enemy movement speed
  - Chain Lightning towers that bounce damage between enemies
- **Tower Upgrade System**: Towers can be leveled up with modifiers for damage, attack speed, and range
- **Wave-Based Enemy System**: Enemies spawn in waves with increasing difficulty
- **Dynamic Grid System**: Interactive grid-based gameplay with ball physics and reordering
- **Level Progression**: Multiple levels with different maps and difficulty settings
- **Save System**: Progress is saved between sessions

## 🏗️ Project Structure

```
Assets/
├── _Project/              # Main game content
│   ├── Art/              # Visual assets and materials
│   ├── Configs/          # ScriptableObject configurations
│   ├── Models/           # 3D models
│   ├── Prefabs/          # Game object prefabs
│   ├── Scenes/           # Game scenes (MainMenu, Game, Maps)
│   └── Scritps/          # C# game scripts
│       ├── Balls/        # Ball system and spawning
│       ├── ChainManagment/ # Chain validation and visualization
│       ├── Core/         # Core gameplay controllers
│       ├── Enemies/      # Enemy AI and spawning
│       ├── GameGrid/     # Grid system implementation
│       ├── LevelManagement/ # Level loading and progression
│       ├── MapManagement/ # Map configuration
│       ├── PathFinding/  # Enemy pathfinding
│       ├── PlayerBase/   # Player base health management
│       ├── SavingSystem/ # Game save/load functionality
│       ├── Towers/       # Tower logic and projectiles
│       ├── UI/           # User interface components
│       ├── Waves/        # Wave spawning system
│       └── Utilities/    # Helper classes and extensions
├── JMO Assets/           # Cartoon FX and visual effects
├── Plugins/              # Third-party plugins (DOTween, Better Hierarchy, etc.)
└── Settings/             # Unity project settings
```

## 🛠️ Technologies & Dependencies

### Unity Engine
- **Unity Version**: 2021.x or later
- **Render Pipeline**: Universal Render Pipeline (URP)

### Key Packages & Plugins
- **DOTween**: Animation and tweening library
- **TextMeshPro**: Advanced text rendering
- **Cinemachine**: Camera control system
- **UniTask**: Async/await support for Unity
- **Cartoon FX Remaster**: Visual effects for attacks and explosions
- **Better Hierarchy**: Enhanced Unity editor hierarchy

### Custom Systems
- **IKhom Grid System**: Custom grid-based positioning system
- **IKhom Event Bus**: Event-driven architecture for game events
- **IKhom Service Locator**: Dependency injection pattern implementation

## 🎯 Core Gameplay Systems

### Chain System
Players interact with the game by dragging across balls of the same color to create chains. The `ChainValidator` validates connections, ensuring only adjacent balls of matching types can be linked. Chains of 3+ balls can be destroyed to trigger tower spawning.

### Tower System
Towers automatically target and attack enemies within range. Each tower type has unique attack behaviors:
- Prioritizes enemies closest to the base
- Supports multiple attack types (single, AOE, slow, chain)
- Upgradeable with level-based modifiers

### Enemy System
Enemies follow predefined paths toward the player's base. They feature:
- Health scaling based on level difficulty
- Pathfinding along waypoints
- Damage dealing when reaching the base
- Visual feedback for damage and slow effects

### Grid & Board Management
The game board uses a custom grid system where:
- Balls occupy grid positions
- Gravity simulation pulls balls downward when chains are destroyed
- Board refills with new balls after chain destruction
- Guaranteed distribution ensures balanced ball spawning

## 🎨 Visual Features

- Particle effects for attacks, explosions, and chain destruction
- Bloom post-processing effects
- Animated UI with DOTween
- Visual feedback for tower targeting and attack ranges
- Health bars and progress indicators

## 🎵 Audio

- Background music system
- Sound effects for:
  - Ball selection and chain creation
  - Tower attacks
  - Enemy damage and destruction
  - UI interactions

## 📱 Platform

Currently configured for desktop platforms with potential for mobile deployment.

## 🚀 Getting Started

1. Open the project in Unity (2021.x or later)
2. Load the `MainMenu` scene from `Assets/_Project/Scenes/`
3. Press Play to start the game
4. Select a level and start defending!
