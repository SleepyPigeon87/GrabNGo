# Contributing Guidelines

This project is designed for 2-student pairs working together on a precision platformer.

## Pair Roles

### Systems Designer (Coder)
**Primary responsibility:** Code implementation

**Owns these folders:**
- `Assets/_Project/Scripts/` - All C# code
- `Assets/_Project/Prefabs/` - Prefab structure (not tuning)

**Key tasks:**
- Implement movement mechanics
- Create combat systems
- Wire up events and services
- Debug code issues

### Gameplay Designer (Config Owner)
**Primary responsibility:** Tuning and configuration

**Owns these folders:**
- `Assets/_Project/Configs/` - All ScriptableObject configs
- `Assets/_Project/Scenes/` - Level layout and placement
- `Assets/_Project/Art/` - Visual assets

**Key tasks:**
- Tune movement feel (speed, acceleration, etc.)
- Configure attack properties (damage, knockback)
- Design level layouts
- Adjust visual feedback

## Why File Ownership Matters

When two people edit the same file simultaneously, Git creates **merge conflicts**. These are frustrating and slow down development.

By agreeing on file ownership:
- Coder edits code, Designer edits configs
- Conflicts are rare or nonexistent
- Both can work in parallel

## Cross-Track Communication

Some changes affect both tracks:

| Change | Coder Does | Designer Does |
|--------|------------|---------------|
| New mechanic | Creates script + config class | Tunes values |
| Visual feedback | Exposes config options | Adjusts feel |
| Level testing | Fixes bugs found | Reports issues |

## Pull Request Process

### Before Creating a PR
1. Test your changes in the Dev Playground
2. Make sure the game still runs
3. Write a clear PR description

### PR Checklist
- [ ] Changes are in your owned folders
- [ ] You've tested the changes
- [ ] Commit messages are clear
- [ ] No debug code left in

### Reviewing Your Partner's PR
1. Pull their branch locally
2. Run the game and test
3. Read the code/config changes
4. Leave constructive feedback
5. Approve when satisfied

## Commit Message Format

Keep it simple and descriptive:

```
Add coyote time to jump system

- Buffer input for 0.1s after leaving ground
- Configurable in JumpConfig
```

**Good messages:**
- "Add horizontal movement with acceleration"
- "Fix jump not working when walking off ledge"
- "Tune dash speed to feel snappier"

**Bad messages:**
- "stuff"
- "WIP"
- "fix"

## Asking for Help

1. First, discuss with your partner
2. Check existing documentation
3. Ask your instructor with:
   - What you're trying to do
   - What you've tried
   - The error message (if any)
