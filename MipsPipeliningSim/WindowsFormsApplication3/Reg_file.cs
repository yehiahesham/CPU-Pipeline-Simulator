using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MipsPipeliningSim
{
    
    class Reg_file
    {
        public int[] regs = new int[17];
        public int result;
        public Reg_file()
        {
            for (int i = 0; i < 16; i++)
                regs[i] = 0;

        }
        public Reg_file(int []A)
        {
            for (int i = 0; i < A.Length; i++)
                regs[i] = A[i];
        }
        public void read(int a)
        {
            if (a != -1)
                result= regs[a]; 
        }
        public void write(int a, int b)
        {
            if(a!=-1)
               regs[a] = b;
        }

    }
    
}
