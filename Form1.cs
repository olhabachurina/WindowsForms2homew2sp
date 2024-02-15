using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms2hw2sp
{

    public partial class Form1 : Form
    {
        private Thread primeThread;
        private Thread fibonacciThread;
        private bool isPrimesRunning = false;
        private bool isFibonacciRunning = false;
        private ManualResetEvent pauseFibonacciEvent = new ManualResetEvent(true);
        private bool stopPrimeThread = false;
        private bool stopFibonacciThread = false;
        public Form1()
        {
            InitializeComponent();
            btnStopPrimes.Enabled = false;
            btnGenerateFibonacci.Enabled = true;
            btnStopFibonacci.Enabled = false;
            btnResumeFibonacci.Enabled = false;
            btnRestartPrimes.Enabled = false;
            btnGeneratePrimes.Click += btnStartPrimes_Click;
            btnGenerateFibonacci.Click += btnStartFibonacci_Click;
            btnStopPrimes.Click += btnStopPrimes_Click;
            btnStopFibonacci.Click += btnStopFibonacci_Click;
            btnPauseFibonacci.Click += btnPauseFibonacci_Click;
            btnResumeFibonacci.Click += btnResumeFibonacci_Click;
        }
        private void btnStartPrimes_Click(object sender, EventArgs e)
        {
            stopPrimeThread = false;
            primeThread = new Thread(() => GeneratePrimeNumbers(
                int.TryParse(txtLowerBound.Text, out int lower) ? lower : 2,
                int.TryParse(txtUpperBound.Text, out int upper) ? upper : int.MaxValue));
            primeThread.Start();
            btnStopPrimes.Enabled = true;
        }
        private void btnStartFibonacci_Click(object sender, EventArgs e)
        {
            stopFibonacciThread = false;
            fibonacciThread = new Thread(GenerateFibonacci);
            fibonacciThread.Start();
            btnStopFibonacci.Enabled = true;
            btnPauseFibonacci.Enabled = true;
            btnResumeFibonacci.Enabled = false;
        }

        private void GeneratePrimeNumbers(int lowerBound, int upperBound)
        {
            for (int i = lowerBound; i <= upperBound && !stopPrimeThread; i++)
            {
                if (IsPrime(i))
                {
                    this.Invoke((MethodInvoker)delegate { lstPrimes.Items.Add(i.ToString()); });
                }
            }
        }

        private void GenerateFibonacci()
        {
            int a = 0, b = 1;
            while (!stopFibonacciThread)
            {
                pauseFibonacciEvent.WaitOne();
                int temp = a;
                a = b;
                b = temp + a;
                this.Invoke((MethodInvoker)delegate { lstFibonacci.Items.Add(temp.ToString()); });
                Thread.Sleep(500);
            }
        }

        private void btnStopPrimes_Click(object sender, EventArgs e)
        {
            stopPrimeThread = true;
            btnStopPrimes.Enabled = false;
        }

        private void btnStopFibonacci_Click(object sender, EventArgs e)
        {
            stopFibonacciThread = true;
            pauseFibonacciEvent.Set(); 
            btnStopFibonacci.Enabled = false;
            btnPauseFibonacci.Enabled = false;
            btnResumeFibonacci.Enabled = false;
        }

        private void btnPauseFibonacci_Click(object sender, EventArgs e)
        {
            pauseFibonacciEvent.Reset();
            btnResumeFibonacci.Enabled = true;
            btnPauseFibonacci.Enabled = false;
        }

        private void btnResumeFibonacci_Click(object sender, EventArgs e)
        {
            pauseFibonacciEvent.Set();
            btnResumeFibonacci.Enabled = false;
            btnPauseFibonacci.Enabled = true;
        }

        private bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;
            int boundary = (int)Math.Sqrt(number);
            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }
}