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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParkingLotDto>>> GetParkingLots()
        {
            var parkingLots = await this.service.GetAllParkingLots();
            return Ok(this.mapper.MapToList<ParkingLotDto, ParkingLot>(parkingLots));
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<ParkingLotDto>> GetParkingLot(int id)
        {
            var parkingLot = await this.service.GetParkingLot(id);
            return Ok(this.mapper.MapTo<ParkingLotDto, ParkingLot>(parkingLot));
        }

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