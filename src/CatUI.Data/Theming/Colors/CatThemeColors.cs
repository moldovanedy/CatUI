using System.Diagnostics.CodeAnalysis;

namespace CatUI.Data.Theming.Colors
{
    /// <summary>
    /// The theme colors utility for your application. These are just color values that change based on either user preference
    /// (e.g. light/dark scheme, normal or high contrast) or your own setting. Without any theme setup, these are just
    /// white for background and black for content (text/icons). These are not used by any element by default, so you
    /// are not forced to use them.
    /// </summary>
    /// <remarks>
    /// These are heavily inspired by Material Design 3 by Google, except for Success colors. Use of these properties
    /// does not imply any kind of link to Material Design 3, as these are just colors in the end. Use of Material
    /// Design 3 is not forced by CatUI, nor is it discouraged or encouraged.
    /// </remarks>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class CatThemeColors
    {
        internal CatThemeColors()
        {
            PrimaryProperty.ValueChangedEvent += SetPrimary;
            OnPrimaryProperty.ValueChangedEvent += SetOnPrimary;
            PrimaryContainerProperty.ValueChangedEvent += SetPrimaryContainer;
            OnPrimaryContainerProperty.ValueChangedEvent += SetOnPrimaryContainer;

            SecondaryProperty.ValueChangedEvent += SetSecondary;
            OnSecondaryProperty.ValueChangedEvent += SetOnSecondary;
            SecondaryContainerProperty.ValueChangedEvent += SetSecondaryContainer;
            OnSecondaryContainerProperty.ValueChangedEvent += SetOnSecondaryContainer;

            TertiaryProperty.ValueChangedEvent += SetTertiary;
            OnTertiaryProperty.ValueChangedEvent += SetOnTertiary;
            TertiaryContainerProperty.ValueChangedEvent += SetTertiaryContainer;
            OnTertiaryContainerProperty.ValueChangedEvent += SetTertiaryContainer;

            ErrorProperty.ValueChangedEvent += SetError;
            OnErrorProperty.ValueChangedEvent += SetOnError;
            ErrorContainerProperty.ValueChangedEvent += SetErrorContainer;
            OnErrorContainerProperty.ValueChangedEvent += SetOnErrorContainer;

            SuccessProperty.ValueChangedEvent += SetSuccess;
            OnSuccessProperty.ValueChangedEvent += SetOnSuccess;
            SuccessContainerProperty.ValueChangedEvent += SetSuccessContainer;
            OnSuccessContainerProperty.ValueChangedEvent += SetOnSuccessContainer;

            SurfaceProperty.ValueChangedEvent += SetSurface;
            SurfaceDimProperty.ValueChangedEvent += SetSurfaceDim;
            SurfaceBrightProperty.ValueChangedEvent += SetSurfaceBright;
            SurfaceContainerLowestProperty.ValueChangedEvent += SetSurfaceContainerLowest;
            SurfaceContainerLowProperty.ValueChangedEvent += SetSurfaceContainerLow;
            SurfaceContainerProperty.ValueChangedEvent += SetSurfaceContainer;
            SurfaceContainerHighProperty.ValueChangedEvent += SetSurfaceContainerHigh;
            SurfaceContainerHighestProperty.ValueChangedEvent += SetSurfaceContainerHighest;

            OnSurfaceProperty.ValueChangedEvent += SetOnSurface;
            OnSurfaceVariantProperty.ValueChangedEvent += SetOnSurfaceVariant;

            InverseSurfaceProperty.ValueChangedEvent += SetInverseSurface;
            InverseOnSurfaceProperty.ValueChangedEvent += SetInverseOnSurface;
            InversePrimaryProperty.ValueChangedEvent += SetInversePrimary;

            OutlineProperty.ValueChangedEvent += SetOutline;
            OutlineVariantProperty.ValueChangedEvent += SetOutlineVariant;
            ScrimProperty.ValueChangedEvent += SetScrim;
            ShadowProperty.ValueChangedEvent += SetShadow;
        }

        ~CatThemeColors()
        {
            PrimaryProperty = null!;
            OnPrimaryProperty = null!;
            PrimaryContainerProperty = null!;
            OnPrimaryContainerProperty = null!;

            SecondaryProperty = null!;
            OnSecondaryProperty = null!;
            SecondaryContainerProperty = null!;
            OnSecondaryContainerProperty = null!;

            TertiaryProperty = null!;
            OnTertiaryProperty = null!;
            TertiaryContainerProperty = null!;
            OnTertiaryContainerProperty = null!;

            ErrorProperty = null!;
            OnErrorProperty = null!;
            ErrorContainerProperty = null!;
            OnErrorContainerProperty = null!;

            SuccessProperty = null!;
            OnSuccessProperty = null!;
            SuccessContainerProperty = null!;
            OnSuccessContainerProperty = null!;

            SurfaceProperty = null!;
            SurfaceDimProperty = null!;
            SurfaceBrightProperty = null!;
            SurfaceContainerLowestProperty = null!;
            SurfaceContainerLowProperty = null!;
            SurfaceContainerProperty = null!;
            SurfaceContainerHighProperty = null!;
            SurfaceContainerHighestProperty = null!;

            OnSurfaceProperty = null!;
            OnSurfaceVariantProperty = null!;

            InverseSurfaceProperty = null!;
            InverseOnSurfaceProperty = null!;
            InversePrimaryProperty = null!;

            OutlineProperty = null!;
            OutlineVariantProperty = null!;
            ScrimProperty = null!;
            ShadowProperty = null!;
        }

        #region Primary

        /// <summary>
        /// The background color of highest emphasis elements.
        /// </summary>
        public Color Primary
        {
            get => _primary;
            internal set
            {
                SetPrimary(value);
                PrimaryProperty.Value = value;
            }
        }

        private Color _primary = new(0xff_ff_ff);
        public ObservableProperty<Color> PrimaryProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetPrimary(Color value)
        {
            _primary = value;
        }

        /// <summary>
        /// The text/icon color against primary elements.
        /// </summary>
        public Color OnPrimary
        {
            get => _onPrimary;
            internal set
            {
                SetOnPrimary(value);
                OnPrimaryProperty.Value = value;
            }
        }

        private Color _onPrimary = new(0);
        public ObservableProperty<Color> OnPrimaryProperty { get; private set; } = new(new Color(0));

        private void SetOnPrimary(Color value)
        {
            _onPrimary = value;
        }

        /// <summary>
        /// The background color of highest emphasis containers. 
        /// </summary>
        public Color PrimaryContainer
        {
            get => _primaryContainer;
            internal set
            {
                SetPrimaryContainer(value);
                PrimaryContainerProperty.Value = value;
            }
        }

        private Color _primaryContainer = new(0xff_ff_ff);
        public ObservableProperty<Color> PrimaryContainerProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetPrimaryContainer(Color value)
        {
            _primaryContainer = value;
        }

        /// <summary>
        /// The text/icon color against primary containers.
        /// </summary>
        public Color OnPrimaryContainer
        {
            get => _onPrimaryContainer;
            internal set
            {
                SetOnPrimaryContainer(value);
                OnPrimaryContainerProperty.Value = value;
            }
        }

        private Color _onPrimaryContainer = new(0);
        public ObservableProperty<Color> OnPrimaryContainerProperty { get; private set; } = new(new Color(0));

        private void SetOnPrimaryContainer(Color value)
        {
            _onPrimaryContainer = value;
        }

        #endregion

        #region Secondary

        /// <summary>
        /// The background color of normal emphasis elements.
        /// </summary>
        public Color Secondary
        {
            get => _secondary;
            internal set
            {
                SetSecondary(value);
                SecondaryProperty.Value = value;
            }
        }

        private Color _secondary = new(0xff_ff_ff);
        public ObservableProperty<Color> SecondaryProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetSecondary(Color value)
        {
            _secondary = value;
        }

        /// <summary>
        /// The text/icon color against secondary elements.
        /// </summary>
        public Color OnSecondary
        {
            get => _onSecondary;
            internal set
            {
                SetOnSecondary(value);
                OnSecondaryProperty.Value = value;
            }
        }

        private Color _onSecondary = new(0);
        public ObservableProperty<Color> OnSecondaryProperty { get; private set; } = new(new Color(0));

        private void SetOnSecondary(Color value)
        {
            _onSecondary = value;
        }

        /// <summary>
        /// The background color of normal emphasis containers. 
        /// </summary>
        public Color SecondaryContainer
        {
            get => _secondaryContainer;
            internal set
            {
                SetSecondaryContainer(value);
                SecondaryContainerProperty.Value = value;
            }
        }

        private Color _secondaryContainer = new(0xff_ff_ff);
        public ObservableProperty<Color> SecondaryContainerProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetSecondaryContainer(Color value)
        {
            _secondaryContainer = value;
        }

        /// <summary>
        /// The text/icon color against secondary containers.
        /// </summary>
        public Color OnSecondaryContainer
        {
            get => _onSecondaryContainer;
            internal set
            {
                SetOnSecondaryContainer(value);
                OnSecondaryContainerProperty.Value = value;
            }
        }

        private Color _onSecondaryContainer = new(0);
        public ObservableProperty<Color> OnSecondaryContainerProperty { get; private set; } = new(new Color(0));

        private void SetOnSecondaryContainer(Color value)
        {
            _onSecondaryContainer = value;
        }

        #endregion

        #region Tertiary

        /// <summary>
        /// The background color of complementary elements.
        /// </summary>
        public Color Tertiary
        {
            get => _tertiary;
            internal set
            {
                SetTertiary(value);
                TertiaryProperty.Value = value;
            }
        }

        private Color _tertiary = new(0xff_ff_ff);
        public ObservableProperty<Color> TertiaryProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetTertiary(Color value)
        {
            _tertiary = value;
        }

        /// <summary>
        /// The text/icon color against tertiary elements.
        /// </summary>
        public Color OnTertiary
        {
            get => _onTertiary;
            internal set
            {
                SetOnTertiary(value);
                OnTertiaryProperty.Value = value;
            }
        }

        private Color _onTertiary = new(0);
        public ObservableProperty<Color> OnTertiaryProperty { get; private set; } = new(new Color(0));

        private void SetOnTertiary(Color value)
        {
            _onTertiary = value;
        }

        /// <summary>
        /// The background color of complementary containers. 
        /// </summary>
        public Color TertiaryContainer
        {
            get => _tertiaryContainer;
            internal set
            {
                SetTertiaryContainer(value);
                TertiaryContainerProperty.Value = value;
            }
        }

        private Color _tertiaryContainer = new(0xff_ff_ff);
        public ObservableProperty<Color> TertiaryContainerProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetTertiaryContainer(Color value)
        {
            _tertiaryContainer = value;
        }

        /// <summary>
        /// The text/icon color against tertiary containers.
        /// </summary>
        public Color OnTertiaryContainer
        {
            get => _onTertiaryContainer;
            internal set
            {
                SetOnTertiaryContainer(value);
                OnTertiaryContainerProperty.Value = value;
            }
        }

        private Color _onTertiaryContainer = new(0);
        public ObservableProperty<Color> OnTertiaryContainerProperty { get; private set; } = new(new Color(0));

        private void SetOnTertiaryContainer(Color value)
        {
            _onTertiaryContainer = value;
        }

        #endregion

        #region Error

        /// <summary>
        /// The background color of attention-grabbing elements that indicate urgency.
        /// </summary>
        public Color Error
        {
            get => _error;
            internal set
            {
                SetError(value);
                ErrorProperty.Value = value;
            }
        }

        private Color _error = new(0xff_ff_ff);
        public ObservableProperty<Color> ErrorProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetError(Color value)
        {
            _error = value;
        }

        /// <summary>
        /// The text/icon color against error elements.
        /// </summary>
        public Color OnError
        {
            get => _onError;
            internal set
            {
                SetOnError(value);
                OnErrorProperty.Value = value;
            }
        }

        private Color _onError = new(0);
        public ObservableProperty<Color> OnErrorProperty { get; private set; } = new(new Color(0));

        private void SetOnError(Color value)
        {
            _onError = value;
        }

        /// <summary>
        /// The background color of attention-grabbing containers that indicate urgency. 
        /// </summary>
        public Color ErrorContainer
        {
            get => _errorContainer;
            internal set
            {
                SetErrorContainer(value);
                ErrorContainerProperty.Value = value;
            }
        }

        private Color _errorContainer = new(0xff_ff_ff);
        public ObservableProperty<Color> ErrorContainerProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetErrorContainer(Color value)
        {
            _errorContainer = value;
        }

        /// <summary>
        /// The text/icon color against error containers.
        /// </summary>
        public Color OnErrorContainer
        {
            get => _onErrorContainer;
            internal set
            {
                SetOnErrorContainer(value);
                OnErrorContainerProperty.Value = value;
            }
        }

        private Color _onErrorContainer = new(0);
        public ObservableProperty<Color> OnErrorContainerProperty { get; private set; } = new(new Color(0));

        private void SetOnErrorContainer(Color value)
        {
            _onErrorContainer = value;
        }

        #endregion

        #region Success

        /// <summary>
        /// The background color of elements that communicate an action success.
        /// </summary>
        /// <remarks>This is an addition to Material Design 3.</remarks>
        public Color Success
        {
            get => _success;
            internal set
            {
                SetSuccess(value);
                SuccessProperty.Value = value;
            }
        }

        private Color _success = new(0xff_ff_ff);
        public ObservableProperty<Color> SuccessProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetSuccess(Color value)
        {
            _success = value;
        }

        /// <summary>
        /// The text/icon color against success elements.
        /// </summary>
        /// <remarks>This is an addition to Material Design 3.</remarks>
        public Color OnSuccess
        {
            get => _onSuccess;
            internal set
            {
                SetOnSuccess(value);
                OnSuccessProperty.Value = value;
            }
        }

        private Color _onSuccess = new(0);
        public ObservableProperty<Color> OnSuccessProperty { get; private set; } = new(new Color(0));

        private void SetOnSuccess(Color value)
        {
            _onSuccess = value;
        }

        /// <summary>
        /// The background color of containers that communicate an action success. 
        /// </summary>
        /// <remarks>This is an addition to Material Design 3.</remarks>
        public Color SuccessContainer
        {
            get => _successContainer;
            internal set
            {
                SetSuccessContainer(value);
                SuccessContainerProperty.Value = value;
            }
        }

        private Color _successContainer = new(0xff_ff_ff);
        public ObservableProperty<Color> SuccessContainerProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetSuccessContainer(Color value)
        {
            _successContainer = value;
        }

        /// <summary>
        /// The text/icon color against success containers.
        /// </summary>
        /// <remarks>This is an addition to Material Design 3.</remarks>
        public Color OnSuccessContainer
        {
            get => _onSuccessContainer;
            internal set
            {
                SetOnSuccessContainer(value);
                OnSuccessContainerProperty.Value = value;
            }
        }

        private Color _onSuccessContainer = new(0);
        public ObservableProperty<Color> OnSuccessContainerProperty { get; private set; } = new(new Color(0));

        private void SetOnSuccessContainer(Color value)
        {
            _onSuccessContainer = value;
        }

        #endregion

        #region BG surfaces

        /// <summary>
        /// The background color for surfaces.
        /// </summary>
        public Color Surface
        {
            get => _surface;
            internal set
            {
                SetSurface(value);
                SurfaceProperty.Value = value;
            }
        }

        private Color _surface = new(0xff_ff_ff);
        public ObservableProperty<Color> SurfaceProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetSurface(Color value)
        {
            _surface = value;
        }

        /// <summary>
        /// A darker background color for surfaces.
        /// </summary>
        public Color SurfaceDim
        {
            get => _surfaceDim;
            internal set
            {
                SetSurfaceDim(value);
                SurfaceDimProperty.Value = value;
            }
        }

        private Color _surfaceDim = new(0xff_ff_ff);
        public ObservableProperty<Color> SurfaceDimProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetSurfaceDim(Color value)
        {
            _surfaceDim = value;
        }

        /// <summary>
        /// A lighter background color for surfaces.
        /// </summary>
        public Color SurfaceBright
        {
            get => _surfaceBright;
            internal set
            {
                SetSurfaceBright(value);
                SurfaceBrightProperty.Value = value;
            }
        }

        private Color _surfaceBright = new(0xff_ff_ff);
        public ObservableProperty<Color> SurfaceBrightProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetSurfaceBright(Color value)
        {
            _surfaceBright = value;
        }

        /// <summary>
        /// The lowest emphasis background color for surface containers.
        /// </summary>
        public Color SurfaceContainerLowest
        {
            get => _surfaceContainerLowest;
            internal set
            {
                SetSurfaceContainerLowest(value);
                SurfaceContainerLowestProperty.Value = value;
            }
        }

        private Color _surfaceContainerLowest = new(0xff_ff_ff);

        public ObservableProperty<Color> SurfaceContainerLowestProperty { get; private set; } =
            new(new Color(0xff_ff_ff));

        private void SetSurfaceContainerLowest(Color value)
        {
            _surfaceContainerLowest = value;
        }

        /// <summary>
        /// The low emphasis background color for surface containers.
        /// </summary>
        public Color SurfaceContainerLow
        {
            get => _surfaceContainerLow;
            internal set
            {
                SetSurfaceContainerLow(value);
                SurfaceContainerLowProperty.Value = value;
            }
        }

        private Color _surfaceContainerLow = new(0xff_ff_ff);
        public ObservableProperty<Color> SurfaceContainerLowProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetSurfaceContainerLow(Color value)
        {
            _surfaceContainerLow = value;
        }

        /// <summary>
        /// The normal emphasis background color for surface containers.
        /// </summary>
        public Color SurfaceContainer
        {
            get => _surfaceContainer;
            internal set
            {
                SetSurfaceContainer(value);
                SurfaceContainerProperty.Value = value;
            }
        }

        private Color _surfaceContainer = new(0xff_ff_ff);
        public ObservableProperty<Color> SurfaceContainerProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetSurfaceContainer(Color value)
        {
            _surfaceContainer = value;
        }

        /// <summary>
        /// The high emphasis background color for surface containers.
        /// </summary>
        public Color SurfaceContainerHigh
        {
            get => _surfaceContainerHigh;
            internal set
            {
                SetSurfaceContainerHigh(value);
                SurfaceContainerHighProperty.Value = value;
            }
        }

        private Color _surfaceContainerHigh = new(0xff_ff_ff);

        public ObservableProperty<Color> SurfaceContainerHighProperty { get; private set; } =
            new(new Color(0xff_ff_ff));

        private void SetSurfaceContainerHigh(Color value)
        {
            _surfaceContainerHigh = value;
        }

        /// <summary>
        /// The highest emphasis background color for surface containers.
        /// </summary>
        public Color SurfaceContainerHighest
        {
            get => _surfaceContainerHighest;
            internal set
            {
                SetSurfaceContainerHighest(value);
                SurfaceContainerHighestProperty.Value = value;
            }
        }

        private Color _surfaceContainerHighest = new(0xff_ff_ff);

        public ObservableProperty<Color> SurfaceContainerHighestProperty { get; private set; } =
            new(new Color(0xff_ff_ff));

        private void SetSurfaceContainerHighest(Color value)
        {
            _surfaceContainerHighest = value;
        }

        #endregion

        #region FG surfaces

        /// <summary>
        /// The regular text/icon color on surfaces.
        /// </summary>
        public Color OnSurface
        {
            get => _onSurface;
            internal set
            {
                SetOnSurface(value);
                OnSurfaceProperty.Value = value;
            }
        }

        private Color _onSurface = new(0);
        public ObservableProperty<Color> OnSurfaceProperty { get; private set; } = new(new Color(0));

        private void SetOnSurface(Color value)
        {
            _onSurface = value;
        }

        /// <summary>
        /// The low emphasis text/icon color on surfaces.
        /// </summary>
        public Color OnSurfaceVariant
        {
            get => _onSurfaceVariant;
            internal set
            {
                SetOnSurfaceVariant(value);
                OnSurfaceVariantProperty.Value = value;
            }
        }

        private Color _onSurfaceVariant = new(0);
        public ObservableProperty<Color> OnSurfaceVariantProperty { get; private set; } = new(new Color(0));

        private void SetOnSurfaceVariant(Color value)
        {
            _onSurfaceVariant = value;
        }

        #endregion

        #region Inverse

        /// <summary>
        /// The inverse background color for surfaces, generally used for high contrast/emphasis between surfaces.
        /// </summary>
        public Color InverseSurface
        {
            get => _inverseSurface;
            internal set
            {
                SetInverseSurface(value);
                InverseSurfaceProperty.Value = value;
            }
        }

        private Color _inverseSurface = new(0);
        public ObservableProperty<Color> InverseSurfaceProperty { get; private set; } = new(new Color(0));

        private void SetInverseSurface(Color value)
        {
            _inverseSurface = value;
        }

        /// <summary>
        /// The text/icon color on inverted surfaces (<see cref="InverseSurface"/>).
        /// </summary>
        public Color InverseOnSurface
        {
            get => _inverseOnSurface;
            internal set
            {
                SetInverseOnSurface(value);
                InverseOnSurfaceProperty.Value = value;
            }
        }

        private Color _inverseOnSurface = new(0xff_ff_ff);
        public ObservableProperty<Color> InverseOnSurfaceProperty { get; private set; } = new(new Color(0xff_ff_ff));

        private void SetInverseOnSurface(Color value)
        {
            _inverseOnSurface = value;
        }

        /// <summary>
        /// The actionable elements color on inverted surfaces (<see cref="InverseSurface"/>).
        /// </summary>
        public Color InversePrimary
        {
            get => _inversePrimary;
            internal set
            {
                SetInversePrimary(value);
                InversePrimaryProperty.Value = value;
            }
        }

        private Color _inversePrimary = new(0);
        public ObservableProperty<Color> InversePrimaryProperty { get; private set; } = new(new Color(0));

        private void SetInversePrimary(Color value)
        {
            _inversePrimary = value;
        }

        #endregion

        #region Misc

        /// <summary>
        /// The outline color of important elements.
        /// </summary>
        public Color Outline
        {
            get => _outline;
            internal set
            {
                SetOutline(value);
                OutlineProperty.Value = value;
            }
        }

        private Color _outline = new(0);
        public ObservableProperty<Color> OutlineProperty { get; private set; } = new(new Color(0));

        private void SetOutline(Color value)
        {
            _outline = value;
        }

        /// <summary>
        /// The outline color of low emphasis or decorative elements.
        /// </summary>
        public Color OutlineVariant
        {
            get => _outlineVariant;
            internal set
            {
                SetOutlineVariant(value);
                OutlineVariantProperty.Value = value;
            }
        }

        private Color _outlineVariant = new(0);
        public ObservableProperty<Color> OutlineVariantProperty { get; private set; } = new(new Color(0));

        private void SetOutlineVariant(Color value)
        {
            _outlineVariant = value;
        }

        /// <summary>
        /// Can be used as the background of a high emphasis actionable element. Generally the same color regardless of
        /// color scheme.
        /// </summary>
        public Color Scrim
        {
            get => _scrim;
            internal set
            {
                SetScrim(value);
                ScrimProperty.Value = value;
            }
        }

        private Color _scrim = new(0);
        public ObservableProperty<Color> ScrimProperty { get; private set; } = new(new Color(0));

        private void SetScrim(Color value)
        {
            _scrim = value;
        }

        /// <summary>
        /// The color of the shadows applied to surfaces. Generally black (#000).
        /// </summary>
        public Color Shadow
        {
            get => _shadow;
            internal set
            {
                SetShadow(value);
                ShadowProperty.Value = value;
            }
        }

        private Color _shadow = new(0);
        public ObservableProperty<Color> ShadowProperty { get; private set; } = new(new Color(0));

        private void SetShadow(Color value)
        {
            _shadow = value;
        }

        #endregion
    }
}
