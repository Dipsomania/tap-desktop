﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TheAirline.Model.GeneralModel;

namespace TheAirline.Model.AirlineModel
{
    [Serializable]
    public class AirlineBudget : ISerializable
    {
        [Versioning("totalbudget")]
        public long TotalBudget { set; get; }
        [Versioning("budgetactive")]
        public DateTime BudgetActive { set; get; }
        [Versioning("budgetexpires")]
        public DateTime BudgetExpires { set; get; }
        [Versioning("marktingbudget")]
        public long MarketingBudget { set; get; }
        [Versioning("maintenancebudget")]
        public long MaintenanceBudget { set; get; }
        [Versioning("securitybudget")]
        public long SecurityBudget { set; get; }
        [Versioning("csbudget")]
        public long CSBudget { set; get; }
        [Versioning("printbudget")]
        public long PrintBudget { set; get; }
        [Versioning("tvbudget")]
        public long TelevisionBudget { set; get; }
        [Versioning("radiobudget")]
        public long RadioBudget { set; get; }
        [Versioning("internetbudget")]
        public long InternetBudget { set; get; }
        [Versioning("overhaulbudget")]
        public long OverhaulBudget { set; get; }
        [Versioning("patsbudget")]
        public long PartsBudget { set; get; }
        [Versioning("enginesbudget")]
        public long EnginesBudget { set; get; }
        [Versioning("remotebudget")]
        public long RemoteBudget { set; get; }
        [Versioning("inflightbudget")]
        public long InFlightBudget { set; get; }
        [Versioning("airportbudget")]
        public long AirportBudget { set; get; }
        [Versioning("equipmentbudget")]
        public long EquipmentBudget { set; get; }
        [Versioning("ITbudget")]
        public long ITBudget { set; get; }
        [Versioning("compbudget")]
        public long CompBudget { set; get; }
        [Versioning("promobudget")]
        public long PromoBudget { set; get; }
        [Versioning("scbudget")]
        public long ServCenterBudget { set; get; }
        [Versioning("prbudget")]
        public long PRBudget { set; get; }
        [Versioning("endyearcash")]
        public long EndYearCash { set; get; }
        [Versioning("fleetsize")]
        public int FleetSize { set; get; }
        [Versioning("fleetvalue")]
        public long FleetValue { set; get; }
        [Versioning("subsidiaries")]
        public int Subsidiaries { set; get; }
        [Versioning("totalsubvalue")]
        public long TotalSubValue { set; get; }
        [Versioning("totalemployees")]
        public int TotalEmployees { set; get; }
        [Versioning("totalpayroll")]
        public int TotalPayroll { set; get; }
        [Versioning("remainingbudget")]
        public long RemainingBudget { set; get; }
        [Versioning("cash")]
        public long Cash { set; get; }
        public AirlineBudget()
        {
        }
        private AirlineBudget(SerializationInfo info, StreamingContext ctxt)
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
