using System;
using System.Windows;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ASAIProgImitator
{
    public partial class MainWindow : Window
    {
        private bool Load_rlModel(string fn)
        {
            FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            this.rlModel = (RLModel)bf.Deserialize(fs);
            UpdateAnim();
            UpdateRLModel();
            return true;
        }

        private bool Save_rlModel(string fn)
        {
            FileStream fs = new FileStream(fn, FileMode.Create, FileAccess.Write);
            // Сериализация (вручную)
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, rlModel);

            fs.Close();
            return true;
        }
    }
}