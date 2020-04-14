using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ASAIProgImitator
{
    public partial class MainWindow : Window
    {
        private BuildWindow buildWindow;
        private CdgStream cdgStream;
        public bool[] chUsdAry;
        private DrctDiag drctDiag;

        private double curPos;
        private double progValue;
        private string curRLSName;

        private delegate void DispatcherDelegate();

        private void buildAction()
        {
            progValue = 0.0;
            double durTime = rlModel.Duration.TotalMilliseconds;
            Point foPos = new Point();
            Point foDrct = new Point();
            // Построение отдельно для каждой РЛС / Категории
            for (int i = 0; i < rlModel.RLSPositionList.Count; i++)
            {
                for (int j = 0; j < rlModel.RLSPositionList[i].rlsList.Count; j++)
                {
                    // Псевдонимы
                    RLS rls = rlModel.RLSPositionList[i].rlsList[j];

                    // Инициализация раскадровки для построения
                    int rfCnt = 0;
                    curRLSName = rls.Name;
                    // mainUIDispatcher.Invoke(new DispatcherDelegate(buildInit));
                    buildInit();
                    drctDiag = new DrctDiag();
                    switch (rls.DiagType)
                    {
                        case RLSDiagType.Uniform:
                            { drctDiag.uniformInit(rls.Width); } break;
                        case RLSDiagType.SinX:
                            { drctDiag.sinxInit(rls.Width); } break;
                    }
                    double curVsrAzm = rls.Visir; double prvVsrAzm = rls.Visir;
                    double progStep = 100.0 * 2 * rls.Distance /
                            (rlModel.RLSPositionList.Count * rlModel.RLSPositionList[i].rlsList.Count *
                             consts.LIGHT_VEL * durTime / 1000.0);
                    double strpPeriod = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond *
                            2 * rls.Distance / consts.LIGHT_VEL)).TotalMilliseconds;

                    CtgPrcs.distList = new List<DistTrace> { };
                    CtgPrcs.Prepare(rls);
                  
                    while (curPos < durTime)
                    {
                        CtgPrcs.distList.Clear();
                        // Начало нового периода
                        switch (rls.Type)
                        {
                            case RLSType.PRL: { if (rls.Ctgs[1].IsEnabled) CtgPrcs.Ctg255PRL_BgnPrd(rls, curVsrAzm); } break;
                            case RLSType.VRL: { if (rls.Ctgs[2].IsEnabled) CtgPrcs.Ctg255VRL_BgnPrd(rls, curVsrAzm); } break;
                            case RLSType.NRZ: { if (rls.Ctgs[2].IsEnabled) CtgPrcs.Ctg255NRZ_BgnPrd(rls, curVsrAzm); } break;
                        }

                        // Предварительные вычисления
                        for (int l = 0; l < rlModel.FOTraceList.Count; l++)
                        {
                            // Псевдоним
                            FOTrace trace = rlModel.FOTraceList[l];
                            // Определение взаиморасположения РЛС и ВО
                            if (trace.Speed != 0.0)
                            {
                                if (curPos <= 3600000 * trace.Length / trace.Speed)
                                    trace.buildGeom.GetPointAtFractionLength(curPos / (3600000.0 * trace.Length / trace.Speed), out foPos, out foDrct);
                                else
                                    trace.buildGeom.GetPointAtFractionLength(1.0, out foPos, out foDrct);
                            }
                            else
                                trace.buildGeom.GetPointAtFractionLength(0.0, out foPos, out foDrct);

                            double range = Math.Sqrt(Math.Pow(rlModel.RLSPositionList[i].Position.X - foPos.X, 2) +
                                                     Math.Pow(rlModel.RLSPositionList[i].Position.Y - foPos.Y, 2));

                            double angle = 90.0 + Math.Atan2((foPos.Y - rlModel.RLSPositionList[i].Position.Y),
                                                             (foPos.X - rlModel.RLSPositionList[i].Position.X)) * 180.0 / Math.PI;
                            if (angle < 0.0) angle += 360.0;
                            double delta = curVsrAzm - angle;
                            if (delta >= 180.0) delta -= 360.0;
                            else if (delta < -180.0) delta += 360.0;

                            // Список дальностей
                            switch (rls.Type)
                            {
                                #region ПРЛ
                                case RLSType.PRL:
                                {
                                    if ((range < rls.Distance) &&
                                        (Math.Abs(delta) < 22.5) &&
                                        (drctDiag.form[(int)(512 * (22.5 + delta) / 45.0)] > rls.Trsh))
                                        CtgPrcs.distList.Add(new DistTrace(l, range, delta));
                                } break;
                                #endregion ПРЛ

                                #region ВРЛ/НРЗ
                                case RLSType.VRL:
                                case RLSType.NRZ:
                                {
                                    if ((range < rls.Distance) &&
                                        (Math.Abs(delta) < rls.Width * 360.0 / 4096.0))
                                        CtgPrcs.distList.Add(new DistTrace(l, range, delta));
                                } break;
                                #endregion ВРЛ/НРЗ
                            }
                        }
                        
                        // Основная обработка
                        IEnumerable<DistTrace> ordDistBfr = CtgPrcs.distList.OrderBy(da => da.Range);
                        switch (rls.Type)
                        {
                            #region ПРЛ
                            case RLSType.PRL:
                            {
                                // Обработка текущего периода
                                double implDur = (4 / 3) * rls.StrDuration * 0.3; // в километрах
                                UInt16 curFstInd = 0; UInt16 curLstInd = 0;
                                UInt16 clsFstInd = 0; UInt16 clsLstInd = 0;
                                List<double> amplList = new List<double> { };
                                for (int d = 0; d < ordDistBfr.Count(); d++)
                                {
                                    // Псевдонимы
                                    DistTrace dt = ordDistBfr.ElementAt(d);
                                    FOTrace trace = rlModel.FOTraceList[dt.TraceIndex];

                                    int ai = (int)(512 * (22.5 + dt.Delta) / 45.0);
                                    double ampl = drctDiag.form[ai];
                                    curFstInd = (UInt16)Math.Ceiling(((dt.Range + (rls.Trsh / ampl) * (implDur / 4)) / consts.DSC));
                                    curLstInd = (UInt16)Math.Floor(((dt.Range + implDur - (rls.Trsh / ampl) * (implDur / 4)) / consts.DSC));

                                    if (amplList.Count == 0)
                                    {
                                        for (int q = curFstInd; q <= curLstInd; q++)
                                            amplList.Add(ampl * StrpForm(q * consts.DSC - dt.Range, implDur));
                                        clsFstInd = curFstInd; clsLstInd = curLstInd;
                                    }
                                    else
                                    {
                                        if (curFstInd > clsLstInd + 1)
                                        {
                                            // Выдача данных
                                            if (rls.Ctgs[0].IsEnabled) CtgPrcs.Ctg107PRL_CrtPrd(rls, clsFstInd, clsLstInd, curVsrAzm);
                                            if (rls.Ctgs[1].IsEnabled) CtgPrcs.Ctg255PRL_CrtPrd(rls, clsFstInd, amplList);

                                            // Очистка буфера амплитуд
                                            amplList.Clear();

                                            for (int q = curFstInd; q <= curLstInd; q++)
                                                amplList.Add(ampl * StrpForm(q * consts.DSC - dt.Range, implDur));
                                            clsFstInd = curFstInd; clsLstInd = curLstInd;
                                        }
                                        else
                                        {
                                            // Наложение амплитуд
                                            for (int q = curFstInd; q <= curLstInd; q++)
                                                if (q - clsFstInd < amplList.Count)
                                                {
                                                    double a = Math.Sqrt(Math.Pow(amplList[q - clsFstInd], 2) +
                                                                         Math.Pow(ampl * StrpForm(q * consts.DSC - ordDistBfr.ElementAt(d).Range, implDur), 2));
                                                    if (a <= 100.0) amplList[q - clsFstInd] = a;
                                                    else amplList[q - clsFstInd] = 100.0;
                                                }
                                                else
                                                    amplList.Add(ampl * StrpForm(q * consts.DSC - ordDistBfr.ElementAt(d).Range, implDur));
                                            if (curLstInd > clsLstInd) clsLstInd = curLstInd;
                                        }
                                    }
                                    
                                    if (d == ordDistBfr.Count() - 1)
                                    {
                                        // Выдача данных
                                        if (rls.Ctgs[0].IsEnabled) CtgPrcs.Ctg107PRL_CrtPrd(rls, clsFstInd, clsLstInd, curVsrAzm);
                                        if (rls.Ctgs[1].IsEnabled) CtgPrcs.Ctg255PRL_CrtPrd(rls, clsFstInd, amplList);

                                        // Очистка буфера амплитуд
                                        amplList.Clear();
                                    }
                                }

                                // Завершение текущего периода
                                if (rls.Ctgs[1].IsEnabled) CtgPrcs.Ctg255PRL_EndPrd(rls, curPos, rlModel.SyncDoub);

                                // Завершение текущего сектора
                                if ((rls.Ctgs[0].IsEnabled) &&
                                    ((byte)(curVsrAzm / consts.SCT_WDT) !=
                                     (byte)(prvVsrAzm / consts.SCT_WDT)))
                                    CtgPrcs.Ctg107PRL_EndSct(rls, curVsrAzm, curPos, rlModel.SyncDoub);
                            } break;
                            #endregion ПРЛ

                            #region ВРЛ
                            case RLSType.VRL:
                            {
                                // Обработка текущего периода
                                for (int d = 0; d < ordDistBfr.Count(); d++)
                                {
                                    // Псевдоним
                                    DistTrace dt = ordDistBfr.ElementAt(d);
                                    // Выдача данных
                                    if (rls.Ctgs[2].IsEnabled) CtgPrcs.Ctg255VRL_CrtPrd(rls,
                                                                                        rlModel.FOTraceList[dt.TraceIndex],
                                                                                        (UInt16)(dt.Range * 40),
                                                                                        curPos);
                                }
                                // Завершение текущего периода
                                if (rls.Ctgs[2].IsEnabled) CtgPrcs.Ctg255VRL_EndPrd(rls,
                                                                                    curPos,
                                                                                    rlModel.SyncDoub);
                            } break;
                            #endregion ВРЛ

                            #region НРЗ
                            case RLSType.NRZ:
                            {
                                // Обработка текущего периода
                                for (int d = 0; d < ordDistBfr.Count(); d++)
                                {
                                    // Псевдоним
                                    DistTrace dt = ordDistBfr.ElementAt(d);
                                    // Выдача данных
                                    if (rls.Ctgs[2].IsEnabled) CtgPrcs.Ctg255NRZ_CrtPrd(rls,
                                                                                        rlModel.FOTraceList[dt.TraceIndex],
                                                                                        (UInt16)(dt.Range * 40),
                                                                                        curPos);
                                }
                                // Завершение текущего периода
                                if (rls.Ctgs[2].IsEnabled) CtgPrcs.Ctg255NRZ_EndPrd(rls,
                                                                                    curPos,
                                                                                    rlModel.SyncDoub);
                            } break;
                            #endregion НРЗ
                        }
                        
                        // Шаг вперед
                        curPos += strpPeriod;
                        progValue += progStep;
                        prvVsrAzm = curVsrAzm;
                        curVsrAzm += 360.0 * rlModel.RLSPositionList[i].rlsList[j].Rate * strpPeriod / 60000;
                        if (curVsrAzm >= 360.0) curVsrAzm -= 360.0;

                        if (rfCnt < int.MaxValue) rfCnt++;
                        else rfCnt = 0;
                        if (rfCnt % 1000 == 0) mainUIDispatcher.Invoke(new DispatcherDelegate(buildUpdate));
                    }
                }
            }

            #region Сборка общего потока кодограмм
            cdgStream = new CdgStream();
            for (int i = 0; i < rlModel.RLSPositionList.Count; i++)
                for (int j = 0; j < rlModel.RLSPositionList[i].rlsList.Count; j++)
                    for (int k = 0; k < rlModel.RLSPositionList[i].rlsList[j].Ctgs.LongLength; k++)
                    {
                        if (rlModel.RLSPositionList[i].rlsList[j].Ctgs[k].IsEnabled)
                        {
                            cdgStream.ctgEPList.Add(
                                new System.Net.IPEndPoint(rlModel.RLSPositionList[i].rlsList[j].Ctgs[k].EndPoint.Address,
                                                          rlModel.RLSPositionList[i].rlsList[j].Ctgs[k].EndPoint.Port));
                            #region Построение имени категории
                            string ctgName = rlModel.RLSPositionList[i].rlsList[j].Name + " - ";
                            switch (rlModel.RLSPositionList[i].rlsList[j].Type)
                            {
                                case RLSType.PRL:
                                {
                                    switch (k)
                                    {
                                        case 0: { ctgName += "Категория 107"; } break;
                                        case 1: { ctgName += "Категория 255"; } break;
                                    }
                                } break;
                                case RLSType.VRL:
                                {
                                    switch (k)
                                    {
                                        case 0: { ctgName += "Категория 1"; } break;
                                        case 1: { ctgName += "Категория 2"; } break;
                                    }
                                } break;
                                case RLSType.NRZ:
                                {
                                    switch (k)
                                    {
                                        case 0: { ctgName += "Категория 1"; } break;
                                        case 1: { ctgName += "Категория 2"; } break;
                                    }
                                } break;
                            }
                            #endregion
                            cdgStream.ctgNameList.Add(ctgName);
                            rlModel.RLSPositionList[i].rlsList[j].Ctgs[k].CdgInd = 0;
                        }
                    }
            bool notRdy = true;
            while (notRdy)
            {
                notRdy = false;
                int[] sel = new int[3] {-1, -1, -1};
                int chCnt = -1;
                int selCh = -1;
                double minPos = double.MaxValue;
                for (int i = 0; i < rlModel.RLSPositionList.Count; i++)
                    for (int j = 0; j < rlModel.RLSPositionList[i].rlsList.Count; j++)
                        for (int k = 0; k < rlModel.RLSPositionList[i].rlsList[j].Ctgs.Length; k++)
                        {
                            Category ctg = rlModel.RLSPositionList[i].rlsList[j].Ctgs[k];
                            if (ctg.IsEnabled) chCnt++;
                            if (ctg.CdgInd != ctg.CdgList.Count)
                            {
                                notRdy = true;
                                if (ctg.CdgList[ctg.CdgInd].pos < minPos)
                                {
                                    sel[0] = i; sel[1] = j; sel[2] = k;
                                    selCh = chCnt;
                                    minPos = ctg.CdgList[ctg.CdgInd].pos;
                                }
                            }
                        }
                if (sel[0] != -1)
                {
                    if (selCh < consts.MAX_CDG_CHN)
                        cdgStream.cdgList.Add(new StrmCdg(rlModel.RLSPositionList[sel[0]].rlsList[sel[1]].Ctgs[sel[2]].CdgList[rlModel.RLSPositionList[sel[0]].rlsList[sel[1]].Ctgs[sel[2]].CdgInd],
                                                          selCh));
                    rlModel.RLSPositionList[sel[0]].rlsList[sel[1]].Ctgs[sel[2]].CdgInd++;
                }
            }
            #endregion Сборка общего потока кодограмм

            // Отображение готовности
            curPos = durTime;
            progValue = 100.0;
            mainUIDispatcher.Invoke(new DispatcherDelegate(buildUpdate));

            // Остановка после проигрывания
            mainUIDispatcher.Invoke(new DispatcherDelegate(buildRestore));
        }

        private void buildInit()
        {
            curPos = 0.0;
            foreach (FOTrace trace in rlModel.FOTraceList)
            {
                trace.buildGeom = new PathGeometry();
                PathFigure traceFig = new PathFigure();

                traceFig.StartPoint = new Point(trace.PntList[0].X,
                                                trace.PntList[0].Y);
                for (int i = 1; i < trace.PntList.Count; i++)
                    traceFig.Segments.Add(new LineSegment(new Point(trace.PntList[i].X,
                                                                    trace.PntList[i].Y),
                                                          true));
                trace.buildGeom.Figures.Add(traceFig);
            }
        }

        private void buildRestore()
        {
            rlModel.playStrBoard.Stop();
            playMode = PlayMode.Stop;
        }

        private void buildUpdate()
        {
            buildWindow.timeTextBlock.Text = TimeSpan.FromMilliseconds(curPos).ToString();
            buildWindow.progressBar.Value = progValue;
            buildWindow.rlsNameTextBlock.Text = curRLSName;
            if (buildWindow.progressBar.Value == 100.0) buildWindow.cancelButton.Content = " Готово ";
        }

        public class DrctDiag
        {
            public double[] form;

            public DrctDiag()
            {
                this.form = new double[512];
            }

            public void uniformInit(int wdt)
            {
                for (int i = -256; i < 256; i++)
                    if ((i > Math.Round(-wdt / 2.0)) &&
                        (i < Math.Round(wdt / 2.0))) form[i + 256] = 50.0;
                    else
                        form[i + 256] = 0.0;
            }

            public void sinxInit(int wdt)
            {
                for (int i = -256; i < 256; i++)
                    if (i != 0) form[i + 256] = 50.0 * Math.Abs(Math.Sin(0.5 * Math.PI * i / wdt) /
                                                                        (0.5 * Math.PI * i / wdt));
                    else form[i + 256] = 50.0;
            }
        }

        public double StrpForm(double x, double lngt)
        {
            double rslt;
            if (x < lngt / 4) rslt = x / (lngt / 4);
            else if (x > 3 * lngt / 4) rslt = (lngt - x) / (lngt / 4);
            else rslt = 1.0;
            return rslt;
        }

        public double DFunc(double d, double dist)
        {
            double rslt;
            if (d < (dist / 10)) rslt = 1.0;
            else rslt = 0.0001 * Math.Pow((dist / d), 4);
            return rslt;
        }
    }
}
