using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyModbus;
using System.Threading;
using System.Data.SqlClient;
using ModServe1;

namespace ModServe1
{
    public partial class Form1 : Form
    {


        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Exception.ToString(), @"Thread Exception",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {


                Application.Exit();
            }
        }





        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void SetTextCallback(string text);

        // This thread is used to demonstrate both thread-safe and
        // unsafe ways to call a Windows Forms control.
        //private Thread demoThread = null;

        public static class MyGlobals
        {
            public static string startStop = "Start";
          

        }



        //int count;
        bool keepGoing = true; // this is loop control flag
        public Form1()
        {
            InitializeComponent();
            ToggleState();
            statusTextBox.Text = "Server Stopped...";

        }

        private void ToggleState()
        {
            // Flip the value of the loop control flag   
            keepGoing = !keepGoing;

            // Enable/disable buttons   
            button2.Enabled = keepGoing;
            button1.Enabled = !keepGoing;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // statusTextBox.Text = "Server Connected...";

            ToggleState();

            MyGlobals.startStop = "Start";

            StartCounting();

        }

        private void button2_Click(object sender, EventArgs e)
        {


            MyGlobals.startStop = "Stop";
            statusTextBox.Text = "Server Stopped...";
            ToggleState();
            Form1.ActiveForm.Dispose();
            

            //Form1 f = new Form1();



        }


        public void StartCounting()
        {

            try
            {

         
            // Set up a new thread to do the counting   
            ThreadStart ts = new ThreadStart(Count);
            Thread t = new Thread(ts);

            // Start the thread   
            t.Start();

                // While this thread is running, the form will continue to respond   
            }
            catch (Exception)
            {

                Form1.ActiveForm.Dispose();

                throw;
            }
        }


        private void Form1_FormClosed(Object sender, FormClosedEventArgs e)
        {

            MyGlobals.startStop = "Stop";

        }

