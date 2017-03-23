using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using btl.generic;
using alg_greedy;
using System.Collections;

namespace ai_lab_1_GA
{
    public partial class Form1 : Form
    {
        int i = 1;



        static List<int> tasks = new List<int>();
        static List<double> pool = new List<double>();
        static List<List<int>> skillsRes = new List<List<int>>();
        static List<List<int>> skillsTasks = new List<List<int>>();

        static int resourcesN = 0;
        static int taskN = 0;
        static int skillsN = 0;
        static int times = 0;

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

        public static bool hasSkill(int res, int task)
        {
            int requiredSkill = skillsTasks[task].IndexOf(skillsTasks[task].Max());
            int requiredLevel = skillsTasks[task].Max();
            int hasLevel = skillsRes[res][requiredSkill];
            return (hasLevel >= requiredLevel);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            i = 1;
            chart1.Series.Clear();
            pool.Clear();
            times = int.Parse(textBox6.Text);
            for (int t = 0; t < times; t++)
            {
                pool.Add(startGA());
            }



            if (times > 2)
            {
                List<double> copy = new List<double>(pool);
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
                textBox1.AppendText("AVERAGE: " + (tasks.Sum() - pool[average]) + Environment.NewLine);
                textBox1.AppendText("WORST: " + (tasks.Sum() - pool[worst]) + Environment.NewLine);
                textBox1.AppendText("======" + Environment.NewLine);
            }
        }

        private int indexOfAlg(List<double> pool, double x, ref List<bool> flags)
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

        private double startGA()
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

            bool onlyOneGA = (times == 1);

            ga.Go(ref chart1, i, ref textBox1, onlyOneGA);

            int[] values;
            int fitness;      
            ga.GetBest(out values, out fitness);
            textBox1.AppendText("GA" + i + Environment.NewLine);
            textBox1.AppendText("Best time: " + (tasks.Sum() - fitness) + Environment.NewLine);
            textBox1.AppendText("Population: " + populationSize + Environment.NewLine);
            textBox1.AppendText("Generations: " + generations + Environment.NewLine);

            double average = ga.totalAverageFitness();

            i++;

            return average;
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

                int resStartIdx = 0;
                int taskStartIdx = 0;


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
                    if (line.Contains("Number of skill types:"))
                    {
                        skillsN = int.Parse(line.Split("".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[4]);
                        continue;
                    }
                    if (line.Contains("ResourceID"))
                    {
                        resStartIdx = i + 1;
                    }
                    if (line.Contains("TaskID"))
                    {
                        taskStartIdx = i + 1;
                        break;
                    }
                }

                for (int i = 0; i < resourcesN; i++)
                {
                    skillsRes.Add(new List<int>());
                    for (int j = 0; j < skillsN; j++)
                    {
                        skillsRes[i].Add(0);
                    }
                }

                for (int i = 0; i < taskN; i++)
                {
                    skillsTasks.Add(new List<int>());
                    for (int j = 0; j < skillsN; j++)
                    {
                        skillsTasks[i].Add(0);
                    }
                }              
                //assignSkillsTasks(lines, taskStartIdx);
                //assignSkillsRes(lines, taskStartIdx, resStartIdx);

                for (int i = 0; i < taskN; i++)
                {
                    string line = lines[taskStartIdx + i];
                    tasks.Add(int.Parse(line.Split("".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]));
                }

                label4.Visible = true;
                button1.Enabled = true;
                button4.Enabled = true;
            }

        }

        private void assignSkillsTasks(string[] lines, int taskStartIdx)
        {
            int nowRes = 0;

            for (int i = taskStartIdx; i < taskStartIdx + taskN; i++)
            {
                string line = lines[i];
                string[] split = line.Split("".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                nowRes = int.Parse(split[0]) - 1;
                skillsTasks[nowRes][(int)Char.GetNumericValue(split[2][1])] = (int)Char.GetNumericValue(split[3][0]) + 1;
            }
        }


        private void assignSkillsRes(string[] lines, int taskStartIdx, int resStartIdx)
        {
            int nowRes = 0;

            for (int i = resStartIdx; i < taskStartIdx - 2; i++)
            {
                string line = lines[i];
                string[] split = line.Split("".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int splitIdx = 0;
                if (line[0] == 'Q')
                {
                    splitIdx = 0;
                    while (true)
                    {
                        try
                        {
                            skillsRes[nowRes][(int)Char.GetNumericValue(split[splitIdx][1])] = (int)Char.GetNumericValue(split[splitIdx + 1][0]) + 1;
                            splitIdx += 2;
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                else
                {
                    nowRes = int.Parse(split[0]) - 1;
                    splitIdx = 2;
                    while (true)
                    {
                        try
                        {
                            skillsRes[nowRes][(int)Char.GetNumericValue(split[splitIdx][1])] = (int)Char.GetNumericValue(split[splitIdx + 1][0]) + 1;
                            splitIdx += 2;
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
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
