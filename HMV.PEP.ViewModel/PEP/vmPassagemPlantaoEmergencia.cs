using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Repository.PEP.PassagemPlantaoE;
using HMV.Core.Domain.Views.PEP;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Helper;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.ServicePassagemPlantao;
using SignalR.Client.Hubs;
using StructureMap;
using System.IO;
using System.Text;
using System.Windows.Markup;
using System.Diagnostics;
using HMV.Core.Framework.WPF;
using System.Windows;

namespace HMV.PEP.ViewModel
{
    public class vmPassagemPlantaoEmergencia : ViewModelBase
    {
        const string cNAOATUALIZA = "#$NAOATUALIZA$#";

        #region Contrutor
        public vmPassagemPlantaoEmergencia(Usuarios pUsuarios, IList<vPacienteInternado> pListaInternados, IList<vEmergenciaPEP> pListaUrgencia, bool pPediatrico)
        {
            _usuario = pUsuarios;
            this._isNurse = _usuario.Prestador.IsNurse;

            IList<PassagemPlantaoEmergenciaLocalPaciente> listurgencia = new List<PassagemPlantaoEmergenciaLocalPaciente>();
            IList<vEmergenciaPEP> listurgenciapaciente = new List<vEmergenciaPEP>();
            
            IRepositorioDePassagemPlantaoLocalPaciente rep = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoLocalPaciente>();
            rep.OndeDataInclusaoMaior(DateTime.Now.Date.AddDays(-7));
            _listlocalpaciente = rep.List().ToList();

            if (_listlocalpaciente.IsNotNull())
            {
                List<PacienteEmergencia> listpac = _listlocalpaciente.Where(x => x.PacienteEmergencia.IsNotNull()).DistinctBy(x => x.PacienteEmergencia.Id).Select(x => x.PacienteEmergencia).ToList();
                List<PassagemPlantaoEmergenciaLocalPaciente> listpassagemaux = new List<PassagemPlantaoEmergenciaLocalPaciente>();

                foreach (var item in listpac)
                {
                    var passplantao = _listlocalpaciente.Where(x => x.PacienteEmergencia.IsNotNull() && x.PacienteEmergencia.Id == item.Id).OrderBy(x => x.ID).FirstOrDefault();
                    listpassagemaux.Add(passplantao);
                }

                listurgencia = listpassagemaux.Where(x => pListaUrgencia.Count(y => x.PacienteEmergencia.BoletimDeEmergencia.IsNotNull()
                    && y.IDBoletim == x.PacienteEmergencia.BoletimDeEmergencia.Id 
                    && x.PacienteEmergencia.BoletimDeEmergencia.DataAlta.IsNull()) > 0
                    && x.PassagemPlantaoEmergenciaLocalizacao.PassagemPlantao == SimNao.Sim).ToList();

                if (pPediatrico)
                    listurgenciapaciente = pListaUrgencia.Where(x => listurgencia.Count(y => y.PacienteEmergencia.BoletimDeEmergencia.Id == x.IDBoletim) == 0
                                                            && (x.Cor == "Amarelo" || x.Cor == "Laranja" || x.Cor == "Vermelho" || x.Cor == "Verde")
                                                            && x.DataHoraAltaBoletim.IsNull()).ToList(); //x.HoraAtendimento.AddHours(12) >= DateTime.Now
                else
                    listurgenciapaciente = pListaUrgencia.Where(x => listurgencia.Count(y => y.PacienteEmergencia.BoletimDeEmergencia.Id == x.IDBoletim) == 0
                                                            && (x.Cor == "Amarelo" || x.Cor == "Laranja" || x.Cor == "Vermelho")
                                                            && x.DataHoraAltaBoletim.IsNull()).ToList(); //x.HoraAtendimento.AddHours(12) >= DateTime.Now
            }



            _atendimentos = pListaInternados.Select(x => x.Atendimento).ToList();
            _atendimentos.AddRange(listurgencia.Select(x => x.Atendimento.ID).ToList());
            _atendimentos.AddRange(listurgenciapaciente.Select(x => x.Atendimento).ToList());
            if (_atendimentos.HasItems())
            {
                IRepositorioDePassagemPlantaoEmergencia rep2 = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoEmergencia>();
                _listapassagemplantao = rep2.OndeIdAtendimentoIn(_atendimentos).List();
            }
            _listaEvolucoes = new ObservableCollection<EvolucaoEmergencia>();
            CarregaEvolucoes(pListaInternados, listurgencia, listurgenciapaciente);

            //            
            connection = new HubConnection(ConfigurationManager.AppSettings["ServerSignalR"]);
            myHub = connection.CreateProxy("SignalRChat.Hubs.Chat");
            myHub.On("atualizapassagem", data => Atualiza(data));
            myHub.On("adicionaMensagem", data => AtualizaObs(data));
            try
            {
                connection.Start().Wait(TimeSpan.FromSeconds(30));
            }
            catch (AggregateException ae)
            {
                DXMessageBox.Show("Erro na Passagem de Plantão." + Environment.NewLine  + ae.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                throw; 
            }
            //

            if (_listaEvolucoesAux.IsNotNull())
                _evolucaoselecionada = _listaEvolucoesAux.FirstOrDefault();

            _pediatrico = pPediatrico;
            CarregaHistoricoObservacao();
        }
        #endregion

        #region Propriedades Publicas       
        public bool PodeEditar
        {
            get
            {
                if (_usuario.Prestador.IsCorpoClinico || this._isNurse)
                    return true;
                return false;
            }
        }

        public string ObservacaoEvolucao
        {
            get
            {
                return _observacaoevolucao;
            }
            set
            {
                if (_observacaoevolucao != value)
                {
                    if (_evolucaoselecionada.IsNull())
                        if (_listaEvolucoes.HasItems())
                            _evolucaoselecionada = _listaEvolucoes.FirstOrDefault();
                    SalvaObs(value);
                    EnviaDadosObs(value);
                    _observacaoevolucao = value;
                }
                OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.ObservacaoEvolucao);
            }
        }

