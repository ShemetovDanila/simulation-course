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
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            labelMean = new Label();
            labelVariance = new Label();
            textBoxLambda = new TextBox();
            textBoxT = new TextBox();
            textBoxN = new TextBox();
            textBoxSeed = new TextBox();
            ((System.ComponentModel.ISupportInitialize)chart1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(166, 417);
            button1.Name = "button1";
            button1.Size = new Size(136, 60);
            button1.TabIndex = 0;
            button1.Text = "Старт";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 111);
            label1.Name = "label1";
            label1.Size = new Size(235, 20);
            label1.TabIndex = 1;
            label1.Text = "Интенсивность потока (Лямбда)";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 165);
            label2.Name = "label2";
            label2.Size = new Size(253, 20);
            label2.TabIndex = 2;
            label2.Text = "Продолжительность генерации (T)";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 220);
            label3.Name = "label3";
            label3.Size = new Size(226, 20);
            label3.TabIndex = 3;
            label3.Text = "Количество экспериментов (N)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(13, 272);
            label4.Name = "label4";
            label4.Size = new Size(176, 20);
            label4.TabIndex = 4;
            label4.Text = "Зерно генерации (seed)";
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            chart1.Legends.Add(legend1);
            chart1.Location = new Point(443, 21);
            chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            chart1.Series.Add(series1);
            chart1.Size = new Size(593, 375);
            chart1.TabIndex = 5;
            chart1.Text = "chart1";
            chart1.Click += chart1_Click;
            // 
            // labelMean
            // 
            labelMean.AutoSize = true;
            labelMean.Location = new Point(512, 417);
            labelMean.Name = "labelMean";
            labelMean.Size = new Size(79, 20);
            labelMean.TabIndex = 6;
            labelMean.Text = "labelMean";
            // 
            // labelVariance
            // 
            labelVariance.AutoSize = true;
            labelVariance.Location = new Point(512, 457);
            labelVariance.Name = "labelVariance";
            labelVariance.Size = new Size(98, 20);
            labelVariance.TabIndex = 7;
            labelVariance.Text = "labelVariance";
            // 
            // textBoxLambda
            // 
            textBoxLambda.Location = new Point(286, 111);
            textBoxLambda.Name = "textBoxLambda";
            textBoxLambda.Size = new Size(125, 27);
            textBoxLambda.TabIndex = 8;
            textBoxLambda.Text = "0,5";
            textBoxLambda.TextChanged += textBoxLambda_TextChanged;
            // 
            // textBoxT
            // 
            textBoxT.Location = new Point(286, 162);
            textBoxT.Name = "textBoxT";
            textBoxT.Size = new Size(125, 27);
            textBoxT.TabIndex = 9;
            textBoxT.Text = "20";
            textBoxT.TextChanged += textBoxT_TextChanged;
            // 
            // textBoxN
            // 
            textBoxN.Location = new Point(286, 217);
            textBoxN.Name = "textBoxN";
            textBoxN.Size = new Size(125, 27);
            textBoxN.TabIndex = 10;
            textBoxN.Text = "50";
            textBoxN.TextChanged += textBox3_TextChanged;
            // 
            // textBoxSeed
            // 
            textBoxSeed.Location = new Point(285, 272);
            textBoxSeed.Name = "textBoxSeed";
            textBoxSeed.Size = new Size(125, 27);
            textBoxSeed.TabIndex = 11;
            textBoxSeed.Text = "8080";
            textBoxSeed.TextChanged += textBoxSeed_TextChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1048, 502);
            Controls.Add(textBoxSeed);
            Controls.Add(textBoxN);
            Controls.Add(textBoxT);
            Controls.Add(textBoxLambda);
            Controls.Add(labelVariance);
            Controls.Add(labelMean);
            Controls.Add(chart1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)chart1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private Label labelMean;
        private Label labelVariance;
        private TextBox textBoxLambda;
        private TextBox textBoxT;
        private TextBox textBoxN;
        private TextBox textBoxSeed;
    }
}
