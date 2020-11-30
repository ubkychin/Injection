using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using DBConnExample.models;
using Microsoft.Extensions.Configuration;

namespace DBConnExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DBConnectionTestController
    {
        //string connectionString = @"Data Source=bikestoresdb.c3raologixkl.us-east-1.rds.amazonaws.com;Initial Catalog=SampleDB;User ID=admin;Password=abcd1234";
        SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder();
        IConfiguration configuration;
        string connectionString = "";

        public DBConnectionTestController(IConfiguration iConfig) {
            this.configuration = iConfig;

            // use configuration to retrieve connection string from appsettings.json
            //this.connectionString = this.configuration.GetSection("ConnectionString").Value;

            // use the SqlConnectionStringBuilder to create our connection string
            this.stringBuilder.DataSource = this.configuration.GetSection("DBConnectionString").GetSection("Url").Value;
            this.stringBuilder.InitialCatalog = this.configuration.GetSection("DBConnectionString").GetSection("Database").Value;
            this.stringBuilder.UserID = this.configuration.GetSection("DBConnectionString").GetSection("User").Value;
            this.stringBuilder.Password = this.configuration.GetSection("DBConnectionString").GetSection("Password").Value;

            this.connectionString = stringBuilder.ConnectionString;

        }

        [HttpGet]
        public List<Customer> TestConnection() {
            List<Customer> customers = new List<Customer>();

            // Connect to an SQL Server Database
            //string connectionString = @"Data Source=bikestoresdb.c3raologixkl.us-east-1.rds.amazonaws.com;Initial Catalog=SampleDB;User ID=admin;Password=abcd1234";
            SqlConnection conn = new SqlConnection(this.connectionString);

            string queryString = "Select * From Customer";

            SqlCommand command = new SqlCommand( queryString, conn);
            conn.Open();
        
            string result = "";
            using(SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result += reader[0] + " | " + reader[1] + reader[2] + "\n";
                    
                    // ORM - Object Relation Mapping
                    customers.Add(
                        new Customer() { Id = (int)reader[0], FirstName = reader[1].ToString(), Surname = reader[2].ToString()});                
                }
            }

            return customers;
        }

        [HttpGet("{searchString}")]
        public List<Customer> FindSurname(string searchString) {
            List<Customer> customers = new List<Customer>();

            SqlConnection conn = new SqlConnection(this.connectionString);

            //string queryString = "Select * From Customer WHERE LastName LIKE \'%" + searchString + "%\'";
            string queryString = $"Select * From Customer WHERE LastName LIKE '%' + @LastName + '%'";

            SqlCommand command = new SqlCommand( queryString, conn);
            // command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar);
            // command.Parameters["@LastName"].Value = $"\'%{searchString}%\'";
            command.Parameters.AddWithValue("@LastName", searchString);

            conn.Open();
        
            string result = "";
            using(SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result += reader[0] + " | " + reader[1] + reader[2] + "\n";
                    
                    // ORM - Object Relation Mapping
                    customers.Add(
                        new Customer() { Id = (int)reader[0], FirstName = reader[1].ToString(), Surname = reader[2].ToString()});                
                }
            }

            return customers;
        }

        [HttpGet("Products")]
        public List<Product> GetProducts() {
            List<Product> products = new List<Product>();

            // Connect to an SQL Server Database
            //string connectionString = @"Data Source=bikestoresdb.c3raologixkl.us-east-1.rds.amazonaws.com;Initial Catalog=SampleDB;User ID=admin;Password=abcd1234";
            SqlConnection conn = new SqlConnection(this.connectionString);

            string queryString = "Select ProductName, UnitPrice, CompanyName From Product join Supplier on Product.SupplierId=Supplier.Id";

            SqlCommand command = new SqlCommand( queryString, conn);
            conn.Open();

            using(SqlDataReader reader = command.ExecuteReader()) {
                while (reader.Read())
                {
                    products.Add(
                        new Product() { Name = reader[0].ToString(), UnitPrice = (decimal)reader[1], SupplierName = reader[2].ToString()});                
                }
            }

            return products;
        }

        [HttpGet("Delete/{Id}")]
        public string Delete91(string Id) {
            //string connectionString = @"Data Source=bikestoresdb.c3raologixkl.us-east-1.rds.amazonaws.com;Initial Catalog=SampleDB;User ID=admin;Password=abcd1234";
            SqlConnection conn = new SqlConnection(this.connectionString);

            string queryString = "Delete From Customer Where Id = @ID";

            SqlCommand command = new SqlCommand(queryString, conn);
            command.Parameters.AddWithValue("@ID", int.Parse(Id));

            conn.Open();

            try {
                var result = command.ExecuteNonQuery();
                return result.ToString();
            } catch (SqlException se) {
                return "Cannot delete user with id 91: " + se.Message;
            }


        }

        [HttpGet("injectionr/{searchString}")]
        public List<Customer> Injection(string searchString) {
            List<Customer> customers = new List<Customer>();

            SqlConnection conn = new SqlConnection(this.connectionString);

            //string queryString = "Select * From Customer WHERE LastName LIKE \'%" + searchString + "%\'";
            string queryString = "Select * From Customer WHERE LastName = '" + searchString + "'";

            SqlCommand command = new SqlCommand( queryString, conn);
            // command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar);
            // command.Parameters["@LastName"].Value = $"\'%{searchString}%\'";
            command.Parameters.AddWithValue("@LastName", searchString);

            conn.Open();
        
            string result = "";
            using(SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result += reader[0].ToString() + reader[1].ToString() + reader[2].ToString() + "\n";
                    
                    // ORM - Object Relation Mapping
                    customers.Add(
                        new Customer() { Id = (int)reader[0], FirstName = reader[1].ToString(), Surname = reader[2].ToString()});                
                }
            }

            return customers;
        }

    }

    
}