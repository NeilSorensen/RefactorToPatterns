using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorToPatterns
{
    public class Loan
    {
        private DateTime? expiry;
        private DateTime? maturity;
        private decimal commitment;
        private decimal outstanding;
        public IList<Payment> Payments { get; set; }
        private IRiskFactor riskFactors;
        private IRiskFactor unusedRiskFactors;

        private Rating riskRating = Rating.Silver;
        private DateTime? start;
        private const decimal DaysPerYear = 365;

        public Loan(DateTime? expiry, DateTime? maturity, decimal commitment, decimal outstanding,
                    IRiskFactor riskFactors, IRiskFactor unusedRiskFactors)
        {
            this.expiry = expiry;
            this.maturity = maturity;
            this.commitment = commitment;
            this.outstanding = outstanding;
            this.riskFactors = riskFactors;
            this.unusedRiskFactors = unusedRiskFactors;
        }

        public decimal Capital()
        {
            if (expiry == null && maturity != null)  //term loan
            {
                return commitment * Duration() * RiskFactor();
            }
            if (expiry != null && maturity == null)
            {
                if (GetUnusedPercentage() != 1.0m)
                {
                    return commitment * GetUnusedPercentage() * Duration() * RiskFactor();
                }
                else
                {
                    return (OutstandingRiskAmount() * Duration() * RiskFactor()) +
                           (UnusedRiskAmount() * Duration() * UnusedRiskFactor());
                }
            }
            return 0;
        }

        private decimal UnusedRiskFactor()
        {
            return unusedRiskFactors.GetFactorsForRating(riskRating);
        }

        private decimal UnusedRiskAmount()
        {
            return (commitment - outstanding);
        }

        private decimal OutstandingRiskAmount()
        {
            return outstanding;
        }

        private decimal GetUnusedPercentage()
        {
            return UnusedRiskAmount() / commitment;
        }

        private decimal RiskFactor()
        {
            return riskFactors.GetFactorsForRating(riskRating);
        }

        public decimal Duration()
        {
            if (expiry == null && maturity != null)
            {
                return WeightedAverageDuration();
            }
            else if (expiry != null && maturity == null)
            {
                return YearsTo(expiry);
            }
            return 0;
        }

        private decimal YearsTo(DateTime? dateTime)
        {
            DateTime from = start.HasValue ? start.Value : DateTime.Today;
            return ((decimal)(dateTime.Value - from).TotalDays) / DaysPerYear;
        }

        private decimal WeightedAverageDuration()
        {
            decimal duration = 0;
            decimal weightedAverage = 0;
            decimal sumOfPayments = 0;
            foreach (var payment in Payments)
            {
                sumOfPayments += payment.Amount;
                weightedAverage += YearsTo(payment.Date) * payment.Amount;
            }
            if (commitment != 0)
            {
                duration = weightedAverage / sumOfPayments;
            }
            return duration;
        }

        public static Loan CreateTermLoan(decimal amount, DateTime start, DateTime maturity, Rating riskRating, IRiskFactor factors)
        {
            var loan = new Loan(null, maturity, amount, amount, factors, null);
            loan.riskRating = riskRating;
            loan.start = start;
            return loan;
        }
    }

    public interface IRiskFactor
    {
        decimal GetFactorsForRating(Rating riskRating);
    }

    public enum Rating
    {
        Gold,
        Silver,
        Mauve,
        Taupe,
        Fuscia,
        Cranberry,
    }

    public class Payment
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