        private void Count()
        {

            try
            {




            //Establish a one-time connection to the Modbus .dll library:


            // ModbusClient modbusClient = new ModbusClient("127.0.0.1", 502);
            ModbusClient modbusClient = new ModbusClient(ipAddressTextBox.Text, int.Parse(portTextBox.Text));


           modbusClient.Connect();



            while (MyGlobals.startStop == "Start")



                {

                if (modbusClient.Connected == true) {


                    Thread demoThread =
                        new Thread(new ThreadStart(this.ThreadProcSafe));

                    demoThread.Start();
                    //statusTextBox.Text = "Server Connected...";
                    //demoThread.Abort();

                    SetText("Server Connected...");

                    Thread.Sleep(1);

                    int[] readHoldingArray = modbusClient.ReadHoldingRegisters(0, 96);



                    // Make the IEEE 754 values out of Registers 44-76

                    float[] floaty = new float[96];

                    for (int i = 76; i < 92; i += 2)
                    {

                        ushort[] received = new ushort[] { (ushort)readHoldingArray[i], (ushort)readHoldingArray[i + 1] };

                        byte[] asByte = new byte[]

                        {
                    (byte)(received[1] % 256),
                    (byte)(received[1] / 256),
                    (byte)(received[0] % 256),
                    (byte)(received[0] / 256),

                        };




                        float result = BitConverter.ToSingle(asByte, 0);

                        // The floaty array holds all the new IEEE 754 values.  e.g. floaty[44] = Register 44/45
                        // converted into a single-precision float.

                        floaty[i] = result;


                    }



                        double[] floatyQuad = new double[256];
                        float[] floatyQuadFinal = new float[256];

                        for (int j = 43; j < 75; j += 4)
                        {

                            ushort[] receivedQuad = new ushort[] { (ushort)readHoldingArray[j], (ushort)readHoldingArray[j + 1], (ushort)readHoldingArray[j + 2], (ushort)readHoldingArray[j + 3] };

                            byte[] asByteQuad = new byte[]

                            {
                    (byte)(receivedQuad[3] % 256),
                    (byte)(receivedQuad[3] / 256),
                    (byte)(receivedQuad[2] % 256),
                    (byte)(receivedQuad[2] / 256),
                    (byte)(receivedQuad[1] % 256),
                    (byte)(receivedQuad[1] / 256),
                    (byte)(receivedQuad[0] % 256),
                    (byte)(receivedQuad[0] / 256),

                            };




                            double resultQuad = BitConverter.ToDouble(asByteQuad, 0);

                            // The floaty array holds all the new IEEE 754 values.  e.g. floaty[44] = Register 44/45
                            // converted into a single-precision float.

                            floatyQuad[j] = resultQuad;
                            floatyQuadFinal[j] = (float)floatyQuad[j];


                        }







                        DateTime myDateTime = DateTime.Now;
                    string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss");




                    //Legacy connection string:
                    //string SQLConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=History";

                    string SQLConnectionString = connectionStringTextBox.Text;



                    // Create a SqlConnection from the provided connection string.



                    using (SqlConnection connection = new SqlConnection(SQLConnectionString))



                    {



                        // Open connection to database.
                        connection.Open();



                        // Begin to formulate the command.
                        SqlCommand command = new SqlCommand();
                        command.Connection = connection;


                        // Specify the query to be executed.
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText =

                        @"INSERT INTO History (TimeStamp, Register1, Register2, Register3, Register4, Register5, Register6, Register7, Register8, Register9, Register10, Register11, Register12, Register13, Register14, Register15, Register16, Register17, Register18, Register19, Register20, Register21, Register22, Register23, Register24, Register25, Register26, Register27, Register28, Register29, Register30, Register31, Register32, Register33, Register34, Register35, Register36, Register37, Register38, Register39, Register40, Register41, Register42, Register43, Register44, Register45, Register46, Register47, Register48, Register50, Register51, Register52, Register53, Register54, Register55, Register56, Register57, Register58, Register59, Register60, Register61, Register62, Register63, Register64, Register65, Register66, Register67, Register68, Register69, Register70, Register71, Register72, Register73, Register74, Register75, Register76, Register77, Register78, Register79, Register80, Register81, Register82, Register83, Register84, Register85, Register86, Register87, Register88, Register89, Register90, Register91, Register92, Register93, Register94, Register95, Register96, Register101, Register102, Register103, Register104, Register105, Register106, Register107, Register108, Register109, Register110, Register111, Register112, Register113) VALUES (@TimeStamp,@Register1, @Register2, @Register3, @Register4, @Register5, @Register6, @Register7, @Register8, @Register9, @Register10, @Register11, @Register12, @Register13, @Register14, @Register15, @Register16, @Register17, @Register18, @Register19, @Register20, @Register21, @Register22, @Register23, @Register24, @Register25, @Register26, @Register27, @Register28, @Register29, @Register30, @Register31, @Register32, @Register33, @Register34, @Register35, @Register36, @Register37, @Register38, @Register39, @Register40, @Register41, @Register42, @Register43, @Register44, @Register45, @Register46, @Register47, @Register48, @Register50, @Register51, @Register52, @Register53, @Register54, @Register55, @Register56, @Register57, @Register58, @Register59, @Register60, @Register61, @Register62, @Register63, @Register64, @Register65, @Register66, @Register67, @Register68, @Register69, @Register70, @Register71, @Register72, @Register73, @Register74, @Register75, @Register76, @Register77, @Register78, @Register79, @Register80, @Register81, @Register82, @Register83, @Register84, @Register85, @Register86, @Register87, @Register88, @Register89, @Register90, @Register91, @Register92, @Register93, @Register94, @Register95, @Register96, @Register101, @Register102, @Register103, @Register104, @Register105, @Register106, @Register107, @Register108, @Register109, @Register110, @Register111, @Register112, @Register113)";



                        // Create your parameters.  This evades SQL Injection attacks.
                        command.Parameters.Add("@TimeStamp", System.Data.SqlDbType.DateTime);
                        command.Parameters.Add("@Register1", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register2", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register3", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register4", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register5", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register6", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register7", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register8", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register9", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register10", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register11", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register12", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register13", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register14", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register15", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register16", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register17", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register18", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register19", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register20", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register21", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register22", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register23", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register24", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register25", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register26", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register27", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register28", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register29", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register30", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register31", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register32", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register33", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register34", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register35", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register36", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register37", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register38", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register39", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register40", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register41", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register42", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register43", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register44", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register45", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register46", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register47", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register48", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register49", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register50", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register51", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register52", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register53", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register54", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register55", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register56", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register57", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register58", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register59", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register60", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register61", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register62", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register63", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register64", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register65", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register66", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register67", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register68", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register69", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register70", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register71", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register72", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register73", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register74", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register75", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register76", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register77", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register78", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register79", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register80", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register81", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register82", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register83", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register84", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register85", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register86", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register87", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register88", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register89", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register90", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register91", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register92", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register93", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register94", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register95", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@Register96", System.Data.SqlDbType.Int);


                        // Here is where you add parameters for the new IEEE 754 values:

                        command.Parameters.Add("@Register101", System.Data.SqlDbType.Float);
                        command.Parameters.Add("@Register102", System.Data.SqlDbType.Float);
                        command.Parameters.Add("@Register103", System.Data.SqlDbType.Float);
                        command.Parameters.Add("@Register104", System.Data.SqlDbType.Float);
                        command.Parameters.Add("@Register105", System.Data.SqlDbType.Float);
                        command.Parameters.Add("@Register106", System.Data.SqlDbType.Float);
                        command.Parameters.Add("@Register107", System.Data.SqlDbType.Float);
                        command.Parameters.Add("@Register108", System.Data.SqlDbType.Float);





                        command.Parameters.Add("@Register109", System.Data.SqlDbType.Float);
                        command.Parameters.Add("@Register110", System.Data.SqlDbType.Float);
                        command.Parameters.Add("@Register111", System.Data.SqlDbType.Float);
                        command.Parameters.Add("@Register112", System.Data.SqlDbType.Float);
                        command.Parameters.Add("@Register113", System.Data.SqlDbType.Float);




                            // set values to parameters from textboxes
                        command.Parameters["@Register1"].Value = readHoldingArray[0];
                        command.Parameters["@Register2"].Value = readHoldingArray[1];
                        command.Parameters["@Register3"].Value = readHoldingArray[2];
                        command.Parameters["@Register4"].Value = readHoldingArray[3];
                        command.Parameters["@Register5"].Value = readHoldingArray[4];
                        command.Parameters["@Register6"].Value = readHoldingArray[5];
                        command.Parameters["@Register7"].Value = readHoldingArray[6];
                        command.Parameters["@Register8"].Value = readHoldingArray[7];
                        command.Parameters["@Register9"].Value = readHoldingArray[8];
                        command.Parameters["@Register10"].Value = readHoldingArray[9];
                        command.Parameters["@Register11"].Value = readHoldingArray[10];
                        command.Parameters["@Register12"].Value = readHoldingArray[11];
                        command.Parameters["@Register13"].Value = readHoldingArray[12];
                        command.Parameters["@Register14"].Value = readHoldingArray[13];
                        command.Parameters["@Register15"].Value = readHoldingArray[14];
                        command.Parameters["@Register16"].Value = readHoldingArray[15];
                        command.Parameters["@Register17"].Value = readHoldingArray[16];
                        command.Parameters["@Register18"].Value = readHoldingArray[17];
                        command.Parameters["@Register19"].Value = readHoldingArray[18];
                        command.Parameters["@Register20"].Value = readHoldingArray[19];
                        command.Parameters["@Register21"].Value = readHoldingArray[20];
                        command.Parameters["@Register22"].Value = readHoldingArray[21];
                        command.Parameters["@Register23"].Value = readHoldingArray[22];
                        command.Parameters["@Register24"].Value = readHoldingArray[23];
                        command.Parameters["@Register25"].Value = readHoldingArray[24];
                        command.Parameters["@Register26"].Value = readHoldingArray[25];
                        command.Parameters["@Register27"].Value = readHoldingArray[26];
                        command.Parameters["@Register28"].Value = readHoldingArray[27];
                        command.Parameters["@Register29"].Value = readHoldingArray[28];
                        command.Parameters["@Register30"].Value = readHoldingArray[29];
                        command.Parameters["@Register31"].Value = readHoldingArray[30];
                        command.Parameters["@Register32"].Value = readHoldingArray[31];
                        command.Parameters["@Register33"].Value = readHoldingArray[32];
                        command.Parameters["@Register34"].Value = readHoldingArray[33];
                        command.Parameters["@Register35"].Value = readHoldingArray[34];
                        command.Parameters["@Register36"].Value = readHoldingArray[35];
                        command.Parameters["@Register37"].Value = readHoldingArray[36];
                        command.Parameters["@Register38"].Value = readHoldingArray[37];
                        command.Parameters["@Register39"].Value = readHoldingArray[38];
                        command.Parameters["@Register40"].Value = readHoldingArray[39];
                        command.Parameters["@Register41"].Value = readHoldingArray[40];
                        command.Parameters["@Register42"].Value = readHoldingArray[41];
                        command.Parameters["@Register43"].Value = readHoldingArray[42];
                        command.Parameters["@Register44"].Value = readHoldingArray[43];
                        command.Parameters["@Register45"].Value = readHoldingArray[44];
                        command.Parameters["@Register46"].Value = readHoldingArray[45];
                        command.Parameters["@Register47"].Value = readHoldingArray[46];
                        command.Parameters["@Register48"].Value = readHoldingArray[47];
                        command.Parameters["@Register49"].Value = readHoldingArray[48];
                        command.Parameters["@Register50"].Value = readHoldingArray[49];
                        command.Parameters["@Register51"].Value = readHoldingArray[50];
                        command.Parameters["@Register52"].Value = readHoldingArray[51];
                        command.Parameters["@Register53"].Value = readHoldingArray[52];
                        command.Parameters["@Register54"].Value = readHoldingArray[53];
                        command.Parameters["@Register55"].Value = readHoldingArray[54];
                        command.Parameters["@Register56"].Value = readHoldingArray[55];
                        command.Parameters["@Register57"].Value = readHoldingArray[56];
                        command.Parameters["@Register58"].Value = readHoldingArray[57];
                        command.Parameters["@Register59"].Value = readHoldingArray[58];
                        command.Parameters["@Register60"].Value = readHoldingArray[59];
                        command.Parameters["@Register61"].Value = readHoldingArray[60];
                        command.Parameters["@Register62"].Value = readHoldingArray[61];
                        command.Parameters["@Register63"].Value = readHoldingArray[62];
                        command.Parameters["@Register64"].Value = readHoldingArray[63];
                        command.Parameters["@Register65"].Value = readHoldingArray[64];
                        command.Parameters["@Register66"].Value = readHoldingArray[65];
                        command.Parameters["@Register67"].Value = readHoldingArray[66];
                        command.Parameters["@Register68"].Value = readHoldingArray[67];
                        command.Parameters["@Register69"].Value = readHoldingArray[68];
                        command.Parameters["@Register70"].Value = readHoldingArray[69];
                        command.Parameters["@Register71"].Value = readHoldingArray[70];
                        command.Parameters["@Register72"].Value = readHoldingArray[71];
                        command.Parameters["@Register73"].Value = readHoldingArray[72];
                        command.Parameters["@Register74"].Value = readHoldingArray[73];
                        command.Parameters["@Register75"].Value = readHoldingArray[74];
                        command.Parameters["@Register76"].Value = readHoldingArray[75];
                        command.Parameters["@Register77"].Value = readHoldingArray[76];
                        command.Parameters["@Register78"].Value = readHoldingArray[77];
                        command.Parameters["@Register79"].Value = readHoldingArray[78];
                        command.Parameters["@Register80"].Value = readHoldingArray[79];
                        command.Parameters["@Register81"].Value = readHoldingArray[80];
                        command.Parameters["@Register82"].Value = readHoldingArray[81];
                        command.Parameters["@Register83"].Value = readHoldingArray[82];
                        command.Parameters["@Register84"].Value = readHoldingArray[83];
                        command.Parameters["@Register85"].Value = readHoldingArray[84];
                        command.Parameters["@Register86"].Value = readHoldingArray[85];
                        command.Parameters["@Register87"].Value = readHoldingArray[86];
                        command.Parameters["@Register88"].Value = readHoldingArray[87];
                        command.Parameters["@Register89"].Value = readHoldingArray[88];
                        command.Parameters["@Register90"].Value = readHoldingArray[89];
                        command.Parameters["@Register91"].Value = readHoldingArray[90];
                        command.Parameters["@Register92"].Value = readHoldingArray[91];
                        command.Parameters["@Register93"].Value = readHoldingArray[92];
                        command.Parameters["@Register94"].Value = readHoldingArray[93];
                        command.Parameters["@Register95"].Value = readHoldingArray[94];
                        command.Parameters["@Register96"].Value = readHoldingArray[95];


                        // Here is where you bind the IEEE 754 values into the SQL parameters:

                        command.Parameters["@Register101"].Value = floaty[76];
                        command.Parameters["@Register102"].Value = floaty[78];
                        command.Parameters["@Register103"].Value = floaty[80];
                        command.Parameters["@Register104"].Value = floaty[82];
                        command.Parameters["@Register105"].Value = floaty[84];
                        command.Parameters["@Register106"].Value = floaty[86];
                        command.Parameters["@Register107"].Value = floaty[88];
                        command.Parameters["@Register108"].Value = floaty[90];
                        command.Parameters["@Register109"].Value = floatyQuad[43];
                        command.Parameters["@Register110"].Value = floatyQuad[47];
                        command.Parameters["@Register111"].Value = floatyQuad[51];
                        command.Parameters["@Register112"].Value = floatyQuad[55];
                        command.Parameters["@Register113"].Value = floatyQuad[59];
                        command.Parameters["@TimeStamp"].Value = sqlFormattedDate;


                        // Read data from the query.
                        //command.ExecuteReader();
                        command.ExecuteNonQuery();

                    }

                    // Pause for the amount of time from the Sampling Rate Text Box:

                    Thread.Sleep((int.Parse(sampleRateTextBox.Text)) * 1000);

                }


                else
                {
                    Application.Exit();
                }


            }


            }
            catch (Exception)
            {
                Application.Exit();
                //throw;
            }


        }

        private void ThreadProcSafe()
        {
            // this.statusTextBox.Text = "Server Connected...";
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.statusTextBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.statusTextBox.Text = text;
            }
        }
    }
} 
