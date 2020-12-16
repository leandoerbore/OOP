using System.Collections.Generic;

namespace Banks
{
    public class GazpromBank : Bank
    {
        public override string Name { get; protected set; } = "GazpromBank";
        public override double LimitForTransactions { get; protected set; } = 50000;
        public override double Fee { get; protected set; } = 3;
        public override double CreditFee { get; protected set; } = 15;
        public override double CreditLimit { get; protected set; } = -100000;
        public override int BIC { get; protected set;} = 403;

        public GazpromBank()
        {
            conditions = new List<(int firstInterval, int secondInterval, double interest)>(){(10000, 30000, 2),(30001, 60000, 3), (60001,100000000, 4)};
        }
    }
}