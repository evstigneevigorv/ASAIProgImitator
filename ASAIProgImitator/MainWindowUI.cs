using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Shapes;
using System.Timers;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ASAIProgImitator
{
    /// <summary>
    /// Обработка событий пользовательского интерфейса
    /// главного окна
    /// </summary>
    public partial class MainWindow : Window
    {
        #region MenuFile

        private string crtDirectory;

        public void FileNew_Click(object sender, RoutedEventArgs e)
        {
            rlModel = new RLModel();
            UpdateAnim();
            UpdateRLModel();
        }

        public void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Открыть РЛ-модель";
            dlg.Filter = "РЛ-модель (*.rlm)|*.rlm";
            dlg.DefaultExt = "rlm";
            dlg.InitialDirectory = crtDirectory;
            dlg.RestoreDirectory = false;
            dlg.Multiselect = false;
            if (dlg.ShowDialog().GetValueOrDefault() != true) return;
            Load_rlModel(dlg.FileName);
        }

        public void FileSave_Click(object sender, RoutedEventArgs e)
        {
            if (rlModel.FileIsSet)
            {
                if (!Save_rlModel(rlModel.FileName)) MessageBox.Show("Не удалось сохранить файл",
                                                                     "Ошибка сохранения");
            }
            else ShowSaveDialog();
        }

        public void FileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            ShowSaveDialog();
        }

        public void ShowSaveDialog()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Сохранить РЛ-модель";
            dlg.Filter = "РЛ-модель (*.rlm)|*.rlm";
            dlg.DefaultExt = "rlm";
            dlg.InitialDirectory = crtDirectory;
            dlg.RestoreDirectory = false;
            if (dlg.ShowDialog().GetValueOrDefault() != true) return;
            if (Save_rlModel(dlg.FileName))
            {
                rlModel.FileIsSet = true;
                rlModel.FileName = dlg.FileName;
                rlModel.SafeFileName = dlg.SafeFileName;
            }
            else MessageBox.Show("Не удалось сохранить файл", "Ошибка сохранения");
        }

        public void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion MenuFile

        #region MenuEdit

        public void EditProp_Click(object sender, RoutedEventArgs e)
        {
            switch (selObject.Type)
            {
                case TargetObject.RLS:
                    {
                        RLSListWindow dlg = new RLSListWindow();
                        dlg.rlsList = new List<RLS>(rlModel.RLSPositionList[selObject.Index].rlsList);
                        foreach (RLS rls in dlg.rlsList)
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            dlg.RLSList_Set(ref lbi, rls);
                            dlg.rlsListBox.Items.Add(lbi);
                        }
                        dlg.ShowDialog();
                        if (dlg.DialogResult == (bool?)true)
                        {
                            // Удалить графически
                            foreach (RLS rls in rlModel.RLSPositionList[selObject.Index].rlsList)
                            {
                                modelCanvas.Children.Remove(rls.Pathes[GRID]);
                                modelCanvas.Children.Remove(rls.Pathes[VSR]);
                            }
                            if (dlg.rlsList.Count == 0)
                            {
                                // Если список пуст, то удалить и физически
                                rlModel.RLSPositionList.RemoveAt(selObject.Index);
                                UpdateRLSIndexes();
                            }
                            else // Обновить графику для текущей позиции
                            {
                                rlModel.RLSPositionList[selObject.Index].rlsList = new List<RLS>(dlg.rlsList);
                                foreach (RLS rls in rlModel.RLSPositionList[selObject.Index].rlsList)
                                {
                                    rls.RateAnimation = new DoubleAnimation(0.0, 360.0,
                                                                            TimeSpan.FromMinutes(1 / rls.Rate));
                                    rls.RateAnimation.RepeatBehavior = RepeatBehavior.Forever;
                                    string UID = SetUID();
                                    this.RegisterName(UID, rls.RateTransform);
                                    rlModel.playStrBoard.Children.Add(rls.RateAnimation);
                                    RLSDraw(rlModel.RLSPositionList[selObject.Index].Position, rls);
                                    Storyboard.SetTargetName(rls.RateAnimation, UID);
                                    Storyboard.SetTargetProperty(rls.RateAnimation, new PropertyPath(RotateTransform.AngleProperty));
                                    rls.Pathes[GRID].Tag = rlModel.RLSPositionList.IndexOf(rlModel.RLSPositionList[selObject.Index]);
                                    rls.Pathes[VSR].Tag = rlModel.RLSPositionList.IndexOf(rlModel.RLSPositionList[selObject.Index]);
                                }
                            }
                        }
                    }; break;
                case TargetObject.Path:
                    {

                    }; break;
            }
        }

        public void EditHand_Click(object sender, RoutedEventArgs e)
        {
            EditHandToolButton.IsChecked = EditHand.IsChecked = true;
            EditRLSToolButton.IsChecked = EditRLS.IsChecked = false;
            EditPathToolButton.IsChecked = EditPath.IsChecked = false;
            EditFlareToolButton.IsChecked = EditFlare.IsChecked = false;
            editMode = EditMode.Hand;
            modelScrollViewer.Cursor = Cursors.Hand;
        }

        public void EditRLS_Click(object sender, RoutedEventArgs e)
        {
            EditHandToolButton.IsChecked = EditHand.IsChecked = false;
            EditRLSToolButton.IsChecked = EditRLS.IsChecked = true;
            EditPathToolButton.IsChecked = EditPath.IsChecked = false;
            EditFlareToolButton.IsChecked = EditFlare.IsChecked = false;
            editMode = EditMode.RLS;
            modelScrollViewer.Cursor = Cursors.UpArrow;
        }

        public void EditPath_Click(object sender, RoutedEventArgs e)
        {
            EditHandToolButton.IsChecked = EditHand.IsChecked = false;
            EditRLSToolButton.IsChecked = EditRLS.IsChecked = false;
            EditPathToolButton.IsChecked = EditPath.IsChecked = true;
            EditFlareToolButton.IsChecked = EditFlare.IsChecked = false;
            editMode = EditMode.Path;
            modelScrollViewer.Cursor = Cursors.Cross;
        }

        public void EditFlare_Click(object sender, RoutedEventArgs e)
        {
            EditHandToolButton.IsChecked = EditHand.IsChecked = false;
            EditRLSToolButton.IsChecked = EditRLS.IsChecked = false;
            EditPathToolButton.IsChecked = EditPath.IsChecked = false;
            EditFlareToolButton.IsChecked = EditFlare.IsChecked = true;
            editMode = EditMode.Flare;
            modelScrollViewer.Cursor = Cursors.Pen;
        }

        public void EditDelete_Click(object sender, RoutedEventArgs e)
        {
            switch (selObject.Type)
            {
                case TargetObject.RLS:
                {
                    RLSPos_Delete(selObject.Index);
                }; break;
                case TargetObject.Path:
                {

                }; break;
            }
            selObject.Index = -1;
            selObject.Type = TargetObject.None;
            EditProp.IsEnabled = false;
            EditDelete.IsEnabled = false;
        }

        public void UpdateRLSIndexes()
        {
            foreach (RLSPosition rlsPos in rlModel.RLSPositionList)
                foreach (RLS rls in rlsPos.rlsList)
                    rls.Pathes[VSR].Tag = rls.Pathes[GRID].Tag = rlModel.RLSPositionList.IndexOf(rlsPos);
        }

        public void RLSPos_Delete(int ind)
        {
            foreach (RLS rls in rlModel.RLSPositionList[ind].rlsList)
            {
                modelCanvas.Children.Remove(rls.Pathes[GRID]);
                modelCanvas.Children.Remove(rls.Pathes[VSR]);
            }
            rlModel.RLSPositionList.RemoveAt(ind);
            UpdateRLSIndexes();
        }

        #endregion MenuEdit

        #region MenuView

        public void ViewZoomIn_Click(object sender, RoutedEventArgs e)
        {
            scaleSlider.Value += 0.1;
        }

        public void ViewZoomOut_Click(object sender, RoutedEventArgs e)
        {
            scaleSlider.Value -= 0.1;
        }

        #endregion MenuView

        #region MenuProject

        public void PrjOptions_Click(object sender, RoutedEventArgs e)
        {
            PrjOptionsWindow prjOptionsWindow = new PrjOptionsWindow();
            prjOptionsWindow.heightComboBox.SelectedIndex = (int)((rlModel.Height - 500) / 500);
            prjOptionsWindow.widthComboBox.SelectedIndex = (int)((rlModel.Width - 500) / 500);
            prjOptionsWindow.gridVisibleCheckBox.IsChecked = (bool?)rlModel.GridVisible;
            prjOptionsWindow.syncDoubCheckBox.IsChecked = (bool?)rlModel.SyncDoub;
            prjOptionsWindow.gridXStepComboBox.SelectedIndex = (int)((rlModel.GridXStep - 50) / 50);
            prjOptionsWindow.gridYStepComboBox.SelectedIndex = (int)((rlModel.GridYStep - 50) / 50);
            prjOptionsWindow.durTextBox.Text = rlModel.Duration.ToString();
            if (prjOptionsWindow.ShowDialog() == (bool?)true)
            {
                rlModel.Height = prjOptionsWindow.heightComboBox.SelectedIndex * 500 + 500;
                rlModel.Width = prjOptionsWindow.widthComboBox.SelectedIndex * 500 + 500;
                rlModel.GridVisible = (bool)prjOptionsWindow.gridVisibleCheckBox.IsChecked;
                rlModel.SyncDoub = (bool)prjOptionsWindow.syncDoubCheckBox.IsChecked;
                rlModel.GridXStep = prjOptionsWindow.gridXStepComboBox.SelectedIndex * 50 + 50;
                rlModel.GridYStep = prjOptionsWindow.gridYStepComboBox.SelectedIndex * 50 + 50;
                rlModel.Duration = TimeSpan.Parse(prjOptionsWindow.durTextBox.Text);
                rlModel.playStrBoard.Duration = TimeSpan.Parse(prjOptionsWindow.durTextBox.Text);
                UpdateRLModel();
            }
        }

        #endregion MenuProject

        #region ToolBar

        public void scaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.OldValue != 0)
            {
                modelScrollViewer.ScrollToHorizontalOffset((e.NewValue / e.OldValue) *
                    (modelScrollViewer.HorizontalOffset + (modelScrollViewer.ViewportWidth / 2))
                                                        - (modelScrollViewer.ViewportWidth / 2));
                modelScrollViewer.ScrollToVerticalOffset((e.NewValue / e.OldValue) *
                    (modelScrollViewer.VerticalOffset + (modelScrollViewer.ViewportHeight / 2))
                                                      - (modelScrollViewer.ViewportHeight / 2));
            }
        }

        #endregion ToolBar

        #region ModelCanvas

        enum EditMode { Hand, RLS, Path, PathDraw, Flare };
        private EditMode editMode;
        public RLModel rlModel;

        private TargetObject trgObject;
        private int trgIndex;

        private Point crtMousePosition;
        private Point prvMousePosition;
        private bool trgDrag;
        
        public void modelCanvas_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            Point mViewerPos = Mouse.GetPosition(modelScrollViewer);
            Point mCanvasPos = Mouse.GetPosition(modelCanvas);
            switch (editMode)
            {
                case (EditMode.Hand):
                    {
                        prvMousePosition = mViewerPos;
                        crtMousePosition = prvMousePosition;
                    } break;
                case (EditMode.RLS):
                    {
                        if (trgObject == TargetObject.None)
                        {
                            RLSListWindow dlg = new RLSListWindow();
                            dlg.rlsList = new List<RLS> { };
                            dlg.ShowDialog();
                            if (dlg.DialogResult == (bool?)true)
                            {
                                RLSPosition rlsPos = new RLSPosition(mCanvasPos);
                                rlModel.RLSPositionList.Add(rlsPos);
                                foreach (RLS rls in dlg.rlsList)
                                {
                                    rls.RateAnimation = new DoubleAnimation(0.0, 360.0,
                                                                            TimeSpan.FromMinutes(1 / rls.Rate));
                                    rls.RateAnimation.RepeatBehavior = RepeatBehavior.Forever;
                                    string UID = SetUID();
                                    this.RegisterName(UID, rls.RateTransform);
                                    rlModel.playStrBoard.Children.Add(rls.RateAnimation);
                                    RLSDraw(rlsPos.Position, rls);
                                    Storyboard.SetTargetName(rls.RateAnimation, UID);
                                    Storyboard.SetTargetProperty(rls.RateAnimation, new PropertyPath(RotateTransform.AngleProperty));
                                    rlsPos.rlsList.Add(rls);
                                    rls.Pathes[GRID].Tag = rlModel.RLSPositionList.IndexOf(rlsPos);
                                    rls.Pathes[VSR].Tag = rlModel.RLSPositionList.IndexOf(rlsPos);
                                }
                            }
                        }
                        else if (trgObject == TargetObject.RLS)
                        {
                            prvMousePosition = mCanvasPos;
                            crtMousePosition = prvMousePosition;
                            //MessageBox.Show("РЛС. Индекс = " + trgIndex.ToString() + Environment.NewLine +
                            //                "Выделана РЛС = " + selTrgIndex);
                        }
                    } break;
                case (EditMode.Path):
                    {
                        if (trgObject == TargetObject.None)
                        {
                            newTracePolyLine = new Polyline();
                            newTracePolyLine.Stroke = Brushes.Blue;
                            newTracePolyLine.StrokeThickness = 5.0;
                            newTracePolyLine.StrokeDashArray.Add(10.0);
                            newTracePolyLine.StrokeDashArray.Add(5.0);
                            newTracePolyLine.Points.Add(mCanvasPos);
                            newTracePolyLine.Points.Add(mCanvasPos);
                            modelCanvas.Children.Add(newTracePolyLine);
                            editMode = EditMode.PathDraw;
                        }
                    } break;
                case (EditMode.PathDraw):
                    {
                        if ((newTracePolyLine.Points.Count > 1) &&
                            (newTracePolyLine.Points[newTracePolyLine.Points.Count - 2] == mCanvasPos))
                        {
                            double traceLength = 0.0;
                            for (int i = 1; i < newTracePolyLine.Points.Count; i++)
                                traceLength += Math.Sqrt(Math.Pow((newTracePolyLine.Points[i].Y - newTracePolyLine.Points[i - 1].Y), 2) +
                                                         Math.Pow((newTracePolyLine.Points[i].X - newTracePolyLine.Points[i - 1].X), 2));
                            traceLength /= RLModel.PX2KM; // graphic to real length
                            TraceOptionsWindow dlg = new TraceOptionsWindow();
                            dlg.lenghtTextBox.Text = traceLength.ToString("F");
                            dlg.ShowDialog();
                            if (dlg.DialogResult == (bool?)true)
                            {
                                FOTrace newFOTrace = new FOTrace();
                                newFOTrace.Length = traceLength;
                                newFOTrace.Speed = dlg.speedSlider.Value;
                                newFOTrace.IndvNumb = UInt32.Parse(dlg.indvNumbTextBox.Text);
                                newFOTrace.BgnOT = double.Parse(dlg.bgnOTTextBox.Text);
                                newFOTrace.EndOT = double.Parse(dlg.endOTTextBox.Text);
                                newFOTrace.BgnH = double.Parse(dlg.bgnHTextBox.Text);
                                newFOTrace.EndH = double.Parse(dlg.endHTextBox.Text);
                                newFOTrace.Trouble = dlg.troubleCheckBox.IsChecked.Value;

                                for (int i = 0; i < newTracePolyLine.Points.Count; i++)
                                    newFOTrace.PntList.Add(new Point(newTracePolyLine.Points[i].X / RLModel.PX2KM,
                                                                     newTracePolyLine.Points[i].Y / RLModel.PX2KM));
                                double a = -Math.Atan((newTracePolyLine.Points[1].X - newTracePolyLine.Points[0].X) /
                                  (newTracePolyLine.Points[1].Y - newTracePolyLine.Points[0].Y)) * 180 / Math.PI;
                                if (newTracePolyLine.Points[1].Y - newTracePolyLine.Points[0].Y >= 0) a += 180;
                                newFOTrace.FORotateTransform = new RotateTransform(a - 90.0, 0.0, 0.0);
                                newFOTrace.FOTransTransform = new TranslateTransform(newTracePolyLine.Points[0].X,
                                                                                     newTracePolyLine.Points[0].Y);

                                #region SetPlayAnimation
                                newFOTrace.FOXAnim = new DoubleAnimationUsingPath();
                                newFOTrace.FOXAnim.RepeatBehavior = new RepeatBehavior(1);
                                if (newFOTrace.Speed != 0.0)
                                    newFOTrace.FOXAnim.Duration = new Duration(TimeSpan.FromHours(newFOTrace.Length / newFOTrace.Speed));
                                else
                                    newFOTrace.FOXAnim.Duration = Duration.Forever;
                                string UID = SetUID();
                                this.RegisterName(UID, newFOTrace.FOTransTransform);
                                Storyboard.SetTargetName(newFOTrace.FOXAnim, UID);
                                Storyboard.SetTargetProperty(newFOTrace.FOXAnim, new PropertyPath(TranslateTransform.XProperty));
                                newFOTrace.FOXAnim.Source = PathAnimationSource.X;

                                newFOTrace.FOYAnim = new DoubleAnimationUsingPath();
                                newFOTrace.FOYAnim.RepeatBehavior = new RepeatBehavior(1);
                                if (newFOTrace.Speed != 0.0)
                                    newFOTrace.FOYAnim.Duration = new Duration(TimeSpan.FromHours(newFOTrace.Length / newFOTrace.Speed));
                                else
                                    newFOTrace.FOYAnim.Duration = Duration.Forever;
                                UID = SetUID();
                                this.RegisterName(UID, newFOTrace.FOTransTransform);
                                Storyboard.SetTargetName(newFOTrace.FOYAnim, UID);
                                Storyboard.SetTargetProperty(newFOTrace.FOYAnim, new PropertyPath(TranslateTransform.YProperty));
                                newFOTrace.FOYAnim.Source = PathAnimationSource.Y;

                                newFOTrace.FOAAnim = new DoubleAnimationUsingPath();
                                newFOTrace.FOAAnim.RepeatBehavior = new RepeatBehavior(1);
                                if (newFOTrace.Speed != 0.0)
                                    newFOTrace.FOAAnim.Duration = new Duration(TimeSpan.FromHours(newFOTrace.Length / newFOTrace.Speed));
                                else
                                    newFOTrace.FOAAnim.Duration = Duration.Forever;
                                UID = SetUID();
                                this.RegisterName(UID, newFOTrace.FORotateTransform);
                                Storyboard.SetTargetName(newFOTrace.FOAAnim, UID);
                                Storyboard.SetTargetProperty(newFOTrace.FOAAnim, new PropertyPath(RotateTransform.AngleProperty));
                                newFOTrace.FOAAnim.Source = PathAnimationSource.Angle;
                                #endregion SetPlayAnimation

                                TraceDraw(newFOTrace);

                                rlModel.playStrBoard.Children.Add(newFOTrace.FOXAnim);
                                rlModel.playStrBoard.Children.Add(newFOTrace.FOYAnim);
                                rlModel.playStrBoard.Children.Add(newFOTrace.FOAAnim);

                                rlModel.FOTraceList.Add(newFOTrace);
                            }
                            newTracePolyLine.Points.Clear();
                            editMode = EditMode.Path;
                        }
                        else newTracePolyLine.Points.Add(mCanvasPos);
                    } break;
            }
        }

        public void modelCanvas_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            trgObject = TargetObject.None;
            trgIndex = -1;
        }

        public void modelCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mViewerPos = Mouse.GetPosition(modelScrollViewer);
            Point mCanvasPos = Mouse.GetPosition(modelCanvas);
            switch (editMode)
            {
                case (EditMode.Hand):
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        prvMousePosition = crtMousePosition;
                        crtMousePosition = mViewerPos;
                        double xOffset = crtMousePosition.X - prvMousePosition.X;
                        double yOffset = crtMousePosition.Y - prvMousePosition.Y;
                        modelScrollViewer.ScrollToHorizontalOffset(modelScrollViewer.HorizontalOffset - xOffset);
                        modelScrollViewer.ScrollToVerticalOffset(modelScrollViewer.VerticalOffset - yOffset);
                    } break;
                case (EditMode.RLS):
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        prvMousePosition = crtMousePosition;
                        crtMousePosition = mCanvasPos;
                        double xOffset = crtMousePosition.X - prvMousePosition.X;
                        double yOffset = crtMousePosition.Y - prvMousePosition.Y;
                        if (trgDrag)
                        {
                            rlModel.RLSPositionList[trgIndex].Position.X += (xOffset / RLModel.PX2KM);
                            rlModel.RLSPositionList[trgIndex].Position.Y += (yOffset / RLModel.PX2KM);
                            foreach (RLS rls in rlModel.RLSPositionList[trgIndex].rlsList)
                            {
                                rls.pathTrans.X += xOffset;
                                rls.pathTrans.Y += yOffset;
                            }
                            foreach (Path pth in selObject.Pathes)
                            {
                                (pth.RenderTransform as TranslateTransform).X += xOffset;
                                (pth.RenderTransform as TranslateTransform).Y += yOffset;
                            }
                        }
                    } break;
                case (EditMode.PathDraw):
                    {
                        newTracePolyLine.Points[newTracePolyLine.Points.Count - 1] = mCanvasPos;
                    } break;
            }
        }

        public void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                newTracePolyLine.Points.Clear();
                editMode = EditMode.Path;
            }
        }
        
        public void rls_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            trgObject = TargetObject.RLS;
            trgDrag = true;
            trgIndex = (int)(sender as Path).Tag;
            if (editMode == EditMode.RLS)
            {
                if (selObject.Index != trgIndex)
                {
                    foreach (Path pth in selObject.Pathes)
                        modelCanvas.Children.Remove(pth);
                    Obj_Select(sender, TargetObject.RLS);
                }
                selObject.Index = trgIndex;
                selObject.Type = TargetObject.RLS;
                
                EditProp.IsEnabled = true;
                EditDelete.IsEnabled = true;
            }
        }

        public void rls_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            trgDrag = false;
        }

        #endregion ModelCanvas

        #region BottomPanel

        enum PlayMode { Stop, Play, Pause };
        private PlayMode playMode;

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            switch (playMode)
            {
                case PlayMode.Stop:
                {
                    playMode = PlayMode.Play;
                    this.playButton.Content = (Image)this.Resources["PauseIcon"];
                    rlModel.playStrBoard.BeginTime = TimeSpan.FromMilliseconds(playSlider.Value);
                    if (playSlider.Value == 0.0) rlModel.playStrBoard.Begin();
                    else rlModel.playStrBoard.Resume();

                    if (buildModeComboBox.SelectedIndex == 1 &&
                        buildCmpltCheckBox.IsChecked.Value)
                    {
                        if (udpThread.ThreadState != ThreadState.Running)
                        {
                            if (udpThread.ThreadState == ThreadState.Stopped)
                            {
                                udpThread = new System.Threading.Thread(udpThread_Action);
                                udpThread.Priority = System.Threading.ThreadPriority.AboveNormal;
                            }
                            udpThread.Start();
                            udpCdgInd = 0;
                        }
                    }
                }; break;
                case PlayMode.Play:
                {
                    playMode = PlayMode.Pause;
                    this.playButton.Content = (Image)this.Resources["PlayIcon"];
                    rlModel.playStrBoard.Pause();

                    if (buildModeComboBox.SelectedIndex == 1)
                    {
                        udpThread.Suspend();
                    }
                }; break;
                case PlayMode.Pause:
                {
                    playMode = PlayMode.Play;
                    this.playButton.Content = (Image)this.Resources["PauseIcon"];
                    rlModel.playStrBoard.Resume();

                    if (buildModeComboBox.SelectedIndex == 1)
                    {
                        udpThread.Resume();
                    }
                }; break;
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (playMode != PlayMode.Stop)
            {
                playMode = PlayMode.Stop;
                this.playButton.Content = (Image)this.Resources["PlayIcon"];
                rlModel.playStrBoard.Pause();
                rlModel.playStrBoard.BeginTime = TimeSpan.FromMilliseconds(0.0);
                playSlider.Value = 0.0;
                rlModel.playStrBoard.Stop();
                if (udpThread.ThreadState == ThreadState.Suspended) udpThread.Resume();
                udpThread.Abort();
            }
        }

        private void playSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (playMode != PlayMode.Play)
            {
                if (rlModel.playStrBoard.Duration.HasTimeSpan)
                    rlModel.playStrBoard.Seek(TimeSpan.FromMilliseconds(playSlider.Value));
            }
        }

        private void buildModeComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            switch (buildModeComboBox.SelectedIndex)
            {
                case 0:
                {
                    playSlider.IsEnabled = true;
                    buildButton.IsEnabled = false;
                    buildBrowseButton.IsEnabled = false;
                } break;
                case 1:
                {
                    playSlider.IsEnabled = false;
                    buildButton.IsEnabled = true;
                    buildBrowseButton.IsEnabled = false;
                    //stopButton_Click(this, e);
                } break;
            }
        }

        private void buildButton_Click(object sender, RoutedEventArgs e)
        {
            buildWindow = new BuildWindow();
            buildWindow.durTextBlock.Text = rlModel.playStrBoard.Duration.TimeSpan.ToString();
            Thread buildThread = new Thread(buildAction);
            buildCmpltCheckBox.IsChecked = false;
            buildThread.Start();
            buildWindow.ShowDialog();
            if (buildWindow.DialogResult.Value == true)
            {
                buildCmpltCheckBox.IsChecked = true;
                buildBrowseButton.IsEnabled = true;
            }
            else
            {
                buildThread.Abort();
                buildCmpltCheckBox.IsChecked = false;
                buildBrowseButton.IsEnabled = false;
            }
        }

        private void buildBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            BuildSaveWindow chDlg = new BuildSaveWindow();
            for (int i = 0; i < cdgStream.ctgNameList.Count; i++)
            {
                if (i < consts.MAX_CDG_CHN)
                {
                    (((chDlg.ctgListBox.Items[i] as ListBoxItem).Content as Grid).Children[1] as TextBlock).Text = cdgStream.ctgNameList[i];
                    (((chDlg.ctgListBox.Items[i] as ListBoxItem).Content as Grid).Children[2] as TextBlock).Text =
                        cdgStream.ctgEPList[i].Address.ToString() + ":" +
                        cdgStream.ctgEPList[i].Port.ToString();
                    cdgStream.chNmbList.Add(i);
                }
                else
                {

                }
            }
            if (chDlg.ShowDialog().Value == true)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Title = "Сохранить поток кодограмм";
                dlg.Filter = "Поток кодограмм (*.rlc)|*.rlc";
                dlg.DefaultExt = "rlc";
                dlg.InitialDirectory = crtDirectory;
                dlg.RestoreDirectory = false;
                if (dlg.ShowDialog().Value == true)
                {
                    string fn = dlg.FileName;
                    System.IO.FileStream fs =
                        new System.IO.FileStream(fn,
                                                 System.IO.FileMode.Create,
                                                 System.IO.FileAccess.Write);
                    // Использование каналов
                    UInt16 useMask = 0x0000;
                    for (int j = 15; j > -1; j--)
                    {
                        bool add = false;
                        for (int i = 0; i < cdgStream.ctgNameList.Count; i++)
                            if (cdgStream.chNmbList[i] == j) add = true;
                        useMask *= 2;
                        if (add) useMask += 1;
                    }
                    fs.WriteByte((byte)(useMask / 256));
                    fs.WriteByte((byte)(useMask % 256));
                    // IP-адреса
                    for (int i = 0; i < consts.MAX_CDG_CHN; i++)
                    {
                        bool chUsed = false;
                        for (int j = 0; j < cdgStream.ctgNameList.Count; j++)
                            if (cdgStream.chNmbList[j] == i)
                            {
                                chUsed = true;
                                // IP-address
                                byte[] ip = cdgStream.ctgEPList[j].Address.GetAddressBytes();
                                fs.WriteByte(ip[0]); fs.WriteByte(ip[1]);
                                fs.WriteByte(ip[2]); fs.WriteByte(ip[3]);
                                // Port
                                fs.WriteByte((byte)(cdgStream.ctgEPList[j].Port / 256));
                                fs.WriteByte((byte)(cdgStream.ctgEPList[j].Port % 256));
                            }
                        if (!chUsed)
                        {
                            fs.WriteByte(0x00); fs.WriteByte(0x00);
                            fs.WriteByte(0x00); fs.WriteByte(0x00);
                            fs.WriteByte(0x00); fs.WriteByte(0x00);
                        }
                    }
                    int cdgCnt = 0;
                    foreach (StrmCdg strmCdg in cdgStream.cdgList)
                    {
                        // Номер канала
                        fs.WriteByte((byte)(strmCdg.chNmb));
                        // Метка времени
                        UInt16 val = (UInt16)(strmCdg.cdg.pos % 0x10000);
                        fs.WriteByte((byte)(val / 0x100));
                        fs.WriteByte((byte)(val % 0x100));
                        // Кодограмма
                        fs.Write(strmCdg.cdg.cont, 0, strmCdg.cdg.length);
                        cdgCnt++;
                    }
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        #endregion BottomPanel

        #region Miscellaneous

        private int lastIndex;

        private string SetUID()
        {
            string s = "RLS" + lastIndex.ToString();
            lastIndex++;
            return s;
        }

        #endregion miscellaneous
    }
}
