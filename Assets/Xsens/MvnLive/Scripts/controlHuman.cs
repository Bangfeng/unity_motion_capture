using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class controlHuman : MonoBehaviour
{

    
    //SerialPort port = new SerialPort();
    List<SerialPort> ports = new List<SerialPort> { };


    float len = new float();
    float[] ANGLE = new float[10];   
    float[] DATA;
    short[,] a = new short[9,3];
    short[,] Angle = new short[9,3];
    short[,] sDStatus = new short[9,3];
    short[,] Gyro = new short[9, 3];
    float[,] outAcc = new float[9, 3];
    float[,] outAn = new float[9, 3];
    float[,] outGyo = new float[9, 3];
    enum ControlPart { LeftHip, RightHip, LeftKnee, RightKnee};
    float acceleration = new float();
    float speed = new float();
    float displacement = new float();


    public void serial_init(SerialPort port,int _ulnum)
    {
       
        try
        {
            ///检测串口是否开着
             if (!port.IsOpen)
            {
                ///设置串口名称
                port.PortName = "COM" + Convert.ToString(_ulnum);
                ///设置波特率
                port.BaudRate = 115200;
                ///数据位
                port.DataBits = 8;
                ///设置无协议
                port.Handshake = Handshake.None;
                ///设置读取不超时
                port.ReadTimeout = -1;
                port.WriteTimeout = -1;
                ///设置停止位
                port.StopBits = StopBits.One;
                port.RtsEnable = true;
                port.DtrEnable = false;
                ///无校验
                port.Parity = Parity.None;
                ///设置数据接收事件
                //port.DataReceived += ReciveData;
                ///打开串                   
                port.Open();
                if (port.IsOpen)
                {

                    print("\n已与:" + port.PortName + "连接\n");

                }
                else
                {
                    print("\n没有可用串口!");
                }
            }

        }
        catch (Exception ex)
        {
            print("\n端口设置错误!");
        }

    }
    // Use this for initialization
    public void CopeSerialData(byte[] ucData, int usLength,int i)
    {
            
        long[] chrTemp = new long[6000];
        Array.Copy(ucData, chrTemp, usLength);        
        int ucRxCnt = 0;
        int usRxLength = 0;

        short T;
        usRxLength += usLength;
        int progress = 0;


        while (usRxLength >= 11)
        {

           

            if (chrTemp[progress] != 0x55)
            {
                progress++;
                usRxLength--;
            }
            switch (chrTemp[progress + 1])
            {

                case 0x51:
                    {
                        a[i,0] = (short)(chrTemp[progress + 3]);
                        a[i,0] = (short)( a[i,0] << 8);
                        a[i,0] = (short)( a[i,0] + chrTemp[progress + 2]);
                        outAcc[i,0] =(float) a[i,0] * 16 / 32768;

                        a[i,1] = (short)(chrTemp[progress + 5]);
                        a[i,1] = (short)(a[i,1] << 8);
                        a[i,1] = (short)(a[i,1] + chrTemp[progress + 4]);
                        outAcc[i,1] = (float)a[i,1]  * 16 / 32768;

                        a[i,2] = (short)(chrTemp[progress + 7]);
                        a[i,2] = (short)(a[i,2] << 8);
                        a[i,2] = (short)(a[i,2] + chrTemp[progress + 6]);
                        outAcc[i,2] = (float)a[i,2]  * 16 / 32768;
                    }
                    break;

                case 0x53:
                    {
                        Angle[i,0] = (short)(chrTemp[progress + 3]);
                        Angle[i,0] = (short)(Angle[i,0] << 8);
                        Angle[i,0] = (short)(Angle[i,0] + chrTemp[progress + 2]);
                        outAn[i,0] = (float)Angle[i,0]  * 180 / 32768;

                        Angle[i,1] = (short)(chrTemp[progress + 5]);
                        Angle[i,1] = (short)(Angle[i,1] << 8);
                        Angle[i,1] = (short)(Angle[i,1] + chrTemp[progress + 4]);
                        outAn[i, 1] = (float)Angle[i,1]  * 180 / 32768;

                        Angle[i,2] = (short)(chrTemp[progress + 7]);
                        Angle[i,2] = (short)(Angle[i,2] << 8);
                        Angle[i,2] = (short)(Angle[i,2] + chrTemp[progress + 6]);
                        outAn[i, 2] = (float)Angle[i,2] * 180 / 32768;
                        //T = (short)(T ^ chrTemp[progress + 8]);
                        //T = (short)(T << 8);
                        //T = (short)(T ^ chrTemp[progress + 9]);

                    }
                    break;
                case 0x52:
                    {
                        Gyro[i, 0] = (short)(chrTemp[progress + 3]);
                        Gyro[i,0] = (short)(Gyro[i,0] << 8);
                        Gyro[i,0] = (short)(Gyro[i,0] + chrTemp[progress + 2]);
                        outGyo[i, 0] = (float)Gyro[i, 0] * 2000 / 32768;

                        Gyro[i,1] = (short)(chrTemp[progress + 5]);
                        Gyro[i,1] = (short)(Gyro[i,1] << 8);
                        Gyro[i,1] = (short)(Gyro[i, 1] + chrTemp[progress + 4]);
                        outGyo[i, 1] = (float)Gyro[i, 1] * 2000 / 32768;

                        Gyro[i,2] = (short)(chrTemp[progress + 7]);
                        Gyro[i,2] = (short)(Gyro[i,2] << 8);
                        Gyro[i,2] = (short)(Gyro[i,2] + chrTemp[progress + 6]);
                        outGyo[i, 2] = (float)Gyro[i, 2] * 2000 / 32768;

                    }
                    break;



            }
            usRxLength -= 11;
            progress += 11;

        }
        
       
    }
    public float[] ReceiveData(byte _serialNum)
    {
        float[] data = new float[_serialNum];
        byte[] buff = new byte[8000];

        for (int i = 0; i < _serialNum; ++i)
        {
            int count = ports[i].BytesToRead;
            ports[i].Read(buff, 0, count);
            CopeSerialData(buff, count,i);
            //print(outAn[i, 0]);
            data[i] = TurnAngle(i, outAn[i,0] );//确定一个绝对坐标系，记录每个九轴的x欧拉角
        }
        return data;//返回数组
    }
    float TurnAngle(int i,float _angle)
    {

        float angle;
        angle = (_angle+90- ANGLE[i]);
        
            ANGLE[i] = _angle + 90;

            return angle;
        
    }
    float CalculationEuler(float HipEular, float KneeEular)
    {      
        float KneeRotation = KneeEular - HipEular;
        return KneeRotation;
    }
    float Calculation(float righthipEular, float lefthipEular, float righthipAy, float lefthipAy)//, float righthipPalstance, float lefthipPalstance, float L
    {
        float A;
        float righthipA;
        float lefthipA;
        float righthipRotation;
        float lefthipRotation;
        righthipRotation = righthipEular - 90;
        lefthipRotation = lefthipEular - 90;
        righthipA =(float)(righthipAy) / (float)Math.Sin((float)(righthipRotation * Math.PI) / 180);//righthipPalstance * righthipPalstance * L / 2 - 
        lefthipA = (float)(lefthipAy) / (float)Math.Sin((float)(lefthipRotation * Math.PI) / 180);//lefthipPalstance * lefthipPalstance * L / 2 - 
         A = (righthipA + lefthipA) / 2;
        return A;
        
    }

    void Start()
    {
        len = 0.02f;
        SerialPort port0 = new SerialPort();
        ports.Add(port0);
        SerialPort port1 = new SerialPort();
        ports.Add(port1);
        SerialPort port2 = new SerialPort();
        ports.Add(port2);
        SerialPort port3 = new SerialPort();
        ports.Add(port3);
        serial_init(ports[0], 21);
        serial_init(ports[1], 17);
        serial_init(ports[2], 9);
        serial_init(ports[3], 24);
    }

    // Update is called once per frame
    void Update()
    {

         DATA =ReceiveData(4);

        transform.Find("Hips/LeftHip").Rotate(0, 0, DATA[0]);

        transform.Find("Hips/LeftHip/LeftKnee").Rotate(0, 0, CalculationEuler(DATA[0], DATA[1]));

        transform.Find("Hips/RightHip").Rotate(0, 0, -DATA[2]);

        transform.Find("Hips/RightHip/RightKnee").Rotate(0, 0, -CalculationEuler(DATA[2], DATA[3]));
    
        //acceleration= Calculation(outAn[0, 0],outAn[2, 0],outAcc[0, 1], outAcc[2, 1]);//, outGyo[0, 0], outGyo[2, 0],len
        //speed += acceleration;
        //if (speed>=5)
        //{
        //    displacement += speed;
        //}
        //transform.position = new Vector3(displacement/500000,0,0);
        if (Input.GetKeyDown(KeyCode.W))
        {

            //transform.Find("Hips/LeftHip").Rotate(0, 0, TurnAngle(90));

            

        }
        if (Input.GetKeyDown(KeyCode.S))
        {

            //transform.Find("Hips/LeftHip/LeftKnee").Rotate(0, 0, TurnAngle(30));

            //transform.Find("Hips/LeftHip").Rotate(0, 0, TurnAngle(-30));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {


            transform.Find("Hips/LeftHip/LeftKnee").Rotate(3, 0, 0);
            //obj1.transform.Rotate(0, 5, 3);

            //mvnActor.Rotate(new Vector3(0, 5, 0));
            //Quaternion orientation;

        }
        if (Input.GetKeyDown(KeyCode.D))
        {


            transform.Find("Hips/LeftHip/LeftKnee").Rotate(0, 3, 0);
            //obj1.transform.Rotate(0, 5, 3);

            //mvnActor.Rotate(new Vector3(0, 5, 0));
            //Quaternion orientation;

        }
    }

}