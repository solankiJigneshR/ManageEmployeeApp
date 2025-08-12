using EmployeeManage.Common.Models;
using EmployeeManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManage.Business.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync(string? nameFilter = null, string? titleFilter = null);
        Task<IEnumerable<TitleSalaryInfo>> GetTitlesAsync();
        Task AddEmployeeAsync(Employee employee, EmployeeSalary salary);
        Task<IEnumerable<EmployeeWithSalary>> GetEmployeesPagedAsync(string name, string title, int pageNumber, int pageSize, string sortColumn, string sortOrder);
    }
}
