using System;
using System.Collections.Generic;

namespace CatUI.Data
{
    public class ObservableProperty<T> : CatObject where T : notnull
    {
        /// <summary>
        /// Represents the actual value of the property. Setting this to a different value than the previous one will notify all
        /// the properties bound to this one, as well as invoking <see cref="ValueChangedEvent"/>.
        /// </summary>
        public T? Value
        {
            get => _value;
            set
            {
                //if (_value == value)
                if (EqualityComparer<T>.Default.Equals(_value, value))
                {
                    return;
                }

                _value = value;
                ValueChangedEvent?.Invoke(value);
            }
        }

        private T? _value;

        public ObservableProperty() { }

        public ObservableProperty(T? value)
        {
            Value = value;
        }

        /// <summary>
        /// Not implemented because cannot guarantee that T is CatObject, therefore the duplication method for T is unknown.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override CatObject Duplicate()
        {
            throw new NotImplementedException("Duplicating ObservableProperty is not supported.");
        }

        /// <summary>
        /// An event invoked when <see cref="Value"/> is changed. In other words, whenever the property's value is changed.
        /// </summary>
        public event ValueChangedEventArgs<T>? ValueChangedEvent;

        /// <summary>
        /// Will call <see cref="ValueChangedEvent"/> directly with the current value. Useful if you want to invoke the
        /// event without setting <see cref="Value"/> to a different value, as setting it to the same value won't have any effect.
        /// </summary>
        public void ForceRecallEvents()
        {
            ValueChangedEvent?.Invoke(_value);
        }

        public void BindUnidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent += OnChangeCall;
        }

        public void UnbindUnidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent -= OnChangeCall;
        }

        public void BindBidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent += OnChangeCall;
            ValueChangedEvent += otherProperty.OnChangeCall;
        }

        public void UnbindBidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent -= OnChangeCall;
            ValueChangedEvent -= otherProperty.OnChangeCall;
        }

        private void OnChangeCall(T? newValue)
        {
            Value = newValue;
        }
    }

    public delegate void ValueChangedEventArgs<T>(T? newValue);
}
