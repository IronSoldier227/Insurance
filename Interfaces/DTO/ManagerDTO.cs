using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.DTO
{
    public class ManagerDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public string PassportNumber { get; set; }
        public string DriverLicense { get; set; }
        public int DrivingExperience { get; set; }
    }
}
