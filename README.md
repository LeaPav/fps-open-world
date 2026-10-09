# Open World 3C - Vehicles & Cameras (Unity)
 
An open world prototype focused on the **3 Cs: Characters, Controls, Camera**. The player explores a procedurally generated city on foot in first person, and can enter a **motorcycle**, a **helicopter** or a **train**. Each vehicle has its own pseudo-realistic handling and its own camera behavior.
 
🎬 **Video:** https://youtu.be/d6u69lv0iEo
🌌 **Portfolio:** https://leapav.github.io/lea-pavel-portfolio/
 
## 📸 Screenshots

<p align="center">
  <img src="screenshots/1.webp" width="45%" />
  <img src="screenshots/2.webp" width="45%" />
</p>
<p align="center">
  <img src="screenshots/3.webp" width="45%" />
  <img src="screenshots/4.webp" width="45%" />
</p>

*(more screenshots in the [`screenshots/`](./screenshots) folder)*

---
 
## Features
 
| Mode | What it does |
|---|---|
| **On foot (FPS)** | `CharacterController`-based movement, jump, sprint, headbob and sway driven by `AnimationCurve`s. Yaw is applied on the player, pitch on a separate camera pivot. |
| **Motorcycle** | Fast acceleration, strong braking, inertia when releasing the throttle, cumulative steering whose amplitude depends on speed, lean in turns, speed-based FOV, knockback on impact. Rear third-person follow camera. |
| **Helicopter** | Tilt-based horizontal movement, independent ascent/descent and yaw, animated rotors. Orbital camera you can rotate and zoom, with automatic obstacle avoidance (`SphereCast`). |
| **Train** | Spline-based movement (Unity Splines), very long acceleration and braking, **derails** if the upcoming curve is too sharp for the current speed. Cinematic camera cycling smoothly between 4 predefined positions. |
| **City** | Procedural grid of roads and lane markings, with districts (houses or towers) chosen by Perlin noise. Reproducible through a seed. Rails are placed along the spline procedurally. |
 
## Controls
 
| Action | Input |
|---|---|
| Move / look (on foot) | `Z` `Q` `S` `D` / mouse |
| Enter / exit a nearby vehicle | `E` |
| **Motorcycle** | `Z` accelerate · `S` brake / reverse · `Q` `D` steer |
| **Helicopter** | `Z` `Q` `S` `D` tilt · `↑` `↓` ascend / descend · `←` `→` rotate · mouse orbit · scroll to zoom |
| **Train** | `Z` accelerate · `S` brake |
 
Keyboard layout is AZERTY (ZQSD). Bindings can be changed in `Assets/InputSystem_Actions.inputactions`.
 
## Technical highlights
 
- **Vehicle state machine**: `VehicleStateMachine` switches between `OnFoot` and `Driving`. Every vehicle implements a shared `IVehicle` interface (`EnterVehicle`, `ExitVehicle`, entry and exit points), so adding a new vehicle does not touch the state machine.
- **ScriptableObject event channels** (Observer pattern): vehicles broadcast values such as speed ratio and steering through `FloatEventChannelSO` / `BoolEventChannelSO` assets. Cameras and feedback scripts listen without any direct reference to the vehicle.
- **Separation of control and feedback**: for each vehicle, the movement script handles physics and input, and a separate feedback script handles lean, FOV and rotors.
- **Camera obstacle avoidance**: the helicopter camera snaps closer instantly when an obstacle is detected and eases back out smoothly once the way is clear.
- **Derailment model**: the train looks ahead on the spline, measures the angle between the current and upcoming tangents, and compares `angle / distance × speed` to a threshold.
- **Tunable feel**: speeds, accelerations, inertia, smoothing and curves are exposed in the Inspector.
## Project structure
 
```
Assets/
├── Scripts/
│   ├── Player/          FirstPersonController, PlayerCameraLook
│   ├── Vehicles/        IVehicle, VehicleStateMachine, Moto*, Helicopter*, Train*
│   ├── City/            CityGenerator, RailGenerator
│   └── EventChannels/   FloatEventChannelSO, BoolEventChannelSO
├── Data/EventChannels/  Event channel assets
├── Prefabs/             Player, Vehicles, City
├── Models/              Vehicles and decor
└── Scenes/SampleScene   Main scene
```
 
## Getting started
 
1. Clone the repository.
2. Open it with **Unity 6000.3.21f1** (Unity 6, Universal Render Pipeline).
3. Open `Assets/Scenes/SampleScene` and press Play.
Packages used: Input System 1.20, Splines 2.9, Universal RP 17.3.
 
## Credits
 
- **Development:** Léa PAVEL
- **3D models:**
Motorcycle: https://www.turbosquid.com/FullPreview/1173447  
Helicopter: https://www.turbosquid.com/FullPreview/1579675   
Train: https://www.turbosquid.com/FullPreview/1085665  
Rails: https://www.turbosquid.com/FullPreview/1860505  
