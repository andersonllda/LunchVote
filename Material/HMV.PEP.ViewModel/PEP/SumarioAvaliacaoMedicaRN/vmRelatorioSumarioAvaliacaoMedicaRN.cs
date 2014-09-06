using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Enum.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaRN
{
    public class vmRelatorioSumarioAvaliacaoMedicaRN : ViewModelBase//IDisposable
    {
        public vmRelatorioSumarioAvaliacaoMedicaRN(vmSumarioAvaliacaoMedicaRN pvmSumarioAvaliacaoMedicaRN)
        {
            _vm = pvmSumarioAvaliacaoMedicaRN;
        }

        #region Relatório

        private List<SumarioAvaliacaoMedicaRN> _listaRelatorio = new List<SumarioAvaliacaoMedicaRN>();
        private vmSumarioAvaliacaoMedicaRN _vm;

        public List<SumarioAvaliacaoMedicaRN> Relatorio()
        {
            SumarioAvaliacaoMedicaRN sumario = new SumarioAvaliacaoMedicaRN();

            sumario.Nome = this._vm.SumarioAvaliacaoMedicaRN.Paciente.Nome;
            sumario.Prontuario = this._vm.SumarioAvaliacaoMedicaRN.Paciente.ID.ToString();
            sumario.Atendimento = this._vm.SumarioAvaliacaoMedicaRN.Atendimento.ID.ToString();
            sumario.DataAtendimento = this._vm.SumarioAvaliacaoMedicaRN.Atendimento.DataAtendimento.ToShortDateString();
            sumario.Sexo = this._vm.SumarioAvaliacaoMedicaRN.Paciente.Sexo.ToString();
            sumario.Cor = this._vm.SumarioAvaliacaoMedicaRN.Paciente.Cor.ToString();

            //if (this._vm.SumarioAvaliacaoMedicaRN.Paciente.Idade.IsNotNull())
            //    sumario.Idade = this._vm.SumarioAvaliacaoMedicaRN.Paciente.Idade.ToString(2);

            // Rodapé
            _listaRodapeRNSource = new List<RodapeSumarioAvaliacaoMedicaRN>();
            RodapeSumarioAvaliacaoMedicaRN rodape = new RodapeSumarioAvaliacaoMedicaRN();
            rodape.MostraCodigoBarras = false;
            rodape.MostraIDPaciente = false;

            if (this._vm.SumarioAvaliacaoMedicaRN.Atendimento.IsNotNull())
            {
                if (this._vm.SumarioAvaliacaoMedicaRN.Paciente.IsNotNull())
                {
                    rodape.NomePaciente = this._vm.SumarioAvaliacaoMedicaRN.Paciente.Nome;
                    rodape.IDPaciente = this._vm.SumarioAvaliacaoMedicaRN.Paciente.ID;
                    rodape.MostraIDPaciente = true;
                }

                rodape.NomeResumo = this._vm.SumarioAvaliacaoMedicaRN.Atendimento.Leito.IsNotNull() ? this._vm.SumarioAvaliacaoMedicaRN.Atendimento.Leito.Descricao : string.Empty;
                rodape.CodigoBarras = this._vm.SumarioAvaliacaoMedicaRN.Atendimento.ID.ToString();
                rodape.MostraCodigoBarras = true;

                if (this._vm.SumarioAvaliacaoMedicaRN.Atendimento.Prestador.IsNotNull())
                {
                    rodape.NomePrestador = this._vm.SumarioAvaliacaoMedicaRN.Atendimento.Prestador.Nome;
                    rodape.Registro = this._vm.SumarioAvaliacaoMedicaRN.Atendimento.Prestador.Registro;
                }
            }

            _listaRodapeRNSource.Add(rodape);
            //
            // Assinatura
            AssinaturaRN assinatura = new AssinaturaRN();
            _listaAssinaturaRNSource = new List<AssinaturaRN>();

            if (this._vm.SumarioAvaliacaoMedicaRN.DataEncerramento.IsNotNull())
            {
                assinatura.Assinatura = this._vm.SumarioAvaliacaoMedicaRN.Usuario.AssinaturaNaLinhaSemColchetes;
                assinatura.DataEncerramento = this._vm.SumarioAvaliacaoMedicaRN.DataEncerramento.Value.ToShortDateString();
                assinatura.DataEncerramento += " " + this._vm.SumarioAvaliacaoMedicaRN.DataEncerramento.Value.ToShortTimeString();
            }
            else
            {
                assinatura.Assinatura = this._vm.SumarioAvaliacaoMedicaRN.Usuario.AssinaturaNaLinhaSemColchetes;
            }
            _listaAssinaturaRNSource.Add(assinatura);

            // Sumário Obstétrico
            _listaSumarioObstetricoRNSource = new List<SumarioObstetricoRN>();
            _listaSumarioObstetricoRNSource = SumarioObstetricoRN(this._vm);

            // Atendimento ao Recém Nascido em sala de parto
            _listaAtendimentoRecemNascidoRNSource = new List<AtendimentoRecemNascidoRN>();
            _listaAtendimentoRecemNascidoRNSource = AtendimentoRecemNascidoRN(_vm);

            // Atendimento ao Recém Nascido em sala de parto
            _listaExameClinicoRNSource = new List<ExameClinicoRN>();
            _listaExameClinicoRNSource = ExameClinicoRN(_vm);

            _listaRelatorio.Add(sumario);

            return this._listaRelatorio;
        }

        #endregion Relatório

        #region Propriedades Públicas

        public List<RodapeSumarioAvaliacaoMedicaRN> _listaRodapeRNSource { get; set; }
        public List<AssinaturaRN> _listaAssinaturaRNSource { get; set; }
        public List<SumarioObstetricoRN> _listaSumarioObstetricoRNSource { get; set; }
        public List<AtendimentoRecemNascidoRN> _listaAtendimentoRecemNascidoRNSource { get; set; }
        public List<ExameClinicoRN> _listaExameClinicoRNSource { get; set; }

        #endregion Propriedades Públicas

        #region Propriedades Privadas

        #endregion Fim Propriedades Privadas

        #region Métodos

        private List<SumarioObstetricoRN> SumarioObstetricoRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<SumarioObstetricoRN> lista = new List<SumarioObstetricoRN>();
            SumarioObstetricoRN sumarioObstetrico = new SumarioObstetricoRN();
            sumarioObstetrico.MostraSumarioObstetricoRN = false;

            sumarioObstetrico.listaIdentificacaoGestacoesAnterioresRN = IdentificacaoGestacoesAnterioresRN(pvm);
            if (sumarioObstetrico.listaIdentificacaoGestacoesAnterioresRN.FirstOrDefault().MostraIdentificacaoGestacoesAnterioresRN)
                sumarioObstetrico.MostraSumarioObstetricoRN = true;

            sumarioObstetrico.listaGestacaoAtualRN = GestacaoAtualRN(pvm);
            if (sumarioObstetrico.listaGestacaoAtualRN.FirstOrDefault().MostraGestacaoAtualRN)
                sumarioObstetrico.MostraSumarioObstetricoRN = true;

            sumarioObstetrico.listaSorologiaExamesRN = SorologiaExamesRN(pvm);
            if (sumarioObstetrico.listaSorologiaExamesRN.FirstOrDefault().MostraSorologiaExamesRN)
                sumarioObstetrico.MostraSumarioObstetricoRN = true;

            sumarioObstetrico.listaPartoRN = PartoRN(pvm);
            if (sumarioObstetrico.listaPartoRN.FirstOrDefault().MostraParto)
                sumarioObstetrico.MostraSumarioObstetricoRN = true;

            lista.Add(sumarioObstetrico);

            return lista;
        }

        private List<IdentificacaoGestacoesAnterioresRN> IdentificacaoGestacoesAnterioresRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<IdentificacaoGestacoesAnterioresRN> lista = new List<IdentificacaoGestacoesAnterioresRN>();
            IdentificacaoGestacoesAnterioresRN gestacao = new IdentificacaoGestacoesAnterioresRN();
            gestacao.MostraIdentificacaoGestacoesAnterioresRN = false;

            if (pvm.IsNotNull())
            {
                if (pvm.vmSumarioObstetrico.IsNotNull())
                {
                    gestacao.listaIdentificacaoRN = IdentificacaoRN(pvm.vmSumarioObstetrico);
                    if (gestacao.listaIdentificacaoRN.FirstOrDefault().MostraIdentificacao)
                        gestacao.MostraIdentificacaoGestacoesAnterioresRN = true;
                }

                gestacao.listaAnomaliasEmGestacoesAnterioresRN = this.AnomaliasEmGestacoesAnterioresRN(pvm);
                if (gestacao.listaAnomaliasEmGestacoesAnterioresRN.FirstOrDefault().MostraAnomaliasEmGestacoesAnterioresRN)
                    gestacao.MostraIdentificacaoGestacoesAnterioresRN = true;
            }

            lista.Add(gestacao);

            return lista;
        }

        private List<IdentificacaoRN> IdentificacaoRN(vmSumarioObstetrico pvm)
        {
            List<IdentificacaoRN> lista = new List<IdentificacaoRN>();
            IdentificacaoRN identificacao = new IdentificacaoRN();
            identificacao.MostraIdentificacao = false;
            identificacao.MostraNomeIdadeECor = false;
            identificacao.MostraObstetra = false;

            if (pvm.NomeMae.ConvertNullToStringEmpty().IsNotEmpty())
                identificacao.NomeIdadeECor = "Nome: " + pvm.NomeMae;

            if (pvm.IdadeMae.ConvertNullToStringEmpty().IsNotEmpty())
                if (identificacao.NomeIdadeECor.IsNotEmpty())
                    identificacao.NomeIdadeECor += "     Idade: " + pvm.IdadeMae;
                else
                    identificacao.NomeIdadeECor = "Idade: " + pvm.IdadeMae;

            if (pvm.CorMae.ConvertNullToStringEmpty().IsNotEmpty())
                if (identificacao.NomeIdadeECor.IsNotEmpty())
                    identificacao.NomeIdadeECor += "     Cor: " + pvm.CorMae;
                else
                    identificacao.NomeIdadeECor = "Cor: " + pvm.CorMae;

            if (identificacao.NomeIdadeECor.IsNotEmpty())
            {
                identificacao.MostraIdentificacao = true;
                identificacao.MostraNomeIdadeECor = true;
            }

            if (pvm.MedicoMae.ConvertNullToStringEmpty().IsNotEmpty())
            {
                identificacao.Obstetra += "Obstetra: " + pvm.MedicoMae;
                identificacao.MostraIdentificacao = true;
                identificacao.MostraObstetra = true;
            }

            lista.Add(identificacao);

            return lista;
        }

        private List<AnomaliasEmGestacoesAnterioresRN> AnomaliasEmGestacoesAnterioresRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<AnomaliasEmGestacoesAnterioresRN> lista = new List<AnomaliasEmGestacoesAnterioresRN>();
            AnomaliasEmGestacoesAnterioresRN gestacao = new AnomaliasEmGestacoesAnterioresRN();
            gestacao.MostraAnomaliasEmGestacoesAnterioresRN = false;
            gestacao.MostraSemIntercorrencias = false;
            gestacao.MostraPrimeiraGestacao = false;

            gestacao.listaAnomaliasEmGestacoesAnterioresItemRN = this.AnomaliasEmGestacoesAnterioresItemRN(pvm);
            if (gestacao.listaAnomaliasEmGestacoesAnterioresItemRN.FirstOrDefault().isSemIntercorrencias)
            {
                gestacao.SemIntercorrencias = "Sem Intercorrências";
                gestacao.MostraAnomaliasEmGestacoesAnterioresRN = true;
                gestacao.MostraSemIntercorrencias = true;
                gestacao.MostraPrimeiraGestacao = false;
            }
            else if (gestacao.listaAnomaliasEmGestacoesAnterioresItemRN.FirstOrDefault().isPrimeiraGestacao)
            {
                gestacao.PrimeiraGestacao = "Primeira Gestação";
                gestacao.MostraAnomaliasEmGestacoesAnterioresRN = true;
                gestacao.MostraSemIntercorrencias = false;
                gestacao.MostraPrimeiraGestacao = true;
            }
            else if (gestacao.listaAnomaliasEmGestacoesAnterioresItemRN.FirstOrDefault().MostraAnomaliasEmGestacoesAnterioresItemRN)
                gestacao.MostraAnomaliasEmGestacoesAnterioresRN = true;

            // Outros
            gestacao.listaOutrosRN = AnomaliasGestacoesAnterioresOutrosRN(pvm.vmSumarioObstetrico);
            if (gestacao.listaOutrosRN.FirstOrDefault().MostraOutros)
                gestacao.MostraAnomaliasEmGestacoesAnterioresRN = true;

            lista.Add(gestacao);

            return lista;
        }

        private List<AnomaliasEmGestacoesAnterioresItemRN> AnomaliasEmGestacoesAnterioresItemRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<AnomaliasEmGestacoesAnterioresItemRN> lista = new List<AnomaliasEmGestacoesAnterioresItemRN>();
            AnomaliasEmGestacoesAnterioresItemRN gestacao;

            if (pvm.vmSumarioObstetrico.IsGestacaoAnterior.IsNotNull())
            {
                if (pvm.vmSumarioObstetrico.IsGestacaoAnterior == SimNao.Sim)
                {
                    gestacao = new AnomaliasEmGestacoesAnterioresItemRN();
                    gestacao.MostraAnomaliasEmGestacoesAnterioresItemRN = false;
                    gestacao.isSemIntercorrencias = true;
                    gestacao.isPrimeiraGestacao = false;
                    lista.Add(gestacao);
                }
            }

            if (pvm.vmSumarioObstetrico.IsPrimeiraGestacao)
            {
                gestacao = new AnomaliasEmGestacoesAnterioresItemRN();
                gestacao.MostraAnomaliasEmGestacoesAnterioresItemRN = false;
                gestacao.isSemIntercorrencias = false;
                gestacao.isPrimeiraGestacao = true;
                lista.Add(gestacao);
            }

            foreach (var item in pvm.vmSumarioObstetrico.CollectionGestacaoAnterior)
            {
                if (item.Selecionado)
                {
                    if (item.ItemCO.GestacaoAnterior == SimNao.Sim)
                    {
                        gestacao = new AnomaliasEmGestacoesAnterioresItemRN();
                        gestacao.Descricao = item.ItemCO.Descricao;
                        gestacao.Observacoes = item.Observacao;
                        gestacao.MostraAnomaliasEmGestacoesAnterioresItemRN = true;
                        gestacao.isSemIntercorrencias = false;
                        gestacao.isPrimeiraGestacao = false;
                        lista.Add(gestacao);
                    }
                }
            }

            if (lista.Count == 0)
            {
                gestacao = new AnomaliasEmGestacoesAnterioresItemRN();
                gestacao.MostraAnomaliasEmGestacoesAnterioresItemRN = false;
                gestacao.isSemIntercorrencias = false;
                gestacao.isPrimeiraGestacao = false;
                lista.Add(gestacao);
            }

            return lista;
        }

        private List<AnomaliasGestacoesAnterioresOutrosRN> AnomaliasGestacoesAnterioresOutrosRN(vmSumarioObstetrico pvm)
        {
            List<AnomaliasGestacoesAnterioresOutrosRN> lista = new List<AnomaliasGestacoesAnterioresOutrosRN>();
            AnomaliasGestacoesAnterioresOutrosRN gestacao = new AnomaliasGestacoesAnterioresOutrosRN();
            gestacao.MostraOutros = false;

            if (pvm.SumarioAvaliacaoMedicaRN.GestacaoAnteriorObservacao.ConvertNullToStringEmpty().IsNotEmpty())
            {
                gestacao.Outros = pvm.SumarioAvaliacaoMedicaRN.GestacaoAnteriorObservacao;
                gestacao.MostraOutros = true;
            }

            lista.Add(gestacao);

            return lista;
        }

        private List<GestacaoAtualRN> GestacaoAtualRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<GestacaoAtualRN> lista = new List<GestacaoAtualRN>();
            GestacaoAtualRN gestacao = new GestacaoAtualRN();
            gestacao.MostraGestacaoAtualRN = false;
            gestacao.MostraGesta = false;
            gestacao.MostraMedicacoes = false;
            gestacao.MostraIdadeGestacional = false;

            // Gesta
            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsNotNull())
            {
                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Gesta.IsNotNull())
                    gestacao.Gesta = "Gesta: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Gesta.ToString();

                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Para.IsNotNull())
                    gestacao.Gesta += "    Para: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Para.ToString();

                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Cesarea.IsNotNull())
                    gestacao.Gesta += "    Cesárea: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Cesarea.ToString();

                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Aborto.IsNotNull())
                    gestacao.Gesta += "    Aborto: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Aborto.ToString();

                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Ectopica.IsNotNull())
                    gestacao.Gesta += "    Ectópica: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Ectopica.ToString();

                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.TipagemPaciente.IsNotNull())
                {
                    gestacao.Gesta += "    Tipagem Sanguínea Materna: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.TipagemPaciente.Value.GetEnumCustomDisplay();

                    if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.RHPaciente.IsNotNull())
                        gestacao.Gesta += pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.RHPaciente.Value.GetEnumDescription();
                }

                if (gestacao.Gesta.IsNotEmpty())
                {
                    gestacao.MostraGestacaoAtualRN = true;
                    gestacao.MostraGesta = true;
                }
            }
            //

            // Medicações
            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsMedicacao.IsNotNull())
            {
                gestacao.Medicacoes = "Medicações: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsMedicacao.Value.GetEnumCustomDisplay();

                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.MedicacaoObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                    gestacao.Medicacoes = "Medicações: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.MedicacaoObservacao;

                gestacao.MostraGestacaoAtualRN = true;
                gestacao.MostraMedicacoes = true;
            }
            //

            // Idade Gestacional
            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsIdadeDesconhecido.IsNotNull())
                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsIdadeDesconhecido == SimNao.Sim)
                    gestacao.IdadeGestacional = "Idade Gestacional: Desconhecida";

            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IdadeSemanas.IsNotNull())
                gestacao.IdadeGestacional = "Idade Gestacional: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IdadeSemanas + " semana(s)";

            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IdadeDias.IsNotNull())
                if (gestacao.IdadeGestacional.IsNotEmpty())
                    gestacao.IdadeGestacional += " e " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IdadeDias + " dia(s)";
                else
                    gestacao.IdadeGestacional = "Idade Gestacional: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IdadeDias + " dia(s)";

            if (gestacao.IdadeGestacional.IsNotEmpty())
            {
                gestacao.MostraGestacaoAtualRN = true;
                gestacao.MostraIdadeGestacional = true;
            }
            //

            // Patologias na Gravidez
            gestacao.listaPatologiasNaGravidezRN = this.PatologiasNaGravidezRN(pvm);
            if (gestacao.listaPatologiasNaGravidezRN.FirstOrDefault().MostraPatologiasNaGravidez)
                gestacao.MostraGestacaoAtualRN = true;
            //

            lista.Add(gestacao);

            return lista;
        }

        private List<PatologiasNaGravidezRN> PatologiasNaGravidezRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<PatologiasNaGravidezRN> lista = new List<PatologiasNaGravidezRN>();
            PatologiasNaGravidezRN patologia = new PatologiasNaGravidezRN();
            patologia.MostraPatologiasNaGravidez = false;
            patologia.MostraNenhuma = false;

            if (pvm.SumarioAvaliacaoMedicaRN.IsPatologia.IsNotNull())
                if (pvm.SumarioAvaliacaoMedicaRN.IsPatologia == SimNao.Sim)
                {
                    patologia.Nenhuma = "Nenhuma";
                    patologia.MostraPatologiasNaGravidez = true;
                    patologia.MostraNenhuma = true;
                }

            patologia.listaPatologiasNaGravidezItem = this.PatologiasNaGravidezItemRN(pvm);
            if (patologia.listaPatologiasNaGravidezItem.FirstOrDefault().MostraPatologiasNaGravidezItemRN)
                patologia.MostraPatologiasNaGravidez = true;

            patologia.listaOutrosObservacoes = PatologiasNaGravidezOutrosObservacoes(pvm);
            if (patologia.listaOutrosObservacoes.FirstOrDefault().MostraOutros)
                patologia.MostraPatologiasNaGravidez = true;

            lista.Add(patologia);

            return lista;
        }

        private List<PatologiasNaGravidezItemRN> PatologiasNaGravidezItemRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<PatologiasNaGravidezItemRN> lista = new List<PatologiasNaGravidezItemRN>();
            PatologiasNaGravidezItemRN patologia;

            foreach (var item in pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNItens)
            {
                if (item.ItemCO.Patologia == SimNao.Sim)
                {
                    patologia = new PatologiasNaGravidezItemRN();
                    patologia.Descricao = item.ItemCO.Descricao;
                    patologia.Observacoes = item.Observacoes;
                    patologia.MostraPatologiasNaGravidezItemRN = true;
                    lista.Add(patologia);
                }
            }

            if (lista.Count == 0)
            {
                patologia = new PatologiasNaGravidezItemRN();
                patologia.MostraPatologiasNaGravidezItemRN = false;
                lista.Add(patologia);
            }

            return lista;
        }

        private List<PatologiasNaGravidezOutrosObservacoes> PatologiasNaGravidezOutrosObservacoes(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<PatologiasNaGravidezOutrosObservacoes> lista = new List<PatologiasNaGravidezOutrosObservacoes>();
            PatologiasNaGravidezOutrosObservacoes patologia = new PatologiasNaGravidezOutrosObservacoes();
            patologia.MostraOutros = false;

            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.PatologiaObservacao.ConvertNullToStringEmpty().IsNotEmpty())
            {
                patologia.Outros = pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.PatologiaObservacao;
                patologia.MostraOutros = true;
            }

            lista.Add(patologia);

            return lista;
        }

        private List<SorologiaExamesRN> SorologiaExamesRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<SorologiaExamesRN> lista = new List<SorologiaExamesRN>();
            SorologiaExamesRN exame = new SorologiaExamesRN();
            exame.MostraSorologiaExamesRN = false;

            exame.listaSorologiaExamesItemRN = this.SorologiaExamesItemRN(pvm);
            if (exame.listaSorologiaExamesItemRN.FirstOrDefault().MostraSorologiaExamesItemRN)
                exame.MostraSorologiaExamesRN = true;

            exame.listaSorologiaExamesOutrosObservacoesRN = this.SorologiaExamesOutrosObservacoesRN(pvm);
            if (exame.listaSorologiaExamesOutrosObservacoesRN.FirstOrDefault().MostraOutrosObservacoes)
                exame.MostraSorologiaExamesRN = true;

            lista.Add(exame);

            return lista;
        }

        private List<SorologiaExamesOutrosObservacoesRN> SorologiaExamesOutrosObservacoesRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<SorologiaExamesOutrosObservacoesRN> lista = new List<SorologiaExamesOutrosObservacoesRN>();
            SorologiaExamesOutrosObservacoesRN outrosObs = new SorologiaExamesOutrosObservacoesRN();
            outrosObs.MostraOutrosObservacoes = false;

            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.SorologiaObservacao.ConvertNullToStringEmpty().IsNotEmpty())
            {
                outrosObs.ValorOutrosObservacoes = pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.SorologiaObservacao;
                outrosObs.MostraOutrosObservacoes = true;
            }

            lista.Add(outrosObs);

            return lista;
        }

        private List<SorologiaExamesItemRN> SorologiaExamesItemRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<SorologiaExamesItemRN> lista = new List<SorologiaExamesItemRN>();
            SorologiaExamesItemRN exame;

            foreach (var item in pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNItens)
            {
                if ((item.ItemCO.Sorologia == SimNao.Sim || item.ItemCO.Exames == SimNao.Sim) && (item.ResultadoRN.IsNotNull()))
                {
                    exame = new SorologiaExamesItemRN();
                    exame.MostraSorologiaExamesItemRN = true;
                    exame.SorologiaExames = item.ItemCO.Descricao;

                    if (item.ResultadoRN == ResultadoItemRN.Negativo)
                        exame.NegNaoRea = "þ";

                    if (item.ResultadoRN == ResultadoItemRN.Positivo)
                        exame.PosRea = "þ";

                    if (item.ResultadoRN == ResultadoItemRN.NaoDisponivel)
                        exame.NaoDisponivel = "þ";

                    lista.Add(exame);
                }
            }

            if (lista.Count == 0)
            {
                exame = new SorologiaExamesItemRN();
                exame.MostraSorologiaExamesItemRN = false;
                lista.Add(exame);
            }

            return lista.OrderBy(x => x.SorologiaExames).ToList();
        }

        private List<PartoRN> PartoRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<PartoRN> lista = new List<PartoRN>();
            PartoRN parto = new PartoRN();
            parto.MostraParto = false;
            parto.MostraSituacaoFetal = false;
            parto.MostraMembranasAmnioticas = false;
            parto.MostraLiquidoAmniotico = false;
            parto.MostraCircularCordao = false;

            // Tipo de Parto
            parto.listaPartoTipoDePartoRN = PartoTipoDePartoRN(pvm);
            if (parto.listaPartoTipoDePartoRN.FirstOrDefault().MostraPartoTipoDeParto)
                parto.MostraParto = true;

            // Situação Fetal
            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsSituacaoFetalNaoTraquilizadora.IsNotNull())
            {
                parto.SituacaoFetal = "Situação Fetal Não Tranquilizadora: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsSituacaoFetalNaoTraquilizadora.Value.GetEnumCustomDisplay();

                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.SituacaoFetalNaoTraquilizadoraObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                    parto.SituacaoFetal += "     Observação: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.SituacaoFetalNaoTraquilizadoraObservacao;

                parto.MostraParto = true;
                parto.MostraSituacaoFetal = true;
            }

            // Membrana Amniótica
            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Membrana.IsNotNull())
                parto.MembranasAmnioticas = "Membranas Amnióticas: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Membrana.Value.GetEnumCustomDisplay();

            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.DataMembrana.IsNotNull())
            {
                if (parto.MembranasAmnioticas.IsNotEmpty())
                    parto.MembranasAmnioticas += "     Data: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.DataMembrana.Value.ToShortDateString();
                else
                    parto.MembranasAmnioticas = "Data: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.DataMembrana.Value.ToShortDateString();

                if (parto.MembranasAmnioticas.IsNotEmpty())
                    parto.MembranasAmnioticas += "     Hora:" + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.DataMembrana.Value.ToShortTimeString();
                else
                    parto.MembranasAmnioticas += "Hora:" + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.DataMembrana.Value.ToShortTimeString();
            }

            if (parto.MembranasAmnioticas.IsNotEmpty())
            {
                parto.MostraParto = true;
                parto.MostraMembranasAmnioticas = true;
            }

            // Líquido Amniótico
            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.LiquidoAmniotico.IsNotNull())
            {
                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.LiquidoAmnioticoObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                    parto.LiquidoAmniotico = "Líquido Amniótico: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.LiquidoAmnioticoObservacao;
                else
                    parto.LiquidoAmniotico = "Líquido Amniótico: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.LiquidoAmniotico.Value.GetEnumCustomDisplay();

                parto.MostraParto = true;
                parto.MostraLiquidoAmniotico = true;
            }

            // Apresentação
            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Apresentacao.IsNotNull())
            {
                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.ApresentacaoObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                    parto.Apresentacao = "Apresentação: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.ApresentacaoObservacao;
                else
                    parto.Apresentacao = "Apresentação: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.Apresentacao.Value.GetEnumCustomDisplay();

                parto.MostraParto = true;
                parto.MostraApresentacao = true;
            }

            // Circular Cordão
            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsCordao.IsNotNull())
            {
                parto.CircularCordao = "Circular cordão: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsCordao.Value.GetEnumCustomDisplay();

                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.CordaoObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                    parto.CircularCordao += "     Observação: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.CordaoObservacao;

                parto.MostraParto = true;
                parto.MostraCircularCordao = true;
            }

            // Tipo Anestesia
            parto.listaPartoTipoDeAnestesiaRN = PartoTipoDeAnestesiaRN(pvm);
            if (parto.listaPartoTipoDeAnestesiaRN.FirstOrDefault().MostraPartoTipoDeAnestesia)
                parto.MostraParto = true;

            // Observações do Tipo de Parto
            parto.listaPartoTipoDePartoObservacoes = PartoTipoDePartoObservacoes(pvm);
            if (parto.listaPartoTipoDePartoObservacoes.FirstOrDefault().MostraObservacoes)
                parto.MostraParto = true;

            lista.Add(parto);

            return lista;
        }

        private List<PartoTipoDePartoObservacoes> PartoTipoDePartoObservacoes(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<PartoTipoDePartoObservacoes> lista = new List<PartoTipoDePartoObservacoes>();
            PartoTipoDePartoObservacoes parto = new PartoTipoDePartoObservacoes();
            parto.MostraObservacoes = false;

            if (pvm.vmSumarioObstetrico.IsNotNull())
                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsNotNull())
                    if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.PartoObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                    {
                        parto.Observacao = pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.PartoObservacao;
                        parto.MostraObservacoes = true;
                    }

            lista.Add(parto);

            return lista;
        }

        private List<PartoTipoDePartoRN> PartoTipoDePartoRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<PartoTipoDePartoRN> lista = new List<PartoTipoDePartoRN>();
            PartoTipoDePartoRN parto = new PartoTipoDePartoRN();
            parto.MostraPartoTipoDeParto = false;

            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.TipoParto.IsNotNull())
                parto.TipoDeParto = "Tipo de Parto: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.TipoParto.Value.GetEnumCustomDisplay();

            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.MotivoCesarianaUrgencia.ConvertNullToStringEmpty().IsNotEmpty())
                if (parto.TipoDeParto.IsNotEmpty())
                    parto.TipoDeParto += ": " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.MotivoCesarianaUrgencia;

            if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsForcipe.IsNotNull())
                if (parto.TipoDeParto.IsNotEmpty())
                    parto.TipoDeParto += "     Fórcipe: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsForcipe.Value.GetEnumCustomDisplay();
                else
                    parto.TipoDeParto = "Fórcipe: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsForcipe.Value.GetEnumCustomDisplay();

            if (parto.TipoDeParto.IsNotEmpty())
                parto.MostraPartoTipoDeParto = true;

            lista.Add(parto);

            return lista;
        }

        private List<AtendimentoRecemNascidoRN> AtendimentoRecemNascidoRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<AtendimentoRecemNascidoRN> lista = new List<AtendimentoRecemNascidoRN>();
            AtendimentoRecemNascidoRN atendimentoRN = new AtendimentoRecemNascidoRN();
            atendimentoRN.MostraAtendimentoRecemNascidoRN = false;
            atendimentoRN.MostraDataNascHoraSexoECor = false;
            atendimentoRN.MostraPesoCompPerCefEPerTor = false;
            atendimentoRN.MostraReanimacaoUrinouEEvacuou = false;
            atendimentoRN.MostraMedicacoes = false;
            atendimentoRN.MostraClassificacaoECapurro = false;

            // Data Nascimento, Hora, Sexo e Cor
            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.DiaNascimento.IsNotNull())
                atendimentoRN.DataNascHoraSexoECor = "Data do Nascimento: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.DiaNascimento.Value.ToShortDateString();

            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.HoraNascimento.IsNotNull())
                if (atendimentoRN.DataNascHoraSexoECor.IsNotEmpty())
                    atendimentoRN.DataNascHoraSexoECor += "     Hora: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.HoraNascimento.Value.ToShortTimeString();
                else
                    atendimentoRN.DataNascHoraSexoECor = "Hora: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.HoraNascimento.Value.ToShortTimeString();

            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Sexo.IsNotNull())
                if (atendimentoRN.DataNascHoraSexoECor.IsNotEmpty())
                    atendimentoRN.DataNascHoraSexoECor += "     Sexo: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Sexo;
                else
                    atendimentoRN.DataNascHoraSexoECor = "Sexo: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Sexo;

            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Cor.IsNotNull())
                if (atendimentoRN.DataNascHoraSexoECor.IsNotEmpty())
                    atendimentoRN.DataNascHoraSexoECor += "     Cor: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Cor;
                else
                    atendimentoRN.DataNascHoraSexoECor = "Cor: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Cor;

            if (atendimentoRN.DataNascHoraSexoECor.IsNotEmpty())
            {
                atendimentoRN.MostraAtendimentoRecemNascidoRN = true;
                atendimentoRN.MostraDataNascHoraSexoECor = true;
            }

            // Peso, Comprimento, Perímetro Cefálico e Perímetro Torácico
            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Peso.IsNotNull())
                atendimentoRN.PesoCompPerCefEPerTor = "Peso: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Peso.Value + " g";

            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Comprimento.IsNotNull())
                if (atendimentoRN.PesoCompPerCefEPerTor.IsNotEmpty())
                    atendimentoRN.PesoCompPerCefEPerTor += "     Comprimento: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Comprimento.Value + " cm";
                else
                    atendimentoRN.PesoCompPerCefEPerTor = "Comprimento: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Comprimento.Value + " cm";

            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.PerimentroCefalico.IsNotNull())
                if (atendimentoRN.PesoCompPerCefEPerTor.IsNotEmpty())
                    atendimentoRN.PesoCompPerCefEPerTor += "     Perímetro Cefálico: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.PerimentroCefalico.ToString() + " cm";
                else
                    atendimentoRN.PesoCompPerCefEPerTor = "Perímetro Cefálico: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.PerimentroCefalico.ToString() + " cm";

            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.PerimetroToracico.IsNotNull())
                if (atendimentoRN.PesoCompPerCefEPerTor.IsNotEmpty())
                    atendimentoRN.PesoCompPerCefEPerTor += "     Perímetro Torácico: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.PerimetroToracico.ToString() + " cm";
                else
                    atendimentoRN.PesoCompPerCefEPerTor = "Perímetro Torácico: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.PerimetroToracico.ToString() + " cm";

            if (atendimentoRN.PesoCompPerCefEPerTor.IsNotEmpty())
            {
                atendimentoRN.MostraAtendimentoRecemNascidoRN = true;
                atendimentoRN.MostraPesoCompPerCefEPerTor = true;
            }

            // Reanimação, Urinou e Evacuou
            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacao.IsNotNull())
            {
                if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacao == SimNao.Sim)
                {
                    if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.TipoReanimacao.IsNotNull())
                        atendimentoRN.ReanimacaoUrinouEEvacuou = "Reanimação: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.TipoReanimacao.Value.GetEnumCustomDisplay();
                    else
                    {
                        string reanimacao = string.Empty;
                        if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoEntubacao == SimNao.Sim)
                            reanimacao = "Entubação, ";
                        if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoMassagem == SimNao.Sim)
                            reanimacao += "Massagem Cardíaca, ";
                        if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoOxigenio == SimNao.Sim)
                            reanimacao += "Oxigênio, ";
                        if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoVentilacao == SimNao.Sim)
                            reanimacao += "Ventilação, ";

                        if (reanimacao.TrimEnd(',').IsNotEmptyOrWhiteSpace())
                            atendimentoRN.ReanimacaoUrinouEEvacuou = "Reanimação: " + reanimacao.Trim().TrimEnd(',');
                    }
                }
                else
                    atendimentoRN.ReanimacaoUrinouEEvacuou = "Reanimação: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacao.Value.GetEnumCustomDisplay();
            }

            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsUrinou.IsNotNull())
                if (atendimentoRN.ReanimacaoUrinouEEvacuou.IsNotEmpty())
                    atendimentoRN.ReanimacaoUrinouEEvacuou += "     Urinou: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsUrinou.Value.GetEnumCustomDisplay();
                else
                    atendimentoRN.ReanimacaoUrinouEEvacuou = "Urinou: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsUrinou.Value.GetEnumCustomDisplay();

            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsEvacuou.IsNotNull())
                if (atendimentoRN.ReanimacaoUrinouEEvacuou.IsNotEmpty())
                    atendimentoRN.ReanimacaoUrinouEEvacuou += "     Evacuou: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsEvacuou.Value.GetEnumCustomDisplay();
                else
                    atendimentoRN.ReanimacaoUrinouEEvacuou = "Evacuou: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsEvacuou.Value.GetEnumCustomDisplay();

            if (atendimentoRN.ReanimacaoUrinouEEvacuou.IsNotEmpty())
            {
                atendimentoRN.MostraAtendimentoRecemNascidoRN = true;
                atendimentoRN.MostraReanimacaoUrinouEEvacuou = true;
            }

            // Medicações
            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsMedicamentos.IsNotNull())
            {
                atendimentoRN.Medicacoes = "Medicações: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsMedicamentos.Value.GetEnumCustomDisplay();

                if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.MedicamentosObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                    atendimentoRN.Medicacoes = "Medicações: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.MedicamentosObservacao;

                atendimentoRN.MostraAtendimentoRecemNascidoRN = true;
                atendimentoRN.MostraMedicacoes = true;
            }

            // Classificação e Capurro
            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Classificacao.IsNotNull())
                atendimentoRN.ClassificacaoECapurro = "Classificação: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.Classificacao.Value.GetEnumCustomDisplay();

            string tituloCapurro = string.Empty;

            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.CapurroSemanas.IsNotNull())
                tituloCapurro = "Capurro: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.CapurroSemanas + " semana(s)";

            if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.CapurroDias.IsNotNull())
                if (tituloCapurro.IsNotEmpty())
                    tituloCapurro += " e " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.CapurroDias + " dia(s)";
                else
                    tituloCapurro = "Capurro: " + pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.CapurroDias + " dia(s)";

            if (tituloCapurro.IsEmpty())
                if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsNaoRealizadoCapurro.IsNotNull())
                    if (pvm.vmSumarioAvaliacaoMedicaRNRecemNascido.SumarioAvaliacaoMedicaRNSalaParto.IsNaoRealizadoCapurro == SimNao.Sim)
                        tituloCapurro = "Capurro: Não Realizado";

            if (tituloCapurro.IsNotEmpty())
                if (atendimentoRN.ClassificacaoECapurro.IsNotEmpty())
                    atendimentoRN.ClassificacaoECapurro += "     " + tituloCapurro;
                else
                    atendimentoRN.ClassificacaoECapurro += tituloCapurro;
            //atendimentoRN.ClassificacaoECapurro += tituloCapurro;


            if (atendimentoRN.ClassificacaoECapurro.IsNotEmpty())
            {
                atendimentoRN.MostraAtendimentoRecemNascidoRN = true;
                atendimentoRN.MostraClassificacaoECapurro = true;
            }
            //

            atendimentoRN.listaApgarRN = ApgarRN(pvm);
            if (atendimentoRN.listaApgarRN.FirstOrDefault().MostraApgarComGrid)
                atendimentoRN.MostraAtendimentoRecemNascidoRN = true;

            if (atendimentoRN.listaApgarRN.FirstOrDefault().MostraApgarSemGrid)
                atendimentoRN.MostraAtendimentoRecemNascidoRN = true;

            atendimentoRN.listaAtendimentoRecemNascidoObservacoesRN = AtendimentoRecemNascidoObservacoesRN(pvm);
            if (atendimentoRN.listaAtendimentoRecemNascidoObservacoesRN.FirstOrDefault().MostraObservacoes)
                atendimentoRN.MostraAtendimentoRecemNascidoRN = true;

            lista.Add(atendimentoRN);

            return lista;
        }

        private List<ApgarRN> ApgarRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<ApgarRN> lista = new List<ApgarRN>();
            ApgarRN apgar = new ApgarRN();
            apgar.MostraApgarComGrid = false;
            apgar.MostraApgarSemGrid = false;

            for (int i = 0; i < pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNApgar.Count; i++)
            {
                apgar.MostraApgarComGrid = true;
                var item = pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNApgar[i];

                if (item.IsNotNull())
                {
                    if (item.FrequenciaCardiaca.IsNotNull())
                    {
                        switch (item.FrequenciaCardiaca)
                        {
                            case 0:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                {
                                    apgar.FCPrimeiroMin0 = "þ";
                                }
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                {
                                    apgar.FCQuintoMin0 = "þ";
                                }
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                {
                                    apgar.FCDecimoMin0 = "þ";
                                    apgar.temZeroDecimoMin = true;
                                }
                                break;
                            case 1:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                    apgar.FCPrimeiroMin1 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                    apgar.FCQuintoMin1 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                    apgar.FCDecimoMin1 = "þ";
                                break;
                            case 2:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                    apgar.FCPrimeiroMin2 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                    apgar.FCQuintoMin2 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                    apgar.FCDecimoMin2 = "þ";
                                break;
                        }
                    }

                    if (item.Esforco.IsNotNull())
                    {
                        switch (item.Esforco)
                        {
                            case 0:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                {
                                    apgar.ERPrimeiroMin0 = "þ";
                                }
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                {
                                    apgar.ERQuintoMin0 = "þ";
                                }
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                {
                                    apgar.ERDecimoMin0 = "þ";
                                    apgar.temZeroDecimoMin = true;
                                }
                                break;
                            case 1:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                    apgar.ERPrimeiroMin1 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                    apgar.ERQuintoMin1 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                    apgar.ERDecimoMin1 = "þ";
                                break;
                            case 2:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                    apgar.ERPrimeiroMin2 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                    apgar.ERQuintoMin2 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                    apgar.ERDecimoMin2 = "þ";
                                break;
                        }
                    }

                    if (item.Tonus.IsNotNull())
                    {
                        switch (item.Tonus)
                        {
                            case 0:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                {
                                    apgar.TMPrimeiroMin0 = "þ";
                                }
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                {
                                    apgar.TMQuintoMin0 = "þ";
                                }
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                {
                                    apgar.TMDecimoMin0 = "þ";
                                    apgar.temZeroDecimoMin = true;
                                }
                                break;
                            case 1:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                    apgar.TMPrimeiroMin1 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                    apgar.TMQuintoMin1 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                    apgar.TMDecimoMin1 = "þ";
                                break;
                            case 2:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                    apgar.TMPrimeiroMin2 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                    apgar.TMQuintoMin2 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                    apgar.TMDecimoMin2 = "þ";
                                break;
                        }
                    }

                    if (item.Irritabilidade.IsNotNull())
                    {
                        switch (item.Irritabilidade)
                        {
                            case 0:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                {
                                    apgar.IRPrimeiroMin0 = "þ";
                                }
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                {
                                    apgar.IRQuintoMin0 = "þ";
                                }
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                {
                                    apgar.IRDecimoMin0 = "þ";
                                    apgar.temZeroDecimoMin = true;
                                }
                                break;
                            case 1:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                    apgar.IRPrimeiroMin1 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                    apgar.IRQuintoMin1 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                    apgar.IRDecimoMin1 = "þ";
                                break;
                            case 2:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                    apgar.IRPrimeiroMin2 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                    apgar.IRQuintoMin2 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                    apgar.IRDecimoMin2 = "þ";
                                break;
                        }
                    }

                    if (item.Cor.IsNotNull())
                    {
                        switch (item.Cor)
                        {
                            case 0:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                {
                                    apgar.COPrimeiroMin0 = "þ";
                                    apgar.temZeroDecimoMin = true;
                                }
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                {
                                    apgar.COQuintoMin0 = "þ";
                                }
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                {
                                    apgar.CODecimoMin0 = "þ";
                                    apgar.temZeroDecimoMin = true;
                                }
                                break;
                            case 1:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                    apgar.COPrimeiroMin1 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                    apgar.COQuintoMin1 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                    apgar.CODecimoMin1 = "þ";
                                break;
                            case 2:
                                if (item.Minuto == MinutoApgarRN.Primeiro)
                                    apgar.COPrimeiroMin2 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Quinto)
                                    apgar.COQuintoMin2 = "þ";
                                else if (item.Minuto == MinutoApgarRN.Dessimo)
                                    apgar.CODecimoMin2 = "þ";
                                break;
                        }
                    }
                }
            }

            bool mostraPontuacao = false;

            if (pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarPrimeiro.IsNotNull())
            {
                apgar.Primeiro = pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarPrimeiro.ToString();
                apgar.MostraApgar = true;
                mostraPontuacao = true;
            }

            if (pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarQuinto.IsNotNull())
            {
                apgar.Quinto = pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarQuinto.ToString();
                apgar.MostraApgar = true;
                mostraPontuacao = true;
            }

            if (pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarDessimo.IsNotNull())
            {
                if (pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarDessimo == 0)
                {
                    if (apgar.temZeroDecimoMin)
                    {
                        apgar.Decimo = pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarDessimo.ToString();
                        apgar.MostraApgar = true;
                        mostraPontuacao = true;
                    }
                }
                else
                {
                    apgar.Decimo = pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarDessimo.ToString();
                    apgar.MostraApgar = true;
                    mostraPontuacao = true;
                }
            }

            if (mostraPontuacao)
                if (pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNApgar.Count > 0)
                    apgar.MostraApgarComGrid = true;
                else
                {
                    apgar.PrimeiroSemGrid = pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarPrimeiro.IsNotNull() ? pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarPrimeiro.ToString() : string.Empty;
                    apgar.QuintoSemGrid = pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarQuinto.IsNotNull() ? pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarQuinto.ToString() : string.Empty;
                    apgar.DecimoSemGrid = pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarDessimo.IsNotNull() ? pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.ApgarDessimo.ToString() : string.Empty;
                    apgar.MostraApgarSemGrid = true;
                }

            lista.Add(apgar);

            return lista;
        }

        private List<AtendimentoRecemNascidoObservacoesRN> AtendimentoRecemNascidoObservacoesRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<AtendimentoRecemNascidoObservacoesRN> lista = new List<AtendimentoRecemNascidoObservacoesRN>();
            AtendimentoRecemNascidoObservacoesRN obs = new AtendimentoRecemNascidoObservacoesRN();
            obs.MostraObservacoes = false;

            if (pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.Observacoes.ConvertNullToStringEmpty().IsNotEmpty())
            {
                obs.ValorObservacoes = pvm.SumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.Observacoes;
                obs.MostraObservacoes = true;
            }

            lista.Add(obs);

            return lista;
        }

        private List<ExameClinicoRN> ExameClinicoRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<ExameClinicoRN> lista = new List<ExameClinicoRN>();
            ExameClinicoRN exame = new ExameClinicoRN();
            exame.MostraExameClinicoRN = false;

            exame.listaExameClinicoParte1RN = ExameClinicoParte1RN(pvm);
            if (exame.listaExameClinicoParte1RN.FirstOrDefault().MostraExameClinicoParte1RN)
                exame.MostraExameClinicoRN = true;

            exame.listaExameClinicoParte2RN = ExameClinicoParte2RN(pvm);
            if (exame.listaExameClinicoParte2RN.FirstOrDefault().MostraExameClinicoParte2RN)
                exame.MostraExameClinicoRN = true;

            exame.listaExameClinicoParte3RN = ExameClinicoParte3RN(pvm);
            if (exame.listaExameClinicoParte3RN.FirstOrDefault().MostraExameClinicoParte3RN)
                exame.MostraExameClinicoRN = true;

            lista.Add(exame);

            return lista;
        }

        private List<ExameClinicoParte1RN> ExameClinicoParte1RN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<ExameClinicoParte1RN> lista = new List<ExameClinicoParte1RN>();
            ExameClinicoParte1RN exame = new ExameClinicoParte1RN();
            exame.MostraExameClinicoParte1RN = false;

            // Sinais Vitais
            exame.listaSinaisVitaisRN = SinaisVitaisRN(pvm);
            if (exame.listaSinaisVitaisRN.FirstOrDefault().MostraSinaisVitaisRN)
                exame.MostraExameClinicoParte1RN = true;

            // Aspecto Geral - Facies 
            exame.listaAspectoGeralRN = AspectoGeralRN(pvm);
            if (exame.listaAspectoGeralRN.FirstOrDefault().MostraAspectoGeral)
                exame.MostraExameClinicoParte1RN = true;

            // Cabeça e Pescoço
            exame.listaCabecaEPescocoRN = CabecaEPescocoRN(pvm);
            if (exame.listaCabecaEPescocoRN.FirstOrDefault().MostraCabecaEPescocoRN)
                exame.MostraExameClinicoParte1RN = true;

            lista.Add(exame);

            return lista;
        }

        private List<SinaisVitaisRN> SinaisVitaisRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<SinaisVitaisRN> lista = new List<SinaisVitaisRN>();
            SinaisVitaisRN sinais = new SinaisVitaisRN();
            sinais.MostraSinaisVitaisRN = false;

            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.FC.IsNotNull())
                sinais.ValorSinaisVitais = "FC: " + pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.FC.ToString() + " bpm";

            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.FR.IsNotNull())
                sinais.ValorSinaisVitais += "    FR: " + pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.FR.ToString() + " mrpm";

            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.TAX.IsNotNull())
                sinais.ValorSinaisVitais += "    Tax: " + pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.TAX.ToString() + " °C";

            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.SAT.IsNotNull())
                sinais.ValorSinaisVitais += "    SAT: " + pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.SAT.ToString() + " %";

            if (sinais.ValorSinaisVitais.IsNotEmpty())
                sinais.MostraSinaisVitaisRN = true;

            lista.Add(sinais);

            return lista;
        }

        public List<AspectoGeralRN> AspectoGeralRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<AspectoGeralRN> lista = new List<AspectoGeralRN>();
            AspectoGeralRN aspecto = new AspectoGeralRN();
            aspecto.MostraAspectoGeral = false;
            aspecto.MostraFaciesECor = false;
            aspecto.MostraTonusEAtividade = false;           

            // Facies
            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.IsFacies.IsNotNull())
            {
                if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.IsFacies == SimNao.Sim)
                    aspecto.FaciesECor = "Facies: Atípica";
                else if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.FaciesObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                    aspecto.FaciesECor = "Facies: " + pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.FaciesObservacao;

                aspecto.MostraAspectoGeral = true;
                aspecto.MostraFaciesECor = true;
            }

            string cor = string.Empty;

            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.IsCorRosada == SimNao.Sim)
                cor = "Cor: " + CorRN.Rosada.GetEnumCustomDisplay();

            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.IsCorPalida == SimNao.Sim)
                if (cor.IsNotEmpty())
                    cor += ", " + CorRN.Palida.GetEnumCustomDisplay();
                else
                    cor = "Cor: " + CorRN.Palida.GetEnumCustomDisplay();

            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.IsCorCianose == SimNao.Sim)
                if (cor.IsNotEmpty())
                    cor += ", " + CorRN.CianoseExtremidades.GetEnumCustomDisplay();
                else
                    cor = "Cor: " + CorRN.CianoseExtremidades.GetEnumCustomDisplay();

            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.IsCorCianoseGeneralizada == SimNao.Sim)
                if (cor.IsNotEmpty())
                    cor += ", " + CorRN.CianoseGeneralizada.GetEnumCustomDisplay();
                else
                    cor = "Cor: " + CorRN.CianoseGeneralizada.GetEnumCustomDisplay();

            if (aspecto.FaciesECor.IsNotEmpty())
                aspecto.FaciesECor += "     " + cor;
            else
                aspecto.FaciesECor = cor;

            if (aspecto.FaciesECor.IsNotEmpty())
            {
                aspecto.MostraAspectoGeral = true;
                aspecto.MostraFaciesECor = true;
            }

            // Tônus e Atividade e Pele
            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.Tonus.IsNotNull())
                aspecto.TonusEAtividade = "Tônus: " + pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.Tonus.Value.GetEnumCustomDisplay();

            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.Atividade.IsNotNull())
                if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.Atividade == AtividadeRN.Reativo)
                    if (aspecto.TonusEAtividade.IsNotEmpty())
                        aspecto.TonusEAtividade += "     Atividade: " + "Reativo ao Manuseio";
                    else
                        aspecto.TonusEAtividade = "Atividade: " + "Reativo ao Manuseio";
                else
                    if (aspecto.TonusEAtividade.IsNotEmpty())
                        aspecto.TonusEAtividade += "     Atividade: " + pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.Atividade.Value.GetEnumCustomDisplay();
                    else
                        aspecto.TonusEAtividade = "Atividade: " + pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.Atividade.Value.GetEnumCustomDisplay();

            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.Pele.IsNotNull())
                if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.Pele == Pele.Normal)
                    if (aspecto.TonusEAtividade.IsNotEmpty())
                        aspecto.TonusEAtividade += "     Pele: " + "Normal";
                    else
                        aspecto.TonusEAtividade = "Pele: " + "Normal";
                else
                    if (aspecto.TonusEAtividade.IsNotEmpty())
                        aspecto.TonusEAtividade += "     Pele: " + " " + pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.PeleOutros;
                    else
                        aspecto.TonusEAtividade = "Pele: " + " " + pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.PeleOutros;

            if (aspecto.TonusEAtividade.IsNotEmpty())
            {
                aspecto.MostraAspectoGeral = true;
                aspecto.MostraTonusEAtividade = true;
            }
           
            lista.Add(aspecto);

            return lista;
        }

        private List<CabecaEPescocoRN> CabecaEPescocoRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<CabecaEPescocoRN> lista = new List<CabecaEPescocoRN>();
            List<CabecaEPescocoRN> listaNormais = new List<CabecaEPescocoRN>();
            CabecaEPescocoRN cabecaEPescoco;
            string normais = string.Empty;
            string olhinhos = string.Empty;

            foreach (var item in pvm.vmSumarioAvaliacaoMedicaRNExameClinico.CollectionCabecaPescoco)
            {
                if (item.Selecionado == true && item.IsNormal.IsNotNull())
                {
                    if (item.IsNormal == SimNao.Sim)
                    {
                        if (item.ItemCO.Descricao == "Teste do Olhinho")
                        {
                            olhinhos = "Teste do Olhinho: Normal";
                        }
                        else
                        {
                            if (normais.IsNotEmpty())
                                normais += ", " + item.ItemCO.Descricao;
                            else
                                normais += "Normal: " + item.ItemCO.Descricao;
                        }
                    }
                    else
                    {
                        if (item.Observacao.ConvertNullToStringEmpty().IsNotEmpty())
                        {
                            cabecaEPescoco = new CabecaEPescocoRN();
                            cabecaEPescoco.Item = item.ItemCO.Descricao + ": " + item.Observacao;
                            cabecaEPescoco.MostraCabecaEPescocoRN = true;
                            lista.Add(cabecaEPescoco);
                        }
                    }
                }
            }

            if (normais.IsNotEmpty())
                listaNormais.Add(new CabecaEPescocoRN { Item = normais, MostraCabecaEPescocoRN = true });

            if (olhinhos.IsNotEmpty())
                listaNormais.Add(new CabecaEPescocoRN { Item = olhinhos, MostraCabecaEPescocoRN = true });

            if (lista.Count == 0 && listaNormais.Count == 0)
            {
                cabecaEPescoco = new CabecaEPescocoRN();
                cabecaEPescoco.MostraCabecaEPescocoRN = false;
                lista.Add(cabecaEPescoco);
            }
            else
            {
                listaNormais.AddRange(lista);
                return listaNormais;
            }

            return lista;
        }

        private List<ExameClinicoParte2RN> ExameClinicoParte2RN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<ExameClinicoParte2RN> lista = new List<ExameClinicoParte2RN>();
            ExameClinicoParte2RN exame = new ExameClinicoParte2RN();
            exame.MostraExameClinicoParte2RN = false;

            exame.listaRespiratorioRN = RespiratorioRN(pvm);
            if (exame.listaRespiratorioRN.FirstOrDefault().MostraRespiratorioRN)
                exame.MostraExameClinicoParte2RN = true;

            exame.listaCardioVascularRN = CardioVascularRN(pvm);
            if (exame.listaCardioVascularRN.FirstOrDefault().MostraCardioVascularRN)
                exame.MostraExameClinicoParte2RN = true;

            exame.listaOutrosRN = OutrosRN(pvm);
            if (exame.listaOutrosRN.FirstOrDefault().MostraOutrosRN)
                exame.MostraExameClinicoParte2RN = true;

            lista.Add(exame);

            return lista;
        }

        private List<RespiratorioRN> RespiratorioRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<RespiratorioRN> lista = new List<RespiratorioRN>();
            List<RespiratorioRN> listaNormais = new List<RespiratorioRN>();
            RespiratorioRN respiratorio;
            string normais = string.Empty;

            foreach (var item in pvm.vmSumarioAvaliacaoMedicaRNExameClinico.CollectionRespiratorio)
            {
                if (item.Selecionado == true && item.IsNormal.IsNotNull())
                {
                    if (item.IsNormal == SimNao.Sim)
                    {
                        if (normais.IsNotEmpty())
                            normais += ", " + item.ItemCO.Descricao;
                        else
                            normais += "Normal: " + item.ItemCO.Descricao;
                    }
                    else
                    {
                        respiratorio = new RespiratorioRN();
                        respiratorio.Item = item.ItemCO.Descricao + ": " + item.Observacao;
                        respiratorio.MostraRespiratorioRN = true;
                        lista.Add(respiratorio);
                    }
                }
            }

            if (normais.IsNotEmpty())
                listaNormais.Add(new RespiratorioRN { Item = normais, MostraRespiratorioRN = true });

            if (lista.Count == 0 && listaNormais.Count == 0)
            {
                respiratorio = new RespiratorioRN();
                respiratorio.MostraRespiratorioRN = false;
                lista.Add(respiratorio);
            }
            else
            {
                listaNormais.AddRange(lista);
                return listaNormais;
            }

            return lista;
        }

        private List<CardioVascularRN> CardioVascularRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<CardioVascularRN> lista = new List<CardioVascularRN>();
            List<CardioVascularRN> listaNormais = new List<CardioVascularRN>();
            CardioVascularRN cardioVascular;
            string normais = string.Empty;

            foreach (var item in pvm.vmSumarioAvaliacaoMedicaRNExameClinico.CollectionCardioVascular)
            {
                if (item.Selecionado == true && item.IsNormal.IsNotNull())
                {
                    if (item.IsNormal == SimNao.Sim)
                    {
                        if (normais.IsNotEmpty())
                            normais += ", " + item.ItemCO.Descricao;
                        else
                            normais += "Normal: " + item.ItemCO.Descricao;
                    }
                    else
                    {
                        cardioVascular = new CardioVascularRN();
                        cardioVascular.Item = item.ItemCO.Descricao + ": " + item.Observacao;
                        cardioVascular.MostraCardioVascularRN = true;
                        lista.Add(cardioVascular);
                    }
                }
            }

            if (normais.IsNotEmpty())
                listaNormais.Add(new CardioVascularRN { Item = normais, MostraCardioVascularRN = true });

            if (lista.Count == 0 && listaNormais.Count == 0)
            {
                cardioVascular = new CardioVascularRN();
                cardioVascular.MostraCardioVascularRN = false;
                lista.Add(cardioVascular);
            }
            else
            {
                listaNormais.AddRange(lista);
                return listaNormais;
            }

            return lista;
        }

        private List<OutrosRN> OutrosRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<OutrosRN> lista = new List<OutrosRN>();
            List<OutrosRN> listaNormais = new List<OutrosRN>();
            OutrosRN outros;
            string normais = string.Empty;

            foreach (var item in pvm.vmSumarioAvaliacaoMedicaRNExameClinico.CollectionOutros)
            {
                if (item.Selecionado == true && item.IsNormal.IsNotNull())
                {
                    if (item.IsNormal == SimNao.Sim)
                    {
                        if (normais.IsNotEmpty())
                            normais += ", " + item.ItemCO.Descricao;
                        else
                            normais += "Normal: " + item.ItemCO.Descricao;
                    }
                    else
                    {
                        outros = new OutrosRN();
                        outros.Item = item.ItemCO.Descricao + ": " + item.Observacao;
                        outros.MostraOutrosRN = true;
                        lista.Add(outros);
                    }
                }
            }

            if (normais.IsNotEmpty())
                listaNormais.Add(new OutrosRN { Item = normais, MostraOutrosRN = true });

            if (lista.Count == 0 && listaNormais.Count == 0)
            {
                outros = new OutrosRN();
                outros.MostraOutrosRN = false;
                lista.Add(outros);
            }
            else
            {
                listaNormais.AddRange(lista);
                return listaNormais;
            }

            return lista;
        }

        private List<ExameClinicoParte3RN> ExameClinicoParte3RN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<ExameClinicoParte3RN> lista = new List<ExameClinicoParte3RN>();
            ExameClinicoParte3RN exame = new ExameClinicoParte3RN();
            exame.MostraExameClinicoParte3RN = false;

            exame.listaOsteoArticularRN = this.OsteoArticularRN(pvm);
            if (exame.listaOsteoArticularRN.FirstOrDefault().MostraOsteoArticularRN)
                exame.MostraExameClinicoParte3RN = true;

            exame.listaExameClinicoObservacoesRN = this.ExameClinicoObservacoesRN(pvm);
            if (exame.listaExameClinicoObservacoesRN.FirstOrDefault().MostraObservacoes)
                exame.MostraExameClinicoParte3RN = true;

            lista.Add(exame);

            return lista;
        }

        private List<ExameClinicoObservacoesRN> ExameClinicoObservacoesRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<ExameClinicoObservacoesRN> lista = new List<ExameClinicoObservacoesRN>();
            ExameClinicoObservacoesRN obs = new ExameClinicoObservacoesRN();

            if (pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.Observacao.ConvertNullToStringEmpty().IsNotEmpty())
            {
                obs.ValorObservacoes = pvm.vmSumarioAvaliacaoMedicaRNExameClinico.SumarioAvaliacaoMedicaRNExameFisico.Observacao;
                obs.MostraObservacoes = true;
            }

            lista.Add(obs);

            return lista;
        }

        private List<OsteoArticularRN> OsteoArticularRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<OsteoArticularRN> lista = new List<OsteoArticularRN>();
            List<OsteoArticularRN> listaNormais = new List<OsteoArticularRN>();
            OsteoArticularRN osteoArticular;
            string normais = string.Empty;

            foreach (var item in pvm.vmSumarioAvaliacaoMedicaRNExameClinico.CollectionOsteoArticular)
            {
                if (item.Selecionado == true && item.IsNormal.IsNotNull())
                {
                    if (item.IsNormal == SimNao.Sim)
                    {
                        if (normais.IsNotEmpty())
                            normais += ", " + item.ItemCO.Descricao;
                        else
                            normais += "Normal: " + item.ItemCO.Descricao;
                    }
                    else
                    {
                        osteoArticular = new OsteoArticularRN();
                        osteoArticular.Item = item.ItemCO.Descricao + ": " + item.Observacao;
                        osteoArticular.MostraOsteoArticularRN = true;
                        lista.Add(osteoArticular);
                    }
                }
            }

            if (normais.IsNotEmpty())
                listaNormais.Add(new OsteoArticularRN { Item = normais, MostraOsteoArticularRN = true });

            if (lista.Count == 0 && listaNormais.Count == 0)
            {
                osteoArticular = new OsteoArticularRN();
                osteoArticular.MostraOsteoArticularRN = false;
                lista.Add(osteoArticular);
            }
            else
            {
                listaNormais.AddRange(lista);
                return listaNormais;
            }

            return lista;
        }

        private List<PartoTipoDeAnestesiaRN> PartoTipoDeAnestesiaRN(vmSumarioAvaliacaoMedicaRN pvm)
        {
            List<PartoTipoDeAnestesiaRN> lista = new List<PartoTipoDeAnestesiaRN>();
            PartoTipoDeAnestesiaRN anestesia = new PartoTipoDeAnestesiaRN();
            anestesia.MostraPartoTipoDeAnestesia = false;

            if (pvm.vmSumarioObstetrico.IsNotNull())
                if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.IsNotNull())
                    if (pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.TipoAnestesia.IsNotNull())
                    {
                        anestesia.TipoDeAnestesia = "Tipo de Anestesia: " + pvm.vmSumarioObstetrico.SumarioAvaliacaoMedicaRN.TipoAnestesia.Value.GetEnumCustomDisplay();
                        anestesia.MostraPartoTipoDeAnestesia = true;
                    }

            lista.Add(anestesia);

            return lista;
        }

        #endregion Métodos
        //bool disposed = false;

        //// método privado para controle
        //// da liberação dos recursos
        //private void Dispose(bool disposing)
        //{
        //    // Verifique se Dispose já foi chamado.
        //    if (!this.disposed)
        //    {
        //        if (disposing)
        //        {
        //            _listaRelatorio = null;
        //            _vm = null;
        //            _listaAssinaturaRNSource = null;
        //            _listaSumarioObstetricoRNSource = null;
        //            _listaAtendimentoRecemNascidoRNSource = null;
        //            _listaExameClinicoRNSource = null;
        //        }

        //        // Seta a variável booleana para true,
        //        // indicando que os recursos já foram liberados
        //        disposed = true;
        //    }
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //~vmRelatorioSumarioAvaliacaoMedicaRN()
        //{
        //    Dispose(false);
        //}
    }

    public class SumarioAvaliacaoMedicaRN
    {
        public string Nome { get; set; }
        public string Prontuario { get; set; }
        public string Atendimento { get; set; }
        public string DataAtendimento { get; set; }
        public string Sexo { get; set; }
        public string Cor { get; set; }
        //public string Idade { get; set; }

        public List<SumarioObstetricoRN> listaSumarioObstetricoRN { get; set; }
        public List<AtendimentoRecemNascidoRN> listaAtendimentoRecemNascidoRN { get; set; }
        public List<ExameClinicoRN> listaExameClinicoRN { get; set; }
    }

    public class RodapeSumarioAvaliacaoMedicaRN
    {
        public string NomePaciente { get; set; }
        public string NomeResumo { get; set; }
        public int IDPaciente { get; set; }
        public string NomePrestador { get; set; }
        public string Registro { get; set; }
        public string CodigoBarras { get; set; }

        public bool MostraCodigoBarras { get; set; }
        public bool MostraIDPaciente { get; set; }
    }

    public class AssinaturaRN
    {
        public string Assinatura { get; set; }
        public string DataEncerramento { get; set; }
    }

    public class SumarioObstetricoRN
    {
        public bool MostraSumarioObstetricoRN { get; set; }

        public List<IdentificacaoGestacoesAnterioresRN> listaIdentificacaoGestacoesAnterioresRN { get; set; }
        public List<GestacaoAtualRN> listaGestacaoAtualRN { get; set; }
        public List<SorologiaExamesRN> listaSorologiaExamesRN { get; set; }
        public List<PartoRN> listaPartoRN { get; set; }
    }

    public class AtendimentoRecemNascidoRN
    {
        public string DataNascHoraSexoECor { get; set; }
        public string PesoCompPerCefEPerTor { get; set; }
        public string ReanimacaoUrinouEEvacuou { get; set; }
        public string Medicacoes { get; set; }
        public string ClassificacaoECapurro { get; set; }

        public bool MostraAtendimentoRecemNascidoRN { get; set; }
        public bool MostraDataNascHoraSexoECor { get; set; }
        public bool MostraPesoCompPerCefEPerTor { get; set; }
        public bool MostraReanimacaoUrinouEEvacuou { get; set; }
        public bool MostraMedicacoes { get; set; }
        public bool MostraClassificacaoECapurro { get; set; }

        public List<ApgarRN> listaApgarRN { get; set; }
        public List<AtendimentoRecemNascidoObservacoesRN> listaAtendimentoRecemNascidoObservacoesRN { get; set; }
    }

    public class ExameClinicoRN
    {
        public List<ExameClinicoParte1RN> listaExameClinicoParte1RN { get; set; }
        public List<ExameClinicoParte2RN> listaExameClinicoParte2RN { get; set; }
        public List<ExameClinicoParte3RN> listaExameClinicoParte3RN { get; set; }

        public bool MostraExameClinicoRN { get; set; }
    }

    public class ExameClinicoParte1RN
    {
        public bool MostraExameClinicoParte1RN { get; set; }

        public List<SinaisVitaisRN> listaSinaisVitaisRN { get; set; }
        public List<AspectoGeralRN> listaAspectoGeralRN { get; set; }
        public List<CabecaEPescocoRN> listaCabecaEPescocoRN { get; set; }
    }

    public class SinaisVitaisRN
    {
        public string ValorSinaisVitais { get; set; }

        public bool MostraSinaisVitaisRN { get; set; }
    }

    public class AspectoGeralRN
    {
        public string TonusEAtividade { get; set; }
        public string FaciesECor { get; set; }        

        public bool MostraAspectoGeral { get; set; }
        public bool MostraFaciesECor { get; set; }
        public bool MostraTonusEAtividade { get; set; }
    }

    public class ExameClinicoParte2RN
    {
        public bool MostraExameClinicoParte2RN { get; set; }

        public List<RespiratorioRN> listaRespiratorioRN { get; set; }
        public List<CardioVascularRN> listaCardioVascularRN { get; set; }
        public List<OutrosRN> listaOutrosRN { get; set; }
    }

    public class ExameClinicoParte3RN
    {
        public bool MostraExameClinicoParte3RN { get; set; }

        public List<OsteoArticularRN> listaOsteoArticularRN { get; set; }
        public List<ExameClinicoObservacoesRN> listaExameClinicoObservacoesRN { get; set; }
    }

    public class ExameClinicoObservacoesRN
    {
        public string ValorObservacoes { get; set; }

        public bool MostraObservacoes { get; set; }
    }

    public class CabecaEPescocoRN
    {
        public string Item { get; set; }

        public bool MostraCabecaEPescocoRN { get; set; }
    }

    public class ApgarRN
    {
        public string PrimeiroSemGrid { get; set; }
        public string QuintoSemGrid { get; set; }
        public string DecimoSemGrid { get; set; }

        public string TituloPrimeiro { get; set; }
        public string TituloQuinto { get; set; }
        public string TituloDecimo { get; set; }
        public string Primeiro { get; set; }
        public string Quinto { get; set; }
        public string Decimo { get; set; }

        public string FCPrimeiroMin0 { get; set; }
        public string FCPrimeiroMin1 { get; set; }
        public string FCPrimeiroMin2 { get; set; }
        public string ERPrimeiroMin0 { get; set; }
        public string ERPrimeiroMin1 { get; set; }
        public string ERPrimeiroMin2 { get; set; }
        public string TMPrimeiroMin0 { get; set; }
        public string TMPrimeiroMin1 { get; set; }
        public string TMPrimeiroMin2 { get; set; }
        public string IRPrimeiroMin0 { get; set; }
        public string IRPrimeiroMin1 { get; set; }
        public string IRPrimeiroMin2 { get; set; }
        public string COPrimeiroMin0 { get; set; }
        public string COPrimeiroMin1 { get; set; }
        public string COPrimeiroMin2 { get; set; }

        public string FCQuintoMin0 { get; set; }
        public string FCQuintoMin1 { get; set; }
        public string FCQuintoMin2 { get; set; }
        public string ERQuintoMin0 { get; set; }
        public string ERQuintoMin1 { get; set; }
        public string ERQuintoMin2 { get; set; }
        public string TMQuintoMin0 { get; set; }
        public string TMQuintoMin1 { get; set; }
        public string TMQuintoMin2 { get; set; }
        public string IRQuintoMin0 { get; set; }
        public string IRQuintoMin1 { get; set; }
        public string IRQuintoMin2 { get; set; }
        public string COQuintoMin0 { get; set; }
        public string COQuintoMin1 { get; set; }
        public string COQuintoMin2 { get; set; }

        public string FCDecimoMin0 { get; set; }
        public string FCDecimoMin1 { get; set; }
        public string FCDecimoMin2 { get; set; }
        public string ERDecimoMin0 { get; set; }
        public string ERDecimoMin1 { get; set; }
        public string ERDecimoMin2 { get; set; }
        public string TMDecimoMin0 { get; set; }
        public string TMDecimoMin1 { get; set; }
        public string TMDecimoMin2 { get; set; }
        public string IRDecimoMin0 { get; set; }
        public string IRDecimoMin1 { get; set; }
        public string IRDecimoMin2 { get; set; }
        public string CODecimoMin0 { get; set; }
        public string CODecimoMin1 { get; set; }
        public string CODecimoMin2 { get; set; }

        public bool MostraApgar { get; set; }
        public bool MostraApgarComGrid { get; set; }
        public bool MostraApgarSemGrid { get; set; }

        public bool temZeroDecimoMin { get; set; }
    }

    public class AtendimentoRecemNascidoObservacoesRN
    {
        public string ValorObservacoes { get; set; }

        public bool MostraObservacoes { get; set; }
    }

    public class IdentificacaoGestacoesAnterioresRN
    {
        public bool MostraIdentificacaoGestacoesAnterioresRN { get; set; }

        public List<IdentificacaoRN> listaIdentificacaoRN { get; set; }
        public List<AnomaliasEmGestacoesAnterioresRN> listaAnomaliasEmGestacoesAnterioresRN { get; set; }
    }

    public class IdentificacaoRN
    {
        public string NomeIdadeECor { get; set; }
        public string Obstetra { get; set; }

        public bool MostraIdentificacao { get; set; }
        public bool MostraNomeIdadeECor { get; set; }
        public bool MostraObstetra { get; set; }
    }

    public class AnomaliasEmGestacoesAnterioresRN
    {
        public string SemIntercorrencias { get; set; }
        public string PrimeiraGestacao { get; set; }

        public bool MostraAnomaliasEmGestacoesAnterioresRN { get; set; }
        public bool MostraSemIntercorrencias { get; set; }
        public bool MostraPrimeiraGestacao { get; set; }

        public List<AnomaliasEmGestacoesAnterioresItemRN> listaAnomaliasEmGestacoesAnterioresItemRN { get; set; }
        public List<AnomaliasGestacoesAnterioresOutrosRN> listaOutrosRN { get; set; }
    }

    public class AnomaliasEmGestacoesAnterioresItemRN
    {
        public string Descricao { get; set; }
        public string Observacoes { get; set; }

        public bool MostraAnomaliasEmGestacoesAnterioresItemRN { get; set; }
        public bool isSemIntercorrencias { get; set; }
        public bool isPrimeiraGestacao { get; set; }
    }

    public class AnomaliasGestacoesAnterioresOutrosRN
    {
        public string Outros { get; set; }

        public bool MostraOutros { get; set; }
    }

    public class GestacaoAtualRN
    {
        public string Gesta { get; set; }
        public string Medicacoes { get; set; }
        public string IdadeGestacional { get; set; }

        public bool MostraGestacaoAtualRN { get; set; }
        public bool MostraGesta { get; set; }
        public bool MostraMedicacoes { get; set; }
        public bool MostraIdadeGestacional { get; set; }

        public List<PatologiasNaGravidezRN> listaPatologiasNaGravidezRN { get; set; }
    }

    public class PatologiasNaGravidezRN
    {
        public string Nenhuma { get; set; }

        public bool MostraPatologiasNaGravidez { get; set; }
        public bool MostraNenhuma { get; set; }

        public List<PatologiasNaGravidezItemRN> listaPatologiasNaGravidezItem { get; set; }
        public List<PatologiasNaGravidezOutrosObservacoes> listaOutrosObservacoes { get; set; }
    }

    public class PatologiasNaGravidezItemRN
    {
        public string Descricao { get; set; }
        public string Observacoes { get; set; }

        public bool MostraPatologiasNaGravidezItemRN { get; set; }
    }

    public class PatologiasNaGravidezOutrosObservacoes
    {
        public string Outros { get; set; }

        public bool MostraOutros { get; set; }
    }

    public class SorologiaExamesRN
    {
        public bool MostraSorologiaExamesRN { get; set; }

        public List<SorologiaExamesItemRN> listaSorologiaExamesItemRN { get; set; }
        public List<SorologiaExamesOutrosObservacoesRN> listaSorologiaExamesOutrosObservacoesRN { get; set; }
    }

    public class SorologiaExamesOutrosObservacoesRN
    {
        public string ValorOutrosObservacoes { get; set; }

        public bool MostraOutrosObservacoes { get; set; }
    }

    public class SorologiaExamesItemRN
    {
        public string SorologiaExames { get; set; }
        public string NegNaoRea { get; set; }
        public string PosRea { get; set; }
        public string NaoDisponivel { get; set; }

        public bool MostraSorologiaExamesItemRN { get; set; }
    }

    public class PartoRN
    {
        public string SituacaoFetal { get; set; }
        public string MembranasAmnioticas { get; set; }
        public string LiquidoAmniotico { get; set; }
        public string Apresentacao { get; set; }
        public string CircularCordao { get; set; }

        public bool MostraParto { get; set; }
        public bool MostraSituacaoFetal { get; set; }
        public bool MostraMembranasAmnioticas { get; set; }
        public bool MostraLiquidoAmniotico { get; set; }
        public bool MostraApresentacao { get; set; }
        public bool MostraCircularCordao { get; set; }

        public List<PartoTipoDePartoRN> listaPartoTipoDePartoRN { get; set; }
        public List<PartoTipoDeAnestesiaRN> listaPartoTipoDeAnestesiaRN { get; set; }
        public List<PartoTipoDePartoObservacoes> listaPartoTipoDePartoObservacoes { get; set; }
    }

    public class PartoTipoDePartoObservacoes
    {
        public string Observacao { get; set; }

        public bool MostraObservacoes { get; set; }
    }

    public class PartoTipoDePartoRN
    {
        public string TipoDeParto { get; set; }

        public bool MostraPartoTipoDeParto { get; set; }
    }

    public class PartoTipoDeAnestesiaRN
    {
        public string TipoDeAnestesia { get; set; }

        public bool MostraPartoTipoDeAnestesia { get; set; }
    }

    public class RespiratorioRN
    {
        public string Item { get; set; }

        public bool MostraRespiratorioRN { get; set; }
    }

    public class CardioVascularRN
    {
        public string Item { get; set; }

        public bool MostraCardioVascularRN { get; set; }
    }

    public class OutrosRN
    {
        public string Item { get; set; }

        public bool MostraOutrosRN { get; set; }
    }

    public class OsteoArticularRN
    {
        public string Item { get; set; }

        public bool MostraOsteoArticularRN { get; set; }
    }
}
