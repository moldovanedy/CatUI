using System;
using System.Collections.Generic;
using CatUI.Data.Containers;
using CatUI.Data.Events.Navigation;
using CatUI.Data.Navigator;
using CatUI.Data.Shapes;
using CatUI.Utils;

namespace CatUI.Elements.Helpers.Navigation
{
    public class Navigator : Element
    {
        /// <summary>
        /// Fired when the navigator successfully navigates to a route, after the new element has already entered the
        /// document.
        /// </summary>
        public event NavigatedEventHandler? NavigatedEvent;

        public NavigatedEventHandler? OnNavigated
        {
            get => _onNavigated;
            set
            {
                NavigatedEvent -= _onNavigated;
                _onNavigated = value;
                NavigatedEvent += _onNavigated;
            }
        }

        private NavigatedEventHandler? _onNavigated;

        /// <summary>
        /// Fired when the navigator fails to navigate to a route, when it already navigated to the "not found" route
        /// or just removed the previous content if no "not found" route was available.
        /// </summary>
        public event NavigationFailedEventHandler? NavigationFailedEvent;

        public NavigationFailedEventHandler? OnNavigationFailed
        {
            get => _onNavigationFailed;
            set
            {
                NavigationFailedEvent -= _onNavigationFailed;
                _onNavigationFailed = value;
                NavigationFailedEvent += _onNavigationFailed;
            }
        }

        private NavigationFailedEventHandler? _onNavigationFailed;

        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<Navigator>? Ref
        {
            get => _ref;
            set
            {
                _ref = value;
                if (_ref != null)
                {
                    _ref.Value = this;
                }
            }
        }

        private ObjectRef<Navigator>? _ref;

        /// <summary>
        /// Specifies all the routes that can be navigated with this Navigator. The key is the path (generally starts
        /// with "/" and doesn't contain any arguments), the value is a function that receives a <see cref="NavArgs"/>
        /// object (you can treat as null if no arguments are desired) and returns a <see cref="NavRoute"/> object.
        /// When setting up the function, ensure you also treat the case when the arguments are null.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Any string key is allowed, except duplicates and empty string, as an empty string represents the "not found"
        /// route, it's used when the given path in <see cref="Navigate"/> is not matched with any route.
        /// When you change these routes, you should also call <see cref="Refresh"/> to see the changes, otherwise they
        /// will reflect only at the next navigation.
        /// </para>
        /// <para>
        /// The convention for routes is generally the same as for web URLs, except it starts with "/", camelCase,
        /// PascalCase and kebab-case are all ok (but choose only one of them and remain consistent throughout your app)
        /// and no parameters in routes, as you have <see cref="NavArgs"/> for that. Examples: "/login",
        /// "/users/modify", "/products/laptops/asus".
        /// </para>
        /// </remarks>
        /// <example>
        /// Routes = new Dictionary&lt;string, Func&lt;NavArgs?, NavRoute&gt;&gt; <br/>
        /// { <br/>
        ///     { "/", _ => new NavRoute(new Element()) }, //main page <br/>
        ///     { "/user", args => new NavRoute(new PageWithArgs(args)) }, //a page with args <br/>
        ///     { "", _ => new NavRoute(new PageNotFound()) } //when the route is not found <br/>
        /// };
        /// </example>
        /// <exception cref="ArgumentException">If two keys are the same.</exception>
        public Dictionary<string, Func<NavArgs?, NavRoute>> Routes
        {
            get => _routes;
            set
            {
                _routes.Clear();
                foreach (KeyValuePair<string, Func<NavArgs?, NavRoute>> route in value)
                {
                    _routes.Add(route.Key, route.Value);
                }
            }
        }

        private readonly Dictionary<string, Func<NavArgs?, NavRoute>> _routes = new();

        /// <summary>
        /// Represents the path that is currently visible. For navigating, see <see cref="Navigate"/>.
        /// </summary>
        public string CurrentPath { get; private set; } = string.Empty;

        /// <summary>
        /// Represents the active <see cref="NavRoute"/>. For navigating, see <see cref="Navigate"/>.
        /// </summary>
        public NavRoute? CurrentRoute
        {
            get => _currentRoute;
            private set
            {
                _currentRoute = value;
                if (value != null)
                {
                    if (Children.Count == 0)
                    {
                        Children.Add(value.RouteElement);
                    }
                    else
                    {
                        Children[0] = value.RouteElement;
                    }
                }
                else if (Children.Count == 1)
                {
                    Children.RemoveAt(0);
                }
            }
        }

        private NavRoute? _currentRoute;

