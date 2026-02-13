using System;
using System.Threading;
using System.Threading.Tasks;
using CatUI.Data;
using CatUI.Elements;
using CatUI.Windowing.DesktopApp;
using CatUI.Windowing.DesktopApp.PlatformImplementations;
using CatUI.Windowing.Headless;

namespace HeadlessExample;

internal static class Program
{
    public static readonly Size FramebufferSize = new(1280, 720);

    private static HeadlessSurface _surface = null!;

    private static CancellationToken _cancellationToken;
    private static readonly CancellationTokenSource _cts = new();

    private static void Main()
    {
        Console.WriteLine("CatUI headless mode example: draws UI to .png files");
        Console.CancelKeyPress += OnExitRequested;

        _cancellationToken = _cts.Token;
        _cancellationToken.ThrowIfCancellationRequested();

        CatApplication
            .NewBuilder()
            .SetPlatformInfo(new DesktopPlatformInfo())
            .Build();

        PngFileGraphicsBackend gfxBackend = new();
        _surface = new HeadlessSurface(FramebufferSize, gfxBackend);

        _surface.SetAppState(UiDocument.AppState.Active);
        _surface.ResizeViewport(FramebufferSize);

        UiTree tree = new();
        _surface.Document.Root = tree.GetRoot();

        for (int i = 0; i < 5; i++)
        {
            try
            {
                Task.Delay(1000, _cancellationToken).Wait();
            }
            catch (Exception)
            {
                break;
            }

            if (CatApplication.Instance.PlatformInformation != null &&
                CatApplication.Instance.Dispatcher is DesktopDispatcher dispatcher)
            {
                dispatcher.CallActions();
            }

            Console.WriteLine($"Writing snapshot #{i}...");
            tree.SetText($"Snap #{i}");
            _surface.DoFrameActions(1.0, true);
        }

        _surface.Terminate();
        Console.WriteLine("Program finished. Check the directory of the executable to see the files.");
    }

    private static void OnExitRequested(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        _cts.Cancel();
        Console.WriteLine("Exiting...");
    }
}
