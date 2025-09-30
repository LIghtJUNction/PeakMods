# PeakChatOps
[![GitHub](https://img.shields.io/badge/GitHub-PeakChatOps-LIghtJUNction?style=for-the-badge&logo=GitHub)](https://github.com/LIghtJUNction/PeakMods)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/LIghtPeak/PeakChatOps?style=for-the-badge&logo=thunderstore&logoColor=white)](https://new.thunderstore.io/c/peak/p/LIghtPeak/PeakChatOps/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/LIghtPeak/PeakChatOps?style=for-the-badge&logo=thunderstore&logoColor=white)](https://new.thunderstore.io/c/peak/p/LIghtPeak/PeakChatOps/)

# UI

The UI is being remade to solve lag issues and to expand more interfaces for better functionality.
Please wait patiently.
If you want me to update as soon as possible, please send an email to: lightjunction.me@gmail.com

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
- Minecraft-like command system! （支持内置命令和插件扩展）
- **System messages now support multi-language and rich text color tags!**
  - Each language line can have its own color, making notifications vibrant and easy to distinguish
  - Default system messages (death, revive, pass out) are preset with colorful styles for all supported languages

## Features
- Chat message sending/receiving and UI display
- Supports custom commands (auto registration/hot reload)
- Multi-language internationalization and dynamic switching
- **System messages (death, revive, pass out) support multi-language and per-line color customization via Unity rich text tags**
- Chat input box Tab completion and prediction
- Supports config hot-reload

## Installation
1. Recommended: Use a mod manager for installation

## AI Feature Guide

PeakChatOps supports AI chat assistant. We highly recommend using the free [Ollama](https://ollama.com/) cloud model (can be deployed on your own server or cloud VM).

### 1. Download and Install Ollama

- Visit [Ollama official site](https://ollama.com/) and download the installer for your OS.
- Follow the instructions to install and start Ollama.
- Optional: Run `ollama run llama3` or `ollama run qwen:7b` in your terminal to pre-download models.
- You can also deploy Ollama on a cloud server (e.g. AWS, Azure, GCP, or any VPS) and expose the API endpoint for remote access.

### 2. Configure AI Settings

- In-game, open the PeakChatOps settings panel.
- Set `AI Endpoint` to your Ollama server address, e.g. `http://localhost:11434` (local) or `http://your-cloud-ip:11434` (cloud).
- Leave `API Key` empty (Ollama does not require a key by default).
- Set `AI Model` to `llama3`, `qwen:7b`, or any model you have downloaded.
- You can adjust `Max Tokens`, `Temperature`, `TopP` and other parameters to control response length and style.

### 3. Using the /ai Command

- Type `/ai your question` in the chat box, e.g.:
 `/ai Translate this sentence`
- The AI assistant will reply in the chat box.
- Use `/ai prompt @send` to send the AI reply as a message.
- Use `/ai @clear` to clear the AI context/history.

### 4. Customizing Prompts

- In the settings panel, you can set a custom `AI Prompt` to define the assistant's behavior.
- Example:
	`You are a professional Unity developer assistant. Please answer in concise English.`
- The prompt will be automatically appended to your AI requests.

### 5. Recommended Free Models

We personally recommend Ollama's cloud models for best compatibility and cost-free usage. Popular free models include:

- llama3 (general-purpose, fast)
- qwen:7b (Chinese/English, strong reasoning)
- phi3 (compact, efficient)
- gemma (multilingual)

You can find more models on the [Ollama model list](https://ollama.com/library) or via the command line.

### 6. Troubleshooting

- If you cannot connect, make sure Ollama is running and listening on port 11434.
- If AI replies are empty or error occurs, check if the model is downloaded and the endpoint is correct.

For advanced usage and parameter details, refer to the Ollama documentation or the PeakChatOps settings panel.

---
## Usage
Chat: Press the configured hotkey (e.g. Y) to open the input box, type and press Enter to send
Commands: Type `/help` to see all commands


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




## build

> dotnet build -c Release -target:PackTS -v d


## Credits (in no particular order)
- [PeakTextChat](https://github.com/borealityy/PeakTextChat) (Inspiration)
- [BepInEx](https://github.com/BepInEx/BepInEx) (Framework)
- [PeakLib](https://github.com/PeakModding/PeakLib) (Utility library)

---
For questions or suggestions, feel free to open an issue or PR!

