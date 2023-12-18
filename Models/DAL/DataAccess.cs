using AE_CommerceApi.Models.Entities;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.Data;

namespace AE_CommerceApi.Models.DAL
{
    public class DataAccess : IDataAccess
    {

        /// <summary>
        /// 
        /// nous avons deux manière d'utiliser la connection a la bae de données autre dit : soit nous déclarons la chaine de connection
        /// dans les fichier appsetting.jon et pour y acceder afin de recuper cette chaine de connection il faut passer par l'objet [IConfiguration]
        /// soit nous créons une classe DBconnection qui contienla chaine de connection et une methodes GetConnection() qui Static donc pas besoin 
        /// d'instancier la classe pour obtenir la chaine de connection. 
        /// 
        /// </summary>

        private readonly IConfiguration _configuration;
        private readonly string _dbconnection;
        private readonly string _dateforme;


        public DataAccess(IConfiguration configuration)
        {
           _configuration = configuration;
         
            _dbconnection = _configuration.GetConnectionString("DB");  //["ConnectionStrings:DB"];
           _dateforme = this._configuration["Constants:DateFormat"];

        }


        public Offer GetOffer(int id)
        {
            // 1- type return of function
            Offer offer = new Offer();


            // 3- connection stting
            using(SqlConnection connection = new(_dbconnection))
            {   
                // 4- Sql command and connection attribut 
                SqlCommand command = new()
                {
                    Connection = connection,
                };

                // 5- Query
                string Query = "Select * from Offers where OfferId=" +id+ ";";


                // 6- association of the Query to the command
                 command.CommandText = Query;


                // 7- open connection
                 connection.Open();

                // 8- execute Query
                SqlDataReader reader = command.ExecuteReader();

                // 9- loop to navigate 
                while( reader.Read())
                {
                    offer.Id = (int)reader["offerId"];
                    offer.title = (string)reader["Title"];
                    offer.Discount = (int)reader["Discount"];
                }

            }
                // 2- return value
                return offer;
        }



        public Product GetProduct(int id)
        {
            //1
            var product = new Product();

            //3
            using (SqlConnection connection = new(_dbconnection)) {

                //4
                SqlCommand command = new()
                {
                    Connection = connection   
                }; 

                 
                // 5
                string query = " SELECT * FROM Products WHERE ProductId =" + id + ";";
                command.CommandText = query;

                //6
                connection.Open();

                //7
                SqlDataReader reader = command.ExecuteReader();

                //8
                while (reader.Read())
                {
                    product.Id = (int)reader["ProductId"];
                    product.Title = (string)reader["Title"];
                    product.Description = (string)reader["Description"];
                    product.Price = (double)reader["Price"];
                    product.Quantity = (int)reader["Quantity"];
                    product.ImageName = (string)reader["ImageName"]; 


                    var categoryid = (int)reader["CategoryId"];
                    product.ProductCategory = GetProductCategory(categoryid);

                    var offerid = (int)reader["OfferId"];
                    product.Offer = GetOffer(offerid);
                }
            
            }
 
            //2
            return product;
        }

        // -plural (ies)
        public List<ProductCategory> GetProductCategories()
        {
            //1
            var productCategories = new List<ProductCategory>();

            //3
            using (SqlConnection connection = new(_dbconnection))
            {


                //4
                SqlCommand command = new()
                {
                    Connection = connection,
                };

                //5
                string query = "select * from ProductCategories;";
                command.CommandText = query;

                //6
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                //7
                while (reader.Read())
                {

                    //8
                    var category = new ProductCategory()
                    {
                        // casting because database column type is different from that entity class model
                        // that is also one of the weakness of ADO.net << Casting >> compared to the use of an ORM
                        Id = (int)reader["CategoryId"],
                        Category = (string)reader["Category"],
                        SubCategory = (string)reader["SubCategory"]
                    };
                    productCategories.Add(category);
                }
            }


            //2
            return productCategories;
        }

