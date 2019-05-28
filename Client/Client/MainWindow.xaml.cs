using HaffmansArhDLL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Windows.Forms;

namespace Client
{
    // "Component" - базовый класс для всех элементов управления
    abstract class Component
    {
        public abstract void Save(string PathSourseFile, string PathEndFile, string endFileNameAndFolder);
    }
    // "Decorator" - базовый класс для всех декораторов
    abstract class Decorator : Component
    {
        // Декорируемый элемент управления
        protected Component component = null;

        public void SetComponent(Component component)
        {
            this.component = component;
        }

        public override void Save(string PathSourseFile, string PathEndFile, string endFileNameAndFolder)
        {
            // Если задан декорируемый компонент - то отрисовать его
            if (component != null)
            {
                component.Save(PathSourseFile, PathEndFile, endFileNameAndFolder);
            }
        }
    }
    // "ConcreteDecoratorA" - объявление конкретного декоратора - рамка вокруг
    class Shifrator : Decorator
    {
        private string addedState;

        public Shifrator(Component component)
        {
            this.component = component;
        }

        public override void Save(string PathSourseFile, string PathEndFile, string endFileNameAndFolder)
        {
            base.Save(PathSourseFile, PathEndFile, endFileNameAndFolder);
            addedState = "New State";
            Console.WriteLine("Border.Draw()");
        }
    }








    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = "c:\\";
            fileDialog.Filter = "All files (*.*)|*.*|All files+ (*.*)|*.*";
            fileDialog.FilterIndex = 1;
            fileDialog.Title = "Выберите файл для сжатия";
            if (fileDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Выберите файл для сохранения или дополнения существующего архива";
                saveFileDialog.Filter = "All files (*.*)|*.*|Compressed files+ (*.hmc)|*.hmc";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.FileName = fileDialog.SafeFileName+".hmc";

                saveFileDialog.AddExtension = false;
                saveFileDialog.CheckFileExists = false;
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    saveFileDialog.InitialDirectory = /*System.IO.Path.GetDirectoryName*/(fileDialog.FileName);
                    
                    HaffmansArh haffmansArh = new HaffmansArh();
                    haffmansArh.CodingFiles(fileDialog.FileName, saveFileDialog.FileName,Path.GetFileName(fileDialog.FileName));
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = "c:\\";
            fileDialog.Filter = "All files (*.*)|*.*|Compressed files (*.hmc)|*.hmc";
            fileDialog.FilterIndex = 2;
            fileDialog.Title = "Выберите архив для разжатия";
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.SelectedPath = System.IO.Path.GetDirectoryName(fileDialog.FileName);
                folderBrowserDialog.Description = "Выберите директорию для сохранения содержимого";
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    HaffmansArh haffmansArh = new HaffmansArh();
                    haffmansArh.EncodingFile(fileDialog.FileName, folderBrowserDialog.SelectedPath);
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = "c:\\";
            folderBrowserDialog.Description = "Выберите директорию которую необходимо сжать";
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Выберите файл для сохранения или дополнения существующего архива";
                saveFileDialog.Filter = "All files (*.*)|*.*|Compressed files+ (*.hmc)|*.hmc";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.FileName = Path.GetFileName(folderBrowserDialog.SelectedPath) + ".hmc";

                saveFileDialog.AddExtension = false;
                saveFileDialog.CheckFileExists = false;
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    HaffmansArh haffmansArh = new HaffmansArh();
                    haffmansArh.CodingFolder(folderBrowserDialog.SelectedPath, saveFileDialog.FileName, Path.GetDirectoryName(folderBrowserDialog.SelectedPath));
                }
            }
        }
    }
   
    
}
