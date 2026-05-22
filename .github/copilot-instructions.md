# GitHub Copilot Instructions for Custom Prisoner Interactions Mod

Welcome to the GitHub Copilot instructions file for the **Custom Prisoner Interactions** mod project. This document serves as a guide to utilizing GitHub Copilot effectively within the project, providing an overview of the mod, its key features, and development patterns.

## Mod Overview and Purpose

**Custom Prisoner Interactions** enhances the prisoner interaction system in RimWorld by providing a broader range of options for converting and releasing prisoners. It incorporates improvements from several older mods into a unified package, streamlining ideology selection and offering per-prisoner customization without global setting changes.

## Key Features and Systems

- **Enhanced Prisoner Interaction Options**: Introduces new options for converting and releasing prisoners, visible as buttons for intuitive selection.
  
- **Release Options**:
  - *When Healthy*: Wait until the prisoner is healthy.
  - *When Able to Walk*: Release once the prisoner can walk.
  - *When No Longer Guilty*: Release when they aren't guilty of a crime.

- **Convert Options**:
  - Recruit after conversion.
  - Lower resistance first, then recruit.
  - Enslave after conversion.
  - Execute after conversion.
  - Options for lowering resistance or will before subsequent actions.

- **Biotech Compatibility**: Automatically enable hemogen farming on new prisoners if Biotech is active.

- **Mod Integration**: Supports Prison Labor mod features like the work and convert actions.

- **Translation**: Includes a Chinese translation provided by Chiwei.

## Coding Patterns and Conventions

- **Class Structure**: Classes are structured around relevant game components, with naming conventions indicating functionality (e.g., `CustomPrisonerInteractionsMod`, `ExtraInteractionsTracker`).
  
- **Method Naming**: Follow descriptive naming conventions to indicate purpose clearly (e.g., `TryInteractWith`, `CapturedBy`).

- **Visibility Modifiers**: Use `public` for classes/methods that require external access and `internal` or `private` for encapsulation as appropriate.

## XML Integration

- XML files are used primarily for defining mod settings and patches.
- Ensure XML elements correspond to C# class properties when integrating with game data.
  
## Harmony Patching

- **Purpose**: Utilize Harmony to patch RimWorld base methods. This allows for modifications or injections of custom behavior without altering the original codebase.
  
- **Usage**: Implement static classes for each patch with descriptive names (e.g., `Pawn_IdeoTracker_IdeoConversionAttempt`).
- **Example**: Use `HarmonyPatch` attributes to specify target methods to patch and include prefix/postfix methods as needed.

## Suggestions for Copilot

When using GitHub Copilot, provide descriptive comments and clear function definitions to guide the AI in generating accurate code suggestions:

- Comment on class and method purposes to help Copilot understand context.
- Break down complex logic into smaller, single-purpose methods.
- Update README and comments if features or functionality change, so Copilot can use the latest information.

By following these guidelines, Copilot can be an effective tool in advancing the development and maintenance of the **Custom Prisoner Interactions** mod.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
