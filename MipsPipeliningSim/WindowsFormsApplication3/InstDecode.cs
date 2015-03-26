using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MipsPipeliningSim ;

namespace MipsPipeliningSim
{
    class InstDecode
    {
        

        // This is what the registers contain .. not the number of the registers.
        public int rs;
        public int rd;
        public int rt;
        public int imm;
        public int op;
        public int func;
        public bool raFlag;
        
        //public string meaning;

        public InstDecode()
        {

        }

        public void retreive( Reg_file regFile)
        {
            
            
            if (Form1.Buff_IF_ID.rs >= 0)
            {
                regFile.read(Form1.Buff_IF_ID.rs);
                rs = regFile.result;
            }
            if (Form1.Buff_IF_ID.rt >= 0)
            {
                regFile.read(Form1.Buff_IF_ID.rt);
                rt = regFile.result;
            }
           
            if (Form1.Buff_IF_ID.imm >= 0)
                imm = Form1.Buff_IF_ID.imm;
            rd = Form1.Buff_IF_ID.rd;
            op = Form1.Buff_IF_ID.op;
            raFlag = Form1.Buff_IF_ID.raflag;
            func = Form1.Buff_IF_ID.func;
            
        }
    }
}
