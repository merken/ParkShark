using ParkShark.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParkShark.Data.Model;
using ParkShark.Infrastructure;
using ParkShark.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ParkShark.Services
{
    public interface IParkingLotService : IParkSharkService
    {
        Task<ParkingLot> CreateNewParkingLot(ParkingLot parkingLot);
        Task<IEnumerable<ParkingLot>> GetAllParkingLots();
        Task<ParkingLot> GetParkingLot(int id);
    }

    public class ParkingLotService : IParkingLotService
    {
        private readonly ParkSharkDbContext context;

        public ParkingLotService(ParkSharkDbContext context)
        {
            this.context = context;
        }

        public async Task<ParkingLot> CreateNewParkingLot(ParkingLot parkingLot)
        {
            await context.ParkingLots.AddAsync(parkingLot);

            if (await context.SaveChangesAsync() == 0)
                throw new PersistenceException("ParkingLot was not created");

            await context.Entry(parkingLot).Reference(p => p.Division).LoadAsync();
            await context.Entry(parkingLot).Reference(p => p.Contact).LoadAsync();
            await context.Entry(parkingLot).Reference(p => p.BuildingType).LoadAsync();

            return parkingLot;
        }

        public async Task<IEnumerable<ParkingLot>> GetAllParkingLots()
        {
            return await context.ParkingLots
                .Include(p => p.Division)
                .Include(p => p.Contact)
                .Include(p => p.BuildingType)
                .OrderBy(p => p.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ParkingLot> GetParkingLot(int id)
        {
            return await context.ParkingLots
                .Include(p => p.Division)
                .Include(p => p.Contact)
                .Include(p => p.BuildingType)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

    }
}
