using EmployeeManage.Common.Models;
using EmployeeManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManage.Data.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(string? nameFilter = null, string? titleFilter = null);
        Task<IEnumerable<TitleSalaryInfo>> GetTitlesWithMinMaxSalaryAsync();
        Task AddEmployeeWithSalaryAsync(Employee employee, EmployeeSalary salary);
        Task<IEnumerable<EmployeeWithSalary>> GetEmployeesPagedAsync(string name, string title, int pageNumber, int pageSize, string sortColumn, string sortOrder);
    }
}
