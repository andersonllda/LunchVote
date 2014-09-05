using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Framework.Extensions;
using HMV.Core.Wrappers.ObjectWrappers;
using System.Linq;
using HMV.Core.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using HMV.Core.Framework.Validations;
using System.Windows;
using DevExpress.Xpf.Core;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia
{
    public class vmSinaisVitaisCO : ViewModelBase
    {
        enum Sinais
        {
            ALT,
            BCF,
            DOR,
            FC,
            FR,
            PA,
            PESO,
            SAT,
            TAX
        }

        #region ----- Construtor -----
        public vmSinaisVitaisCO(vmBoletimEmergenciaCO pVm)
        {
            pVm.DictionaryCO.Add(vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO.SinaisVitais, this);
            _usuario = pVm.Usuarios;
            _boletim = pVm.Boletim;
            _sinaisVitaisDTO = new SinaisVitaisDTO();
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpUsuarios _usuario;
        private SinaisVitaisDTO _sinaisVitaisDTO;
        private wrpBoletimDeEmergencia _boletim;
        private DateTime _data;
        private IList<SinaisVitaisTipo> tiposDeSinais;
        #endregion

        #region ----- Propriedades Públicas -----
        public IList<SinaisVitaisDTO> ListaDeSinaisVitais
        {
            get
            {
                IList<SinaisVitaisDTO> _lista = new List<SinaisVitaisDTO>();

                foreach (var item in _boletim.SinaisVitais.OrderByDescending(x=>x.Data))
	            {
                    SinaisVitaisDTO dto = _lista.FirstOrDefault(x=>x.DataHora == item.Data);

                    if (dto.IsNull())
                    {
                        dto = new SinaisVitaisDTO() { Data = item.Data.ToShortDateString(), Hora = item.HoraInclusao, Usuario = item.Usuario, Id = item.ID };
                        _lista.Add(dto);
                    }
                    if ( item.Sigla.Sigla == Sinais.BCF.ToString() ) 
                        dto.BCF =  double.Parse(item.Valor);
                    if (item.Sigla.Sigla == Sinais.DOR.ToString())
                        dto.DOR = double.Parse(item.Valor);
                    if (item.Sigla.Sigla == Sinais.FC.ToString())
                        dto.FC = double.Parse(item.Valor);
                    if (item.Sigla.Sigla == Sinais.FR.ToString())
                        dto.FR = double.Parse(item.Valor);
                    if (item.Sigla.Sigla == Sinais.TAX.ToString())
                        dto.TAX = double.Parse(item.Valor);
                    if (item.Sigla.Sigla == Sinais.PA.ToString())
                        dto.PA = item.Valor;
	            }

                return _lista;
            }
        }

        public SinaisVitaisDTO SinaisVitaisDTO
        {
            get { return _sinaisVitaisDTO; }
            set {
                _sinaisVitaisDTO = value;
                OnPropertyChanged<vmSinaisVitaisCO>(x => x.SinaisVitaisDTO);
            }
        }
           
        #endregion

        #region ----- Métodos Privados -----
        private bool valida(bool adicionar)
        {
            //if (_boletim.SinaisVitais.Count(x => x.Data == _sinaisVitaisDTO.DataHora) > 0)
            //{
            //    DXMessageBox.Show("Já existe Sinais Vitais para essa data e hora.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Stop); ;
            //    return false;
            //}
                          

            List<string> message = new List<string>();

            if (!string.IsNullOrWhiteSpace(_sinaisVitaisDTO.SinaisVitais) || adicionar)
            {
                if (_boletim.SinaisVitais.Count(x => x.Data == _sinaisVitaisDTO.DataHora) > 0)
                {
                    DXMessageBox.Show("Já existe Sinais Vitais para essa data e hora.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Stop); ;
                    return false;
                }

                if (string.IsNullOrWhiteSpace(_sinaisVitaisDTO.Data))
                    message.Add("Data deve ser informada.");
                if (string.IsNullOrWhiteSpace(_sinaisVitaisDTO.Hora))
                    message.Add("Hora deve ser informada.");
                if (string.IsNullOrWhiteSpace(_sinaisVitaisDTO.PA))
                    message.Add("PA deve ser informado.");
                if (!_sinaisVitaisDTO.TAX.HasValue)
                    message.Add("Tax deve ser informado.");
                if (!_sinaisVitaisDTO.FC.HasValue)
                    message.Add("FC deve ser informado.");
                if (!_sinaisVitaisDTO.FR.HasValue)
                    message.Add("FR deve ser informado.");
                if (!_sinaisVitaisDTO.DOR.HasValue)
                    message.Add("DOR deve ser informado.");
                if (!_sinaisVitaisDTO.BCF.HasValue)
                    message.Add("BCF deve ser informado.");

                if ( !_sinaisVitaisDTO.IsValid)
                    message.Add("Verifique os valores digitados.");
            }

            if (message.Count > 0)
            {
                DXMessageBox.Show(string.Join(Environment.NewLine, message), "Atenção", MessageBoxButton.OK, MessageBoxImage.Stop); ;
                return false;
            }

            return true;
        }
        #endregion

        #region ----- Métodos Públicos -----
        public override bool IsValid
        {
            get
            {
                bool valid = valida(false);
                if (valid)
                {
                    if (!string.IsNullOrWhiteSpace(_sinaisVitaisDTO.SinaisVitais))
                        CommandSelecionar(null);
                    return true;
                }

                return false;
            }
        }
       
        #endregion

        #region ----- Commands -----
        private void AddSinais(Sinais tipo, String valor)
        {
            var wrp = new wrpSinaisVitais
                {   Usuario = _usuario
                ,   DataInclusao = _data.Date
                ,   HoraInclusao = _data.ToString("HH:mm")
                ,   Sigla = new wrpSinaisVitaisTipo(tiposDeSinais.First(x => x.Sigla == tipo.ToString()))
                ,   Valor = valor.ToString()
                };

            wrp.Save();
            _boletim.SinaisVitais.Add(wrp);
        }

        protected override void CommandSelecionar(object param)
        {
            if (!valida(true))
                return;

            tiposDeSinais = ObjectFactory.GetInstance<IRepositorioSinaisVitaisTipo>().List();
            _data = Convert.ToDateTime(_sinaisVitaisDTO.DataHora);

            if ( _sinaisVitaisDTO.BCF.HasValue ) 
                AddSinais(Sinais.BCF, _sinaisVitaisDTO.BCF.ToString());
            if (_sinaisVitaisDTO.DOR.HasValue)
                AddSinais(Sinais.DOR, _sinaisVitaisDTO.DOR.ToString());
            if (_sinaisVitaisDTO.FC.HasValue)
                AddSinais(Sinais.FC, _sinaisVitaisDTO.FC.ToString());
            if (_sinaisVitaisDTO.FR.HasValue)
                AddSinais(Sinais.FR, _sinaisVitaisDTO.FR.ToString());
            if (!string.IsNullOrEmpty(_sinaisVitaisDTO.PA))
                AddSinais(Sinais.PA, _sinaisVitaisDTO.PA.ToString());
            if (_sinaisVitaisDTO.TAX.HasValue)
                AddSinais(Sinais.TAX, _sinaisVitaisDTO.TAX.ToString());

            _boletim.Save();
            
            _sinaisVitaisDTO = new SinaisVitaisDTO();

            OnPropertyChanged<vmSinaisVitaisCO>(x => x.SinaisVitaisDTO);
            OnPropertyChanged<vmSinaisVitaisCO>(x => x.ListaDeSinaisVitais);
        }
        #endregion
    }

    public class SinaisVitaisDTO : ValidationViewModelBase
    {
        public SinaisVitaisDTO()
        {
            Data = DateTime.Now.ToString("dd/MM/yyyy");
            Hora = DateTime.Now.ToString("HH:mm");
        }

        public int Id { get; set; }
        private string _pa;
        public wrpUsuarios Usuario { get; set; }
        public DateTime DataHora { get { return DateTime.Parse(Data + " " + Hora); } }
        public string DataHoraNaGrid { get { return (Data + " " + Hora); } }
        public bool Valida { get { return Id == 0; } }

        [ValidaMaximoEMinimo(0, 300, "Valida")]
        [ValidaCampoObrigatorio("Valida")]        
        public Double? PAAlta
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_pa) )
                {
                    string[] lp = _pa.Replace('X', '/').Replace('x', '/').Split('/');
                    if (lp.Length > 0)
                    {
                        int alta;
                        if (int.TryParse(lp[0].Replace("_", ""), out alta))
                            return alta;
                    }
                }
                return null;
            } 
            set {
                _pa = value.ToString() + "/" + PABaixa.ToString(); ;
            } 
        }
        [ValidaCampoObrigatorio("Valida")]
        [ValidaMaximoEMinimo(0, 300, "Valida")]
        public Double? PABaixa
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_pa))
                {
                    string[] lp = _pa.Replace('X', '/').Replace('x', '/').Split('/');
                    if (lp.Length > 1)
                    {
                        int baixa;
                        if (int.TryParse(lp[1].Replace("_", ""), out baixa))
                            return baixa;
                    }
                }
                return null;
            }
            set
            {
                _pa = PAAlta.ToString() + "/" + value.ToString(); 
            }
        }
        [ValidaMaximoEMinimo(34, 42, "Valida")]
        [ValidaCampoObrigatorio("Valida")]
        public Double? TAX { get; set; }
        [ValidaMaximoEMinimo(30, 300, "Valida")]
        [ValidaCampoObrigatorio("Valida")]
        public Double? FC { get; set; }
        [ValidaMaximoEMinimo(5, 100, "Valida")]
        [ValidaCampoObrigatorio("Valida")]
        public Double? FR { get; set; }
        [ValidaCampoObrigatorio("Valida")]
        [ValidaMaximoEMinimo(0, 10, "Valida")]
        public Double? DOR { get; set; }
        [ValidaMaximoEMinimo(0, 300, "Valida")]
        [ValidaCampoObrigatorio("Valida")]
        public Double? BCF { get; set; }
        [ValidaCampoObrigatorio("Valida")]
        public string Hora { get; set; }
        [ValidaCampoObrigatorio("Valida")]
        public string Data { get; set; }

        public string PA
        {
            get
            {
                if (this.PAAlta.HasValue || this.PABaixa.HasValue)
                    return this.PAAlta.ToString() + "/" + this.PABaixa.ToString();
                return string.Empty;
            }
            set
            {
                _pa = value;
            }
        }
                    
        public bool IsNurse
        {
            get { return Usuario.Prestador.IsNurse; }
        }

        public string SinaisVitais
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrEmpty(PA))
                    sb.Append("Pa: " + PA + " mmHg   ");
                if (TAX.HasValue)
                    sb.Append("Tax:" + TAX + " ºC   ");
                if (FC.HasValue)
                    sb.Append("FC:" + FC + " bpm   ");
                if (FR.HasValue)
                    sb.Append("FR:" + FR + " mpm   ");
                if (DOR.HasValue)
                    sb.Append("DOR:" + DOR + "   ");
                if (BCF.HasValue)
                    sb.Append("BCF:" + BCF + " bpm   ");

                return sb.ToString();
            }
        }  
    }

}
