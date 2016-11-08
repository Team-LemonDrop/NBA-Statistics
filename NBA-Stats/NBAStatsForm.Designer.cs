namespace NBA_Stats
{
    partial class NBAStatsForm
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
            this.btnGenerateZipFile = new System.Windows.Forms.Button();
            this.btnFillMongoDb = new System.Windows.Forms.Button();
            this.btnImportDataIntoSqlServer = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGenerateZipFile
            // 
            this.btnGenerateZipFile.Location = new System.Drawing.Point(64, 60);
            this.btnGenerateZipFile.Name = "btnGenerateZipFile";
            this.btnGenerateZipFile.Size = new System.Drawing.Size(124, 23);
            this.btnGenerateZipFile.TabIndex = 0;
            this.btnGenerateZipFile.Text = "Generate ZIP file";
            this.btnGenerateZipFile.UseVisualStyleBackColor = true;
            this.btnGenerateZipFile.Click += new System.EventHandler(this.btnGenerateZipFile_Click);
            // 
            // btnFillMongoDb
            // 
            this.btnFillMongoDb.Location = new System.Drawing.Point(64, 104);
            this.btnFillMongoDb.Name = "btnFillMongoDb";
            this.btnFillMongoDb.Size = new System.Drawing.Size(124, 23);
            this.btnFillMongoDb.TabIndex = 1;
            this.btnFillMongoDb.Text = "Fill Mongo DB";
            this.btnFillMongoDb.UseVisualStyleBackColor = true;
            this.btnFillMongoDb.Click += new System.EventHandler(this.btnFillMongoDb_Click);
            // 
            // btnImportDataIntoSqlServer
            // 
            this.btnImportDataIntoSqlServer.Location = new System.Drawing.Point(64, 155);
            this.btnImportDataIntoSqlServer.Name = "btnImportDataIntoSqlServer";
            this.btnImportDataIntoSqlServer.Size = new System.Drawing.Size(180, 23);
            this.btnImportDataIntoSqlServer.TabIndex = 2;
            this.btnImportDataIntoSqlServer.Text = "Import data into SQL Server";
            this.btnImportDataIntoSqlServer.UseVisualStyleBackColor = true;
            this.btnImportDataIntoSqlServer.Click += new System.EventHandler(this.btnImportDataIntoSqlServer_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(64, 211);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(253, 103);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Fill SqliteDb";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // NBAStatsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 402);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnImportDataIntoSqlServer);
            this.Controls.Add(this.btnFillMongoDb);
            this.Controls.Add(this.btnGenerateZipFile);
            this.Name = "NBAStatsForm";
            this.Text = "NBA Stats";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGenerateZipFile;
        private System.Windows.Forms.Button btnFillMongoDb;
        private System.Windows.Forms.Button btnImportDataIntoSqlServer;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

