using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassXYZ.Vault.Tests
{
    public class SerilogFixture : IDisposable
    {
        public ILogger Logger { get; private set; }

        public SerilogFixture()
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(@"logs\xunit_log.txt")
                .CreateLogger();

            Logger.Debug("SerilogFixture: initialized");
        }

        public void Dispose()
        {
            Logger.Debug("SerilogFixture: closed");
            Log.CloseAndFlush();
        }
    }

    [CollectionDefinition("Serilog collection")]
    public class SerilogCollection : ICollectionFixture<SerilogFixture>
    {
    }

}
