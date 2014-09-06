using System.Linq;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.ViewModelBaseClasses;
using StructureMap;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmTransferencia : ViewModelBase
    {
        #region Contrutor
        public vmTransferencia(wrpSumarioAlta pSumarioAlta)
        {
            this.SumarioAlta = pSumarioAlta;
            this.Transferencia = pSumarioAlta.Transferencia;

            //Meio de Transporte
            IRepositorioDeMeioDeTransporte rep = ObjectFactory.GetInstance<IRepositorioDeMeioDeTransporte>();
            this._meiosdetransporte = new wrpMeioDeTransporteCollection(rep.List().OrderBy(x => x.Descricao).ToList());
            if (_Transferencia.MeioDetransporte.IsNotNull())
                this._meiodetransporteselecionado = this._meiosdetransporte.Where(x => x.Id == _Transferencia.MeioDetransporte.Id).FirstOrDefault();

            //Países
            IRepositorioDePais repP = ObjectFactory.GetInstance<IRepositorioDePais>();
            this._paises = new wrpPaisCollection(repP.List().OrderBy(x => x.Descricao).ToList());
            this._paisselecionado = this._paises.Where(x => x.Id == 1).Single();

            this.CarregaEstado();
            if (_Transferencia.Municipio.IsNotNull())
            {
                if (string.IsNullOrEmpty(_Transferencia.Municipio.Descricao))
                {
                    this._estadoselecionado = this._estados.Where(x => x.Id == "RS").Single();
                }
                else
                {
                    this._estadoselecionado = this._estados.Where(x => x.Id == _Transferencia.Municipio.Estado.Id).SingleOrDefault();
                }
            }
            else
                this._estadoselecionado = this._estados.Where(x => x.Id == "RS").Single();


            this.CarregaCidades();
            if (_Transferencia.Municipio.IsNotNull())
            {
                if (string.IsNullOrEmpty(_Transferencia.Municipio.Descricao))
                {
                    _Transferencia.Municipio = this._cidades.Where(c => c.Descricao == "PORTO ALEGRE").Single();
                    this._cidadeselecionada = this._cidades.Where(c => c.Descricao == "PORTO ALEGRE").Single();
                }
                else
                    this._cidadeselecionada = this._cidades.Where(x => x.Id == _Transferencia.Municipio.Id).SingleOrDefault();
            }
            else
            {
                _Transferencia.Municipio = this._cidades.Where(c => c.Descricao == "PORTO ALEGRE").Single();
                this._cidadeselecionada = this._cidades.Where(c => c.Descricao == "PORTO ALEGRE").Single();
            }
        }
        #endregion

        #region Metodos Privados
        private void CarregaEstado()
        {
            //Estados
            IRepositorioDeCidade repE = ObjectFactory.GetInstance<IRepositorioDeCidade>();                    
            this._estados = new wrpUFCollection(repE.List().Where(x => x.Estado.Pais.Id == this._paisselecionado.Id)
                                                .Select(x => x.Estado).DistinctBy(x => x.Descricao).OrderBy(x=> x.Descricao).ToList());

            this.OnPropertyChanged("Estados");
        }
        private void CarregaCidades()
        {
            //Cidades
            if (this._estadoselecionado == null)
                this._cidades = null;
            IRepositorioDeCidade rep = ObjectFactory.GetInstance<IRepositorioDeCidade>();
            rep.OndeIdUfIgual(this._estadoselecionado.Id);
            this._cidades = new wrpCidadeCollection(rep.List().OrderBy(x => x.Descricao).ToList());

            this.OnPropertyChanged("Cidades");
        }
        #endregion

        #region Propriedades Publicas
        public wrpSumarioAlta SumarioAlta { get; set; }

        public wrpTransferencia Transferencia
        {
            get
            {
                return this._Transferencia;
            }
            set
            {
                this._Transferencia = value;
                this.OnPropertyChanged("Transferencia");
            }
        }

        public int? PressaoAlta
        {
            get
            {
                if (this.Transferencia.PressaoArterial.Alta.HasValue)
                    return this.Transferencia.PressaoArterial.Alta.Value;
                return null;
            }
            set
            {
                if (value.HasValue)
                    if (this.PressaoBaixa.HasValue)
                        this.Transferencia.PressaoArterial = new wrpPressaoArterial(value.Value, this.PressaoBaixa.Value);
                    else
                        this.Transferencia.PressaoArterial = new wrpPressaoArterial(value.Value, 0);

                this.OnPropertyChanged("PressaoAlta");
            }
        }

        public int? PressaoBaixa
        {
            get
            {
                if (this.Transferencia.PressaoArterial.Baixa.HasValue)
                    return this.Transferencia.PressaoArterial.Baixa.Value;
                return null;
            }
            set
            {
                if (value.HasValue)
                    if (this.PressaoAlta.HasValue)
                        this.Transferencia.PressaoArterial = new wrpPressaoArterial(this.PressaoAlta.Value, value.Value);
                    else
                        this.Transferencia.PressaoArterial = new wrpPressaoArterial(0, value.Value);

                this.OnPropertyChanged("PressaoBaixa");
            }
        }

        public wrpMeioDeTransporteCollection MeiosDeTransportes
        {
            get
            {
                return _meiosdetransporte;
            }
        }

        public wrpMeioDeTransporte MeioDeTransporteSelecionado
        {
            get { return _meiodetransporteselecionado; }
            set
            {
                _meiodetransporteselecionado = value;
                this._Transferencia.MeioDetransporte = _meiodetransporteselecionado;
                this.OnPropertyChanged("MeioDeTransporteSelecionado");
            }
        }

        public wrpPaisCollection Paises
        {
            get
            {                
                return this._paises;
            }
        }

        public wrpPais PaisSelecionado
        {
            get
            {
                if (this._paisselecionado == null)
                {
                    //comentado por enquanto não precisa dos paises, BRASIL vai ser o default

                    //if (_Transferencia.Municipio != null)
                    //{
                    //    if (string.IsNullOrEmpty(_Transferencia.Municipio.Descricao))
                    this._paisselecionado = this.Paises.Where(x => x.Id == 1).Single();
                    //    else 
                    //        this._PaisSelecionado = _Transferencia.Municipio.Estado.Pais;
                    //}
                    //else
                    //    this._PaisSelecionado = this.Paises.Where(x => x.Descricao == "BRA").Single();
                }
                return this._paisselecionado;
            }
            set
            {
                this._paisselecionado = value;
                this.CarregaEstado();
                this.OnPropertyChanged("PaisSelecionado");
                this.OnPropertyChanged("EstadoSelecionado");
            }
        }

        public wrpUFCollection Estados
        {
            get
            {
                return _estados;
            }
        }

        public wrpUF EstadoSelecionado
        {
            get
            {
                return this._estadoselecionado;
            }
            set
            {
                this._estadoselecionado = value;
                this.CarregaCidades();
                this._cidadeselecionada = this.Cidades.FirstOrDefault();
                this._Transferencia.Municipio = this.Cidades.FirstOrDefault();
                this.OnPropertyChanged("CidadeSelecionada");
                this.OnPropertyChanged("EstadoSelecionado");
            }
        }

        public wrpCidadeCollection Cidades
        {
            get
            {
                return _cidades;
            }
        }

        public wrpCidade CidadeSelecionada
        {
            get
            {
                return _cidadeselecionada;
            }
            set
            {
                this._Transferencia.Municipio = value;
                this._cidadeselecionada = value;
                this.OnPropertyChanged("CidadeSelecionada");
            }
        }
        #endregion

        #region Propriedades Privadas
        wrpTransferencia _Transferencia;
        wrpMeioDeTransporteCollection _meiosdetransporte;
        wrpMeioDeTransporte _meiodetransporteselecionado;
        wrpPaisCollection _paises;
        wrpPais _paisselecionado;
        wrpUFCollection _estados;
        wrpUF _estadoselecionado;
        wrpCidadeCollection _cidades;
        wrpCidade _cidadeselecionada;
        #endregion
    }
}
