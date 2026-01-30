# Precision Platformer – Controls & Feel

GAME 326 – Applied Principles: Programming

Professor: Tim Lindsey

Assignment Due: Session 6 (In-Class Live Demo)

---

## Assignment Overview

In this assignment, you will work from a provided Unity project template to build a precision platformer focused on input handling, movement feel, and visual feedback.

**This is not a clone exercise.**

You are not being graded on level design, art quality, or feature count. You are being evaluated on whether you understand why the provided architecture exists, how to extend it correctly, and how input flows from hardware to character movement.

**You will implement two new movement mechanics that work together, and design a level that requires both.**

A mechanic that works but feels bad is incomplete. A mechanic that feels good demonstrates understanding. Two mechanics that complement each other demonstrate mastery.

**This is a team assignment (2 students).**

One programmer focus, one artist focus. Both teammates write code and both contribute to feel.

**Template Repository:**

https://github.com/profangrybeard/PlatformerTemplate

---

## Core Goal (Non-Negotiable)

Your game must include:

- Working walk and jump mechanics (provided in template)
- **Two new movement mechanics** implemented by your team
- A playable level that **requires both mechanics** to complete
- Animations that communicate player state
- Visual feedback (particles, screen shake, or equivalent juice)

Both mechanics must be playable and feel intentional. The level must demonstrate that you understand how your mechanics interact — if either mechanic can be ignored to complete the level, the assignment is incomplete.

---

## Required Architectural Understanding

You are expected to understand, use, and extend the following systems from the template:

- **ServiceLocator** — How systems find each other
- **InputReader** — How hardware input becomes game input
- **PlayerController** — How input becomes movement
- **MovementConfig / InputConfig** — How tunable values are separated from code

You must be able to explain what each system does, why it exists, and how input flows from button press to character movement.

---

## Mechanic Options

Choose TWO movement mechanics to implement. They should complement each other — think about how players will chain them together.

| Mechanic | Description |
|----------|-------------|
| **Wall Slide & Jump** | Stick to walls, slide down slowly, jump off |
| **Dash** | Quick horizontal burst with cooldown |
| **Climb** | Grab and climb specific surfaces |
| **Double Jump** | Second jump in mid-air with distinct feel |
| **Ground Pound** | Fast fall attack with landing impact |

**Strong combinations** (mechanics that create interesting interactions):
- Wall Slide + Dash (dash to reach walls, wall jump to continue)
- Double Jump + Ground Pound (reach heights, then slam down)
- Climb + Dash (dash to grab ledges, climb up)

