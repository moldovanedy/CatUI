namespace CatUI.Elements.Behaviors
{
    /// <summary>
    /// Any element that implements this will ignore the <see cref="Element.PreferredWidth"/> and
    /// <see cref="Element.PreferredHeight"/>, only respecting the minimum and maximum width and height constraints
    /// (like <see cref="Element.MaxHeight"/>). It tries to occupy as little space as possible (in regard to
    /// the minimum size), but is free to expand until the maximum size is reached.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Elements that implement this interface can compute their size in a container, so they can set
    /// <see cref="Element.AbsoluteWidth"/> and <see cref="Element.AbsoluteHeight"/> whilst in a container.
    /// </para>
    /// <para>
    /// What exactly happens with an element depends on each element that implements this interface. See the remarks
    /// of <see cref="CanExpandHorizontally"/> and <see cref="CanExpandVertically"/> for each element for more information.
    /// </para>
    /// </remarks>
    public interface IExpandable
    {
        /// <summary>
        /// If true, will try to shrink as much as possible until <see cref="Element.MinWidth"/>, but will grow
        /// until <see cref="Element.MaxWidth"/> if necessary. <see cref="Element.PreferredWidth"/> will be ignored.
        /// </summary>
        public bool CanExpandHorizontally { get; set; }

        /// <summary>
        /// If true, will try to shrink as much as possible until <see cref="Element.MinHeight"/>, but will grow
        /// until <see cref="Element.MaxHeight"/> if necessary. <see cref="Element.PreferredHeight"/> will be ignored.
        /// </summary>
        public bool CanExpandVertically { get; set; }
    }
}
