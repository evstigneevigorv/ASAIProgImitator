using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASAIProgImitator
{
    public partial class MainWindow : Window
    {
        public class CtgPrcs
        {
            public static List<DistTrace> distList;
            public static int amplCnt;

            public static void Prepare(RLS rls)
            {
                for (int i = 0; i < rls.Ctgs.Length; i++)
                    if (rls.Ctgs[i].IsEnabled)
                        switch (rls.Type)
                        {
                            case RLSType.PRL:
                            {
                                switch (i)
                                {
                                    // Категория 107 ПРЛ
                                    case 0:
                                    {
                                        rls.Ctgs[0].Cdg = new byte[consts.MAX_CDG_LENGTH];
                                        rls.Ctgs[0].CdgList = new List<Cdg> { };
                                        rls.Ctgs[0].Cdg[0] = 0xAA;
                                        rls.Ctgs[0].Cdg[1] = 0x02;
                                        rls.Ctgs[0].Cdg[2] = 107;
                                        rls.Ctgs[0].Cdg[3] = 0x00;
                                        rls.Ctgs[0].Cdg[4] = 0x00;
                                        rls.Ctgs[0].CdgInd = 5;
                                    } break;
                                    // Категория 255 ПРЛ
                                    case 1:
                                    {
                                        // Шапка
                                        rls.Ctgs[1].Cdg = new byte[consts.MAX_CDG_LENGTH];
                                        rls.Ctgs[1].CdgList = new List<Cdg> { };
                                        rls.Ctgs[1].Cdg[0] = 0xAA;
                                        rls.Ctgs[1].Cdg[1] = 0x02;
                                        rls.Ctgs[1].Cdg[2] = 255;
                                        rls.Ctgs[1].Cdg[3] = 0x00;
                                        rls.Ctgs[1].Cdg[4] = 0x00;
                                        // Идентификатор канала
                                        rls.Ctgs[1].Cdg[5] = (byte)(rls.ChNmb);
                                        // Инициализация номера кадра
                                        // (с учетом инкремента)
                                        rls.Ctgs[1].Cdg[6] = 0xFF;
                                        rls.Ctgs[1].Cdg[7] = 0xFF;
                                    } break;
                                    // Категория не используется
                                    case 2:
                                    {

                                    } break;
                                }
                            } break;
                            case RLSType.VRL:
                            {
                                switch (i)
                                {
                                    // Категория 1 ВРЛ
                                    case 0:
                                    {

                                    } break;
                                    // Категория 2 ВРЛ
                                    case 1:
                                    {

                                    } break;
                                    // Категория 255 ВРЛ
                                    case 2:
                                    {
                                        // Шапка
                                        rls.Ctgs[2].Cdg = new byte[consts.MAX_CDG_LENGTH];
                                        rls.Ctgs[2].CdgList = new List<Cdg> { };
                                        rls.Ctgs[2].Cdg[0] = 0xAA;
                                        rls.Ctgs[2].Cdg[1] = 0x02;
                                        rls.Ctgs[2].Cdg[2] = 255;
                                        rls.Ctgs[2].Cdg[3] = 0x00;
                                        rls.Ctgs[2].Cdg[4] = 0x00;
                                        // Идентификатор канала
                                        rls.Ctgs[2].Cdg[5] = (byte)(0x05 + rls.ChNmb);
                                        // Инициализация номера кадра
                                        // (с учетом инкремента)
                                        rls.Ctgs[2].Cdg[6] = 0xFF;
                                        rls.Ctgs[2].Cdg[7] = 0xFF;
                                    } break;
                                }
                            } break;
                            case RLSType.NRZ:
                            {
                                switch (i)
                                {
                                    // Категория 1 НРЗ
                                    case 0:
                                    {

                                    } break;
                                    // Категория 2 НРЗ
                                    case 1:
                                    {

                                    } break;
                                    // Категория 255 НРЗ
                                    case 2:
                                    {
                                        // Шапка
                                        rls.Ctgs[2].Cdg = new byte[consts.MAX_CDG_LENGTH];
                                        rls.Ctgs[2].CdgList = new List<Cdg> { };
                                        rls.Ctgs[2].Cdg[0] = 0xAA;
                                        rls.Ctgs[2].Cdg[1] = 0x02;
                                        rls.Ctgs[2].Cdg[2] = 255;
                                        rls.Ctgs[2].Cdg[3] = 0x00;
                                        rls.Ctgs[2].Cdg[4] = 0x00;
                                        // Идентификатор канала
                                        rls.Ctgs[2].Cdg[5] = (byte)(0x03 + rls.ChNmb);
                                        // Инициализация номера кадра
                                        // (с учетом инкремента)
                                        rls.Ctgs[2].Cdg[6] = 0xFF;
                                        rls.Ctgs[2].Cdg[7] = 0xFF;
                                    } break;
                                }
                            } break;
                        }
            }

            #region ПРЛ
            // Категория 107
            public static void Ctg107PRL_CrtPrd(RLS rls,
                                    UInt16 fstIndex,
                                    UInt16 lstIndex,
                                    double curVsrAzm)
            {
                if (fstIndex < (UInt16)Math.Ceiling(consts.CTG_107PRL_MAX_DIST / consts.DSC))
                {
                    // Азимут
                    rls.Ctgs[0].Cdg[rls.Ctgs[0].CdgInd] = 0x00; rls.Ctgs[0].CdgInd++;
                    rls.Ctgs[0].Cdg[rls.Ctgs[0].CdgInd] = (byte)(curVsrAzm * 16 / 360.0); rls.Ctgs[0].CdgInd++;
                    rls.Ctgs[0].Cdg[rls.Ctgs[0].CdgInd] = (byte)((curVsrAzm * 4096 / 360.0) % 256); rls.Ctgs[0].CdgInd++;
                    // Дальность
                    rls.Ctgs[0].Cdg[rls.Ctgs[0].CdgInd] = (byte)(0x30 + fstIndex / 256); rls.Ctgs[0].CdgInd++;
                    rls.Ctgs[0].Cdg[rls.Ctgs[0].CdgInd] = (byte)(fstIndex % 256); rls.Ctgs[0].CdgInd++;
                    rls.Ctgs[0].Cdg[rls.Ctgs[0].CdgInd] = (byte)(lstIndex - fstIndex); rls.Ctgs[0].CdgInd++;
                }
            }
            public static void Ctg107PRL_EndSct(RLS rls,
                                                double curVsrAzm,
                                                double curPos,
                                                bool syncDoub)
            {
                rls.Ctgs[0].Cdg[rls.Ctgs[0].CdgInd] = 0xFF; rls.Ctgs[0].CdgInd++;
                rls.Ctgs[0].Cdg[rls.Ctgs[0].CdgInd] = 0xFF; rls.Ctgs[0].CdgInd++;
                rls.Ctgs[0].Cdg[rls.Ctgs[0].CdgInd] = (byte)((curVsrAzm / consts.SCT_WDT) + 0x08); rls.Ctgs[0].CdgInd++;
                rls.Ctgs[0].Cdg[rls.Ctgs[0].CdgInd] = 0xAA; rls.Ctgs[0].CdgInd++;
                rls.Ctgs[0].Cdg[rls.Ctgs[0].CdgInd] = 0x03; rls.Ctgs[0].CdgInd++;

                // Дублирование синхробайта в БД
                byte[] cdgBytes = new byte[consts.MAX_CDG_LENGTH];
                cdgBytes[0] = 0xAA; cdgBytes[1] = 0x02;
                int k = 2;
                for (int l = 2; l < (rls.Ctgs[0].CdgInd - 2); l++)
                {
                    if (syncDoub)
                    {
                        if (rls.Ctgs[0].Cdg[l] == consts.SYNC_BYTE)
                        {
                            cdgBytes[k] = consts.SYNC_BYTE; k++;
                        }
                    }
                    cdgBytes[k] = rls.Ctgs[0].Cdg[l]; k++;
                }
                cdgBytes[3] = (byte)((k - 2) / 256.0);
                cdgBytes[4] = (byte)((k - 2) % 256.0);
                cdgBytes[k] = 0xAA; cdgBytes[k + 1] = 0x03;

                rls.Ctgs[0].CdgList.Add(new Cdg(cdgBytes,
                                                k + 2,
                                                curPos));
                rls.Ctgs[0].CdgInd = 5;
            }
            // Категория 255
            public static void Ctg255PRL_BgnPrd(RLS rls,
                                                double curVsrAzm)
            {
                // Номер кадра
                if (rls.Ctgs[1].Cdg[7] < 0xFF)
                    rls.Ctgs[1].Cdg[7]++;
                else
                {
                    rls.Ctgs[1].Cdg[7] = 0x00;
                    if (rls.Ctgs[1].Cdg[6] < 0xFF)
                        rls.Ctgs[1].Cdg[6]++;
                    else
                        rls.Ctgs[1].Cdg[6] = 0x00;
                }
                // Азимут
                rls.Ctgs[1].Cdg[8] = 0x10;
                rls.Ctgs[1].Cdg[9] = (byte)(curVsrAzm * 256 / 360.0);
                rls.Ctgs[1].Cdg[10] = (byte)((curVsrAzm * 65536 / 360.0) % 256);
                // С учетом ИК и азимута
                rls.Ctgs[1].CdgInd = 11;
                amplCnt = 0;
            }

            public static void Ctg255PRL_CrtPrd(RLS rls,
                                                UInt16 clsFstInd,
                                                List<double> amplList)
            {
                if (amplCnt <= consts.CTG_255PRL_MAX_AMPL)
                {
                    if (rls.Distance > 409.5)
                    {
                        // Дальность [Дискреты по 400 м]
                        rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = 0x21; rls.Ctgs[1].CdgInd++;
                        rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = (byte)((clsFstInd / 4) / 256); rls.Ctgs[1].CdgInd++;
                        rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = (byte)((clsFstInd / 4) % 256); rls.Ctgs[1].CdgInd++;
                        // Амплитуды
                        for (int i = 0; i < amplList.Count; i += 4)
                        {
                            if (amplCnt <= consts.CTG_255PRL_MAX_AMPL)
                            {
                                rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = 0x32; rls.Ctgs[1].CdgInd++;
                                rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = (byte)((amplList[i] * 655.35) / 256); rls.Ctgs[1].CdgInd++;
                                rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = (byte)((amplList[i] * 655.35) % 256); rls.Ctgs[1].CdgInd++;
                                amplCnt++;
                            }
                        }
                    }
                    else
                    {
                        // Дальность [Дискреты по 100 м]
                        rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = 0x20; rls.Ctgs[1].CdgInd++;
                        rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = (byte)(clsFstInd / 256); rls.Ctgs[1].CdgInd++;
                        rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = (byte)(clsFstInd % 256); rls.Ctgs[1].CdgInd++;
                        // Амплитуды
                        for (int i = 0; i < amplList.Count; i++)
                        {
                            if (amplCnt <= consts.CTG_255PRL_MAX_AMPL)
                            {
                                rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = 0x32; rls.Ctgs[1].CdgInd++;
                                rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = (byte)((amplList[i] * 655.35) / 256); rls.Ctgs[1].CdgInd++;
                                rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = (byte)((amplList[i] * 655.35) % 256); rls.Ctgs[1].CdgInd++;
                                amplCnt++;
                            }
                        }
                    }
                }
            }

            public static void Ctg255PRL_EndPrd(RLS rls,
                                                double curPos,
                                                bool syncDoub)
            {
                // Признак УД
                if (amplCnt > consts.CTG_255PRL_MAX_AMPL)
                {
                    rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = 0x40; rls.Ctgs[1].CdgInd++;
                    rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = 0x00; rls.Ctgs[1].CdgInd++;
                    rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = (byte)((amplCnt - consts.CTG_255PRL_MAX_AMPL) % 256); rls.Ctgs[1].CdgInd++;
                }
                amplCnt = 0;
                rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = 0xAA; rls.Ctgs[1].CdgInd++;
                rls.Ctgs[1].Cdg[rls.Ctgs[1].CdgInd] = 0x03; rls.Ctgs[1].CdgInd++;

                // Дублирование синхробайта в БД
                byte[] cdgBytes = new byte[consts.MAX_CDG_LENGTH];
                cdgBytes[0] = 0xAA; cdgBytes[1] = 0x02;
                int k = 2;
                for (int l = 2; l < (rls.Ctgs[1].CdgInd - 2); l++)
                {
                    if (syncDoub)
                    {
                        if (rls.Ctgs[1].Cdg[l] == consts.SYNC_BYTE)
                        {
                            cdgBytes[k] = consts.SYNC_BYTE; k++;
                        }
                    }
                    cdgBytes[k] = rls.Ctgs[1].Cdg[l]; k++;
                }
                cdgBytes[3] = (byte)((k - 2) / 256.0);
                cdgBytes[4] = (byte)((k - 2) % 256.0);
                cdgBytes[k] = 0xAA; cdgBytes[k + 1] = 0x03;

                rls.Ctgs[1].CdgList.Add(new Cdg(cdgBytes,
                                                k + 2,
                                                curPos));
            }
            #endregion ПРЛ

            #region ВРЛ
            // Категория 255
            public static void Ctg255VRL_BgnPrd(RLS rls,
                                                double curVsrAzm)
            {
                // Номер кадра
                if (rls.Ctgs[2].Cdg[7] < 0xFF)
                    rls.Ctgs[2].Cdg[7]++;
                else
                {
                    rls.Ctgs[2].Cdg[7] = 0x00;
                    if (rls.Ctgs[2].Cdg[6] < 0xFF)
                        rls.Ctgs[2].Cdg[6]++;
                    else
                        rls.Ctgs[2].Cdg[6] = 0x00;
                }
                // Азимут
                rls.Ctgs[2].Cdg[8] = 0x10;
                rls.Ctgs[2].Cdg[9] = (byte)(curVsrAzm * 256 / 360.0);
                rls.Ctgs[2].Cdg[10] = (byte)((curVsrAzm * 65536 / 360.0) % 256);
                // С учетом ИК и азимута
                rls.Ctgs[2].CdgInd = 11;
            }

            public static void Ctg255VRL_CrtPrd(RLS rls,
                                                FOTrace trace,
                                                UInt16 fstIndex,
                                                double curPos)
            {
                // Дальность
                rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)0x20; rls.Ctgs[2].CdgInd++;
                rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(fstIndex / 256); rls.Ctgs[2].CdgInd++;
                rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(fstIndex % 256); rls.Ctgs[2].CdgInd++;
                if (rls.ReqSignal == RLSReqSignal.IndvNumb)
                {
                    // Индивидуальный номер
                    rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(0x60 + (trace.IndvNumb / 65536)); rls.Ctgs[2].CdgInd++;
                    rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)((trace.IndvNumb % 65536) / 256); rls.Ctgs[2].CdgInd++;
                    rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)((trace.IndvNumb % 256)); rls.Ctgs[2].CdgInd++;
                }
                else
                {
                    // Высота / топливо
                    // Остаток топлива
                    double k;
                    if (trace.Speed != 0.0)
                    {
                        if (curPos <= 3600000 * trace.Length / trace.Speed)
                            k = 3600000 * trace.Length / trace.Speed;
                        else k = 1.0;
                    }
                    else k = 0.0;
                    double ot = trace.BgnOT + (trace.EndOT - trace.BgnOT) * k;
                    double h = trace.BgnH + (trace.EndH - trace.BgnH) * k;
                    if (ot <= 50.0)
                        rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(0x70 + ot / 5.0);
                    else
                        rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(0x70 + 10.0 + (ot - 50.0) / 10);
                    rls.Ctgs[2].CdgInd++;
                    // Бедствие/норма
                    if (trace.Trouble) rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = 0xC0;
                    else rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = 0x40;
                    rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] += (byte)(h * 100.0 / 256);rls.Ctgs[2].CdgInd++;
                    rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(h * 100.0 % 256); rls.Ctgs[2].CdgInd++;
                }
            }

            public static void Ctg255VRL_EndPrd(RLS rls,
                                                double curPos,
                                                bool syncDoub)
            {
                rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = 0xAA; rls.Ctgs[2].CdgInd++;
                rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = 0x03; rls.Ctgs[2].CdgInd++;

                // Дублирование синхробайта в БД
                byte[] cdgBytes = new byte[consts.MAX_CDG_LENGTH];
                cdgBytes[0] = 0xAA; cdgBytes[1] = 0x02;
                int k = 2;
                for (int l = 2; l < (rls.Ctgs[2].CdgInd - 2); l++)
                {
                    if (syncDoub)
                    {
                        if (rls.Ctgs[2].Cdg[l] == consts.SYNC_BYTE)
                        {
                            cdgBytes[k] = consts.SYNC_BYTE; k++;
                        }
                    }
                    cdgBytes[k] = rls.Ctgs[2].Cdg[l]; k++;
                }
                cdgBytes[3] = (byte)((k - 2) / 256.0);
                cdgBytes[4] = (byte)((k - 2) % 256.0);
                cdgBytes[k] = 0xAA; cdgBytes[k + 1] = 0x03;

                rls.Ctgs[2].CdgList.Add(new Cdg(cdgBytes,
                                                k + 2,
                                                curPos));
            }
            #endregion ВРЛ

            #region НРЗ
            // Категория 255
            public static void Ctg255NRZ_BgnPrd(RLS rls,
                                                double curVsrAzm)
            {
                // Номер кадра
                if (rls.Ctgs[2].Cdg[7] < 0xFF)
                    rls.Ctgs[2].Cdg[7]++;
                else
                {
                    rls.Ctgs[2].Cdg[7] = 0x00;
                    if (rls.Ctgs[2].Cdg[6] < 0xFF)
                        rls.Ctgs[2].Cdg[6]++;
                    else
                        rls.Ctgs[2].Cdg[6] = 0x00;
                }
                // Азимут
                rls.Ctgs[2].Cdg[8] = 0x10;
                rls.Ctgs[2].Cdg[9] = (byte)(curVsrAzm * 256 / 360.0);
                rls.Ctgs[2].Cdg[10] = (byte)((curVsrAzm * 65536 / 360.0) % 256);
                // Состояние НРЗ
                if (rls.Ctgs[2].Cdg[7] == 0x00)
                {
                    rls.Ctgs[2].Cdg[11] = 0x80;
                    rls.Ctgs[2].Cdg[12] = 0x00;
                    rls.Ctgs[2].Cdg[13] = 0x00;
                    for (int b = 0; b < consts.NRZ_STO_WDT; b++)
                        if (rls.StatNRZ[b]) rls.Ctgs[2].Cdg[13] += (byte)Math.Pow(2, b);
                    // С учетом ИК и азимута
                    rls.Ctgs[2].CdgInd = 14;
                }
                else rls.Ctgs[2].CdgInd = 11;
            }

            public static void Ctg255NRZ_CrtPrd(RLS rls,
                                                FOTrace trace,
                                                UInt16 fstIndex,
                                                double curPos)
            {
                // Дальность
                rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)0x20; rls.Ctgs[2].CdgInd++;
                rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(fstIndex / 256); rls.Ctgs[2].CdgInd++;
                rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(fstIndex % 256); rls.Ctgs[2].CdgInd++;
                if (rls.ReqSignal == RLSReqSignal.IndvNumb)
                {
                    // Индивидуальный номер
                    rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(0x60 + (trace.IndvNumb / 65536)); rls.Ctgs[2].CdgInd++;
                    rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)((trace.IndvNumb % 65536) / 256); rls.Ctgs[2].CdgInd++;
                    rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)((trace.IndvNumb % 256)); rls.Ctgs[2].CdgInd++;
                }
                else
                {
                    // Высота / топливо
                    // Остаток топлива
                    double k;
                    if (trace.Speed != 0.0)
                    {
                        if (curPos <= 3600000 * trace.Length / trace.Speed)
                            k = 3600000 * trace.Length / trace.Speed;
                        else k = 1.0;
                    }
                    else k = 0.0;
                    double ot = trace.BgnOT + (trace.EndOT - trace.BgnOT) * k;
                    double h = trace.BgnH + (trace.EndH - trace.BgnH) * k;
                    if (ot <= 50.0)
                        rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(0x70 + ot / 5.0);
                    else
                        rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(0x70 + 10.0 + (ot - 50.0) / 10);
                    rls.Ctgs[2].CdgInd++;
                    // Бедствие/норма
                    if (trace.Trouble) rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = 0xC0;
                    else rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = 0x40;
                    rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] += (byte)(h * 100.0 / 256); rls.Ctgs[2].CdgInd++;
                    rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = (byte)(h * 100.0 % 256); rls.Ctgs[2].CdgInd++;
                }
            }

            public static void Ctg255NRZ_EndPrd(RLS rls,
                                                double curPos,
                                                bool syncDoub)
            {
                rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = 0xAA; rls.Ctgs[2].CdgInd++;
                rls.Ctgs[2].Cdg[rls.Ctgs[2].CdgInd] = 0x03; rls.Ctgs[2].CdgInd++;

                // Дублирование синхробайта в БД
                byte[] cdgBytes = new byte[consts.MAX_CDG_LENGTH];
                cdgBytes[0] = 0xAA; cdgBytes[1] = 0x02;
                int k = 2;
                for (int l = 2; l < (rls.Ctgs[2].CdgInd - 2); l++)
                {
                    if (syncDoub)
                    {
                        if (rls.Ctgs[2].Cdg[l] == consts.SYNC_BYTE)
                        {
                            cdgBytes[k] = consts.SYNC_BYTE; k++;
                        }
                    }
                    cdgBytes[k] = rls.Ctgs[2].Cdg[l]; k++;
                }
                cdgBytes[3] = (byte)((k - 2) / 256.0);
                cdgBytes[4] = (byte)((k - 2) % 256.0);
                cdgBytes[k] = 0xAA; cdgBytes[k + 1] = 0x03;

                rls.Ctgs[2].CdgList.Add(new Cdg(cdgBytes,
                                                k + 2,
                                                curPos));
            }
            #endregion НРЗ
        }

        public class DistTrace
        {
            public int TraceIndex;
            public double Range;
            public double Delta;

            public DistTrace(int ind, double range, double delta)
            {
                this.TraceIndex = ind;
                this.Range = range;
                this.Delta = delta;
            }
        }
    }
}
