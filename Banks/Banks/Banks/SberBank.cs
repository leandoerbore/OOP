using System.Collections.Generic;

namespace Banks
{
    public class SberBank : Bank
    {
        public override string Name { get; protected set; } = "SberBank";
        public override double LimitForTransactions { get; protected set; } = 40000;
        public override double Fee { get; protected set; } = 3;
        public override double CreditFee { get; protected set; } = 20;
        public override double CreditLimit { get; protected set; } = -80000;

        public override int BIC { get; protected set; } = 401;

        public SberBank()
        {
            conditions = new List<(int firstInterval, int secondInterval, double interest)>(){(10000, 40000, 2),(40001, 90000, 3), (90001, 10000000, 3.5)};
        }
    }
}