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
            components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            groupBox1 = new GroupBox();
            war = new Label();
            button2 = new Button();
            clear = new Button();
            label4 = new Label();
            textBoxSt = new TextBox();
            label5 = new Label();
            textBoxPl = new TextBox();
            label6 = new Label();
            textBoxM = new TextBox();
            label3 = new Label();
            textBoxA = new TextBox();
            label2 = new Label();
            textBoxV = new TextBox();
            label1 = new Label();
            textBoxH = new TextBox();
            button1 = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            results = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)chart1).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)results).BeginInit();
            SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea1);
            legend1.MaximumAutoSize = 100F;
            legend1.Name = "Legend1";
            chart1.Legends.Add(legend1);
            chart1.Location = new Point(5, 171);
            chart1.Name = "chart1";
            series1.BorderWidth = 3;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = Color.FromArgb(255, 128, 0);
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            chart1.Series.Add(series1);
            chart1.Size = new Size(1074, 373);
            chart1.TabIndex = 0;
            chart1.Text = "chart1";
            chart1.Click += chart1_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(war);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(clear);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(textBoxSt);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(textBoxPl);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(textBoxM);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(textBoxA);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(textBoxV);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(textBoxH);
            groupBox1.Controls.Add(button1);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1088, 163);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Enter += groupBox1_Enter;
            // 
            // war
            // 
            war.AutoSize = true;
            war.Location = new Point(123, 137);
            war.Name = "war";
            war.Size = new Size(0, 20);
            war.TabIndex = 15;
            // 
            // button2
            // 
            button2.Location = new Point(810, 12);
            button2.Name = "button2";
            button2.Size = new Size(100, 151);
            button2.TabIndex = 14;
            button2.Text = "ЗАПУСК! (сразу 5 вариантов)";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click_1;
            // 
            // clear
            // 
            clear.Location = new Point(939, 10);
            clear.Name = "clear";
            clear.Size = new Size(149, 153);
            clear.TabIndex = 13;
            clear.Text = "Очистить график и таблицу";
            clear.UseVisualStyleBackColor = true;
            clear.Click += button2_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(304, 109);
            label4.Name = "label4";
            label4.Size = new Size(58, 20);
            label4.TabIndex = 12;
            label4.Text = "Шаг (с)";
            // 
            // textBoxSt
            // 
            textBoxSt.Location = new Point(364, 105);
            textBoxSt.Name = "textBoxSt";
            textBoxSt.Size = new Size(111, 27);
            textBoxSt.TabIndex = 11;
            textBoxSt.Text = "0,01";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(246, 71);
            label5.Name = "label5";
            label5.Size = new Size(116, 20);
            label5.TabIndex = 10;
            label5.Text = "Площадь (м^2)";
            // 
            // textBoxPl
            // 
            textBoxPl.Location = new Point(364, 67);
            textBoxPl.Name = "textBoxPl";
            textBoxPl.Size = new Size(111, 27);
            textBoxPl.TabIndex = 9;
            textBoxPl.Text = "0,07";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(283, 36);
            label6.Name = "label6";
            label6.Size = new Size(79, 20);
            label6.TabIndex = 8;
            label6.Text = "Масса (кг)";
            // 
            // textBoxM
            // 
            textBoxM.Location = new Point(364, 32);
            textBoxM.Name = "textBoxM";
            textBoxM.Size = new Size(111, 27);
            textBoxM.TabIndex = 7;
            textBoxM.Text = "0,4";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(32, 109);
            label3.Name = "label3";
            label3.Size = new Size(89, 20);
            label3.TabIndex = 6;
            label3.Text = "Угол (град.)";
            // 
            // textBoxA
            // 
            textBoxA.Location = new Point(123, 105);
            textBoxA.Name = "textBoxA";
            textBoxA.Size = new Size(111, 27);
            textBoxA.TabIndex = 5;
            textBoxA.Text = "30";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 71);
            label2.Name = "label2";
            label2.Size = new Size(111, 20);
            label2.TabIndex = 4;
            label2.Text = "Скорость (м/с)";
            label2.Click += label2_Click;
            // 
            // textBoxV
            // 
            textBoxV.Location = new Point(123, 67);
            textBoxV.Name = "textBoxV";
            textBoxV.Size = new Size(111, 27);
            textBoxV.TabIndex = 3;
            textBoxV.Text = "50";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(37, 36);
            label1.Name = "label1";
            label1.Size = new Size(84, 20);
            label1.TabIndex = 2;
            label1.Text = "Высота (м)";
            label1.Click += label1_Click;
            // 
            // textBoxH
            // 
            textBoxH.Location = new Point(123, 32);
            textBoxH.Name = "textBoxH";
            textBoxH.Size = new Size(111, 27);
            textBoxH.TabIndex = 1;
            textBoxH.Text = "1,8";
            // 
            // button1
            // 
            button1.Location = new Point(496, 10);
            button1.Name = "button1";
            button1.Size = new Size(272, 153);
            button1.TabIndex = 0;
            button1.Text = "ЗАПУСК! (классический)";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // timer1
            // 
            timer1.Interval = 5;
            timer1.Tick += timer1_Tick;
            // 
            // results
            // 
            results.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            results.Location = new Point(0, 550);
            results.Name = "results";
            results.RowHeadersWidth = 51;
            results.Size = new Size(1088, 365);
            results.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1088, 914);
            Controls.Add(results);
            Controls.Add(groupBox1);
            Controls.Add(chart1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)chart1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)results).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private GroupBox groupBox1;
        private Button button1;
        private Label label4;
        private TextBox textBoxSt;
        private Label label5;
        private TextBox textBoxPl;
        private Label label6;
        private TextBox textBoxM;
        private Label label3;
        private TextBox textBoxA;
        private Label label2;
        private TextBox textBoxV;
        private Label label1;
        private TextBox textBoxH;
        private System.Windows.Forms.Timer timer1;
        private DataGridView results;
        private Button clear;
        private Button button2;
        private Label war;
    }
}
