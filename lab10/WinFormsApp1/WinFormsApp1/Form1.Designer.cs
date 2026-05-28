namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            button1 = new Button();
            label1 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label2 = new Label();
            textBox3 = new TextBox();
            label3 = new Label();
            label6 = new Label();
            textBox4 = new TextBox();
            label4 = new Label();
            textBox5 = new TextBox();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)chart1).BeginInit();
            SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            chart1.Legends.Add(legend1);
            chart1.Location = new Point(352, 12);
            chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            chart1.Series.Add(series1);
            chart1.Size = new Size(584, 714);
            chart1.TabIndex = 0;
            chart1.Text = "chart1";
            // 
            // button1
            // 
            button1.Location = new Point(89, 529);
            button1.Name = "button1";
            button1.Size = new Size(167, 112);
            button1.TabIndex = 1;
            button1.Text = "Старт";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(21, 35);
            label1.Name = "label1";
            label1.Size = new Size(325, 20);
            label1.TabIndex = 2;
            label1.Text = "Интенсивность поступления заявок (лямбда)";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(21, 67);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(325, 27);
            textBox1.TabIndex = 3;
            textBox1.Text = "15";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(21, 162);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(325, 27);
            textBox2.TabIndex = 5;
            textBox2.Text = "5";
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(21, 130);
            label2.Name = "label2";
            label2.Size = new Size(282, 20);
            label2.TabIndex = 4;
            label2.Text = "Интенсивность обработки заявок (мю)";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(21, 251);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(325, 27);
            textBox3.TabIndex = 7;
            textBox3.Text = "10000";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(21, 219);
            label3.Name = "label3";
            label3.Size = new Size(141, 20);
            label3.TabIndex = 6;
            label3.Text = "Количество заявок";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(976, 12);
            label6.Name = "label6";
            label6.Size = new Size(75, 20);
            label6.TabIndex = 8;
            label6.Text = "Результат";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(21, 346);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(325, 27);
            textBox4.TabIndex = 10;
            textBox4.Text = "10";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(21, 314);
            label4.Name = "label4";
            label4.Size = new Size(279, 20);
            label4.TabIndex = 9;
            label4.Text = "Количество приборов (обработчиков)";
            // 
            // textBox5
            // 
            textBox5.Location = new Point(21, 439);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(325, 27);
            textBox5.TabIndex = 12;
            textBox5.Text = "5";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(21, 407);
            label5.Name = "label5";
            label5.Size = new Size(118, 20);
            label5.TabIndex = 11;
            label5.Text = "Мест в очереди";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1450, 738);
            Controls.Add(textBox5);
            Controls.Add(label5);
            Controls.Add(textBox4);
            Controls.Add(label4);
            Controls.Add(label6);
            Controls.Add(textBox3);
            Controls.Add(label3);
            Controls.Add(textBox2);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(chart1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)chart1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private Button button1;
        private Label label1;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label2;
        private TextBox textBox3;
        private Label label3;
        private Label label6;
        private TextBox textBox4;
        private Label label4;
        private TextBox textBox5;
        private Label label5;
    }
}
