using System.Threading.Tasks;
using TransportLayer;

namespace ServerBuilder
{
    public interface IEventFactory
    {
        Task Construct(BaseModel baseObj, TNetPeer peer);
    }
}
