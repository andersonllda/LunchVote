using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Repository;

namespace HMV.PEP.WPF
{
    public partial class winSumarioAtendimentoLegenda : WindowBase
    {
        public winSumarioAtendimentoLegenda()
        {
            InitializeComponent();           
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }     
    }
}
