using CatUI.Data;
using CatUI.Data.Events.Input.Gestures;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.Behaviors;
using CatUI.Utils;

namespace CatUI.Elements.Buttons
{
    public class BaseButton : Element, IClickable, IFocusable
    {
        public const string STATE_DISABLED = "disabled";

        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<BaseButton>? Ref
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

        private ObjectRef<BaseButton>? _ref;

        public bool CanUserCancelClick
        {
            get => _canUserCancelClick;
            set => CanUserCancelClickProperty.Value = value;
        }

        private bool _canUserCancelClick = true;
        public ObservableProperty<bool> CanUserCancelClickProperty { get; } = new(true);

        private void SetCanUserCancelClick(bool value)
        {
            _canUserCancelClick = value;
            SetLocalValue(nameof(CanUserCancelClick), value);
        }

        public event ClickEventHandler? ClickEvent;

        public ClickEventHandler? OnClick
        {
            get => _onClick;
            set
            {
                ClickEvent -= _onClick;
                _onClick = value;
                ClickEvent += _onClick;
            }
        }

        private ClickEventHandler? _onClick;


        #region Focus

        public IFocusable? NextFocusableElement { get; set; }
        public IFocusable? PreviousFocusableElement { get; set; }

        public void OnFocusStateChanged(bool hasEnteredFocus)
        {
            if (hasEnteredFocus)
            {
                AddPseudoClass(IFocusable.PSEUDO_CLASS_FOCUSED);
            }
            else
            {
                RemovePseudoClass(IFocusable.PSEUDO_CLASS_FOCUSED);
            }

            FocusChangedEvent?.Invoke(this, hasEnteredFocus);
        }

        public bool IsFocusEnabled
        {
            get => _isFocusEnabled;
            set => IsFocusEnabledProperty.Value = value;
        }

        private bool _isFocusEnabled = true;
        public ObservableProperty<bool> IsFocusEnabledProperty { get; } = new(true);

        private void SetIsFocusEnabled(bool value)
        {
            _isFocusEnabled = value;
            SetLocalValue(nameof(IsFocusEnabled), value);
        }

        public event FocusChangedEventHandler? FocusChangedEvent;

        public FocusChangedEventHandler? OnFocusChanged
        {
            get => _onFocusChanged;
            set
            {
                FocusChangedEvent -= _onFocusChanged;
                _onFocusChanged = value;
                FocusChangedEvent += _onFocusChanged;
            }
        }

        private FocusChangedEventHandler? _onFocusChanged;

        #endregion

        private bool _isDown;

        public BaseButton()
        {
            Init();
        }

        public BaseButton(BaseButton other) : base(other)
        {
            Init();
            CanUserCancelClick = other.CanUserCancelClick;
        }

        private void Init()
        {
            CanUserCancelClickProperty.ValueChangedEvent += SetCanUserCancelClick;
            IsFocusEnabledProperty.ValueChangedEvent += SetIsFocusEnabled;
            PointerDownEvent += PrivatePointerDown;
            PointerUpEvent += PrivatePointerUp;
            ClickEvent += PrivateClick;
        }

        public virtual void Click(object sender, ClickEventArgs e) { }
        public virtual void FocusChanged(object sender, FocusChangedEventHandler e) { }

        /// <inheritdoc cref="IClickable.FocusedSelectActionTriggered"/>
        /// <remarks>
        /// For this BaseButton, this will trigger a <see cref="ClickEvent"/> with <see cref="ClickEventArgs"/> that
        /// have the position set as 0, while <see cref="ClickEventArgs.AbsolutePosition"/> will be the top-left
        /// point of the element.
        /// </remarks>
        public void FocusedSelectActionTriggered()
        {
            ClickEvent?.Invoke(
                this,
                new ClickEventArgs(Point2D.Zero, new Point2D(Bounds.X, Bounds.Y)));
        }

        public override BaseButton Duplicate()
        {
            var el = new BaseButton(this);
            DuplicateChildrenUtil(el);
            return el;
        }

        private void PrivatePointerDown(object sender, PointerDownEventArgs e)
        {
            _isDown = true;
        }

        private void PrivatePointerUp(object sender, PointerUpEventArgs e)
        {
            if (!_isDown)
            {
                return;
            }

            _isDown = false;
            if (e.WasCancelled && CanUserCancelClick)
            {
                return;
            }

            ClickEvent?.Invoke(this, new ClickEventArgs(e.Position, e.AbsolutePosition));
        }

        private void PrivateClick(object sender, ClickEventArgs e)
        {
            this.GrabFocus();
        }
    }
}
