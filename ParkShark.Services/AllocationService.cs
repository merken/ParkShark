using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ParkShark.Data.Model;
using ParkShark.Domain;
using ParkShark.Domain.Exceptions;
using ParkShark.Infrastructure;
using ParkShark.Infrastructure.Exceptions;

namespace ParkShark.Services
{
    public interface IAllocationService : IParkSharkService
    {
        Task<Allocation> CreateAllocation(Allocation allocation);
        Task<IEnumerable<Allocation>> GetAllocationsForParkingLot(int parkingLotId);
    }

    public class AllocationService : IAllocationService
    {
        private readonly ParkSharkDbContext context;
        private readonly IMemberService memberService;
        private readonly IParkingLotService parkingLotService;

        public AllocationService(ParkSharkDbContext context, IMemberService memberService, IParkingLotService parkingLotService)
        {
            this.context = context;
            this.memberService = memberService;
            this.parkingLotService = parkingLotService;
        }


        public async Task<Allocation> CreateAllocation(Allocation allocation)
        {
            var member = await memberService.GetMember(allocation.MemberId);
            if (member == null)
                throw new ValidationException<Allocation>($"Member with id {allocation.MemberId} does not exist");

            var parkingLot = await parkingLotService.GetParkingLot(allocation.ParkingLotId);
            if (parkingLot == null)
                throw new ValidationException<Allocation>($"Parking Lot with id {allocation.ParkingLotId} does not exist");

            if (!member.LicensePlate.Equals(allocation.LicensePlate))
                throw new ValidationException<Allocation>("The provided licensePlate most be equal to the members' license plate");

            var allocations = await GetAllocationsForParkingLot(allocation.ParkingLotId);
            if (allocations.Count() >= parkingLot.Capacity)
                throw new AllocationException($"Allocation failed, parking lot {allocation.ParkingLotId} is full");

            await context.Allocations.AddAsync(allocation);

            if (await context.SaveChangesAsync() == 0)
                throw new PersistenceException("Allocation was not created");

            await context.Entry(allocation).Reference(a => a.Member).LoadAsync();
            await context.Entry(allocation).Reference(a => a.ParkingLot).LoadAsync();

            return allocation;
        }

        public async Task<IEnumerable<Allocation>> GetAllocationsForParkingLot(int parkingLotId)
        {
            return await context.Allocations
                .Where(a => a.ParkingLotId == parkingLotId)
                .Include(a => a.Member)
                .Include(a => a.ParkingLot)
                .ToListAsync();
        }
    }
}
