using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Platform;
using CatUI.Platform.CommonInterface;

namespace CatUI.Elements.DocumentManagers
{
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

        public void SetCursor(int id)
        {
            if (!IsCursorChangingAvailable)
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

        public void SetCursorMode(ICursorProvider.CursorMode cursorMode)
        {
            OS.CursorProvider?.SetCursorMode(_windowId, cursorMode);
        }

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

        public bool AddCustomCursor(Size size, Point2D hotspot, byte[] pixelData)
        {
            if (!IsCursorChangingAvailable)
            {
                return false;
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
                return false;
            }

            bool result = _availableCursors.TryAdd(_lastCustomId, cursorIcon);
            _lastCustomId++;
            return result;
        }

        public void RemoveCustomCursor(int id)
        {
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
}
