using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkShark.Domain;
using ParkShark.Infrastructure;
using ParkShark.Services;
using ParkShark.Web.DTO;

namespace ParkShark.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllocationsController : ControllerBase
    {
        private readonly Mapper mapper;
        private readonly IAllocationService service;

        public AllocationsController(Mapper mapper, IAllocationService service)
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
        public async Task<ActionResult<AllocationDto>> CreateAllocation(CreateAllocationDto createAllocation)
        {
            var allocation = this.mapper.MapTo<Allocation, CreateAllocationDto>(createAllocation);

            var newAllocation = await this.service.CreateAllocation(allocation);
            var dto = this.mapper.MapTo<AllocationDto, Allocation>(newAllocation);

            return Ok(dto);
        }

        [HttpPut]
        public async Task<ActionResult<AllocationDto>> StopAllocation(StopAllocationDto stopAllocation)
        {
            var newAllocation = await this.service.StopAllocation(stopAllocation.AllocationId, stopAllocation.MemberId);
            var dto = this.mapper.MapTo<AllocationDto, Allocation>(newAllocation);

            return Ok(dto);
        }
    }
}
