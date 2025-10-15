using CatUI.Windowing.DesktopApp;
using NUnit.Framework;

namespace CatUI.Tests.Windowing;

public class WindowingTests
{
    private readonly BaseWindowingManager _manager = new();

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void SimpleOpenAndRunWindow()
    {
        _manager.OpenAndRunWindow();
        Assert.Pass();
    }

    [Test]
    public void OpenFullscreenWindow()
    {
        _manager.OpenAndRunWindow(startupMode: DesktopWindow.WindowMode.Fullscreen);
        Assert.Pass();
    }
}
