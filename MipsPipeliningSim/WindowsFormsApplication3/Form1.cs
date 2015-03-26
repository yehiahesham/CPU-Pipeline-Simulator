using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
namespace MipsPipeliningSim
{
    //adel

    public partial class Form1 : Form
    {
        public struct IF_B_ID
        {
            public int mem_read, rs, rt, rd, pc, op, func, imm;
            public bool raflag;
        };
         public struct ID_B_EX
        {
            public int mem_read, rs, rt, rd, pc;
            public int reg_rs, reg_rt,imm, op, func;
            public bool raflag;
        };
         public struct EX_B_MEM
        {
             public int mem_read, execution_result, rd,rs, rt, op, func, reg_rs, reg_rt;
        };
         public struct MEM_B_WB
        {
             public int mem_read, Mem_result, execution_result, op, func, rd, rt, reg_rs, reg_rt;
        };


        SplitInst IF = new SplitInst();
        InstDecode ID = new InstDecode();
        Wb_Stage RF = new Wb_Stage(new int[10] {0,0,4,0,0,10,0,9,6,0});
        memStage M = new memStage(new int[5] {0,0,0,0,5});
        InstExcute IEx = new InstExcute();
        string instruction;
        public int x=330;
        public int y=150;
        public string text;
        public int PC = 0, counter = 0, counter2=0;
        public int Clock_cycles = 0;
        public string temp;
        ArrayList btnlist = new ArrayList();
        public bool stall1;
        public bool stall2;
        public bool if_stall=false;
        public bool id_stall = false;
        public string t;
        public bool mem_stall=false;
        public bool b_j_flag = false;
        public bool inc_before = false;
        public bool b_j_2flag = false;
        public bool finished_flag = false;
        public bool inc_in_ex = false;
        public bool inc_inside_b_j = false;
        public static IF_B_ID Buff_IF_ID = new IF_B_ID ();
        public static ID_B_EX Buff_ID_EX = new ID_B_EX();//= new List<int>();
        public static EX_B_MEM Buff_EX_MEM = new EX_B_MEM();//= new List<int>();
        public static MEM_B_WB Buff_MEM_WB = new MEM_B_WB();//= new List<int>();fstall

        List<string> pipelining = new List<string>();
       

        //_______________________________________________________________________Forwarding unit

