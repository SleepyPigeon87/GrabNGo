# Precision Platformer Template

A teaching-focused 2D platformer template that demonstrates proper input handling, movement physics, and game feel.

## What's In The Box

```
Scripts/
├── Core/
│   └── ServiceLocator.cs    # Simple dependency injection
├── Config/
│   ├── InputConfig.cs       # Deadzones, buffering settings
│   └── MovementConfig.cs    # Speed, acceleration, jumping
└── Player/
    ├── InputReader.cs       # Input reading & processing
    └── PlayerController.cs  # Physics & movement
```

## Quick Setup

### 1. Create Input Actions

1. Right-click in Project: **Create > Input Actions**
2. Name it `PlayerInputActions`
3. Double-click to open the Input Actions editor
4. Add Action Map named `Player`
5. Add these actions:

| Action | Type | Bindings |
|--------|------|----------|
| Move | Value (Vector2) | Gamepad Left Stick, WASD/Arrows (2D Vector Composite) |
| Jump | Button | Gamepad South Button, Spacebar |

6. Save and close

### 2. Create Config Assets

Right-click in Project > **Create > Platformer**:
- Create **Input Config** → name it `DefaultInputConfig`
- Create **Movement Config** → name it `DefaultMovementConfig`

### 3. Set Up Layers

1. Edit > Project Settings > Tags and Layers
2. Add a layer named `Ground`

### 4. Create the Player

1. Create empty GameObject, name it `Player`
2. Add components:
   - **Rigidbody2D**
   - **BoxCollider2D** (or CapsuleCollider2D)
   - **InputReader** (our script)
   - **PlayerController** (our script)
3. Create child empty GameObject at feet, name it `GroundCheck`
4. Assign references:
   - InputReader: `PlayerInputActions` asset, `DefaultInputConfig`
   - PlayerController: `DefaultMovementConfig`, `GroundCheck` transform
5. In MovementConfig, set Ground Layer to include your `Ground` layer

### 5. Create Ground

1. Create empty GameObject, name it `Ground`
2. Add **BoxCollider2D**, scale to be a platform
3. Add **SpriteRenderer** if you want to see it (use Unity's built-in white square)
4. Set Layer to `Ground`

### 6. Add Visual (Optional)

Add a SpriteRenderer to Player with any square sprite to see it.

### 7. Play!

Press Play. WASD/Arrow keys or left stick to move, Space/A button to jump.

---

## Understanding the Code

### Read These Files In Order

1. **ServiceLocator.cs** - The simplest file. Explains the pattern.
2. **InputConfig.cs** - Explains deadzones and input buffering concepts.
3. **MovementConfig.cs** - Explains every tunable value and what it affects.
4. **InputReader.cs** - How hardware input becomes game input.
5. **PlayerController.cs** - How input becomes movement.

Every file has extensive comments explaining the WHY, not just the WHAT.

### Key Concepts Taught

**Input Handling:**
- Circular deadzones and why they matter
- Input buffering for responsive controls
- Keyboard vs controller parity
- Device detection

**Movement Physics:**
- Acceleration-based movement (not instant velocity)
- Turn-around boost for snappy direction changes
- Air control multiplier
- Direct velocity control vs AddForce

**Jumping:**
- Variable jump height (tap vs hold)
- Coyote time (grace period after leaving ground)
- Input buffering (press jump before landing)
- Gravity scaling (fall faster than rise)

---

## Tuning Guide

### It Feels Sluggish
- Increase `acceleration` in MovementConfig
- Increase `turnAroundMultiplier`
- Decrease `deceleration` slightly

### It Feels Too Slippery
- Increase `deceleration`
- Decrease `acceleration` slightly

### Jumps Feel Floaty
- Increase `fallGravityMultiplier`
- Decrease `jumpForce` and compensate with higher gravity

### Jumps Feel Too Snappy
- Decrease `fallGravityMultiplier`
- Increase `jumpForce`

### Inputs Feel Dropped
- Increase `jumpBufferDuration` in InputConfig
- Increase `coyoteTime` in MovementConfig

### Controller Stick Drifts
- Increase `deadzone` in InputConfig

---

## Architecture Notes

### Why ServiceLocator?

PlayerController needs InputReader. Options:

1. **GetComponent** - Only works if on same GameObject
2. **Find** - Slow, uses magic strings
3. **Inspector Reference** - Works but gets messy with many connections
4. **ServiceLocator** - Register once, Get anywhere

ServiceLocator is the right tool when you have "services" that multiple things need access to. It's overkill for this small template, but teaches a pattern you'll use in larger projects.

### Why Separate InputReader and PlayerController?

**Separation of concerns:**
- InputReader handles hardware weirdness (deadzones, device detection)
- PlayerController handles game logic (physics, movement rules)

This means you can:
- Change input handling without touching movement code
- Test movement with fake input values
- Support new input devices without rewriting physics
