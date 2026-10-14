<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (DouYin Mini Game)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（抖音小游戏）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（抖音小遊戲）', 'ja': 'GameFrameX OperationKeyboard（Douyin ミニゲーム）', 'ko': 'GameFrameX OperationKeyboard(더우인 미니게임)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.douyinminigame)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.douyinminigame/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.douyinminigame)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.douyinminigame/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## Table of Contents

- [Project Overview](#project-overview)
- [Quick Start](#quick-start)
- [Usage Examples](#usage-examples)
- [Documentation & Resources](#documentation--resources)
- [Community & Support](#community--support)
- [License](#license)

---

## Project Overview

GameFrameX OperationKeyboard (DouYin Mini Game) is the keyboard adapter channel package for the Douyin Mini Game platform. It implements `IOperationKeyboardManager` (`DouYinMiniGameOperationKeyboardManager`, direct passthrough of `TT.ShowKeyboard(ShowKeyboardOptions, onSuccess, onError)` / `TT.HideKeyboard` / `TT.UpdateKeyboard` plus the C# events `TT.OnKeyboardInput/OnKeyboardConfirm/OnKeyboardComplete`) and self-registers into the main package's `OperationKeyboardRegistry` via `DouYinMiniGameOperationKeyboardBootstrap` at the `BeforeSceneLoad` stage (when the macros `UNITY_WEBGL && ENABLE_DOUYIN_MINI_GAME && ENABLE_DOUYIN_MINI_GAME_OPERATIONKEYBOARD` are not enabled the code is stripped and registration does not happen; referencing the package is registering). No configuration is needed: keyboard options are derived from the field contract by the core facade.

### Key Features

- **Douyin Mini Game Keyboard** - native SDK integration with the showKeyboard family and single-slot callback events
- **Self-Registration** - registers into `OperationKeyboardRegistry` at `BeforeSceneLoad` (referencing means registering)
- **Zero Coupling** - the core package never references the channel SDK; the SDK is provided by the target project

### System Requirements

- Unity 2019.4+

---

## Quick Start

### Installation

Choose one of the following methods:

1. Edit your Unity project's `Packages/manifest.json` and add the `scopedRegistries` section:
   ```json
   {
     "scopedRegistries": [
       {
         "name": "GameFrameX",
         "url": "https://gameframex.upm.alianblank.uk",
         "scopes": [
           "com.gameframex"
         ]
       }
     ],
     "dependencies": {
       "com.gameframex.unity.operationkeyboard.douyinminigame": "1.0.0"
     }
   }
   ```

   `scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

2. Add to `manifest.json` dependencies:
   ```json
   {
      "com.gameframex.unity.operationkeyboard.douyinminigame": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.douyinminigame.git"
   }
   ```
3. Use **Package Manager** in Unity with **Git URL**: `https://github.com/gameframex/com.gameframex.unity.operationkeyboard.douyinminigame.git`
4. Clone the repository into your Unity project's `Packages` directory. It will be loaded automatically.

## Usage Examples

### Channel Registration (Bootstrap Self-Registration)

No startup configuration is required. After the package is installed together with the Douyin Mini Game SDK (`com.bytedance.starksdk`), the channel registers itself automatically; UI input fields are driven through the core facade (install `com.gameframex.unity.operationkeyboard.ugui` for automatic `InputField` / `TMP_InputField` binding).

```csharp
using GameFrameX.OperationKeyboard.Runtime;

// UI binding packages call these automatically on focus / blur.
// Manual use (custom IOperationKeyboardField implementations):
// OperationKeyboard.Attach(field);  /  OperationKeyboard.Detach(field);
OperationKeyboard.UpdateValue("programmatic value");
OperationKeyboard.OnHeightChanged(height => { /* avoidance layout */ });
```

The channel `ChannelId` constant is `"douyinminigame"`; `OperationKeyboardRegistry.ResolveManager()` resolves channels in registration order, or use `GetManager("douyinminigame")` to fetch this channel explicitly.

### Project Structure

```
Runtime/
├── DouYinMiniGame/
│   ├── DouYinMiniGameOperationKeyboardManager.cs  # Douyin Mini Game keyboard manager
│   └── DouYinMiniGameOperationKeyboardBootstrap.cs  # channel bootstrap (self-registration)
└── GameFrameXOperationKeyboardDouYinMiniGameCroppingHelper.cs  # code cropping helper
```

### Main Classes

- **`DouYinMiniGameOperationKeyboardManager`**: implements `IOperationKeyboardManager` for the Douyin Mini Game keyboard SDK (`public const string ChannelId`); unregisters every platform event on `Hide`.
- **`DouYinMiniGameOperationKeyboardBootstrap`**: channel startup registration; self-registers into the main package's `OperationKeyboardRegistry` at `BeforeSceneLoad`.
- **`GameFrameXOperationKeyboardDouYinMiniGameCroppingHelper`**: code cropping helper for build optimization.

### Notes

- Events are C# events (`+=` / `-=`); `Hide()` unsubscribes all of them.
- StarkSDK has no keyboard-height API; `OnHeightChanged` is an empty implementation by design (the core contract stays full).

---

## Documentation & Resources

- **Documentation**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **Issue Tracker**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.douyinminigame/issues)

---

## Community & Support

- **QQ Group**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **Feature Requests**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.douyinminigame/discussions)

---

## Dependencies
| Package | Description |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard core |
| com.gameframex.unity | GameFrameX core |
| com.bytedance.starksdk | provided by the target project |

## Changelog

See [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.douyinminigame/releases) for changelog.

## License

See [LICENSE.md](LICENSE.md) for license information.
