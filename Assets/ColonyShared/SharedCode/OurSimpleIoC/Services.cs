namespace SharedCode.OurSimpleIoC
{
    public static class ServicesPool
    {
        private static IServices _services;
        public static IServices Services
        {
            get
            {
                if (_services == null)
                    _services = new ServicesProvider();

                return _services;
            }
        }

        public static void Clear()
        {
            _services = null;
        }
    }
}
