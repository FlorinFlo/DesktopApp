using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrinchWebService1
{
    public class Setup
    {
        private int setupId;
        private int prebookingDays;
        private int ageRestriction;
        private bool controlPanel;
        private bool cdmStatus;
        private int schedule;
        //private byte[] image;

        public Setup(int setupId,int prebookindDays,int ageRestriction,bool controlPanel,bool cmdStatus,int schedule)
        {
            this.setupId = setupId;
            this.prebookingDays = prebookindDays;
            this.ageRestriction = ageRestriction;
            this.controlPanel = controlPanel;
            this.cdmStatus = cmdStatus;
            this.schedule = schedule;
            

        }

        public Setup()
        {

        }
        public int SetupID
        { 
            get { return setupId; }
            set { setupId = value; }
        }
        public int PrebookingDays
        { 
            get {return prebookingDays ;}
            set { prebookingDays = value; }
        }
        public int AgeRestriction 
        {
            get { return ageRestriction; }
            set { ageRestriction = value; }
        }
        public bool ControlPanel
        { 
            get { return controlPanel; }
            set { controlPanel = value; }
        }
        public bool CmdStatus 
        {
            get { return cdmStatus; }
            set { cdmStatus = value; }
        }
        public int Schedule
        { 
            get { return schedule; }
            set { schedule = value; }
        }
        
    }
}