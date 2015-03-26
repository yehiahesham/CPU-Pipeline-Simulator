using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MipsPipeliningSim
{
    
    class InstExcute
    {
        public int PC,result;

        public InstExcute()
        { 
        }
        public void instExcute( Reg_file regFile, Memory mem)
        {
           // instDecoded.retreive(inst, regFile);
            //here should also be a control unit

            // ADD 

            if (Form1.Buff_ID_EX.op == 1 && Form1.Buff_ID_EX.func == 0)
            {
                result = Form1.Buff_ID_EX.reg_rs + Form1.Buff_ID_EX.reg_rt;
                PC++;
            }
            //SLT
            if (Form1.Buff_ID_EX.op == 1 && Form1.Buff_ID_EX.func == 3)
            {
                
                if (Form1.Buff_ID_EX.reg_rs < Form1.Buff_ID_EX.reg_rt)
                    result = 1;
                else
                   result = 0;
                    PC++;
            }
            // OR
            if (Form1.Buff_ID_EX.op == 1 && Form1.Buff_ID_EX.func == 1)
            {
                result = Form1.Buff_ID_EX.reg_rs | Form1.Buff_ID_EX.reg_rt;
                PC++;
            }
            // XOR
            if (Form1.Buff_ID_EX.op == 1 && Form1.Buff_ID_EX.func == 2)
            {
                result = Form1.Buff_ID_EX.reg_rs ^ Form1.Buff_ID_EX.reg_rt;
                PC++;
            }
            // ADDI
            if (Form1.Buff_ID_EX.op == 2 && Form1.Buff_ID_EX.func == 0)
            {
                result = Form1.Buff_ID_EX.reg_rs + Form1.Buff_ID_EX.imm;
                PC++;
            }

           

            // SUBI
            if (Form1.Buff_ID_EX.op == 2 && Form1.Buff_ID_EX.func == 1)
            {
                result = Form1.Buff_ID_EX.reg_rs - Form1.Buff_ID_EX.imm;
                PC++;
            }


            // LW
            if (Form1.Buff_ID_EX.op == 2 && Form1.Buff_ID_EX.func == 2)
            {
                // This result should be saved back in the register file in the WB stage but for now we are just calculating its values.
                result = Form1.Buff_ID_EX.reg_rs + Form1.Buff_ID_EX.imm; // result =rt
                PC++;

            }

            // SW
            if (Form1.Buff_ID_EX.op == 2 && Form1.Buff_ID_EX.func == 3)
            {
                //mem.write(Form1.Buff_ID_EX.reg_rs + instDecoded.imm, Form1.Buff_ID_EX.reg_rt);
                result = Form1.Buff_ID_EX.reg_rs + Form1.Buff_ID_EX.imm;
                PC++;
                
            }
            

             // BEQ
            if (Form1.Buff_ID_EX.op == 2 && Form1.Buff_ID_EX.func == 4)
            {
                if (Form1.Buff_ID_EX.reg_rs == Form1.Buff_ID_EX.reg_rt)
                {
                    result = Form1.Buff_ID_EX.imm;
                }
                else
                    PC++;
            }

            // BNE
            if (Form1.Buff_ID_EX.op == 2 && Form1.Buff_ID_EX.func == 5)
            {
                if (Form1.Buff_ID_EX.reg_rs != Form1.Buff_ID_EX.reg_rt)
                {
                    result = Form1.Buff_ID_EX.imm;
                }
                else
                    PC++;
            }

            // BLE
            if (Form1.Buff_ID_EX.op == 2 && Form1.Buff_ID_EX.func == 6)
            {
                if (Form1.Buff_ID_EX.reg_rs <= Form1.Buff_ID_EX.reg_rt)
                {
                    result = Form1.Buff_ID_EX.imm;
                }
                else
                    PC++;
            }
            
            /******************************************* DOesn't NEEDS CHECKING ****************************************************/
            /*
            // J
            if (Form1.Buff_ID_EX.op == 7)
            {
                PC = Form1.Buff_ID_EX.imm;
            }

             // JAL
            if (Form1.Buff_ID_EX.op == 8)
            {
                regFile.write(16,PC + 1);
                PC = Form1.Buff_ID_EX.imm;
            }

            // JR
            if (Form1.Buff_ID_EX.op == 9)
            {
                if (Form1.Buff_ID_EX.raflag == true)
                { 
                    regFile.read(16);
                    PC = regFile.result;
                }
                else
                {
                    regFile.read(Form1.Buff_ID_EX.imm);
                    PC = regFile.result;
                }
            }
             */

        }
    }
}
