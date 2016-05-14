using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class DateConvert
    {
        //output the data 12-Apr-2015
        public string GetDate(int day, int month, int year, string format, string sp)
        {
            string outDate = "";
            string Day = "";
            string Month = "";
            string Year = "";
            switch (format)
            {
                case "ddmmyyy":
                    Day = day.ToString();
                    Month = month.ToString();
                    Year = year.ToString();
                    outDate = Day + sp + Month + sp + Year;
                    break;
                case "ddmmyy":
                    Day = day.ToString();
                    Month = month.ToString();
                    Year = year.ToString().Substring(2,2);
                    outDate = Day + sp + Month + sp + Year;
                    break;
                case "ddMMyy":
                    Day = day.ToString();
                    Month = GetMonthName(month);
                    Year = year.ToString().Substring(2, 2);
                    outDate = Day + sp + Month + sp + Year;
                    break;
                case "ddMMyyyy":
                    Day = day.ToString();
                    Month = GetMonthName(month);
                    Year = year.ToString();
                    outDate = Day + sp + Month + sp + Year;
                    break;
                case "yyyyMMdd":
                    Day = day.ToString();
                    Month = GetMonthName(month);
                    Year = year.ToString();
                    outDate = Year + sp + Month + sp + Day;
                    break;
                case "yyMMdd":
                    Day = day.ToString();
                    Month = GetMonthName(month);
                    Year = year.ToString().Substring(2, 2);
                    outDate = Year + sp + Month + sp + Day;
                    break;
            }
            return outDate;
        }


        //get month name
        public static string GetMonthName(int month)
        {
            string MonthName = "";
            switch (month)
            {
                case 1:
                    MonthName = "Jan";
                    break;
                case 2:
                    MonthName = "Feb";
                    break;
                case 3:
                    MonthName = "Mar";
                    break;
                case 4:
                    MonthName = "Apr";
                    break;
                case 5:
                    MonthName = "May";
                    break;
                case 6:
                    MonthName = "Jun";
                    break;
                case 7:
                    MonthName = "Jul";
                    break;
                case 8:
                    MonthName = "Aug";
                    break;
                case 9:
                    MonthName = "Sep";
                    break;
                case 10:
                    MonthName = "Oct";
                    break;
                case 11:
                    MonthName = "Nov";
                    break;
                case 12:
                    MonthName = "Dec";
                    break;
            }
            return MonthName;
        }
    }
}