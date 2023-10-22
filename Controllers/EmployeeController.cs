using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlWebAPI.Model;
using System.Data;

namespace MySqlWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public EmployeeController (IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                        select EmployeeId, EmployeeName, Department, 
                        DATE_FORMAT (Dateofjoining, '%y - %m- %d') as Dateofjoining,
                        photofilename
                        FROM Employee";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand mycommand = new MySqlCommand(query, mycon))
                {
                    myReader = mycommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Employee emp)
        {
            string query = @"
                        insert into Employee (EmployeeName, Department, DateOfJoining,
                        photofilename) 
                        values (@EmployeeName, @Department, @DateOfJoining,
                        @photofilename);";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand mycommand = new MySqlCommand(query, mycon))
                {
                    mycommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    mycommand.Parameters.AddWithValue("@Department", emp.Department);
                    // Converte a string da data em DateTime
                    if (DateTime.TryParse(emp.DateOfJoining, out DateTime dateOfJoining))
                    {
                        mycommand.Parameters.AddWithValue("@DateOfJoining", dateOfJoining);
                    }
                    else
                    {
                        // Se a conversão falhar, você pode lidar com isso adequadamente, por exemplo, definindo a data como DateTime.MinValue ou outra data padrão.
                        mycommand.Parameters.AddWithValue("@DateOfJoining", DateTime.MinValue);
                    }

                    mycommand.Parameters.AddWithValue("@photofilename", emp.PhotoFileName);


                    myReader = mycommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }
            return new JsonResult("Added successfully.");
        }

        [HttpPut]
        public JsonResult Put(Employee emp)
        {
            string query = @"
                        update Employee set 
                        EmployeeName = @EmployeeName, 
                        Department = @Department, 
                        Dateofjoining = @Dateofjoining,
                        photofilename = @photofilename
                        where EmployeeId = @EmployeeId;";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand mycommand = new MySqlCommand(query, mycon))
                {
                    mycommand.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);
                    mycommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    mycommand.Parameters.AddWithValue("@Department", emp.Department);
                    mycommand.Parameters.AddWithValue("@Dateofjoining", emp.DateOfJoining);
                    mycommand.Parameters.AddWithValue("@photofilename", emp.PhotoFileName);

                    myReader = mycommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }
            return new JsonResult("Updated successfully.");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                        delete from Employee 
                        where EmployeeId = @EmployeeId;";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand mycommand = new MySqlCommand(query, mycon))
                {
                    mycommand.Parameters.AddWithValue("@EmployeeId", id);

                    myReader = mycommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }
            return new JsonResult("Deleted successfully.");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpResult = Request.Form;
                var postedFile = httpResult.Files[0];

                if (postedFile == null || postedFile.Length == 0)
                {
                    return new JsonResult("Nenhum arquivo enviado.");
                }

                // Verifique se o diretório existe ou crie-o
                var uploadDirectory = Path.Combine(_environment.ContentRootPath, "Photos");
                Directory.CreateDirectory(uploadDirectory);

                // Gere um nome de arquivo único e seguro
                var fileName = Path.GetRandomFileName();
                var fileExtension = Path.GetExtension(postedFile.FileName);
                fileName = Path.ChangeExtension(fileName, fileExtension);

                var filePath = Path.Combine(uploadDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(fileName);
            }
            catch (System.IO.IOException ex)
            {
                // Lide com erros de E/S, se necessário
                return new JsonResult("Erro de E/S: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Lide com outras exceções de maneira apropriada para o seu aplicativo
                return new JsonResult("Erro: " + ex.Message);
            }
        }
    }
}
