﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace TheAirline.Model.GeneralModel
{
    //the class for setting the inflation (prices etc.) for a specific year
    [Serializable]
    public class Inflation : ISerializable
    {
        [Versioning("year")]
        public int Year { get; set; }
        [Versioning("fuel")]
        public double FuelPrice { get; set; }
        [Versioning("percent")]
        public double InflationPercent { get; set; }
        [Versioning("modifier")]
        public double Modifier { get; set; }
        public Inflation(int year, double fuelprice, double inflationpercent, double modifier)
        {
            this.Year = year;
            this.FuelPrice = fuelprice;
            this.InflationPercent = inflationpercent;
            this.Modifier = modifier;
        }
           private Inflation(SerializationInfo info, StreamingContext ctxt)
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
    //the list of inflation years
    public class Inflations
    {
        public static int BaseYear = 1960;
        private static List<Inflation> inflations = new List<Inflation>();
        //adds an inflation year to the list
        public static void AddInflationYear(Inflation inflation)
        {
            inflations.Add(inflation);
                
        }
        //returns the inflation for an year
        public static Inflation GetInflation(int year)
        {
            Inflation inflation = inflations.Find(i => i.Year == year);

            if (inflation == null)
            {
                Random rnd = new Random();

                double rndInflation = (((rnd.NextDouble() * 5)-1) / 100.0);
                double inflationPercent = 1 + rndInflation;

                Inflation prevInflation = inflations.Find(i=>i.Year == year-1);

                Inflation newInflation = new Inflation(year, prevInflation.FuelPrice * inflationPercent, rndInflation, prevInflation.Modifier * inflationPercent);
                Inflations.AddInflationYear(newInflation);

                return newInflation;

            }
            else
                return inflation;
        }
        //clears the list of inflations
        public static void Clear()
        {
            inflations.Clear();
        }
    }
}
