using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Platform;
using CatUI.Platform.CommonInterface;

namespace CatUI.Elements.DocumentManagers;

public class CursorManager
{
    /// <summary>
    /// Specifies how to treat the case of a built-in cursor missing on the runtime platform. See
    /// <see cref="ICursorProvider.FakeCursorMode"/> for more info.
    /// </summary>
    public ICursorProvider.FakeCursorMode BuiltInFakeCursorMode
    {
        get => _builtInFakeCursorMode;
        set
        {
            _builtInFakeCursorMode = value;
            OS.CursorProvider?.SetBuiltInCursorFakeMode(value);
        }
    }

    private ICursorProvider.FakeCursorMode _builtInFakeCursorMode = ICursorProvider.FakeCursorMode.DrawFakeCursor;

    public CursorIcon? CurrentCursorIcon { get; private set; }

    /// <summary>
    /// Returns true if the runtime platform supports changing cursors, false otherwise. If false, all the functions
    /// in this manager will have no effect.
    /// </summary>
    public static bool IsCursorChangingAvailable => OS.CursorProvider != null;

    private readonly object? _windowId;

    private int _lastCustomId = 256;
    private bool _isCursorOverriden;
    private readonly Dictionary<int, CursorIcon> _availableCursors = new();

    public CursorManager(object? windowIdentifier)
    {
        if (!IsCursorChangingAvailable)
        {
            return;
        }

        CurrentCursorIcon = OS.CursorProvider?.GetDefaultCursorIcon(new Size(32, 32), Point2D.Zero);
        OS.CursorProvider?.SetBuiltInCursorFakeMode(ICursorProvider.FakeCursorMode.DrawFakeCursor);
        _windowId = windowIdentifier;
    }

