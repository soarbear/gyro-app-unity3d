using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UdpServer:MonoBehaviour
{
    Socket socket;
    EndPoint clientEnd;
    IPEndPoint ipEnd;
    string recvStr;
    string sendStr;
    byte[] recvData=new byte[512];
    byte[] sendData=new byte[512];
    int recvLen;
    Thread connectThread;
    
    private static float x,y;

    void InitSocket()
    {
        ipEnd=new IPEndPoint(IPAddress.Any,9750);
        socket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
        socket.Bind(ipEnd);
        IPEndPoint sender=new IPEndPoint(IPAddress.Any,9750);
        clientEnd=(EndPoint)sender;
        print("waiting for UDP dgram");
        connectThread=new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketSend(string sendStr)
    {
        sendData=new byte[512];
        sendData=Encoding.ASCII.GetBytes(sendStr);
        socket.SendTo(sendData,sendData.Length,SocketFlags.None,clientEnd);
    }

    void SocketReceive()
    {        
    	print("Entering for Receiving");
        while(true)
        {
            recvData=new byte[512];
            recvLen=socket.ReceiveFrom(recvData,ref clientEnd);
            print("message from: "+clientEnd.ToString());
            recvStr=Encoding.ASCII.GetString(recvData,0,recvLen);
            print(recvStr);
            string[] values = recvStr.Split('\t');
			if (values[0] != "\t" && values[0] != "") {
				x = float.Parse(values[0]);
				y = float.Parse(values[1]);
			}
            //sendStr="From Server: "+recvStr;
            //SocketSend(sendStr);
        }
    }

    void SocketQuit()
    {
        if(connectThread!=null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        if(socket!=null)
            socket.Close();
        print("disconnect");
    }

    void Start()
    {
    	Application.targetFrameRate = 60;
        InitSocket();
    }

    void Update()
    {
		transform.rotation = Quaternion.AngleAxis((float)(10.0 * y), Vector3.forward) * Quaternion.AngleAxis((float)(10.0 * x), Vector3.right);
    }

    void OnApplicationQuit()
    {
        SocketQuit();
    }
}
