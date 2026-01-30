# Assembly Guide

Follow these steps exactly. Don't skip ahead.

---

## Step 1: Create Input Actions Asset

1. In Project window, navigate to `Assets/_Project`
2. Right-click > **Create > Input Actions**
3. Name it `PlayerInputActions`
4. Double-click to open the Input Actions editor

### Configure the Asset

In the Input Actions window:

1. Click the **+** next to "Action Maps" (left panel)
2. Name the new map `Player`

### Add Move Action

1. With Player map selected, click **+** next to "Actions" (middle panel)
2. Name it `Move`
3. In the right panel, set **Action Type** to `Value`
4. Set **Control Type** to `Vector 2`

Now add bindings:

**For Gamepad:**
1. Click **+** next to Move action > **Add Binding**
2. Click the new binding, then in right panel click **Path** dropdown
3. Select: **Gamepad > Left Stick**

**For Keyboard:**
1. Click **+** next to Move action > **Add 2D Vector Composite**
2. Name it `WASD`
3. Click **Up**, set Path to: **Keyboard > W**
4. Click **Down**, set Path to: **Keyboard > S**
5. Click **Left**, set Path to: **Keyboard > A**
6. Click **Right**, set Path to: **Keyboard > D**

### Add Jump Action

1. Click **+** next to "Actions"
2. Name it `Jump`
3. Action Type should be `Button` (default)

Add bindings:

1. Click **+** next to Jump > **Add Binding**
2. Set Path to: **Keyboard > Space**

3. Click **+** next to Jump > **Add Binding**
4. Set Path to: **Gamepad > Button South** (A on Xbox, X on PlayStation)

### Save

1. Click **Save Asset** (top of Input Actions window)
2. Close the Input Actions window

---

## Step 2: Create Config Assets

### Input Config

1. In Project window, still in `Assets/_Project`
2. Right-click > **Create > Platformer > Input Config**
3. Name it `DefaultInputConfig`
4. Leave default values for now

### Movement Config

1. Right-click > **Create > Platformer > Movement Config**
2. Name it `DefaultMovementConfig`
3. Leave default values for now (we'll tune later)

---

## Step 3: Set Up Ground Layer

1. Menu: **Edit > Project Settings**
2. Select **Tags and Layers** (left panel)
3. Scroll to **Layers** section
4. Click an empty User Layer slot (Layer 6 or higher)
5. Name it `Ground`
6. Close Project Settings

---

## Step 4: Create the Scene

### Open Your Scene

1. Open the scene you'll be working in (or create new: **File > New Scene**)
2. Save it as `DevPlayground` in `Assets/_Project`

### Create Ground Platform

1. Menu: **GameObject > 2D Object > Sprites > Square**
2. Name it `Ground`
3. Set Transform:
   - Position: (0, -2, 0)
   - Scale: (20, 1, 1)
4. In Inspector, set **Layer** to `Ground`
5. Add Component: **Box Collider 2D** (if not already present)

### Create Player

1. Menu: **GameObject > Create Empty**
2. Name it `Player`
3. Set Position to (0, 0, 0)

Add components to Player:

4. Add Component: **Sprite Renderer**
   - Click the circle next to Sprite field
   - Search for and select `Capsule` (built-in capsule shape)
   - Set Color to something visible (cyan, white, etc.)
   - Note: The default capsule is 1 unit wide by 2 units tall

5. Add Component: **Rigidbody 2D**
   - Collision Detection: `Continuous`
   - Interpolate: `Interpolate`
   - Freeze Rotation: check **Z** (under Constraints)

6. Add Component: **Capsule Collider 2D**
   - This matches our capsule sprite shape
   - Default size (1 x 2) should match the sprite

7. Add Component: **Input Reader** (our script from Platformer.Player)
   - Drag `PlayerInputActions` asset into **Input Actions** field
   - Drag `DefaultInputConfig` asset into **Config** field

8. Add Component: **Player Controller** (our script from Platformer.Player)
   - Drag `DefaultMovementConfig` asset into **Config** field
   - Leave Ground Check Point empty for now (next step)

### Create Ground Check Point

1. Right-click on Player in Hierarchy > **Create Empty**
2. Name it `GroundCheck`
3. Set Position to **(0, -1, 0)** — this places it at the feet of the capsule (which is 2 units tall, centered at origin)
4. Select Player again
5. Drag `GroundCheck` into the **Ground Check Point** field on Player Controller

**IMPORTANT:** The GroundCheck position must be at the player's feet. If your player sprite is a different size, adjust the Y position so GroundCheck sits just below the collider. Jump will not work if this is positioned incorrectly.

### Configure Movement Config

1. Select `DefaultMovementConfig` in Project window
2. In Inspector, find **Ground Layer**
3. Click the dropdown and check `Ground`

---

## Step 5: Set Up Camera

1. Select **Main Camera** in Hierarchy
2. Set Position to (0, 0, -10)
3. Set Size to 8 (or adjust for desired zoom)

Optional - make camera follow player:
1. Create new script or just parent camera to player for now
2. (We'll add proper camera follow later)

---

## Step 6: Test

1. Press **Play**
2. Test controls:
   - **A/D or Left Stick**: Move left/right
   - **Space or South Button**: Jump
3. Verify:
   - [ ] Player moves left and right
   - [ ] Player jumps when pressing jump button
   - [ ] Player lands on ground
   - [ ] Player can't double jump (no air jump)

---

## Troubleshooting

### Player doesn't move

Check Console for errors. Common issues:
- "InputActionAsset not assigned" → Drag PlayerInputActions into InputReader component
- "'Player' action map not found" → Open PlayerInputActions, make sure map is named exactly `Player`
- "'Move' action not found" → Make sure action is named exactly `Move`

### Player falls through ground

- Check Ground object has **Box Collider 2D** component
- Check Ground object's **Layer** is set to `Ground`
- Check `DefaultMovementConfig` has **Ground Layer** set to include `Ground`

### Player doesn't jump

- Check Console for "InputReader not found" error
- **Most common issue:** GroundCheck is not at the player's feet
  - Select GroundCheck in Hierarchy
  - Set Y position to -1 (for default 2-unit tall capsule)
  - The GroundCheck must be at or slightly below the bottom of the collider
- Check ground detection: Select Player, look at GroundCheck in Scene view.
  A green line should appear when grounded, red when airborne.

### Jump feels wrong

This is expected! Default values are a starting point. We'll tune in the next section.

---

## Next: Tuning

Once everything works, try adjusting these in `DefaultMovementConfig`:

| Feeling | Adjust |
|---------|--------|
| Too slow | Increase `maxSpeed` |
| Sluggish start | Increase `acceleration` |
| Slides too much | Increase `deceleration` |
| Jump too low | Increase `jumpForce` |
| Jump too floaty | Increase `fallGravityMultiplier` |
| Inputs feel dropped | Increase `coyoteTime` or `jumpBufferDuration` |

Play. Adjust one value. Play again. Repeat.