**Weak combinations** (mechanics that don't interact much):
- Double Jump + Wall Slide (both solve the same problem: reaching heights)

Other mechanics may be proposed to the instructor for approval.

---

## Content Requirements

### Code Requirements

| Requirement | Description |
|-------------|-------------|
| Two mechanic scripts | Each follows template patterns (uses ServiceLocator, InputReader) |
| Config ScriptableObjects | Tunable values for each mechanic (one config per mechanic) |
| Exposed state | Animator can read all mechanic states (IsWallSliding, IsDashing, etc.) |
| State interaction | Mechanics must handle transitions between each other cleanly |

### Level Design Requirements

| Requirement | Description |
|-------------|-------------|
| Playable level | A complete level with a clear start and end |
| Mechanic gates | Sections that **require** Mechanic 1 to pass |
| Mechanic gates | Sections that **require** Mechanic 2 to pass |
| Combination challenge | At least one section requiring **both mechanics together** |
| Readable layout | Player can see where to go; mechanics are taught before tested |

The level is not graded on art quality. It is graded on whether it demonstrates understanding of your mechanics and how they interact.

### Animation Requirements

| Requirement | Description |
|-------------|-------------|
| Core states | Idle, Run, Jump, Fall, Land |
| Mechanic states | Animations for **both** chosen mechanics |
| PlayerAnimator script | Drives Animator parameters from gameplay state |
| Clean transitions | No animation pops or broken blends between mechanic states |

### Juice Requirements

At least THREE of the following (mechanics need feedback):

- Particle effects (dust, trails, impact)
- Screen shake on impactful actions
- Squash and stretch on jump/land
- Visual feedback for mechanic activation
- Audio cues for mechanic activation (optional but encouraged)

---

## Rules & Constraints

### Required

- Input reading happens in `Update()`, physics in `FixedUpdate()`
- Cross-system access uses ServiceLocator
- Tunable values live in ScriptableObject configs, not hardcoded
- Both keyboard and controller input must work
- Animations respond to actual gameplay state, not faked

### Forbidden

- Hardcoded gameplay values in MonoBehaviours
- Direct references between systems that bypass ServiceLocator
- Animations that don't match gameplay state
- Builds that only work in the editor

---

## Student Learning Outcomes

By completing this assignment, students will demonstrate the ability to:

1. **Analyze and extend an existing codebase** — Read, understand, and modify a provided Unity project template while preserving architectural intent.

2. **Implement responsive input handling** — Use Unity's Input System with proper deadzone processing and input buffering.

3. **Create movement systems with good game feel** — Apply acceleration, coyote time, variable jump height, and gravity scaling.

4. **Integrate animation with gameplay** — Drive Animator parameters from code and create clean state transitions.

5. **Add visual feedback that reinforces feel** — Implement juice elements that make actions feel impactful.

6. **Collaborate effectively** — Work as a team where both members contribute to code and feel.

7. **Communicate technical reasoning** — Verbally explain system behavior, input flow, and design decisions during a live demo.

---

## Final Demo (In-Class)

There are no slides and no formal presentation.

You will:

- Launch your game
- Play it live
- Explain what is happening while the game is running

While playing, you must be able to explain:

- How input flows from button press to character movement
- How each mechanic works and why you made specific choices
- How the two mechanics interact (state transitions, input priority)
- Why your level requires both mechanics
- How animations connect to gameplay state
- One architectural decision you now understand because of this project

**This is a live technical demo, not a speech.**

---

## Grading Rubric

| Component | Points | Description |
|-----------|--------|-------------|
| **Mechanics** | 30 | Both mechanics work correctly, feel intentional, and interact cleanly |
| **Level Design** | 25 | Level requires both mechanics; teaches before it tests; has clear flow |
| **Animation & Feedback** | 20 | Animations match gameplay state; juice reinforces actions |
| **Code Quality** | 15 | Follows template patterns; proper architecture; clean state management |
| **Presentation** | 10 | Clear demo; can explain input flow, mechanic interaction, and design choices |
| **Total** | 100 | |

---

## Project Delivery Instructions

You are responsible for delivering a complete, runnable project.

### Submission Format

Submit a single ZIP file named:

**`Lastname_FirstInitial_GAME326_PrecisionPlatformer.zip`**

Example: `Smith_J_GAME326_PrecisionPlatformer.zip`

For team submissions, use the programmer's name.

### Required Contents

Your ZIP must contain:

| Item | Description |
|------|-------------|
| **Windows Build** | A folder containing your built .exe and all required files |
| **Project ZIP** | Your complete Unity project as a separate .zip inside the main zip |
| **Video** | A 1-minute (max) gameplay video showing both mechanics and the level |

### Build Instructions

1. **File > Build Settings**
2. Set Platform to **Windows**
3. Click **Build**
4. Choose a folder named `Build` inside your project
5. Test the build on a different computer if possible

### Video Requirements

- Maximum 1 minute
- Show both mechanics being used
- Show the level being completed
- No narration required (but allowed)
- Screen recording is fine (OBS, Windows Game Bar, etc.)

### Also Required

- ✅ Push all code to your team branch on GitHub
- ✅ Ensure project runs from a fresh clone
- ✅ Participate in the live in-class demo

**If it only works on your machine, it is not finished.**

### Common Submission Errors (Avoid These)

- Missing ScriptableObject assets (configs not saved)
- Input Actions asset not included or not assigned
- Code that only works in the editor
- Mechanic that is broken or feels accidental
- Animations that don't match gameplay state
- Only one teammate's commits in the repo
- Level can be completed without using both mechanics
- Mechanics conflict or break each other's states

These issues indicate lack of testing or incomplete work.

---

## Resources

- [Unity Input System Documentation](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
- [Celeste & TowerFall Physics (GDC Talk)](https://www.youtube.com/watch?v=yorTG9at90g)
- [The Art of Screenshake](https://www.youtube.com/watch?v=AJdEqssNZ-U)
- [Juice It or Lose It](https://www.youtube.com/watch?v=Fy0aCDmgnxg)

For Git help, see **GIT_GUIDE.md** in this repository.

---

## Questions?

Ask your instructor or post in the course Discord.
