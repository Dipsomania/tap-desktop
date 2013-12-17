﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheAirline.Model.GeneralModel;
using TheAirline.Model.AirportModel;
using System.Runtime.Serialization;
using System.Reflection;



namespace TheAirline.Model.AirlineModel
{
    [Serializable] 
    //the profile for an airline
    public class AirlineProfile : ISerializable
    {
        [Versioning("name")]
        public string Name { get; set; }
        [Versioning("ceo")]
        public string CEO { get; set; }
        [Versioning("iata")]
        public string IATACode { get; set; }
        [Versioning("country")]
        public Country Country { get; set; }
        [Versioning("countries")]
        public List<Country> Countries { get; set; }
        [Versioning("color")]
        public string Color { get; set; }
        public string Logo { get { return getCurrentLogo();} private set{;} }
        [Versioning("logos")]
        public List<AirlineLogo> Logos { get; set; }
        [Versioning("preferedairport")]
        public Airport PreferedAirport { get; set; }
        [Versioning("founded")]
        public int Founded { get; set; }
        [Versioning("folded")]
        public int Folded { get; set; }
        [Versioning("isreal")]
        public Boolean IsReal { get; set; }
        [Versioning("narrative")]
        public string Narrative { get; set; }
        public AirlineProfile(string name, string iata, string color,  string ceo, Boolean isReal, int founded, int folded)
        {
            this.Name = name;
            this.IATACode = iata;
            this.CEO = ceo;
            this.Color = color;
            this.IsReal = isReal;
            this.Founded = founded;
            this.Folded = folded;
            this.Countries = new List<Country>();
            this.Logos = new List<AirlineLogo>();
        }
        //adds a logo to the airline
        public void addLogo(AirlineLogo logo)
        {
            this.Logos.Add(logo);
        }
        //returns the current logo for the airline
        private string getCurrentLogo()
        {
            return this.Logos.Find(l => l.FromYear <= GameObject.GetInstance().GameTime.Year && l.ToYear >= GameObject.GetInstance().GameTime.Year).Path;

        }
        private AirlineProfile(SerializationInfo info, StreamingContext ctxt)
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
    //the class for an airline logo
    [Serializable]
    public class AirlineLogo : ISerializable
    {
        [Versioning("fromyear")]
        public int FromYear { get; set; }
        [Versioning("toyear")]
        public int ToYear { get; set; }
        [Versioning("path")]
        public string Path { get; set; }
        public AirlineLogo(int fromYear, int toYear, string path)
        {
            this.FromYear = fromYear;
            this.ToYear = toYear;
            this.Path = path;
        }
        public AirlineLogo(string path) : this(1900,2199,path)
        {

        }
        private AirlineLogo(SerializationInfo info, StreamingContext ctxt)
        {
            int version = info.GetInt16("version");

            IList<PropertyInfo> props = new List<PropertyInfo>(this.GetType().GetProperties().Where(p => p.GetCustomAttribute(typeof(Versioning)) != null && ((Versioning)p.GetCustomAttribute(typeof(Versioning))).AutoGenerated));

            foreach (SerializationEntry entry in info)
            {
                PropertyInfo prop = props.FirstOrDefault(p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Name == entry.Name);


                if (prop != null)
                    prop.SetValue(this, entry.Value);
            }

            var notSetProps = props.Where(p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Version > version);

            foreach (PropertyInfo prop in notSetProps)
            {
                Versioning ver = (Versioning)prop.GetCustomAttribute(typeof(Versioning));

                if (ver.AutoGenerated)
                    prop.SetValue(this, ver.DefaultValue);

            }




        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("version", 1);

            Type myType = this.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties().Where(p => p.GetCustomAttribute(typeof(Versioning)) != null));

            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(this, null);

                Versioning att = (Versioning)prop.GetCustomAttribute(typeof(Versioning));

                info.AddValue(att.Name, propValue);
            }

        }
    }
}
