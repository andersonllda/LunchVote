using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.PEP.DTO;
using HMV.PEP.Consult;
using HMV.Core.Domain.Model;
using StructureMap;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmProcedimentosRealizados : ViewModelBase
    {
        #region Construtor
        public vmProcedimentosRealizados(Paciente pPaciente, bool pMostraTitulo = true, bool pIsResumo = false)
        {
            this._mostratitulo = pMostraTitulo;

            ISumarioDeAtendimentosConsult consult = ObjectFactory.GetInstance<ISumarioDeAtendimentosConsult>();

            this._procedimentosrealizados = (from T in consult.carregaProcedimentosRealizados(pPaciente)
                                             orderby T.DataAtendimento descending, T.IdAtendimento descending
                                             select T).ToList();

            if (_procedimentosrealizados.HasItems())
                _procedimentorealizadoselecionado = _procedimentosrealizados.FirstOrDefault();

            _mostracirurgiao = false;
            _mostradataprocedimento = false;

            if (pIsResumo)
            {
                _mostracirurgiao = true;
                _mostradataprocedimento = true;
            }
        }
        #endregion

        #region Propriedades Privadas
        private IList<ProcedimentosRealizadosDTO> _procedimentosrealizados { get; set; }
        private bool _mostratitulo;
        private bool _mostracirurgiao;
        private bool _mostradataprocedimento;
        private ProcedimentosRealizadosDTO _procedimentorealizadoselecionado { get; set; }
        private AvisoCirurgia _DescricaoCirurgica { get; set; }
        #endregion

        #region Propriedades Públicas
        public bool MostraTitulo
        {
            get
            {
                return this._mostratitulo;
            }
        }
        public IList<ProcedimentosRealizadosDTO> ProcedimentosRealizados
        {
            get
            {
                return _procedimentosrealizados;
            }
        }

        public ProcedimentosRealizadosDTO ProcedimentosRealizadoSelecionado
        {
            get
            {
                return _procedimentorealizadoselecionado;
            }
            set
            {
                _procedimentorealizadoselecionado = value;
                OnPropertyChanged<vmProcedimentosRealizados>(x => x.ProcedimentosRealizadoSelecionado);
            }
        }

        public bool MostraCirurgiao
        {
            get
            {
                return this._mostracirurgiao;
            }
        }
        public bool MostraDataProcedimento
        {
            get
            {
                return this._mostradataprocedimento;
            }
        }

        public IList<AvisoCirurgia> DescricoesCirurgicas
        {
            get
            {
                IRepositorioAvisosDeCirurgia rep = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
                rep.OndeCodigoDoAvisoIgual(_procedimentorealizadoselecionado.IdAvisoCirurgia);
                return rep.OndeCodigoAtendimentoIgual(_procedimentorealizadoselecionado.IdAtendimento).List().OrderByDescending(x => x.DataAviso).ToList();
            }
        }

        public AvisoCirurgia DescricaoCirurgicaSelecionada
        {
            get
            {
                if (this._DescricaoCirurgica == null)
                    if (DescricoesCirurgicas.Count() == 0)
                        return null;
                    else
                        return DescricoesCirurgicas[0];
                return this._DescricaoCirurgica;
            }
            set
            {
                this._DescricaoCirurgica = value;
            }
        }
        #endregion

        #region Métodos Privados 

        #endregion

        #region Métodos Públicos
        public void SelectByAviso(int pAviso)
        {
            var prc = _procedimentosrealizados.Where(x => x.IdAvisoCirurgia == pAviso).FirstOrDefault();
            _procedimentorealizadoselecionado = prc;
        }

        public class DescricoesCirurg
        {
            public string Descricao { get; set; }
        }

        public List<DescricoesCirurg> RelDescricoes
        {
            get
            {
                List<DescricoesCirurg> qry = new List<DescricoesCirurg>();

                if (this._DescricaoCirurgica != null && this._DescricaoCirurgica.DescricaoCirurgia != null)
                {
                    var listaDescr = this._DescricaoCirurgica.DescricaoCirurgia.TrimEnd(Environment.NewLine.ToCharArray()).Split(new char[] { '\n' }).ToList();

                    for (int i = 0; i < listaDescr.Count(); i++)
                    {
                        DescricoesCirurg dc = new DescricoesCirurg();
                        if (!string.IsNullOrEmpty(listaDescr[i].ToString()))
                        {
                            dc.Descricao = listaDescr[i].ToString();
                            qry.Add(dc);
                        }
                    }
                }
                return qry;
            }
        }
        #endregion
    }
}
