using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Taski
{
    public partial class Form1 : Form
    {
        #region konstruktory
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Buttony
        private async void button1_Click(object sender, EventArgs e) //Przycisk Liczący bez taskDzieci
        {
            richTextBox1.Clear();
            int dolnaGranica = int.Parse(textBox1.Text);
            int gornaGranica = int.Parse(textBox2.Text);
            int ilosc = int.Parse(textBox4.Text);
            if (dolnaGranica <= 0)
            {
                dolnaGranica = 1;
            }
            if (dolnaGranica > gornaGranica)
            {
                return;
            }


            await BezDzieci(dolnaGranica, gornaGranica, ilosc);

            
        }

        private async void button2_Click(object sender, EventArgs e) //Przycisk liczący z dzieckiem task
        {
            richTextBox1.Clear();
            int dolnaGranica = int.Parse(textBox1.Text);
            int gornaGranica = int.Parse(textBox2.Text);
            int ilosc = int.Parse(textBox4.Text);
            if (dolnaGranica <= 0)
            {
                dolnaGranica = 1;
            }
            if (dolnaGranica > gornaGranica)
            {
                return;
            }


            await Dzieci(dolnaGranica, gornaGranica, ilosc);
        }

        #endregion

        #region Taski
        private async Task Dzieci(int dolnaGranica, int gornaGranica, int ilosc)
        {
            Task<int>[] taskArray = new Task<int>[ilosc];
            int[] results = new int[ilosc];
            int skok = (gornaGranica - dolnaGranica) / ilosc;
            int dolna = dolnaGranica, gorna = dolnaGranica + skok;
            for (int i = 0; i < taskArray.Length - 1; i++)
            {
                int lewo = dolna;
                int prawo = gorna;
                taskArray[i] = new Task<int>(() =>
                {
                    int lewaGranica = lewo;
                    int prawaGranica = prawo;
                    int result = 0;


                    for (int j = lewaGranica; j <= prawaGranica; j++)
                    {
                        bool czyPierwsza = false;
                        Task childTask = Task.Factory.StartNew(() =>
                        {
                            czyPierwsza = true;
                            for (int k = 2; k < j; k++)
                            {
                                if (j % k == 0)
                                {
                                    czyPierwsza = false;
                                }
                            }
                        }, TaskCreationOptions.AttachedToParent);
                        childTask.Wait();
                        if (czyPierwsza)
                        {
                            result++;
                        }
                    }

                    return result;
                });


                dolna = gorna + 1;
                gorna = dolna + skok;
            }


            int lewo2 = dolna;
            int prawo2 = gornaGranica;
            taskArray[ilosc - 1] = new Task<int>(() =>
            {
                int lewaGranica = lewo2;
                int prawaGranica = prawo2;
                int result = 0;


                for (int j = lewaGranica; j <= prawaGranica; j++)
                {
                    bool czyPierwsza = false;
                    Task childTask = Task.Factory.StartNew(() =>
                    {
                        czyPierwsza = true;
                        for (int k = 2; k < j; k++)
                        {
                            if (j % k == 0)
                            {
                                czyPierwsza = false;
                            }
                        }
                    }, TaskCreationOptions.AttachedToParent);
                    childTask.Wait();
                    if (czyPierwsza)
                    {
                        result++;
                    }
                }

                return result;
            });


            int sum = 0;

            for (int i = 0; i < taskArray.Length; i++)
            {
                richTextBox1.Text += "Wywołano .Start dla Task pod indeskem: " + i + "\n";
                taskArray[i].Start();
            }

            for (int i = 0; i < taskArray.Length; i++)
            {
                sum += await taskArray[i];
                richTextBox1.Text += $"Task z indeksu {i} zwrocil wynik\n";
            }

            textBox3.Text = sum.ToString();
        }

        private async Task BezDzieci(int dolnaGranica, int gornaGranica, int ilosc)
        {
            Task<int>[] taskArray = new Task<int>[ilosc];
            int[] results = new int[ilosc];
            int skok = (gornaGranica - dolnaGranica) / ilosc;
            int dolna = dolnaGranica, gorna = dolnaGranica + skok;
            for (int i = 0; i < taskArray.Length - 1; i++)
            {
                int lewo = dolna;
                int prawo = gorna;
                taskArray[i] = new Task<int>(() => zliczPierwsze(lewo, prawo));
                dolna = gorna + 1;
                gorna = dolna + skok;
            }
            int lewo2 = dolna;
            int prawo2 = gornaGranica;
            taskArray[ilosc - 1] = new Task<int>(() => zliczPierwsze(lewo2, prawo2));
            int sum = 0;

            for (int i = 0; i < taskArray.Length; i++)
            {
                richTextBox1.Text += "Wywołano .Start dla Task pod indeskem: " + i + "\n";
                taskArray[i].Start();
            }

            for (int i = 0; i < taskArray.Length; i++)
            {
                sum += await taskArray[i];
                richTextBox1.Text += $"Task z indeksu {i} zwrocil wynik\n";
            }

            textBox3.Text = sum.ToString();
        }

        #endregion

        #region MetodyPomocnicze
        private int zliczPierwsze(int dolnaGranica, int gornaGranica)
        {
            {
                int iloscPierwszych = 0;
                for (; dolnaGranica <= gornaGranica; dolnaGranica++)
                {
                    if (CzyPierwsza(dolnaGranica))
                    {
                        iloscPierwszych++;
                    }
                }
                return iloscPierwszych;
            }
        }

        private bool CzyPierwsza(int liczba)
        {
            for (int i = 2; i < liczba; i++)
            {
                if (liczba % i == 0)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
