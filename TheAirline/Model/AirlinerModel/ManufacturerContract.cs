﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using TheAirline.Model.AirlineModel;
using TheAirline.Model.GeneralModel;

namespace TheAirline.Model.AirlinerModel
{
    //the contract for an airline for a manufacturer
    [Serializable]
    public class ManufacturerContract : ISerializable
    {
        [Versioning("manufacturer")]
        public Manufacturer Manufacturer { get; set; }
        [Versioning("signingdate")]
        public DateTime SigningDate { get; set; }
        [Versioning("length")]
        public int Length { get; set; }
        [Versioning("expiredate")]
        public DateTime ExpireDate { get; set; }
        [Versioning("discount")]
        public double Discount { get; set; } //in percent
        [Versioning("airliners")]
        public int Airliners { get; set; } //the number of airliners to purchase in that period
        [Versioning("purchasedairliners")]
        public int PurchasedAirliners { get; set; }
        public ManufacturerContract(Manufacturer manufacturer, DateTime date, int length, double discount)
        {
            this.Manufacturer = manufacturer;
            this.Airliners = length;
            this.SigningDate = date;
            this.Length = length;
            this.Discount = discount;
            this.ExpireDate = date.AddYears(this.Length);
            this.PurchasedAirliners = 0;
        }
        //returns the termination fee for the contract
        public double getTerminationFee()
        {
            return GeneralHelpers.GetInflationPrice(this.Length * 1000000);
        }
        //the discount for airliners ordered under a contract
        public double getDiscount(int airliners)
        {
            airliners = PurchasedAirliners;
            if (Length <= 3)
            { this.Discount = (this.PurchasedAirliners / 2) + 1; }
            else if (Length <= 5)
            { this.Discount = (this.PurchasedAirliners / 2) + 2; }
            else if (Length <= 7)
            { this.Discount = (this.PurchasedAirliners / 2) + 4; }
            else if (Length <= 15)
            { this.Discount = (this.PurchasedAirliners / 2) + 7; }
            else
                this.Discount = 1;

            return Discount;

           }   
           private ManufacturerContract(SerializationInfo info, StreamingContext ctxt)
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
}
