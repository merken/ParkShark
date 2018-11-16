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
    public class ParkingLotsController : ControllerBase
    {
        private readonly Mapper mapper;
        private readonly IParkingLotService service;

        public ParkingLotsController(Mapper mapper, IParkingLotService service)
        {
            this.mapper = mapper;
            this.service = service;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<DivisionDto>>> GetDivisions()
        //{
        //    var divisions = await this.service.GetAllDivisions();
        //    return Ok(this.mapper.MapToList<DivisionDto, Division>(divisions));
        //}

        [HttpPost]
        public async Task<ActionResult<ParkingLotDto>> CreateNewParkingLot(CreateNewParkingLotDto createNewParkingLotDto)
        {
            var parkingLot = this.mapper.MapTo<ParkingLot, CreateNewParkingLotDto>(createNewParkingLotDto);

            var newParkingLot = await this.service.CreateNewParkingLot(parkingLot);
            var parkingLotDto = this.mapper.MapTo<ParkingLotDto, ParkingLot>(newParkingLot);

            return Ok(parkingLotDto);
        }
    }
}