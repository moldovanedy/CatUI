# Supported platforms

CatUI runs wherever .NET runs, which means almost every major platform (as of .NET 8). The minimum supported .NET 
version is 8 because this version targets all major platforms. Running on older .NET versions (such as 6 or Core 3.1) 
is not supported but might still work. For detailed information about .NET support, consult the Microsoft documentation 
for the .NET version you want to use (for example, 
[here](https://github.com/dotnet/core/blob/main/release-notes/8.0/supported-os.md) you can find info about .NET 8 
supported OS versions).

## Windows

As of .NET 8, only Windows 10 (build 1607) and 11 are supported (along with most of the Windows Server versions). 
All architectures are supported (x64, x86, and Arm64). Older Windows versions such as 8.1 or 7 are not officially 
supported, but your application might still run there.

Windows UWP is not supported at the moment.

## macOS

macOS versions 13 (Ventura), 14 (Sonoma) and 15 (Sequoia) are supported on both Arm64 and x64 architectures, but 
Apple made the x64 arch obsolete, so you should generally only focus on Arm64. Mac Catalyst is also available as a 
target platform. Beware that macOS samples are not tested because of a lack of Apple hardware. It should generally 
run OK, but beware that it's still experimental.

## Linux

Linux support is complex because of the vast array of distributions, each one with its own packages. In general, 
any platform that supports glibc version 2.23 or musl 1.2.2 should be useful. This is true for x64, Arm64, and, 
in some cases, Arm32. Refer to the 
[official .NET 8 support policy](https://github.com/dotnet/core/blob/main/release-notes/8.0/supported-os.md) 
for more info.

Additionally, XDG desktop portal is used for some core features. If it isn't available on the runtime platform, 
your app will still run completely fine. However, you won't have access to features such as dark mode detection or 
high contrast detection (those will have a "fallback" value set by default or by you, as stated in this manual).

## Android

Android is supported from API 24 (Android 7.0 Nougat) onwards for all architectures (x64, Arm64, and Arm32).

## iOS (Not supported yet)

There is no technical reason why CatUI can't run on iOS; it's just the lack of Apple hardware that prevents the 
development of an iOS wrapper. If you own Apple hardware, please consider helping CatUI by developing a wrapper; 
you can use the Android wrapper for inspiration.

## WebAssembly (WASM) (Not supported yet)

There are no plans yet for supporting WASM simply because there are more important issues that need to be fixed 
in the core CatUI. However, you can still contribute be creating a WASM wrapper.
