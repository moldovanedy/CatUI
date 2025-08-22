using CatUI.Elements.ControlFlow;
using CatUI.Utils;

namespace CatUI.Elements.Buttons
{
    public class TriStateCheckBoxIndicator : SwitchElement<CheckBox.CheckBoxState>
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<TriStateCheckBoxIndicator>? Ref
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

        private ObjectRef<TriStateCheckBoxIndicator>? _ref;

        /// <summary>
        /// The element that appears when <see cref="SwitchElement{T}.Value"/> is
        /// <see cref="CheckBox.CheckBoxState.Checked"/> (set automatically by <see cref="CheckBox"/> when used there).
        /// </summary>
        public Element CheckedElement
        {
            get => CaseLabels[0].MatchElementFunction.Invoke(CheckBox.CheckBoxState.Checked);
            set
            {
                CaseLabels[0].MatchElementFunction = _ => value;
                Reevaluate();
            }
        }

        /// <summary>
        /// The element that appears when <see cref="SwitchElement{T}.Value"/> is
        /// <see cref="CheckBox.CheckBoxState.Unchecked"/> (set automatically by <see cref="CheckBox"/> when used there).
        /// </summary>
        public Element UncheckedElement
        {
            get => CaseLabels[1].MatchElementFunction.Invoke(CheckBox.CheckBoxState.Unchecked);
            set
            {
                CaseLabels[1].MatchElementFunction = _ => value;
                Reevaluate();
            }
        }

        /// <summary>
        /// The element that appears when <see cref="SwitchElement{T}.Value"/> is
        /// <see cref="CheckBox.CheckBoxState.Indeterminate"/> (set automatically by <see cref="CheckBox"/> when
        /// used there). If it's not set, Indeterminate will equal to <see cref="CheckBox.CheckBoxState.Unchecked"/>.
        /// </summary>
        public Element? IndeterminateElement
        {
            get =>
                _isIndeterminateSet
                    ? CaseLabels[2].MatchElementFunction.Invoke(CheckBox.CheckBoxState.Indeterminate)
                    : null;
            set
            {
                CaseLabels[2].MatchElementFunction = _ => value ?? new Element();
                _isIndeterminateSet = value != null;
                Reevaluate();
            }
        }

        private bool _isIndeterminateSet;

        public TriStateCheckBoxIndicator(
            CheckBox.CheckBoxState initialValue,
            Element checkedElement,
            Element uncheckedElement,
            Element? indeterminateElement = null)
            : base(initialValue)
        {
            Init(checkedElement, uncheckedElement, indeterminateElement);
        }

        public TriStateCheckBoxIndicator(TriStateCheckBoxIndicator other) : base(other)
        {
            Value = other.Value;
            Init(
                other.CheckedElement.Duplicate(),
                other.UncheckedElement.Duplicate(),
                other.IndeterminateElement?.Duplicate());
        }

        public override TriStateCheckBoxIndicator Duplicate()
        {
            var el = new TriStateCheckBoxIndicator(this);
            DuplicateChildrenUtil(el);
            return el;
        }

        private void Init(
            Element checkedElement,
            Element uncheckedElement,
            Element? indeterminateElement = null)
        {
            CaseLabels.Add(new ExactCaseLabel(CheckBox.CheckBoxState.Checked, _ => checkedElement));
            CaseLabels.Add(new ExactCaseLabel(CheckBox.CheckBoxState.Unchecked, _ => uncheckedElement));
            CaseLabels.Add(new ExactCaseLabel(CheckBox.CheckBoxState.Indeterminate,
                _ => indeterminateElement ?? new Element()));

            _isIndeterminateSet = indeterminateElement != null;
            Reevaluate();
        }
    }
}