        // singular (y)
        public ProductCategory GetProductCategory(int id)
        {
            // 1- the Object that will be returner
            ProductCategory productCategory = new ProductCategory();

            // 3- connection setting
            using (SqlConnection connection = new(_dbconnection))
            {

                // 4 - Sql coomand and Connection Attribut
                // don't forget this principe every request that you will want to send to server require first to open conncetion and send request
                SqlCommand cmd = new()
                {
                    Connection = connection,
                };

                // 5- Query
                   String query = "SELECT * FROM ProductCategories  WHERE CategoryId=" +id+ ";";

                // 6- Association of the Query to the command
                   cmd.CommandText = query;


                // 7- Opening the Connection 
                   connection.Open();

                // 8- run the query
                SqlDataReader reader =cmd.ExecuteReader();

                // 9- loop to navigate
                while(reader.Read())
                {
                    productCategory.Id = (int)reader["CategoryId"];
                    productCategory.Category =(string)reader["Category"];
                    productCategory.SubCategory = (string)reader["SubCategory"];
                }
            }

            // 2- return object
            return productCategory;
        }

        public List<Product> GetProduits(string categorie, string subcategory, int count)
        {
            // 1- the object that will be returned
            List<Product> products = new List<Product>();

            // 3- Connection setting
            using(SqlConnection connection = new(_dbconnection))
            {

                // 4- SqlCommand and connection attribut
                SqlCommand command = new()
                {
                    Connection=connection,
                };

                // 5- Query with newid()
                string Query = "SELECT TOP " + count + " * FROM Products WHERE CategoryId =(SELECT CategoryId FROM ProductCategories WHERE Category =@c and SubCategory = @s ) ORDER BY newid()" +  ";";

                // 6- Association of the Query to the Command
                  command.CommandText = Query;

                ///Added new things

                /*L'utilisation de "System.Data.SqlDbType.NVarChar" permet de définir le type de données d'un paramètre ou d'une colonne
                 * dans une requête SQL, afin de garantir la compatibilité et la <<sécurité>> des données manipulées dans une application
                 * qui interagit avec une base de données SQL Server à l'aide de ADO.NET.*/

                command.Parameters.Add("@c", System.Data.SqlDbType.NVarChar).Value =categorie;                /// What is the difference between the two ??? search
                command.Parameters.Add("@s",System.Data.SqlDbType.NVarChar).Value =subcategory; 
                ///command.Parameters[0].Value = id;

                // 7- Opening connection
                 connection.Open();

                // 8- command Execution
                  SqlDataReader reader=command.ExecuteReader();

                // 9- loop navigate
                 while(reader.Read())
                 {
                    // 10-
                    var product = new Product()
                    {
                        Id = (int)reader["ProductId"],
                        Description = (string)reader["Description"],
                        Title = (string)reader["Title"],
                        Price = (double)reader["price"],
                        Quantity = (int)reader["Quantity"],
                        ImageName = (string)reader["ImageName"],
                          
                    };

                    // 11-
                    // -CategoryId for dataBaseColumn 
                    var categoryid = (int)reader["CategoryId"];
                    // ProductCategory for the model class
                    product.ProductCategory = GetProductCategory(categoryid);


                    // 12-
                    var offerid = (int)reader["OfferId"];
                    product.Offer = GetOffer(offerid);

                    // 13-
                    products.Add(product);
                 }
            
            }





            // 2- return Object
            return products;
        }

