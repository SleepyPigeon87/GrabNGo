# Branch Strategy for Pairs

This project uses a simple branching strategy optimized for 2-student teams.

## Branch Naming Convention

Use prefixes to categorize your work:

| Prefix | Purpose | Example |
|--------|---------|---------|
| `feature/` | New functionality | `feature/dash-mechanic` |
| `fix/` | Bug fixes | `fix/jump-not-grounded` |
| `config/` | Config/tuning changes | `config/movement-speed` |
| `docs/` | Documentation updates | `docs/readme-update` |

## The Golden Rule

**Never commit directly to `main`.**

All changes must go through a Pull Request reviewed by your partner.

## Workflow for Pairs

### Step 1: Create a Branch
```bash
git checkout main
git pull origin main
git checkout -b feature/your-feature-name
```

### Step 2: Make Changes
- Keep commits small and focused
- Write clear commit messages
- Test your changes before committing

### Step 3: Push and Create PR
```bash
git push -u origin feature/your-feature-name
```
Then create a Pull Request on GitHub.

### Step 4: Partner Review
- Your partner reviews the PR
- They can comment, request changes, or approve
- **Both partners must understand the change**

### Step 5: Merge
After approval, the PR author merges to main.

## Conflict Prevention Tips

1. **Communicate** - Tell your partner what you're working on
2. **Small PRs** - Merge frequently to avoid large conflicts
3. **Stick to your track** - Follow the file ownership in CONTRIBUTING.md
4. **Pull before starting** - Always `git pull` before creating a new branch

## Common Git Commands

```bash
# See what's changed
git status
git diff

# Stage and commit
git add <file>
git commit -m "Brief description of change"

# Sync with remote
git pull origin main
git push

# Switch branches
git checkout main
git checkout <branch-name>

# Create new branch
git checkout -b feature/new-feature
```

## Resolving Merge Conflicts

If you get a conflict:

1. Open the conflicting file(s)
2. Look for conflict markers: `<<<<<<<`, `=======`, `>>>>>>>`
3. Discuss with your partner which version to keep
4. Remove the conflict markers
5. Stage and commit the resolution

## Track-Specific Branches

### Platformer Track
- `feature/movement-*` - Movement mechanics
- `feature/jump-*` - Jump systems
- `config/movement-*` - Movement tuning

### Combat Track
- `feature/combat-*` - Combat mechanics
- `feature/hitbox-*` - Hitbox systems
- `config/combat-*` - Combat tuning
