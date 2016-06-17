using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrinchBook
{
    public class Booking
    {
        private long id;
        private User user;
        private long library_id;
        private string resource_id;
        private DateTime bookingDateStart;
        private DateTime bookingDateEnd;


        public Booking(long id, User user, long library_id, string resource_id, DateTime bookingDateStart, DateTime bookingDateEnd)
        {
            this.id=id;
            this.user=user;
            this.library_id=library_id;
            this.resource_id=resource_id;
            this.bookingDateStart=bookingDateStart;
            this.bookingDateEnd = bookingDateEnd;
           
           
        }

        public Booking()
        {
            // TODO: Complete member initialization
        }

        public long Id{
            get { return id; }
            set { id = value; }
        }
        public User User
        {
            get { return user; }
            set { user = value; }
        }
        public long LibraryId
        {
            get { return library_id; }
            set { library_id = value; }
        }
        public string ResourceId
        {
            get { return resource_id; }
            set { resource_id= value; }
        }
        public DateTime BookinDateStart
        {
            get { return bookingDateStart; }
            set { bookingDateStart = value; }
        }
        public DateTime BookingDateEnd
        {
            get { return bookingDateEnd; }
            set { bookingDateEnd= value; }
        }
        
    }
}