using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Net;
using System.Timers;
using System.Threading;
using System.Runtime.Serialization;
using System.Windows.Shapes;

namespace ASAIProgImitator
{
    public class consts
    {
        public const int MAX_CTG = 3;
        public const int MAX_CDG_LENGTH = 20000;
        public const int MAX_CDG_CHN = 16;
        public const double LIGHT_VEL = 299792.458; // km/sec
        public const double SCT_WDT = 360.0 / 256.0;
        public const byte SYNC_BYTE = 0xAA;
        public const double CTG_107PRL_MAX_DIST = 409.6;
        public const double DSC = 0.1; // дискрет = 100 м
        public const int CTG_255PRL_MAX_AMPL = 256;
        public const int NRZ_STO_WDT = 7;
    }

    public enum RLSType { PRL, VRL, NRZ }
    public enum RLSReqSignal { IndvNumb, HT }
    public enum RLSDiagType { Uniform, SinX }

    [Serializable]
    public class RLModel
    {
        public const int PX2KM = 2;

        public double Height;
        public double Width;
        public double GridXStep;
        public double GridYStep;
        public bool GridVisible;
        public bool SyncDoub;
        [NonSerialized]
        public bool IsChanged;
        [NonSerialized]
        public bool FileIsSet;
        [NonSerialized]
        public string FileName;
        [NonSerialized]
        public string SafeFileName;
        public List<RLSPosition> RLSPositionList;
        public List<FOTrace> FOTraceList;
        public TimeSpan Duration;
        [NonSerialized]
        public Storyboard playStrBoard;
        [NonSerialized]
        public Storyboard buildStrBoard;
        
        public RLModel(double h, double w,
                       bool gv,
                       double xs, double ys)
        {
            this.Height = h; this.Width = w;
            this.GridVisible = gv;
            this.GridXStep = xs; this.GridYStep = ys;
            this.SyncDoub = true;
            this.IsChanged = false;
            RLSPositionList = new List<RLSPosition> { };
            FOTraceList = new List<FOTrace> { };
            this.Duration = TimeSpan.FromSeconds(600.0);
            this.playStrBoard = new Storyboard();
            this.playStrBoard.Duration = this.Duration;
            this.FileIsSet = false;
            this.SafeFileName = "РЛ-Модель.rlm";
            this.FileName = "";
        }

        public RLModel() : this(1000, 1000,
                                true,
                                50, 50) { }
    }

    [Serializable]
    public class RLSPosition
    {
        public Point Position;
        public List<RLS> rlsList;

        public RLSPosition(Point p)
        {
            this.Position.X = p.X / RLModel.PX2KM;
            this.Position.Y = p.Y / RLModel.PX2KM;
            this.rlsList = new List<RLS> { };
        }
    }

    [Serializable]
    public class RLS
    {
        public string Name;
        public RLSType Type;
        public int ChNmb;
        public RLSReqSignal ReqSignal;
        public double Distance;
        public double DStep;
        public double AStep;
        public double Rate;
        public int Width;
        public double Trsh;
        public double Visir;
        public RLSDiagType DiagType;
        public double StrDuration;
        public bool[] StatNRZ;
        public Category[] Ctgs;

        [NonSerialized]
        public RotateTransform RateTransform;
        [NonSerialized]
        public DoubleAnimation RateAnimation;
        [NonSerialized]
        public List<Path> Pathes;
        [NonSerialized]
        public TranslateTransform pathTrans;

        public RLS()
        {
            this.Name = "РЛС 1";
            this.Type = RLSType.PRL;
            this.ChNmb = 0;
            this.ReqSignal = RLSReqSignal.IndvNumb;
            this.Distance = 400.0;
            this.DStep = 100.0;
            this.AStep = 30.0;
            this.Rate = 1.0;
            this.Width = 12;
            this.Trsh = 30.0;
            this.Visir = 0.0;
            this.DiagType = RLSDiagType.Uniform;
            this.StrDuration = 4.0;
            this.StatNRZ = new bool[consts.NRZ_STO_WDT];

            this.RateTransform = new RotateTransform(0.0);
            this.RateAnimation = new DoubleAnimation(0.0, 360.0,
                                                     TimeSpan.FromMinutes(1/this.Rate));
            this.Ctgs = new Category[consts.MAX_CTG];
            for (int i = 0; i < consts.MAX_CTG; i++)
            {
                this.Ctgs[i] = new Category();
                this.Ctgs[i].EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
                this.Ctgs[i].CdgList = new List<Cdg> { };
            }
        }

