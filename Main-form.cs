using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace OC5
{ 
    public partial class Form1 : Form
    {
        private Thread solutionthread;

        public Form1()
        {
            InitializeComponent();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.Columns.Add("Column1", "");
            dataGridView1.Rows.Add();
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.Columns.Add("Column1", "");
            dataGridView2.Rows.Add();
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ToolStripMenuItem fileItem = new ToolStripMenuItem("Меню");
            fileItem.DropDownItems.Add(new ToolStripMenuItem("Сохранить"));
            //menuStrip1.Items.Add(fileItem);

        }
        private void ProgressBarConfig(int b, ProgressBar progressBar)
        {
            // Display the ProgressBar control.
            progressBar.Visible = true;
            // Set Minimum to 1 to represent the first file being copied.
            progressBar.Minimum = 1;
            // Set Maximum to the total number of files to copy.
            progressBar.Maximum = b + 1;
            progressBar.Value = 1;
            progressBar.Step = 1;
        }
        //запуск мини-игры
        private void ButtonStartMinigame_Click(object sender, EventArgs e)
        {
            Mini_game mg = new Mini_game(this);
            ButtonStartMinigame.Enabled = false;
            mg.Show();
        }
        //добавление в ячеёку символов
        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

            if (e.Control != null)
            {
                e.Control.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                Console.WriteLine(e.Control.Text);
            }
        }
        //Обработка нажатий для минуса точки и цифр
        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != '.')
                e.Handled = true;
            if (e.KeyChar == '.')
            {
                if (((DataGridViewTextBoxEditingControl)sender).Text.Length == 0)
                    e.Handled = true;
                if (((DataGridViewTextBoxEditingControl)sender).Text.Contains('.'))
                    e.Handled = true;
            }
            if (e.KeyChar == '-')
            {
                if (((DataGridViewTextBoxEditingControl)sender).Text.Length != 0)
                    e.Handled = true;
                if (((DataGridViewTextBoxEditingControl)sender).Text.Contains('-'))
                    e.Handled = true;
            }
        }
        //изменение количества строчек и столбоцов в матрице
        private void SizeChanged_numeric(object sender, EventArgs e)
        {
            if (((NumericUpDown)sender).Tag.ToString() == "matrix_a")
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                for (int j = 0; j < size_matrix_a_y.Value; j++)
                {
                    dataGridView1.Columns.Add("Column" + j, "");
                    dataGridView1.Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                for (int i = 0; i < size_matrix_a_x.Value; i++)
                {
                    dataGridView1.Rows.Add();
                }
            }
            else
            {
                dataGridView2.Rows.Clear();
                dataGridView2.Columns.Clear();
                for (int j = 0; j < size_matrix_b_y.Value; j++)
                {
                    dataGridView2.Columns.Add("Column" + j, "");
                    dataGridView2.Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                for (int i = 0; i < size_matrix_b_x.Value; i++)
                {
                    dataGridView2.Rows.Add();
                }
            }
        }

        private void start_execute(object sender, EventArgs e)
        { 
            buttonstart.Enabled = false;
            buttonstop.Enabled = true;
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            dataGridView1.ReadOnly = false;
            dataGridView2.ReadOnly = false;
            buttonloadmatrixb.Enabled = false;
            buttonloadmatrixa.Enabled = false;
            solutionthread = new Thread(new ParameterizedThreadStart(Start_Solution));
            solutionthread.Start(radioButton1.Checked);
        }
        private void Start_Solution(object flag)
        {
            if (dataGridView3.InvokeRequired)
                this.Invoke(new MethodInvoker(() => dataGridView3.Rows.Clear()));
            else
                dataGridView3.Rows.Clear();
            if (dataGridView3.InvokeRequired)
                this.Invoke(new MethodInvoker(() => dataGridView3.Columns.Clear()));
            else
                dataGridView3.Columns.Clear();
            Fill_Data(dataGridView1);
            Fill_Data(dataGridView2);
            List<List<string>> a = GetListFromDataGrid(dataGridView1);
            List<List<string>> b = GetListFromDataGrid(dataGridView2);
            int a_m = a.Count, a_n = a[0].Count;
            int b_m = b.Count, b_n = b[0].Count;
            int x_cfg;
            
            if ((bool)flag)
            {
                if (a_m != b_m)
                {
                    MessageBox.Show("Ошибка. Количество строк матриц A и B должны быть одинаковыми.");
                    StopTred();
                    return;
                }
                else x_cfg = a_m * b_n;
            }
            else
            {
                if (a_n != b_n)
                {
                    MessageBox.Show("Ошибка. Количество столбцов матриц A и B должны быть одинаковыми.");
                    StopTred();
                    return;
                }
                else x_cfg = a_n * b_m;

            }
            int a_cfg = a_n * a_m;
            int b_cfg = b_n * b_m;
            if (progressBar1.InvokeRequired)
                this.Invoke(new MethodInvoker(() => ProgressBarConfig((a_cfg + b_cfg + x_cfg), progressBar1)));
            else
                ProgressBarConfig((a_cfg + b_cfg + x_cfg), progressBar1);
            List<List<string>> x = new List<List<string>>();
            Solution solution = new Solution(a, b);
            solution.Setter_x(flag, this);
            x = solution.Getter_x();
            if(x == null)
            {
                MessageBox.Show("Дискриминант матрицы А равен нулю или матрица А не квадратная, решения нет!");
                StopTred();
                return;
            }
            Draw_Matrix_Solution(x);
            StopTred();
        }
        



        private void Draw_Matrix_Solution(List<List<string>> x)
        {

            for (int j = 0; j < x[0].Count; j++)
            {
                if (dataGridView3.InvokeRequired)  
                    this.Invoke(new MethodInvoker(() => dataGridView3.Columns.Add("Column" + j, "")));
                else  
                    dataGridView3.Columns.Add("Column" + j, "");

                if (dataGridView3.InvokeRequired)  
                    this.Invoke(new MethodInvoker(() => dataGridView3.Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells));
                else  
                    dataGridView3.Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


                dataGridView3.Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            for (int i = 0; i < x.Count-1; i++)
            {
                if (dataGridView3.InvokeRequired)  
                    this.Invoke(new MethodInvoker(() => dataGridView3.Rows.Add()));
                else  
                    dataGridView3.Rows.Add();
            }
            for (int i = 0; i < x.Count; i++)
            {
                for (int j = 0; j < x[0].Count; j++)
                {
                    if (dataGridView3.InvokeRequired)  
                        this.Invoke(new MethodInvoker(() => dataGridView3.Rows[i].Cells[j].Value = x[i][j].ToString()));
                    else  
                        dataGridView3.Rows[j].Cells[i].Value = x[i][j].ToString();
                }
            }
            if (buttonstart.InvokeRequired)  
                this.Invoke(new MethodInvoker(() => buttonstart.Enabled = true));
            else  
                buttonstart.Enabled = true;

            if (buttonstop.InvokeRequired)  
                this.Invoke(new MethodInvoker(() => buttonstop.Enabled = false));
            else  
                buttonstop.Enabled = false;
        }

        private List<List<string>> GetListFromDataGrid(DataGridView dataGridView)
        {
            List<List<string>> list = new List<List<string>>();
            for(int i = 0; i < dataGridView.Rows.Count; i++)
            {
                List<string> temp = new List<string>();
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    temp.Add(dataGridView.Rows[i].Cells[j].Value.ToString());
                }
                list.Add(temp);
            }
            return list;
        }


        private void Fill_Data(DataGridView dataGridView)
        {
            Random rand = new Random();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {

                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if (dataGridView.Rows[i].Cells[j].Value == null)
                        dataGridView.Rows[i].Cells[j].Value = rand.Next(-9, 9).ToString();
                    if (dataGridView.Rows[i].Cells[j].Value.ToString() == "-" || dataGridView.Rows[i].Cells[j].Value.ToString().StartsWith(".") || dataGridView.Rows[i].Cells[j].Value.ToString().StartsWith("-."))
                        dataGridView.Rows[i].Cells[j].Value = null;
                    if (dataGridView.Rows[i].Cells[j].Value == null)
                        dataGridView.Rows[i].Cells[j].Value = rand.Next(-9, 9).ToString();
                }
            }
        }
        
        

        private void LoadToSaveMatrix(DataGridView dataGridView)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog{Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*"};

            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            int a_m = dataGridView.Rows.Count, a_n = dataGridView.Columns.Count;
            try
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.ASCII);
                
                for (int i = 0; i < a_m; i++)
                {
                    for (int j = 0; j < a_n; j++)
                    {
                        sw.Write(dataGridView.Rows[i].Cells[j].Value);
                        sw.Write(" ");
                    }
                    sw.Write("\n");
                }
                sw.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception: " + exc.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }

        private void SaveMatrix(object sender, EventArgs e)
        {
            if(((Button)sender) == this.buttonsavematrixa)
            {
                LoadToSaveMatrix(dataGridView1);
            }
            else if (((Button)sender) == this.buttonsavematrixb)
            {
                LoadToSaveMatrix(dataGridView2);
            }
            else
            {
                LoadToSaveMatrix(dataGridView3);
            }
        }
        private void LoadMatrix(object sender, EventArgs e)
        {
            if (((Button)sender) == this.buttonloadmatrixa)
            {
                LoadToLoadMatrix(dataGridView1, size_matrix_a_x, size_matrix_a_y);
            }
            else if (((Button)sender) == this.buttonloadmatrixb)
            {
                LoadToLoadMatrix(dataGridView2, size_matrix_b_x, size_matrix_b_y);
            }
        }
        private void LoadToLoadMatrix(DataGridView dataGridView, NumericUpDown x,NumericUpDown y)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*" };
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            StreamReader sw = new StreamReader(openFileDialog.FileName, Encoding.ASCII);
            string filename = openFileDialog.FileName;
            // читаем файл в строку
            string fileText = File.ReadAllText(filename);;
            List<string> mass = new List<string>();

            while (sw.EndOfStream != true)
            {
                string s= sw.ReadLine();
                mass.Add(s);
            }
            List<List<string>> res = new List<List<string>>();
            for(int i=0;i<mass.Count;i++)
            {
                List<string> temp = new List<string>();
                temp = mass[i].Split(' ').ToList();
                var size = temp.Count();
                List<string> final_temp = new List<string>(size);
                for (int j = 0; j < temp.Count; j++)
                {
                    if (IsNumeric(temp[j])) final_temp.Add(temp[j]);
                    else final_temp.Add(null);
                }
                res.Add(final_temp);
            }

            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();
            
        
            try
            {
                x.Value = res.Count;
                y.Value = res[0].Count;
                for (int i = 0; i < res.Count; i++)
                {
                    for (int j = 0; j < res[i].Count; j++)
                    {
                        dataGridView.Rows[i].Cells[j].Value = res[i][j];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Были введены не числа или они превысили размер матрицы!");
                return;
            }
            finally
            {
                sw.Close();
            }
        }

        private void StopThread(object sender, EventArgs e)
        {
            StopTred();
        }
        private void StopTred()
        {
                this.Invoke(new MethodInvoker(() => {
                    buttonstart.Enabled = true;
                    buttonstop.Enabled = false;
                    groupBox1.Enabled = true;
                    buttonloadmatrixb.Enabled = true;
                    buttonloadmatrixa.Enabled = true;
                    groupBox2.Enabled = true;
                    groupBox3.Enabled = true;
                    dataGridView1.ReadOnly = true;
                    dataGridView2.ReadOnly = true;
                    progressBar1.Value = 1;
                })) ;
            solutionthread.Abort();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(solutionthread != null)
            {
                solutionthread.Abort();
            }
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "helper.chm");
        }

        public static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
    }
}
