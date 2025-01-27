using System.Collections.Generic;
using System.Linq;
using CatUI.Data;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;
using CatUI.Elements.Themes;
using CatUI.RenderingEngine;
using SkiaSharp;

namespace CatUI.Elements
{
    /// <summary>
    /// Represents the root of all elements. Every window has one document, and all elements attached to the document
    /// will participate in the application lifecycle.
    /// </summary>
    public class UiDocument
    {
        /// <summary>
        /// Represents the root element of the document/window. All other elements are children of this element or one of its descendants.
        /// Setting this to another element will remove the previous one with all the children (invoking ExitDocument) and 
        /// attach this element to the document, along with its children (calling EnterDocument).
        /// </summary>
        public Element? Root
        {
            get => _root;
            set
            {
                if (_root != value)
                {
                    _root?.InvokeExitDocument();
                    _root?.Children.Clear();
                }

                _root = value;
                if (_root == null)
                {
                    return;
                }

                _root.SetDocument(this);

                _root.MinHeight = "100%";
                _root.MinWidth = "100%";
                _root.PreferredHeight = "100%";
                _root.PreferredWidth = "100%";

                _root.AbsolutePosition = Point2D.Zero;
                _root.AbsoluteWidth = ViewportSize.Width;
                _root.AbsoluteHeight = ViewportSize.Height;
            }
        }

        private Element? _root;

        public Size ViewportSize
        {
            get => _viewportSize;
            set
            {
                _viewportSize = value;
                Renderer.SetNewSize(new SKSize(value.Width, value.Height));
                Root?.RecalculateLayout();
            }
        }

        private Size _viewportSize = new();

        public Renderer Renderer { get; private set; } = new();
        public int ElementCacheSize { get; set; } = 4096;

        public Color BackgroundColor
        {
            get => _background;
            set
            {
                _background = value;
                Renderer.SetBgColor(value);
            }
        }

        private Color _background = new(0xff_ff_ff);

        public float ContentScale
        {
            get => _contentScale;
            set
            {
                _contentScale = value;
                Renderer.SetContentScale(value);
            }
        }

        private float _contentScale = 1f;

        public Theme RootTheme { get; private set; } = new();

        private readonly Dictionary<string, Element> _cachedElements = new();

        public UiDocument()
        {
            RootTheme.AddThemeDefinition<Element>(new ThemeDefinition<ElementThemeData>());
            RootTheme.AddThemeDefinition<AbstractShape>(new ThemeDefinition<ElementThemeData>());
            RootTheme.AddThemeDefinition<Ellipse>(new ThemeDefinition<ElementThemeData>());
            RootTheme.AddThemeDefinition<Rectangle>(new ThemeDefinition<ElementThemeData>());
            RootTheme.AddThemeDefinition<GeometricPath>(new ThemeDefinition<ElementThemeData>());

            //TODO: fix the types so it can accept any T
            // RootTheme.AddThemeDefinition<TextElement>(new ThemeDefinition<TextElementThemeData>());
            // RootTheme.AddThemeDefinition<Label>(new ThemeDefinition<TextElementThemeData>());
            //
            // RootTheme.AddThemeDefinition<ImageView>(new ThemeDefinition<ImageViewThemeData>());

            RootTheme.AddThemeDefinition<TextElement>(new ThemeDefinition<ElementThemeData>());
            RootTheme.AddThemeDefinition<Label>(new ThemeDefinition<ElementThemeData>());

            RootTheme.AddThemeDefinition<ImageView>(new ThemeDefinition<ElementThemeData>());
        }

        public void DrawAllElements()
        {
            Root?.InvokeDraw();
        }

        public void SetRootTheme(Theme theme)
        {
            RootTheme = theme;
        }

        public Element? GetElementByName(string name)
        {
            if (Root == null)
            {
                return null;
            }

            return _cachedElements.TryGetValue(name, out Element? element) ? element : Search(Root, name);
        }

        public void CacheElement(string name, Element element)
        {
            if (!_cachedElements.TryGetValue(name, out _))
            {
                _cachedElements.Add(name, element);

                //remove the first element from the cache, as the dictionary is full
                if (_cachedElements.Count <= ElementCacheSize)
                {
                    _cachedElements.Remove(_cachedElements.ElementAt(0).Key);
                }
            }
        }

        private Element? Search(Element current, string name)
        {
            foreach (Element child in current.Children)
            {
                if (child.Name == name)
                {
                    if (_cachedElements.Count <= ElementCacheSize)
                    {
                        CacheElement(child.Name, child);
                    }

                    return child;
                }

                Search(child, name);
            }

            return null;
        }
    }
}
