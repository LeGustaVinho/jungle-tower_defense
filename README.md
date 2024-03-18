# Jungle - Tower Defense - Game Engineer Unity Challenge

**Table of Contents**

[TOCM]

### Business Rules:

- [x] Enemies come in infinite waves and try to reach the goal line.
- [x] If an enemy reaches the goal line, the player loses Health Points.
- [x] The player can buy new defenses and upgrade existing defenses with currency
at any time.
- [x] There should be no predefined path. The player should be able to place towers
anywhere, unless it completely blocks the enemies movement.
- [x] For each enemy killed the player receives a currency reward and score points.
- [x] The game ends when the player runs out of Health Points.
- [x] The player's score is saved on a leaderboard (locally).
- [x] Creating at least 3 types of enemies and 3 types of defenses.
- [x] Using Unity Primitives is recommended.
- [x] How the enemy moves, how much damage they do, how the defenses work and all the game economy/balance is up to you.
- [x] It must run out of the box without any compile errors.

### Editor and Executable

- Made with Unity 2021.3.36 LTS
- Executable run on portrait, 734x1305 pixels (9:16)
- Coded with JetBrains Rider

### Architecture

The architecture used is similar to MVC, because of the separation of responsibilities between business rules, visualization, data and flow control, with a mix of microservices and Event Driven and following SOLID principles.

Examples:

- View Classes:
	- Entity (MonoBehaviour)
	- NpcEntity (MonoBehaviour)
	- StructureEntity (MonoBehaviour)
	- ProjectileEntity (MonoBehaviour)
	- GridDrawer (MonoBehaviour)
	- StartScreen (User Interface)
	- InGameScreen (User Interface)
	- UIStructure (User Interface)

- Data Classes:
	 - LevelConfig (ScriptableObject)
	 - PlayerConfig (ScriptableObject)
	 - EntityConfig (ScriptableObject)
	 - StructureBuilderConfig (ScriptableObject)
	 - Player

- Controllers and Systems Classes:
	- GameInitialization (MonoBehaviour)
	- Leaderboard
	- TimeManager
	- CombatSystem
	- TargetSystem
	- StructureBuilder
	- ScreenController
	- LevelController

### Scalability & Maintainability & Extensibility

- It has a single entry point in the *GameInitialization* class, which instantiates the main services/systems and distributes their dependencies.
- All game data is separated into ScriptableObject classes with a *Config* suffix, making it possible to change the entire Level Design/Balancing mechanics, create new enemies or structures, etc. without needing to change the code, making it user-friendly for Game Designers and Artists.
- Systems communicate with each other via events, making extension easy.
- Systems have interfaces so they can be mocked in unit tests.
- The use of MonoBehaviour was reduced to a minimum to make the code less dependent on Unity.
- Some unit uniting to test system stability

### Perfomance

- All instantiation of NPCs and projectiles was done using a Pool with the aim of reducing the allocation and deallocation of resources, avoiding performance problems.
- I didn't investigate in depth, but Unity's NavMesh takes 1 to 2 frames to be recalculated every time a dynamic obstacle is inserted, causing a brief stop in the movement of NPCs.
- It wasn't necessary due to the lack of use of art and graphics, but I would have used Addressables to load the assets dynamically.


### Pathfinding

The solution used to pathfind the NPCs was from Unity itself called NavMesh, which comes out-of-box, with no implementation challenges.

NPCs have the NavMeshAgent component that already performs all the movement mechanics and uses pathfinding.

The scene has a surface that will be used for pathfinding that is already baked without obstacles.

The structures (towers) have the NavMeshObstacle component that already performs all the mechanics of dynamically occupying space and blocking passage in the NavMesh.

The detection algorithm of NPCs paths blocking by structures (towers) also implemented with Unity's NavMesh.

If it were not possible to use Unity's NavMesh, I would create a grid waypoint system from scratch throughout the entire scenario, use the A* algorithm to calculate the route and dynamically calculate whether a waitpoint would be available or not if it were blocked by a tower. .


### Notes:

- An external library of my own called Legendary Tools was used, all the code is in my Git Hub, this library is also hosted in the package manager called [OpenUPM](https://openupm.com/packages/com.legustavinho.legendary-tools-common/?subPage=related "OpenUPM").
- The following external third-party tools were used:
	- [Odin Inspector](https://odininspector.com/ "Odin Inspector"): Makes inspector customization and serialization easy.
	- Newtonsoft JSON: Official Unity fork of the JSON serialization lib
	- [Moq](https://github.com/devlooped/moq "Moq"): Friendly mocking library for .NET