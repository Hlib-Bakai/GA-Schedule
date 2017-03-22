using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace btl.generic
{

    public delegate int GASFunction(int[] values);

    public class GA_SKILLS
    {
        private double m_mutationRate;
        private double m_crossoverRate;
        private int m_populationSize;
        private int m_generationSize;
        private int m_genomeSize;
        private int m_totalFitness;
        private string m_strFitness;
        private bool m_elitism;
        private int m_resourcesN;
        private int m_sumOfTasks;

        private ArrayList m_thisGeneration;
        private ArrayList m_nextGeneration;
        private ArrayList m_fitnessTable;

        private Genome ellit;

        static Random m_random = new Random();

        static private GAFunction getFitness;



        public GA_SKILLS
            (double crossoverRate,
            double mutationRate,
            int populationSize,
            int generationSize,
            int genomeSize,
            int resourcesN,
            bool elitism,
            int sumOfTasks)
        {
            m_mutationRate = mutationRate;
            m_crossoverRate = crossoverRate;
            m_populationSize = populationSize;
            m_generationSize = generationSize;
            m_genomeSize = genomeSize;
            m_resourcesN = resourcesN;
            m_elitism = elitism;
            m_sumOfTasks = sumOfTasks;
            m_strFitness = "";
        }



        public void Go(ref Chart chart1, int iGlobal, ref TextBox textbox1)
        {
            if (getFitness == null)
                throw new ArgumentNullException("No fitness function");
            if (m_genomeSize == 0)
                throw new IndexOutOfRangeException("Genome size not set");

            m_fitnessTable = new ArrayList();
            m_thisGeneration = new ArrayList(m_generationSize);
            m_nextGeneration = new ArrayList(m_generationSize);
            Genome.MutationRate = m_mutationRate;

            CreateGenomes();
            RankPopulation();



            bool write = false;

            for (int i = 0; i < m_generationSize; i++)
            {
                ellit = new Genome((Genome)m_thisGeneration[m_populationSize - 1], m_mutationRate);
                CreateNextGeneration();
                RankPopulation();
                int d;
                d = ((Genome)m_thisGeneration[m_populationSize - 1]).Fitness;
                if (write)
                {
                    textbox1.AppendText(i + ": " + (m_sumOfTasks - d));
                    textbox1.AppendText(Environment.NewLine);
                }
                chart1.Series["GA" + iGlobal].Points.AddXY(i, m_sumOfTasks - d);
            }

            //////


        }

        double averageFitness(ArrayList array)
        {
            double result = 0;
            foreach (Genome gen in array)
            {
                result += gen.Fitness;
            }
            result /= array.Count;
            return result;
        }

        private int RouletteSelection()
        {
            int randomFitness = (int)(m_random.NextDouble() * m_totalFitness);
            int idx = -1;
            int mid;
            int first = 0;
            int last = m_populationSize - 1;
            mid = (last - first) / 2;

            while (idx == -1 && first <= last)
            {
                if (randomFitness < (int)m_fitnessTable[mid])
                {
                    last = mid;
                }
                else if (randomFitness > (int)m_fitnessTable[mid])
                {
                    first = mid;
                }
                else
                {
                    idx = mid;
                }
                mid = (first + last) / 2;
                if ((last - first) == 1 || last == first)
                    idx = last;
            }
            return idx;
        }


        private void RankPopulation()
        {
            m_totalFitness = 0;
            for (int i = 0; i < m_populationSize; i++)
            {
                Genome g = ((Genome)m_thisGeneration[i]);
                g.Fitness = FitnessFunction(g.Genes());
                m_totalFitness += g.Fitness;
                ///
                ///
			}
            m_thisGeneration.Sort(new GenomeComparer());

            int fitness = 0;
            m_fitnessTable.Clear();

            if (ellit != null && m_elitism && ((Genome)m_thisGeneration[m_populationSize - 1]).Fitness < ellit.Fitness)
            {
                m_thisGeneration[m_populationSize - 1] = ellit;
            }

            for (int i = 0; i < m_populationSize; i++)
            {
                fitness += ((Genome)m_thisGeneration[i]).Fitness;
                m_fitnessTable.Add(fitness);
            }
        }


        private void CreateGenomes()
        {
            for (int i = 0; i < m_populationSize; i++)
            {
                Genome g = new Genome(m_genomeSize, m_resourcesN);
                m_thisGeneration.Add(g);
            }
        }

        private void CreateNextGeneration()
        {
            m_nextGeneration.Clear();
            for (int i = 0; i < m_populationSize; i += 2)
            {
                int pidx1 = RouletteSelection();
                int pidx2 = RouletteSelection();
                Genome parent1, parent2, child1, child2;
                parent1 = ((Genome)m_thisGeneration[pidx1]);
                parent2 = ((Genome)m_thisGeneration[pidx2]);

                if (m_random.NextDouble() < m_crossoverRate)
                {
                    parent1.Crossover(ref parent2, out child1, out child2);
                }
                else
                {
                    child1 = parent1;
                    child2 = parent2;
                }
                child1.Mutate();
                child2.Mutate();

                m_nextGeneration.Add(child1);
                m_nextGeneration.Add(child2);
            }

            m_thisGeneration.Clear();
            for (int i = 0; i < m_populationSize; i++)
                m_thisGeneration.Add(m_nextGeneration[i]);
        }

        /// <summary>
        /// 
        /// 
        /// 
        /// 
        ///                                 SOME STAFF
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>

        public GAFunction FitnessFunction
        {
            get
            {
                return getFitness;
            }
            set
            {
                getFitness = value;
            }
        }


        public int PopulationSize
        {
            get
            {
                return m_populationSize;
            }
            set
            {
                m_populationSize = value;
            }
        }

        public int Generations
        {
            get
            {
                return m_generationSize;
            }
            set
            {
                m_generationSize = value;
            }
        }

        public int GenomeSize
        {
            get
            {
                return m_genomeSize;
            }
            set
            {
                m_genomeSize = value;
            }
        }

        public double CrossoverRate
        {
            get
            {
                return m_crossoverRate;
            }
            set
            {
                m_crossoverRate = value;
            }
        }
        public double MutationRate
        {
            get
            {
                return m_mutationRate;
            }
            set
            {
                m_mutationRate = value;
            }
        }

        public string FitnessFile
        {
            get
            {
                return m_strFitness;
            }
            set
            {
                m_strFitness = value;
            }
        }


        public bool Elitism
        {
            get
            {
                return m_elitism;
            }
            set
            {
                m_elitism = value;
            }
        }

        public void GetBest(out int[] values, out int fitness)
        {
            Genome g = ((Genome)m_thisGeneration[m_populationSize - 1]);
            values = new int[g.Length];
            g.GetValues(ref values);
            fitness = (int)g.Fitness;
        }

        public void GetWorst(out int[] values, out int fitness)
        {
            GetNthGenome(0, out values, out fitness);
        }

        public void GetNthGenome(int n, out int[] values, out int fitness)
        {
            if (n < 0 || n > m_populationSize - 1)
                throw new ArgumentOutOfRangeException("n too large, or too small");
            Genome g = ((Genome)m_thisGeneration[n]);
            values = new int[g.Length];
            g.GetValues(ref values);
            fitness = (int)g.Fitness;
        }
    }
}
