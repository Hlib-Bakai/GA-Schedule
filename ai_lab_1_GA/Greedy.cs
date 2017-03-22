using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace alg_greedy
{
    class Greedy
    {
        private List<int> m_resources;
        private List<int> m_tasks;
        
        public Greedy(List<int> resources, List<int> tasks)
        {
            m_resources = new List<int>(resources);
            m_tasks = new List<int>(tasks);
        }

        public void Go(out List<int> results)
        {
            m_tasks.Sort();
            results = new List<int>(m_resources);
            int idx = 0;
            int curr = 0;
            for (int i = 0; i < m_tasks.Count; i++)
            {
                curr = m_tasks[i];
                for (int j = 0; j < results.Count; j++)
                {
                    m_resources[j] += results[j] + curr;
                }
                idx = m_resources.IndexOf(m_resources.Min());
                results[idx] += curr;
            }
        }

    }
}
