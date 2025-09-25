# PeakChatOps
[![GitHub](https://img.shields.io/badge/GitHub-BetterPingDistance-brightgreen?style=for-the-badge&logo=GitHub)](https://github.com/LIghtJUNction/PeakMods)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/LucydDemon/BetterPingDistance?style=for-the-badge&logo=thunderstore&logoColor=white)](https://new.thunderstore.io/c/peak/p/LIghtPeak/PeakChatOps/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/LucydDemon/BetterPingDistance?style=for-the-badge&logo=thunderstore&logoColor=white)](https://new.thunderstore.io/c/peak/p/LIghtPeak/PeakChatOps/)

PeakChatOps is a modified version based on PeakTextChat.

Retained from the original:
- PeakTextChat's UI framework
- Some patches from PeakTextChat

Major changes:
- Removed PeakTextChat's original chat message logic, replaced with a custom message handler chain
- Added a new command system supporting custom commands (auto registration/hot reload)
- Some components now use PeakLib implementations
- Improved Chinese support


Features:
- Chat box now supports paging! Use the mouse wheel to scroll up and down
- Supports config hot-reload, changes take effect immediately
- Minecraft-like command system! (Easily extensible, currently a few built-in commands, more can be added)
- **System messages now support multi-language and rich text color tags!**
	- Each language line can have its own color, making notifications vibrant and easy to distinguish
	- Default system messages (death, revive, pass out) are preset with colorful styles for all supported languages

New command set mod:
PeakChatOps Extra
In development, for reference
Source: https://github.com/LIghtJUNction/PeakMods


## Features
- Chat message sending/receiving and UI display
- Supports custom commands (auto registration/hot reload)
- Multi-language internationalization and dynamic switching
- **System messages (death, revive, pass out) support multi-language and per-line color customization via Unity rich text tags**
- Chat input box Tab completion and prediction
- Supports config hot-reload
- Supports player teleport and other extension commands

## Installation
1. Recommended: Use a mod manager for installation

## Usage
- Chat: Press the configured hotkey (e.g. Y) to open the input box, type and press Enter to send
- Commands: Type `/help` to see all commands


## Configuration
- Chat box size, position, font, opacity, etc. can be adjusted in the config file
- **System message templates can be customized for each language and color using Unity rich text `<color>` tags.**
	- Example (default death message):
		```
		<color=#FF4040>没想到我也有死的这一天！</color>
		<color=#FFA500>I never thought I'd see the day I die!</color>
		<color=#40A0FF>まさか自分が死ぬ日が来るとは！</color>
		...
		```
- Supports runtime hot-reload

## Development & Extension
- To add a new command: Inherit from `PCmd`, implement the Handler, and put it in an extension DLL for auto loading


## build

> dotnet build -c Release -target:PackTS -v d


## Credits (in no particular order)
- [PeakTextChat](https://github.com/borealityy/PeakTextChat) (Inspiration)
- [BepInEx](https://github.com/BepInEx/BepInEx) (Framework)
- [PeakLib](https://github.com/PeakModding/PeakLib) (Utility library)

---
For questions or suggestions, feel free to open an issue or PR!

