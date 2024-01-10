using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickSort
{
    public partial class Form1 : Form
    {
        int arrayLength = 250;
        int[] numberList;
        int minValue = 0;
        int maxValue = 250;
        bool loadedYet = false;
        Thread t1;
        Thread t2;

        int[] states;

        Random random;

        int pivot;

        bool DrawingScrambledLines = true;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Beep(uint dwFreq, uint dwDuration);

        public Form1()
        {
            InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            numberList = new int[arrayLength];
            states = new int[arrayLength];
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            Cursor.Hide();

            this.Invalidate();

            random = new Random(DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Millisecond + DateTime.Now.Hour + DateTime.Now.Minute); //check my program on trev's pc because Random.Next maxNumber is actually the number inputted - 1;
            int newRand;
            for (int a = 0; a < numberList.Length; a++)
            {
                do
                {
                    newRand = random.Next(minValue, maxValue + 1);
                } while (numberList.Contains(newRand));

                numberList[a] = newRand;
            }

            loadedYet = true;
            this.Invalidate();

            pivot = numberList[numberList.Length-1];

            Debug.WriteLine("Original List:");

            foreach (int number in numberList)
            {
                Debug.Write(number + " ");
            }

            Debug.WriteLine("\nNew List: ");

            QuickSort(numberList, 0, numberList.Length - 1);

            Debug.WriteLine("\nDone!");

            Debug.WriteLine("\nThe pivot was: " + pivot);

            Debug.WriteLine("Results: ");

            foreach (int integer in numberList)
            {
                Debug.Write(integer + " ");
            }
        }
        List<Task> tasks1 = new List<Task>();
        Task t;
        float correct;
        async Task QuickSort(int[] array, int start, int end)
        {
            correct = 0;
            for (int i = 0; i < numberList.Length; i++)
            {
                if (numberList[i] == i + 1)
                {
                    correct++;
                }
            }

            correct = correct * 100;
            correct = (float)Math.Round(correct / numberList.Length, 2);

            if (start >= end)
            {
                return;
            }

            int index = await partition(array, start, end);
            states[index] = 0;
            tasks1.Add(Task.Run(() => 
           {
               QuickSort(array, start, index - 1);
               QuickSort(array, index + 1, end);
           }));

            try
            {
                t = Task.WhenAll(tasks1);
                try
                {
                    t.Wait();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Task Wait Error: " + e.Message);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Task WhenAll Error: " + e.Message);
            }
        }

        async Task<int> partition(int[] array, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                states[i] = 2;
            }

            int pivotValue = array[end];
            int pivotIndex = start;
            states[pivotIndex] = 1;
            for (int i = start; i < end; i++)
            {
                if (array[i] < pivotValue)
                {
                    await Swap(array, i, pivotIndex);
                    states[pivotIndex] = 0;
                    pivotIndex++;
                    states[pivotIndex] = 1;
                }
            }
            await Swap(array, pivotIndex, end);

            for (int i = start; i < end; i++)
            {
                if (i != pivotIndex)
                {
                    states[i] = 0;
                }
            }

            return pivotIndex;
        }



        int minFreq = 10000; // Never finished implementing sorting audio
        int maxFreq = 0;

        async Task Swap(int[] array, int a, int b)
        {
            await Task.Delay(35);

            int temp = array[a];
            array[a] = array[b];
            array[b] = temp;

            this.Invalidate();
            DrawingScrambledLines = true;
        }

        //public void Beeper(float value) //may need to intake an int and change it through 2 variables
        //{
        //    float frequency;
        //    int frequencyInt;

        //    frequencyInt = (int)value;

        //    frequency = (float)(value / maxValue);
        //    frequency = frequency * 8000;
        //    frequency += 2000;
        //    Debug.WriteLine("FREQ: " + frequency);
        //    frequencyInt = (int)Math.Floor(frequency);
        //    System.Console.Beep(frequencyInt, 17);

        //    if (frequencyInt < minFreq)
        //    {
        //        minFreq = frequencyInt;
        //    }

        //    if (frequencyInt > maxFreq)
        //    {
        //        maxFreq = frequencyInt;
        //    }
        //}


        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }

            if (e.KeyChar == (char)Keys.Tab)
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        Pen p;
        Pen specialPen1;
        Pen specialPen2;
        void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (loadedYet)
            {
                label1.Text = ("Correct: " + correct + "%").ToString();
                label2.Text = ("MinF: " + minFreq);
                label3.Text = ("MaxF " + maxFreq);
                if (DrawingScrambledLines)
                {
                    float spacing = (this.Width / arrayLength) * 0.5f; 

                    float heighting = 0;
                    Graphics l = e.Graphics;
                    float width = 0.95f;
                    p = new Pen(Color.White, (int)((this.Width / arrayLength) * width));
                    specialPen1 = new Pen(Color.FromArgb(247, 94, 74), (int)((this.Width / arrayLength) * width));
                    specialPen2 = new Pen(Color.FromArgb(128, 255, 162), (int)((this.Width / arrayLength) * width));

                    for (int i = 0; i < numberList.Length; i++)
                    {
                        heighting = numberList[i];

                        heighting = (((heighting / maxValue) * this.Height));
                        heighting = heighting - this.Height;
                        heighting = -heighting;

                        if (states[i] == 1)
                        {
                            l.DrawLine(specialPen1, spacing, this.Height, spacing, heighting);
                        }
                        else if (states[i] == 2)
                        {
                            l.DrawLine(specialPen2, spacing, this.Height, spacing, heighting);
                        }
                        else
                        {
                            l.DrawLine(p, spacing, this.Height, spacing, heighting);
                        }
                        spacing += this.Width / arrayLength;

                    }
                }
            }
        }
    }
}