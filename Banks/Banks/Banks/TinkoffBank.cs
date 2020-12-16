using System.Collections.Generic;

namespace Banks
{
    public class TinkoffBank : Bank
    {
        public override string Name { get; protected set;} = "TinkoffBank";
        public override double LimitForTransactions { get; protected set; } = 50000;
        public override double Fee { get; protected set; } = 2;
        public override double CreditFee { get; protected set; } = 30;
        public override double CreditLimit { get; protected set; } = -100000;

        public override int BIC { get; protected set; } = 402;

        public TinkoffBank()
        {
            conditions = new List<(int firstInterval, int secondInterval, double interest)>(){(30000, 50000, 3),(50001, 100000, 3.5), (100000,100000000, 4)};
        }
    }
}