        public ObservableCollection<EvolucaoEmergencia> ListaEvolucoes
        {
            get
            {
                return _listaEvolucoesAux;
            }
        }

        public ObservableCollection<EvolucaoEmergenciaImpressao> ListaEvolucoesImpressao
        {
            get
            {
                var retorno = new List<EvolucaoEmergenciaImpressao>();
                var ret = new List<EvolucaoEmergencia>(_listaEvolucoesAux.Where(x => x.Imprime == true));
                ret.Each(x =>
                {
                    var novo = new EvolucaoEmergenciaImpressao();
                    novo.Alergias = x.Alergias;
                    novo.Atendimento = x.Atendimento;
                    novo.AvaliacaoRisco = x.AvaliacaoRisco;
                    novo.CorTeste = x.CorTeste;
                    novo.DataNascimento = x.DataNascimento;
                    novo.Idade = x.Idade;
                    novo.IsInternado = x.IsInternado;
                    novo.IsUltimos = x.IsUltimos;
                    novo.Leito = x.Leito;
                    novo.Localizacao = x.Localizacao;
                    novo.Medico = x.Medico;
                    novo.NomePaciente = x.NomePaciente;
                    if (x.ListaProblemas.IsNotEmpty())
                    {
                        if (IsValidXAML(x.ListaProblemas))
                        {
                            var stream = new MemoryStream(Encoding.UTF8.GetBytes(x.ListaProblemas));
                            var doc = (FlowDocument)XamlReader.Load(stream);
                            var rt = new RichTextBox();
                            rt.Document = doc;
                            novo.ListaProblemas = RemoveXmlInvalidCharacters(StringFromRichTextBox(rt));
                        }
                        else
                            novo.ListaProblemas = RemoveXmlInvalidCharacters(x.ListaProblemas);
                    }
                    //
                    if (x.Plano.IsNotEmpty())
                    {
                        if (IsValidXAML(x.Plano))
                        {
                            var stream = new MemoryStream(Encoding.UTF8.GetBytes(x.Plano));
                            var doc = (FlowDocument)XamlReader.Load(stream);
                            var rt = new RichTextBox();
                            rt.Document = doc;
                            novo.Plano = RemoveXmlInvalidCharacters(StringFromRichTextBox(rt));
                        }
                        else
                            novo.Plano = RemoveXmlInvalidCharacters(x.Plano);
                    }
                    //
                    if (x.Enfermagem.IsNotEmpty())
                    {
                        if (IsValidXAML(x.Enfermagem))
                        {
                            var stream = new MemoryStream(Encoding.UTF8.GetBytes(x.Enfermagem));
                            var doc = (FlowDocument)XamlReader.Load(stream);
                            var rt = new RichTextBox();
                            rt.Document = doc;
                            novo.Enfermagem = RemoveXmlInvalidCharacters(StringFromRichTextBox(rt));
                        }
                        else
                            novo.Enfermagem = RemoveXmlInvalidCharacters(x.Enfermagem);
                    }
                    retorno.Add(novo);
                });
                return new ObservableCollection<EvolucaoEmergenciaImpressao>(retorno);
            }
        }        
        //bool controlasave;
        public EvolucaoEmergencia EvolucaoSelecionada
        {
            get
            {
                return _evolucaoselecionada;
            }
            set
            {
                if (_evolucaoselecionada != value)
                {
                    SalvaPassagem(_evolucaoselecionada);
                    //controlasave = true;
                    EnviaDados(_evolucaoselecionada, false);
                    //TimedAction.ExecuteWithDelay((Action)(() => controlasave = false), TimeSpan.FromSeconds(1));
                    _evolucaoselecionada = value;
                }
                OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.EvolucaoSelecionada);
            }
        }

