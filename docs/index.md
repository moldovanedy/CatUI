# CatUI

## General

CatUI is a fast, cross-platform and easy-to-use UI framework for .NET. It is decoupled from native platform UI options and instead uses SkiaSharp for rendering, meaning that the user interface will look exactly the same on all platforms (Windows, macOS, Linux and Android, see [Supported platforms](manual/supported-platforms.md) for more info). However, CatUI also provides cross-platform APIs that makes development easier, as the need for platform-specific APIs is lower for most applications.

> [!NOTE]
> macOS is theoretically supported, but at the moment there are no tests done on it because of the lack of Apple hardware. You can help CatUI by giving feedback, report issues or solve macOS-specific problems if you own one. For iOS, although there is no technical reason for it to not be supported, this is again the cause of lack of Apple hardware for development. Unlike for macOS, for iOS a platform-specific wrapper is needed, so you can help by creating one (you can get inspiration from the implementation on Android or Desktop in `CatUI.Windowing.Desktop` and `CatUI.Windowing.Android` projects).

The initial project structure is roughly the same as the one from other cross-platform platforms: a shared project, where most of the UI and business logic code resides (you can, of course, create other separate projects for further code organization), then a project for each platform the app runs on, where you put the platform-specific code. By default, there are platform-specific projects for Desktop (includes Windows, macOS and Linux, but you can further separate each platform in another project) and Android (in the future, there should also be iOS and WebAssembly (WASM)), but you are not forced to support all platforms, as you can choose which projects you want to use. You can also use platform-specific APIs in a project like Desktop or even the shared one, but you must be careful as in that case you can no longer use the application on another platforms unless you use platform-specific compilation or conditional code execution using `System.OperatingSystem`.

## Principles

CatUI development respects some core principles that guides the entire API. If you want to improve the framework, your pull request must respect these principles. These principles are:

-   **App size must be small by default**: There are way too many apps out there that are huge compared to what they offer (e.g. 150MB storage needed for a basic calculator or some app that only communicates to a server API) and some users are rightfully frustrated by this. CatUI is different: using assembly trimming or NativeAOT you can create an app that occupies as little as 14 MB (this is around the minimum size (a "Hello world" app), apps that have resources, other packages or native libraries, it will increase the app size). No embedded browser, no hundreds of MB of cache memory stored on the user's disk etc. Just what it's needed for an application. Apps that need more will use more, apps that don't will not; it's that simple.
-   **Design and architectural patterns are NOT enforced, but are encouraged**.
-   **Every visual is an Element**.
-   **Additional functionality that is not necessary for every app should be developed as a separate package, especially if it includes other packages considerable in size**.
