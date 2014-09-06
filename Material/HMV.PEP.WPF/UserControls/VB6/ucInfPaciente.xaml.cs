﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HMV.Core.Interfaces;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.VB6
{
    /// <summary>
    /// Interaction logic for ucInfPaciente.xaml
    /// </summary>
    public partial class ucInfPaciente : UserControlBase, IUserControl
    {                 
        public ucInfPaciente()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            try
            {
                System.Windows.Forms.Integration.WindowsFormsHost host =
                                          new System.Windows.Forms.Integration.WindowsFormsHost();

                IUserControlVB6 ActX = new IUserControlVB6();
                ActX.SetData("SigaNPEnfermagemMV.ctlInfPac");

                ActX.ChamaMetodo("Inicializa");

                host.Child = ActX;

                this.gridIntegra.Children.Add(host);
            }
            catch 
            {
                DXMessageBox.Show("Erro ao Inicializar componente VB6.\nEntre em contato com o setor responsável e solicite a instalação.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CancelClose
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}