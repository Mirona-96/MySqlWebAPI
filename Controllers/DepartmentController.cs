using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlWebAPI.Model;
using System.Data;

namespace MySqlWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get() 
        {
            string query = @"
                        select DepartmentId, DepartmentName FROM Department";

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
        public JsonResult Post(Department dep)
        {
            string query = @"
                        insert into Department (DepartmentName) values (@DepartmentName);";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand mycommand = new MySqlCommand(query, mycon))
                {
                    mycommand.Parameters.AddWithValue("@DepartmentName", dep.DepartmentName);

                    myReader = mycommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }
            return new JsonResult("Added successfully.");
        }

        [HttpPut]
        public JsonResult Put (Department dep)
        {
            string query = @"
                        update Department set 
                        DepartmentName = @DepartmentName
                        where DepartmentId = @DepartmentId;";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand mycommand = new MySqlCommand(query, mycon))
                {
                    mycommand.Parameters.AddWithValue("@DepartmentId", dep.DepartmentId);
                    mycommand.Parameters.AddWithValue("@DepartmentName", dep.DepartmentName);

                    myReader = mycommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }
            return new JsonResult("Updated successfully.");
        }

        [HttpDelete ("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                        delete from Department 
                        where DepartmentId = @DepartmentId;";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand mycommand = new MySqlCommand(query, mycon))
                {
                    mycommand.Parameters.AddWithValue("@DepartmentId", id);

                    myReader = mycommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }
            return new JsonResult("Deleted successfully.");
        }
    }
}