        void Branch_control_unit()
        {
            /*
                  op = 9; // JR 
                  op = 8; //JAL
                  op = 7; //J
                  Buff_ID_EX.func:
                  5 -> BNE,  6->BLE;   4 -> BEQ;
                 */
            if (b_j_flag == true)
            { b_j_flag = false; b_j_2flag = true; }
            switch (Buff_ID_EX.op)
            {
                case 7:
                    IF.currentPC = Buff_ID_EX.imm;
                    b_j_flag = true;
                    b_j_2flag = false;
                    inc_inside_b_j=true;
                    break;
                case 8:
                    RF.y.regs[16] = IF.currentPC+1;
                    IF.currentPC = Buff_ID_EX.imm;
                    b_j_flag = true;
                    b_j_2flag = false;
                    inc_inside_b_j = true;
                    break;
                case 9:
                    IF.currentPC = RF.y.regs[16];
                    RF.y.regs[16] = 0;
                    b_j_flag = true;
                    b_j_2flag = false;
                    inc_inside_b_j = true;
                    break;
                case 2:
                    switch (Buff_ID_EX.func)
                    {
                        case 5:
                            if (Buff_ID_EX.reg_rs != Buff_ID_EX.reg_rt)
                            {   //IF.currentPC = Buff_ID_EX.imm; 
                                b_j_flag = true; 
                                b_j_2flag = false;
                                inc_inside_b_j = true; }
                            break;
                        case 6:
                            if (Buff_ID_EX.reg_rs <= Buff_ID_EX.reg_rt)
                            { //IF.currentPC = Buff_ID_EX.imm;
                                b_j_flag = true; b_j_2flag = false; inc_inside_b_j = true; }
                            break;
                        case 4:
                            if (Buff_ID_EX.reg_rs == Buff_ID_EX.reg_rt)
                            { //IF.currentPC = Buff_ID_EX.imm; 
                                b_j_flag = true; b_j_2flag = false; inc_inside_b_j = true; }
                            break;
                    }
                    
                    if (IF.currentPC == listBox1.Items.Count - 1)
                        finished_flag = true;
                    else
                        finished_flag = false;
                    break;
            }

            reg_update();
        }
        void Forwarding_unit()
        {
            /////////// in1
            if (Buff_ID_EX.op == 1) // R
            {
                if (Buff_ID_EX.rs == Buff_EX_MEM.rd) // if hazards with R-type
                    Buff_ID_EX.reg_rs = Buff_EX_MEM.execution_result;

                if (Buff_ID_EX.rt == Buff_EX_MEM.rd)  // if hazards with R-type
                    Buff_ID_EX.reg_rt = Buff_EX_MEM.execution_result;

                if (Buff_EX_MEM.op == 2) // if hazards with I-type
                {
                    if (Buff_ID_EX.rs == Buff_EX_MEM.rt)
                    {
                        if (Buff_EX_MEM.func == 0 || Buff_EX_MEM.func == 1) // ADDI , SUBI
                        {
                            Buff_ID_EX.reg_rs = Buff_EX_MEM.execution_result;
                            stall1 = false;
                        }
                        else if (Buff_EX_MEM.op == 2 && Buff_EX_MEM.func == 2) // lw
                        {
                            Buff_ID_EX.reg_rs = Buff_MEM_WB.mem_read;
                            stall1 = true;
                        }
                    }
                    else if (Buff_ID_EX.rt == Buff_EX_MEM.rt)
                    {
                        if (Buff_EX_MEM.func == 0 || Buff_EX_MEM.func == 1) // ADDI , SUBI
                        {
                            Buff_ID_EX.reg_rt = Buff_EX_MEM.execution_result;
                            stall1 = false;
                        }
                        else if (Buff_EX_MEM.func == 2) // lw
                        {
                            Buff_ID_EX.reg_rt = Buff_MEM_WB.mem_read;
                            stall1 = true;
                        }

                    }
                }

            }

            else if (Buff_ID_EX.op == 2) //I
            {
                if (Buff_ID_EX.rs == Buff_EX_MEM.rd) // if hazards with R-type
                    Buff_ID_EX.reg_rs = Buff_EX_MEM.execution_result;
                if (Buff_ID_EX.rt == Buff_EX_MEM.rd) // if hazards with R-type
                    Buff_ID_EX.reg_rt = Buff_EX_MEM.execution_result;

                if (Buff_EX_MEM.op == 2)// if hazards with I-type
                {
                    if (Buff_ID_EX.rs == Buff_EX_MEM.rt)
                    {
                        if (Buff_EX_MEM.func == 0 || Buff_EX_MEM.func == 1) // ADDI , SUBI
                        {
                            Buff_ID_EX.reg_rs = Buff_EX_MEM.execution_result;
                            stall2 = false;
                        }
                        else if (Buff_EX_MEM.func == 2) // lw
                        {
                            Buff_ID_EX.reg_rs = Buff_MEM_WB.mem_read;
                            stall2 = true;
                        }
                    }
                    else if (Buff_ID_EX.rt == Buff_EX_MEM.rt)
                    {
                        if (Buff_EX_MEM.func == 0 || Buff_EX_MEM.func == 1) // ADDI , SUBI
                        {
                            Buff_ID_EX.reg_rt = Buff_EX_MEM.execution_result;
                            stall2 = false;
                        }
                        else if (Buff_EX_MEM.func == 2) // lw
                        {
                            Buff_ID_EX.reg_rt = Buff_MEM_WB.mem_read;
                            stall2 = true;
                        }
                    }
                }


            }
            /////////// in2
            if (Buff_ID_EX.op == 1) // R
            {
                if (Buff_ID_EX.rs == Buff_MEM_WB.rd) // if hazards with R-type
                    Buff_ID_EX.reg_rs = Buff_MEM_WB.execution_result;

                if (Buff_ID_EX.rt == Buff_MEM_WB.rd)  // if hazards with R-type
                    Buff_ID_EX.reg_rt = Buff_MEM_WB.execution_result;

                if (Buff_MEM_WB.op == 2) // if hazards with I-type
                {
                    if (Buff_ID_EX.rs == Buff_MEM_WB.rt)
                    {
                        if (Buff_MEM_WB.func == 0 || Buff_MEM_WB.func == 1) // ADDI , SUBI
                        {
                            Buff_ID_EX.reg_rs = Buff_MEM_WB.execution_result;
                            stall1 = false;
                        }
                        else if (Buff_MEM_WB.func == 2) // lw
                        {
                            Buff_ID_EX.reg_rs = Buff_MEM_WB.mem_read;
                            stall1 = true;
                        }
                    }
                    else if (Buff_ID_EX.rt == Buff_MEM_WB.rt)
                    {
                        if (Buff_MEM_WB.func == 0 || Buff_MEM_WB.func == 1) // ADDI , SUBI
                        {
                            Buff_ID_EX.reg_rt = Buff_MEM_WB.execution_result;
                            stall1 = false;
                        }
                        else if (Buff_EX_MEM.func == 2) // lw
                        {
                            Buff_ID_EX.reg_rt = Buff_MEM_WB.mem_read;
                            stall1 = true;
                        }

                    }
                }

            }

            else if (Buff_ID_EX.op == 2) //I
            {
                if (Buff_ID_EX.rs == Buff_MEM_WB.rd) // if hazards with R-type
                    Buff_ID_EX.reg_rs = Buff_EX_MEM.execution_result;
                if (Buff_ID_EX.rt == Buff_MEM_WB.rd) // if hazards with R-type
                    Buff_ID_EX.reg_rt = Buff_EX_MEM.execution_result;

                if (Buff_MEM_WB.op == 2)// if hazards with I-type
                {
                    if (Buff_ID_EX.rs == Buff_MEM_WB.rt)
                    {
                        if (Buff_MEM_WB.func == 0 || Buff_MEM_WB.func == 1) // ADDI , SUBI
                        {
                            Buff_ID_EX.reg_rs = Buff_MEM_WB.execution_result;
                            stall2 = false;
                        }
                        else if (Buff_MEM_WB.func == 2) // lw
                        {
                            Buff_ID_EX.reg_rs = Buff_MEM_WB.mem_read;
                            stall2 = true;
                        }
                    }
                    else if (Buff_ID_EX.rt == Buff_MEM_WB.rt)
                    {
                        if (Buff_MEM_WB.func == 0 || Buff_MEM_WB.func == 1) // ADDI , SUBI
                        {
                            Buff_ID_EX.reg_rt = Buff_MEM_WB.execution_result;
                            stall2 = false;
                        }
                        else if (Buff_MEM_WB.func == 2) // lw
                        {
                            Buff_ID_EX.reg_rt = Buff_MEM_WB.mem_read;
                            stall2 = true;
                        }
                    }
                }


            }
        }

