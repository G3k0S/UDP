﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;

namespace UdpFileClient
{
    public class Program
    {
        [Serializable]
        public class FileDetails
        {
            public string fileType = "";
            public long fileSize = 0;
        }

        private static FileDetails details = new FileDetails();

        private static readonly int localPort = 5002;
        private static UdpClient remoteClient = new UdpClient(localPort);
        private static IPEndPoint remotePoint = null;

        private static FileStream fileStream;
        private static byte[] data = new byte[0];

        static void Main(string[] args)
        {
            getFileInfo();
            getFileData();
            Console.Read();
        }

        private static void getFileData()
        {
            Random r = new Random();
            try
            {
                Console.WriteLine("----> Ожидаю файл");
                data = remoteClient.Receive(ref remotePoint);

                Console.WriteLine("----> Файл получен... Сохраняю его!");
                fileStream = new FileStream(r.Next(Int32.MinValue, Int32.MaxValue).ToString() + "." + details.fileType, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fileStream.Write(data, 0, data.Length);

                Console.WriteLine("----> Файл сохранен!");
                Console.WriteLine("----> Открываем файл!");

                Process.Start(fileStream.Name);
                //Thread.Sleep(2000);
                //Process[] proc = null;
                //Process.GetProcessesByName(fileStream.Name);
                //Thread.Sleep(1000);
                //proc[0].Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                fileStream.Close();
                remoteClient.Close();
            }
        }

        private static void getFileInfo()
        {
            try
            {
                Console.WriteLine("----> Ожидаю информацию о нашем файле");
                data = remoteClient.Receive(ref remotePoint);

                Console.WriteLine("----> Информация получена!");

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileDetails));
                MemoryStream memory = new MemoryStream();
                memory.Write(data, 0, data.Length);
                memory.Position = 0;

                details = (FileDetails)xmlSerializer.Deserialize(memory);
                Console.WriteLine("Файл описания получен! Информация:");
                Console.WriteLine(details.fileSize + "байт");
                Console.WriteLine("Тип " + details.fileType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}