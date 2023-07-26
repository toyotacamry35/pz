using System;

namespace Core.Environment.Logging.Extension
{
    public readonly struct MessageBuilder
    {
        private readonly MessageContext _ctx;

        public MessageBuilder(MessageContext mc) => _ctx = mc; 
        
        public static MessageBuilder operator +(in MessageBuilder mb, ValueTuple<object, object> property)
        {
            mb._ctx.Property(property.Item1, property.Item2);
            return mb;
        }

        public static MessageBuilder operator +(in MessageBuilder mb, Exception exception)
        {
            mb._ctx.Exception(exception);
            return mb;
        }

        public static MessageBuilder operator +(in MessageBuilder mb, Guid eneityId)
        {
            mb._ctx.Property(EntityIdLayoutRenderer.EntityIdKey, eneityId);
            return mb;
        }
    }
}