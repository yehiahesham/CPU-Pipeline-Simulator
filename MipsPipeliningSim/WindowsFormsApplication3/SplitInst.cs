using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MipsPipeliningSim
{
    class SplitInst
    {
        
        public int distination , source, target, imm,op,func,memRead;
        public bool raflag=false;
        public int currentPC=0;
        
        public SplitInst()
        {
             
        }
        public void getInst(string ins)
        {
            /* R, op=1. functions from 0 to 3
            I, op=2.function from 0 to 6
            J, op=3,4,5,6,7,8,9
             */
             string[] words = ins.Split(' ');
             switch (words[0])
             {

                 case "ADD":  //ADD,r
                     {
                         distination = Convert.ToInt32(words[1]);
                         source = Convert.ToInt32(words[2]);
                         target = Convert.ToInt32(words[3]);
                         op = 1;
                         func = 0;
                         imm = -1;
                         memRead = -1;
                         break;
                     }
                 case "ADDI":  //ADDi,i
                     {

                         target = Convert.ToInt32(words[1]);
                         source = Convert.ToInt32(words[2]);
                         imm = Convert.ToInt32(words[3]);
                         op = 2;
                         func = 0;
                         distination = -1;
                         memRead = -1;
                         break;
                     }


                 case "SUBI":  //SUBI
                     {
                         target = Convert.ToInt32(words[1]);
                         source = Convert.ToInt32(words[2]);
                         imm = Convert.ToInt32(words[3]);
                         op = 2;
                         func = 1;
                         distination = -1; 
                         memRead = -1;
                         break;
                     }

                 case "OR":   //OR
                     {
                         distination = Convert.ToInt32(words[1]);
                         source = Convert.ToInt32(words[2]);
                         target = Convert.ToInt32(words[3]);
                         op = 1;
                         func = 1;
                         imm = -1;
                         memRead = -1;
                         break;
                     }
                 case "XOR":   //XOR
                     {
                         distination = Convert.ToInt32(words[1]);
                         source = Convert.ToInt32(words[2]);
                         target = Convert.ToInt32(words[3]);
                         op = 1;
                         func = 2;
                         imm = -1;
                        
                         memRead = -1;
                         break;
                     }

                 case "LW":   //LW,i
                     {
                         target = Convert.ToInt32(words[1]);
                         string[] temp = words[2].Split('(');
                         imm = Convert.ToInt32(temp[0]);
                         string[] temp2 = temp[1].Split(')');
                         source = Convert.ToInt32(temp2[0]);  //Extracts substring between the brackets
                         op = 2;
                         func = 2;
                         distination = -1;
                         memRead = 1;
                         break;
                     }

                 case "SW":   //SW,i
                     {
                         target = Convert.ToInt32(words[1]);
                         string[] temp = words[2].Split('(');
                         imm = Convert.ToInt32(temp[0]);
                         string[] temp2 = temp[1].Split(')');
                         source = Convert.ToInt32(temp2[0]);  //Extracts substring between the brackets
                         op = 2;
                         func = 3;
                         distination = -1;
                         memRead = 0;
                         break;
                     }

                 case "BEQ":  //Branch equal,i
                     {
                         source = Convert.ToInt32(words[1]);
                         target = Convert.ToInt32(words[2]);
                         imm = Convert.ToInt32(words[3]);
                         op = 2;
                         func = 4;
                         distination = -1;
                         memRead = -1;
                         break;
                     }

                 case "BNE":  //Branch not equal,i
                     {
                         source = Convert.ToInt32(words[1]);
                         target = Convert.ToInt32(words[2]);
                         imm = Convert.ToInt32(words[3]);
                         op = 2;
                         func = 5;
                        
                         distination = -1;
                         memRead = -1;
                         break;
                     }
                 case "BLE":  //branch on less than or equal
                     {
                         // ble $t0, $t1, Lab2 # Branch if $t0 <= $t1 bgt $t0
                         source = Convert.ToInt32(words[1]);
                         target = Convert.ToInt32(words[2]);
                         imm = Convert.ToInt32(words[3]);
                         op = 2;
                         func = 6;
                        
                         distination = -1;
                         memRead = -1;
                         break;
                     }

                 case "J":  //Jump,j
                     {
                         op = 7;
                         imm = Convert.ToInt32(words[1]);
                        
                         source = -1;
                         distination = -1;
                         target = -1;
                         func = -1;
                         memRead = -1;
                         break;
                     }
                 case "JAL":  //Jump & link
                     {
                         op = 8;
                         imm = Convert.ToInt32(words[1]);
                        
                         source = -1;
                         distination = -1;
                         target = -1;
                         func = -1;
                         memRead = -1;
                         break;
                     }
                 case "JR":  //Jump register
                     {
                         op = 9;
                        
                         source = -1;
                         distination = -1;
                         target = -1;
                         func = -1;
                         memRead = -1;
                         if (words[1] == "ra")
                         {
                             raflag = true;

                         }
                         else
                         {
                             imm = Convert.ToInt32(words[1]);
                         }
                         break;
                     }

                 case "SLT":   //SLT,r
                     {
                         op = 1;
                         func = 3;
                         distination = Convert.ToInt32(words[1]);
                         source = Convert.ToInt32(words[2]);
                         target = Convert.ToInt32(words[3]);
                         imm = -1;
                         memRead = -1;
                         break;
                     }
             }
        }
    
    }
}
