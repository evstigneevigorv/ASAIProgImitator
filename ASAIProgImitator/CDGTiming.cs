using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ASAIProgImitator
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private Thread udpThread;
        private int udpCdgInd;

        private void udpThread_Action()
        {
            double durPos = (double)Dispatcher.Invoke(new Func<double>(() => playSlider.Maximum));
            double curPos;
            while (udpCdgInd < cdgStream.cdgList.Count)
            {
                curPos = (double)Dispatcher.Invoke(new Func<double>(() => playSlider.Value));
                while ((udpCdgInd < cdgStream.cdgList.Count) &&
                       (curPos > cdgStream.cdgList[udpCdgInd].cdg.pos))
                {
                    udpClient.Send(cdgStream.cdgList[udpCdgInd].cdg.cont,
                                   cdgStream.cdgList[udpCdgInd].cdg.length,
                                   cdgStream.ctgEPList[cdgStream.cdgList[udpCdgInd].chNmb]);
                    udpCdgInd++;
                }
                Thread.Sleep(10);
            }
        }
    }

    public class CdgStream
    {
        public List<StrmCdg> cdgList;
        public List<IPEndPoint> ctgEPList;
        public List<string> ctgNameList;
        public List<int> chNmbList;

        public CdgStream()
        {
            this.cdgList = new List<StrmCdg> { };
            this.ctgEPList = new List<IPEndPoint> { };
            this.ctgNameList = new List<string> { };
            this.chNmbList = new List<int> { };
        }
    }

    public class Cdg
    {
        public byte[] cont;
        public int length;
        public double pos;

        public Cdg(byte[] c,
                   int l,
                   double p)
        {
            this.cont = new byte[l];
            for (int i = 0; i < l; i++) this.cont[i] = c[i];
            this.length = l;
            this.pos = p;
        }
    }

    public class StrmCdg
    {
        public int chNmb;
        public Cdg cdg;
        public StrmCdg(Cdg c,
                       int ch)
        {
            this.chNmb = ch;
            this.cdg = c;
        }
    }
}
