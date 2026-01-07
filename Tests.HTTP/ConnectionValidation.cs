using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HTTP.Base;

namespace Tests.HTTP
{
    [TestClass]
    public class ConnectionValidation : TestBase
    {
        [TestMethod]
        public async Task ValidateConnection_Success()
        {
            var validator = new Apps.HTTP.Connections.ConnectionValidator();

            var result = await validator.ValidateConnection(Creds, CancellationToken.None);
            Console.WriteLine(result.Message);
            Assert.IsTrue(result.IsValid);
        }
    }
}
