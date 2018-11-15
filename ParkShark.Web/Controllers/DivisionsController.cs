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
        public async Task<IEnumerable<DivisionDto>> GetDivisions()
        {
            var divisions = await this.service.GetAllDivisions();
            return this.mapper.MapToList<DivisionDto, Division>(divisions);
        }

        [HttpPost]
        public async Task<DivisionDto> CreateDivision(CreateDivisionDto createDivision)
        {
            var division = this.mapper.MapTo<Division, CreateDivisionDto>(createDivision);

            var newDivision = await this.service.CreateDivision(division);

            return this.mapper.MapTo<DivisionDto, Division>(newDivision);
        }
    }
}