        private readonly Stack<Tuple<string, NavArgs?>> _navigationStack = new();
        private readonly Stack<Tuple<string, NavArgs?>> _backStack = new();

        public Navigator() { }

        public Navigator(
            Dictionary<string, Func<NavArgs?, NavRoute>> routes,
            string initialPath,
            NavArgs? initialArgs = null)
        {
            Routes = routes;
            Navigate(initialPath, initialArgs);
        }

        /// <inheritdoc cref="Element.Duplicate"/>
        /// <remarks>
        /// The routes are shallow copied and <see cref="CurrentRoute"/> is duplicated as described in <see cref="NavRoute.Duplicate"/>.
        /// </remarks>
        public override Element Duplicate()
        {
            return new Navigator
            {
                Routes = new Dictionary<string, Func<NavArgs?, NavRoute>>(Routes),
                CurrentPath = CurrentPath,
                CurrentRoute = CurrentRoute?.Duplicate(),
                //
                Position = Position,
                Background = Background.Duplicate(),
                ClipPath = (ClipShape?)ClipPath?.Duplicate(),
                ClipType = ClipType,
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate()
            };
        }

        #region Public API

        /// <summary>
        /// Navigate to the given path, optionally passing arguments. If the path is not mapped to any route, the
        /// Navigator will go to the empty string route. If not even that is present in <see cref="Routes"/>, it simply
        /// removes the existing child.
        /// </summary>
        /// <remarks>
        /// Navigating to the current path will stil run the routing logic and the function from <see cref="Routes"/>,
        /// but will also remove the content and add it again directly, which can be computationally expensive, so use
        /// with caution.
        /// </remarks>
        /// <param name="path">The path to navigate to.</param>
        /// <param name="args">The arguments to give to the route. Set to null if you don't want arguments.</param>
        /// <param name="isStoredOnNavigationStack">
        /// If true, the navigation will be stored on the navigation stack, so you can then call <see cref="GoBack"/>.
        /// It won't be stored if it's the current route or if it's from <see cref="Refresh"/>.
        /// If false, going back after navigation to another route won't consider this route and jump directly to the
        /// route that was active before this one.
        /// </param>
        public void Navigate(string path, NavArgs? args = null, bool isStoredOnNavigationStack = true)
        {
            string oldPath = CurrentPath;
            CurrentPath = path;

            if (!Routes.TryGetValue(path, out Func<NavArgs?, NavRoute>? route))
            {
                //if the path is not found, try the empty string; if not even that is found, just pass null to remove the element
                CurrentRoute = Routes.TryGetValue("", out route) ? route.Invoke(args) : null;
                if (isStoredOnNavigationStack && path != CurrentPath)
                {
                    _navigationStack.Push(new Tuple<string, NavArgs?>(path, args));
                }

                NavigationFailedEvent?.Invoke(this, new NavigationFailedEventArgs(oldPath, CurrentPath));
                return;
            }

            CurrentRoute = route.Invoke(args);
            if (isStoredOnNavigationStack && path != CurrentPath)
            {
                _navigationStack.Push(new Tuple<string, NavArgs?>(path, args));
            }

            NavigatedEvent?.Invoke(this, new NavigatedEventArgs(oldPath, CurrentPath));
        }

        /// <summary>
        /// Go back to the previous route if at least one exists or has been added when calling <see cref="Navigate"/>.
        /// </summary>
        /// <returns>True if there were any previous routes, false otherwise.</returns>
        public bool GoBack()
        {
            if (!_navigationStack.TryPop(out Tuple<string, NavArgs?>? nav))
            {
                return false;
            }

            _backStack.Push(nav);
            Navigate(nav.Item1, nav.Item2, false);
            return true;
        }

        /// <summary>
        /// Go forward from any routes that were navigated to using <see cref="GoBack"/> if any routes exist.
        /// </summary>
        /// <returns>True if at least a route that was navigated using <see cref="GoBack"/> exists, false otherwise.</returns>
        public bool GoForward()
        {
            if (!_backStack.TryPop(out Tuple<string, NavArgs?>? nav))
            {
                return false;
            }

            _navigationStack.Push(nav);
            Navigate(nav.Item1, nav.Item2, false);
            return true;
        }

        /// <summary>
        /// Recalls <see cref="Navigate"/> so any changes to the <see cref="Routes"/> are reflected on the current
        /// route (navigating without calling this will still respect the new <see cref="Routes"/>). The child element
        /// will be removed from the document, then added again, which can slow the application down, so use with caution.
        /// </summary>
        public void Refresh()
        {
            Navigate(CurrentPath);
        }

        #endregion
    }
}
