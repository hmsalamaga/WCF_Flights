using Host.Objects;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace Host.Services
{
    public class FlightSchedule : IFlightSchedule
    {
        public static List<Flight> flights;

        public FlightSchedule()
        {
            using (var reader = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}/Resources/flights.csv"))
            {
                using (var csvReader = new CsvReader(reader))
                {
                    csvReader.Configuration.Delimiter = ",";

                    flights = csvReader.GetRecords<Flight>().ToList();
                }
            }
        }

        public List<Flight> GetFlightSchedule(string portA, string portB, TimeSpan? timeA, TimeSpan? timeB)
        {
            if (!flights.Any(flight => flight.PortA == portA) && !flights.Any(flight => flight.PortA == portB))
            {
                throw new FaultException("The city " + portA + " does not exist.");
            }

            if (!flights.Any(flight => flight.PortB == portB) && !flights.Any(flight => flight.PortA == portB))
            {
                throw new FaultException("The city " + portB + " does not exist.");
            }

            List<Flight> found_flights = new List<Flight>();

            // find direct flies
            foreach (Flight flight in flights)
            {
                if (flight.PortA == portA && flight.PortB == portB)
                {
                    // if no time interval provided
                    if (!timeA.HasValue || !timeB.HasValue)
                    {
                        found_flights.Add(flight);
                        continue;
                    }

                    // with time interval provided
                    if (timeA <= flight.TimeA && flight.TimeB <= timeB)
                    {
                        found_flights.Add(flight);
                    }
                }
            }

            // find flight with transfer
            foreach (Flight flight in flights)
            {
                //if time interval provided
                if (timeA.HasValue && timeB.HasValue && flight.TimeA < timeA)
                {
                    continue;
                }

                if (flight.PortA == portA)
                {
                    foreach (Flight midFlight in flights)
                    {
                        // if no time interval provided
                        if ((!timeA.HasValue || !timeB.HasValue) &&
                            midFlight.PortA == flight.PortB && midFlight.PortB == portB)
                        {
                            found_flights.Add(flight);
                            found_flights.Add(midFlight);
                            continue;
                        }

                        // with time interval provided
                        if (midFlight.PortA == flight.PortB && midFlight.TimeA >= flight.TimeB &&
                            midFlight.TimeB <= timeB && midFlight.PortB == portB)
                        {
                            found_flights.Add(flight);
                            found_flights.Add(midFlight);
                        }
                    }
                }
            }

            return found_flights;
        }

        public bool TestConnection()
        {
            return true;
        }
    }
}

