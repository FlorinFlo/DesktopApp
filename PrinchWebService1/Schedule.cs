using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace PrinchWebService1
{
    [Serializable]
    public class Schedule
    {
        private TimeSpan openingHour;
        private TimeSpan closingHour;
        
        public Schedule(TimeSpan openingHour, TimeSpan closingHour)
        {
            this.openingHour = openingHour;
            this.closingHour = closingHour;
        }
        public Schedule()
        {

        }
        [XmlIgnore]
        public TimeSpan OpeningHour
        {
            get { return openingHour; }
            set { openingHour = value; }
        }
        [XmlIgnore]
        public TimeSpan ClosingHour
        {
            get { return closingHour; }
            set { closingHour = value; }
        }

        [Browsable(false)]
        [XmlElement(DataType = "duration", ElementName = "OpeningHour")]
        public string OpeningHourString
        {
            get
            {
                return XmlConvert.ToString(OpeningHour);
            }
            set
            {
                OpeningHour = string.IsNullOrEmpty(value) ?
                TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
            }
        }
        [Browsable(false)]
        [XmlElement(DataType = "duration", ElementName = "ClosingHour")]
        public string ClosingHourString
        {
            get
            {
                return XmlConvert.ToString(ClosingHour);
            }
            set
            {
                OpeningHour = string.IsNullOrEmpty(value) ?
                TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
            }
        }
    }
}