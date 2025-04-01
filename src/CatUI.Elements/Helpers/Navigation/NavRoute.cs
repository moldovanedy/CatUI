using CatUI.Data;

namespace CatUI.Elements.Helpers.Navigation
{
    /// <summary>
    /// Represents a route in a <see cref="Navigator"/>. Will support transitions (animations) between routes when navigating.
    /// </summary>
    public class NavRoute : CatObject
    {
        /// <summary>
        /// The element that is used as the root element of this route. This will be presented to the user when it
        /// navigates to the path to which this NavRoute is mapped to.
        /// </summary>
        public Element RouteElement { get; set; }
        //TODO: transition support
        //

        /// <summary>
        /// Creates a route with a given element as <see cref="RouteElement"/>.
        /// </summary>
        /// <param name="routeElement">The <see cref="RouteElement"/>.</param>
        public NavRoute(Element routeElement)
        {
            RouteElement = routeElement;
        }

        /// <inheritdoc cref="Element.Duplicate"/>
        /// <remarks>
        /// The <see cref="RouteElement"/> is not cloned, but given as-is.
        /// </remarks>
        public override NavRoute Duplicate()
        {
            return new NavRoute(RouteElement);
        }
    }
}
