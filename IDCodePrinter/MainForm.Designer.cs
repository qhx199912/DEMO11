﻿namespace IDCodePrinter
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.btnQuery = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.btnPrint = new System.Windows.Forms.Button();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.Column17 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column22 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column23 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column24 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column20 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAutoPrint = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cmbGrade = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtR = new System.Windows.Forms.TextBox();
            this.txtF = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtBule = new System.Windows.Forms.TextBox();
            this.cmbSpec = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 30000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy年MM月dd日 HH:mm:ss";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(55, 17);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(182, 21);
            this.dateTimePicker1.TabIndex = 0;
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(437, 17);
            this.btnQuery.Margin = new System.Windows.Forms.Padding(2);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(65, 22);
            this.btnQuery.TabIndex = 1;
            this.btnQuery.Text = "查找";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "时间";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "yyyy年MM月dd日 HH:mm:ss";
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(241, 17);
            this.dateTimePicker2.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(182, 21);
            this.dateTimePicker2.TabIndex = 0;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(502, 17);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(65, 22);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "打印";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false;
            this.DGV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column17,
            this.Column21,
            this.Column22,
            this.Column1,
            this.Column23,
            this.Column24,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8,
            this.Column9,
            this.Column10,
            this.Column11,
            this.Column12,
            this.Column13,
            this.Column14,
            this.Column15,
            this.Column16,
            this.Column18,
            this.Column19,
            this.Column20});
            this.DGV.Location = new System.Drawing.Point(18, 54);
            this.DGV.Margin = new System.Windows.Forms.Padding(2);
            this.DGV.Name = "DGV";
            this.DGV.RowHeadersWidth = 51;
            this.DGV.RowTemplate.Height = 27;
            this.DGV.Size = new System.Drawing.Size(1148, 377);
            this.DGV.TabIndex = 4;
            // 
            // Column17
            // 
            this.Column17.DataPropertyName = "status";
            this.Column17.HeaderText = "选择";
            this.Column17.MinimumWidth = 6;
            this.Column17.Name = "Column17";
            this.Column17.Width = 50;
            // 
            // Column21
            // 
            this.Column21.DataPropertyName = "create_Time";
            this.Column21.HeaderText = "新增时间";
            this.Column21.MinimumWidth = 6;
            this.Column21.Name = "Column21";
            this.Column21.Width = 125;
            // 
            // Column22
            // 
            this.Column22.DataPropertyName = "Print_Time";
            this.Column22.HeaderText = "打印时间";
            this.Column22.MinimumWidth = 6;
            this.Column22.Name = "Column22";
            this.Column22.Width = 125;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "ROLL_PLAN_NO";
            this.Column1.HeaderText = "轧制计划号";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            this.Column1.Width = 125;
            // 
            // Column23
            // 
            this.Column23.DataPropertyName = "NewROLLCode";
            this.Column23.HeaderText = "南钢轧制号";
            this.Column23.MinimumWidth = 6;
            this.Column23.Name = "Column23";
            this.Column23.Width = 125;
            // 
            // Column24
            // 
            this.Column24.DataPropertyName = "NewHEAT_NO";
            this.Column24.HeaderText = "南钢炉号";
            this.Column24.MinimumWidth = 6;
            this.Column24.Name = "Column24";
            this.Column24.Width = 125;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "UNIT_CODE";
            this.Column2.HeaderText = "机组代码";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.Width = 125;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "IN_MAT_NO";
            this.Column3.HeaderText = "入口材料号";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.Width = 125;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "OUT_MAT_NO";
            this.Column4.HeaderText = "出口材料号";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            this.Column4.Width = 125;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "ORDER_NO";
            this.Column5.HeaderText = "合同号";
            this.Column5.MinimumWidth = 6;
            this.Column5.Name = "Column5";
            this.Column5.Width = 125;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "HEAT_NO";
            this.Column6.HeaderText = "炉号";
            this.Column6.MinimumWidth = 6;
            this.Column6.Name = "Column6";
            this.Column6.Width = 125;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "SG_STD";
            this.Column7.HeaderText = "标准";
            this.Column7.MinimumWidth = 6;
            this.Column7.Name = "Column7";
            this.Column7.Width = 125;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "SG_SIGN";
            this.Column8.HeaderText = "牌号";
            this.Column8.MinimumWidth = 6;
            this.Column8.Name = "Column8";
            this.Column8.Width = 125;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "HOOK_NO";
            this.Column9.HeaderText = "钩号";
            this.Column9.MinimumWidth = 6;
            this.Column9.Name = "Column9";
            this.Column9.Width = 125;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "MAT_ACT_THICK";
            this.Column10.HeaderText = "材料实际直径";
            this.Column10.MinimumWidth = 6;
            this.Column10.Name = "Column10";
            this.Column10.Width = 125;
            // 
            // Column11
            // 
            this.Column11.DataPropertyName = "MAT_ACT_LEN";
            this.Column11.HeaderText = "材料实际长度";
            this.Column11.MinimumWidth = 6;
            this.Column11.Name = "Column11";
            this.Column11.Width = 125;
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "MAT_NUM";
            this.Column12.HeaderText = "材料支数";
            this.Column12.MinimumWidth = 6;
            this.Column12.Name = "Column12";
            this.Column12.Width = 125;
            // 
            // Column13
            // 
            this.Column13.DataPropertyName = "MAT_ACT_WT";
            this.Column13.HeaderText = "实际重量";
            this.Column13.MinimumWidth = 6;
            this.Column13.Name = "Column13";
            this.Column13.Width = 125;
            // 
            // Column14
            // 
            this.Column14.DataPropertyName = "MAT_THEORY_WT";
            this.Column14.HeaderText = "材料理论重量";
            this.Column14.MinimumWidth = 6;
            this.Column14.Name = "Column14";
            this.Column14.Width = 125;
            // 
            // Column15
            // 
            this.Column15.DataPropertyName = "SUR_DEC_CODE";
            this.Column15.HeaderText = "表面判定结果";
            this.Column15.MinimumWidth = 6;
            this.Column15.Name = "Column15";
            this.Column15.Width = 125;
            // 
            // Column16
            // 
            this.Column16.DataPropertyName = "PROD_TIM";
            this.Column16.HeaderText = "生产时刻";
            this.Column16.MinimumWidth = 6;
            this.Column16.Name = "Column16";
            this.Column16.Width = 125;
            // 
            // Column18
            // 
            this.Column18.DataPropertyName = "SHIFT_NO";
            this.Column18.HeaderText = "班次";
            this.Column18.MinimumWidth = 6;
            this.Column18.Name = "Column18";
            this.Column18.Width = 125;
            // 
            // Column19
            // 
            this.Column19.DataPropertyName = "SHIFT_GROUP";
            this.Column19.HeaderText = "班组";
            this.Column19.MinimumWidth = 6;
            this.Column19.Name = "Column19";
            this.Column19.Width = 125;
            // 
            // Column20
            // 
            this.Column20.DataPropertyName = "PROD_MAKER";
            this.Column20.HeaderText = "生产责任者";
            this.Column20.MinimumWidth = 6;
            this.Column20.Name = "Column20";
            this.Column20.Width = 125;
            // 
            // btnAutoPrint
            // 
            this.btnAutoPrint.Location = new System.Drawing.Point(567, 17);
            this.btnAutoPrint.Margin = new System.Windows.Forms.Padding(2);
            this.btnAutoPrint.Name = "btnAutoPrint";
            this.btnAutoPrint.Size = new System.Drawing.Size(65, 22);
            this.btnAutoPrint.TabIndex = 1;
            this.btnAutoPrint.Text = "自动打印";
            this.btnAutoPrint.UseVisualStyleBackColor = true;
            this.btnAutoPrint.Click += new System.EventHandler(this.btnAutoPrint_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(1097, 4);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(65, 46);
            this.btnOK.TabIndex = 21;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cmbGrade
            // 
            this.cmbGrade.FormattingEnabled = true;
            this.cmbGrade.Items.AddRange(new object[] {
            "HRB400E",
            "HRB400"});
            this.cmbGrade.Location = new System.Drawing.Point(722, 4);
            this.cmbGrade.Name = "cmbGrade";
            this.cmbGrade.Size = new System.Drawing.Size(69, 20);
            this.cmbGrade.TabIndex = 27;
            this.cmbGrade.Text = "HRB400E";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(690, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 26;
            this.label7.Text = "牌号";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(809, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 31;
            this.label3.Text = "初始轧制号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(821, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 30;
            this.label2.Text = "初始炉号";
            // 
            // txtR
            // 
            this.txtR.Location = new System.Drawing.Point(876, 29);
            this.txtR.Name = "txtR";
            this.txtR.Size = new System.Drawing.Size(74, 21);
            this.txtR.TabIndex = 29;
            this.txtR.Text = "C11000217";
            this.txtR.TextChanged += new System.EventHandler(this.txtF_TextChanged);
            // 
            // txtF
            // 
            this.txtF.Location = new System.Drawing.Point(876, 4);
            this.txtF.Name = "txtF";
            this.txtF.Size = new System.Drawing.Size(74, 21);
            this.txtF.TabIndex = 28;
            this.txtF.Text = "21697116";
            this.txtF.TextChanged += new System.EventHandler(this.txtF_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(965, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 33;
            this.label6.Text = "当前捆包号";
            // 
            // txtBule
            // 
            this.txtBule.Location = new System.Drawing.Point(1032, 3);
            this.txtBule.Name = "txtBule";
            this.txtBule.Size = new System.Drawing.Size(56, 21);
            this.txtBule.TabIndex = 32;
            this.txtBule.Text = "7001";
            this.txtBule.TextChanged += new System.EventHandler(this.txtBule_TextChanged);
            // 
            // cmbSpec
            // 
            this.cmbSpec.FormattingEnabled = true;
            this.cmbSpec.Items.AddRange(new object[] {
            "6.0X0",
            "8.0X0",
            "10.0X0",
            "12.0X0"});
            this.cmbSpec.Location = new System.Drawing.Point(722, 29);
            this.cmbSpec.Name = "cmbSpec";
            this.cmbSpec.Size = new System.Drawing.Size(69, 20);
            this.cmbSpec.TabIndex = 35;
            this.cmbSpec.Text = "6.0X0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(690, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 34;
            this.label4.Text = "规格";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1168, 442);
            this.Controls.Add(this.cmbSpec);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtBule);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtR);
            this.Controls.Add(this.txtF);
            this.Controls.Add(this.cmbGrade);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.DGV);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAutoPrint);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "标牌打印服务";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column17;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column21;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column22;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column23;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column24;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column18;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column19;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column20;
        private System.Windows.Forms.Button btnAutoPrint;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cmbGrade;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtR;
        private System.Windows.Forms.TextBox txtF;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBule;
        private System.Windows.Forms.ComboBox cmbSpec;
        private System.Windows.Forms.Label label4;
    }
}

