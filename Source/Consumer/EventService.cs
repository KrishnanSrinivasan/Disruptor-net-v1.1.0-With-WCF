using System.ServiceModel;
using Common;

namespace Consumer
{
    /// <summary>
    /// A Service that reads byte data from a underlying transport layer and passes it
    /// to the <see cref="EventSequencer"/> to be processed in a Diamond Path configuration.
    /// </summary>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.PerSession,
        AddressFilterMode = AddressFilterMode.Any)]
    internal sealed class EventService : IEventService
    {
        void IEventService.Handle(byte[] eventDTO)
        {
            EventSequencer.Publish(eventDTO);
        }
    }
}
