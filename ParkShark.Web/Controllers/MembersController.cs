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
    public class MembersController : ControllerBase
    {
        private readonly Mapper mapper;
        private readonly IMemberService service;

        public MembersController(Mapper mapper, IMemberService service)
        {
            this.mapper = mapper;
            this.service = service;
        }

        [HttpPost]
        public async Task<ActionResult<MemberDto>> CreateNewMember(CreateNewMemberDto createNewMemberDto)
        {
            var member = this.mapper.MapTo<Member, CreateNewMemberDto>(createNewMemberDto);

            var newMember = await this.service.CreateNewMember(member);
            var memberDto = this.mapper.MapTo<MemberDto, Member>(newMember);

            return Ok(memberDto);
        }
    }
}
