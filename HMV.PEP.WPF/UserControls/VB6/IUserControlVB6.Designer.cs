namespace HMV.PEP.WPF.UserControls.VB6
{
    partial class IUserControlVB6
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IUserControlVB6));
            this.axctlIntegra = new AxSigaIntegradorNMV.AxctlIntegra();
            ((System.ComponentModel.ISupportInitialize)(this.axctlIntegra)).BeginInit();
            this.SuspendLayout();
            // 
            // axctlIntegra
            // 
            this.axctlIntegra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axctlIntegra.Enabled = true;
            this.axctlIntegra.Location = new System.Drawing.Point(23, 21);
            this.axctlIntegra.Name = "axctlIntegra";
            this.axctlIntegra.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axctlIntegra.OcxState")));
            this.axctlIntegra.Size = new System.Drawing.Size(394, 457);
            this.axctlIntegra.TabIndex = 0;
            // 
            // IUserControlVB6
            // 
            this.Controls.Add(this.axctlIntegra);
            this.Name = "IUserControlVB6";
            ((System.ComponentModel.ISupportInitialize)(this.axctlIntegra)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxSigaIntegradorNMV.AxctlIntegra axctlIntegra;

        //private AxSigaIntegradorNMV.AxctlIntegra axctlIntegra;

        //private AxSigaIntegradorNMV.AxctlIntegra axctlIntegra;




    }
}
