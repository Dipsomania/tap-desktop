﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAirline.Model.AirportModel;
using TheAirline.Model.AirlinerModel;
using TheAirline.Model.AirlineModel;
using TheAirline.Model.AirlinerModel.RouteModel;
using TheAirline.Model.GeneralModel;
using TheAirline.Model.GeneralModel.StatisticsModel;
using TheAirline.Model.GeneralModel.InvoicesModel;
using TheAirline.Model.AirlineModel.SubsidiaryModel;
using TheAirline.Model.PilotModel;
using System.Reflection;
using System.Runtime.Serialization;


namespace TheAirline.Model.GeneralModel
{
    [Serializable]
    public class RandomEvent : ISerializable
    {
        public enum EventType { Safety, Security, Maintenance, Customer, Employee, Political }
        public enum Focus { Aircraft, Airport, Airline }
        [Versioning("type")]
        public EventType Type { get; set; }
        [Versioning("focus")]
        public Focus focus { get; set; }
        [Versioning("airline")]
        public Airline Airline { get; set; }
        [Versioning("name")]
        public string EventName { get; set; }
        [Versioning("message")]
        public string EventMessage { get; set; }
        [Versioning("airliner")]
        public FleetAirliner Airliner { get; set; }
        [Versioning("airport")]
        public Airport Airport { get; set; }
        [Versioning("country")]
        public Country Country { get; set; }
        [Versioning("route")]
        public Route Route { get; set; }
        
        public bool CriticalEvent { get; set; }
        
        public DateTime DateOccurred { get; set; }
        public int CustomerHappinessEffect { get; set; } //0-100
        public int AircraftDamageEffect { get; set; } //0-100
        public int AirlineSecurityEffect { get; set; } //0-100
        public int AirlineSafetyEffect { get; set; } //0-100
        public int EmployeeHappinessEffect { get; set; } //0-100
        public int FinancialPenalty { get; set; } //dollar amount to be added or subtracted from airline cash
        public double PaxDemandEffect { get; set; } //0-2
        public double CargoDemandEffect { get; set; } //0-2
        public int EffectLength { get; set; } //should be defined in months
        public string EventID { get; set; }
        public int Frequency { get; set; } //frequency per 3 years
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public RandomEvent(EventType type, Focus focus, string name, string message, bool critical, int custHappiness, int aircraftDamage, int airlineSecurity, int airlineSafety, int empHappiness, int moneyEffect, double paxDemand, double cargoDemand, int length, string id, int frequency, DateTime stat, DateTime end)
        {

            this.DateOccurred = GameObject.GetInstance().GameTime;
            this.CustomerHappinessEffect = 0;
            this.AircraftDamageEffect = 0;
            this.AirlineSecurityEffect = 0;
            this.EmployeeHappinessEffect = 0;
            this.FinancialPenalty = 0;
            this.PaxDemandEffect = 1;
            this.CargoDemandEffect = 1;
            this.EffectLength = 1;
            this.CriticalEvent = false;
            this.EventName = "";
            this.EventMessage = "";
            this.Type = type;

         
            this.EventID = id;
        }

        //applies the effects of an event
        public void ExecuteEvents(Airline airline, DateTime time) 
        {
            Random rnd = new Random();
            foreach (RandomEvent rEvent in airline.EventLog)
            {
                if (rEvent.DateOccurred.DayOfYear == time.DayOfYear)
                {
                    rEvent.Airliner.Airliner.Condition += AircraftDamageEffect;
                    airline.Money += rEvent.FinancialPenalty;
                    airline.Scores.CHR.Add(rEvent.CustomerHappinessEffect);
                    airline.Scores.EHR.Add(rEvent.EmployeeHappinessEffect);
                    airline.Scores.Safety.Add(rEvent.AirlineSafetyEffect);
                    airline.Scores.Security.Add(rEvent.AirlineSecurityEffect);
                    PassengerHelpers.ChangePaxDemand(airline, (rEvent.PaxDemandEffect * rnd.Next(9, 11) / 10));
                }
            }
        }

       

        /*public RandomEvent GenerateRandomEvent()
        {
            //code needed

        }*/

        //adds an event to an airline's event log
        public void AddEvent(Airline airline, RandomEvent rEvent)
        {
            airline.EventLog.Add(rEvent);
        }


        //removes an event from the airlines event log
        public static void RemoveEvent(Airline airline, RandomEvent rEvent)
        {
            airline.EventLog.Remove(rEvent);
        }