        public string PesquisaLeito
        {
            get
            {
                return _pesquisaLeito;
            }
            set
            {
                _pesquisaLeito = value;
                AtualizaLista();
                OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.PesquisaLeito);
            }
        }

        public string PesquisaNome
        {
            get
            {
                return _pesquisaNome;
            }
            set
            {
                _pesquisaNome = value;
                AtualizaLista();
                OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.PesquisaNome);
            }
        }

        public bool MostraEnfermagem
        {
            get
            {
                return this._isNurse;
            }
        }

        public IList<PassagemPlantaoEmergencia> ListaHistorico
        {
            get
            {
                return _listahistorico;
            }
        }

        public IList<EvolucaoEmergenciaObservacao> ListaHistoricoObservacao
        {
            get
            {
                return _listahistoricoobservacao;
            }
        }

        public PassagemPlantaoEmergencia HistoricoSelecionado
        {
            get
            {
                return _historicoselecionado;
            }
            set
            {
                this._historicoselecionado = value;
                OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.HistoricoSelecionado);
            }
        }

        public EvolucaoEmergenciaObservacao HistoricoObservacaoSelecionado
        {
            get
            {
                return _historicoselecionadoobservacao;
            }
            set
            {
                this._historicoselecionadoobservacao = value;
                OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.HistoricoObservacaoSelecionado);
                OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.IsEnableHistoricoObs);
            }
        }

        public bool MarcarTodos
        {
            get
            {
                return _marcartodos;
            }
            set
            {
                _marcartodos = value;
                MarcaTodos();
                OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.MarcarTodos);
            }
        }

        public wrpPassagemPlantaoEmergenciaLocalizacaoCollection LeitosVagos
        {
            get
            {
                return _leitosvagos;
            }
        }

        public wrpPassagemPlantaoEmergenciaLocalizacao LeitoSelecionado
        {
            get
            {
                return _leitosvagoselecionado;
            }
            set
            {
                _leitosvagoselecionado = value;
                OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.LeitoSelecionado);
            }
        }

        public bool IsEnableHistoricoObs
        {
            get
            {
                return _historicoselecionadoobservacao.IsNotNull();
            }
        }
        #endregion

        #region Metodos Privados
        private static string RemoveXmlInvalidCharacters(string s)
        {
            if (s.IsEmptyOrWhiteSpace()) return string.Empty;

            return Regex.Replace(
                s,
                @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD\u10000-\u10FFFF]",
                string.Empty);
        }

        private void AtualizaLista()
        {
            //Memory.MinimizeMemory();
            this._listaEvolucoesAux = _listaEvolucoes;
            if (_pesquisaLeito.IsNotEmptyOrWhiteSpace() && _pesquisaNome.IsNotEmptyOrWhiteSpace())
                this._listaEvolucoesAux = new ObservableCollection<EvolucaoEmergencia>(this._listaEvolucoesAux.Where(x => x.Leito.Contains(_pesquisaLeito)
                                                                                        && x.NomePaciente.Contains(_pesquisaNome)).OrderBy(x => x.Ordem));
            else if (_pesquisaLeito.IsNotEmptyOrWhiteSpace() && _pesquisaNome.IsEmpty())
                this._listaEvolucoesAux = new ObservableCollection<EvolucaoEmergencia>(this._listaEvolucoesAux.Where(x => x.Leito.Contains(_pesquisaLeito)).OrderBy(x => x.Ordem));

            else if (_pesquisaNome.IsNotEmptyOrWhiteSpace() && _pesquisaLeito.IsEmpty())
                this._listaEvolucoesAux = new ObservableCollection<EvolucaoEmergencia>(this._listaEvolucoesAux.Where(x => x.NomePaciente.Contains(_pesquisaNome)).OrderBy(x => x.Ordem));

            else
                this._listaEvolucoesAux = new ObservableCollection<EvolucaoEmergencia>(this._listaEvolucoesAux.OrderBy(x => x.Ordem));

            Memory.MinimizeMemory();
            OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.ListaEvolucoes);
        }

        private PassagemPlantaoEmergenciaLocalizacao BuscaLocalizacao(int pidAtendimento)
        {
            if (!_listlocalpaciente.HasItems())
                return null;

            var ret = _listlocalpaciente.Where(x => x.Atendimento.IsNotNull() && x.Atendimento.ID == pidAtendimento).ToList();
            if (ret.HasItems())
            {
                return ret.OrderByDescending(x => x.ID).FirstOrDefault().PassagemPlantaoEmergenciaLocalizacao;
            }
            else
                return null;
        }

        private string BuscaListaProblemas(int pidAtendimento) //, bool pIsInternado
        {
            if (!_listapassagemplantao.HasItems())
                return string.Empty;

            var ret = _listapassagemplantao.Where(x => x.Atendimento.ID == pidAtendimento).ToList();
            if (ret.HasItems())
            {
                return ret.OrderByDescending(x => x.Data).FirstOrDefault().ListaProblema;
            }
            else
                return string.Empty;
        }

        private string BuscaPlano(int pidAtendimento)//, bool pIsInternado
        {
            if (!_listapassagemplantao.HasItems())
                return string.Empty;

            var ret = _listapassagemplantao.Where(x => x.Atendimento.ID == pidAtendimento).ToList();
            if (ret.HasItems())
            {
                return ret.OrderByDescending(x => x.Data).FirstOrDefault().Plano;
            }
            else
                return string.Empty;
        }

        private string BuscaEnfermagem(int pidAtendimento) //, bool pIsInternado
        {           
            if (!_listapassagemplantao.HasItems())
                return string.Empty;

            var ret = _listapassagemplantao.Where(x => x.Atendimento.ID == pidAtendimento).ToList();

            if (ret.HasItems())
            {
                return ret.OrderByDescending(x => x.Data).FirstOrDefault().Enfermagem;
            }
            else
            {
                IRepositorioDeParametrosClinicas repp = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                Parametro _parametro = repp.OndePEPPadraoEnfermagemPassagemPlantao().Single();

                return _parametro.Valor;
            }
        }

        private string BuscaAlergias(PassagemPlantaoEmergenciaLocalPaciente pItem)
        {
            if (pItem.PacienteEmergencia.BoletimDeEmergencia.Atendimento.Paciente.Alergias.HasItems())
            {
                return string.Join(",", pItem.PacienteEmergencia.BoletimDeEmergencia.Atendimento.Paciente.Alergias.Select(x => x.Agente).ToList());
            }
            return string.Empty;
        }

        private string BuscaMessagemAvaliacaoRisco(int pidAtendimento)
        {
            string ret = string.Empty;
            //string fixo = "( )AD ( )NT ( )PE ( )CI+CA+PC ( )Banho";
            if (this._isNurse)
            {
                IRepositorioDeAtendimento repAte = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
                Atendimento atend = repAte.OndeCodigoAtendimentoIgual(pidAtendimento).Single();
                ret = atend.MensagemAvaliacaoDeRisco(true);
            }
            //return ret.IsNotEmpty() ? ret + Environment.NewLine + fixo : fixo;
            return ret;
        }

        private void CarregaEvolucoes(IList<vPacienteInternado> pListaInternados, IList<PassagemPlantaoEmergenciaLocalPaciente> pListaUrgencia, IList<vEmergenciaPEP> pListurgenciaPacienteClassificacao)
        {
            if (_listaEvolucoes.IsNotNull())
                _listaEvolucoes.Clear();
            if (_listaEvolucoesAux.IsNotNull())
                _listaEvolucoesAux.Clear();

            if (pListaUrgencia.HasItems())
                foreach (var item in pListaUrgencia.OrderBy(x => x.PassagemPlantaoEmergenciaLocalizacao.Ordem))
                {
                    _listaEvolucoes.Add(new EvolucaoEmergencia(this)
                    {
                        Atendimento = item.PacienteEmergencia.BoletimDeEmergencia.Atendimento.ID,
                        DataNascimento = item.PacienteEmergencia.BoletimDeEmergencia.Atendimento.Paciente.DataNascimento.HasValue ?
                                         item.PacienteEmergencia.BoletimDeEmergencia.Atendimento.Paciente.DataNascimento.Value.ToShortDateString() : string.Empty,
                        Idade = item.PacienteEmergencia.BoletimDeEmergencia.Atendimento.Paciente.Idade.ToString(2),
                        Medico = item.PacienteEmergencia.BoletimDeEmergencia.Atendimento.Prestador.NomeExibicao,
                        NomePaciente = item.PacienteEmergencia.BoletimDeEmergencia.Atendimento.Paciente.Nome,
                        ListaProblemas = BuscaListaProblemas(item.PacienteEmergencia.BoletimDeEmergencia.Atendimento.ID),
                        Plano = BuscaPlano(item.PacienteEmergencia.BoletimDeEmergencia.Atendimento.ID),
                        Enfermagem = BuscaEnfermagem(item.PacienteEmergencia.BoletimDeEmergencia.Atendimento.ID),
                        Alergias = BuscaAlergias(item),
                        Localizacao = item.PassagemPlantaoEmergenciaLocalizacao,
                        IsInternado = false,
                        AvaliacaoRisco = item.Atendimento.MensagemAvaliacaoDeRisco(true),
                        AlterouEnfermagem = false,
                        AlterouLista = false,
                        AlterouPlano = false
                    });
                }

            if (pListaInternados.HasItems())
                foreach (var item in pListaInternados.OrderBy(x => x.Data))
                {
                    _listaEvolucoes.Add(new EvolucaoEmergencia(this)
                    {
                        Atendimento = item.Atendimento,
                        Idade = item.Idade,
                        DataAtendimento = item.Data.ToString("dd/MM/yyyy"),
                        DataNascimento = item.DataNascimento.ToShortDateString(),
                        Medico = item.MedicoAssistente,
                        NomePaciente = item.Paciente,
                        ListaProblemas = BuscaListaProblemas(item.Atendimento),
                        Plano = BuscaPlano(item.Atendimento),
                        Enfermagem = BuscaEnfermagem(item.Atendimento),
                        Alergias = item.Alergias,
                        Localizacao = BuscaLocalizacao(item.Atendimento),
                        IsInternado = true,
                        AvaliacaoRisco = BuscaMessagemAvaliacaoRisco(item.Atendimento),
                        AlterouEnfermagem = false,
                        AlterouLista = false,
                        AlterouPlano = false
                    });
                }

            if (pListurgenciaPacienteClassificacao.HasItems())
                foreach (var item in pListurgenciaPacienteClassificacao.OrderBy(x => x.DataClassificacaoRisco))
                {
                    _listaEvolucoes.Add(new EvolucaoEmergencia(this)
                    {
                        Atendimento = item.Atendimento,
                        Idade = item.Idade.ToString(),
                        DataNascimento = item.DataNascimento.HasValue ?
                                         item.DataNascimento.Value.ToShortDateString() : string.Empty,
                        Medico = item.Prestador,
                        NomePaciente = item.Paciente,
                        ListaProblemas = BuscaListaProblemas(item.Atendimento),
                        Plano = BuscaPlano(item.Atendimento),
                        Enfermagem = BuscaEnfermagem(item.Atendimento),
                        Localizacao = BuscaLocalizacao(item.Atendimento),
                        IsInternado = false,
                        IsUltimos = true,
                        AvaliacaoRisco = BuscaMessagemAvaliacaoRisco(item.Atendimento),
                        AlterouEnfermagem = false,
                        AlterouLista = false,
                        AlterouPlano = false
                    });
                }

            if (!MostraEnfermagem)
            {
                IRepositorioDeParametrosClinicas repp = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                Parametro _parametro = repp.OndeCorNaoImprimirPassagemPlantao().Single();

                IList<int> codigos = _parametro.Valor.Split(',').Select(x => int.Parse(x)).ToList();

                _listaEvolucoes.Each(x => { if (x.Localizacao.IsNotNull() && !codigos.Contains(x.Localizacao.Cor.Id)) x.Imprime = true; else x.Imprime = false; });
            }

            AtualizaLista();
        }

        private void Atualiza(dynamic pData)
        {
            try
            {
                if (_usuario.cd_usuario == (string)pData.Usuario)
                    return;

                PassagemPlantaoEmergenciaLocalizacao local = null;
                var atend = (string)pData.Atendimento;
                var evolucao = _listaEvolucoes.Where(x => x.Atendimento == int.Parse(atend)).FirstOrDefault();
                if (evolucao.IsNotNull())
                {
                    //Salva atual antes de atualizar
                    if (evolucao.ListaProblemas != (string)pData.ListaProblemas || evolucao.Plano != (string)pData.Plano || evolucao.Enfermagem != (string)pData.Enfermagem)//&& !controlasave
                        SalvaPassagem(evolucao);

                    if (pData.Enfermagem != cNAOATUALIZA)
                        evolucao.Enfermagem = pData.Enfermagem;
                    if (pData.ListaProblemas != cNAOATUALIZA)
                        evolucao.ListaProblemas = pData.ListaProblemas;
                    if (pData.Plano != cNAOATUALIZA)
                        evolucao.Plano = pData.Plano;

                    if (int.Parse((string)pData.IdLocalPaciente) > 0)
                    {
                        IRepositorioDePassagemPlantaoLocalizacao repL = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoLocalizacao>();
                        local = repL.OndeIdIgual(int.Parse((string)pData.IdLocalPaciente)).Single();
                        evolucao.Localizacao = local;
                    }

                    if (_listaEvolucoesAux.HasItems())
                    {
                        evolucao = _listaEvolucoesAux.Where(x => x.Atendimento == int.Parse(atend)).FirstOrDefault();
                        if (evolucao.IsNotNull())
                        {
                            if (pData.Enfermagem != cNAOATUALIZA)
                                evolucao.Enfermagem = pData.Enfermagem;
                            if (pData.ListaProblemas != cNAOATUALIZA)
                                evolucao.ListaProblemas = pData.ListaProblemas;
                            if (pData.Plano != cNAOATUALIZA)
                                evolucao.Plano = pData.Plano;
                            if (int.Parse((string)pData.IdLocalPaciente) > 0)
                                evolucao.Localizacao = local;
                        }
                    }

                    List<int> atendimentos = _listaEvolucoes.Select(x => x.Atendimento).ToList();
                    IRepositorioDePassagemPlantaoEmergencia rep2 = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoEmergencia>();
                    _listapassagemplantao = rep2.OndeIdAtendimentoIn(atendimentos).List();

                    if ((string)pData.AtualizaLista == "S")
                        AtualizaLista();

                    Memory.MinimizeMemory();
                }
            }
            catch (AggregateException ae)
            {
                DXMessageBox.Show("Erro na Passagem de Plantão." + Environment.NewLine + ae.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void AtualizaObs(dynamic pData)
        {
            try
            {
                int _obsID = int.Parse((string)pData.nome);
                string _obs = (string)pData.mensagem;

                if (_obs.IsNotEmptyOrWhiteSpace())
                {
                    //Salva atual antes de atualizar
                    if (_observacaoevolucao != _obs)
                        SalvaObs(_observacaoevolucao);
                    _observacaoevolucaoID = _obsID;
                    _observacaoevolucao = _obs;
                }
                //sempre da refresh na propriedade
                OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.ObservacaoEvolucao);

                Memory.MinimizeMemory();
            }
            catch (AggregateException ae)
            {
                DXMessageBox.Show("Erro na Passagem de Plantão." + Environment.NewLine + ae.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void CopiaHistorico()
        {
            _evolucaoselecionada.ListaProblemas = _historicoselecionado.ListaProblema;
            _evolucaoselecionada.Plano = _historicoselecionado.Plano;
            _evolucaoselecionada.Enfermagem = _historicoselecionado.Enfermagem;

            SalvaPassagem(_evolucaoselecionada);
            EnviaDados(_evolucaoselecionada, false);
            AtualizaLista();
        }

        [DebuggerHidden]
        private bool IsValidXAML(string pxaml)
        {
            try
            {
                var test = XamlReader.Parse(pxaml);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;
        }        
        #endregion

        #region Propriedades Privadas
        private HubConnection connection;
        private IHubProxy myHub;
        private ObservableCollection<EvolucaoEmergencia> _listaEvolucoes;
        private ObservableCollection<EvolucaoEmergencia> _listaEvolucoesAux;
        private EvolucaoEmergencia _evolucaoselecionada;
        private Usuarios _usuario;
        private IList<PassagemPlantaoEmergencia> _listapassagemplantao;
        private IList<PassagemPlantaoEmergencia> _listahistorico;
        private IList<EvolucaoEmergenciaObservacao> _listahistoricoobservacao;
        private PassagemPlantaoEmergencia _historicoselecionado;
        private EvolucaoEmergenciaObservacao _historicoselecionadoobservacao;
        private string _pesquisaLeito;
        private string _pesquisaNome;
        //private bool _liberatimer;
        private bool _marcartodos;
        private bool _pediatrico;
        private wrpPassagemPlantaoEmergenciaLocalizacaoCollection _leitosvagos;
        private wrpPassagemPlantaoEmergenciaLocalizacao _leitosvagoselecionado;
        private IList<PassagemPlantaoEmergenciaLocalPaciente> _listlocalpaciente;
        public event EventHandler ExecuteMarcaTodos;
        private int _observacaoevolucaoID;
        private string _observacaoevolucao;
        private bool _isNurse;
        private List<int> _atendimentos;
        #endregion

        #region Metodos Publicos
        public void SalvaPassagem(EvolucaoEmergencia pPassagem)
        {
            if (pPassagem.IsNotNull())
                if (pPassagem.Enfermagem.IsNotEmptyOrWhiteSpace() || pPassagem.Plano.IsNotEmptyOrWhiteSpace() || pPassagem.ListaProblemas.IsNotEmptyOrWhiteSpace())
                    if (pPassagem.AlterouPlano || pPassagem.AlterouLista || pPassagem.AlterouEnfermagem)
                    {
                        IRepositorioDePassagemPlantaoEmergencia rep = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoEmergencia>();
                        IRepositorioDeAtendimento repAte = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();

                        Atendimento atend = repAte.OndeCodigoAtendimentoIgual(pPassagem.Atendimento).Single();

                        PassagemPlantaoEmergencia novo = new PassagemPlantaoEmergencia(atend, _usuario);
                        novo.Plano = pPassagem.Plano;
                        novo.ListaProblema = pPassagem.ListaProblemas;
                        novo.Enfermagem = pPassagem.Enfermagem;
                        rep.Save(novo);
                        Memory.MinimizeMemory();
                    }
        }

        public void SalvaObs(string pObs)
        {
            if (pObs.IsNotEmptyOrWhiteSpace())
            {
                IRepositorioDePassagemPlantaoEmergenciaObservacao rep = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoEmergenciaObservacao>();
                PassagemPlantaoEmergenciaObservacao novo = new PassagemPlantaoEmergenciaObservacao(_usuario);
                novo.Observacao = pObs;
                rep.Save(novo);

                rep.Refresh(novo);
                _observacaoevolucaoID = novo.ID;
                Memory.MinimizeMemory();
            }
        }

        public void EnviaDados(EvolucaoEmergencia pPassagem, bool pAtualizaLocalizacao)
        {
            try
            {
                if (_evolucaoselecionada.IsNotNull())
                {

                    int idlocal = 0;
                    string user = _usuario.cd_usuario;
                    if (pAtualizaLocalizacao)
                        if (_evolucaoselecionada.Localizacao.IsNotNull())
                            idlocal = _evolucaoselecionada.Localizacao.ID;

                    //System.Net.ServicePointManager.DefaultConnectionLimit = 100;
                    if (this._isNurse)
                        using (var servico = new PassagemPlantaoEmergenciaClient())
                        {
                            servico.Open();
                            if (_evolucaoselecionada.AlterouEnfermagem)
                                servico.AtualizaPassagem(_evolucaoselecionada.Atendimento, cNAOATUALIZA, cNAOATUALIZA
                                                        , _evolucaoselecionada.Enfermagem, idlocal, user, pAtualizaLocalizacao);
                            else
                                servico.AtualizaPassagem(_evolucaoselecionada.Atendimento, cNAOATUALIZA, cNAOATUALIZA
                                                        , cNAOATUALIZA, idlocal, user, pAtualizaLocalizacao);
                            servico.Close();
                        }
                    else
                        using (var servico = new PassagemPlantaoEmergenciaClient())
                        {
                            servico.Open();
                            if (_evolucaoselecionada.AlterouLista && _evolucaoselecionada.AlterouPlano)
                                servico.AtualizaPassagem(_evolucaoselecionada.Atendimento, _evolucaoselecionada.Plano, _evolucaoselecionada.ListaProblemas
                                                    , cNAOATUALIZA, idlocal, user, pAtualizaLocalizacao);
                            else if (_evolucaoselecionada.AlterouLista)
                                servico.AtualizaPassagem(_evolucaoselecionada.Atendimento, cNAOATUALIZA, _evolucaoselecionada.ListaProblemas
                                                    , cNAOATUALIZA, idlocal, user, pAtualizaLocalizacao);
                            else if (_evolucaoselecionada.AlterouPlano)
                                servico.AtualizaPassagem(_evolucaoselecionada.Atendimento, _evolucaoselecionada.Plano, cNAOATUALIZA
                                                    , cNAOATUALIZA, idlocal, user, pAtualizaLocalizacao);
                            servico.Close();
                        }

                    _evolucaoselecionada.AlterouEnfermagem = false;
                    _evolucaoselecionada.AlterouLista = false;
                    _evolucaoselecionada.AlterouPlano = false;

                    Memory.MinimizeMemory();
                }
            }
             catch (AggregateException ae)
            {
                DXMessageBox.Show("Erro na Passagem de Plantão." + Environment.NewLine  + ae.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                throw; 
            }
        }

        public void EnviaDadosObs(string pObs)
        {
            try
            {
                if (_observacaoevolucao.IsNotEmptyOrWhiteSpace())
                {
                    using (var servico = new PassagemPlantaoEmergenciaClient())
                    {
                        servico.Open();
                        servico.AdicionarMensagem(pObs, _observacaoevolucaoID.ToString());
                        servico.Close();
                    }
                    Memory.MinimizeMemory();
                }
            }
            catch (AggregateException ae)
            {
                DXMessageBox.Show("Erro na Passagem de Plantão." + Environment.NewLine + ae.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public void CarregaHistorico()
        {
            _historicoselecionado = null;
            _listahistorico = null;                       

            IRepositorioDePassagemPlantaoEmergencia rep2 = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoEmergencia>();
            _listapassagemplantao = rep2.OndeIdAtendimentoIn(_atendimentos).List();

            if (this._isNurse)
                _listahistorico = _listapassagemplantao.Where(x => x.Atendimento.ID == _evolucaoselecionada.Atendimento && x.Usuario.Prestador.IsNurse).OrderByDescending(x => x.Data).ToList();
            else
                _listahistorico = _listapassagemplantao.Where(x => x.Atendimento.ID == _evolucaoselecionada.Atendimento && x.Usuario.Prestador.IsNotNurse).OrderByDescending(x => x.Data).ToList();
            
            IRepositorioDeAtendimento repAte = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento atend = repAte.OndeCodigoAtendimentoIgual(_evolucaoselecionada.Atendimento).Single();
            repAte.Refresh(atend);
            var ultatend = atend.Paciente.Atendimentos.Where(x => x.ID < atend.ID && x.DataAlta.IsNotNull()).OrderByDescending(x => x.DataHoraAtendimento).FirstOrDefault();
            if (ultatend.IsNotNull())
            {
                rep2 = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoEmergencia>();
                if (this._isNurse)
                {
                    var list = rep2.OndeIdAtendimentoIgual(ultatend.ID).List().Where(x => x.Usuario.Prestador.IsNurse).OrderByDescending(x => x.Data).ToList();
                    _listahistorico = _listahistorico.Concat(list).ToList();
                }
                else
                {
                    var list = rep2.OndeIdAtendimentoIgual(ultatend.ID).List().Where(x => x.Usuario.Prestador.IsNotNurse).OrderByDescending(x => x.Data).ToList();
                    _listahistorico = _listahistorico.Concat(list).ToList();
                }
            }
        }

        public void CarregaHistoricoObservacao()
        {
            IRepositorioDePassagemPlantaoEmergenciaObservacao repobs = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoEmergenciaObservacao>();
            var obs = repobs.OrdenaPorIDDesc().List().FirstOrDefault();
            if (obs.IsNotNull())
            {
                _observacaoevolucaoID = obs.ID;
                _observacaoevolucao = obs.Observacao;
            }
            _listahistoricoobservacao = new List<EvolucaoEmergenciaObservacao>();
            repobs.OrdenaPorIDDesc().List().Each(x => _listahistoricoobservacao.Add(new EvolucaoEmergenciaObservacao { ID = x.ID, Data = x.Data, Observacao = x.Observacao, Usuario = x.Usuario.NomeExibicao }));
        }

        public void CarregaLeitosVagos()
        {
            List<PassagemPlantaoEmergenciaLocalizacao> _leitos = new List<PassagemPlantaoEmergenciaLocalizacao>();
            IRepositorioDePassagemPlantaoLocalizacao rep = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoLocalizacao>();
            var listleitos = rep.List();
            var localizacoesatuais = _listaEvolucoes.Where(x => x.Localizacao.IsNotNull()).Select(x => x.Localizacao).ToList();
            foreach (var leito in listleitos.OrderBy(x => x.Ordem))
            {
                var ocupado = localizacoesatuais.Where(x => x.ID == leito.ID).FirstOrDefault();
                if (ocupado.IsNotNull())
                    continue;
                _leitos.Add(leito);
            }

            _leitosvagos = new wrpPassagemPlantaoEmergenciaLocalizacaoCollection(_leitos);
            OnPropertyChanged<vmPassagemPlantaoEmergencia>(x => x.LeitosVagos);
        }

        public void AlteraLeito()
        {
            IRepositorioDePassagemPlantaoLocalPaciente rep = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoLocalPaciente>();

            _evolucaoselecionada.Localizacao = _leitosvagoselecionado.DomainObject;
            var novo = new PassagemPlantaoEmergenciaLocalPaciente(_usuario, _evolucaoselecionada.Localizacao);
            if (_evolucaoselecionada.IsInternado || (_evolucaoselecionada.IsUltimos && !_evolucaoselecionada.IsInternado))
            {
                IRepositorioDeAtendimento repAte = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
                Atendimento atend = repAte.OndeCodigoAtendimentoIgual(_evolucaoselecionada.Atendimento).Single();
                novo.Atendimento = atend;
            }
            else if (!_evolucaoselecionada.IsUltimos)
            {
                var pacloc = _listlocalpaciente.Where(x => x.PacienteEmergencia.IsNotNull()
                                                    && x.PacienteEmergencia.BoletimDeEmergencia.IsNotNull()
                                                    && x.PacienteEmergencia.BoletimDeEmergencia.Atendimento.ID == _evolucaoselecionada.Atendimento)
                                                    .OrderBy(y => y.ID).FirstOrDefault();
                novo.PacienteEmergencia = pacloc.PacienteEmergencia;
            }

            rep.Save(novo);
            SalvaPassagem(_evolucaoselecionada);
            EnviaDados(_evolucaoselecionada, true);
            //AtualizaLista();
        }

        public void Atualiza(IList<vPacienteInternado> pListaInternados, IList<vEmergenciaPEP> pListaUrgencia)
        {
            IList<PassagemPlantaoEmergenciaLocalPaciente> listurgencia = new List<PassagemPlantaoEmergenciaLocalPaciente>();
            IList<vEmergenciaPEP> listurgenciapaciente = new List<vEmergenciaPEP>();
            IRepositorioDePassagemPlantaoLocalPaciente rep = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoLocalPaciente>();
            rep.OndeDataInclusaoMaior(DateTime.Now.Date.AddDays(-7));
            _listlocalpaciente = rep.List().ToList();
            if (_listlocalpaciente.IsNotNull())
            {
                List<PacienteEmergencia> listpac = _listlocalpaciente.Where(x => x.PacienteEmergencia.IsNotNull()).DistinctBy(x => x.PacienteEmergencia.Id).Select(x => x.PacienteEmergencia).ToList();
                List<PassagemPlantaoEmergenciaLocalPaciente> listpassagemaux = new List<PassagemPlantaoEmergenciaLocalPaciente>();
                foreach (var item in listpac)
                {
                    var passplantao = _listlocalpaciente.Where(x => x.PacienteEmergencia.IsNotNull() && x.PacienteEmergencia.Id == item.Id).OrderBy(x => x.ID).FirstOrDefault();
                    listpassagemaux.Add(passplantao);
                }
                listurgencia = listpassagemaux.Where(x => pListaUrgencia.Count(y => x.PacienteEmergencia.BoletimDeEmergencia.IsNotNull()
                    && y.IDBoletim == x.PacienteEmergencia.BoletimDeEmergencia.Id && x.PacienteEmergencia.BoletimDeEmergencia.DataAlta.IsNull()) > 0
                    && x.PassagemPlantaoEmergenciaLocalizacao.PassagemPlantao == SimNao.Sim).ToList();

                if (_pediatrico)
                    listurgenciapaciente = pListaUrgencia.Where(x => listurgencia.Count(y => y.PacienteEmergencia.BoletimDeEmergencia.Id == x.IDBoletim) == 0
                                                            && (x.Cor == "Amarelo" || x.Cor == "Laranja" || x.Cor == "Vermelho" || x.Cor == "Verde")
                                                            && x.HoraAtendimento.AddHours(12) >= DateTime.Now).ToList();
                else
                    listurgenciapaciente = pListaUrgencia.Where(x => listurgencia.Count(y => y.PacienteEmergencia.BoletimDeEmergencia.Id == x.IDBoletim) == 0
                                                                && (x.Cor == "Amarelo" || x.Cor == "Laranja" || x.Cor == "Vermelho")
                                                                && x.HoraAtendimento.AddHours(12) >= DateTime.Now).ToList();
            }

            _atendimentos.Clear();
            _atendimentos = pListaInternados.Select(x => x.Atendimento).ToList();
            _atendimentos.AddRange(listurgencia.Select(x => x.Atendimento.ID).ToList());
            _atendimentos.AddRange(listurgenciapaciente.Select(x => x.Atendimento).ToList());
            if (_atendimentos.HasItems())
            {
                IRepositorioDePassagemPlantaoEmergencia rep2 = ObjectFactory.GetInstance<IRepositorioDePassagemPlantaoEmergencia>();
                _listapassagemplantao = rep2.OndeIdAtendimentoIn(_atendimentos).List();
            }

            CarregaEvolucoes(pListaInternados, listurgencia, listurgenciapaciente);
            CarregaHistoricoObservacao();
        }

        public void CloseConnection()
        {
            myHub = null;
            connection.Items.Clear();
            connection.Stop();
        }

        public void ExecuteSalvar()
        {
            if (_evolucaoselecionada.IsNotNull())
            {
                SalvaPassagem(_evolucaoselecionada);
                EnviaDados(_evolucaoselecionada, false);
            }
            if (_observacaoevolucao.IsNotEmptyOrWhiteSpace())
            {
                SalvaObs(_observacaoevolucao);
                EnviaDadosObs(_observacaoevolucao);
            }
        }

        public void CopiaHistoricoObservacao()
        {
            _observacaoevolucaoID = _historicoselecionadoobservacao.ID;
            _observacaoevolucao = _historicoselecionadoobservacao.Observacao;

            SalvaObs(_observacaoevolucao);
            EnviaDadosObs(_observacaoevolucao);
        }
        #endregion

        #region Commands
        protected virtual void MarcaTodos()
        {
            if (ExecuteMarcaTodos != null) ExecuteMarcaTodos(this, EventArgs.Empty);
        }
        protected override bool CommandCanExecuteSelecionar(object param)
        {
            return _historicoselecionado.IsNotNull();
        }
        protected override void CommandSelecionar(object param)
        {
            CopiaHistorico();
        }
        protected override bool CommandCanExecuteAlterar(object param)
        {
            return (_leitosvagoselecionado.IsNotNull());
        }
        protected override void CommandAlterar(object param)
        {
            AlteraLeito();
        }
        protected override bool CommandCanExecuteSalvar(object param)
        {
            return _evolucaoselecionada.IsNotNull();
        }
        protected override void CommandSalvar(object param)
        {
            SalvaPassagem(_evolucaoselecionada);
            EnviaDados(_evolucaoselecionada, false);
        }
        #endregion
    }

    #region Classes
    public class EvolucaoEmergenciaObservacao
    {
        public virtual int ID { get; set; }
        public virtual string Observacao { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual string Usuario { get; set; }
    }
    public class EvolucaoEmergencia : ViewModelBase, INotifyPropertyChanged
    {
        string _listaProblemas = string.Empty;
        string _plano = string.Empty;
        string _enfermagem = string.Empty;
        bool _imprimir;
        vmPassagemPlantaoEmergencia _vm;
        bool _mostraEnfermagem;
        PassagemPlantaoEmergenciaLocalizacao _passagemPlantaoEmergencia;

        public bool AlterouLista { get; set; }
        public bool AlterouPlano { get; set; }
        public bool AlterouEnfermagem { get; set; }

        public EvolucaoEmergencia(vmPassagemPlantaoEmergencia pVm)
        {
            _vm = pVm;
            _mostraEnfermagem = _vm.MostraEnfermagem;
            _vm.ExecuteMarcaTodos += new EventHandler(MarcaImprimir);

        }

        private void MarcaImprimir(object sender, EventArgs e)
        {
            if (_vm.MarcarTodos)
                _imprimir = true;
            else
                _imprimir = false;
            OnChanged("Imprime");
        }

        public int? Ordem { get { return _passagemPlantaoEmergencia.IsNotNull() ? _passagemPlantaoEmergencia.Ordem : IsUltimos ? 99999 : 99998; } }
        public int Atendimento { get; set; }
        public string NomePaciente { get; set; }
        public string Idade { get; set; }
        public string DataNascimento { get; set; }
        public string DataAtendimento { get; set; }
        public bool MostraDI { get { return !string.IsNullOrWhiteSpace(DataAtendimento); } }
        public string Medico { get; set; }
        public string Alergias { get; set; }
        public PassagemPlantaoEmergenciaLocalizacao Localizacao
        {
            get { return _passagemPlantaoEmergencia; }
            set
            {
                _passagemPlantaoEmergencia = value;
                OnChanged("Leito");
                OnChanged("CorTeste");
                OnChanged("Localizacao");
            }
        }
        public string Leito { get { if (_passagemPlantaoEmergencia.IsNotNull()) return _passagemPlantaoEmergencia.Descricao; else return string.Empty; } }
        public bool Imprime { get { return _imprimir; } set { _imprimir = value; OnChanged("Imprime"); } }
        public bool IsInternado { get; set; }
        public bool IsUltimos { get; set; }
        public bool MostraEnfermagem
        {
            get
            {
                return _mostraEnfermagem;
            }
        }

        public string ListaProblemas
        {
            get
            {
                if (_listaProblemas.IsEmptyOrWhiteSpace())
                    return "<FlowDocument AllowDrop=\"True\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />";
                return _listaProblemas;
            }
            set
            {
                if (_listaProblemas != value)
                    AlterouLista = true;
                _listaProblemas = value;
                OnChanged("ListaProblemas");
            }
        }
        public string Plano
        {
            get
            {
                if (_plano.IsEmptyOrWhiteSpace())
                    return "<FlowDocument AllowDrop=\"True\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />";
                return _plano;
            }
            set
            {
                if (_plano != value)
                    AlterouPlano = true;
                _plano = value;
                OnChanged("Plano");
            }
        }
        public string Enfermagem
        {
            get
            {
                if (_enfermagem.IsEmptyOrWhiteSpace())
                    return "<FlowDocument AllowDrop=\"True\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />";
                return _enfermagem;
            }
            set
            {
                if (_enfermagem != value)
                    AlterouEnfermagem = true;
                _enfermagem = value;
                OnChanged("Enfermagem");
            }
        }

        public string AvaliacaoRisco { get; set; }

        public bool MostraLabelAval { get { return AvaliacaoRisco.IsNotEmptyOrWhiteSpace(); } }

        public SolidColorBrush CorTeste
        {
            get
            {
                if (_passagemPlantaoEmergencia.IsNotNull() && _passagemPlantaoEmergencia.Cor.IsNotNull())
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(_passagemPlantaoEmergencia.Cor.CodigoCor_Net));
                return new SolidColorBrush(Colors.Black);
            }
        }

        protected void OnChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    public class EvolucaoEmergenciaImpressao
    {
        public int Atendimento { get; set; }
        public string NomePaciente { get; set; }
        public string Idade { get; set; }
        public string DataNascimento { get; set; }
        public string Medico { get; set; }
        public string Alergias { get; set; }
        public PassagemPlantaoEmergenciaLocalizacao Localizacao { get; set; }
        public string Leito { get; set; }
        public bool IsInternado { get; set; }
        public bool IsUltimos { get; set; }
        public string ListaProblemas { get; set; }
        public string Plano { get; set; }
        public string Enfermagem { get; set; }
        public string AvaliacaoRisco { get; set; }
        public SolidColorBrush CorTeste { get; set; }
    }
    #endregion
}
