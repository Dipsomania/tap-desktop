﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheAirline.Model.GeneralModel.StatisticsModel
{
    //the class for general statistics
    public class GeneralStatistics
    {
        private Dictionary<int, List<StatisticsValue>> StatValues;
        public GeneralStatistics()
        {
            this.StatValues = new Dictionary<int, List<StatisticsValue>>();

        }
        //returns the value for a statistics type for a year
        public int getStatisticsValue(int year, StatisticsType type)
        {
            if (this.StatValues.ContainsKey(year))
            {
                StatisticsValue value = this.StatValues[year].Find(sv => sv.Stat == type);
                if (value != null) return value.Value;
            }
            return 0;

        }
        //adds the value for a statistics type for a year
        public void addStatisticsValue(int year, StatisticsType type, int value)
        {
            if (!this.StatValues.ContainsKey(year))
                this.StatValues.Add(year, new List<StatisticsValue>());
            StatisticsValue statValue = this.StatValues[year].Find(sv => sv.Stat == type);
            if (statValue != null)
                statValue.Value += value;
            else
                this.StatValues[year].Add(new StatisticsValue(type,value));
        }
        //sets the value for a statistics type for a year
        public void setStatisticsValue(int year, StatisticsType type, int value)
        {
            if (!this.StatValues.ContainsKey(year))
                this.StatValues.Add(year, new List<StatisticsValue>());
            StatisticsValue statValue = this.StatValues[year].Find(sv => sv.Stat == type);
            if (statValue != null)
                statValue.Value = value;
            else
                this.StatValues[year].Add(new StatisticsValue(type, value));

        }
       

    }
}