        public bool InsertUser(User user)
        {

            //1
            using (SqlConnection connection = new(_dbconnection))
            {
                //3
                SqlCommand command = new SqlCommand()
                {
                    Connection = connection,
                };

                //4
                connection.Open();

                //5
                //   --> count emaiil ,in order to   verify || check if email exist
                string query = "SELECT COUNT(*) FROM Users WHERE Email=@Email;";


                //6
                command.CommandText = query;
                command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar).Value = user.Email;

                //7
                int count = (int)command.ExecuteScalar();

                //8                 
                if (count > 0) { 
                
                  connection.Close();

                    return false;
                }

                query = "insert into Users (FirstName, LastName, Address, Mobile, Email, Password, CreatedAt, ModifiedAt) values (@fn, @ln, @add, @mb, @em,@pwd,@cat,@mat);";
 
                command.CommandText = query;

                // here! we can call <- command.Parameters.AddwithValue -> methods if you want, but the syntaxe will be different with <- command.Parameters.Add ->
                // cuz with <- command.Parameters.AddwithValue -> you should pass the parameter name and parameter Value, which is not the case with <- command.Parameters.Add ->
                // who needs the parameter name and parameter type.

                command.Parameters.Add("@fn", System.Data.SqlDbType.NVarChar).Value = user.FirstName;
                command.Parameters.Add("@ln", System.Data.SqlDbType.NVarChar).Value=user.LastName;
                command.Parameters.Add("@add", System.Data.SqlDbType.NVarChar).Value= user.Address;
                command.Parameters.Add("@mb", System.Data.SqlDbType.NVarChar).Value = user.Mobile;
                command.Parameters.Add("@em",  System.Data.SqlDbType.NVarChar).Value =user.Email;
                command.Parameters.Add("@pwd", System.Data.SqlDbType.NVarChar).Value = user.Password;
                command.Parameters.Add("@cat", System.Data.SqlDbType.NVarChar).Value = user.CreateAt; 
                command.Parameters.Add("@mat", System.Data.SqlDbType.NVarChar).Value =user.ModifiedAt;

                command.ExecuteNonQuery();
            }


            //2
            return true;
        }

        public string IsUserPresent(string email, string password)
        {
            // -1
            User user = new();

            // -2
            using(SqlConnection connection = new(_dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                connection.Open();

                string Query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password;";

                command.CommandText = Query;

                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                int count = (int)command.ExecuteScalar(); 

                if(count ==0)
                {
                    connection.Close();
                    return "";
                }

                string Query_2 = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password;";

                command.CommandText = Query_2;

                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    user.Id = (int)reader["UserId"];
                    user.FirstName = (string)reader["FirstName"];
                    user.LastName = (string)reader["LastName"];
                    user.Email = (string)reader["Email"];
                    user.Address = (string)reader["Address"];
                    user.Mobile = (string)reader["Mobile"];
                    user.Password = (string)reader["Password"];
                    user.CreateAt = (string)reader["CreatedAt"];
                    user.ModifiedAt = (string)reader["ModifiedAt"];
                };

                // before running || execute SqlCommnand class object, we will first  use JWT Token

                // generally the tokenis generayed on the server side, so on the back-end side

                // 1- la clé
                string key = "MNU66iBl3T5rh6H52i69"; //--> il est préférable d'utiliser une fonction qui générera un chaine de caractère au hasard

                string duration = "60";

                //2- création d'une clé symétrique 
                var symetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                //3- création de la signature à partir de la fonction de hachage hmacSha256 et de la clé symétrique 
                var credentials  = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);


                // 4- Section 2 : Les révendications (les informations utilisateurs)
                var claims = new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("firstname", user.FirstName),
                    new Claim("lastname", user.LastName),

                    new Claim("address", user.Address),
                    new Claim("mobile", user.Mobile),
                    new Claim("email", user.Email),

                                       new Claim("id", user.Id.ToString()),
                    new Claim("createdat", user.CreateAt),
                    new Claim("modifiedat", user.ModifiedAt),
                };

                //5- section 3 : création du jeton (token) à partir des infos ci-dessus

                var jwtToken = new JwtSecurityToken(

                    issuer: "localhost",//--> le fournisseur
                    
                    audience: "localhost", //--> public ciblé
                    
                    claims: claims, //--> lrévendication
                    
                    expires: DateTime.Now.AddMinutes(Int32.Parse(duration)), //--> date de validité du token

                    signingCredentials: credentials  //--> signature 

                    );
                return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            } 


            return ""; 

        }
    } 
}
