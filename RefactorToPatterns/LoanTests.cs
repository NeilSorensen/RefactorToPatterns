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
    public class LoanTests
    {
        [Test]
        public void NotTermLoanTest()
        {
            IRiskFactor riskFactors = MockRepository.GenerateMock<IRiskFactor>();
            riskFactors.Stub(x => x.GetFactorsForRating(Rating.Cranberry)).Return(.01m).IgnoreArguments();
            Loan testLoan = new Loan(DateTime.Today.AddYears(5), null, 1000, 100, riskFactors, null);
            var capital = decimal.Round(testLoan.Capital(), 2, MidpointRounding.AwayFromZero);
            Assert.That(capital, Is.EqualTo(45.02));
        }

        [Test]
        public void TermLoanSamePayments()
        {
            IRiskFactor riskFactors = MockRepository.GenerateMock<IRiskFactor>();
            riskFactors.Stub(x => x.GetFactorsForRating(Rating.Cranberry)).Return(.035m).IgnoreArguments();
            DateTime start = new DateTime(2003,11,20);
            DateTime maturity = new DateTime(2006,11,20);
            var testLoan = Loan.CreateTermLoan(3000, start, maturity, Rating.Cranberry, riskFactors);
            testLoan.Payments = new List<Payment>
                                    {
                                        new Payment{Amount = 1000, Date = new DateTime(2004, 11,20)},
                                        new Payment{Amount = 1000, Date = new DateTime(2005, 11,20)},
                                        new Payment{Amount = 1000, Date = new DateTime(2006, 11,20)},
                                    };
            var duration = decimal.Round(testLoan.Duration(), 2, MidpointRounding.AwayFromZero);
            var capital = decimal.Round(testLoan.Capital(), 2, MidpointRounding.AwayFromZero);
            Assert.That(duration, Is.EqualTo(2));
            Assert.That(capital, Is.EqualTo(210.29));
        }
    }
}
