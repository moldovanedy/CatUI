using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CatUI.Data.Containers.LinearContainers
{
    public class LinearArrangement : Arrangement, INotifyPropertyChanged
    {
        /// <summary>
        /// See <see cref="JustificationType"/> for more info. Default value is <see cref="JustificationType.Start"/>.
        /// </summary>
        public JustificationType ContentJustification
        {
            get => _contentJustification;
            set
            {
                _contentJustification = value;
                NotifyPropertyChanged();
            }
        }

        private JustificationType _contentJustification = JustificationType.Start;

        /// <summary>
        /// Represents the space that should be added between each child. If <see cref="ContentJustification"/> is
        /// not <see cref="JustificationType.Start"/>, <see cref="JustificationType.Center"/> or
        /// <see cref="JustificationType.End"/>, this has no effect.
        /// </summary>
        public Dimension Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                NotifyPropertyChanged();
            }
        }

        private Dimension _spacing;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsSpacingRelevant =>
            ContentJustification == JustificationType.Start ||
            ContentJustification == JustificationType.Center ||
            ContentJustification == JustificationType.End;

        public LinearArrangement() { }

        public LinearArrangement(JustificationType contentJustification, Dimension spacing)
        {
            ContentJustification = contentJustification;
            Spacing = spacing;
        }


        #region Convenience

        /// <summary>
        /// Utility to create a LinearArrangement with a given spacing between each object.
        /// </summary>
        /// <param name="spacing">The space between each object.</param>
        /// <param name="contentJustification">
        /// The justification type. It MUST be on of <see cref="JustificationType.Start"/>,
        /// <see cref="JustificationType.Center"/> or <see cref="JustificationType.End"/>, otherwise the default value
        /// of <see cref="JustificationType.Start"/> will be used.
        /// </param>
        /// <returns>A LinearArrangement instance.</returns>
        public static LinearArrangement SpacedBy(
            Dimension spacing,
            JustificationType contentJustification = JustificationType.Start)
        {
            if (
                contentJustification != JustificationType.Start &&
                contentJustification != JustificationType.Center &&
                contentJustification != JustificationType.End
            )
            {
                contentJustification = JustificationType.Start;
            }

            return new LinearArrangement { Spacing = spacing, ContentJustification = contentJustification };
        }

        /// <summary>
        /// Utility to create a LinearArrangement with the given justification type.
        /// </summary>
        /// <param name="contentJustification">The justification type</param>
        /// <returns>A LinearArrangement instance.</returns>
        public static LinearArrangement WithContentJustification(JustificationType contentJustification)
        {
            return new LinearArrangement { ContentJustification = contentJustification };
        }

        #endregion

        /// <inheritdoc cref="CatObject.Duplicate"/>
        public override CatObject Duplicate()
        {
            return new LinearArrangement(ContentJustification, Spacing);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Specifies the type of arrangement for this container's content.
        /// </summary>
        public enum JustificationType
        {
            /// <summary>
            /// All children will be packed at the start of the container (in RowContainer it means left (when LTR),
            /// in ColumnContainer it means top).
            /// </summary>
            Start = 0,

            /// <summary>
            /// All children will be packed around the center of the container.
            /// </summary>
            Center = 1,

            /// <summary>
            /// All children will be packed at the end of the container (in RowContainer it means right (when LTR),
            /// in ColumnContainer it means bottom).
            /// </summary>
            End = 2,

            /// <summary>
            /// Evenly distributes the children with plenty of space between them. There will also be space before the
            /// first child, as well as after the last child (this space is half of the space between the children
            /// themselves). Visually: #1##2##3#, where # represents a free space quota.
            /// </summary>
            /// <remarks>
            /// It will stretch to the entire width of the container, unless there is a single child, in which case it
            /// will behave like <see cref="Center"/>.
            /// </remarks>
            SpaceAround = 3,

            /// <summary>
            /// Evenly distributes the children with plenty of space between them. There will NOT be any space before the
            /// first child or after the last child (in contrast to <see cref="SpaceAround"/>).
            /// Visually: 1##2##3, where ## represents a free space quota.
            /// </summary>
            /// <remarks>
            /// It will stretch to the entire width of the container, unless there is a single child, in which case it
            /// will behave like <see cref="Center"/>.
            /// </remarks>
            SpaceBetween = 4,

            /// <summary>
            /// Evenly distributes the children with plenty of space between them, before the first child and after
            /// the last child. All the space is even, in contrast to <see cref="SpaceAround"/>.
            /// Visually: #1#2#3#, where # represents a free space quota.
            /// </summary>
            /// <remarks>
            /// It will stretch to the entire width of the container, unless there is a single child, in which case it
            /// will behave like <see cref="Center"/>.
            /// </remarks>
            SpaceEvenly = 5
        }
    }
}
