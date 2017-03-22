﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using btl.generic;
using alg_greedy;

namespace ai_lab_1_GA
{
    public partial class Form1 : Form
    {
        int i = 1;



        static List<int> tasks = new List<int>();
        static List<int> pool = new List<int>();

        static int resourcesN = 0;
        static int taskN = 0;

        public Form1()
        {
            InitializeComponent();

        }


        public static int theFitnessFunction(int[] values)
        {
            int res = 0;
            int sum = tasks.Sum();
            List<int> resources = new List<int>();
            for (int i = 0; i < resourcesN; i++)
            {
                resources.Add(i);
            }

            for (int i = 0; i < values.Length; i++)
            {
                resources[values[i]] += tasks[i];
            }

            res = resources.Max();

            return sum - res;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            i = 1;
            chart1.Series.Clear();
            pool.Clear();
            int times = int.Parse(textBox6.Text);
            for (int t = 0; t < times; t++)
            {
                pool.Add(startGA());
            }



            if (times > 2)
            {
                List<int> copy = new List<int>(pool);
                List<bool> flags = new List<bool>();
                for (int i = 0; i < pool.Count; i++)
                {
                    flags.Add(false);
                }
                copy.Sort();

                int best = indexOfAlg(pool, pool.Max(), ref flags);
                chart1.Series["GA" + (1 + best)].Name = "Best";


                int worst = indexOfAlg(pool, pool.Min(), ref flags);
                chart1.Series["GA" + (1 + worst)].Name = "Worst";

                int average = indexOfAlg(pool, copy[copy.Count / 2], ref flags);
                chart1.Series["GA" + (1 + average)].Name = "Average";

                for (int a = 0; a < copy.Count; a++)
                {
                    if (a != best && a != worst && a != average)
                        chart1.Series.Remove(chart1.Series["GA" + (a + 1)]);
                }
                textBox1.AppendText("======" + Environment.NewLine);
                textBox1.AppendText("BEST: " + (tasks.Sum() - pool[best]) + Environment.NewLine);
                textBox1.AppendText("AVERAGE: " + (tasks.Sum() - pool[average]) +  Environment.NewLine);
                textBox1.AppendText("WORST: " + (tasks.Sum() - pool[worst]) + Environment.NewLine);
                textBox1.AppendText("======" + Environment.NewLine);
            }
        }

        private int indexOfAlg(List<int> pool, int x, ref List<bool> flags)
        {
            int result = -1;
            for (int i = 0; i < pool.Count; i++)
            {
                if ((pool[i] == x) && (flags[i] == false))
                {
                    result = i;
                    flags[i] = true;
                    break;
                }
            }
            return result;
        }

        private int startGA()
        {
            chart1.Series.Add("GA" + i);
            chart1.Series["GA" + i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            int populationSize = int.Parse(textBox2.Text);
            int generations = int.Parse(textBox3.Text);
            double crossoverRate = double.Parse(textBox4.Text, System.Globalization.CultureInfo.InvariantCulture);
            double mutationRate = double.Parse(textBox5.Text, System.Globalization.CultureInfo.InvariantCulture);
            bool elitism = checkBox1.Checked;

            GA ga = new GA(crossoverRate, mutationRate, populationSize, generations, taskN, resourcesN, true, tasks.Sum());

            ga.FitnessFunction = new GAFunction(theFitnessFunction);

            ga.Elitism = true;
            ga.Go(ref chart1, i, ref textBox1);

            int[] values;
            int fitness;
            ga.GetBest(out values, out fitness);
            textBox1.AppendText("GA" + i + Environment.NewLine);
            textBox1.AppendText("Best time: " + (tasks.Sum() - fitness) + Environment.NewLine);
            textBox1.AppendText("Population: " + populationSize + Environment.NewLine);
            textBox1.AppendText("Generations: " + generations + Environment.NewLine);

            i++;

            return fitness;
        }

        private void startGreedy()
        {
            List<int> results;
            List<int> resources = new List<int>(resourcesN);
            for (int i = 0; i < resourcesN; i++)
            {
                resources.Add(0);
            }
            Greedy gr = new Greedy(resources, tasks);
            gr.Go(out results);
            results.Sort();
            textBox1.AppendText("Greedy:" + results.Last<int>() + Environment.NewLine);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string path = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName;
                string[] lines = System.IO.File.ReadAllLines(path);

                int startIdx = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (line.Contains("Tasks:"))
                    {
                        taskN = int.Parse(line.Split("".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]);
                        continue;
                    }
                    if (line.Contains("Resources:"))
                    {
                        resourcesN = int.Parse(line.Split("".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]);
                        continue;
                    }
                    if (line.Contains("TaskID"))
                    {
                        startIdx = i + 1;
                        break;
                    }
                }

                for (int i = 0; i < taskN; i++)
                {
                    string line = lines[startIdx + i];
                    tasks.Add(int.Parse(line.Split("".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]));
                }

                label4.Visible = true;
                button1.Enabled = true;
                button4.Enabled = true;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            startGreedy();
        }
    }
}