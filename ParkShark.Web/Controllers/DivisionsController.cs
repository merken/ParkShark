using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkShark.Data.Model;
using ParkShark.Domain;
using ParkShark.Domain.Exceptions;
using ParkShark.Infrastructure;
using ParkShark.Services;
using ParkShark.Web.DTO;

namespace ParkShark.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController : ControllerBase
    {
        private readonly Mapper mapper;
        private readonly IDivisionService service;

        public DivisionsController(Mapper mapper, IDivisionService service)
        {
            this.mapper = mapper;
            this.service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DivisionDto>>> GetDivisions()
        {
            var divisions = await this.service.GetAllDivisions();
            return Ok(this.mapper.MapToList<DivisionDto, Division>(divisions));
        }

        [HttpPost]
        public async Task<ActionResult<DivisionDto>> CreateDivision(CreateDivisionDto createDivision)
        {
            var division = this.mapper.MapTo<Division, CreateDivisionDto>(createDivision);

            var newDivision = await this.service.CreateDivision(division);
            var dto = this.mapper.MapTo<DivisionDto, Division>(newDivision);

            return Ok(dto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<DivisionDto>> GetDivision(int id)
        {
            var division = await this.service.GetDivision(id);

            if (division == null)
                return NotFound();

            return Ok(this.mapper.MapTo<DivisionDto, Division>(division));
        }
    }
}