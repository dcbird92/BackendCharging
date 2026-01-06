using EVCharging.Data;
using Microsoft.EntityFrameworkCore;

namespace EVChargingTests.Utility;

public static class TestDBFactory
{
    public static EvChargingDbContext Create()
    {
        var options = new DbContextOptionsBuilder<EvChargingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new EvChargingDbContext(options);
    }

}
