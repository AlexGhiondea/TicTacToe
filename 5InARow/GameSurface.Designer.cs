namespace TicTacToe
{
    partial class TicTacToe
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
            this.GameSurface = new System.Windows.Forms.Panel();
            this.lblNext = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUndo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GameSurface
            // 
            this.GameSurface.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameSurface.Location = new System.Drawing.Point(2, 44);
            this.GameSurface.Name = "GameSurface";
            this.GameSurface.Size = new System.Drawing.Size(541, 356);
            this.GameSurface.TabIndex = 0;
            this.GameSurface.Paint += new System.Windows.Forms.PaintEventHandler(this.GameSurface_Paint);
            this.GameSurface.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GameSurface_MouseDown);
            this.GameSurface.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GameSurface_MouseMove);
            this.GameSurface.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GameSurface_MouseUp);
            // 
            // lblNext
            // 
            this.lblNext.AutoSize = true;
            this.lblNext.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNext.Location = new System.Drawing.Point(165, 6);
            this.lblNext.Name = "lblNext";
            this.lblNext.Size = new System.Drawing.Size(30, 32);
            this.lblNext.TabIndex = 1;
            this.lblNext.Text = "x";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "Next move:";
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(457, 12);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(75, 23);
            this.btnUndo.TabIndex = 3;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // TicTacToe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 401);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.lblNext);
            this.Controls.Add(this.GameSurface);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Name = "TicTacToe";
            this.Text = "5 in a row tic-tac-toe";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel GameSurface;
        private System.Windows.Forms.Label lblNext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUndo;
    }
}

