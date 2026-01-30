# Session Plan: Precision Platformer Project

## Course Structure

**Format:** 2.5 hour sessions
- 20 min lecture
- 45 min work
- 15 min break
- 20 min lecture
- Remaining time: work

**Teams:** 2 students (1 Programmer focus, 1 Artist focus — both code and design)

---

## Session Overview

| Session | Focus | Lecture Topics |
|---------|-------|----------------|
| 1 | Setup & Foundations | Git basics, template architecture, input systems |
| 2 | Mechanic 1 | Extending existing systems, state management |
| 3 | Mechanic 2 & Integration | Multi-mechanic state, animation state machines |
| 4 | Level Design & Juice | Level design for mechanics, visual feedback |
| 5 | Tuning & Testing | Parameter tuning, playtesting methodology |
| 6 | Presentation | Live demos |

---

## Session 1: Setup & Foundations

### Lecture 1 (20 min): Git & Collaboration
- What is version control and why it matters
- Clone, branch, commit, push workflow
- Lab machine constraints (no GitHub Desktop)
- Live demo: clone template, create branch

### Work Block 1 (45 min)
- Both teammates: Git setup, clone repo, create team branch
- Verify template runs in Unity

### Lecture 2 (20 min): Template Architecture
- ServiceLocator pattern — what and why
- InputReader/PlayerController separation
- ScriptableObject configs for tuning
- Code walkthrough: trace a jump from input to physics

### Work Block 2 (remaining)
- Read all template code files
- Test template with keyboard and controller
- Discuss and choose TWO mechanics as a team (think about how they'll interact)
- First commit checkpoint

---

## Session 2: Mechanic 1 Implementation

### Lecture 1 (20 min): Extending the Controller
- How to add a new movement mechanic
- Creating a new controller script
- Creating config ScriptableObjects
- Exposing state for other systems

### Work Block 1 (45 min)
- **Programmer:** Begin first mechanic implementation
- **Artist:** Set up Animator Controller, begin idle/run animations

### Lecture 2 (20 min): Animation Fundamentals for Programmers
- Animator parameters and how code drives them
- State machine basics
- Why animation timing matters for feel

### Work Block 2 (remaining)
- **Programmer:** Get first mechanic working (basic version)
- **Artist:** Complete idle, run, jump, fall animations
- Commit checkpoint

---

## Session 3: Mechanic 2 & Integration

### Lecture 1 (20 min): Managing Multiple Mechanics
- State interaction: what happens when both inputs fire?
- Priority systems and state machines
- Preventing conflicting states
- Input buffering across mechanics

### Work Block 1 (45 min)
- **Programmer:** Begin second mechanic, plan state transitions
- **Artist:** First mechanic animations, start second mechanic

### Lecture 2 (20 min): Connecting Animation to Code
- Creating a PlayerAnimator script
- Caching animator parameter hashes
- Driving parameters from gameplay state
- Transition conditions in the Animator

### Work Block 2 (remaining)
- **Programmer:** Both mechanics working, transitions clean
- **Artist:** Both mechanics animated, PlayerAnimator connected
- Test: Can you use both mechanics without bugs?
- Commit checkpoint

---

## Session 4: Level Design & Juice

### Lecture 1 (20 min): Level Design for Mechanics
- **Why this matters:** Your level proves you understand your mechanics
- Teach before you test: introduce mechanics safely
- Gating: sections that REQUIRE a specific mechanic
- Combination challenges: requiring both mechanics together
- The artist must PLAY to design — feel comes from iteration

### Work Block 1 (45 min)
- **Artist:** Begin level layout — paper sketch first, then blockout
- **Programmer:** Implement camera shake, integrate with mechanics
- Both: Identify where each mechanic is required

### Lecture 2 (20 min): What is Juice?
- Feedback loops: action → feedback → satisfaction
- Particle systems for movement
- Video reference: "Juice It or Lose It"
- When juice is too much

### Work Block 2 (remaining)
- **Artist:** Create particle effects, continue level iteration
- **Programmer:** Polish mechanics, support level needs
- Test: Play the level. Does it require BOTH mechanics?
- Commit checkpoint

---

## Session 5: Tuning & Testing

### Lecture 1 (20 min): The Tuning Process
- One variable at a time
- Feel targets: tight vs floaty, weighty vs snappy
- Reference games and why they feel the way they do
- Config organization for rapid iteration

### Work Block 1 (45 min)
- Pair tuning: one plays, one adjusts values
- Tune both mechanics AND level difficulty together

### Lecture 2 (20 min): Playtesting
- How to run a playtest (watch silently)
- What to look for: confusion, failed inputs, missed communication
- Bug vs design issue
- **Key question:** Did the player use both mechanics?

### Work Block 2 (remaining)
- Cross-team playtesting (have another team play your level)
- Watch them: Do they discover both mechanics? Do they get stuck?
- Bug fixing and final polish
- Final commit

---

## Session 6: Presentations

### Format
- 5-7 minutes per team
- Live demo (no slides)
- Technical and art highlights
- Q&A

### Schedule
- Brief intro (5 min)
- Team demos (5-7 min each)
- Wrap-up discussion

---

## Lecture Resources

### Session 1
- Git Basics (Atlassian): https://www.atlassian.com/git/tutorials

### Session 2
- Celeste & TowerFall Physics (Matt Thorson): https://www.youtube.com/watch?v=yorTG9at90g

### Session 4
- Juice It or Lose It: https://www.youtube.com/watch?v=Fy0aCDmgnxg
- The Art of Screenshake: https://www.youtube.com/watch?v=AJdEqssNZ-U

### Session 5
- Game Feel by Steve Swink (book reference)

---

## Assessment Checkpoints

| Session | What to Check |
|---------|---------------|
| 1 | Git working, template running, TWO mechanics chosen |
| 2 | Mechanic 1 functional, base animations exist |
| 3 | Both mechanics working, clean state transitions |
| 4 | Level blockout exists, requires both mechanics |
| 5 | Tuned feel, level playtested, bugs fixed |
| 6 | Live demo, can explain both mechanics and level design choices |
