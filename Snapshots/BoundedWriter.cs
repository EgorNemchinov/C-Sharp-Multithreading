using System.Collections.Generic;
using System.Linq;

namespace Snapshots
{
    public class BoundedWriter
    {
        private const int RegistersAmount = 2;

        private Register[] regs;
        private bool[,] q;
        private IEnumerable<int> range;

        class Register
        {
            public int value;
            public bool toggle;
            public bool[] p;
            public int[] snapshot;

            public Register(int registersAmount)
            {
                p = new bool[registersAmount];
                snapshot = new int[registersAmount];
            }

            public void Update(int value, bool[] p, int[] snapshot)
            {
                this.value = value;
                this.p = p;
                this.snapshot = snapshot;
                toggle = !toggle;
            }
        }

        public BoundedWriter()
        {
            regs = new Register[RegistersAmount];
            q = new bool[RegistersAmount, RegistersAmount];
            range = Enumerable.Range(0, RegistersAmount - 1);

            for (int i = 0; i < RegistersAmount; i++)
                regs[i] = new Register(RegistersAmount);
        }

        private int[] Scan(int i)
        {
            var moved = new int[RegistersAmount];

            while (true)
            {
                for (int j = 0; j < RegistersAmount; j++)
                    q[i, j] = regs[j].p[i];
                var a = Collect();
                var b = Collect();

                var allUnchanged = range.All(
                    (j) => a[j].p[i] == q[i, j] &
                           a[j].p[i] == q[i, j] &
                           a[j].toggle == b[j].toggle
                );
                
                if (allUnchanged)
                {
                    return b.Select((reg) => reg.value).ToArray();
                }
                else
                {
                    var changedRegs = range.Where(
                        (j) => a[j].p[i] != q[i, j] ||
                        a[j].p[i] != q[i, j] ||
                        a[j].toggle != b[j].toggle)
                        .ToList();
                    
                    foreach (var j in changedRegs)
                    {
                        if (moved[j] == 1)
                            return b[j].snapshot;
                        else
                            moved[j]++;
                    }
                }
            }
        }

        public void Update(int i, int value)
        {
            bool[] f = new bool[RegistersAmount];
            
            for (int j = 0; j < RegistersAmount; j++)
                f[j] = !q[j, i];
            
            var snapshot = Scan(i);

            regs[i].Update(value, f, snapshot);
        }

        private Register[] Collect()
        {
            Register[] result = new Register[RegistersAmount];
            regs.CopyTo(result, 0);
            return result;
        }
    }
}
