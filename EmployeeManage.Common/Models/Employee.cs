using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManage.Common.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string? Name { get; set; }
        public string? SSN { get; set; }
        public DateTime DOB { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Phone { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? ExitDate { get; set; }

        // For query results
        public decimal CurrentSalary { get; set; }
        public string? CurrentTitle { get; set; }
    }
}
