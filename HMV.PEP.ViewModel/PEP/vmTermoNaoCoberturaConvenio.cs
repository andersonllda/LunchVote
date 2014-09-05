using System;
using System.Linq;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository.SumarioAvaliacaoMedica;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;
using HMV.Core.Domain.Enum;
using System.Collections.Generic;
using HMV.Core.Domain.Repository;
using HMV.PEP.Interfaces;
using DevExpress.Xpf.Core;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmTermoNaoCoberturaConvenio : ViewModelBase
    {
        #region Contrutor

        public vmTermoNaoCoberturaConvenio(Atendimento pAtendimento, Usuarios usuario)
        {
            try
            {
                Atendimento atendimento = pAtendimento;
                ObjectFactory.GetInstance<IRepositorioDeAtendimento>().Refresh(atendimento);

                _listaDTO = new List<TermoNaoCoberturaConvenioDTO>();

                //PrescricaoMedica prescricao = atendimento.PrescricaoMedica.OrderBy(x => x.Id).LastOrDefault();
                IList<PrescricaoMedica> prescricoes = atendimento.PrescricaoMedica.Where(x => x.DataPrescricao > DateTime.Now.AddDays(-2)).ToList();

                foreach (PrescricaoMedica prescricao in prescricoes)
                {
                    IRepositorioDePrescricao repPrescricao = ObjectFactory.GetInstance<IRepositorioDePrescricao>();
                    repPrescricao.Refresh(prescricao);
                    IRepositorioDeItemPrescricao repItens = ObjectFactory.GetInstance<IRepositorioDeItemPrescricao>();
                    IList<ItemPrescricaoMedica> itens = repItens.ondePrescricaoIgual(prescricao).List();

                    if (prescricao.IsNull() || /*!prescricao.ItensPrescricao.HasItems()*/!itens.HasItems())
                    {
                        continue;
                    }

                    // verifica se existe já algum termo para o atendimento e prescricao 
                    IRepositorioDeTermoNaoCoberturaConvenio repTermo = ObjectFactory.GetInstance<IRepositorioDeTermoNaoCoberturaConvenio>();
                    repTermo.OndeAtendimentoIgual(atendimento);
                    repTermo.OndePrescricaoIgual(prescricao);
                    if (repTermo.List().Count > 0)
                    {
                        continue;
                    }

                    //IList<ItemPrescricaoMedica> itens = prescricao.ItensPrescricao;
                    _lista = new List<TermoNaoCoberturaConvenio>();

                    IRepositorioDeGrupoTipoEsquema repGrupo = ObjectFactory.GetInstance<IRepositorioDeGrupoTipoEsquema>();
                    IList<GrupoTipoEsquema> grupos = repGrupo.List();
                    GrupoTipoEsquema controlaGrupo = null;
                    TermoNaoCoberturaConvenio termo = null;

                    IRepositorioDeProcedimento repProcedimento = ObjectFactory.GetInstance<IRepositorioDeProcedimento>();

                    foreach (GrupoTipoEsquema iGrupo in grupos.OrderBy(x => x.Grupo).ToList())
                    {
                        // verifica se tem algum item para o tipo de esquema 
                        //var _itens = itens.Where(x => x.TipoEsquema == iGrupo.TipoEsquema.Id
                        //    && x.DataCancelamento == null && (x.Cancelado == null || x.Cancelado.Value == SimNao.Nao)).ToList(); 

                        var _itens = itens.Where(x => x.TipoEsquema == iGrupo.TipoEsquema.Id
                            && !x.DataCancelamento.HasValue && (!x.Cancelado.HasValue || x.Cancelado.Value == SimNao.Nao)).ToList();

                        if (!_itens.HasItems())
                            continue;

                        foreach (var iItem in _itens)
                        {
                            // localiza a proibicao do procedimento 
                            IRepositorioDeProibicaoConvenio repPro = ObjectFactory.GetInstance<IRepositorioDeProibicaoConvenio>();
                            repPro.OndeConvenioIgual(atendimento.Convenio);
                            repPro.OndePlanoIgual(atendimento.Plano.ID);
                            repPro.OndeTipoAtendimentoIgual(atendimento.TipoDeAtendimento);

                            IRepositorioDeTipoPrescricaoMedica repItem = ObjectFactory.GetInstance<IRepositorioDeTipoPrescricaoMedica>();
                            TipoPrescricaoMedica _item = repItem.OndeIdIgual(iItem.TipoPrescricaoMedica.Id).Single();

                            if (_item.Procedimento != null)
                                repPro.OndeProcedimentoIgual(_item.Procedimento);
                            else if (_item.Produto != null && _item.Produto.Procedimento != null)
                                repPro.OndeProcedimentoIgual(_item.Produto.Procedimento);
                            else if (_item.ExameImagem != null && _item.ExameImagem.Procedimento != null)
                                repPro.OndeProcedimentoIgual(_item.ExameImagem.Procedimento);
                            else
                                continue;

                            ProibicaoConvenio pro = repPro.List().OrderByDescending(x => x.DataProibicao).FirstOrDefault();

                            if (pro.IsNotNull() && (pro.TipoProibicao == "NA" || pro.TipoProibicao == "AG"))
                            {
                                // busca o valor de venda do procedimento sempre para o convenio 150 e plano 1 
                                decimal valor = repProcedimento.ValorDeVenda(pro.Procedimento.ID, 150, 1,
                                    HMV.Core.Framework.Types.Enum<TipoAtendimento>.GetDescriptionOf(atendimento.TipoDeAtendimento));

                                string obs = string.Empty;
                                if (pro.TipoProibicao == "NA")
                                    obs = "NÃO AUTORIZADO";
                                else if (pro.TipoProibicao == "AG")
                                    obs = "PENDENTE DE AUTORIZAÇÃO";

                                // Cria um novo termo para cada grupo                 
                                if (controlaGrupo == null || controlaGrupo.Grupo != iGrupo.Grupo)
                                {
                                    controlaGrupo = iGrupo;
                                    termo = new TermoNaoCoberturaConvenio();
                                    termo.Atendimento = atendimento;
                                    termo.Prescricao = prescricao;
                                    termo.DataEmissao = DateTime.Now;
                                    termo.UsuarioEmissao = usuario;
                                    termo.Itens = new List<ItemTermoNaoCoberturaConvenio>();
                                    _lista.Add(termo);
                                }

                                termo.Itens.Add(new ItemTermoNaoCoberturaConvenio()
                                {
                                    Observacao = obs,
                                    Procedimento = pro.Procedimento,
                                    TipoPrescricaoMedica = iItem.TipoPrescricaoMedica,
                                    Quantidade = iItem.Dose.HasValue ? iItem.Dose.Value : 1,
                                    Termo = termo,
                                    ValorUnitario = valor
                                });
                                termo.ValorTotal += valor;
                                repTermo.Save(termo);
                            }
                        }
                    }

                    _copias = ObjectFactory.GetInstance<IParametroPEPService>().NumeroCopiaTermoDeNaoCoberturaDoConvenio();

                    //// Monta DTO
                    if (_lista.HasItems())
                    {
                        for (int i = 0; i < _copias; i++)
                        {
                            foreach (var iTermo in _lista.Where(x => x.ID > 0 && x.Itens.HasItems()).ToList())
                            {
                                TermoNaoCoberturaConvenioDTO dto = new TermoNaoCoberturaConvenioDTO();
                                dto.Prescricao = iTermo.Prescricao.Id;
                                dto.Atendimento = iTermo.Atendimento.ID;
                                dto.Convenio = iTermo.Atendimento.Convenio.Descricao;
                                dto.DataEmissao = iTermo.DataEmissao.ToShortDateString();
                                dto.NomePaciente = iTermo.Atendimento.Paciente.Nome;
                                dto.Termo = iTermo.ID;
                                dto.Itens = new List<TermoNaoCoberturaConvenioItemDTO>();
                                int seq = 1;

                                foreach (var item in iTermo.Itens)
                                {
                                    dto.Itens.Add(new TermoNaoCoberturaConvenioItemDTO()
                                    {
                                        Codigo = item.Procedimento.ID,
                                        Descricao = seq + "-Cod " + item.Procedimento.ID + " " + item.TipoPrescricaoMedica.Descricao,
                                        Observacao = item.Observacao,
                                        Quantidade = item.Quantidade,
                                        ValorUnitario = item.ValorUnitario,
                                        ValorTotal = item.Quantidade * item.ValorUnitario
                                    });
                                    seq++;
                                }
                                _listaDTO.Add(dto);
                            }

                        }

                    }
                }
            }
            catch (Exception err)
            {
                throw err;
                //DXMessageBox.Show(err.Message);
            }
        }
        #endregion

        #region Propriedades Públicas
        public IList<TermoNaoCoberturaConvenioDTO> DTO
          {
              get {
                  return _listaDTO;
              }
          }
        #endregion

        #region Propriedades Privadas
        private wrpAtendimento _atendimento { get; set; }
        IList<TermoNaoCoberturaConvenioDTO> _listaDTO;
        IList<TermoNaoCoberturaConvenio> _lista;
        int _copias;
        #endregion
    }

    public class TermoNaoCoberturaConvenioDTO
    {
        public string NomePaciente { get; set; }
        public string DataEmissao { get; set; }
        public int Atendimento { get; set; }
        public int Termo { get; set; }
        public string Convenio { get; set; }
        public int Prescricao { get; set; }
        
        public IList<TermoNaoCoberturaConvenioItemDTO> Itens { get; set; }
       
    }

    public class TermoNaoCoberturaConvenioItemDTO
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacao { get; set; }
    }

}
