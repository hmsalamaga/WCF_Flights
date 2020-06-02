using System;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Client
{
    class Program
    {
        static ServiceReference.FlightScheduleClient proxy;
        static ServiceReference.Flight[] msg;

        static void Main(string[] args)
        {
            try
            {
                proxy = new ServiceReference.FlightScheduleClient();
                proxy.TestConnection();
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                Console.WriteLine("Server unavailable.");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                int err = 0;
                Console.WriteLine("Write name of first station:");
                var portA = Console.ReadLine();
                Console.WriteLine("Write name of second station:");
                var portB = Console.ReadLine();
                if (int.TryParse(portA, out int _result) || int.TryParse(portB, out int _result2))
                {
                    if (_result != 0)
                    {
                        Console.WriteLine("Wrong name of city: {0}", portA);
                        err++;
                    }
                    else
                    {
                        Console.WriteLine("Wrong name of city: {0}", portB);
                        err++;
                    }
                }

                Console.WriteLine("Write the low boundary (use hh:mm format) or leave empty:");
                var timeA = Console.ReadLine();

                TimeSpan? _timeA = null;
                TimeSpan? _timeB = null;

                TimeSpan tmp = new TimeSpan();
                if (timeA != string.Empty && !TimeSpan.TryParse(timeA, out tmp))
                {
                    Console.WriteLine("Wrong form of time (use hh:mm))");
                    err++;
                }

                if (timeA != string.Empty)
                {
                    _timeA = tmp;
                    Console.WriteLine("Write the high boundary (use hh:mm format) or leave empty:");
                    var timeB = Console.ReadLine();

                    if (timeB != string.Empty && !TimeSpan.TryParse(timeB, out tmp))
                    {
                        Console.WriteLine("Wrong form of time (use hh:mm)");
                        err++;
                    }

                    if (timeB != string.Empty)
                    {
                        _timeB = tmp;

                        if (_timeA > _timeB)
                        {
                            Console.WriteLine("Lower boundary is higher than high boundary");
                            err++;
                        }
                    }
                }

                if (err > 0)
                {
                    continue;
                }

                try
                {
                    msg = proxy.GetFlightSchedule(portA, portB, _timeA, _timeB);
                }
                catch (FaultException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (EndpointNotFoundException)
                {
                    Console.WriteLine("The connection to the server has been interrupted.");
                    Console.ReadKey();
                    return;
                }

                if (!msg.Any())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("No connections between {0} and {1}", portA, portB);

                    if (_timeA.HasValue && _timeB.HasValue)
                    {
                        sb.AppendFormat(" between hours {0} and {1}", _timeA, _timeB);
                    }

                    Console.WriteLine(sb);
                    continue;
                }

                foreach (ServiceReference.Flight flight in msg)
                {
                    if (flight.PortB == portB)
                    {
                        Console.WriteLine($"{flight.PortA} {flight.TimeA} {flight.PortB} {flight.TimeB}");
                    }
                    else
                    {
                        Console.Write($"{flight.PortA} {flight.TimeA} {flight.PortB} {flight.TimeB} -> ");
                    }
                }
                Console.WriteLine(Environment.NewLine);
            }
        }
    }
}