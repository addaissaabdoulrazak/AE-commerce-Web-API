using AE_CommerceApi.Models.DAL;
using AE_CommerceApi.Models.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace AE_CommerceApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;
        private readonly string _DataFormat;  // => this parameter value is mentioned inside the configuration file 
        public ShoppingController(IDataAccess dataAccess, IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _DataFormat = configuration["Constants:DateFormat"];
        }

        // Rappel : HttpGet, HttpPost, HttpDelete sont appellent des verbs ou Methodes => ce sont des fonction 
        [HttpGet("GetCategoryList")]
        public IActionResult GetCategoryList()
        {
            var result = _dataAccess.GetProductCategories(); 
            
            return Ok(result);
        }

        [HttpGet("GetProducts")]
        public IActionResult GetProducts(string category, string subcategory, int count)
        {
            var result = _dataAccess.GetProduits(category, subcategory, count);
            return Ok(result);
        }

        [HttpGet("GetProduct/{id}")]
        public IActionResult GetProduct(int id)
        {
            var result = _dataAccess.GetProduct(id);

            return Ok(result);
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            user.CreateAt = DateTime.Now.ToString(_DataFormat);
            user.ModifiedAt = DateTime.Now.ToString(_DataFormat);

            var result= _dataAccess.InsertUser(user);

            string? message;
            if (result) message = "inserted";
            else message = "email not available";

            return Ok(message);
        }

        // An Token, you are attribute only after if you are already registered upstream(en Amont).
        // An you want to  Logged-In now.
        // remarks : if you are currently logged-In, there's no need to assign you a token. 
        [HttpPost("LoginUser")]
        public IActionResult LoginUser([FromBody] User user)
        {

            var token = _dataAccess.IsUserPresent(user.Email, user.Password);
            if (token == "") token = "invalid";
            return Ok(token);




            return Ok(token);
        }

    }
}
