using CatUI.Data;

namespace CatUI.Elements
{
    public static class El
    {
        /// <summary>
        /// A lambda function that has as parameter the new state the element is in and must return the value <see cref="T"/>,
        /// where the value is the value that will be given to the property from
        /// <see cref="El.Style{T}(CatUI.Elements.Element,CatUI.Data.ObservableProperty{T},PropertyStyleLambda{T})"/>
        /// or <see cref="El.Style{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value of <see cref="ObservableProperty{T}"/>.</typeparam>
        public delegate T PropertyStyleLambda<out T>(string? newState) where T : notnull;

        /// <summary>
        /// A lambda function that has as parameter the new state the element is in and the element itself. You should
        /// set the properties directly depending on the state (i.e. use a switch statement or something).
        /// </summary>
        public delegate void StyleLambda(string? newState, Element element);

        /// <summary>
        /// Will register the given lambda to get the value for the property whenever the style changes, acting like a
        /// styling utility (e.g. setting the <see cref="Element.Background"/> to different values based on whether the
        /// element is in normal, "hover" or "pressed" state).
        /// </summary>
        /// <param name="el">The element you want to style.</param>
        /// <param name="property">The element's property (MUST be from the same instance as el).</param>
        /// <param name="propertyStyleLambda">The lambda that will give the value based on the element's state.</param>
        /// <typeparam name="T">The type of the property that you want to style.</typeparam>
        /// <remarks>
        /// Warning! This will simply subscribe to <see cref="Element.StateProperty"/> without unsubscribing, so calling
        /// this multiple times for the same element and the same property will call each lambda and the end result is
        /// unpredictable. The only way to unsubscribe is to call <see cref="Element.UnbindStyles"/>, however that will
        /// also unsubscribe other functions unrelated to this method. Therefore, it is recommended to only use this
        /// method once for a given element and its property (you CAN use it for the same element, but other properties)
        /// for the lifetime of the element (i.e. use it in <see cref="Element.InitializationFunction"/>, then don't
        /// interfere with that initialization function again).
        /// </remarks>
        /// <example>
        /// <code>
        /// // different background colors based on state
        /// InitializationFunction = el => El.Style(
        /// el,
        /// el.BackgroundProperty,
        /// (state) => {
        ///     switch(state)
        ///     {
        ///         default:
        ///         case null: return new ColorBrush(new Color(0x21_21_21));
        ///         case "hover": return new ColorBrush(new Color(0x42_42_42));
        ///         case "pressed": return new ColorBrush(new Color(0x31_31_31));
        ///     }
        /// }),
        /// </code>
        /// </example>
        public static void Style<T>(
            Element el,
            ObservableProperty<T> property,
            PropertyStyleLambda<T> propertyStyleLambda)
            where T : notnull
        {
            el.StateProperty.ValueChangedEvent += state => property.Value = propertyStyleLambda.Invoke(state);
        }

        /// <summary>
        /// The same as <see cref="Style{T}(CatUI.Elements.Element,CatUI.Data.ObservableProperty{T},PropertyStyleLambda{T})"/>,
        /// but for all element properties directly. The given delegate will have as parameters the element itself and
        /// the new state, then you are free to set the properties directly inside that delegate taking into account the state.
        /// </summary>
        /// <param name="el">The element you want to style.</param>
        /// <param name="styleLambda">
        /// The function that will be called each time the state changes. See <see cref="StyleLambda"/> for more
        /// information or the given example on how to use it.
        /// </param>
        /// <remarks>
        /// Warning! This will simply subscribe to <see cref="Element.StateProperty"/> without unsubscribing, so calling
        /// this multiple times for the same element will call each lambda and the end result is unpredictable.
        /// The only way to unsubscribe is to call <see cref="Element.UnbindStyles"/>, however that will
        /// also unsubscribe other functions unrelated to this method. Therefore, it is recommended to only use this
        /// method once for a given element for the lifetime of the element (i.e. use it in
        /// <see cref="Element.InitializationFunction"/>, then don't interfere with that initialization function again).
        /// </remarks>
        /// <example>
        /// <code>
        /// El.Style(
        ///     e,
        ///     (state, el) =>
        ///     {
        ///         switch (state)
        ///         {
        ///             default:
        ///             case null:
        ///                 el.Background = new ColorBrush(new Color(0x21_21_21));
        ///                 //other properties
        ///                 break;
        ///             case "hover":
        ///                 el.Background = new ColorBrush(new Color(0x42_42_42));
        ///                 break;
        ///         }
        ///     })
        /// </code>
        /// </example>
        public static void Style(Element el, StyleLambda styleLambda)
        {
            el.StateProperty.ValueChangedEvent += state => styleLambda.Invoke(state, el);
        }
    }
}
