using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
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

        protected static T GetResult<T>(ActionResult<T> actionResult)
            where T : class
        {
            var okResult = actionResult.Result as OkObjectResult;
            return okResult.Value as T;
        }
    }
}
