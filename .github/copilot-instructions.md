# Copilot Instructions — BichinhoVirtualTCC (Godot + C#)

---

## 🎯 Output Control

**Code only, no explanation.** No educational preambles.  
**Bullets over paragraphs. No explanations unless asked.**

---

## 📋 Project Context

**Project**: Bichinho Virtual TCC (Tamagotchi simulation)  
**Tech Stack**: Godot 4.5 + C# (.NET 8)  
**Purpose**: Test automation research + offline evolution mechanics  
**Key Files**:
- `Main.cs`: UI controller, timer, sprite evolution, persistence
- `PetStatus.cs`: Pet state logic (hunger, happiness, energy)
- `GameSaveService.cs`: JSON serialization (pet_save.json)
- `tests/`: Unit tests (regression suite + offline calculations)

---

## 💻 Code Conventions

**Language**: C# (.NET)

**Naming**:
- Classes: PascalCase (`PetStatus`, `GameSaveService`)
- Methods/Properties: PascalCase (`GetStatus()`, `ApplyTick()`)
- Fields: `_camelCase` for private (e.g., `_pet`, `_fomeLabel`)
- Constants: `private const` in code

**Indentation**: 2 spaces (check `.editorconfig`)

**Godot specifics**:
- Use `#nullable enable` at top of files
- Extend `Node`, `Control`, or specific Godot types
- Use `GetNode<T>()` for scene node references
- Follow the `partial` pattern when using Godot script generation

**Error Handling**:
- Validate inputs in public methods (e.g., `GameSaveService` file checks)
- Use meaningful exception types
- Null coalescing (`??`) where safe

**Testing**:
- Tests live in `tests/BichinhoVirtualTCC.Tests/` (isolated from Godot)
- Use xUnit + theory tests for multiple cases
- Keep test names: `Should[Behavior]When[Condition]`

---

## 🎮 Key Rules

**Offline Evolution**:
- Pet state calculated from saved `lastSeenTime` → current time
- Each tick = 1 real second
- Total cycle = 3 days (real-world)

**Save/Load**:
- Uses `pet_save.json` in user directory (`user://`)
- Handles missing files gracefully
- Normalizes timestamps on load

**State Management**:
- Pet attributes: Fome, Felicidade, Energia (0–100)
- Each action: Alimentar (+20 fome, -4 energia), Brincar (+20 felicidade, -10 energia, -6 fome), Dormir (+25 energia, -10 fome, +3 felicidade)
- When any stat hits 0 → pet faints, buttons disabled

**Sprite Evolution**:
- Day 1, 2, 3 use separate PNG files
- Final state (Good, Neutral, Evil) changes sprite on day 3
- Fainted sprites for death state

---

## ⚠️ Constraints

- ❌ **Never run Godot without authorization**
- ❌ **Never execute tests without explicit permission**
- ✅ Focus on C# logic unless asked about scenes

---

## 🔄 Workflow

**For logic changes**: Modify `PetStatus.cs` → update tests → show diffs  
**For UI changes**: Modify `Main.cs` → explain scene references  
**For serialization**: Keep JSON format consistent with `GameSaveData`

---

**Repo**: mDenys7/BichinhoVirtualTCC
