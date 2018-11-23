using System;
using System.Collections.Generic;
using System.Text;
using ParkShark.Domain.Exceptions;

namespace ParkShark.Domain
{
    public class Allocation
    {
        private Allocation() { }

        public Allocation(int memberId, int parkingLotId, LicensePlate licensePlate, DateTime startAllocationDate)
        {
            if (default(int) == memberId)
            {
                throw new ValidationException<Allocation>("memberId is required");
            }

            if (default(int) == parkingLotId)
            {
                throw new ValidationException<Allocation>("parkingLotId is required");
            }

            if (licensePlate == null)
            {
                throw new ValidationException<Allocation>("licensePlate is required");
            }

            if (default(DateTime) == startAllocationDate)
            {
                throw new ValidationException<Allocation>("startAllocationDate is required");
            }

            MemberId = memberId;
            ParkingLotId = parkingLotId;
            LicensePlate = licensePlate;
            StartDateTime = startAllocationDate;
        }

        public Guid Id { get; private set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public int ParkingLotId { get; set; }
        public ParkingLot ParkingLot { get; set; }
        public LicensePlate LicensePlate { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }
}
