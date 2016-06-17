using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrinchWebService1
{
    public class Booking
    {
        private long bookingId;
        private long lib_id;
        private string clientId;
        private string clientPass;
        private string resourceId;
        private DateTime bookingStart;
        private DateTime bookingEnd;


        public Booking(long bookingId, long libId, string clientId, string clientPass, string resourceId, DateTime bookingStart, DateTime bookingEnd)
        {
            this.bookingId = bookingId;
            this.lib_id = libId;
            this.clientId = clientId;
            this.clientPass = clientPass;
            this.resourceId = resourceId;
            this.bookingStart = bookingStart;
            this.bookingEnd = bookingEnd;

        }

        public Booking()
        {
            // TODO: Complete member initialization
        }

        public long BookingID
        {
            get { return bookingId; }
            set { bookingId = value; }
        }
        public long LibraryID
        {
            get { return lib_id; }
            set { lib_id = value; }
        }
        public string ClientID
        {
            get { return clientId; }
            set { clientId = value; }
        }
        public string ClientPass
        {
            get { return clientPass; }
            set { clientPass = value; }
        }
        public string ResourceID
        {
            get { return resourceId; }
            set { resourceId = value; }
        }
        public DateTime BookingStart
        {
            get { return bookingStart; }
            set { bookingStart = value; }
        }
        public DateTime BookingEnd
        {
            get { return bookingEnd; }
            set { bookingEnd = value; }
        }
       
       
    }
}