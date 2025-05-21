using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Shapes;
using CatUI.Utils;

namespace CatUI.Elements.ControlFlow
{
    /// <summary>
    /// A control-flow element that will attach a single other element to the document (as a child) based on a
    /// switch-like condition. There can only ever be one element attached to the document. DO NOT directly manipulate
    /// children (add, remove), as they are used internally and might cause errors otherwise.
    /// </summary>
    /// <typeparam name="T">The type of data that you want to use as a switch.</typeparam>
    public class SwitchElement<T> : ControlFlowElementBase where T : notnull
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<SwitchElement<T>>? Ref
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

        private ObjectRef<SwitchElement<T>>? _ref;

        /// <summary>
        /// The value that is checked when set (evaluating the cases). If it's an <see cref="ObservableProperty{T}"/>,
        /// any change to it will create a reevaluation of the cases.
        /// </summary>
        public T Value
        {
            get => _value;
            set => ValueProperty.Value = value;
        }

        private T _value;
        public ObservableProperty<T> ValueProperty { get; } = new();

        private void SetValue(T? value)
        {
            //Value should not be null, so ignore null values
            if (value == null)
            {
                return;
            }

            _value = value;
            SetLocalValue(nameof(Value), value);
            Reevaluate();
        }

        /// <summary>
        /// Set the possible cases for this switch. Use objects like <see cref="ExactCaseLabel"/> and
        /// <see cref="EvaluationCaseLabel"/>. The cases are evaluated in order. After any modification, call
        /// <see cref="Reevaluate"/> to see any changes, as by default nothing is changed when these cases are modified.
        /// </summary>
        public List<SwitchLabel> CaseLabels { get; } = [];

        /// <summary>
        /// Set the Element that will be attached to the document when no case from <see cref="CaseLabels"/> is matched.
        /// After any modification, call <see cref="Reevaluate"/> to see any changes, as by default nothing is changed
        /// when this element is modified.
        /// </summary>
        public Element? DefaultElement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">
        /// The initial value that is used for matching. It sets <see cref="Value"/> to the given value.
        /// </param>
        /// <param name="caseLabels">
        /// Sets <see cref="CaseLabels"/>. Any later modification is not visible until you call <see cref="Reevaluate"/>.
        /// </param>
        /// <param name="defaultElement">
        /// Sets <see cref="DefaultElement"/>. Any later modification is not visible until you call <see cref="Reevaluate"/>.
        /// </param>
        public SwitchElement(T value, List<SwitchLabel>? caseLabels = null, Element? defaultElement = null)
        {
            ValueProperty.ValueChangedEvent += SetValue;

            if (caseLabels != null)
            {
                CaseLabels = caseLabels;
            }

            DefaultElement = defaultElement;
            Value = value;
            //silence compiler
            _value = value;

            //force reevaluation when the value is the default one and no evaluation is made
            Reevaluate();
        }

        /// <summary>
        /// Reevaluates the set cases. Starts from the first one and goes until a case from <see cref="CaseLabels"/>
        /// is matched. In that case, the element will be attached to the document. If no case is matched,
        /// <see cref="DefaultElement"/> is attached if set, otherwise there won't be any element attached.
        /// </summary>
        public void Reevaluate()
        {
            foreach (SwitchLabel label in CaseLabels)
            {
                label.SetElValue(Value);
                if (label.IsMatching())
                {
                    if (Children.Count > 0)
                    {
                        Children.RemoveAt(0);
                    }

                    Children.Add(label.MatchElementFunction.Invoke(Value));
                    return;
                }
            }

            if (DefaultElement != null)
            {
                if (Children.Count > 0)
                {
                    Children.RemoveAt(0);
                }

                Children.Add(DefaultElement);
                return;
            }

            Children.Clear();
        }

        public override SwitchElement<T> Duplicate()
        {
            SwitchElement<T> el = new(Value)
            {
                //
                State = State,
                Position = Position,
                Background = Background.Duplicate(),
                ClipPath = (ClipShape?)ClipPath?.Duplicate(),
                ClipType = ClipType,
                LocallyVisible = LocallyVisible,
                LocallyEnabled = LocallyEnabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate(),
                Layout = Layout
            };

            DuplicateChildrenUtil(el);
            return el;
        }


        #region Switch labels

        public abstract class SwitchLabel
        {
            protected T? ElValue { get; set; }
            internal Func<T?, Element> MatchElementFunction { get; set; }

            public SwitchLabel(Func<T?, Element> elFunction)
            {
                MatchElementFunction = elFunction;
            }

            protected internal abstract bool IsMatching();

            internal void SetElValue(T value)
            {
                ElValue = value;
            }
        }

        /// <summary>
        /// This checks the given value with the one from the switch using <see cref="object.Equals(object?)"/>.
        /// </summary>
        /// <remarks>
        /// For primitive data types (numbers), this might create unexpected results (e.g. int != uint, int != float),
        /// so in that case it's better to use <see cref="EvaluationCaseLabel"/> with the actual comparison operators.
        /// </remarks>
        public class ExactCaseLabel : SwitchLabel
        {
            private readonly T? _valueToCheck;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="valueToCheck">The value that will be checked against the value from the switch.</param>
            /// <param name="elementFunction">
            /// The function to return the element that will be attached to the document if this case is matched.
            /// The argument is the actual value matched by the switch.
            /// </param>
            public ExactCaseLabel(T valueToCheck, Func<T?, Element> elementFunction) : base(elementFunction)
            {
                _valueToCheck = valueToCheck;
            }

            protected internal override bool IsMatching()
            {
                if (_valueToCheck == null && ElValue == null)
                {
                    return true;
                }

                return _valueToCheck?.Equals(ElValue) ?? false;
            }
        }

        /// <summary>
        /// This checks the given value with the one from the switch using a given function that returns a true for a
        /// match, false otherwise. This is useful for creating more advanced cases (like using number comparison
        /// operators, pattern matching etc.) and for numerical equality checks (as <see cref="ExactCaseLabel"/> will
        /// create false for equal numbers, but of different type).
        /// </summary>
        public class EvaluationCaseLabel : SwitchLabel
        {
            private readonly Func<T?, bool> _valueChecker;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="valueChecker">
            /// The function that will be executed when the switch elementFunction checks for case matches. This must return
            /// true if the case is matched, false otherwise.
            /// </param>
            /// <param name="elementFunction">
            /// The function to return the element that will be attached to the document if this case is matched.
            /// The argument is the actual value matched by the switch.
            /// </param>
            public EvaluationCaseLabel(Func<T?, bool> valueChecker, Func<T?, Element> elementFunction)
                : base(elementFunction)
            {
                _valueChecker = valueChecker;
            }

            protected internal override bool IsMatching()
            {
                return _valueChecker.Invoke(ElValue);
            }
        }

        #endregion
    }
}
