using System;
using System.ServiceModel;

namespace Common
{
    /// <summary>
    /// A Simple Service that takes a serialized Event to handle.
    /// </summary>
    [ServiceContract]
    public interface IEventService
    {
        [OperationContract]
        void Handle(Byte[] serializedEvent);
    }
}