    /// <summary>
    /// Sets the current cursor. Use <see cref="Element.Cursor"/> instead, as this does not persist the cursor shape,
    /// it will only retain the shape until the next call, which can be from the elements themselves on hover. If
    /// you want to persist the cursor and override the <see cref="Element.Cursor"/>, use
    /// <see cref="SetPersistentCursor"/> instead.
    /// </summary>
    /// <remarks>
    /// Calling <see cref="SetPersistentCursor"/> will render this function useless until the call to
    /// <see cref="SetPersistentCursor"/> with the ID of <see cref="CursorIcon.CURSOR_AUTO"/>.
    /// </remarks>
    /// <param name="id">
    /// The cursor ID, either from <see cref="CursorIcon"/> "CURSOR_*" constants, or a custom ID from
    /// <see cref="AddCustomCursor"/>.
    /// </param>
    public void SetCursor(int id)
    {
        if (!IsCursorChangingAvailable || _isCursorOverriden)
        {
            return;
        }

        CursorIcon cursorIcon;
        if (_availableCursors.TryGetValue(id, out CursorIcon? availableCursor))
        {
            cursorIcon = availableCursor;
        }
        else if (id < CursorIcon.BUILT_IN_CURSOR_LENGTH)
        {
            AddBuiltInCursor(id);

            if (_availableCursors.TryGetValue(id, out availableCursor))
            {
                cursorIcon = availableCursor;
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }

        OS.CursorProvider?.SetCursorAsActive(_windowId, cursorIcon);
    }

    /// <summary>
    /// Sets the current cursor and prevents any sort of override from calls to <see cref="SetCursor"/>, which can
    /// happen from elements with <see cref="Element.Cursor"/>. In other words, this sets the cursor and persists
    /// it until another call to this function.
    /// </summary>
    /// <remarks>
    /// To restore the <see cref="SetCursor"/> behavior, call this function with the id of
    /// <see cref="CursorIcon.CURSOR_AUTO"/>.
    /// </remarks>
    /// <param name="id">
    /// The ID of the new cursor. Using <see cref="CursorIcon.CURSOR_AUTO"/> will restore <see cref="SetCursor"/>
    /// and, implicitly, <see cref="Element.Cursor"/>. 
    /// </param>
    public void SetPersistentCursor(int id)
    {
        _isCursorOverriden = false;
        SetCursor(id);
        _isCursorOverriden = id != CursorIcon.CURSOR_AUTO;
    }

    /// <summary>
    /// Sets the current cursor mode. See <see cref="ICursorProvider.CursorMode"/> for more info. This is not
    /// called internally by any built-in CatUI element, so this will not be overriden.
    /// </summary>
    /// <param name="cursorMode">The new cursor mode.</param>
    public void SetCursorMode(ICursorProvider.CursorMode cursorMode)
    {
        OS.CursorProvider?.SetCursorMode(_windowId, cursorMode);
    }

    /// <summary>
    /// Gets the cursor icon from the given ID, or null if the ID is not found. Returns both built-in cursor and
    /// custom cursors.
    /// </summary>
    /// <param name="id">The ID to search for.</param>
    /// <returns>The cursor icon or null if the given ID is not found.</returns>
    public CursorIcon? GetCursorIcon(int id)
    {
        if (!IsCursorChangingAvailable)
        {
            return null;
        }

        if (_availableCursors.TryGetValue(id, out CursorIcon? cursorIcon))
        {
            return cursorIcon;
        }

        if (id < 256)
        {
            AddBuiltInCursor(id);
        }

        return null;
    }

    /// <summary>
    /// Adds a custom cursor in the internal cache, available for use afterwards (until an eventual call to
    /// <see cref="RemoveCustomCursor"/>). Returns the new cursor's ID, or null if something failed.
    /// </summary>
    /// <param name="size">The cursor size.</param>
    /// <param name="hotspot">The cursor's hotspot point, relative to the cursor coordinates.</param>
    /// <param name="pixelData">
    /// A byte array containing the RGBA 32-bit pixel data. The first byte is the R component of the first pixel,
    /// the fifth byte is the R component of the second pixel etc. Pixels are placed left-to-right, top-to-bottom
    /// (row-major order). This RGBA 32-bit format is used to speed up native cursor creation (use
    /// as few allocations as possible).
    /// </param>
    /// <returns>The new cursor's ID, or null if something failed.</returns>
    /// <exception cref="ArgumentException">
    /// If the hotspot is outside the cursor's bounds OR if the pixelData's length is different from the cursor's
    /// size.Width * size.Height * 4 (an RGBA pixel occupies 4 bytes).
    /// </exception>
    public int? AddCustomCursor(Size size, Point2D hotspot, byte[] pixelData)
    {
        if (!IsCursorChangingAvailable)
        {
            return null;
        }

        if (hotspot.X > size.Width || hotspot.Y > size.Height || hotspot.X < 0 || hotspot.Y < 0)
        {
            throw new ArgumentException("hotspot is outside the cursor's size.", nameof(hotspot));
        }

        if (pixelData.Length != 4 * (int)size.Width * (int)size.Height)
        {
            throw new ArgumentException("pixelData's length is not equal to the declared size.", nameof(pixelData));
        }

        CursorIcon? cursorIcon =
            OS.CursorProvider?.CreateCursor(_lastCustomId, size, hotspot, false, pixelData);
        if (cursorIcon == null)
        {
            return null;
        }

        bool result = _availableCursors.TryAdd(_lastCustomId, cursorIcon);
        if (!result)
        {
            return null;
        }

        return ++_lastCustomId;
    }

    /// <summary>
    /// Removes a custom cursor. Does not work for built-in cursors.
    /// </summary>
    /// <param name="id">The ID of the cursor to remove.</param>
    public void RemoveCustomCursor(int id)
    {
        if (id >= 256)
        {
            return;
        }

        if (_availableCursors.TryGetValue(id, out CursorIcon? cursorIcon))
        {
            OS.CursorProvider?.DestroyCursor(cursorIcon);
        }
    }

    private void AddBuiltInCursor(int id)
    {
        if (!IsCursorChangingAvailable)
        {
            return;
        }

        if (id >= CursorIcon.BUILT_IN_CURSOR_LENGTH)
        {
            return;
        }

        CursorIcon? cursorIcon = OS.CursorProvider?.CreateCursor(id, new Size(), Point2D.Zero, true);
        if (cursorIcon == null)
        {
            return;
        }

        _availableCursors[id] = cursorIcon;
    }
}
