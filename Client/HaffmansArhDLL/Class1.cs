using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HaffmansArhDLL
{
    public class Node
    {
        public int N { get; set; }
        public byte C { get; set; }
        public Node right { get; set; }
        public Node left { get; set; }
    }
    public class HaffmansArh
    {
        int[] Dict = new int[256];//частотный словарь
        List<bool> code = new List<bool>();//код символа
        Dictionary<byte, List<bool>> table = new Dictionary<byte, List<bool>>();//таблица символов

        void BuildTable(Node root)//рекурсивная функция записи кодов в словарь
        {
            if (root.left != null)
            {
                code.Add(false);
                BuildTable(root.left);
            }

            if (root.right != null)
            {
                code.Add(true);
                BuildTable(root.right);
            }

            if (root.left == null && root.right == null)
            {
                List<bool> tt = new List<bool>();
                foreach (bool item in code)
                {
                    tt.Add(item);
                }
                table[root.C] = tt;
            }
            //code.Clear();

            if (code.Count > 0)
            {
                code.RemoveAt(code.Count - 1);
            }

        }

        public void EncodingFile(string PathSourseFile, string PathEndFolder)
        {
            FileStream sourseFileStream = File.Open(PathSourseFile, FileMode.Open);
            for (; ; )
            {

                byte leng = (byte)sourseFileStream.ReadByte();//считывание размера имени

                byte[] tempBytename = new byte[leng];
                sourseFileStream.Read(tempBytename, 0, leng);
                string fileName = "";
                foreach (var item in tempBytename)
                {
                    fileName = fileName + (char)item;//востановление имени
                }

                int endFilecountBytes = 0;//получение размера исходного файла и построение дерева
                for (int i = 0; i < 256; i++)
                {
                    byte[] tempByte = new byte[4];
                    sourseFileStream.Read(tempByte, 0, 4);
                    int asd = BitConverter.ToInt32(tempByte, 0);
                    endFilecountBytes += Dict[i] = asd;
                }

                List<Node> t = new List<Node>();
                for (int i = 0; i < 256; i++)//создание листа нодов из словаря
                {
                    t.Add(new Node { C = (byte)i, N = Dict[i] });
                }

                while (t.Count != 1)//отбор с самых низких значений, и создание дерева нодов с одним оставшимся рутом
                {
                    Node a1 = t[0];

                    foreach (Node item in t)
                    {
                        if (a1.N > item.N)
                        {
                            a1 = item;
                        }
                    }
                    t.Remove(a1);

                    Node a2 = t[0];

                    foreach (Node item in t)
                    {
                        if (a2.N > item.N)
                        {
                            a2 = item;
                        }
                    }
                    t.Remove(a2);

                    Node p = new Node() { left = a1, right = a2, N = a1.N + a2.N };
                    t.Add(p);
                }

                Node root = t[0];
                table.Clear();
                BuildTable(root);//рекурсивная функция записи кодов в словарь

                //считывание размера считываемого массива
                byte[] tempByteL = new byte[4];
                sourseFileStream.Read(tempByteL, 0, 4);
                int inputFileLength = BitConverter.ToInt32(tempByteL, 0);

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(PathEndFolder + "\\" + fileName));
                }
                catch (Exception) { }

                FileStream endFileStream = File.Create(PathEndFolder + "\\" + fileName);//создание стрима для записи файла
                
                int counterOfEndFile = 0;
                Node temp = root;

                for (int i = 0; i < inputFileLength; i++)
                {
                    byte tempByte = (byte)sourseFileStream.ReadByte();
                    
                    for (int j = 0; j < 8; j++)
                    {
                        int tr = ((tempByte >> 7 - j) & 0x01);

                        if (counterOfEndFile == endFilecountBytes)
                        {
                            continue;
                        }
                        if (temp.left == null && temp.right == null)
                        {
                            endFileStream.WriteByte(temp.C);
                            counterOfEndFile++;
                            if (tr == 0)
                            {
                                temp = root.left;
                            }
                            else
                            {
                                temp = root.right;
                            }
                        }
                        else if (tr == 0)
                        {
                            temp = temp.left;
                        }
                        else if (tr == 1)
                        {
                            temp = temp.right;
                        }
                    }
                }

                endFileStream.Close();

                if (sourseFileStream.Position == sourseFileStream.Length)
                {
                    //File.WriteAllText(@"c:\temp\ThathAll", "");

                    sourseFileStream.Close();

                    return;
                }
            }
        }

        public void CodingFile(string PathSourseFile, string PathEndFile)
        {
            //byte[] WorkingString = File.ReadAllBytes(PathSourseFile);

            FileStream inputFile = File.Open(PathSourseFile, FileMode.Open);

            for (int i = 0; i < 256; i++)//обнуление частотного словаря
            {
                Dict[i] = 0;
            }
            for (int i = 0; i < inputFile.Length; i++)
            {
                int byt = inputFile.ReadByte();
                Dict[byt] = Dict[byt] + 1;
            }
            inputFile.Seek(0, SeekOrigin.Begin);
            /*
            foreach (byte item in WorkingString)//создание частотного словаря
            {
                Dict[item] = Dict[(int)item] + 1;
            }*/
            
            List<Node> t = new List<Node>();

            for (int i = 0; i < 256; i++)//создание листа нодов из словаря
            {
                t.Add(new Node { C = (byte)i, N = Dict[i] });
            }

            while (t.Count != 1)//отбор с самых низких значений, и создание дерева нодов с одним оставшимся рутом
            {
                Node a1 = t[0];

                foreach (Node item in t)
                {
                    if (a1.N > item.N)
                    {
                        a1 = item;
                    }
                }
                t.Remove(a1);

                Node a2 = t[0];

                foreach (Node item in t)
                {
                    if (a2.N > item.N)
                    {
                        a2 = item;
                    }
                }
                t.Remove(a2);

                Node p = new Node() { left = a1, right = a2, N = a1.N + a2.N };
                t.Add(p);
            }
            Node root = t[0];
            table.Clear();//очистка словаря
            BuildTable(root);//рекурсивная функция записи кодов в словарь

            int countOfBits = 0;

            for (int i = 0; i < 256; i++)
            {
                countOfBits += table[(byte)i].Count * Dict[(byte)i];//подсчёт количества Бит конечного файла
            }


            ////////////////////////////////////////////////////////////////////////////////
            //сохранение файла
            string filename = Path.GetFileName(PathSourseFile);
            byte[] filNamBytStr = new byte[filename.Length];
            for (int i = 0; i < filename.Length; i++)
            {
                filNamBytStr[i] = (byte)filename[i];
            }

            FileStream OutputFileStream;
            OutputFileStream = File.Open(PathEndFile, FileMode.Append);
            OutputFileStream.WriteByte((byte)filename.Length);//запись количества букв в названии макс 256
            OutputFileStream.Write(filNamBytStr, 0, filNamBytStr.Count());//запись названия файла

            for (int i = 0; i < 256; i++)//запись частоты повторов(cловаря)
            {
                int tem = Dict[i];
                byte[] x = new byte[4];
                x[0] = (byte)(tem & 0xff);
                x[1] = (byte)((tem >> 8) & 0xff);
                x[2] = (byte)((tem >> 16) & 0xff);
                x[3] = (byte)(tem >> 24);

                OutputFileStream.Write(x, 0, 4);
            }

            int MassSize = (countOfBits / 8) + 1;//запись размера конечного файла
            byte[] xx = new byte[4];
            xx[0] = (byte)(MassSize & 0xff);
            xx[1] = (byte)((MassSize >> 8) & 0xff);
            xx[2] = (byte)((MassSize >> 16) & 0xff);
            xx[3] = (byte)(MassSize >> 24);

            OutputFileStream.Write(xx, 0, 4);
           
            //byte[] OutFileMass = new byte[(countOfBits / 8) + 1];//создание массива байт для сохранения
            int counterForOutFileMass = 0;



            int counterforBit = 0;
            int[] MassBit = new int[8];


            for (int i = 0; i < inputFile.Length; i++)
            {
                byte tempByte = (byte)inputFile.ReadByte();

                List<bool> tem = table[tempByte];
                foreach (bool item in tem)
                {
                    if (counterforBit == 8)
                    {
                        //OutputFileStream.WriteByte(Convert.ToByte(tempStr, 2));
                        //counterForOutFileMass++;
                        //OutFileMass[counterForOutFileMass++] = Convert.ToByte(tempStr, 2);
                        //tempStr = "";
                        int resbyte = 0;
                        for (int j = 0; j < 8; j++)
                        {
                            resbyte |= MassBit[j] << 7 - j;
                        }
                        OutputFileStream.WriteByte((byte)resbyte);
                        counterForOutFileMass++;
                        counterforBit = 0;
                    }

                    if (item)
                    {
                        MassBit[counterforBit++] = 1;
                        //tempStr = tempStr + "1";
                    }
                    else
                    {
                        MassBit[counterforBit++] = 0;
                        //tempStr = tempStr + "0";
                    }
                }

            }



            //реализация с массивом
            /*
            foreach (byte b in WorkingString)
            {
                List<bool> tem = table[b];
                foreach (bool item in tem)
                {
                    if (counterforBit==8)
                    {
                        int resbyte = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            resbyte |= MassBit[i] <<7- i;
                        }
                        OutputFileStream.WriteByte((byte)resbyte);
                        counterForOutFileMass++;
                        counterforBit = 0;
                    }
                    if (item)
                    {
                        MassBit[counterforBit++] = 1;
                    }
                    else
                    {
                        MassBit[counterforBit++] = 0;
                    }
                }
            }
            */
            if (counterforBit>0)
            {
                for (; counterforBit<8;)
                {
                    MassBit[counterforBit++] = 0;
                }

                int resbyte = 0;
                for (int i = 0; i < 8; i++)
                {
                    resbyte |= MassBit[i] <<7- i;
                }
                OutputFileStream.WriteByte((byte)resbyte);
                counterForOutFileMass++;
                counterforBit = 0;
            }
            
            if (counterForOutFileMass < MassSize)
            {
                OutputFileStream.WriteByte(0);
                counterForOutFileMass++;
            }
            
            OutputFileStream.Close();
            inputFile.Close();

        }

        public void CodingFiles(string PathSourseFile, string PathEndFile, string endFileNameAndFolder)
        {
            byte[] WorkingString = File.ReadAllBytes(PathSourseFile);

            //FileStream inputFile = File.Open(PathSourseFile, FileMode.Open);

            for (int i = 0; i < 256; i++)//обнуление частотного словаря
            {
                Dict[i] = 0;
            }/*
            for (int i = 0; i < inputFile.Length; i++)
            {
                int byt = inputFile.ReadByte();
                Dict[byt] = Dict[byt] + 1;
            }*/
             //inputFile.Seek(0, SeekOrigin.Begin);

            foreach (byte item in WorkingString)//создание частотного словаря
            {
                Dict[item] = Dict[(int)item] + 1;
            }

            List<Node> t = new List<Node>();

            for (int i = 0; i < 256; i++)//создание листа нодов из словаря
            {
                t.Add(new Node { C = (byte)i, N = Dict[i] });
            }

            while (t.Count != 1)//отбор с самых низких значений, и создание дерева нодов с одним оставшимся рутом
            {
                Node a1 = t[0];

                foreach (Node item in t)
                {
                    if (a1.N > item.N)
                    {
                        a1 = item;
                    }
                }
                t.Remove(a1);

                Node a2 = t[0];

                foreach (Node item in t)
                {
                    if (a2.N > item.N)
                    {
                        a2 = item;
                    }
                }
                t.Remove(a2);

                Node p = new Node() { left = a1, right = a2, N = a1.N + a2.N };
                t.Add(p);
            }
            Node root = t[0];
            table.Clear();//очистка словаря
            BuildTable(root);//рекурсивная функция записи кодов в словарь

            int countOfBits = 0;

            for (int i = 0; i < 256; i++)
            {
                countOfBits += table[(byte)i].Count * Dict[(byte)i];//подсчёт размера массива байтоБит
            }


            ////////////////////////////////////////////////////////////////////////////////
            //сохранение файла
            string filename = endFileNameAndFolder;
            byte[] filNamBytStr = new byte[filename.Length];
            for (int i = 0; i < filename.Length; i++)
            {
                filNamBytStr[i] = (byte)filename[i];
            }

            FileStream OutputFileStream;
            OutputFileStream = File.Open(PathEndFile, FileMode.Append);
            OutputFileStream.WriteByte((byte)filename.Length);//запись количества букв в названии макс 256
            OutputFileStream.Write(filNamBytStr, 0, filNamBytStr.Count());//запись названия файла

            for (int i = 0; i < 256; i++)//запись частоты повторов
            {
                int tem = Dict[i];
                byte[] x = new byte[4];
                x[0] = (byte)(tem & 0xff);
                x[1] = (byte)((tem >> 8) & 0xff);
                x[2] = (byte)((tem >> 16) & 0xff);
                x[3] = (byte)(tem >> 24);

                OutputFileStream.Write(x, 0, 4);
            }

            int MassSize = (countOfBits / 8) + 1;//запись размера конечного файла
            //int MassSize = OutFileMass.Count();
            byte[] xx = new byte[4];
            xx[0] = (byte)(MassSize & 0xff);
            xx[1] = (byte)((MassSize >> 8) & 0xff);
            xx[2] = (byte)((MassSize >> 16) & 0xff);
            xx[3] = (byte)(MassSize >> 24);

            OutputFileStream.Write(xx, 0, 4);
            /////////////////////////////

            //string tempStr = "";
            //byte[] OutFileMass = new byte[(countOfBits / 8) + 1];//создание массива байт для сохранения
            int counterForOutFileMass = 0;

            /*
                        for (int i = 0; i < inputFile.Length; i++)
                        {
                            byte tempByte = (byte)inputFile.ReadByte();

                            List<bool> tem = table[tempByte];
                            foreach (bool item in tem)
                            {
                                if (tempStr.Length == 8)
                                {
                                    OutputFileStream.WriteByte(Convert.ToByte(tempStr, 2));
                                    counterForOutFileMass++;
                                    //OutFileMass[counterForOutFileMass++] = Convert.ToByte(tempStr, 2);
                                    tempStr = "";
                                }

                                if (item)
                                {
                                    tempStr = tempStr + "1";
                                }
                                else
                                {
                                    tempStr = tempStr + "0";
                                }
                            }

                        }*/
            int counterforBit = 0;
            int[] MassBit = new int[8];

            int probe = 0;
            int counterForProbe = 0;

            foreach (byte b in WorkingString)
            {
                List<bool> tem = table[b];
                foreach (bool item in tem)
                {
                    if (counterforBit/*counterForProbe*/ == 8)
                    {
                        int resbyte = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            resbyte |= MassBit[i] << 7 - i;
                        }
                        OutputFileStream.WriteByte((byte)resbyte);
                        counterForOutFileMass++;
                        counterforBit = 0;
                        /*
                        OutputFileStream.WriteByte((byte)probe);
                        probe = 0;
                        counterForProbe = 0;*/
                    }
                    if (item)
                    {
                        //probe |= 1 << 7 - counterForProbe++;
                        MassBit[counterforBit++] = 1;
                    }
                    else
                    {
                        //probe |= 0 << 7 - counterForProbe++;
                        MassBit[counterforBit++] = 0;
                    }
                }
            }

            if (counterforBit/*counterForProbe*/ > 0)
            {
                for (; counterforBit/*counterForProbe*/ < 8;)
                {
                    MassBit[counterforBit++] = 0;
                    //probe |= 0 << 7 - counterForProbe++;
                }
                
                int resbyte = 0;
                for (int i = 0; i < 8; i++)
                {
                    resbyte |= MassBit[i] << 7 - i;
                }
                OutputFileStream.WriteByte((byte)resbyte);
                counterForOutFileMass++;
                counterforBit = 0;

                //OutputFileStream.WriteByte((byte)probe);

            }

            if (counterForOutFileMass/*counterForProbe */< MassSize)
            {
                OutputFileStream.WriteByte(0);
                counterForOutFileMass++;
            }

            OutputFileStream.Close();

        }

        public void CodingFolder(string foldersPath, string EndFilePathName, string BaseFolder)
        {
            DirectoryInfo dinfo = new DirectoryInfo(foldersPath);
            foreach (FileInfo item in dinfo.GetFiles())
            {
                CodingFiles(item.FullName, EndFilePathName, item.FullName.Substring(BaseFolder.Length));
            }
            foreach (DirectoryInfo item in dinfo.GetDirectories())
            {
                CodingFolder(item.FullName, EndFilePathName,BaseFolder);
            }
        }
    }
}