        //checks if an event's effects are expired
        public static void CheckExpired(DateTime expDate)
        {
            foreach (Airline airline in Airlines.GetAllAirlines())
            {
                foreach (RandomEvent rEvent in airline.EventLog)
                {
                    expDate = GameObject.GetInstance().GameTime.AddMonths(rEvent.EffectLength);
                    if (expDate < GameObject.GetInstance().GameTime)
                    {
                        PassengerHelpers.ChangePaxDemand(airline, (1 / rEvent.PaxDemandEffect));
                        RemoveEvent(airline, rEvent);
                    }  }  }
        }
           private RandomEvent(SerializationInfo info, StreamingContext ctxt)
        {
            int version = info.GetInt16("version");

            var fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(p => p.GetCustomAttribute(typeof(Versioning)) != null);

            IList<PropertyInfo> props = new List<PropertyInfo>(this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(p => p.GetCustomAttribute(typeof(Versioning)) != null));

            var propsAndFields = props.Cast<MemberInfo>().Union(fields.Cast<MemberInfo>());

            foreach (SerializationEntry entry in info)
            {
                MemberInfo prop = propsAndFields.FirstOrDefault(p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Name == entry.Name);


                if (prop != null)
                {
                    if (prop is FieldInfo)
                        ((FieldInfo)prop).SetValue(this, entry.Value);
                    else
                        ((PropertyInfo)prop).SetValue(this, entry.Value);
                }
            }

            var notSetProps = propsAndFields.Where(p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Version > version);

            foreach (MemberInfo notSet in notSetProps)
            {
                Versioning ver = (Versioning)notSet.GetCustomAttribute(typeof(Versioning));

                if (ver.AutoGenerated)
                {
                    if (notSet is FieldInfo)
                        ((FieldInfo)notSet).SetValue(this, ver.DefaultValue);
                    else
                        ((PropertyInfo)notSet).SetValue(this, ver.DefaultValue);

                }

            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("version", 1);

            Type myType = this.GetType();

            var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(p => p.GetCustomAttribute(typeof(Versioning)) != null);

            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(p => p.GetCustomAttribute(typeof(Versioning)) != null));

            var propsAndFields = props.Cast<MemberInfo>().Union(fields.Cast<MemberInfo>());

            foreach (MemberInfo member in propsAndFields)
            {
                object propValue;

                if (member is FieldInfo)
                    propValue = ((FieldInfo)member).GetValue(this);
                else
                    propValue = ((PropertyInfo)member).GetValue(this, null);

                Versioning att = (Versioning)member.GetCustomAttribute(typeof(Versioning));

                info.AddValue(att.Name, propValue);
            }

        }
    }

    public class RandomEvents
    {
        private static Dictionary<string, RandomEvent> events = new Dictionary<string, RandomEvent>();

        public static void Clear()
        {
            events = new Dictionary<string, RandomEvent>();
        }

        public static void AddEvent(RandomEvent rEvent)
        {
            events.Add(rEvent.EventName, rEvent);
        }

        //gets a single event by name
        public static RandomEvent GetEvent(string name)
        {
            return events[name];
        }


        //gets a list of all events
        public static List<RandomEvent> GetEvents()
        {
            return events.Values.ToList();
        }

        //gets all events of a given type
        public static List<RandomEvent> GetEvents(RandomEvent.EventType type)
        {
            return GetEvents().FindAll((delegate(RandomEvent rEvent) {return rEvent.Type ==type; }));
        }

        //gets x number of random events of a given type
        public static List<RandomEvent> GetEvents(RandomEvent.EventType type, int number, Airline airline)
        {
            Random rnd = new Random();
            Dictionary<int,RandomEvent> rEvents = new Dictionary<int,RandomEvent>();
            List<RandomEvent> tEvents = GetEvents(type);
            int i = 1;
            int j = 0;
            foreach (RandomEvent r in tEvents)
                if (r.Start <= GameObject.GetInstance().GameTime && r.End >= GameObject.GetInstance().GameTime)
                {
                    {
                        r.DateOccurred = MathHelpers.GetRandomDate(GameObject.GetInstance().GameTime, GameObject.GetInstance().GameTime.AddMonths(12));
                        r.Airline = airline;
                        r.Airliner = Helpers.AirlinerHelpers.GetRandomAirliner(airline);
                        r.Route = r.Airliner.Routes[rnd.Next(r.Airliner.Routes.Count())];
                        r.Country = r.Route.Destination1.Profile.Country;
                        r.Airport = r.Route.Destination1;

                        if (r.focus == RandomEvent.Focus.Airline)
                        {
                            r.Airliner = null;
                            r.Airport = null;
                            r.Country = null;
                            r.Route = null;
                        }

                        rEvents.Add(i, r);
                        i++;
                    }
                }

            tEvents.Clear();

            while (j < number)
            {
                int item = rnd.Next(rEvents.Count());
                tEvents.Add(rEvents[item]);
                j++;
            }

            return tEvents;
        }

    }


}
