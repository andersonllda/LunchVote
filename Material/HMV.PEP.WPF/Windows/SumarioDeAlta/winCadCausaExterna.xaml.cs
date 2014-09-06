using System.Windows;
using HMV.Core.Wrappers;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Exception;
using NHibernate.Validator.Engine;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
using DevExpress.Xpf.Editors;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Cadastros.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for winCadCausaExterna.xaml
    /// </summary>
    public partial class winCadCausaExterna : WindowBase
    {
        public winCadCausaExterna(vmCausaExterna pSumarioData)
        {
            InitializeComponent();
            if (pSumarioData != null && pSumarioData.CausaExternaSelecionada != null)           
                pSumarioData.CausaExternaSelecionada = new wrpCausaExterna(new CausaExterna(null));
         
            this.DataContext = pSumarioData;
        }

        private void txtObservacao_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtObservacao.Text))
                label1.Content = "Máximo 256 caracteres";
            else label1.Content = string.Format(txtObservacao.Text.Length < 255 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 256 - txtObservacao.Text.Length);

        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            winSelCID win = new winSelCID(true);
            win.ShowDialog(this);
            ICidService srv = ObjectFactory.GetInstance<ICidService>();
            if (win.CID != null)
            {
                (this.DataContext as vmCausaExterna).SetaCausaExternaSelecionada(win.CID);
            }
            else
            {
                (this.DataContext as vmCausaExterna).SetaCausaExternaSelecionada(null);
            }
        }

        private void cid_EditValueChanging(object sender, EditValueChangingEventArgs e)
        {
            try
            {
                ICidService serv = ObjectFactory.GetInstance<ICidService>();
                Cid cid = serv.FiltraPorCid10(e.NewValue.ToString());
                if (cid != null)
                    (this.DataContext as vmCausaExterna).SetaCausaExternaSelecionada(cid);
                else
                    (this.DataContext as vmCausaExterna).SetaCausaExternaSelecionada(null);
            }
            catch (BusinessValidatorException err)
            {
                IList<InvalidValue> erros = err.GetErros();
                DXMessageBox.Show(erros[0].Message);
            }
        }
    }
}
