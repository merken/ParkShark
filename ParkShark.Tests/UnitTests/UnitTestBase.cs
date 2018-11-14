using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ParkShark.Data.Model;

namespace ParkShark.Tests.UnitTests
{
    public abstract class UnitTestBase
    {
        private static DbContextOptions CreateNewInMemoryDatabaseOptions()
        {
            return new DbContextOptionsBuilder<ParkSharkDbContext>()
                .UseInMemoryDatabase("ParkSharkDb" + Guid.NewGuid().ToString("N"))
                .Options;
        }

        protected static ParkSharkDbContext NewInMemoryParkSharkDbContext()
        {
            return new ParkSharkDbContext(CreateNewInMemoryDatabaseOptions());
        }
    }
}
