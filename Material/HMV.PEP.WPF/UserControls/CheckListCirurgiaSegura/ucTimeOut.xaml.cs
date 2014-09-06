using System;
using System.Windows.Input;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.CheckListDeCirurgia;
using StructureMap;
using System.Configuration;
using DevExpress.Xpf.Editors;
using HMV.Core.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.WPF.UserControls.CheckListCirurgiaSegura
{
    /// <summary>
    /// Interaction logic for ucTimeOut.xaml
    /// </summary>
    public partial class ucTimeOut : UserControlBase, IUserControl
    {
        //private bool _nomeAuxiliar = false;

        public ucTimeOut()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as vmCheckList).vmTimeOut;
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

        #region ----- Cirurgiao -----
        private void btnCirurgia_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(btnCirurgia.Text) && e.Key == Key.Enter)
            {
                (this.DataContext as vmTimeOut).TimeOut.Cirurgiao = AddPrestador(btnCirurgia.Text);
                if ((this.DataContext as vmTimeOut).TimeOut.Cirurgiao != null)
                    btnAuxiliar1.Focus();
            }
            else
                (this.DataContext as vmTimeOut).TimeOut.Cirurgiao = null;
        }

        private void btnCirurgia_KeyUp(object sender, KeyEventArgs e)
        {
            (this.DataContext as vmTimeOut).TimeOut.Cirurgiao = null;
        }

        private void btnCirurgia_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winSelProfissional win = new winSelProfissional(btnCirurgia.Text);
            if (win.ShowDialog(this.OwnerBase) == true)
            {
                (this.DataContext as vmTimeOut).TimeOut.Cirurgiao = new wrpPrestador(win.GetPrestador());
                btnAuxiliar1.Focus();
            }
        }
        #endregion

        #region ----- Instrumentador -----
        private void btnInstrumentador_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(btnInstrumentador.Text) && e.Key == Key.Enter)
            {
                (this.DataContext as vmTimeOut).TimeOut.Instrumentador = AddPrestador(btnInstrumentador.Text, true);
                if ((this.DataContext as vmTimeOut).TimeOut.Instrumentador != null)
                    btnCirculante.Focus();
            }
        }

        private void btnInstrumentador_KeyUp(object sender, KeyEventArgs e)
        {
            (this.DataContext as vmTimeOut).TimeOut.Instrumentador = null;
        }

        private void btnInstrumentador_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winSelProfissional win = new winSelProfissional(btnInstrumentador.Text);
            win.pFiltraTecnicos = true;
            if (win.ShowDialog(this.OwnerBase) == true)
            {
                (this.DataContext as vmTimeOut).TimeOut.Instrumentador = new wrpPrestador(win.GetPrestador());
                btnCirculante.Focus();
            }
        }

        #endregion

        #region ----- Circulante -----
        private void btnCirculante_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(btnCirculante.Text) && e.Key == Key.Enter)
            {
                (this.DataContext as vmTimeOut).TimeOut.Circulante = AddPrestador(btnCirculante.Text, true);
                btnAnestesista.Focus();
            }
        }

        private void btnCirculante_KeyUp(object sender, KeyEventArgs e)
        {
            (this.DataContext as vmTimeOut).TimeOut.Circulante = null;
        }

        private void btnCirculante_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winSelProfissional win = new winSelProfissional(btnCirculante.Text);
            win.pFiltraTecnicos = true;
            if (win.ShowDialog(this.OwnerBase) == true)
            {
                (this.DataContext as vmTimeOut).TimeOut.Circulante = new wrpPrestador(win.GetPrestador());
                btnAnestesista.Focus();
            }
        }

        #endregion

        #region ----- Auxiliar1 -----
        private void btnAuxiliar1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(btnAuxiliar1.Text) && e.Key == Key.Enter)
            {
                wrpPrestador pre = AddPrestador(btnAuxiliar1.Text);
                if (pre != null)
                {
                    (this.DataContext as vmTimeOut).TimeOut.Auxiliar1 = new wrpPrestadorOutros() { Registro = pre.Registro, Nome = pre.Nome };
                    btnAuxiliar2.Focus();
                }
            }
        }

        private void btnAuxiliar1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winSelProfissional win = new winSelProfissional(btnAuxiliar1.Text);
            if (win.ShowDialog(this.OwnerBase) == true)
            {
                wrpPrestador pre = new wrpPrestador(win.GetPrestador());
                if (pre != null)
                {
                    (this.DataContext as vmTimeOut).TimeOut.Auxiliar1 = new wrpPrestadorOutros() { Registro = pre.Registro, Nome = pre.Nome };
                    btnAuxiliar2.Focus();
                }
            }
        }

        private void btnAuxiliar1_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            try
            {
                if (e.NewValue != null)
                {
                    if ((this.DataContext as vmTimeOut).TimeOut.Auxiliar1 == null)
                        (this.DataContext as vmTimeOut).TimeOut.Auxiliar1 = new wrpPrestadorOutros();
                    (this.DataContext as vmTimeOut).TimeOut.Auxiliar1.Registro = (sender as ButtonEdit).Text;
                    if (string.IsNullOrWhiteSpace((this.DataContext as vmTimeOut).TimeOut.Auxiliar1.Registro))
                        (this.DataContext as vmTimeOut).TimeOut.Auxiliar1 = null;
                }
            }
            catch { }
        }

        private void txtAuxiliar1_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(e.NewValue.ToNullSafeString()))
                {
                    if ((this.DataContext as vmTimeOut).TimeOut.Auxiliar1 == null)
                        (this.DataContext as vmTimeOut).TimeOut.Auxiliar1 = new wrpPrestadorOutros();
                    (this.DataContext as vmTimeOut).TimeOut.Auxiliar1.Nome = (sender as TextEdit).Text;
                }
            }
            catch { }
        }

        #endregion

        #region ----- Auxiliar2 -----
        private void btnAuxiliar2_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(btnAuxiliar2.Text) && e.Key == Key.Enter)
            {
                wrpPrestador pre = AddPrestador(btnAuxiliar2.Text);
                if (pre != null)
                {
                    (this.DataContext as vmTimeOut).TimeOut.Auxiliar2 = new wrpPrestadorOutros() { Registro = pre.Registro, Nome = pre.Nome };
                    btnInstrumentador.Focus();
                }
            }
        }

        private void btnAuxiliar2_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winSelProfissional win = new winSelProfissional(btnAuxiliar2.Text);
            if (win.ShowDialog(this.OwnerBase) == true)
            {
                wrpPrestador pre = new wrpPrestador(win.GetPrestador());
                if (pre != null)
                {
                    (this.DataContext as vmTimeOut).TimeOut.Auxiliar2 = new wrpPrestadorOutros() { Registro = pre.Registro, Nome = pre.Nome };
                    btnInstrumentador.Focus();
                }
            }
        }

        private void btnAuxiliar2_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            try
            {
                if (e.NewValue != null)
                {
                    if ((this.DataContext as vmTimeOut).TimeOut.Auxiliar2 == null)
                        (this.DataContext as vmTimeOut).TimeOut.Auxiliar2 = new wrpPrestadorOutros();
                    (this.DataContext as vmTimeOut).TimeOut.Auxiliar2.Registro = (sender as ButtonEdit).Text;
                    if (string.IsNullOrWhiteSpace((this.DataContext as vmTimeOut).TimeOut.Auxiliar2.Registro))
                        (this.DataContext as vmTimeOut).TimeOut.Auxiliar2 = null;
                }
            }
            catch { }
        }

        private void txtAuxiliar2_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(e.NewValue.ToNullSafeString()))
                {
                    if ((this.DataContext as vmTimeOut).TimeOut.Auxiliar2 == null)
                        (this.DataContext as vmTimeOut).TimeOut.Auxiliar2 = new wrpPrestadorOutros();
                    (this.DataContext as vmTimeOut).TimeOut.Auxiliar2.Nome = (sender as TextEdit).Text;
                }
            }
            catch { }
        }

        #endregion

        #region ----- Anestesista -----
        private void btnAnestesista_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(btnAnestesista.Text) && e.Key == Key.Enter)
            {
                wrpPrestador pre = AddPrestador(btnAnestesista.Text);
                if (pre != null)
                {
                    (this.DataContext as vmTimeOut).TimeOut.Anestesista = new wrpPrestadorOutros() { Registro = pre.Registro, Nome = pre.Nome };
                }
            }
        }

        private void btnAnestesista_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winSelProfissional win = new winSelProfissional(btnAnestesista.Text);
            if (win.ShowDialog(this.OwnerBase) == true)
            {
                wrpPrestador pre = new wrpPrestador(win.GetPrestador());
                (this.DataContext as vmTimeOut).TimeOut.Anestesista = new wrpPrestadorOutros() { Registro = pre.Registro, Nome = pre.Nome };
            }
        }

        private void btnAnestesista_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {  try
            {
                if (e.NewValue != null)
                {
            if ((this.DataContext as vmTimeOut).TimeOut.Anestesista == null) (this.DataContext as vmTimeOut).TimeOut.Anestesista = new wrpPrestadorOutros();
            (this.DataContext as vmTimeOut).TimeOut.Anestesista.Registro = (sender as ButtonEdit).Text;
            if (string.IsNullOrWhiteSpace((this.DataContext as vmTimeOut).TimeOut.Anestesista.Registro))
                (this.DataContext as vmTimeOut).TimeOut.Anestesista = null;   }
            }
            catch { }
        }

        private void txtAnestesista_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(e.NewValue.ToNullSafeString()))
                {
                    if ((this.DataContext as vmTimeOut).TimeOut.Anestesista == null)
                        (this.DataContext as vmTimeOut).TimeOut.Anestesista = new wrpPrestadorOutros();
                    (this.DataContext as vmTimeOut).TimeOut.Anestesista.Nome = (sender as TextEdit).Text;
                }
            }
            catch { }
        }
        #endregion

        #region ----- Metodos Privados -----
        private wrpPrestador AddPrestador(string Registro, bool pFiltraTecnicos = false)
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            int idClin = int.Parse(ConfigurationManager.AppSettings["ClinicaDefault"].ToString());

            serv.FiltraPorRegistro(Registro);
            serv.FiltraPorClinica(idClin);
            serv.FiltraOndeRegistroInformado();
            IList<Prestador> listaPrestador = new List<Prestador>();
            if (pFiltraTecnicos)
                listaPrestador = serv.Carrega().Where(x => x.Conselho.cd_conselho == 2).ToList();
            else
                listaPrestador = serv.Carrega();

            if (listaPrestador.Count.Equals(1))
                return new wrpPrestador(listaPrestador.FirstOrDefault());

            else if (listaPrestador.Count > 1)
            {
                winSelProfissional win = new winSelProfissional(Registro);
                win.pFiltraTecnicos = pFiltraTecnicos;
                if (win.ShowDialog(this.OwnerBase) == true)
                    return new wrpPrestador(win.GetPrestador());
                return null;
            }
            return null;
        }
        #endregion
    }
}