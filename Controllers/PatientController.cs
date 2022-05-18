using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MUDENT_API.Models.Core;
using MUDENT_API.Models.Patient.GetMedicine;
using MUDENT_API.Models.Patient.GetPatients;
using MUDENT_API.Models.Patient.GetTreatment;
using MUDENT_API.Models.Patient.Login;
using MUDENT_API.Models.Patient.Register;
using MUDENT_API.Models.Patient.SaveTreatment;
using MUDENT_API.Models.User.Patient.SaveDrugAllergy;
using MUDENT_API.Models.User.Patient.SaveMedicine;
using MUDENT_API.Services.Interface;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MUDENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromForm]LoginRequest request)
        {
            Validate x = new Validate();

            if (string.IsNullOrEmpty(request.Username))
            {
                x.isMissing = true;
                x.missingError += "[username], ";
            }
            if (string.IsNullOrEmpty(request.Password))
            {
                x.isMissing = true;
                x.missingError += "[password], ";
            }
            if (x.isMissing)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.missingError)
                });
            }

            try
            {
                using (MySqlConnection y = new MySqlConnection(a["ConnectionString"]))
                {
                    y.Open();

                    MySqlCommand t = new MySqlCommand("Q_PATIENT", y)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    t.Parameters.Add(new MySqlParameter("PI_USERNAME", MySqlDbType.VarChar, 30)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Username
                    });
                    t.Parameters.Add(new MySqlParameter("PI_PASSWORD", MySqlDbType.VarChar, 45)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Password
                    });

                    using (MySqlDataReader v = t.ExecuteReader())
                    {
                        if (!v.HasRows)
                        {
                            return Ok(new Response<LoginResponse>()
                            {
                                Success = true,
                                Message = string.Empty,
                                Data = new LoginResponse(false)
                            });
                        }

                        v.Read();

                        LoginResponse q = new LoginResponse(true);
                        foreach (MySqlDbColumn entry in v.GetColumnSchema())
                        {
                            q.Data.Add(entry.ColumnName, v[entry.ColumnName].ToString());
                        }

                        return Ok(new Response<LoginResponse>()
                        {
                            Success = true,
                            Message = string.Empty,
                            Data = q
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<object>()
                {
                    Success = false,
                    Message = g.GetExceptionMessage(ex)
                });
            }
        }

        private readonly IConfiguration a;

        [HttpPost("register")]
        public IActionResult Register([FromForm]RegisterRequest request)
        {
            Validate x = new Validate();

            if (string.IsNullOrEmpty(request.Username))
            {
                x.isMissing = true;
                x.missingError += "[username], ";
            }
            if (string.IsNullOrEmpty(request.Password))
            {
                x.isMissing = true;
                x.missingError += "[password], ";
            }
            if (string.IsNullOrEmpty(request.Name))
            {
                x.isMissing = true;
                x.missingError += "[name], ";
            }
            if (string.IsNullOrEmpty(request.CID))
            {
                x.isMissing = true;
                x.missingError += "[cid], ";
            }
            if (x.isMissing)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.missingError)
                });
            }

            if (!b.IsMatch(request.CID))
            {
                x.isInvalid = true;
                x.invalidError += "[cid] - 13 digits, ";
            }
            if (x.isInvalid)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.invalidError)
                });
            }

            try
            {
                using (MySqlConnection y = new MySqlConnection(a["ConnectionString"]))
                {
                    y.Open();

                    MySqlCommand t = new MySqlCommand("I_PATIENT", y)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    t.Parameters.Add(new MySqlParameter("PI_USERNAME", MySqlDbType.VarChar, 30)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Username
                    });
                    t.Parameters.Add(new MySqlParameter("PI_PASSWORD", MySqlDbType.VarChar, 45)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Password
                    });
                    t.Parameters.Add(new MySqlParameter("PI_NAME", MySqlDbType.VarChar, 100)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Name
                    });
                    t.Parameters.Add(new MySqlParameter("PI_CID", MySqlDbType.VarChar, 13)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.CID
                    });
                    t.Parameters.Add(new MySqlParameter("PI_ADDRESS", MySqlDbType.VarChar, 4000)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Address
                    });

                    if (t.ExecuteNonQuery() == 0)
                    {
                        return StatusCode(500, new Response<object>()
                        {
                            Success = false,
                            Message = "Unable to register patient"
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
                    Message = g.GetExceptionMessage(ex)
                });
            }
        }

        [HttpGet("all")]
        public IActionResult GetPatients()
        {
            List<GetPatientsResponse> q = new List<GetPatientsResponse>();
            try
            {
                using (MySqlConnection y = new MySqlConnection(a["ConnectionString"]))
                {
                    y.Open();

                    MySqlCommand t = new MySqlCommand("SELECT * FROM PATIENT", y)
                    {
                        CommandType = CommandType.Text
                    };

                    using (MySqlDataReader v = t.ExecuteReader())
                    {
                        while (v.Read())
                        {
                            q.Add(new GetPatientsResponse()
                            {
                                CID = v["CID"].ToString(),
                                Name = v["NAME"].ToString(),
                                DrugAllergy = v["DRUG_ALLERGY"].ToString()
                            });
                        }
                    }
                }

                return Ok(new Response<List<GetPatientsResponse>>()
                {
                    Success = true,
                    Message = string.Empty,
                    Data = q
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<object>()
                {
                    Success = false,
                    Message = g.GetExceptionMessage(ex)
                });
            }
        }

        public PatientController(
            IConfiguration z,
            IUtilService u)
        {
            a = z;
            g = u;
        }

        [HttpGet("treatment")]
        public IActionResult GetTreatment([FromQuery] string cid)
        {
            Validate x = new Validate();

            if (string.IsNullOrEmpty(cid))
            {
                x.isMissing = true;
                x.missingError += "[cid], ";
            }
            if (x.isMissing)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.missingError)
                });
            }

            if (!b.IsMatch(cid))
            {
                x.isInvalid = true;
                x.invalidError += "[cid] - 13 digits, ";
            }
            if (x.isInvalid)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.invalidError)
                });
            }

            List<GetTreatmentResponse> q = new List<GetTreatmentResponse>();
            try
            {
                using (MySqlConnection y = new MySqlConnection(a["ConnectionString"]))
                {
                    y.Open();

                    MySqlCommand t = new MySqlCommand($"" +
                        $"SELECT TREATMENT.*, CONCAT(DOCTOR.FIRSTNAME, ' ', DOCTOR.LASTNAME) AS DENTIST " +
                            $"FROM TREATMENT " +
                            $"JOIN DOCTOR " +
                                $"ON DOCTOR.USERNAME = TREATMENT.DR_USER " +
                            $"WHERE PATIENT_CID = '{cid}' " +
                            $"ORDER BY CREATED_DATE DESC", y)
                    {
                        CommandType = CommandType.Text
                    };

                    using (MySqlDataReader v = t.ExecuteReader())
                    {
                        while (v.Read())
                        {
                            q.Add(new GetTreatmentResponse()
                            {
                                Dentist = v["DENTIST"].ToString(),
                                ToothNo = v["TOOTH_NO"].ToString(),
                                ToothSide = v["TOOTH_SIDE"].ToString(),
                                Diagnosis = v["DIAGNOSIS"].ToString(),
                                Choice = v["CHOICE"].ToString(),
                                CreatedDate = v.GetDateTime("CREATED_DATE").ToString("g", new CultureInfo("th-TH"))
                            });
                        }                     
                    }
                }

                return Ok(new Response<List<GetTreatmentResponse>>()
                {
                    Success = true,
                    Message = string.Empty,
                    Data = q
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<object>()
                {
                    Success = false,
                    Message = g.GetExceptionMessage(ex)
                });
            }
        }

        private readonly Regex p = new Regex(@"^(?!0)(\d)+$");

        [HttpGet("treatment-desc")]
        public IActionResult GetTreatmentDescription([FromQuery] string cid)
        {
            Validate x = new Validate();

            if (string.IsNullOrEmpty(cid))
            {
                x.isMissing = true;
                x.missingError += "[cid], ";
            }
            if (x.isMissing)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.missingError)
                });
            }

            if (!b.IsMatch(cid))
            {
                x.isInvalid = true;
                x.invalidError += "[cid] - 13 digits, ";
            }
            if (x.isInvalid)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.invalidError)
                });
            }

            List<GetTreatmentResponse> q = new List<GetTreatmentResponse>();
            try
            {
                using (MySqlConnection y = new MySqlConnection(a["ConnectionString"]))
                {
                    y.Open();

                    MySqlCommand t = new MySqlCommand("Q_TREATMENT", y)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    t.Parameters.Add(new MySqlParameter("PI_CID", MySqlDbType.VarChar, 13)
                    {
                        Direction = ParameterDirection.Input,
                        Value = cid
                    });

                    using (MySqlDataReader v = t.ExecuteReader())
                    {
                        while (v.Read())
                        {
                            q.Add(new GetTreatmentResponse()
                            {
                                Dentist = v["DR_USER"].ToString(),
                                ToothNo = v["TOOTH_NO"].ToString(),
                                ToothSide = v["TOOTH_SIDE"].ToString(),
                                Diagnosis = v["DIAGNOSIS"].ToString(),
                                Choice = v["CHOICE"].ToString(),
                                CreatedDate = v.GetDateTime("CREATED_DATE").ToString("g", new CultureInfo("th-TH"))
                            });
                        }
                    }
                }

                return Ok(new Response<List<GetTreatmentResponse>>()
                {
                    Success = true,
                    Message = string.Empty,
                    Data = q
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<object>()
                {
                    Success = false,
                    Message = g.GetExceptionMessage(ex)
                });
            }
        }

        [HttpPost("treatment")]
        public IActionResult SaveTreatment([FromForm] SaveTreatmentRequest request)
        {
            Validate x = new Validate();

            if (string.IsNullOrEmpty(request.Dentist))
            {
                x.isMissing = true;
                x.missingError += "[dentist], ";
            }
            if (string.IsNullOrEmpty(request.CID))
            {
                x.isMissing = true;
                x.missingError += "[cid], ";
            }
            if (string.IsNullOrEmpty(request.ToothNo))
            {
                x.isMissing = true;
                x.missingError += "[toothNo], ";
            }
            if (string.IsNullOrEmpty(request.ToothSide))
            {
                x.isMissing = true;
                x.missingError += "[toothSide], ";
            }
            if (string.IsNullOrEmpty(request.Diagnosis))
            {
                x.isMissing = true;
                x.missingError += "[diagnosis], ";
            }
            if (string.IsNullOrEmpty(request.Choice))
            {
                x.isMissing = true;
                x.missingError += "[choice], ";
            }
            if (x.isMissing)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.missingError)
                });
            }

            if (!b.IsMatch(request.CID))
            {
                x.isInvalid = true;
                x.invalidError += "[cid] - 13 digits, ";
            }
            if (!p.IsMatch(request.ToothNo))
            {
                x.isInvalid = true;
                x.invalidError += "[toothNo] - integer, ";
            }
            if (!p.IsMatch(request.ToothSide))
            {
                x.isInvalid = true;
                x.invalidError += "[toothSide] - integer, ";
            }
            if (!p.IsMatch(request.Diagnosis))
            {
                x.isInvalid = true;
                x.invalidError += "[diagnosis] - integer, ";
            }
            if (!p.IsMatch(request.Choice))
            {
                x.isInvalid = true;
                x.invalidError += "[choice] - integer, ";
            }
            if (x.isInvalid)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.invalidError)
                });
            }

            try
            {
                using (MySqlConnection y = new MySqlConnection(a["ConnectionString"]))
                {
                    y.Open();

                    MySqlCommand t = new MySqlCommand("I_TREATMENT", y)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    t.Parameters.Add(new MySqlParameter("PI_DENTIST", MySqlDbType.VarChar, 50)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Dentist
                    });
                    t.Parameters.Add(new MySqlParameter("PI_CID", MySqlDbType.VarChar, 13)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.CID
                    });
                    t.Parameters.Add(new MySqlParameter("PI_TOOTH_NO", MySqlDbType.Int32)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.ToothNo
                    });
                    t.Parameters.Add(new MySqlParameter("PI_TOOTH_SIDE", MySqlDbType.Int32)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.ToothSide
                    });
                    t.Parameters.Add(new MySqlParameter("PI_DIAGNOSIS", MySqlDbType.Int32)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Diagnosis
                    });
                    t.Parameters.Add(new MySqlParameter("PI_CHOICE", MySqlDbType.Int32)
                    {
                        Direction = ParameterDirection.Input,
                        Value = request.Choice
                    });

                    if (t.ExecuteNonQuery() == 0)
                    {
                        return StatusCode(500, new Response<object>()
                        {
                            Success = false,
                            Message = "Unable to save treatment record"
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
                    Message = g.GetExceptionMessage(ex)
                });
            }
        }

        [HttpGet("medicine")]
        public IActionResult GetMedicine([FromQuery] string cid)
        {
            Validate x = new Validate();

            if (string.IsNullOrEmpty(cid))
            {
                x.isMissing = true;
                x.missingError += "[cid], ";
            }
            if (x.isMissing)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.missingError)
                });
            }

            if (!b.IsMatch(cid))
            {
                x.isInvalid = true;
                x.invalidError += "[cid] - 13 digits, ";
            }
            if (x.isInvalid)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.invalidError)
                });
            }

            List<GetMedicineResponse> q = new List<GetMedicineResponse>();
            try
            {
                using (MySqlConnection y = new MySqlConnection(a["ConnectionString"]))
                {
                    y.Open();

                    MySqlCommand t = new MySqlCommand("Q_MEDICINE", y)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    t.Parameters.Add(new MySqlParameter("PI_CID", MySqlDbType.VarChar, 13)
                    {
                        Direction = ParameterDirection.Input,
                        Value = cid
                    });

                    using (MySqlDataReader v = t.ExecuteReader())
                    {
                        while (v.Read())
                        {
                            q.Add(new GetMedicineResponse()
                            {
                                Doctor = v.GetString("DR_USER"),
                                Medicine = v.GetInt32("MED_ID"),
                                Amount = v.GetInt32("AMOUNT"),
                                CreatedDate = v.GetDateTime("CREATED_DATE").ToString("g", new CultureInfo("th-TH"))
                            });
                        }
                    }
                }

                return Ok(new Response<List<GetMedicineResponse>>()
                {
                    Success = true,
                    Message = string.Empty,
                    Data = q
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<object>()
                {
                    Success = false,
                    Message = g.GetExceptionMessage(ex)
                });
            }
        }

        private readonly Regex b = new Regex(@"^(?!0)(\d){13}$");

        [HttpPost("medicine")]
        public IActionResult SaveMedicine([FromBody] List<SaveMedicineRequest> request)
        {
            Validate x = new Validate();

            foreach (SaveMedicineRequest entry in request)
            {
                if (string.IsNullOrEmpty(entry.Medicine))
                    continue;

                if (string.IsNullOrEmpty(entry.Doctor))
                {
                    x.isMissing = true;
                    x.missingError += "[doctor], ";
                }
                if (string.IsNullOrEmpty(entry.CID))
                {
                    x.isMissing = true;
                    x.missingError += "[cid], ";
                }
                if (entry.Amount == null)
                {
                    x.isMissing = true;
                    x.missingError += "[amount], ";
                }
                if (x.isMissing)
                {
                    return BadRequest(new Response<object>()
                    {
                        Success = false,
                        Message = g.GetValidateMessage(x.missingError)
                    });
                }

                if (!b.IsMatch(entry.CID))
                {
                    x.isInvalid = true;
                    x.invalidError += "[cid] - 13 digits, ";
                }
                if (x.isInvalid)
                {
                    return BadRequest(new Response<object>()
                    {
                        Success = false,
                        Message = g.GetValidateMessage(x.invalidError)
                    });
                }
            }

            try
            {
                using (MySqlConnection y = new MySqlConnection(a["ConnectionString"]))
                {
                    y.Open();

                    MySqlCommand t = new MySqlCommand("I_MEDICINE", y)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    MySqlParameter ax = new MySqlParameter("PI_DOCTOR", MySqlDbType.VarChar, 20)
                    {
                        Direction = ParameterDirection.Input
                    };
                    MySqlParameter bq = new MySqlParameter("PI_CID", MySqlDbType.VarChar, 13)
                    {
                        Direction = ParameterDirection.Input
                    };
                    MySqlParameter ep = new MySqlParameter("PI_MED_ID", MySqlDbType.Int32)
                    {
                        Direction = ParameterDirection.Input
                    };
                    MySqlParameter ui = new MySqlParameter("PI_AMOUNT", MySqlDbType.Int32)
                    {
                        Direction = ParameterDirection.Input
                    };

                    t.Parameters.Add(ax);
                    t.Parameters.Add(bq);
                    t.Parameters.Add(ep);
                    t.Parameters.Add(ui);

                    foreach (SaveMedicineRequest entry in request)
                    {
                        if (string.IsNullOrEmpty(entry.Medicine))
                            continue;

                        ax.Value = entry.Doctor;
                        bq.Value = entry.CID;
                        ep.Value = entry.Medicine;
                        ui.Value = entry.Amount;

                        t.ExecuteNonQuery();
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
                    Message = g.GetExceptionMessage(ex)
                });
            }
        }

        private readonly IUtilService g;

        [HttpPost("drug-allergy")]
        public IActionResult SaveDrugAllergy([FromForm] SaveDrugAllergyRequest request)
        {
            Validate x = new Validate();

            if (string.IsNullOrEmpty(request.CID))
            {
                x.isMissing = true;
                x.missingError += "[cid], ";
            }
            if (x.isMissing)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.missingError)
                });
            }

            if (!b.IsMatch(request.CID))
            {
                x.isInvalid = true;
                x.invalidError += "[cid] - 13 digits, ";
            }
            if (x.isInvalid)
            {
                return BadRequest(new Response<object>()
                {
                    Success = false,
                    Message = g.GetValidateMessage(x.invalidError)
                });
            }

            try
            {
                using (MySqlConnection y = new MySqlConnection(a["ConnectionString"]))
                {
                    y.Open();

                    MySqlCommand t = new MySqlCommand($"UPDATE PATIENT SET DRUG_ALLERGY='{request.Allergy}' WHERE CID='{request.CID}'", y)
                    {
                        CommandType = CommandType.Text
                    };

                    int record = t.ExecuteNonQuery();
                    if (record == 0)
                    {
                        return StatusCode(500, new Response<object>()
                        {
                            Success = false,
                            Message = "unable to update drug allergy"
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
                    Message = g.GetExceptionMessage(ex)
                });
            }
        }
    }
}
