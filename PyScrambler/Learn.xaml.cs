using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PyScrambler
{
    /// <summary>
    /// Interaction logic for Learn.xaml
    /// </summary>
    public partial class Learn : Page
    {
        int now = 1;
        int now_str = 0;
        int flag = 0;
        int correct = 0;

        FontFamily fFam = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#SeoulNamsan");
        
        TextBlock Question = new TextBlock();
        TextBlock Words = new TextBlock();
        TextBox Answer = new TextBox();
        TextBlock Info = new TextBlock();
        
        List<string> real_lines = new List<string>();
        List<string> questions = new List<string>();

        string[] splited;
        string[] shuffled;
        string wordlist;
        string wordlist2;
        string wordlist3;

        public Learn()
        {
            InitializeComponent();

            Question.FontFamily = fFam;
            Words.FontFamily = fFam;
            Answer.FontFamily = fFam;
            Info.FontFamily = fFam;

            Learning();
        }

        public void Learning()
        {
            var MainWin = (MainWindow)Application.Current.MainWindow;
            List<string> Para = MainWin.Paragraph;

            // Read Article 이식과정

            if (flag == 0)
            {
                foreach (string line in Para)
                {
                    int StartIndex = line.IndexOf("[");
                    int EndIndex = line.IndexOf("]");

                    string answer = "";
                    string question = "";

                    if (StartIndex == -1 || EndIndex == -1)
                    {
                        answer = line;
                        question = string.Concat(Enumerable.Repeat("_", (int)(answer.Length / 2)));
                    }
                    else
                    {
                        answer = line.Substring(StartIndex + 1, EndIndex - StartIndex - 1);
                        question = line.Substring(0, StartIndex) + string.Concat(Enumerable.Repeat("_", (int)(answer.Length / 2))) + line.Substring(EndIndex + 1);
                    }

                    question = question.Replace("\n", "");
                    question = question.Trim();

                    var temp = Regex.Replace(question, "\\W+", "").ToLower().Trim();

                    if (temp != "")
                    {
                        if (temp != "_")
                        {
                            real_lines.Add(answer);
                            questions.Add(question);
                        }
                    }
                }

                flag = 1;
            }

            // Main Command Start (Show Questions, Command 이식)

            Remain.Text = "남은 문장 개수 : " + (questions.Count() - now_str).ToString() + "개";
            wordlist = "";

            if (splited != null)
            {
                Array.Clear(splited, 0, splited.Count());
                Array.Clear(shuffled, 0, shuffled.Count());
            }

            if (now_str == real_lines.Count())
            {
                MessageBox.Show("모든 작업이 끝났습니다!\n프로그램을 종료합니다.",
                                          "PyScrambler",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Information);

                Environment.Exit(0);
            }

            if (now_str == -1)
            {
                now_str++;
            }

            splited = real_lines[now_str].Split();
            Random rnd = new Random();
            shuffled = splited.OrderBy(x => rnd.Next()).ToArray();

            foreach (string temp in shuffled)
            {
                wordlist = wordlist + temp + " / ";
            }

            Question.Text = questions[now_str];
            Words.Text = "단어 : " + wordlist.Substring(0, wordlist.Length-3);

            // Several Boxes (Question, Words, Answer)

            Question.FontSize = 17;
            Question.TextWrapping = TextWrapping.Wrap;
            Question.Margin = new Thickness(20, 10, 20, 10);

            Words.FontSize = 17;
            Words.TextWrapping = TextWrapping.Wrap;
            Words.Margin = new Thickness(20, 0, 20, 10);

            Answer.Text = "";
            Answer.FontSize = 17;
            Answer.TextWrapping = TextWrapping.Wrap;
            Answer.Margin = new Thickness(20, 0, 20, 10);
            Answer.Padding = new Thickness(5);
            Answer.KeyDown += new KeyEventHandler(Answer_Change);

            Info.FontSize = 13;
            Info.TextWrapping = TextWrapping.Wrap;
            Info.Margin = new Thickness(20, 10, 20, 10);

            Sub.Children.Add(Question);
            Sub.Children.Add(Words);
            Sub.Children.Add(Answer);

            Answer.Focus();
        }

        private void Answer_Change(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                // Check Process Start

                string n = Answer.Text;

                string now_str2 = real_lines[now_str].Trim();
                string[] n2 = n.Split();

                string a = Regex.Replace(now_str2, "\\W+", "").ToLower().Trim();
                string b = Regex.Replace(n, "\\W+", "").ToLower().Trim();

                if (a == b)
                {
                    correct = 1;

                    MessageBox.Show("정답입니다!",
                                              "PyScrambler",
                                              MessageBoxButton.OK,
                                              MessageBoxImage.Information);
                }

                else if (n == "")
                {
                    // 그냥 무응답
                }

                else if (n == "p")
                {
                    correct = 1;
                    Answer.Text = "";
                }

                else if (n == "r")
                {
                    correct = 1;
                    now_str--;
                    Answer.Text = "";
                }

                else if (n == "b")
                {
                    correct = 1;
                    now_str--; now_str--;
                    Answer.Text = "";
                }

                else if (n == "h")
                {
                    Info.Text = "힌트 : " + now_str2;
                    Answer.Text = "";
                }

                else
                {
                    // 오타 체크 및 여러가지 과정 거치기

                    List<string> def_word = new List<string>();
                    List<string> inp_word = new List<string>();

                    foreach (string temp in shuffled)
                    {
                        def_word.Add(Regex.Replace(temp, "\\W+", "").ToLower().Trim());
                    }

                    foreach (string temp in n2)
                    {
                        inp_word.Add(Regex.Replace(temp, "\\W+", "").ToLower().Trim());
                    }

                    List<string> def_word2 = new List<string>(def_word);
                    List<string> inp_word2 = new List<string>(inp_word);

                    foreach (string temp in def_word)
                    {
                        try
                        {
                            inp_word.Remove(temp);
                            inp_word.Remove("");
                        }
                        catch { }
                    }

                    foreach (string temp in inp_word2)
                    {
                        try
                        {
                            def_word2.Remove(temp);
                        }
                        catch { }
                    }

                    foreach (string temp in def_word2)
                    {
                        wordlist2 = wordlist2 + temp + " / ";
                    }

                    foreach (string temp in inp_word)
                    {
                        wordlist3 = wordlist3 + temp + " / ";
                    }

                    if (wordlist2.Length == 0 && wordlist3.Length == 0)
                    {
                        Info.Text = "누락된 단어, 감지된 오타 없음. 단어배열 순서를 확인하세요.";
                    }
                    else if (wordlist2.Length == 0 && wordlist3.Length != 0)
                    {
                        Info.Text = "누락된 단어 : 없음\n감지된 오타 : " + wordlist3.Substring(0, wordlist3.Length - 3);
                        wordlist3 = "";
                    }
                    else if (wordlist3 == null || (wordlist2.Length != 0 && wordlist3.Length == 0))
                    {
                        Info.Text = "누락된 단어 : " + wordlist2.Substring(0, wordlist2.Length - 3) + "\n감지된 오타 : 없음";
                        wordlist2 = "";
                        wordlist3 = "";
                    }
                    else
                    {
                        Info.Text = "누락된 단어 : " + wordlist2.Substring(0, wordlist2.Length - 3) + "\n감지된 오타 : " + wordlist3.Substring(0, wordlist3.Length - 3);
                        wordlist2 = "";
                        wordlist3 = "";
                    }


                }
                               
                if (correct == 1)
                {                                        
                    now_str++;
                    Sub.Children.Clear();
                    Info.Text = "";
                    correct = 0;
                    Learning();
                }

                else
                {
                    // 오답일 시

                    if (now == 1)
                    {
                        Sub.Children.Add(Info);
                        now = 0;
                    }

                    else
                    {
                        Sub.Children.Clear();

                        Sub.Children.Add(Question);
                        Sub.Children.Add(Words);
                        Sub.Children.Add(Answer);
                        Sub.Children.Add(Info);

                        Answer.Focus();
                    }
                }
            }
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
