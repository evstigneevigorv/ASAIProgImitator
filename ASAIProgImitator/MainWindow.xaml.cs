using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Net;
using System.Net.Sockets;

namespace ASAIProgImitator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int GRID = 0;
        const int VSR = 1;

        private Dispatcher mainUIDispatcher { set; get; }

        public MainWindow()
        {
            InitializeComponent();
            mainUIDispatcher = Dispatcher.CurrentDispatcher;
            udpClient = new UdpClient();
            udpThread = new System.Threading.Thread(udpThread_Action);
            udpThread.Priority = System.Threading.ThreadPriority.AboveNormal;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Model Init
            rlModel = new RLModel();
            rlModel.playStrBoard = (Storyboard)this.Resources["playSB"];
            rlModel.playStrBoard.Duration = TimeSpan.FromSeconds(600);

            lastIndex = 0;
            UpdateRLModel();
            // Interface Init
            editMode = new EditMode();
            editMode = EditMode.Hand;
            trgObject = TargetObject.None;
            trgIndex = -1;
            selObject = new SelObject();
            modelScrollViewer.ScrollToHorizontalOffset((modelCanvas.Width - modelScrollViewer.ViewportWidth) / 2);
            modelScrollViewer.ScrollToVerticalOffset((modelCanvas.Height - modelScrollViewer.ViewportHeight) / 2);
            playMode = new PlayMode();
            playMode = PlayMode.Stop;
            chUsdAry = new bool[consts.MAX_CDG_CHN];
            for (int i = 0; i < consts.MAX_CDG_CHN; i++) chUsdAry[i] = false;

            // Files Init
            crtDirectory = "%HOME%";
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (udpThread != null) udpThread.Abort();
        }

        private void UpdateRLModel()
        {
            modelCanvas.Children.Clear();
            modelCanvas.Height = rlModel.Height * RLModel.PX2KM;
            modelCanvas.Width = rlModel.Width * RLModel.PX2KM;
            if (rlModel.GridVisible)
            {
                int xSt = (int)(rlModel.Width / rlModel.GridXStep + 1);
                int ySt = (int)(rlModel.Height / rlModel.GridYStep + 1);
                Line[] lnX = new Line[xSt];
                Line[] lnY = new Line[ySt];
                for (int i = 0; i < xSt; i++)
                {
                    lnX[i] = new Line();
                    lnX[i].Stroke = Brushes.Gray;
                    lnX[i].StrokeDashArray.Add(5.0);
                    lnX[i].StrokeDashArray.Add(1.0);
                    lnX[i].X1 = lnX[i].X2 = i * rlModel.GridXStep* RLModel.PX2KM;
                    lnX[i].Y1 = 0; lnX[i].Y2 = rlModel.Height * RLModel.PX2KM;
                    modelCanvas.Children.Add(lnX[i]);
                }
                for (int j = 0; j < ySt; j++)
                {
                    lnY[j] = new Line();
                    lnY[j].Stroke = Brushes.Gray;
                    lnY[j].StrokeDashArray.Add(5.0);
                    lnY[j].StrokeDashArray.Add(1.0);
                    lnY[j].X1 = 0; lnY[j].X2 = rlModel.Width * RLModel.PX2KM;
                    lnY[j].Y1 = lnY[j].Y2 = j * RLModel.PX2KM * rlModel.GridYStep;
                    modelCanvas.Children.Add(lnY[j]);
                }
            }
            // RLSPosiotions Draw
            foreach (RLSPosition rlsPos in rlModel.RLSPositionList)
                foreach (RLS rls in rlsPos.rlsList)
                {
                    RLSDraw(rlsPos.Position, rls);
                    rls.Pathes[GRID].Tag = rlModel.RLSPositionList.IndexOf(rlsPos);
                    rls.Pathes[VSR].Tag = rlModel.RLSPositionList.IndexOf(rlsPos);
                }
            // Pathes Draw
            foreach (FOTrace trace in rlModel.FOTraceList)
                TraceDraw(trace);
            // Flares Draw

            modelScrollViewer.ScrollToHorizontalOffset((modelCanvas.Width - modelScrollViewer.ViewportWidth) / 2);
            modelScrollViewer.ScrollToVerticalOffset((modelCanvas.Height - modelScrollViewer.ViewportHeight) / 2);
        } //UpdateRLModel

        private void UpdateAnim()
        {
            rlModel.playStrBoard = new Storyboard();
            rlModel.playStrBoard = (Storyboard)this.Resources["playSB"];
            rlModel.playStrBoard.Duration = rlModel.Duration;
            foreach (RLSPosition rlsPos in rlModel.RLSPositionList)
            {
                foreach (RLS rls in rlsPos.rlsList)
                {
                    foreach (Category ctg in rls.Ctgs)
                        ctg.CdgList = new List<Cdg> { };
                    rls.RateAnimation = new DoubleAnimation(0.0, 360.0, TimeSpan.FromMinutes(1 / rls.Rate));
                    rls.RateAnimation.RepeatBehavior = RepeatBehavior.Forever;
                    rls.RateTransform = new RotateTransform(0.0);
                    string UID = SetUID();
                    this.RegisterName(UID, rls.RateTransform);
                    rlModel.playStrBoard.Children.Add(rls.RateAnimation);
                    Storyboard.SetTargetName(rls.RateAnimation, UID);
                    Storyboard.SetTargetProperty(rls.RateAnimation, new PropertyPath(RotateTransform.AngleProperty));
                }
            }
            foreach (FOTrace trace in rlModel.FOTraceList)
            {
                trace.FOTransTransform = new TranslateTransform(trace.PntList[0].X * RLModel.PX2KM,
                                                                trace.PntList[0].Y * RLModel.PX2KM);
                double a = -Math.Atan((trace.PntList[1].X - trace.PntList[0].X) /
                                      (trace.PntList[1].Y - trace.PntList[0].Y)) * 180 / Math.PI;
                if (trace.PntList[1].Y - trace.PntList[0].Y >= 0) a += 180;
                trace.FORotateTransform = new RotateTransform(a - 90.0, 0.0, 0.0);

                #region SetPlayAnimation
                trace.FOXAnim = new DoubleAnimationUsingPath();
                trace.FOXAnim.RepeatBehavior = new RepeatBehavior(1);
                if (trace.Speed != 0.0)
                    trace.FOXAnim.Duration = new Duration(TimeSpan.FromHours(trace.Length / trace.Speed));
                else
                    trace.FOXAnim.Duration = Duration.Forever;
                string UID = SetUID();
                this.RegisterName(UID, trace.FOTransTransform);
                Storyboard.SetTargetName(trace.FOXAnim, UID);
                Storyboard.SetTargetProperty(trace.FOXAnim, new PropertyPath(TranslateTransform.XProperty));
                trace.FOXAnim.Source = PathAnimationSource.X;

                trace.FOYAnim = new DoubleAnimationUsingPath();
                trace.FOYAnim.RepeatBehavior = new RepeatBehavior(1);
                if (trace.Speed != 0.0)
                    trace.FOYAnim.Duration = new Duration(TimeSpan.FromHours(trace.Length / trace.Speed));
                else
                    trace.FOYAnim.Duration = Duration.Forever;
                UID = SetUID();
                this.RegisterName(UID, trace.FOTransTransform);
                Storyboard.SetTargetName(trace.FOYAnim, UID);
                Storyboard.SetTargetProperty(trace.FOYAnim, new PropertyPath(TranslateTransform.YProperty));
                trace.FOYAnim.Source = PathAnimationSource.Y;

                trace.FOAAnim = new DoubleAnimationUsingPath();
                trace.FOAAnim.RepeatBehavior = new RepeatBehavior(1);
                if (trace.Speed != 0.0)
                    trace.FOAAnim.Duration = new Duration(TimeSpan.FromHours(trace.Length / trace.Speed));
                else
                    trace.FOAAnim.Duration = Duration.Forever;
                UID = SetUID();
                this.RegisterName(UID, trace.FORotateTransform);
                Storyboard.SetTargetName(trace.FOAAnim, UID);
                Storyboard.SetTargetProperty(trace.FOAAnim, new PropertyPath(RotateTransform.AngleProperty));
                trace.FOAAnim.Source = PathAnimationSource.Angle;
                #endregion SetPlayAnimation

                rlModel.playStrBoard.Children.Add(trace.FOXAnim);
                rlModel.playStrBoard.Children.Add(trace.FOYAnim);
                rlModel.playStrBoard.Children.Add(trace.FOAAnim);
            }
        }

        private void playStoryboard_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            if (rlModel.playStrBoard.Duration.HasTimeSpan)
                playSlider.Value = rlModel.playStrBoard.GetCurrentTime().TotalMilliseconds;
        }

        private void playStoryboard_Changed(object sender, EventArgs e)
        {
            if (rlModel.playStrBoard.Duration.HasTimeSpan)
                playSlider.Maximum = rlModel.playStrBoard.Duration.
                                     TimeSpan.TotalMilliseconds;
        }

    }
}
