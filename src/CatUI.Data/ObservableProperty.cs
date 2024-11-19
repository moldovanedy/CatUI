namespace CatUI.Data
{
    public class ObservableProperty<T> where T : new()
    {
        public T? Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                ValueChangedEvent?.Invoke(value);
            }
        }
        private T? _value;

        public event ValueChangedEventArgs<T>? ValueChangedEvent;

        public void BindUnidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent += ChangeCall;
        }

        public void UnbindUnidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent -= ChangeCall;
        }

        public void BindBidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent += ChangeCall;
            ValueChangedEvent += otherProperty.ChangeCall;
        }

        public void UnbindBidirectional(ObservableProperty<T> otherProperty)
        {
            otherProperty.ValueChangedEvent -= ChangeCall;
            ValueChangedEvent -= otherProperty.ChangeCall;
        }

        private void ChangeCall(T? newValue)
        {
            Value = newValue;
        }
    }

    public delegate void ValueChangedEventArgs<T>(T? newValue);
}