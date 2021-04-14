namespace Client
{
    partial class Client
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelMessage = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBoxAmigos = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonAddAmigos = new System.Windows.Forms.Button();
            this.textBoxMensagens = new System.Windows.Forms.TextBox();
            this.labelAmigo = new System.Windows.Forms.Label();
            this.linkLabel_login = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new System.Drawing.Font("Cooper Black", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labelMessage.Location = new System.Drawing.Point(109, 453);
            this.labelMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(154, 31);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "Mensagem";
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Font = new System.Drawing.Font("Cooper Black", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMessage.Location = new System.Drawing.Point(109, 486);
            this.textBoxMessage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(481, 75);
            this.textBoxMessage.TabIndex = 1;
            // 
            // buttonSend
            // 
            this.buttonSend.Font = new System.Drawing.Font("Cooper Black", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSend.Location = new System.Drawing.Point(589, 486);
            this.buttonSend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(129, 76);
            this.buttonSend.TabIndex = 2;
            this.buttonSend.Text = "Enviar";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBoxAmigos
            // 
            this.textBoxAmigos.Font = new System.Drawing.Font("Cooper Black", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAmigos.Location = new System.Drawing.Point(727, 101);
            this.textBoxAmigos.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxAmigos.Multiline = true;
            this.textBoxAmigos.Name = "textBoxAmigos";
            this.textBoxAmigos.ReadOnly = true;
            this.textBoxAmigos.Size = new System.Drawing.Size(235, 461);
            this.textBoxAmigos.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Cooper Black", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Location = new System.Drawing.Point(720, 65);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 31);
            this.label1.TabIndex = 5;
            this.label1.Text = "Amigos ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonAddAmigos
            // 
            this.buttonAddAmigos.Font = new System.Drawing.Font("Cooper Black", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAddAmigos.Location = new System.Drawing.Point(907, 50);
            this.buttonAddAmigos.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonAddAmigos.Name = "buttonAddAmigos";
            this.buttonAddAmigos.Size = new System.Drawing.Size(56, 46);
            this.buttonAddAmigos.TabIndex = 6;
            this.buttonAddAmigos.Text = "+";
            this.buttonAddAmigos.UseVisualStyleBackColor = true;
            // 
            // textBoxMensagens
            // 
            this.textBoxMensagens.Font = new System.Drawing.Font("Cooper Black", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMensagens.Location = new System.Drawing.Point(109, 101);
            this.textBoxMensagens.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxMensagens.Multiline = true;
            this.textBoxMensagens.Name = "textBoxMensagens";
            this.textBoxMensagens.ReadOnly = true;
            this.textBoxMensagens.Size = new System.Drawing.Size(608, 347);
            this.textBoxMensagens.TabIndex = 7;
            // 
            // labelAmigo
            // 
            this.labelAmigo.AutoSize = true;
            this.labelAmigo.Font = new System.Drawing.Font("Cooper Black", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAmigo.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labelAmigo.Location = new System.Drawing.Point(109, 66);
            this.labelAmigo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAmigo.Name = "labelAmigo";
            this.labelAmigo.Size = new System.Drawing.Size(156, 31);
            this.labelAmigo.TabIndex = 8;
            this.labelAmigo.Text = "Utilizador";
            // 
            // linkLabel_login
            // 
            this.linkLabel_login.AutoSize = true;
            this.linkLabel_login.Location = new System.Drawing.Point(106, 9);
            this.linkLabel_login.Name = "linkLabel_login";
            this.linkLabel_login.Size = new System.Drawing.Size(43, 17);
            this.linkLabel_login.TabIndex = 9;
            this.linkLabel_login.TabStop = true;
            this.linkLabel_login.Text = "Login";
            this.linkLabel_login.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_login_LinkClicked);
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1053, 607);
            this.Controls.Add(this.linkLabel_login);
            this.Controls.Add(this.labelAmigo);
            this.Controls.Add(this.textBoxMensagens);
            this.Controls.Add(this.buttonAddAmigos);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxAmigos);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.labelMessage);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Client";
            this.Text = "Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Client_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxAmigos;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonAddAmigos;
        private System.Windows.Forms.TextBox textBoxMensagens;
        private System.Windows.Forms.Label labelAmigo;
        private System.Windows.Forms.LinkLabel linkLabel_login;
    }
}

