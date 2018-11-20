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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllMembers()
        {
            var members = await this.service.GetAllMembers();
            return Ok(mapper.MapToList<MemberDto, Member>(members));
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<MemberDto>> GetMember(int id)
        {
            var member = await this.service.GetMember(id);

            if (member == null)
                return NotFound();

            return Ok(mapper.MapTo<MemberDto, Member>(member));
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
