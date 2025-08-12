using EmployeeManage.Business.Interfaces;
using EmployeeManage.Common.Models;
using EmployeeManage.Data.Interfaces;
using EmployeeManage.Data.Repositories;
using EmployeeManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManage.Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;

        public EmployeeService(IEmployeeRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Employee>> GetEmployeesAsync(string? nameFilter = null, string? titleFilter = null)
            => _repo.GetAllEmployeesAsync(nameFilter, titleFilter);

        public Task<IEnumerable<TitleSalaryInfo>> GetTitlesAsync()
            => _repo.GetTitlesWithMinMaxSalaryAsync();

        public Task AddEmployeeAsync(Employee employee, EmployeeSalary salary)
            => _repo.AddEmployeeWithSalaryAsync(employee, salary);

        public Task<IEnumerable<EmployeeWithSalary>> GetEmployeesPagedAsync(
    string name,
    string title,
    int pageNumber,
    int pageSize,
        string sortColumn,
    string sortOrder)
        {
            return _repo.GetEmployeesPagedAsync(name, title, pageNumber, pageSize, sortColumn, sortOrder);
        }

    }
}
