# AI Context

## Project

Engine: Unity  
Language: C#  
Current priority: MVP first, simple implementation, no overengineering.

## Communication

- Відповідай українською.
- Технічні терміни Unity/C# залишай англійською, якщо так зрозуміліше.
- Пояснюй зміни коротко й практично.

## Core Rules

- Do not refactor unrelated files.
- Do not introduce third-party packages without explicit approval.
- Do not rename scenes, prefabs, classes, serialized fields, public APIs, or folders unless requested.
- Do not modify unrelated scenes or prefabs.
- Do not silently change project architecture.
- Prefer small, safe, local changes.
- Explain all changed files after implementation.
- Mention assumptions, risks, and how to test.

## Unity Rules

- Use C#.
- Keep project code under Assets/Scripts.
- Put prefabs under Assets/Content/Prefabs.
- Prefer MonoBehaviour for MVP gameplay systems.
- Use ScriptableObjects only when useful for configuration, data, or reusable content.
- Avoid singletons unless the task clearly requires global state.
- Avoid complex dependency injection for MVP unless explicitly requested.
- Keep systems loosely coupled when possible.
- Do not add new manager classes unless there is a clear need.
- Do not create abstract architecture “just in case”.


## Folder Structure

Use this structure as the preferred direction, but do not create empty folders without need.

```text
Assets/
  Content/
    Art/
      Characters/
      Environment/
      UI/
      Icons/
    Audio/
      Music/
      SFX/
      Ambience/
    Prefabs/
      Player/
      Gameplay/
      UI/
      Environment/
    ScriptableObjects/
    Data/
      Items/
      Characters/
      Levels/
      Balance/
    Materials/
    Animations/
    VFX/
  Scenes/
    Boot/
    MainMenu/
    Levels/
    Tests/
  Scripts/
    Core/
    Player/
    Camera/
    Gameplay/
    Interaction/
    Inventory/
    UI/
    Save/
    Audio/
  Settings/
  External/
  Plugins/
```
