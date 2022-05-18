using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MUDENT_API.Models.Core;
using MUDENT_API.Models.Doctor.GetAllDentist;
using MUDENT_API.Models.Doctor.GetAppointment;
using MUDENT_API.Models.Doctor.Login;
using MUDENT_API.Models.Doctor.UpdateAppointment;
using MUDENT_API.Services.Interface;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace MUDENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromForm] LoginRequest request)
        {
            Validate t = new Validate();

            if (string.IsNullOrEmpty(request.Username))
            {
                t.isMissing = true;
                t.missingError += "[username], ";
            }
            if (string.IsNullOrEmpty(request.Password))
            {
                t.isMissing = true;
                t.missingError += "[password], ";
            }
            if (t.isMissing)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = p.GetValidateMessage(t.missingError)
                });
            }

            try
            {
                using (MySqlConnection z = new MySqlConnection(m["ConnectionString"]))
                {
                    z.Open();

                    MySqlCommand s = new MySqlCommand("Q_DOCTOR", z)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    MySqlParameter pUsername = new MySqlParameter("PI_USERNAME", MySqlDbType.VarChar, 20)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Username
                    };
                    MySqlParameter pPassword = new MySqlParameter("PI_PASSWORD", MySqlDbType.VarChar, 45)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Password
                    };

                    s.Parameters.Add(pUsername);
                    s.Parameters.Add(pPassword);

                    using (MySqlDataReader e = s.ExecuteReader())
                    {
                        if (!e.HasRows)
                        {
                            return Ok(new Response<LoginResponse>()
                            {
                                Success = true,
                                Message = string.Empty,
                                Data = new LoginResponse(false)
                            });
                        }
                        e.Read();

                        LoginResponse response = new LoginResponse(true);
                        foreach (MySqlDbColumn entry in e.GetColumnSchema())
                        {
                            response.Data.Add(entry.ColumnName, e[entry.ColumnName].ToString());
                        }

                        return Ok(new Response<LoginResponse>()
                        {
                            Success = true,
                            Message = string.Empty,
                            Data = response
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<object>()
                {
                    Success = false,
                    Message = p.GetExceptionMessage(ex)
                });
            }
        }

        private readonly Regex r = new Regex(@"^((\d){4}-(\d){2}-(\d){2}) ((\d){1,2}:(\d){1,2}:(\d){1,2}.(\d){3})$");

        [HttpGet("all")]
        public IActionResult GetAllDentist()
        {
            List<GetAllDentistResponse> response = new List<GetAllDentistResponse>();
            try
            {
                using (MySqlConnection z = new MySqlConnection(m["ConnectionString"]))
                {
                    z.Open();

                    MySqlCommand s = new MySqlCommand("SELECT * FROM DOCTOR", z)
                    {
                        CommandType = CommandType.Text
                    };

                    using (MySqlDataReader e = s.ExecuteReader())
                    {
                        while (e.Read())
                        {
                            response.Add(new GetAllDentistResponse()
                            {
                                Fullname = e["FIRSTNAME"].ToString() + " " + e["LASTNAME"].ToString(),
                                Username = e["USERNAME"].ToString()
                            });
                        }
                    }
                }

                return Ok(new Response<List<GetAllDentistResponse>>()
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
                    Message = p.GetExceptionMessage(ex)
                });
            }
        }

        public DoctorController(
            IConfiguration i,
            IUtilService b)
        {
            m = i;
            p = b;
        }

        [HttpGet("appointment")]
        public IActionResult GetAppointment([FromQuery] string user)
        {
            List<GetAppointmentResponse> response = new List<GetAppointmentResponse>();
            try
            {
                using (MySqlConnection z = new MySqlConnection(m["ConnectionString"]))
                {
                    z.Open();

                    MySqlCommand s = new MySqlCommand("Q_APPOINTMENT", z)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    s.Parameters.Add(new MySqlParameter("PI_USERNAME", MySqlDbType.VarChar, 20)
                    {
                        Direction = ParameterDirection.Input,
                        Value = user
                    });

                    using (MySqlDataReader e = s.ExecuteReader())
                    {
                        while (e.Read())
                        {
                            response.Add(new GetAppointmentResponse()
                            {
                                Username = e.GetString("USERNAME"),
                                DateTime = p.DatetimeToString(e.GetDateTime("DATETIME")),
                                Patient = e["PATIENT"].ToString()
                            });
                        }
                    }
                }

                return Ok(new Response<List<GetAppointmentResponse>>()
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
                    Message = p.GetExceptionMessage(ex)
                });
            }
        }

        private readonly IUtilService p;

        [HttpPost("appointment")]
        public IActionResult UpdateAppointment([FromForm] UpdateAppointmentRequest request)
        {
            Validate t = new Validate();

            if (string.IsNullOrEmpty(request.Username))
            {
                t.isMissing = true;
                t.missingError += "[username], ";
            }
            if (string.IsNullOrEmpty(request.DateTime))
            {
                t.isMissing = true;
                t.missingError += "[datetime], ";
            }
            if (string.IsNullOrEmpty(request.CID))
            {
                t.isMissing = true;
                t.missingError += "[cid], ";
            }
            if (t.isMissing)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = p.GetValidateMessage(t.missingError)
                });
            }

            if (!r.IsMatch(request.DateTime))
            {
                t.isInvalid = true;
                t.invalidError += "[datetime] - YYYY-MM-DD HH:MM:SS.MS, ";
            }
            if (!q.IsMatch(request.CID))
            {
                t.isInvalid = true;
                t.invalidError += "[cid] - 13 digits, ";
            }
            if (t.isInvalid)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = p.GetValidateMessage(t.invalidError)
                });
            }

            string[] bn = request.DateTime.Split(" ");
            string[] lv = bn[0].Split("-");
            string[] cv = bn[1].Split(":");

            int gm = int.Parse(lv[2]);
            int up = int.Parse(cv[0]);
            int ne = int.Parse(lv[1]);
            int qt = int.Parse(lv[0]);
            int zo = int.Parse(cv[1]);

            try
            {
                using (MySqlConnection z = new MySqlConnection(m["ConnectionString"]))
                {
                    z.Open();

                    MySqlCommand s = new MySqlCommand("I_APPOINTMENT", z)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    s.Parameters.Add(new MySqlParameter("PI_USERNAME", MySqlDbType.VarChar, 20)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Username
                    });
                    s.Parameters.Add(new MySqlParameter("PI_DATETIME", MySqlDbType.DateTime)
                    {
                        Direction = ParameterDirection.Input,
                        Value = new DateTime(qt, ne, gm, up, zo, 0)
                    });
                    s.Parameters.Add(new MySqlParameter("PI_CID", MySqlDbType.VarChar, 13)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.CID
                    });

                    if (s.ExecuteNonQuery() == 0)
                    {
                        return StatusCode(500, new Response<object>()
                        {
                            Success = false,
                            Message = "unable to update data"
                        });
                    }
                }

                return Ok(new Response<object>()
                {
                    Success = true,
                    Message = string.Empty
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<object>()
                {
                    Success = false,
                    Message = p.GetExceptionMessage(ex)
                });
            }
        }

        private readonly Regex q = new Regex(@"^(?!0)(\d){13}$");
        private readonly IConfiguration m;
    }
}
