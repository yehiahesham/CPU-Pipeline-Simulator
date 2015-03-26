using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MipsPipeliningSim
{
    class Wb_Stage
    {
        
        public Reg_file y;
        public Wb_Stage()
        { 
            y= new Reg_file();
        }
        public Wb_Stage(int []A)
        {
            y = new Reg_file(A);
        }

        public void write_back() //
        {
            
            //b alu result
            //c meoery read output.
            //when he enter wb , print WB block.
            if (Form1.Buff_MEM_WB.mem_read == -1)
            {
                if ((Form1.Buff_MEM_WB.op == 2) && ((Form1.Buff_MEM_WB.func == 0) || (Form1.Buff_MEM_WB.func == 1)))
                    y.write(Form1.Buff_MEM_WB.rt, Form1.Buff_MEM_WB.execution_result); // for r-types and SUBI and ADDI
                else
                    y.write(Form1.Buff_MEM_WB.rd, Form1.Buff_MEM_WB.execution_result); 
            }
            else if (Form1.Buff_MEM_WB.mem_read == 1)
            {
                if ((Form1.Buff_MEM_WB.op == 2) && (Form1.Buff_MEM_WB.func == 2))
                    y.write(Form1.Buff_MEM_WB.rt, Form1.Buff_MEM_WB.Mem_result);
                else
                    y.write(Form1.Buff_MEM_WB.rd, Form1.Buff_MEM_WB.Mem_result);
            }
        }
    }
}
