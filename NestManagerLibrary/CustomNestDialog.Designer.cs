namespace NestManagerLibrary
{
    partial class CustomNestDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lblJobNumber = new System.Windows.Forms.Label();
            this.textJobNumber = new System.Windows.Forms.TextBox();
            this.textPieceMark = new System.Windows.Forms.TextBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.textQuantity = new System.Windows.Forms.TextBox();
            this.lblThickness = new System.Windows.Forms.Label();
            this.comboThicknessToAdd = new System.Windows.Forms.ComboBox();
            this.btnAddPieceMark = new System.Windows.Forms.Button();
            this.dgvCustomCutlist = new System.Windows.Forms.DataGridView();
            this.buttonOK = new System.Windows.Forms.Button();
            this.btnRemoveRecutItem = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomCutlist)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "PieceMark:";
            // 
            // lblJobNumber
            // 
            this.lblJobNumber.AutoSize = true;
            this.lblJobNumber.Location = new System.Drawing.Point(12, 49);
            this.lblJobNumber.Name = "lblJobNumber";
            this.lblJobNumber.Size = new System.Drawing.Size(67, 13);
            this.lblJobNumber.TabIndex = 1;
            this.lblJobNumber.Text = "Job Number:";
            // 
            // textJobNumber
            // 
            this.textJobNumber.Location = new System.Drawing.Point(85, 46);
            this.textJobNumber.Name = "textJobNumber";
            this.textJobNumber.Size = new System.Drawing.Size(140, 20);
            this.textJobNumber.TabIndex = 2;
            // 
            // textPieceMark
            // 
            this.textPieceMark.Location = new System.Drawing.Point(85, 74);
            this.textPieceMark.Name = "textPieceMark";
            this.textPieceMark.Size = new System.Drawing.Size(140, 20);
            this.textPieceMark.TabIndex = 3;
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(30, 105);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(49, 13);
            this.lblQuantity.TabIndex = 4;
            this.lblQuantity.Text = "Quantity:";
            // 
            // textQuantity
            // 
            this.textQuantity.Location = new System.Drawing.Point(85, 102);
            this.textQuantity.Name = "textQuantity";
            this.textQuantity.Size = new System.Drawing.Size(37, 20);
            this.textQuantity.TabIndex = 5;
            // 
            // lblThickness
            // 
            this.lblThickness.AutoSize = true;
            this.lblThickness.Location = new System.Drawing.Point(18, 22);
            this.lblThickness.Name = "lblThickness";
            this.lblThickness.Size = new System.Drawing.Size(59, 13);
            this.lblThickness.TabIndex = 6;
            this.lblThickness.Text = "Thickness:";
            // 
            // comboThicknessToAdd
            // 
            this.comboThicknessToAdd.FormattingEnabled = true;
            this.comboThicknessToAdd.Location = new System.Drawing.Point(85, 19);
            this.comboThicknessToAdd.Name = "comboThicknessToAdd";
            this.comboThicknessToAdd.Size = new System.Drawing.Size(121, 21);
            this.comboThicknessToAdd.TabIndex = 7;
            this.comboThicknessToAdd.SelectedValueChanged += new System.EventHandler(this.ThicknessValueChanged);
            // 
            // btnAddPieceMark
            // 
            this.btnAddPieceMark.Location = new System.Drawing.Point(85, 128);
            this.btnAddPieceMark.Name = "btnAddPieceMark";
            this.btnAddPieceMark.Size = new System.Drawing.Size(121, 21);
            this.btnAddPieceMark.TabIndex = 8;
            this.btnAddPieceMark.Text = "Add To List";
            this.btnAddPieceMark.UseVisualStyleBackColor = true;
            this.btnAddPieceMark.Click += new System.EventHandler(this.AddRecutItem);
            // 
            // dgvCustomCutlist
            // 
            this.dgvCustomCutlist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomCutlist.Location = new System.Drawing.Point(331, 12);
            this.dgvCustomCutlist.Name = "dgvCustomCutlist";
            this.dgvCustomCutlist.Size = new System.Drawing.Size(593, 179);
            this.dgvCustomCutlist.TabIndex = 9;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(671, 197);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(208, 32);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // btnRemoveRecutItem
            // 
            this.btnRemoveRecutItem.Location = new System.Drawing.Point(251, 12);
            this.btnRemoveRecutItem.Name = "btnRemoveRecutItem";
            this.btnRemoveRecutItem.Size = new System.Drawing.Size(74, 23);
            this.btnRemoveRecutItem.TabIndex = 11;
            this.btnRemoveRecutItem.Text = "Remove Selected Item";
            this.btnRemoveRecutItem.UseVisualStyleBackColor = true;
            this.btnRemoveRecutItem.Click += new System.EventHandler(this.RemoveSelectedItem);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(517, 197);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(96, 32);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // CustomNestDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(936, 242);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.btnRemoveRecutItem);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.dgvCustomCutlist);
            this.Controls.Add(this.btnAddPieceMark);
            this.Controls.Add(this.comboThicknessToAdd);
            this.Controls.Add(this.lblThickness);
            this.Controls.Add(this.textQuantity);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.textPieceMark);
            this.Controls.Add(this.textJobNumber);
            this.Controls.Add(this.lblJobNumber);
            this.Controls.Add(this.label1);
            this.Name = "CustomNestDialog";
            this.Text = "CustomNestDialog";
            this.Load += new System.EventHandler(this.CustomNestDialogLoad);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomCutlist)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblJobNumber;
        private System.Windows.Forms.TextBox textJobNumber;
        private System.Windows.Forms.TextBox textPieceMark;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.TextBox textQuantity;
        private System.Windows.Forms.Label lblThickness;
        private System.Windows.Forms.ComboBox comboThicknessToAdd;
        private System.Windows.Forms.Button btnAddPieceMark;
        private System.Windows.Forms.DataGridView dgvCustomCutlist;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button btnRemoveRecutItem;
        private System.Windows.Forms.Button buttonCancel;
    }
}