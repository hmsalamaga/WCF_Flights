using Host.Objects;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Host.Services
{
    [ServiceContract]
    public interface IFlightSchedule
    {
        [OperationContract]
        [FaultContract(typeof(FaultException))]
        List<Flight> GetFlightSchedule(string portA, string portB, TimeSpan? timeA, TimeSpan? timeB);

        [OperationContract]
        bool TestConnection();
    }
}
