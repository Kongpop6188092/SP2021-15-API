using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MUDENT_API.Models.Core;
using MUDENT_API.Services.Interface;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace MUDENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilController : ControllerBase
    {
        public UtilController(
            IConfiguration p,
            IUtilService m)
        {
            b = p;
            q = m;
        }

        [HttpGet("lookup")]
        public IActionResult GetLUT([FromQuery] string name)
        {
            List<Dictionary<string, string>> response = new List<Dictionary<string, string>>();
            try
            {
                using (MySqlConnection f = new MySqlConnection(b["ConnectionString"]))
                {
                    f.Open();

                    MySqlCommand t = new MySqlCommand($"SELECT * FROM LUT_{name} ORDER BY ID", f)
                    {
                        CommandType = CommandType.Text
                    };

                    using (MySqlDataReader a = t.ExecuteReader())
                    {
                        List<string> h = new List<string>();                    
                        foreach (MySqlDbColumn v in a.GetColumnSchema())
                        {
                            h.Add(v.ColumnName);
                        }

                        while (a.Read())
                        {
                            Dictionary<string, string> l = new Dictionary<string, string>();

                            foreach (string v in h)
                            {
                                l.Add(v, a[v].ToString());
                            }
                            response.Add(l);
                        }
                    }
                }

                return Ok(new Response<object>()
                {
                    Success = true,
                    Message = string.Empty,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<object>()
                {
                    Success = false,
                    Message = q.GetExceptionMessage(ex)
                });
            }
        }

        private readonly IConfiguration b;
        private readonly IUtilService q;
    }
}
