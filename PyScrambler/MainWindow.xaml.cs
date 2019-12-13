using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Net;
using System.Diagnostics;
using System.ComponentModel;

namespace PyScrambler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Result.Text = "Version 0.1 (C# WPF) Build 0002";  // CURRENT VERSION
        }

        public List<string> Paragraph = new List<string>();

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".txt";
            dlg.Filter = "본문 파일 (*.txt)|*.txt";

            var result = dlg.ShowDialog();

            if (result == true)
            {
                Paragraph.AddRange(File.ReadAllLines(dlg.FileName));
                
                Learn learn = new Learn();
                Content = learn;
            }

        }

        delegate void Work();
        delegate void UpdateUI();

        string ver;
        int update;
        string upd;

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            // obbcth.github.io/pyscrambler.html 파싱

            Update.Content = "확인중...";
            upd = Result.Text;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += Worker;
            bw.RunWorkerCompleted += TaskWorkComplete;
            bw.RunWorkerAsync();
        }

        protected void TaskWorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Display();
        }

        public void Worker(object sender, DoWorkEventArgs e)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://obbcth.github.io/pyscrambler.html");

            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                int i = 0;

                ver = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("UTF-8")).ReadLine();

                if (upd != ver)
                {

                    update = 1;

                }
                else
                {
                    update = 0;
                }

            }
            catch
            {
                update = 2;
            }
        }

        private void Display()
        {
            if (update == 0)
            {
                MessageBox.Show("최신버전입니다!   // " + ver, "PyScrambler", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            else if (update == 1)
            {
                MessageBox.Show("업데이트가 있습니다.   // " + ver + "\n\nhttps://github.com/obbcth/PyScrambler 에서 다운받으세요.",
                                            "PyScrambler",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Information);
                Process.Start(new ProcessStartInfo("https://github.com/obbcth/PyScrambler"));
            }

            else
            {
                MessageBox.Show("사이트에 접속할 수 없습니다.\n\n인터넷 연결을 확인하세요.", "PyScrambler", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Update.Content = "업데이트 확인";
        }

        private void Help_Click(object sender, RoutedEventArgs e)
            {
                MessageBox.Show("PyScrambler 사용방법:\n\n" +
                    "1. 본문 파일 (*.txt) 을 선택합니다.\n" +
                    "2. PyScrambler가 알아서 문제를 만들 것입니다. \n" +
                    "3. 문장을 입력하여 학습을 진행합니다.\n\n" +
                    "PyScrambler 부가기능:\n\n" +
                    "p : 문장을 패스합니다.\n" +
                    "r : 단어 배열을 리셋합니다.\n" +
                    "b : 이전의 문장으로 돌아갑니다.\n" +
                    "h : 힌트 (정답)을 보여줍니다.\n\n" +
                    "본문 파일 제작방법 및 PyScrambler에 대한 자세한 사항, 업데이트 등은 아래 링크에서 확인하기 바랍니다.\n" +
                    "https://github.com/obbcth/PyScrambler", "PyScrambler", MessageBoxButton.OK, MessageBoxImage.Information);
            }
    }
}
