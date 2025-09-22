
# PeakChatOps

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

New command set mod:
PeakChatOps Extra
In development, for reference
Source: https://github.com/LIghtJUNction/PeakMods

## Features
- Chat message sending/receiving and UI display
- Supports custom commands (auto registration/hot reload)
- Multi-language internationalization and dynamic switching
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

