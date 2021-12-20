using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace OC5
{
    class Solution
    {
        public void ChangeProgressBarFromSolution(Form1 mainForm)
        {
            if (mainForm.progressBar1.InvokeRequired)
                mainForm.Invoke(new MethodInvoker(() => mainForm.progressBar1.Value = mainForm.progressBar1.Value + 1));
            else
                mainForm.progressBar1.Value = mainForm.progressBar1.Value + 1;
            Thread.Sleep(10);               

        }
        private List<List<string>> a = new List<List<string>>();
        private List<List<string>> b = new List<List<string>>();
        public Solution(List<List<string>> a, List<List<string>> b)
        {
            this.a = a;
            this.b = b; 
        }
        private List<List<string>> x = new List<List<string>>();
        public void Setter_x(object choice_2, Form1 mainForm)
        {
            x = Get_Solution_matrix(choice_2, mainForm);
        }
        public List<List<string>> Getter_x()
        {
            return this.x;
        }
        

        public List<List<string>> Get_Solution_matrix(object choice_2, Form1 mainForm)
        {
            
            bool choice = (bool)choice_2;
            int x_n, x_m;
            int a_m = a.Count, a_n = a[0].Count;
            int b_m = b.Count, b_n = b[0].Count;

            if (choice)
            {
                if (a_m != b_m)
                    return null;
                else
                {
                    x_m = a_n;
                    x_n = b_n;
                }
            }
            else
            {
                if (a_n != b_n)
                    return null;
                else
                {
                    x_n = a_m;
                    x_m = b_m;
                }

            }
            Matrix matrix_a = new Matrix(a_m, a_n);
            Matrix matrix_b = new Matrix(b_m, b_n);
            for (int i = 0; i < a_m; i++)
            {
                for (int j = 0; j < a_n; j++)
                {
                    matrix_a[i, j] = Convert.ToDouble(a[i][j]);
                    ChangeProgressBarFromSolution(mainForm);
                }
            }
            for (int i = 0; i < b_m; i++)
            {
                for (int j = 0; j < b_n; j++)
                {
                    matrix_b[i, j] = Convert.ToDouble(b[i][j]);
                    ChangeProgressBarFromSolution(mainForm);
                }
            }
            Matrix matrix_inverted_a = matrix_a.CreateInvertibleMatrix();
            if (matrix_inverted_a == null)
                return null;
            Matrix matrix_x;
            if (choice)
                matrix_x = matrix_inverted_a * matrix_b;
            else
                matrix_x = matrix_b * matrix_inverted_a;
            List<List<string>> x = new List<List<string>>();
            
            for (int i = 0; i < matrix_x.M; i++)
            {
                List<string> temp = new List<string>();
                for (int j = 0; j < matrix_x.N; j++)
                {
                    temp.Add(matrix_x[i, j].ToString());
                    ChangeProgressBarFromSolution(mainForm);
                }
                x.Add(temp);
            }
            return x;
            

        }
    }
}
