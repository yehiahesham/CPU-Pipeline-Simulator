using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MipsPipeliningSim
{
    class Memory
    {
        public int[] mem = new int[16];
        public int data_read;
        public Memory()
        {
            for (int i = 0; i < 16; i++)
                mem[i] = 0;

        }
        public Memory(int[] A)
        {
            for (int i = 0; i < A.Length; i++)
                mem[i] = A[i];
        }
        public void read(int a)
        {
            if(a!=-1)
                data_read= mem[a]; 
        }
        public void write(int a, int b)
        {
            if (a != -1)
               mem[a] = b;
        }

    }
}
