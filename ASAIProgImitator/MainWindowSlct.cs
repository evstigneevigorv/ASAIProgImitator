using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;

namespace ASAIProgImitator
{
    public enum TargetObject { None, RLS, Path, Flare };

    public class SelObject
    {
        public TargetObject Type;
        public int Index;

        public RectangleGeometry RectGeom;
        public List<Path> Pathes;

        public SelObject()
        {
            // Инициализация полей
            this.Index = -1;
            this.Type = TargetObject.None;
            this.Pathes = new List<Path> { };
            // Подготовка графического примитива для селектора
            this.RectGeom =
                new RectangleGeometry(new Rect(-3.0 * RLModel.PX2KM,
                                               -3.0 * RLModel.PX2KM,
                                                6.0 * RLModel.PX2KM,
                                                6.0 * RLModel.PX2KM));
        }
    }

    public partial class MainWindow : Window
    {
        private SelObject selObject;

        private void Obj_Select(object selObj, TargetObject type)
        {
            TranslateTransform trn = (selObj as Path).RenderTransform as TranslateTransform;

            Path p = new Path();
            p.Data = selObject.RectGeom;
            p.Stroke = Brushes.Blue;
            p.Fill = Brushes.White;
            p.StrokeThickness = 5.0;
            p.RenderTransform =
                new TranslateTransform(trn.X,
                                       trn.Y);
            selObject.Pathes.Add(p);
            modelCanvas.Children.Add(p);
        }
    }
}
