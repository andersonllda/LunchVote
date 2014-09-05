using System;
using System.Windows;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.Services;
using HMV.Core.Domain.Enum;
using System.Collections.Generic;

namespace HMV.PEP.WPF
{
    public partial class ucTabEmergenciaNova : UserControlBase, IUserControl
    {       
        #region --- Construtor ---       

        public ucTabEmergenciaNova()
        {
            InitializeComponent();
            _dictionaryTab = new Dictionary<int, ucPacienteEmergencia>();
            
            _ultimaTab = App.BuscaChaveUltimoLog("TAB_EMERGENCIA");
            tabEmergencia.SelectedTabIndex = _ultimaTab;
        }

        #endregion     

        #region --- Propriedades Privadas --- 
        private int _ultimaTab;
        private Dictionary<int, ucPacienteEmergencia> _dictionaryTab;
        #endregion

        #region --- Metodos Publicos --- 
        public void SetData(object pData)
        {
            throw new NotImplementedException();
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

        public event EventHandler<EventArgs> DoubleClick;
        public event EventHandler<EventArgs> TabChildChanged;
        public void EventoDoubleClick(){
            
            if (DoubleClick != null)
                DoubleClick(null, null);
        }
        public void EventoTabChildChanged()
        {

            if (TabChildChanged != null)
                TabChildChanged(null, null);
        }

        public int AtendimentoEmergencia
        {
            get {
                return _dictionaryTab[tabEmergencia.SelectedTabIndex].AtendimentoEmergencia;
            }
        }

        public int AtendimentoInternadoEmergencia
        {
            get
            {
                return _dictionaryTab[tabEmergencia.SelectedTabIndex].AtendimentoInternadoEmergencia;
            }
        }

        public void DepoisDeEntrarNoProntuario()
        {
            _dictionaryTab[tabEmergencia.SelectedTabIndex].DepoisDeEntrarNoProntuario();
        }

        public void StopTimer()
        {
            _dictionaryTab[tabEmergencia.SelectedTabIndex].StopTimer();
        }

        public int TipoTab
        {
            get
            {
                return tabEmergencia.SelectedTabIndex;
            }
        }
        #endregion 

        #region --- Eventos --- 

        private void LayoutGroup_SelectedTabChildChanged(object sender, DevExpress.Xpf.Core.ValueChangedEventArgs<FrameworkElement> e)
        {
            EventoTabChildChanged();
            if (!_dictionaryTab.ContainsKey(tabEmergencia.SelectedTabIndex))
            {
                if (tabEmergencia.SelectedTabIndex == (int)TipoTabEmergencia.Adulto)
                {
                    _dictionaryTab.Add(tabEmergencia.SelectedTabIndex, new ucPacienteEmergencia(this, (TipoTabEmergencia)tabEmergencia.SelectedTabIndex, new EmergenciaAdultoService()));
                    gridTabEmergenciaAdulto.Children.Add(_dictionaryTab[tabEmergencia.SelectedTabIndex]);
                }
                if (tabEmergencia.SelectedTabIndex == (int)TipoTabEmergencia.Pediatrica)
                {
                    _dictionaryTab.Add(tabEmergencia.SelectedTabIndex, new ucPacienteEmergencia(this, (TipoTabEmergencia)tabEmergencia.SelectedTabIndex, new EmergenciaPediatricaService()));
                    gridTabEmergenciaPediatrica.Children.Add(_dictionaryTab[tabEmergencia.SelectedTabIndex]);
                }
                if (tabEmergencia.SelectedTabIndex == (int)TipoTabEmergencia.Obstetrica)
                {
                    _dictionaryTab.Add(tabEmergencia.SelectedTabIndex, new ucPacienteEmergencia(this, (TipoTabEmergencia)tabEmergencia.SelectedTabIndex, new EmergenciaObstetricaService()));
                    gridTabEmergenciaObstetrica.Children.Add(_dictionaryTab[tabEmergencia.SelectedTabIndex]);                    
                }
            }
            else
                _dictionaryTab[tabEmergencia.SelectedTabIndex].Refresh();          
        }

        #endregion

    }
}