        void fetch()
        {

            listBox1.SetSelected(IF.currentPC, true);
            read_inst();
            IF.getInst(instruction);
            ////////// updating buffers 
            Buff_IF_ID.mem_read = IF.memRead;
            Buff_IF_ID.pc = IF.currentPC;
            Buff_IF_ID.rd = IF.distination;
            Buff_IF_ID.rs = IF.source;
            Buff_IF_ID.rt = IF.target;
            Buff_IF_ID.op = IF.op;
            Buff_IF_ID.func = IF.func;
            Buff_IF_ID.imm = IF.imm;
            Buff_IF_ID.raflag = IF.raflag;
            
                
            //////////
            string text = "IF";
            createLabel(x, y, text);
        }

        void decode()
        {
            ////////// updating buffers 
            
            Buff_ID_EX.mem_read = Buff_IF_ID.mem_read;
            Buff_ID_EX.op=Buff_IF_ID.op;
            Buff_ID_EX.func= Buff_IF_ID.func;
            Buff_ID_EX.pc=Buff_IF_ID.pc;
            Buff_ID_EX.rs=Buff_IF_ID.rs;
            Buff_ID_EX.rt = Buff_IF_ID.rt;
            Buff_ID_EX.rd = Buff_IF_ID.rd;
            Buff_ID_EX.imm = Buff_IF_ID.imm;
            Buff_ID_EX.raflag = Buff_IF_ID.raflag;
         
            //////////
            ID.retreive(RF.y);
            
                //////////// updating buffers 
                Buff_ID_EX.reg_rs = ID.rs;
                Buff_ID_EX.reg_rt = ID.rt;
            
            ////////// branch control 
                Branch_control_unit();
            /////////// Forwarding unit
                Forwarding_unit();
         

            


        }

