namespace SharedCode.Network
{
    public enum MessageSendOptions
    {
        Unreliable = 0,
        ReliableUnordered = 1,
        Sequenced = 2,
        ReliableOrdered = 3,
        ReliableSequenced = 4
    }
}
