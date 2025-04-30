using System.Collections.Generic;
using System.ComponentModel;

namespace CatUI.Data
{
    /// <summary>
    /// An object that holds a value and notifies all subscribers of <see cref="ValueChangedEvent"/> when that value
    /// changes. It is also capable of binding two ObservableProperty objects so their values are synchronized.
    /// On destruction, this object will automatically remove all the listeners of <see cref="ValueChangedEvent"/>.
    /// </summary>
    /// <typeparam name="T">The type of the contained object.</typeparam>
    /// <remarks>
    /// When the object T implements INotifyPropertyChanged, it will also trigger changes whenever the object invokes
    /// the property changed event.
    /// </remarks>
    public class ObservableProperty<T> where T : notnull
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
                if (EqualityComparer<T>.Default.Equals(_value, value))
                {
                    return;
                }

                if (_value is INotifyPropertyChanged oldReactiveObject)
                {
                    oldReactiveObject.PropertyChanged -= OnPropertyChanged;
                }

                _value = value;

                if (value is INotifyPropertyChanged newReactiveObject)
                {
                    newReactiveObject.PropertyChanged += OnPropertyChanged;
                }

                ValueChangedEvent?.Invoke(value);
            }
        }

        private T? _value;

        public ObservableProperty() { }

        public ObservableProperty(T? value)
        {
            Value = value;
        }

        ~ObservableProperty()
        {
            ValueChangedEvent = null;
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

        /// <summary>
        /// Will set the newValue of the property without invoking <see cref="ValueChangedEvent"/>.
        /// </summary>
        /// <param name="newValue">The new value that the property will have.</param>
        public void SetValueNoNotify(T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(_value, newValue))
            {
                return;
            }

            _value = newValue;
        }

        /// <summary>
        /// Connects the given property to this one such as when that property changes, this one will take its value
        /// and use it. The opposite (when this property changes) will not work (i.e. the other property won't be notified).
        /// So it makes the two properties connected, but only in one way.
        /// </summary>
        /// <param name="otherProperty">The property to listen to.</param>
        public void BindUnidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent += OnChangeCall;
        }

        /// <summary>
        /// Disconnects the given property. It's basically the opposite of <see cref="BindUnidirectional"/>.
        /// </summary>
        /// <param name="otherProperty">The property to disconnect from.</param>
        public void UnbindUnidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent -= OnChangeCall;
        }

        /// <summary>
        /// Connects the given property to this one and viceversa, so when either of the properties change its
        /// value, the other one will be notified and will change its value as well. It makes the two properties
        /// completely connected.
        /// </summary>
        /// <param name="otherProperty">The property to listen to and from.</param>
        public void BindBidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent += OnChangeCall;
            ValueChangedEvent += otherProperty.OnChangeCall;
        }

        /// <summary>
        /// Disconnects the given property. It's basically the opposite of <see cref="BindBidirectional"/>.
        /// </summary>
        /// <param name="otherProperty">The property to disconnect.</param>
        public void UnbindBidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent -= OnChangeCall;
            ValueChangedEvent -= otherProperty.OnChangeCall;
        }

        private void OnChangeCall(T? newValue)
        {
            Value = newValue;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs? e)
        {
            ValueChangedEvent?.Invoke(_value);
        }
    }

    public delegate void ValueChangedEventArgs<T>(T? newValue);
}
