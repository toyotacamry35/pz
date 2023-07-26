namespace SharedCode.EntitySystem
{
    public class EntityEventArgs
    {
        public EntityEventArgs(object newValue, string propertyName, PropertyAddress propertyAddress, IDeltaObject sender)
        {
            NewValue = newValue;
            PropertyName = propertyName;
            PropertyAddress = propertyAddress;
            Sender = sender;
        }

        public object NewValue { get; }

        public string PropertyName { get; }

        public PropertyAddress PropertyAddress { get; }

        public IDeltaObject Sender { get; }
    }
}