        public bool Set(ref RLS_UI rls_ui)
        {
            this.Name = rls_ui.Name;
            switch (rls_ui.Type)
            {
                case 0 : { this.Type = RLSType.PRL; } break;
                case 1 : { this.Type = RLSType.VRL; } break;
                case 2 : { this.Type = RLSType.NRZ; } break;
                default : { return false; }
            }
            this.ChNmb = rls_ui.ChNmb;
            switch (rls_ui.ReqSignal)
            {
                case 0 : { this.ReqSignal = RLSReqSignal.IndvNumb; } break;
                case 1 : { this.ReqSignal = RLSReqSignal.HT; } break;
                default : { return false; }
            }
            this.Distance = rls_ui.Distance;
            switch (rls_ui.DStep)
            {
                case 0: { this.DStep = 50.0; } break;
                case 1: { this.DStep = 100.0; } break;
                case 2: { this.DStep = 150.0; } break;
                case 3: { this.DStep = 200.0; } break;
                default: { return false; }
            }
            switch (rls_ui.AStep)
            {
                case 0: { this.AStep = 15.0; } break;
                case 1: { this.AStep = 30.0; } break;
                case 2: { this.AStep = 45.0; } break;
                case 3: { this.AStep = 90.0; } break;
                default: { return false; }
            }
            this.Rate = rls_ui.Rate;
            this.Visir = rls_ui.Visir;
            this.Width = (int)rls_ui.Width;
            this.Trsh = rls_ui.Trsh;
            rls_ui.statNRZ.CopyTo(this.StatNRZ, 0);
            this.RateAnimation.Duration = TimeSpan.FromMinutes(1 / this.Rate);
            for (int i = 0; i < consts.MAX_CTG; i++)
            {
                this.Ctgs[i].IsEnabled = rls_ui.ctgEn[i];
                this.Ctgs[i].EndPoint.Address = IPAddress.Parse(rls_ui.ipAddress[i]);
                this.Ctgs[i].EndPoint.Port = int.Parse(rls_ui.ipPort[i]);
            }
            switch (rls_ui.DiagType)
            {
                case 0: { this.DiagType = RLSDiagType.Uniform; } break;
                case 1: { this.DiagType = RLSDiagType.SinX; } break;
                default: { return false; }
            }
            this.StrDuration = rls_ui.StrDuration;
            return true;
        }
    }
    
    [Serializable]
    public class Category
    {
        public bool IsEnabled;
        public IPEndPoint EndPoint;
        [NonSerialized]
        public byte[] Cdg;
        public int CdgInd;
        [NonSerialized]
        public List<Cdg> CdgList;
    }

    public class RLS_UI
    {
        public string Name { set; get; }
        public int Type { set; get; }
        public int ChNmb { set; get; }
        public int ReqSignal { set; get; }
        public double Distance { set; get; }
        public int DStep { set; get; }
        public int AStep { set; get; }
        public double Rate { set; get; }
        public double Width { set; get; }
        public double Trsh { set; get; }
        public double Visir { set; get; }
        public bool[] statNRZ { set; get; }
        public bool[] ctgEn { set; get; }
        public string[] ipAddress { set; get; }
        public string[] ipPort { set; get; }
        public int DiagType { set; get; }
        public double StrDuration { set; get; }

