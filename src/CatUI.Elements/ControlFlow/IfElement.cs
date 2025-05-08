using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Shapes;
using CatUI.Utils;

namespace CatUI.Elements.ControlFlow
{
    /// <summary>
    /// A control flow element that attaches one element (or another if set) based on a given condition. DO NOT
    /// directly manipulate children (add, remove), as they are used internally and might cause errors otherwise.
    /// </summary>
    public class IfElement : ControlFlowElementBase
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<IfElement>? Ref
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

        private ObjectRef<IfElement>? _ref;

        /// <summary>
        /// The condition that determines which element is attached to the Document. Changing its
        /// <see cref="ObservableProperty{T}.Value"/> will trigger the evaluation and change the attached element.
        /// By default, it will be an <see cref="ObservableProperty{T}"/> with a value of true.
        /// </summary>
        public ObservableProperty<bool> Condition
        {
            get => _condition;
            set
            {
                SetCondition(value);
                ConditionProperty.Value = value;
            }
        }

        private ObservableProperty<bool> _condition = new(true);

        public ObservableProperty<ObservableProperty<bool>> ConditionProperty { get; }
            = new(new ObservableProperty<bool>(true));

        private void SetCondition(ObservableProperty<bool>? value)
        {
            if (value == null)
            {
                return;
            }

            _condition.ValueChangedEvent -= EvaluateCondition;
            _condition = value;
            _condition.ValueChangedEvent += EvaluateCondition;

            EvaluateCondition(value.Value);
        }

        /// <summary>
        /// The element that will be attached to the Document if the <see cref="Condition"/> is true. This is
        /// required.
        /// </summary>
        public Element TrueBranchElement
        {
            get => _trueBranchElement;
            set
            {
                SetTrueBranchElement(value);
                TrueBranchElementProperty.Value = value;
            }
        }

        private Element _trueBranchElement;
        public ObservableProperty<Element> TrueBranchElementProperty { get; } = new(null);

        private void SetTrueBranchElement(Element? value)
        {
            if (value == null)
            {
                return;
            }

            _trueBranchElement = value;

            if (Condition.Value)
            {
                if (Children.Count == 1)
                {
                    Children.RemoveAt(0);
                }

                Children.Insert(0, value);
            }
        }

        /// <summary>
        /// The element that will be attached to the Document if the <see cref="Condition"/> is false. This is not
        /// required, but by default it's an empty element.
        /// </summary>
        public Element FalseBranchElement
        {
            get => _falseBranchElement;
            set
            {
                SetFalseBranchElement(value);
                FalseBranchElementProperty.Value = value;
            }
        }

        private Element _falseBranchElement = new();
        public ObservableProperty<Element> FalseBranchElementProperty { get; } = new(new Element());

        private void SetFalseBranchElement(Element? value)
        {
            if (value == null)
            {
                return;
            }

            _falseBranchElement = value;

            if (!Condition.Value)
            {
                if (Children.Count == 1)
                {
                    Children.RemoveAt(0);
                }

                Children.Insert(0, value);
            }
        }

        public IfElement(ObservableProperty<bool> condition, Element trueBranchElement)
        {
            Condition = condition;
            TrueBranchElement = trueBranchElement;
            //silence compiler
            _trueBranchElement = trueBranchElement;

            ConditionProperty.ValueChangedEvent += SetCondition;
            TrueBranchElementProperty.ValueChangedEvent += SetTrueBranchElement;
            FalseBranchElementProperty.ValueChangedEvent += SetFalseBranchElement;
        }

        public IfElement(ObservableProperty<bool> condition, Element trueBranchElement, Element falseBranchElement)
            : this(condition, trueBranchElement)
        {
            FalseBranchElement = falseBranchElement;
        }

        private void EvaluateCondition(bool condition)
        {
            //edge case on creation
            if (Children.Count == 0)
            {
                return;
            }

            Children.RemoveAt(0);
            Children.Insert(0, condition ? TrueBranchElement : FalseBranchElement);
        }

        public override IfElement Duplicate()
        {
            IfElement el = new(
                new ObservableProperty<bool>(Condition.Value),
                TrueBranchElement.Duplicate(),
                FalseBranchElement.Duplicate())
            {
                //
                State = State,
                Position = Position,
                Background = Background.Duplicate(),
                ClipPath = (ClipShape?)ClipPath?.Duplicate(),
                ClipType = ClipType,
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate(),
                Layout = Layout
            };

            DuplicateChildrenUtil(el);
            return el;
        }
    }
}
