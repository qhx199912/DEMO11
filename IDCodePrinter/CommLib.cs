using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO.Ports;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

namespace IDCodePrinter
{
    /// <summary>
    /// 监控程序串口通讯类
    /// </summary>
    class CommLib
    {
        /// <summary>
        /// 从机实时读取队列
        /// </summary>
        public static Queue<byte[]> readBuff = new Queue<byte[]>();
        /// <summary>
        /// 实时数据处理缓存
        /// </summary>
        public static List<byte> temBuff = new List<byte>();
        /// <summary>
        /// 本地缓存 当前支架总数+1
        /// </summary>
        public static byte[][] localBuff;

        public static int[] rvcArr;//总行程数组

        public static SerialPort comm = new SerialPort();

        /// <summary>
        /// 数据处理线程RunFlag
        /// </summary>
        public static bool RDCRunFlag = true;

        public static string rbt = "";

        /// <summary>
        /// 关闭所有串口
        /// </summary>
        public static void CloseComms()
        {
            comm.Close();
            RDCRunFlag = false;

            Thread.Sleep(1000);
        }

        public static int initComms(string commPortName, int contAmount, int startCont, int par, int stpb, int databit, int baurate)
        {
            try
            {
                //接收实时数据
                comm.PortName = commPortName;
                comm.Parity = Parity.None;
                comm.StopBits = StopBits.One;
                comm.ReadTimeout = 1000;
                comm.WriteTimeout = 1000;
                comm.BaudRate = baurate;

                comm.Open();
                //RDCRunFlag = true;

                //comm2.DataReceived += new SerialDataReceivedEventHandler(Form1.receiveComm2);
                //comm2.ReceivedBytesThreshold = 6;

                //支架控制器状态数据实时处理线程
                //Thread t = new Thread(receiveDataCheckThread);
                //t.Name = "receiveDataCheckThread";
                //t.IsBackground = true;
                //t.Start();
            }
            catch { }

            return 0;
        }

        /// <summary>
        /// 支架控制器状态数据实时处理线程
        /// </summary>
        private static void receiveDataCheckThread()
        {
            //byte[] tem = new byte[41];
            //int modbusReturn = 1;


            //while (RDCRunFlag)
            //{
            //    try
            //    {
            //        if (readBuff.Count != 0)
            //            temBuff.AddRange(readBuff.Dequeue());

            //        //temBuff[6] + 9 判断数据长度
            //        //41字节 单架状态最长帧  现为29 无角度传感器状态
            //        //13字节 煤机位置
            //        if (temBuff.Count >= 9)
            //        {
            //            if (temBuff[6] + 9 <= temBuff.Count)
            //            {
            //                temBuff.CopyTo(0, tem, 0, temBuff[6] + 9);
            //                modbusReturn = ModbusLib.modbusClientReceive(tem);

            //                if (modbusReturn == 0)
            //                {
            //                    //if (temBuff[7] == 0 && temBuff[8] == 0)
            //                    //{
            //                    //    //////////煤机位置实时更新
            //                    //    temBuff.CopyTo(9, localBuff[0], 0, 2);
            //                    //}
            //                    //else if (temBuff[7] < 2 && temBuff[8] < 0xF5)
            //                    //{
            //                    //    /////////支架状态实时更新
            //                    //    ushort ui16; //支架号
            //                    //    ui16 = (ushort)((tem[7] << 8) + tem[8]);

            //                    //    byte[] tem2 = new byte[4];
            //                    //    Array.ConstrainedCopy(tem, 9, tem2, 0, 4);
            //                    //    BitArray bitArr = new BitArray(tem2);
            //                    //    BitArray bitArr2 = new BitArray(tem2);

            //                    //    int i4 = 7;
            //                    //    int i5 = 0;
            //                    //    int i6 = 1;
            //                    //    for (int i3 = 0; i3 < bitArr.Count; i3++)
            //                    //    {
            //                    //        if ((i3 % 8 == 0) && (i3 != 0))
            //                    //        {
            //                    //            i4 = (i6 + 1) * 8 - 1;
            //                    //            i5 = i6 * 8;
            //                    //            i6++;
            //                    //        }
            //                    //        if (i4 >= i5)
            //                    //        {
            //                    //            bitArr.Set(i3, bitArr2.Get(i4));
            //                    //            i4--;
            //                    //        }
            //                    //    }

            //                    //    int i7 = 0;
            //                    //    for (int i = 0; i < 13; i++)
            //                    //    {
            //                    //        //对总行程值进行累加
            //                    //        if (i == 1)
            //                    //        {
            //                    //            int rv = (localBuff[ui16][i * 2] << 8) + localBuff[ui16][i * 2 + 1];
            //                    //            int rvn = (temBuff[i7 * 2 + 13] << 8) + temBuff[i7 * 2 + 1 + 13];

            //                    //            if (rvn > rv)
            //                    //                rvcArr[ui16] += rvn - rv;
            //                    //        }

            //                    //        if (bitArr.Get(i))
            //                    //        {
            //                    //            localBuff[ui16][i * 2] = temBuff[i7 * 2 + 13];
            //                    //            localBuff[ui16][i * 2 + 1] = temBuff[i7 * 2 + 1 + 13];

            //                    //            i7++;
            //                    //        }
            //                    //    }
            //                    //}
            //                    //删除已处理有效帧
            //                    temBuff.RemoveRange(0, temBuff[6] + 9);
            //                    ++Form1.rtd;
            //                }
            //                else
            //                {
            //                    foreach (byte ttttt in temBuff)
            //                        rbt += ttttt.ToString("X2") + " ";
            //                    rbt += " " + DateTime.Now + "CRCtemBuff count " + temBuff.Count + "\n";
            //                    //调试输出信息
            //                    System.Diagnostics.Debug.WriteLine(rbt);

            //                    //数据帧异常 清空缓存
            //                    temBuff.Clear();
            //                    ++Form1.crcwtd;
            //                }
            //            }
            //            else if (temBuff[6] + 9 > 29)
            //            {
            //                foreach (byte ttttt in temBuff)
            //                    rbt += ttttt.ToString("X2") + " ";
            //                rbt += " " + DateTime.Now + "temBuff count " + temBuff.Count + "\n";
            //                //调试输出信息
            //                System.Diagnostics.Debug.WriteLine(rbt);

            //                //数据帧异常 清空缓存
            //                temBuff.Clear();
            //                ++Form1.wtd;
            //            }
            //        }
            //        //Thread.Sleep(10);
            //    }
            //    catch
            //    {
            //        MessageBox.Show("实时状态数据处理异常");
            //        Thread.Sleep(2000);
            //    }
            //}
        }
    }
}