        public RLS_UI()
        {
            this.Name = "РЛС 1";
            this.Type = 0;
            this.ChNmb = 0;
            this.ReqSignal = 0;
            this.Distance = 400.0;
            this.DStep = 1;
            this.AStep = 1;
            this.Rate = 3.0;
            this.Width = 12.0;
            this.Trsh = 30.0;
            this.Visir = 0.0;
            this.DiagType = 0;
            this.StrDuration = 4.0;
            this.statNRZ = new bool[consts.NRZ_STO_WDT];

            this.ctgEn = new bool[consts.MAX_CTG];

            this.ipAddress = new string[consts.MAX_CTG];
            this.ipPort = new string[consts.MAX_CTG];
            for (int i = 0; i < consts.MAX_CTG; i++)
            {
                if (i < consts.MAX_CTG - 1) this.ctgEn[i] = true;
                else this.ctgEn[i] = false;
                this.ipAddress[i] = "127.0.0.1";
                this.ipPort[i] = "6000";
            }
        }

        public bool Set(RLS rls)
        {
            this.Name = rls.Name;
            switch (rls.Type)
            {
                case RLSType.PRL: { this.Type = 0; } break;
                case RLSType.VRL: { this.Type = 1; } break;
                case RLSType.NRZ: { this.Type = 2; } break;
            }
            this.ChNmb = rls.ChNmb;
            switch (rls.ReqSignal)
            {
                case RLSReqSignal.IndvNumb: { this.ReqSignal = 0; } break;
                case RLSReqSignal.HT: { this.ReqSignal = 1; } break;
            }

            this.Distance = rls.Distance;

            if (rls.DStep == 50.0) this.DStep = 0;
            else if (rls.DStep == 100.0) this.DStep = 1;
            else if (rls.DStep == 150.0) this.DStep = 2;
            else if (rls.DStep == 200.0) this.DStep = 3;
            else return false;

            if (rls.AStep == 15.0) this.AStep = 0;
            else if (rls.AStep == 30.0) this.AStep = 1;
            else if (rls.AStep == 45.0) this.AStep = 2;
            else if (rls.AStep == 90.0) this.AStep = 3;
            else return false;

            this.Rate = rls.Rate;

            this.Width = rls.Width;
            this.Trsh = rls.Trsh;

            this.Visir = rls.Visir;

            if (rls.DiagType == RLSDiagType.Uniform) this.DiagType = 0;
            else if (rls.DiagType == RLSDiagType.SinX) this.DiagType = 1;
            else return false;

            this.StrDuration = rls.StrDuration;
            rls.StatNRZ.CopyTo(this.statNRZ, 0);

            for (int i = 0; i < consts.MAX_CTG; i++)
            {
                this.ctgEn[i] = rls.Ctgs[i].IsEnabled;
                this.ipAddress[i] = rls.Ctgs[i].EndPoint.Address.ToString();
                this.ipPort[i] = rls.Ctgs[i].EndPoint.Port.ToString();
            }
            return true;
        }
    }

    [Serializable]
    public class FOTrace
    {
        public double Speed;
        public double Length;
        public UInt32 IndvNumb;
        public double BgnOT;
        public double EndOT;
        public double BgnH;
        public double EndH;
        public bool Trouble;
        public List<Point> PntList;
        [NonSerialized]
        public TranslateTransform FOTransTransform;
        [NonSerialized]
        public RotateTransform FORotateTransform;
        [NonSerialized]
        public DoubleAnimationUsingPath FOXAnim;
        [NonSerialized]
        public DoubleAnimationUsingPath FOYAnim;
        [NonSerialized]
        public DoubleAnimationUsingPath FOAAnim;

        [NonSerialized]
        public PathGeometry buildGeom;
        
        public FOTrace()
        {
            this.Speed = 0.0;
            this.Length = 0.0;
            this.IndvNumb = 0;
            this.BgnOT = 0.0; this.EndOT = 0.0;
            this.BgnH = 0.0; this.EndH = 0.0;
            this.Trouble = false;
            this.PntList = new List<Point> { };
            this.FOTransTransform = new TranslateTransform();
            this.FORotateTransform = new RotateTransform();
        }
    }
}
