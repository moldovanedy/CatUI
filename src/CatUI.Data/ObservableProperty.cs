using System.Collections.Generic;

namespace CatUI.Data
{
    public class ObservableProperty<T> where T : notnull
    {
        /// <summary>
        /// Represents the actual value of the property. Setting this to a different value than the previous one will notify all
        /// the properties bound to this one, as well as invoking <see cref="ValueChangedEvent"/>.
        /// </summary>
        public T? Value
        {
            get
            {
                return _value;
            }
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

        /// <summary>
        /// An event invoked when <see cref="Value"/> is changed. In other words, whenever the property's value is changed.
        /// </summary>
        public event ValueChangedEventArgs<T>? ValueChangedEvent;

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