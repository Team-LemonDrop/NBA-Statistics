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
            this.btnImportZipDataToSqlServer = new System.Windows.Forms.Button();
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
            // btnImportZipDataToSqlServer
            // 
            this.btnImportZipDataToSqlServer.Location = new System.Drawing.Point(64, 155);
            this.btnImportZipDataToSqlServer.Name = "btnImportZipDataToSqlServer";
            this.btnImportZipDataToSqlServer.Size = new System.Drawing.Size(195, 23);
            this.btnImportZipDataToSqlServer.TabIndex = 2;
            this.btnImportZipDataToSqlServer.Text = "Import ZIP data to SQL Server";
            this.btnImportZipDataToSqlServer.UseVisualStyleBackColor = true;
            this.btnImportZipDataToSqlServer.Click += new System.EventHandler(this.btnImportZipDataToSqlServer_Click);
            // 
            // NBAStatsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 402);
            this.Controls.Add(this.btnImportZipDataToSqlServer);
            this.Controls.Add(this.btnFillMongoDb);
            this.Controls.Add(this.btnGenerateZipFile);
            this.Name = "NBAStatsForm";
            this.Text = "NBA Stats";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGenerateZipFile;
        private System.Windows.Forms.Button btnFillMongoDb;
        private System.Windows.Forms.Button btnImportZipDataToSqlServer;
    }
}

