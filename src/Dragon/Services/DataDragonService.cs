using RiotPls.DataDragon;

namespace Dragon.Services
{
    public class DataDragonService
    {
        public DataDragonClient Client { get; }

        public DataDragonService(DataDragonClient client)
        {
            Client = client;
        }
    }
}