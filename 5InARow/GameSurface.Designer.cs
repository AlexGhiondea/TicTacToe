﻿namespace TicTacToe
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
            this.button1 = new System.Windows.Forms.Button();
            this.rbPlayer = new System.Windows.Forms.RadioButton();
            this.rbAI = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GameSurface
            // 
            this.GameSurface.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameSurface.Location = new System.Drawing.Point(2, 62);
            this.GameSurface.Name = "GameSurface";
            this.GameSurface.Size = new System.Drawing.Size(541, 360);
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
            this.lblNext.Location = new System.Drawing.Point(165, 26);
            this.lblNext.Name = "lblNext";
            this.lblNext.Size = new System.Drawing.Size(30, 32);
            this.lblNext.TabIndex = 1;
            this.lblNext.Text = "x";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "Next move:";
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(457, 32);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(75, 23);
            this.btnUndo.TabIndex = 3;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(364, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Suggest move";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // rbPlayer
            // 
            this.rbPlayer.AutoSize = true;
            this.rbPlayer.Checked = true;
            this.rbPlayer.Location = new System.Drawing.Point(224, 34);
            this.rbPlayer.Name = "rbPlayer";
            this.rbPlayer.Size = new System.Drawing.Size(54, 17);
            this.rbPlayer.TabIndex = 5;
            this.rbPlayer.TabStop = true;
            this.rbPlayer.Text = "Player";
            this.rbPlayer.UseVisualStyleBackColor = true;
            // 
            // rbAI
            // 
            this.rbAI.AutoSize = true;
            this.rbAI.Location = new System.Drawing.Point(284, 35);
            this.rbAI.Name = "rbAI";
            this.rbAI.Size = new System.Drawing.Size(35, 17);
            this.rbAI.TabIndex = 6;
            this.rbAI.Text = "AI";
            this.rbAI.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(544, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameToolStripMenuItem});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.gameToolStripMenuItem.Text = "Game";
            // 
            // newGameToolStripMenuItem
            // 
            this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            this.newGameToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newGameToolStripMenuItem.Text = "New game";
            this.newGameToolStripMenuItem.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click);
            // 
            // TicTacToe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 423);
            this.Controls.Add(this.rbAI);
            this.Controls.Add(this.rbPlayer);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.lblNext);
            this.Controls.Add(this.GameSurface);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TicTacToe";
            this.Text = "5 in a row tic-tac-toe";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel GameSurface;
        private System.Windows.Forms.Label lblNext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton rbPlayer;
        private System.Windows.Forms.RadioButton rbAI;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
    }
}

