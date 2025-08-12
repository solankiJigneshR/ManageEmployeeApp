using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManage.Common.Models
{
    public class EmployeeSalary
    {
        public int SalaryID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Title { get; set; }
        public decimal Salary { get; set; }
    }
}
