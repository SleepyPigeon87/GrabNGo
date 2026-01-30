# Git Guide for GAME 326

This guide covers everything you need to use Git for this course. Keep it open as a reference.

---

## What is Git?

Git is version control software. It tracks changes to your code so you can:
- Undo mistakes
- Work on features without breaking the main project
- Collaborate without overwriting each other's work

---

## What is Git Bash?

Git Bash is a command-line terminal for running Git commands on Windows. It looks like a black window with text — no buttons, no menus, just typing.

**How to open Git Bash:**
1. Press the **Windows key** on your keyboard
2. Type `Git Bash`
3. Click on **Git Bash** when it appears

You'll see a window that looks something like this:
```
YourName@LABCOMPUTER MINGW64 ~
$
```

The `$` is your prompt. You type commands after it and press **Enter** to run them.

**Important command-line concepts:**
- You type a command and press **Enter** to run it
- Commands are case-sensitive (`git` works, `Git` might not)
- The `$` symbol shown in examples is the prompt — don't type it, it's already there
- If something goes wrong, you can close the window and open a new one

---

## First-Time Setup

These steps only need to be done once per computer.

### Step 1: Configure your identity

Open Git Bash and type these commands (replace with your actual info):

```
git config --global user.name "Your Name"
```

```
git config --global user.email "your.email@student.scad.edu"
```

These commands don't show any output if they work. No news is good news.

This tags your commits with your identity so your teammates can see who made which changes.

---

## Cloning a Project

"Cloning" means downloading a copy of a project from GitHub to your computer.

### Step 1: Navigate to where you want the project

```
cd /c/Users/YOUR_USERNAME/Documents
```

**What does this mean?**
- `cd` stands for "change directory" — it moves you to a folder
- `/c/Users/YOUR_USERNAME/Documents` is the path to your Documents folder
- In Git Bash, `C:\` becomes `/c/`

**How to find your username:** Look at the Git Bash prompt. It says `YourName@COMPUTER`. The first part before the `@` is your username.

### Step 2: Clone the repository

```
git clone https://github.com/USERNAME/REPONAME.git
```

You'll see output showing the download progress. When it's done, you'll have a new folder with the project.

### Step 3: Move into the project folder

```
cd REPONAME
```

---

## Creating a Branch

A "branch" is like making a copy of the project that you can change without affecting the original.

### Create and switch to a new branch

```
git checkout -b your-branch-name
```

Example: if your team is called "PixelJumpers":
```
git checkout -b team/PixelJumpers
```

You should see: `Switched to a new branch 'your-branch-name'`

### Verify you're on your branch

```
git branch
```

You'll see a list with your branch marked with an asterisk:
```
  main
* your-branch-name
```

The `*` shows which branch you're on.

### Push your branch to GitHub

Right now your branch only exists on your computer. This uploads it:

```
git push -u origin your-branch-name
```

GitHub may ask you to log in. Use your GitHub credentials.

The `-u` flag sets up tracking so future pushes are simpler.

---

## Daily Workflow

Every time you sit down to work:

### 1. Open Git Bash and navigate to your project

```
cd /c/Users/YOUR_USERNAME/Documents/PROJECTNAME
```

### 2. Get any changes your teammates made

```
git pull
```

### 3. Do your work in Unity

Make changes, test them, etc.

### 4. Save your work to Git

Run these three commands in order:

```
git add .
git commit -m "Brief description of what you changed"
git push
```

**What do these commands do?**
- `git add .` — Stages all your changes (the `.` means "everything in this folder")
- `git commit -m "message"` — Saves a snapshot with your description
- `git push` — Uploads your commits to GitHub

---

## Golden Rules

1. **Commit often** — Every 15-30 minutes of work
2. **Write useful messages** — "Added wall slide detection" not "Fixed stuff"
3. **Pull before you start** — Get your teammate's changes first
4. **Push before you leave** — Don't leave work only on your computer
5. **Read error messages** — Git usually tells you what's wrong

---

## Command Reference

**First-time setup (once per computer):**
```
git config --global user.name "Your Name"
git config --global user.email "you@email.com"
```

**Download a project from GitHub:**
```
git clone https://github.com/username/repo.git
```

**Create and switch to a new branch:**
```
git checkout -b branch-name
```

**See what branch you're on:**
```
git branch
```

**See what files have changed:**
```
git status
```

**The save-and-upload workflow:**
```
git add .
git commit -m "Your message here"
git push
```

**Get your teammate's changes:**
```
git pull
```

**See commit history:**
```
git log --oneline
```
(Press `q` to exit the log view)

---

## Common Problems

### "fatal: not a git repository"
You're not in a git project folder. Use `cd` to navigate to your project.

### "Please tell me who you are"
You haven't configured your name/email. Run the config commands from First-Time Setup.

### "failed to push some refs"
Your teammate pushed changes you don't have. Run `git pull` first, then try pushing again.

### "merge conflict"
You and your teammate changed the same lines. Ask your instructor for help resolving this.

### "Permission denied"
GitHub doesn't recognize you. You may need to log in again or set up authentication.

---

## Getting Help

If you get stuck:
1. Read the error message carefully
2. Copy the error and search for it online
3. Ask your teammate
4. Ask your instructor
