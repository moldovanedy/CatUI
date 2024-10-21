using System.Collections.Generic;
using System.Linq;
using CatUI.Data;
using CatUI.RenderingEngine;
using SkiaSharp;

namespace CatUI.Elements
{
    /// <summary>
    /// Represents the root of all elements. Every window has one document, and all elements attached to the document
    /// will participate in the application lifecycle.
    /// </summary>
    public class UIDocument
    {
        public Element? Root
        {
            get
            {
                return _root;
            }
            set
            {
                bool wasDifferentElement = false;
                if (_root != value)
                {
                    wasDifferentElement = true;
                    _root?.InvokeExitDocument();
                }

                _root = value;
                _root?.SetDocument(this);

                if (wasDifferentElement)
                {
                    _root?.InvokeEnterDocument();
                }
            }
        }
        private Element? _root;
        public Size ViewportSize
        {
            get
            {
                return _viewportSize;
            }
            set
            {
                _viewportSize = value;
                Renderer.SetNewSize(new SKSize(value.Width, value.Height));
            }
        }
        private Size _viewportSize;
        public Renderer Renderer { get; private set; } = new Renderer();
        public int ElementCacheSize { get; set; } = 4096;
        public Color BackgroundColor
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                Renderer.SetBgColor(value);
            }
        }
        private Color _background;

        private readonly Dictionary<string, Element> _cachedElements = new Dictionary<string, Element>();

        public void DrawAllElements()
        {
            Root?.InvokeDraw();
        }

        public Element? GetElementByName(string name)
        {
            if (Root == null)
            {
                return null;
            }
            else
            {
                if (_cachedElements.TryGetValue(name, out Element? element))
                {
                    return element;
                }
                return Search(Root, name);
            }
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
            foreach (Element child in current.GetChildren())
            {
                if (child.Name == name)
                {
                    if (_cachedElements.Count <= ElementCacheSize)
                    {
                        _cachedElements.Add(child.Name, child);
                    }

                    return child;
                }
                Search(child, name);
            }

            return null;
        }
    }
}
