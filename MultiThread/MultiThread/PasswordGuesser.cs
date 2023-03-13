using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

static class PasswordGuesser
{
    public static BankOfBitsAndBytes.BankOfBitsNBytes bank { get; set; }
    public static int maxNumGuesses;
    public static int processorCount = 0;

    public static int withdrawAmount { get; set; } = 0;

    public static void Init(int passLength, BankOfBitsAndBytes.BankOfBitsNBytes _bank)
    {
        maxNumGuesses = (int)Math.Pow(26, passLength);
        bank = _bank;
        processorCount = Environment.ProcessorCount;
    }

    public static void ParallelBankBreaker(int passLength)
    {
        int numChunks = processorCount * 2;
        int chunkSize = maxNumGuesses / numChunks;

        List<Task> tasks = new List<Task>();
        for (int i = 0; i < numChunks; i++)
        {
            int chunkStart = i * chunkSize;
            int chunkEnd = (i == numChunks - 1) ? maxNumGuesses : (i + 1) * chunkSize;

            Task task = Task.Factory.StartNew(() =>
            {
                FilterChunk(passLength, chunkStart, chunkEnd);
            });

            tasks.Add(task);
        }

        Task.WaitAll(tasks.ToArray());
    }

    public static void FilterChunk(int passLength, int startChunk, int endChunk)
    {
        char[] passwordGuess = new char[passLength];
        int withdrawAmount = 0;

        object bankLock = new object();

        for (int j = startChunk; j < endChunk; j++)
        {
            int passwordIndex = j;

            for (int k = 0; k < passLength; k++)
            {
                passwordGuess[k] = BankOfBitsAndBytes.BankOfBitsNBytes.acceptablePasswordChars[passwordIndex % 26];
                passwordIndex /= 26;
            }

            if (withdrawAmount == 500 || withdrawAmount == -1)
            {
                PasswordGuesser.withdrawAmount = withdrawAmount;
                withdrawAmount = 0;
                return;
            }

            lock (bankLock)
            {
                withdrawAmount = bank.WithdrawMoney(passwordGuess);
            }
        }
    }
}
