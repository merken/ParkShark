using System;
using System.Collections.Generic;
using System.Text;
using ParkShark.Domain.Exceptions;

namespace ParkShark.Domain
{
    public class MemberShipLevel
    {
        public enum Level
        {
            Bronze,
            Silver,
            Gold
        }

        private MemberShipLevel()
        {
        }

        public MemberShipLevel(Level name, decimal monthlyCost, decimal allocationReduction, decimal maximumDurationInMinutes)
        {
            if (default(decimal) == maximumDurationInMinutes)
            {
                throw new ValidationException<MemberShipLevel>("maximumDurationInMinutes is required");
            }

            Name = name;
            MonthlyCost = monthlyCost;
            AllocationReduction = allocationReduction;
            MaximumDurationInMinutes = maximumDurationInMinutes;
        }

        public Level Name { get; set; }
        public decimal MonthlyCost { get; set; }
        public decimal AllocationReduction { get; set; }
        public decimal MaximumDurationInMinutes { get; set; }
    }
}
