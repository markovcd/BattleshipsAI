namespace IntegrationTest
{
    partial class MainForm
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
            this.GameField = new System.Windows.Forms.TextBox();
            this.BattleshipsField = new System.Windows.Forms.TextBox();
            this.GenerateButton = new System.Windows.Forms.Button();
            this.RunButton = new System.Windows.Forms.Button();
            this.MovesList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // GameField
            // 
            this.GameField.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GameField.Location = new System.Drawing.Point(12, 12);
            this.GameField.Multiline = true;
            this.GameField.Name = "GameField";
            this.GameField.Size = new System.Drawing.Size(115, 186);
            this.GameField.TabIndex = 1;
            this.GameField.Text = "  0123456789\r\n0 ----------\r\n1 ----------\r\n2 ----------\r\n3 ----------\r\n4 ---------" +
    "-\r\n5 ----------\r\n6 ----------\r\n7 ----------\r\n8 ----------\r\n9 ----------";
            // 
            // BattleshipsField
            // 
            this.BattleshipsField.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.BattleshipsField.Location = new System.Drawing.Point(133, 12);
            this.BattleshipsField.Multiline = true;
            this.BattleshipsField.Name = "BattleshipsField";
            this.BattleshipsField.Size = new System.Drawing.Size(115, 186);
            this.BattleshipsField.TabIndex = 2;
            this.BattleshipsField.Text = "  0123456789\r\n0 ----------\r\n1 ----------\r\n2 ----------\r\n3 ----------\r\n4 ---------" +
    "-\r\n5 ----------\r\n6 ----------\r\n7 ----------\r\n8 ----------\r\n9 ----------";
            // 
            // GenerateButton
            // 
            this.GenerateButton.Location = new System.Drawing.Point(12, 204);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(115, 23);
            this.GenerateButton.TabIndex = 4;
            this.GenerateButton.Text = "Generate game";
            this.GenerateButton.UseVisualStyleBackColor = true;
            this.GenerateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // RunButton
            // 
            this.RunButton.Location = new System.Drawing.Point(133, 204);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(115, 23);
            this.RunButton.TabIndex = 5;
            this.RunButton.Text = "Run game";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // MovesList
            // 
            this.MovesList.FormattingEnabled = true;
            this.MovesList.Location = new System.Drawing.Point(254, 12);
            this.MovesList.Name = "MovesList";
            this.MovesList.Size = new System.Drawing.Size(91, 186);
            this.MovesList.TabIndex = 6;
            this.MovesList.SelectedIndexChanged += new System.EventHandler(this.MovesList_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 268);
            this.Controls.Add(this.MovesList);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.GenerateButton);
            this.Controls.Add(this.BattleshipsField);
            this.Controls.Add(this.GameField);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox GameField;
        private System.Windows.Forms.TextBox BattleshipsField;
        private System.Windows.Forms.Button GenerateButton;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.ListBox MovesList;

    }
}

