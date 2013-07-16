/* **************************************************************
* File Name     :- ExtensionUtility.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 10th Jan 2013
* Modified Date :- 10th Jan 2013
* Description   :- This file contains return the hard coded value
****************************************************************/

#region Namespace
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Extension
{
    public static class ExtensionUtility
    {
        /// <summary>
        /// This function will get all months
        /// </summary>
        /// <returns></returns>
        public static List<Month> GetMonths()
        {
            return new List<Month>()
            {
                new Month(){ ID = 1, Name = "January"},
                new Month(){ ID = 2, Name = "February"},
                new Month(){ ID = 3, Name = "March"},
                new Month(){ ID = 4, Name = "April"},
                new Month(){ ID = 5, Name = "May"},
                new Month(){ ID = 6, Name = "June"},
                new Month(){ ID = 7, Name = "July"},
                new Month(){ ID = 8, Name = "August"},
                new Month(){ ID = 9, Name = "September"},
                new Month(){ ID = 10, Name = "October"},
                new Month(){ ID = 11, Name = "November"},
                new Month(){ ID = 12, Name = "December"}
            };
        }

        /// <summary>
        /// This function will get all days
        /// </summary>
        /// <returns></returns>
        public static List<Day> GetDays()
        {
            var dayList = new List<Day>();

            for (int dayCount = 1; dayCount <= 31; dayCount++)
            {
                var day = new Day() { ID = dayCount, Name = dayCount.ToString() };
                dayList.Add(day);
            }
            return dayList;            
        }

        /// <summary>
        /// This function will get all Years
        /// </summary>
        /// <returns></returns>
        public static List<Year> GetYears()
        {
            var yearList = new List<Year>();

            for (int yearCount = 1900; yearCount <= 2002; yearCount++)
            {
                var year = new Year() { ID = yearCount, Name = yearCount.ToString() };
                yearList.Add(year);
            }
            return yearList;
        }

        /// <summary>
        /// This Function will get Titles
        /// </summary>
        /// <returns></returns>
        public static List<Title> GetTitle()
        {
            return new List<Title>()
            {
                new Title(){ ID = 1, Value = "Mr."},
                new Title(){ ID = 1, Value = "Mrs."},
                
            };
        }

        /// <summary>
        /// This function gets all periods
        /// </summary>
        /// <returns></returns>
        public static List<Period> GetPeriod()
        {
            return new List<Period>()
            {
                new Period(){ ID = 1, Value = "Monthly"},
                new Period(){ ID = 2, Value = "Quaterly"},
                new Period(){ ID = 3, Value = "Annualy"}
            };
        }

        /// <summary>
        /// This function gets all deposit acceptance values
        /// </summary>
        /// <returns></returns>
        public static List<DepositAcceptance> GetAllDepositAcceptance()
        {
            return new List<DepositAcceptance>()
            {
                new DepositAcceptance(){ ID = 1, Value = "Daily"},
                new DepositAcceptance(){ ID = 2, Value = "Weekly"},
                new DepositAcceptance(){ ID = 3, Value = "Bi-weekly"},
                new DepositAcceptance(){ ID = 4, Value = "Monthly"},
                new DepositAcceptance(){ ID = 5, Value = "Quaterly"},
                new DepositAcceptance(){ ID = 6, Value = "Annually"}
            };
        }

        /// <summary>
        /// This function gets all source type values
        /// </summary>
        /// <returns></returns>
        public static List<SourceType> GetAllSourceTypes()
        {
            return new List<SourceType>()
            {
                new SourceType(){ ID = 1, Value = "Bank Account"},
                new SourceType(){ ID = 2, Value = "Skrill Moneybookers"},
                new SourceType(){ ID = 3, Value = "Neteller"}
            };
        }

        /// <summary>
        /// This method gets all fund approval options
        /// </summary>
        /// <returns></returns>
        public static List<FundApprovalOption> GetAllApprovalOptions()
        {
            return new List<FundApprovalOption>()
            {
                new FundApprovalOption(){ ID = 1, Value = "Immediate"},
                new FundApprovalOption(){ ID = 2, Value = "Administrator"},
                new FundApprovalOption(){ ID = 3, Value = "Limited"}
            };
        }
    }

    /// <summary>
    /// Month Class
    /// </summary>
    public class Month
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Day Class
    /// </summary>
    public class Day
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Year Class
    /// </summary>
    public class Year
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Title class
    /// </summary>
    public class Title
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// Period class
    /// </summary>
    public class Period
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// DepositAcceptance class
    /// </summary>
    public class DepositAcceptance
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// SourceType class
    /// </summary>
    public class SourceType
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// FundApprovalOption class
    /// </summary>
    public class FundApprovalOption
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }
}