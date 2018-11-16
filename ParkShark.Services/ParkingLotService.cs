using ParkShark.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ParkShark.Data.Model;
using ParkShark.Infrastructure;
using ParkShark.Infrastructure.Exceptions;

namespace ParkShark.Services
{

    public interface IParkingLotService : IParkSharkService
    {
        Task<ParkingLot> CreateNewParkingLot(ParkingLot parkingLot);
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
                throw new PersistenceException("Division was not created");

            await context.Entry(parkingLot).Reference(p => p.Division).LoadAsync();
            await context.Entry(parkingLot).Reference(p => p.Contact).LoadAsync();

            return parkingLot;
        }
    }
}
