using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MipsPipeliningSim
{
    
    class memStage
    {
        public Memory x;
        public int output;
        public memStage()
        {
            x = new Memory();
        }

        public memStage(int[] A)
        {
             x = new Memory(A);
        }

       public  void call()
        {
           // a is source(data) + imm
           // b is data of register rt
            if (Form1.Buff_MEM_WB.mem_read == 0) // write
            {
                x.write(Form1.Buff_MEM_WB.execution_result, Form1.Buff_MEM_WB.reg_rt);
            }
            else if (Form1.Buff_MEM_WB.mem_read == 1)
            {
                x.read(Form1.Buff_MEM_WB.execution_result);
                output = x.data_read;
            }
           //else -1, r-Type, Memory block in call function anyways.
        }
    }
}
