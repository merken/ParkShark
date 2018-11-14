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

namespace ParkShark.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController : ControllerBase
    {
        private readonly ParkSharkDbContext context;

        public DivisionsController(ParkSharkDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Division>> GetDivisions()
        {
            return await this.context.Divisions.AsNoTracking().ToListAsync();
        }

        [HttpPost]
        public async Task<Division> CreateDivision(Division division)
        {
            if (division.Id != default(int))
            {
                throw new ValidationException<Division>("A new division should not contain an Id");
            }

            if (String.IsNullOrEmpty(division.Name))
            {
                throw new ValidationException<Division>("Name is required");
            }

            if (String.IsNullOrEmpty(division.Director))
            {
                throw new ValidationException<Division>("Director is required");
            }

            await this.context.AddAsync(division);
            await this.context.SaveChangesAsync();
            
            return division;
        }
    }
}