        void execute()
        {
  //////////// updating buffers 
            Buff_EX_MEM.mem_read = Buff_ID_EX.mem_read;
            Buff_EX_MEM.rd = Buff_ID_EX.rd;
            Buff_EX_MEM.rt = Buff_ID_EX.rt; 
            Buff_EX_MEM.rs = Buff_ID_EX.rs;
            Buff_EX_MEM.reg_rt = Buff_ID_EX.reg_rt ;
            Buff_EX_MEM.reg_rs = Buff_ID_EX.reg_rs;
            Buff_EX_MEM.op = Buff_ID_EX.op;
            Buff_EX_MEM.func = Buff_ID_EX.func;
            //////////


                IEx.instExcute(RF.y, M.x);
           
                //////////// updating buffers 
                Buff_EX_MEM.execution_result = IEx.result;
                if (b_j_flag)
                {
                    IF.currentPC = IEx.result;
                    inc_in_ex = true;
                }
            
            //////////

            string text = "EX";
            createLabel(x, y, text);
            
        }

        void memory()
        {
            //////////// updating buffers 
            Buff_MEM_WB.mem_read = Buff_EX_MEM.mem_read;
            Buff_MEM_WB.execution_result = Buff_EX_MEM.execution_result;
            Buff_MEM_WB.op = Buff_EX_MEM.op;
            Buff_MEM_WB.func= Buff_EX_MEM.func;
            Buff_MEM_WB.rd = Buff_EX_MEM.rd;
            Buff_MEM_WB.rt = Buff_EX_MEM.rt;
            Buff_MEM_WB.reg_rt = Buff_EX_MEM.reg_rt;
            Buff_MEM_WB.reg_rs = Buff_EX_MEM.reg_rs;
            

            //////////

            M.call();

            //////////// updating buffers 
            Buff_MEM_WB.Mem_result = M.output;
            //////////

            string text = "MEM";
            createLabel(x, y, text);
        }

