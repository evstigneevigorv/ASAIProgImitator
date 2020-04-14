using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace ASAIProgImitator
{
    public partial class MainWindow : Window
    {
        private Polyline newTracePolyLine;

        private void RLSDraw(Point p, RLS rls)
        {
            rls.Pathes = new List<Path> { };
            Path gridPath = new Path();
            GeometryGroup RLSGroup = new GeometryGroup();
            for (int i = 1; i <= (int)(rls.Distance / rls.DStep); i++)
            {
                EllipseGeometry rnd = new EllipseGeometry(new Point(0, 0),
                                                          i * rls.DStep * RLModel.PX2KM,
                                                          i * rls.DStep * RLModel.PX2KM);
                RLSGroup.Children.Add(rnd);
            }
            rls.pathTrans = new TranslateTransform(p.X * RLModel.PX2KM,
                                                   p.Y * RLModel.PX2KM);
            for (int i = (int)(360.0 / rls.AStep); i > 0; i--)
            {
                LineGeometry radLine = new LineGeometry(new Point(0, 0),
                                                        new Point((rls.Distance + 10) * RLModel.PX2KM * Math.Cos(Math.PI * i * rls.AStep / 180.0),
                                                                  (rls.Distance + 10) * RLModel.PX2KM * Math.Sin(Math.PI * i * rls.AStep / 180.0)));
                RLSGroup.Children.Add(radLine);
            }
            gridPath.Data = RLSGroup;
            gridPath.Stroke = Brushes.DarkGreen;
            gridPath.StrokeThickness = 3;
            gridPath.Cursor = Cursors.Arrow;
            gridPath.RenderTransform = rls.pathTrans;

            Path vsrPath = new Path();
            vsrPath.Data = (PathGeometry)this.Resources["vsrPathGeom"];
            vsrPath.Stroke = Brushes.DarkGreen;
            vsrPath.StrokeThickness = 2;
            vsrPath.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            TransformGroup vsrTransform = new TransformGroup();
            vsrTransform.Children.Add(new ScaleTransform(rls.Distance * RLModel.PX2KM / 1000.0,
                                                         rls.Distance * RLModel.PX2KM / 1000.0, 0, 0));
            vsrTransform.Children.Add(new RotateTransform(rls.Visir));
            vsrTransform.Children.Add(rls.RateTransform);
            vsrTransform.Children.Add(rls.pathTrans);
            vsrPath.RenderTransform = vsrTransform;

            rls.Pathes.Add(gridPath);
            rls.Pathes.Add(vsrPath);

            modelCanvas.Children.Add(gridPath);
            modelCanvas.Children.Add(vsrPath);

            gridPath.MouseLeftButtonDown += new MouseButtonEventHandler(rls_MouseLeftButtonDown);
            gridPath.MouseLeftButtonUp += new MouseButtonEventHandler(rls_MouseLeftButtonUp);
        }

        private void TraceDraw(FOTrace trace)
        {
            Path tracePath = new Path();
            PathGeometry traceGeom = new PathGeometry();
            PathFigure traceFig = new PathFigure();

            traceFig.StartPoint = new Point(trace.PntList[0].X * RLModel.PX2KM,
                                            trace.PntList[0].Y * RLModel.PX2KM);
            for (int i = 1; i < trace.PntList.Count; i++)
                traceFig.Segments.Add(new LineSegment(new Point(trace.PntList[i].X * RLModel.PX2KM,
                                                                trace.PntList[i].Y * RLModel.PX2KM),
                                                      true));
            traceGeom.Figures.Add(traceFig);
            tracePath.Data = traceGeom;
            tracePath.Stroke = Brushes.DarkBlue;
            tracePath.StrokeThickness = 3.0;
            tracePath.StrokeDashArray.Add(10.0);
            tracePath.StrokeDashArray.Add(5.0);

            Path foPath = new Path();
            PathGeometry foGeom = new PathGeometry();
            PathFigure foFig = new PathFigure(new Point(0, 0),
                                              new List<PathSegment>
                                                  {
                                                      new LineSegment(new Point(-10, 4), true),
                                                      new LineSegment(new Point(0, -14), true),
                                                      new LineSegment(new Point(10, 4), true),
                                                  }, true);
            foGeom.Figures.Add(foFig);
            foPath.Data = foGeom;
            foPath.Stroke = Brushes.Cyan;
            foPath.StrokeThickness = 3.0;
            foPath.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            TransformGroup foTransGroup = new TransformGroup();

            foTransGroup.Children.Add(new RotateTransform(90.0, 0.0, 0.0));
            foTransGroup.Children.Add(trace.FORotateTransform);
            foTransGroup.Children.Add(trace.FOTransTransform);
            foPath.RenderTransform = foTransGroup;

            modelCanvas.Children.Add(tracePath);
            modelCanvas.Children.Add(foPath);

            trace.FOXAnim.PathGeometry = traceGeom;
            trace.FOYAnim.PathGeometry = traceGeom;
            trace.FOAAnim.PathGeometry = traceGeom;
        }
    }
}
