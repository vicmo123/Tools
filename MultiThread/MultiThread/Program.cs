using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BankOfBitsAndBytes
{
    class Program
    {
        static readonly int passwordLength = 6;
        static BankOfBitsNBytes bbb;

        static void Main(string[] args)
        {
            bbb = new BankOfBitsNBytes(passwordLength);
            PasswordGuesser.Init(passwordLength, bbb);
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            while (PasswordGuesser.withdrawAmount != -1)
            {
                PasswordGuesser.ParallelBankBreaker(passwordLength);
            }
            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
