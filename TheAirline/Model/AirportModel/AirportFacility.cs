﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using TheAirline.Model.GeneralModel;

namespace TheAirline.Model.AirportModel
{
    [Serializable]
    //the class for a facility at an airport
    public class AirportFacility : ISerializable
    {
        public static string Section { get; set; }
        [Versioning("uid")]
        public string Uid { get; set; }
        public enum FacilityType { Lounge, Service, CheckIn, SelfCheck, TicketOffice, Cargo }
        [Versioning("type")]
        public FacilityType Type { get; set; }
        [Versioning("shortname")]
        public string Shortname { get; set; }
        [Versioning("price")]
        private double APrice;
        public double Price { get { return GeneralHelpers.GetInflationPrice(this.APrice); } set { this.APrice = value; } }
        [Versioning("typelevel")]
        public int TypeLevel { get; set; }
        [Versioning("luxurylevel")]
        public int LuxuryLevel { get; set; } //for business customers
        [Versioning("servicelevel")]
        public int ServiceLevel { get; set; } //for repairing airliners 
        [Versioning("buildingdays")]
        public int BuildingDays { get; set; }
        [Versioning("employees")]
        public int NumberOfEmployees { get; set; }
        public enum EmployeeTypes { Maintenance, Support }
        [Versioning("employeetype")]
        public EmployeeTypes EmployeeType { get; set; }
        public AirportFacility(string section, string uid, string shortname,FacilityType type,int buildingDays, int typeLevel, double price, int serviceLevel, int luxuryLevel)
        {
            AirportFacility.Section = section;
            this.Uid = uid;
            this.Shortname = shortname;
            this.Price = price;
            this.LuxuryLevel = luxuryLevel;
            this.ServiceLevel = serviceLevel;
            this.BuildingDays = buildingDays;
            this.TypeLevel = typeLevel;
            this.Type = type;
        }

        public string Name
        {
            get { return Translator.GetInstance().GetString(AirportFacility.Section, this.Uid); }
        }
           private AirportFacility(SerializationInfo info, StreamingContext ctxt)
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
    //the collection of facilities
    public class AirportFacilities
    {
        private static Dictionary<string, AirportFacility> facilities = new Dictionary<string, AirportFacility>();
        //clears the list
        public static void Clear()
        {
            facilities = new Dictionary<string, AirportFacility>();
        }
        //adds a new facility to the collection
        public static void AddFacility(AirportFacility facility)
        {
            facilities.Add(facility.Shortname, facility);
        }
        //returns a facility
        public static AirportFacility GetFacility(string shortname)
        {
            return facilities[shortname];
        }
        //returns the list of facilities
        public static List<AirportFacility> GetFacilities()
        {
            return facilities.Values.ToList();
        }
       //returns all facilities of a specific type
        public static List<AirportFacility> GetFacilities(AirportFacility.FacilityType type)
        {
            return GetFacilities().FindAll((delegate(AirportFacility facility) { return facility.Type ==type; }));
        }
    }
   
}
