# Overview

## General

CatUI is a fast, cross-platform and easy-to-use UI framework for .NET. It is decoupled from native platform UI options
and instead uses SkiaSharp for rendering, meaning that the user interface will look exactly the same on all platforms
(Windows, macOS, Linux, and Android, see [Supported platforms](supported-platforms.md) for more info). However,
CatUI also provides cross-platform APIs that make development easier, therefore lowering the need for platform-specific
APIs for most applications.

> [!NOTE]
> macOS is theoretically supported, but at the moment there are no tests done on it because of the lack of Apple
hardware. You can help CatUI by giving feedback, report issues or solve macOS-specific problems if you own such
hardware. For iOS, although there is no technical reason for it to not be supported, this is again the cause of
lacking Apple hardware for development. Unlike for macOS, for iOS a platform-specific wrapper is needed, so you
can help by creating one (you can get inspiration from the implementation on Android or Desktop in
`CatUI.Windowing.Desktop` and `CatUI.Windowing.Android` projects).

The initial project structure is roughly the same as the one from other cross-platform platforms: a shared project,
where most of the UI and business logic code resides (you can, of course, create other separate projects for
further code organization), then a project for each platform the app runs on, where you put the platform-specific code.
By default, there are platform-specific projects for Desktop (includes Windows, macOS, and Linux, but you can further
separate each platform in another project) and Android (in the future, there should also be iOS and WebAssembly (WASM)).
However, you are not forced to support all platforms, as you can choose which projects you want to use. You can also
use platform-specific APIs in a project like Desktop or even the shared one, but you must be careful as in that case
you can no longer use the application on another platform unless you use platform-specific compilation or conditional
code execution using `System.OperatingSystem`.

## Principles

CatUI development respects some core principles that guide the entire API. If you want to improve the framework,
your pull request must respect these principles. These principles are:

-   **App size must be small by default**: There are way too many apps out there that are huge compared to what
    they offer (e.g. 150MB storage needed for a basic calculator or some app that only communicates to a server API), and
    some users are rightfully frustrated by this. CatUI is different: using assembly trimming or NativeAOT, you can create
    an app that occupies as little as 14 MB on desktop (this is around the minimum size (a "Hello world" app); for apps
    that have resources, other packages, or native libraries, the app size will increase. Other frameworks have the minimum
    size of 30 MB or even 100 MB in the case of an Electron app). No embedded browser, no hundreds of MB of cache memory
    stored on the user's disk etc. Just what it's necessary for an application. Apps that need more will use more, apps
    that don't need that much will not. It's that simple!
-   **Design and architectural patterns are NOT enforced, but are encouraged**: Design patterns are great and help you
    be more productive on large-scale projects, but for smaller apps it just increases the complexity. This is why CatUI
    does NOT enforce a specific architectural pattern like MVVM, MVC, or MVP, but you can use those if you will.
    CatUI centers around data binding, so it's close to MVVM but, again, nothing is enforced by design.
-   **Every visual is an Element**.
-   **Additional functionality that is not necessary for every app should be developed as a separate package,
    especially if it includes other packages with a considerable size**: in most applications, you won't use all the
    available framework features, so there is no need to include all that unnecessary code that just takes more space
    and possibly degrades performance. In CatUI, every feature that is not necessary (e.g. SVG support, video support,
    embedded web browser) will be available as a separate package, usually ending with "Extensions".

## Getting started

For supported platforms, see [Supported platforms](supported-platforms.md).

To start developing great applications with CatUI (including installation, setting up the development environment,
and the general project structure), see the "[Getting started](getting-started/overview.md)" section.

**[Developing UIs](developing-uis/overview.md)**: For the general development of user interfaces. This is the main focus 
of the manual, where you will find most of the resources you need to create your applications.

**[Beyond UI](beyond-ui/overview.md)**: For the more advanced users, where you need to integrate your app with the 
runtime platform services, use complex data fetching with APIs, performance tips, and more.

**[App deployment](app-deployment/overview.md)**: For general information about the process of app deployment to end 
users for each platform (i.e. prepare and publish the app on an app store or host the executable files on a website).

**[Samples and tutorials](samples-and-tutorials/overview.md)**: For samples and tutorials that help you better 
understand the CatUI framework, as well as some inspiration for your projects.