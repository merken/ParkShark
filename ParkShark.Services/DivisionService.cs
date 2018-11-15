using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ParkShark.Data.Model;
using ParkShark.Domain;
using ParkShark.Domain.Exceptions;
using ParkShark.Infrastructure;
using ParkShark.Infrastructure.Exceptions;

namespace ParkShark.Services
{
    public interface IDivisionService : IParkSharkService
    {
        Task<Division> CreateDivision(Division division);
        Task<Division> CreateSubDivision(Division division);
        Task<Division> GetDivision(int id);
        Task<IEnumerable<Division>> GetAllDivisions();
    }

    public class DivisionService : IDivisionService
    {
        private readonly ParkSharkDbContext context;

        public DivisionService(ParkSharkDbContext context)
        {
            this.context = context;
        }

        public async Task<Division> CreateDivision(Division division)
        {
            await context.Divisions.AddAsync(division);

            if (await context.SaveChangesAsync() == 0)
                throw new PersistenceException("Division was not created");

            return division;
        }

        public async Task<Division> CreateSubDivision(Division division)
        {
            if (division.ParentDivisionId == null)
                throw new ValidationException<Division>("A subdivision should have a ParentDivisionId");

            await context.Divisions.AddAsync(division);
            
            if (await context.SaveChangesAsync() == 0)
                throw new PersistenceException("SubDivision was not created");

            return division;
        }

        public async Task<Division> GetDivision(int id)
        {
            return await context.Divisions.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Division>> GetAllDivisions()
        {
            var divisions = await context.Divisions.AsNoTracking().ToListAsync();
            return divisions;
        }
    }
}