        void writeback()
        {
            RF.write_back();
            string text = "WB";
            createLabel(x, y, text);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        { 

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Browse for Instructions File";
            ofd.Filter = "Text file|*.txt";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            { 
                StreamReader sr = new StreamReader(File.OpenRead(ofd.FileName));
                while(!sr.EndOfStream)
                {
                    listBox1.Items.Add(sr.ReadLine());
                }
                sr.Dispose();
            }
            fetch();
            PC++; pipelining.Add("IF");
           
  
        } // browse

        public string read_inst()
        {

            if (IF.currentPC >= listBox1.Items.Count)
            {
                MessageBox.Show("No instruction to fetch!");
                return "";
            }
            else
            {
                

                textBox34.Text = (IF.currentPC ).ToString(); //(Convert.ToInt32(textBox10.Text) + 1).ToString();
                instruction = listBox1.Items[IF.currentPC ].ToString();
                textBox33.Text = instruction;
                return instruction;
            }

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            
            listBox1.Items.Clear();
            for (int i = 0; i < 16; i++)
            {
                RF.y.regs[i]=0;
                M.x.mem[i] =0;
            }
            IF.currentPC = 0;
            IEx.PC = 0;
            x=330;
            y=150;
            delete();
            instruction = "";
            textBox1.Text = "0";
            textBox2.Text = "0";
            textBox3.Text = "0";
            textBox4.Text = "0";
            textBox5.Text = "0";
            textBox6.Text = "0";
            textBox7.Text = "0";
            textBox8.Text = "0";
            textBox9.Text = "0";
            textBox10.Text = "0";
            textBox11.Text = "0";
            textBox12.Text = "0";
            textBox13.Text = "0";
            textBox14.Text = "0";
            textBox15.Text = "0";
            textBox16.Text = "0";
            textBox17.Text = "0";
            textBox18.Text = "0";
            textBox19.Text = "0";
            textBox20.Text = "0";
            textBox21.Text = "0";
            textBox22.Text = "0";
            textBox23.Text = "0";
            textBox24.Text = "0";
            textBox25.Text = "0";
            textBox26.Text = "0";
            textBox27.Text = "0";
            textBox28.Text = "0";
            textBox29.Text = "0";
            textBox30.Text = "0";
            textBox31.Text = "0";
            textBox32.Text = "0";
            textBox33.Text = "";
            textBox34.Text = "0";
            textBox35.Text = "";
           

        } //reset
        public void buffers()
        {
            textBox36.Text = Buff_ID_EX.func.ToString();
            textBox37.Text = Buff_ID_EX.op.ToString();
            textBox38.Text = Buff_ID_EX.imm.ToString();
            textBox39.Text = Buff_ID_EX.mem_read.ToString();
            textBox40.Text = Buff_ID_EX.rd.ToString();
            textBox41.Text = Buff_ID_EX.rs.ToString();
            textBox42.Text = Buff_ID_EX.rt.ToString();
            textBox43.Text = Buff_ID_EX.reg_rs.ToString();
            textBox44.Text = Buff_ID_EX.reg_rt.ToString();

            textBox45.Text = Buff_EX_MEM.func.ToString();
            textBox46.Text = Buff_EX_MEM.op.ToString();

            textBox47.Text = Buff_EX_MEM.rd.ToString();
            textBox48.Text = Buff_EX_MEM.rs.ToString();
            textBox49.Text = Buff_EX_MEM.rt.ToString();

            textBox50.Text = Buff_EX_MEM.reg_rs.ToString();
            textBox51.Text = Buff_EX_MEM.reg_rt.ToString();

            textBox52.Text = Buff_EX_MEM.execution_result.ToString();
            textBox53.Text = Buff_EX_MEM.mem_read.ToString();


            textBox54.Text = Buff_MEM_WB.execution_result.ToString();
            textBox55.Text = Buff_MEM_WB.Mem_result.ToString();

            textBox56.Text = Buff_MEM_WB.rd.ToString();
            textBox57.Text = Buff_MEM_WB.rt.ToString();

            textBox58.Text = Buff_MEM_WB.reg_rs.ToString();
            textBox59.Text = Buff_MEM_WB.reg_rt.ToString();

            textBox60.Text = Buff_MEM_WB.op.ToString();
            textBox61.Text = Buff_MEM_WB.func.ToString();
            textBox62.Text = Buff_MEM_WB.mem_read.ToString();
        } // display the buffers
        private void button2_Click_1(object sender, EventArgs e) // next clock
        {
            
                y = 150;
                x = x + 50;

              
                    for (int i = 0; i < PC; i++)
                    {
                        
                        if (pipelining[i] == "IF")
                        {
                             if (id_stall)
                           {
                                t = "stall";
                                createLabel(x, y, t);
                                if_stall = true;
                            }
                           else if (b_j_flag)
                            { 
                                t = "IDE";
                                pipelining[i] = t;
                                counter++;
                                b_j_flag = false;

                            }

                            else
                            {
                                t = "ID";
                                pipelining[i] = t;
                                decode();
                                buffers();

                            }
                            createLabel(x, y, t);
                            y = y + 40;
                           
                        }
                        else if(pipelining[i] == "IDE")
                        {

                            if (counter > 3)
                            {
                                counter = 0;
                                pipelining[i] = " ";
                               
                            }
                            else
                            {
                                t = "IDE";
                                createLabel(x, y, t);
                              
                                counter++;
                            }
                            y = y + 40;
                        }
                       

                        else if (pipelining[i] == "ID")
                        {

                            if (stall1 || stall2)
                            {
                                t = "stall";
                                createLabel(x, y, t);
                                id_stall = true;
                                stall1 = false;
                                stall2 = false;
                                ///////////////
                                Buff_ID_EX.mem_read = 0;
                                Buff_ID_EX.op = 0;
                                Buff_ID_EX.func = 0;
                                Buff_ID_EX.pc = 0;
                                Buff_ID_EX.rs = 0;
                                Buff_ID_EX.rt = 0;
                                Buff_ID_EX.rd = 0;
                                Buff_ID_EX.imm = 0;
                                Buff_ID_EX.raflag = false;
                                Buff_ID_EX.reg_rs = 0;
                                Buff_ID_EX.reg_rt = 0;
                                
                                ///////////////
                            }
                            else
                            {
                                pipelining[i] = "EX";
                                execute();
                                buffers();
                            }
                            
                            y = y + 40;
                        }

                        else if (pipelining[i] == "EX")
                        {       
                                memory();
                                pipelining[i] = "MEM";
                                mem_update();
                                buffers();
                                y = y + 40;
                        }

                        else if (pipelining[i] == "MEM")
                        {
                            writeback();
                            pipelining[i] = "WB";
                            reg_update();
                            buffers();
                            y = y + 40;
                        }

                        else if (pipelining[i] == "WB")
                        {
                            pipelining[i] = " ";
                            buffers();
                            y = y + 40;
                            
                        }
                        else if (pipelining[i] == "stall")
                        {

                            
                                counter2 = 0;
                                pipelining[i] = " ";

                            y = y + 40;
                        }

                        else
                        {
                            y = y + 40;
                        }
                        
                    }
                    if (b_j_2flag == true)
                    {
                        b_j_2flag = false;
                        IF.currentPC++;
                         PC++;
                        inc_before = true;
                    }

                    if (inc_inside_b_j)
                    { 
                        PC++;
                        inc_inside_b_j = false;
                    }
                    
                if (IF.currentPC < listBox1.Items.Count)
                {
                    if (if_stall)
                    {
                        t = "stall";
                        createLabel(x, y, t);
                        stall1 = false;
                        stall2 = false;
                        id_stall = false;
                        if_stall = false;
                    }
                    else
                    {
                        if (finished_flag != true)
                        {
                            if (b_j_2flag)
                            { IF.currentPC++; PC++; }
                            if (b_j_flag == false && b_j_2flag == false && inc_before == false && inc_in_ex == false)
                            { IF.currentPC++; PC++; }
                            fetch();
                            pipelining.Add("IF");
                            if (inc_before == true)
                                inc_before = false;
                            if (IF.currentPC == listBox1.Items.Count - 1)
                                finished_flag = true;

                        }
                    }
                       
                            buffers();
                            Clock_cycles++;
                            textBox63.Text = Clock_cycles.ToString();
                            y = y + 40;
                }
                if (inc_in_ex)
                    inc_in_ex = false;

        }

        public void createLabel(int x, int y, string text)
        {
            Label l = new Label();
            l.Text = text;
            l.Location = new Point(x, y);
            l.Font = new System.Drawing.Font(l.Font.FontFamily.Name, 12);
            l.Size = new System.Drawing.Size(40,30);
            l.ForeColor = Color.Black;
            l.BackColor = Color.Yellow;
            this.Controls.Add(l);
        }
        public void delete()
        {
            foreach (Label lab in Controls.OfType<Label>())
            {
                if (lab.BackColor == Color.Yellow)
                {
                    Controls.Remove(lab);
                    lab.Dispose();
                }
            }
        }
     
        private void button4_Click(object sender, EventArgs e) // to exit
        

        {
            MessageBox.Show("Your Pipeline completed Sucsessfully!");
            Application.Exit();
        }

        private void reg_update ()
        {
            textBox1.Text = RF.y.regs[0].ToString();
            textBox2.Text = RF.y.regs[1].ToString();
            textBox3.Text = RF.y.regs[2].ToString();
            textBox4.Text = RF.y.regs[3].ToString();
            textBox5.Text = RF.y.regs[4].ToString();
            textBox6.Text = RF.y.regs[5].ToString();
            textBox7.Text = RF.y.regs[6].ToString();
            textBox8.Text = RF.y.regs[7].ToString();
            textBox9.Text = RF.y.regs[8].ToString();
            textBox10.Text = RF.y.regs[9].ToString();
            textBox11.Text = RF.y.regs[10].ToString();
            textBox12.Text = RF.y.regs[11].ToString();
            textBox13.Text = RF.y.regs[12].ToString();
            textBox14.Text = RF.y.regs[13].ToString();
            textBox15.Text = RF.y.regs[14].ToString();
            textBox16.Text = RF.y.regs[15].ToString();
            textBox35.Text = RF.y.regs[16].ToString();
        }

        private void mem_update()
        {
            textBox17.Text = M.x.mem[0].ToString();
            textBox18.Text = M.x.mem[1].ToString();
            textBox19.Text = M.x.mem[2].ToString();
            textBox20.Text = M.x.mem[3].ToString();
            textBox21.Text = M.x.mem[4].ToString();
            textBox22.Text = M.x.mem[5].ToString();
            textBox23.Text = M.x.mem[6].ToString();
            textBox24.Text = M.x.mem[7].ToString();
            textBox25.Text = M.x.mem[8].ToString();
            textBox26.Text = M.x.mem[9].ToString();
            textBox27.Text = M.x.mem[10].ToString();
            textBox28.Text = M.x.mem[11].ToString();
            textBox29.Text = M.x.mem[12].ToString();
            textBox30.Text = M.x.mem[13].ToString();
            textBox31.Text = M.x.mem[14].ToString();
            textBox32.Text = M.x.mem[15].ToString();
            
        }


    }
}
