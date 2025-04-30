using GitCRUDAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GitCRUDAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAPIController : ControllerBase
    {
        private readonly PathrolContext _context;
        public EmployeeAPIController(PathrolContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _context.Employees
                    .FromSqlRaw(
                        @"EXEC sp_Employee_CRUD 
                    @Action = {0}, 
                    @Id = NULL, 
                    @Name = NULL, 
                    @Dob = NULL, 
                    @State = NULL, 
                    @Country = NULL, 
                    @NewId = NULL, 
                    @msg = NULL",
                        "READALL")
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            try
            {
                if (employee == null)
                    return BadRequest("Employee is null");

                // Prepare all parameters
                var actionParam = new SqlParameter("@Action", "CREATE");

                var idParam = new SqlParameter("@Id", System.Data.SqlDbType.Int)
                {
                    Value = DBNull.Value
                };

                var nameParam = new SqlParameter("@Name", employee.Name ?? (object)DBNull.Value);
                var dobParam = new SqlParameter("@Dob", employee.Dob);
                var stateParam = new SqlParameter("@State", employee.State ?? (object)DBNull.Value);
                var countryParam = new SqlParameter("@Country", employee.Country ?? (object)DBNull.Value);

                var newIdParam = new SqlParameter("@NewId", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };

                var msgParam = new SqlParameter("@msg", System.Data.SqlDbType.VarChar, 100)
                {
                    Direction = System.Data.ParameterDirection.Output
                };

                // Execute stored procedure


                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Employee_CRUD @Action, @Id, @Name, @Dob, @State, @Country, @NewId OUTPUT, @msg OUTPUT",
                    actionParam, idParam, nameParam, dobParam, stateParam, countryParam, newIdParam, msgParam
                );
                // Assign output values
                employee.Id = (int)newIdParam.Value;
                string message = msgParam.Value?.ToString();

                return Ok(new
                {
                    employee,
                    message
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database concurrency error: {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database update error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred: {ex.Message}");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (employee == null || id != employee.Id)
                return BadRequest("Invalid employee data");

            try
            {
                var parameters = new[]
                {
            new SqlParameter("@Action", "UPDATE"),
            new SqlParameter("@Id", id),
            new SqlParameter("@Name", employee.Name ?? (object)DBNull.Value),
            new SqlParameter("@Dob", employee.Dob),
            new SqlParameter("@State", employee.State ?? (object)DBNull.Value),
            new SqlParameter("@Country", employee.Country ?? (object)DBNull.Value),
            new SqlParameter("@NewId", System.Data.SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@msg", System.Data.SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output }
        };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Employee_CRUD @Action, @Id, @Name, @Dob, @State, @Country, @NewId OUTPUT, @msg OUTPUT",
                    parameters
                );

                string message = parameters[7].Value?.ToString();
                return Ok(new { employee.Id, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@Action", "DELETE"),
            new SqlParameter("@Id", id),
            new SqlParameter("@Name", DBNull.Value),
            new SqlParameter("@Dob", DBNull.Value),
            new SqlParameter("@State", DBNull.Value),
            new SqlParameter("@Country", DBNull.Value),
            new SqlParameter("@NewId", System.Data.SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@msg", System.Data.SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output }
        };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Employee_CRUD @Action, @Id, @Name, @Dob, @State, @Country, @NewId OUTPUT, @msg OUTPUT",
                    parameters
                );

                string message = parameters[7].Value?.ToString();
                return Ok(new { id, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            try
            {
                var employee = await Task.Run(() =>
                    _context.Employees
                        .FromSqlRaw("EXEC sp_Employee_CRUD @Action = {0}, @Id = {1}, @Name = NULL, @Dob = NULL, @State = NULL, @Country = NULL, @NewId = NULL, @msg = NULL",
                                    "READ", id)
                        .AsNoTracking()
                        .AsEnumerable()
                        .FirstOrDefault()
                );

                if (employee == null)
                    return NotFound();

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }





    }

}
