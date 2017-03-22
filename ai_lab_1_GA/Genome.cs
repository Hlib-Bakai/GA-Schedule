using System;
using System.Collections;
using btl.generic;

namespace btl.generic
{
	public class Genome
	{
        public int[] m_genes;
        private int m_length;
        private int m_fitness;
        private int m_resourcesN;
        static Random m_random = new Random();
        public static double m_mutationRate;

        public Genome(int length, int resourcesN)
		{
			m_length = length;
            m_resourcesN = resourcesN;
            m_genes = new int[ length ];
			CreateGenes();
		}

        public Genome(Genome copyFrom, double mutRate)
        {
            m_fitness = copyFrom.Fitness;
            m_genes = copyFrom.m_genes;
            m_length = copyFrom.m_length;
            m_resourcesN = copyFrom.m_resourcesN;
            m_mutationRate = mutRate;
        }

		public Genome(int length, int resourcesN, bool createGenes)
		{
			m_length = length;
            m_resourcesN = resourcesN;
            m_genes = new int[ length ];
			if (createGenes)
				CreateGenes();
		}
		 

		private void CreateGenes()
		{
			for (int i = 0 ; i < m_length ; i++)
				m_genes[i] = m_random.Next(0, m_resourcesN);
		}

		public void Crossover(ref Genome genome2, out Genome child1, out Genome child2)
		{
			int pos = (int)(m_random.NextDouble() * m_length);
			child1 = new Genome(m_length, m_resourcesN, false);
			child2 = new Genome(m_length, m_resourcesN, false);
			for(int i = 0 ; i < m_length ; i++)
			{
				if (i < pos)
				{
					child1.m_genes[i] = m_genes[i];
					child2.m_genes[i] = genome2.m_genes[i];
				}
				else
				{
					child1.m_genes[i] = genome2.m_genes[i];
					child2.m_genes[i] = m_genes[i];
				}
			}
		}


		public void Mutate()
		{
			for (int pos = 0 ; pos < m_length; pos++)
			{
                if (m_random.NextDouble() < m_mutationRate)
                    m_genes[pos] = m_random.Next(0, m_resourcesN);
			}
		}

		public int[] Genes()
		{
			return m_genes;
		}


		public void GetValues(ref int[] values)
		{
			for (int i = 0 ; i < m_length ; i++)
				values[i] = m_genes[i];
		}


		public int Fitness
		{
			get
			{
				return m_fitness;
			}
			set
			{
				m_fitness = value;
			}
		}




		public static double MutationRate
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

		public int Length
		{
			get
			{
				return m_length;
			}
		}
	}
}
