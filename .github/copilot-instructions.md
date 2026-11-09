# GitHub Copilot Instructions for Custom Prisoner Interactions Mod

## Mod Overview and Purpose

Custom Prisoner Interactions is a RimWorld mod that adds various extra options for managing prisoner interactions, specifically for converting and releasing prisoners. It combines multiple previous mods that added similar functionality, providing a more comprehensive and user-friendly experience for players. This mod offers improved usability by adding options buttons for the Convert and Release choices, making it easier to manage ideologies and individual prisoner settings. It also supports the Prison Labor mod and includes options for automatic hemogen farm management if Biotech is active.

## Key Features and Systems

### Release Options

- **When healthy:** Waits until the prisoner is considered healthy.
- **When able to walk:** Waits until the prisoner can walk again.
- **When no longer guilty:** Waits until the prisoner is no longer guilty for a crime.

### Convert Options

- **Recruit after:** Automatically recruits the prisoner after conversion.
- **Lower resistance first:** Lowers the prisoner's resistance before any other action.
- **Lower resistance first, Recruit after:** Lowers the prisoner's resistance first and then recruits them.
- **Enslave after:** Enslaves the prisoner after conversion.
- **Lower will first:** Lowers the prisoner's will before any other action.
- **Lower will first, Enslave after:** Lowers the prisoner's will first and then enslaves them.
- **Release after:** Releases the prisoner after conversion.
- **Lower resistance first, Release after:** Lowers the prisoner's resistance first and then releases them.
- **Execute after:** Executes the prisoner after conversion.
- **Lower resistance first, Execute after:** Lowers the prisoner's resistance first and then executes them.

### Other Features

- Chinese translation by Chiwei.
- Supports the work and convert actions added by the Prison Labor mod.
- Automatically enables hemogen farm on new prisoners if Biotech is active.
- Mod settings allow customization of default options.

## Coding Patterns and Conventions

### C# Coding Patterns

- The mod uses a modular approach with multiple C# files, each handling specific aspects of the functionality.
- The mod uses Harmony patching to modify vanilla RimWorld behavior, as detailed in the Harmony patching section below.

### XML Integration

- The mod defines new XML files for UpdateInfo, which are used to provide information about the mod updates.

## Harmony Patching

Custom Prisoner Interactions uses Harmony patching to modify existing vanilla RimWorld behavior. The following are the Harmony patches used in the mod:

### Pawn_IdeoTracker_IdeoConversionAttempt

- **Postfix:** Modifies the ideology conversion attempt behavior to include custom options.

### Pawn_InteractionsTracker_TryInteractWith

- **Postfix:** Modifies the interaction behavior to include custom options.
- **Prefix:** Modifies the interaction behavior before the postfix.

### RitualOutcomeEffectWorker_Conversion_Apply

- **Postfix:** Modifies the conversion ritual outcome behavior to include custom options.
- **Prefix:** Modifies the conversion ritual outcome behavior before the postfix.

### WorkGiver_Warden_Chat_JobOnThing

- **Postfix:** Modifies the work giver behavior for wardens to include custom options.
- **Prefix:** Modifies the work giver behavior for wardens before the postfix.

## Suggestions for Copilot

- **Custom Prisoner Interaction Management:** Utilize Copilot to assist in creating and managing custom prisoner interactions, including adding new options and handling specific prisoner statuses.
- **Harmony Patching Assistance:** Copilot can help with the creation and management of Harmony patches, ensuring that the mod integrates seamlessly with vanilla RimWorld behavior.
- **XML Defs:** Copilot can assist in generating and managing XML files for the mod, including the UpdateInfo file used for mod updates.

By following these guidelines, developers can effectively utilize GitHub Copilot to enhance the Custom Prisoner Interactions mod, improving both functionality and maintainability.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

