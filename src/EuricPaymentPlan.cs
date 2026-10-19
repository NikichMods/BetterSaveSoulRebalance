using System;

namespace SoulDLCRebalance
{
    internal static class EuricPaymentPlan
    {
        internal const string PaidKey = "bssr_euric_paid_v1";
        internal const int FirstPayment = 10;

        internal static int ReadMask(float value)
        {
            int mask = (int)value;
            if (float.IsNaN(value) || float.IsInfinity(value) || value != mask ||
                mask < 0 || mask > 15 || (mask != 0 && (mask & 1) == 0))
                throw new InvalidOperationException("Invalid Euric payment record");
            return mask;
        }

        internal static int Amount(int stage)
        {
            switch (stage) { case 1: return 20; case 2: return 30; case 3: return 50; }
            throw new ArgumentOutOfRangeException("stage");
        }

        internal static bool IsDue(int paidMask, int earnedMask, int stage)
        {
            int bit = 1 << stage;
            return paidMask != 0 && (earnedMask & bit) != 0 && (paidMask & bit) == 0;
        }
    }
}
