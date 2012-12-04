using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;

namespace RefactorToPatterns
{
    [TestFixture]
    class AuthorizationTests
    {
        [Test]
        public void UsesAuthorizationTest()
        {
            var mockAuthorizer = MockRepository.GenerateMock<IAuthorizer>();
            mockAuthorizer.Stub(x => x.IsAuthorized("Token1")).Return(true);

            var testUser = new UsesAuthorization(mockAuthorizer);
            string result = testUser.AccessAuthorizedResource();
            Assert.That(result, Is.EqualTo("Authorized Resource"));
        }
    }
}
