using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sufinn.Visitor.Core.Common;
using Sufinn.Visitor.Core.Entity;
using Sufinn.Visitor.Core.Model;
using Sufinn.Visitor.Repository.Context;
using Sufinn.Visitor.Services.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sufinn.Visitor.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly IBaseService _service;
        private readonly AppDBContext _context;
        public MasterController(IBaseService baseService, AppDBContext context)
        {
            _service = baseService;
            _context = context;
        }
        [HttpGet]
        [Route("getPurposes")]
        public async Task<Result<List<MstPurpose>>> GetPurposes()
        {
            try
            {
                var data = await _service.GetRespository<MstPurpose>().ExecuteStoredProcedureAsync("pr_get_purpose", null);
                if (data.ToList().Count > 0)
                {
                    return Common<List<MstPurpose>>.getResponse(true, "Data found.", data.ToList());
                }
                return Common<List<MstPurpose>>.getResponse(false, "No record found.", null);
            }
            catch (Exception ex)
            {
                return Common<List<MstPurpose>>.getResponse(false, ex.Message, null);
            }
        }
        [HttpGet]
        [Route("getPeopleDetails")]
        public async Task<Result<List<EmployeeEntity>>> GetPeopleDetails()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "p_id", "" }
                };
                var data = await _service.GetRespository<EmployeeEntity>().ExecuteStoredProcedureAsync("pr_get_people_detail", parameters, true);

                if (data.ToList().Count > 0)
                {
                    return Common<List<EmployeeEntity>>.getResponse(true, "Data found.", data.ToList());
                }
                return Common<List<EmployeeEntity>>.getResponse(false, "No record found.", null);
            }
            catch (Exception ex)
            {
                return Common<List<EmployeeEntity>>.getResponse(false, ex.Message, null);
            }
        }
        [HttpGet]
        [Route("getPeopleDetails/{searchTerm}")]
        public async Task<Result<List<EmployeeEntity>>> GetPeopleDetails(string searchTerm)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "p_id", searchTerm }
                };
                var data = await _service.GetRespository<EmployeeEntity>().ExecuteStoredProcedureAsync("pr_get_people_detail", parameters, true);


                if (data.ToList().Count > 0)
                {
                    return Common<List<EmployeeEntity>>.getResponse(true, "Data found.", data.ToList());
                }
                return Common<List<EmployeeEntity>>.getResponse(false, "No record found.", null);
            }
            catch (Exception ex)
            {
                return Common<List<EmployeeEntity>>.getResponse(false, ex.Message, null);
            }
        }
    }
}
