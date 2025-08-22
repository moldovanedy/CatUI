using System.Collections.Generic;
using CatUI.Elements.Behaviors;

namespace CatUI.Elements.DocumentManagers
{
    public class PseudoClassesManager
    {
        private readonly Dictionary<string, uint> _registeredPseudoClasses = new()
        {
            { Element.PSEUDO_CLASS_PRESSED, 200 },
            { Element.PSEUDO_CLASS_HOVER, 150 },
            { IFocusable.PSEUDO_CLASS_FOCUSED, 100 },
            { IToggleable.PSEUDO_CLASS_ACTIVE, 50 },
            { Element.PSEUDO_CLASS_NORMAL, 0 }
        };

        /// <summary>
        /// Registers a pseudo-class so it can be used by elements. Already defined classes cannot be overwritten, so
        /// false will be returned when there already is a pseudo-class with the same name, regardless of the priority.
        /// Keep the overall pseudo class count low, both for performance and maintainability.
        /// </summary>
        /// <remarks>
        /// You should call this method directly in the constructor of an element if the class name is known at compile
        /// time. If the name is defined in an interface, implementations should call this in their constructor.
        /// Another option is to simply register all desired pseudo-classes at the app startup (the built-in
        /// pseudo-classes are defined at compile-time, so this method is OK).
        /// </remarks>
        /// <param name="className">The name of the pseudo-class.</param>
        /// <param name="priority">
        /// The priority (the higher the number, the higher the priority). It's safe to use any value above 500, as those
        /// will never be used by the built-in pseudo-classes.
        /// </param>
        /// <returns>
        /// Returns true if the class was added, false if there already is a pseudo-class with the same name.
        /// </returns>
        public bool RegisterPseudoClass(string className, uint priority = uint.MaxValue)
        {
            return _registeredPseudoClasses.TryAdd(className, priority);
        }

        /// <summary>
        /// Removes a pseudo-class from the internal dictionary and also removes that pseudo-class from all elements
        /// (this might be an expensive operation if lots of elements use this pseudo-class). This also triggers style
        /// recalculations and refreshes on all affected elements, so try to avoid using this method unless really
        /// necessary.
        /// </summary>
        /// <param name="document">A reference to the UI document where you want this change to take effect.</param>
        /// <param name="className">The pseudo-class to remove from the document.</param>
        public void UnregisterPseudoClass(UiDocument document, string className)
        {
            if (!_registeredPseudoClasses.Remove(className))
            {
                return;
            }

            if (document.Root != null)
            {
                RemovePseudoClassFromElementRecursive(document.Root, className);
            }
        }

        internal bool AddPseudoClassToElement(Element el, string className)
        {
            if (!_registeredPseudoClasses.TryGetValue(className, out uint thisPriority))
            {
                return false;
            }

            for (int i = 0; i < el.InternalPseudoClasses.Count; i++)
            {
                if (!_registeredPseudoClasses.TryGetValue(el.InternalPseudoClasses[i], out uint priority))
                {
                    continue;
                }

                if (priority < thisPriority)
                {
                    el.InternalPseudoClasses.Insert(i, className);
                    return true;
                }
            }

            el.InternalPseudoClasses.Add(className);
            return true;
        }

        private static void RemovePseudoClassFromElementRecursive(Element el, string className)
        {
            el.InternalPseudoClasses.Remove(className);
            foreach (Element child in el.Children)
            {
                RemovePseudoClassFromElementRecursive(child, className);
            }
        }
    }
}
