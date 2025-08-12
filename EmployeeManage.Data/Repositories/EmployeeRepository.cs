using Dapper;
using EmployeeManage.Common.Models;
using EmployeeManage.Data.Interfaces;
using EmployeeManage.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManage.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(string? nameFilter = null, string? titleFilter = null)
        {
            using var conn = CreateConnection();
            var sql = @"
SELECT e.EmployeeID, e.Name, e.SSN, e.DOB, e.Address, e.City, e.State, e.Zip, e.Phone, e.JoinDate, e.ExitDate,
       s.Salary AS CurrentSalary, s.Title AS CurrentTitle
FROM Employee e
INNER JOIN EmployeeSalary s ON e.EmployeeID = s.EmployeeID
WHERE s.ToDate IS NULL
";

            var parameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(nameFilter))
            {
                sql += " AND e.Name LIKE @NameFilter";
                parameters.Add("NameFilter", $"%{nameFilter}%");
            }
            if (!string.IsNullOrEmpty(titleFilter))
            {
                sql += " AND s.Title LIKE @TitleFilter";
                parameters.Add("TitleFilter", $"%{titleFilter}%");
            }

            var rows = await conn.QueryAsync<Employee>(sql, parameters);
            return rows.ToList();
        }

        public async Task<IEnumerable<TitleSalaryInfo>> GetTitlesWithMinMaxSalaryAsync()
        {
            using var conn = CreateConnection();
            var sql = @"
SELECT Title, MIN(Salary) AS MinSalary, MAX(Salary) AS MaxSalary
FROM EmployeeSalary
GROUP BY Title
ORDER BY Title";
            var rows = await conn.QueryAsync<TitleSalaryInfo>(sql);
            return rows.ToList();
        }

        public async Task AddEmployeeWithSalaryAsync(Employee employee, EmployeeSalary salary)
        {
            using var conn = CreateConnection();
            if (conn is DbConnection dbConn)
            {
                await dbConn.OpenAsync();
            }
            using var tran = conn.BeginTransaction();

            var insertEmployee = @"
INSERT INTO Employee (Name, SSN, DOB, Address, City, State, Zip, Phone, JoinDate, ExitDate)
VALUES (@Name, @SSN, @DOB, @Address, @City, @State, @Zip, @Phone, @JoinDate, @ExitDate);
SELECT CAST(SCOPE_IDENTITY() as int);";

            var empId = await conn.ExecuteScalarAsync<int>(insertEmployee, new
            {
                employee.Name,
                employee.SSN,
                employee.DOB,
                employee.Address,
                employee.City,
                employee.State,
                employee.Zip,
                employee.Phone,
                employee.JoinDate,
                employee.ExitDate
            }, tran);

            var insertSalary = @"
INSERT INTO EmployeeSalary (EmployeeID, FromDate, ToDate, Title, Salary)
VALUES (@EmployeeID, @FromDate, @ToDate, @Title, @Salary);";

            await conn.ExecuteAsync(insertSalary, new
            {
                EmployeeID = empId,
                FromDate = salary.FromDate,
                ToDate = (object?)salary.ToDate ?? DBNull.Value,
                Title = salary.Title,
                Salary = salary.Salary
            }, tran);

            tran.Commit();
        }

        public async Task<IEnumerable<EmployeeWithSalary>> GetEmployeesPagedAsync(
    string nameFilter,
    string titleFilter,
    int pageNumber,
    int pageSize,
    string sortColumn,
    string sortOrder)
        {
            using var connection = CreateConnection();

            string query = @"
        WITH EmployeeCTE AS (
            SELECT 
                e.EmployeeId,
                e.Name,
                es.Title,
                es.Salary,
                ROW_NUMBER() OVER (ORDER BY 
                    CASE WHEN @SortColumn = 'Name' AND @SortOrder = 'ASC' THEN e.Name END ASC,
                    CASE WHEN @SortColumn = 'Name' AND @SortOrder = 'DESC' THEN e.Name END DESC,
                    CASE WHEN @SortColumn = 'Title' AND @SortOrder = 'ASC' THEN es.Title END ASC,
                    CASE WHEN @SortColumn = 'Title' AND @SortOrder = 'DESC' THEN es.Title END DESC,
                    CASE WHEN @SortColumn = 'Salary' AND @SortOrder = 'ASC' THEN es.Salary END ASC,
                    CASE WHEN @SortColumn = 'Salary' AND @SortOrder = 'DESC' THEN es.Salary END DESC
                ) AS RowNum
            FROM Employee e
            INNER JOIN EmployeeSalary es ON e.EmployeeId = es.EmployeeId
            WHERE (@NameFilter IS NULL OR e.Name LIKE '%' + @NameFilter + '%')
              AND (@TitleFilter IS NULL OR es.Title LIKE '%' + @TitleFilter + '%')
              AND es.ToDate IS NULL
        )
        SELECT * FROM EmployeeCTE
        WHERE RowNum BETWEEN (@PageNumber - 1) * @PageSize + 1 AND @PageNumber * @PageSize
        ORDER BY RowNum;
    ";

            return await connection.QueryAsync<EmployeeWithSalary>(query, new
            {
                NameFilter = string.IsNullOrWhiteSpace(nameFilter) ? null : nameFilter,
                TitleFilter = string.IsNullOrWhiteSpace(titleFilter) ? null : titleFilter,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortOrder = sortOrder
            });
        }


    }
}
