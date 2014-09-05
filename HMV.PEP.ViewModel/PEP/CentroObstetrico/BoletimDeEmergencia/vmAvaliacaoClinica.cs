using HMV.Core.Framework.ViewModelBaseClasses;
using System.Windows;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;
using HMV.Core.Wrappers.CollectionWrappers.PEP.CentroObstetrico;
using System.Linq;
using System;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Enum;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Validations;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia
{
    public class vmAvaliacaoClinica : ViewModelBase
    {
        #region ----- Construtor -----
        public vmAvaliacaoClinica(vmBoletimEmergenciaCO pVm)
        {
            pVm.DictionaryCO.Add(vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO.AvaliacaoClinica, this);
            _boletim = pVm.Boletim;
            _boletimCO = pVm.BoletimCO;
            _usuario = pVm.Usuarios;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpBoletimDeEmergencia _boletim;
        private wrpBoletimCentroObstetrico _boletimCO;
        private wrpUsuarios _usuario;
        private wrpAvaliacaoClinica _avaliacaoClinica;
        #endregion

        #region ----- Propriedades Públicas -----

        public wrpAvaliacaoClinica AvaliacaoClinica
        {
            get
            {
                return _avaliacaoClinica;
            }
            set
            {
                _avaliacaoClinica = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.AvaliacaoClinica);
                atualizaAvliacaoClinica();
            }
        }

        public string ToqueDescricao
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Toque.IsNotNull() ? _avaliacaoClinica.Toque.Descricao : string.Empty;
            }
            set
            {
                IniciaAvaliacaoClinica();

                if (_avaliacaoClinica.Toque.IsNull())
                    _avaliacaoClinica.Toque = new wrpTextoNA();

                _avaliacaoClinica.Toque.Descricao = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.ToqueDescricao);
                atualizaAvliacaoClinica();
            }
        }

        public SimNao ToqueNaoAvaliado
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Toque.IsNotNull() ? _avaliacaoClinica.Toque.NaoAvaliado : SimNao.Nao;
            }
            set
            {
                IniciaAvaliacaoClinica();
                if (value == SimNao.Sim)
                {
                    if (ToqueDescricao.IsNotEmptyOrWhiteSpace())
                    {
                        if (DXMessageBox.Show("As informações digitadas na descrição abaixo serão perdidos, deseja continuar ?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                        {
                            OnPropertyChanged<vmAvaliacaoClinica>(x => x.ToqueNaoAvaliado);
                            return;
                        }
                    }
                    ToqueDescricao = "Não se aplica";
                }
                else
                    ToqueDescricao = "";

                _avaliacaoClinica.Toque.NaoAvaliado = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.ToqueNaoAvaliado);
                atualizaAvliacaoClinica();
            }
        }

        public string ExameDescricao
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.ExameEspecular.IsNotNull() ? _avaliacaoClinica.ExameEspecular.Descricao : string.Empty;
            }
            set
            {
                IniciaAvaliacaoClinica();
                if (_avaliacaoClinica.ExameEspecular.IsNull())
                    _avaliacaoClinica.ExameEspecular = new wrpTextoNA();

                _avaliacaoClinica.ExameEspecular.Descricao = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.ExameDescricao);
                atualizaAvliacaoClinica();
            }
        }

        public SimNao ExameNaoAvaliado
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.ExameEspecular.IsNotNull() ? _avaliacaoClinica.ExameEspecular.NaoAvaliado : SimNao.Nao;
            }
            set
            {
                IniciaAvaliacaoClinica();
                if (value == SimNao.Sim)
                {
                    if (ExameDescricao.IsNotEmptyOrWhiteSpace())
                    {
                        if (DXMessageBox.Show("As informações digitadas na descrição abaixo serão perdidos, deseja continuar ?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                        {
                            OnPropertyChanged<vmAvaliacaoClinica>(x => x.ExameNaoAvaliado);
                            return;
                        }
                    }
                    ExameDescricao = "Não se aplica";
                }
                else
                    ExameDescricao = "";

                _avaliacaoClinica.ExameEspecular.NaoAvaliado = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.ExameNaoAvaliado);
                atualizaAvliacaoClinica();
            }
        }

        public string EcografiaDescricao
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Ecografia.IsNotNull() ? _avaliacaoClinica.Ecografia.Descricao : string.Empty;
            }
            set
            {
                IniciaAvaliacaoClinica();
                if (_avaliacaoClinica.Ecografia.IsNull())
                    _avaliacaoClinica.Ecografia = new wrpTextoNA();

                _avaliacaoClinica.Ecografia.Descricao = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.EcografiaDescricao);
                atualizaAvliacaoClinica();
            }
        }

        public SimNao EcografiaNaoAvaliado
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Ecografia.IsNotNull() ? _avaliacaoClinica.Ecografia.NaoAvaliado : SimNao.Nao;
            }
            set
            {
                IniciaAvaliacaoClinica();
                if (value == SimNao.Sim)
                {
                    if (EcografiaDescricao.IsNotEmptyOrWhiteSpace())
                    {
                        if (DXMessageBox.Show("As informações digitadas na descrição abaixo serão perdidos, deseja continuar ?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                        {
                            OnPropertyChanged<vmAvaliacaoClinica>(x => x.EcografiaNaoAvaliado);
                            return;
                        }
                    }
                    EcografiaDescricao = "Não se aplica";
                }
                else
                    EcografiaDescricao = "";

                _avaliacaoClinica.Ecografia.NaoAvaliado = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.EcografiaNaoAvaliado);
                atualizaAvliacaoClinica();
            }
        }

        public string MonitorizacaoDescricao
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Monitorizacao.IsNotNull() ? _avaliacaoClinica.Monitorizacao.Descricao : string.Empty;
            }
            set
            {
                IniciaAvaliacaoClinica();
                if (_avaliacaoClinica.Monitorizacao.IsNull())
                    _avaliacaoClinica.Monitorizacao = new wrpTextoNA();

                _avaliacaoClinica.Monitorizacao.Descricao = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.MonitorizacaoDescricao);
                atualizaAvliacaoClinica();
            }
        }

        public SimNao MonitorizacaoNaoAvaliado
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Monitorizacao.IsNotNull() ? _avaliacaoClinica.Monitorizacao.NaoAvaliado : SimNao.Nao;
            }
            set
            {
                IniciaAvaliacaoClinica();
                if (value == SimNao.Sim)
                {
                    if (MonitorizacaoDescricao.IsNotEmptyOrWhiteSpace())
                    {
                        if (DXMessageBox.Show("As informações digitadas na descrição abaixo serão perdidos, deseja continuar ?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                        {
                            OnPropertyChanged<vmAvaliacaoClinica>(x => x.MonitorizacaoNaoAvaliado);
                            return;
                        }
                    }
                    MonitorizacaoDescricao = "Não se aplica";
                }
                else
                    MonitorizacaoDescricao = "";

                _avaliacaoClinica.Monitorizacao.NaoAvaliado = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.MonitorizacaoNaoAvaliado);
                atualizaAvliacaoClinica();
            }
        }

        public string TonusDescricao
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Tonus.IsNotNull() ? _avaliacaoClinica.Tonus.Descricao : string.Empty;
            }
            set
            {
                IniciaAvaliacaoClinica();
                if (_avaliacaoClinica.Tonus.IsNull())
                    _avaliacaoClinica.Tonus = new wrpTextoNA();

                _avaliacaoClinica.Tonus.Descricao = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.TonusDescricao);
                atualizaAvliacaoClinica();
            }
        }

        public SimNao TonusNaoAvaliado
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Tonus.IsNotNull() ? _avaliacaoClinica.Tonus.NaoAvaliado : SimNao.Nao;
            }
            set
            {
                IniciaAvaliacaoClinica();
                if (value == SimNao.Sim)
                {
                    if (TonusDescricao.IsNotEmptyOrWhiteSpace())
                    {
                        if (DXMessageBox.Show("As informações digitadas na descrição abaixo serão perdidos, deseja continuar ?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                        {
                            OnPropertyChanged<vmAvaliacaoClinica>(x => x.TonusNaoAvaliado);
                            return;
                        }
                    }
                    TonusDescricao = "Não se aplica";
                }
                else
                    TonusDescricao = "";

                if (_avaliacaoClinica.IsNotNull())
                    _avaliacaoClinica.Tonus.NaoAvaliado = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.TonusNaoAvaliado);
                atualizaAvliacaoClinica();
            }
        }

        public string ExameFisico
        {
            get
            {
                return _avaliacaoClinica.IsNull() ? string.Empty : _avaliacaoClinica.ExameFisico;
            }
            set
            {
                IniciaAvaliacaoClinica();
                _avaliacaoClinica.ExameFisico = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.ExameFisico);
                atualizaAvliacaoClinica();
            }
        }

        public string HistoriaAtual
        {
            get
            {
                return _avaliacaoClinica.IsNull() ? string.Empty : _avaliacaoClinica.HistoriaAtual;
            }
            set
            {
                IniciaAvaliacaoClinica();
                _avaliacaoClinica.HistoriaAtual = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.HistoriaAtual);
                atualizaAvliacaoClinica();
            }
        }

        [ValidaMaximoEMinimo(0, 20)]
        public int? DinamicaValor
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Dinamica.IsNotNull() ? _avaliacaoClinica.Dinamica.Valor : (int?)null;
            }
            set
            {
                IniciaAvaliacaoClinica();
                _avaliacaoClinica.Dinamica.Valor = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.DinamicaValor);
                atualizaAvliacaoClinica();
            }
        }


        public SimNao DinamicaNaoAvaliado
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Dinamica.IsNotNull() ? _avaliacaoClinica.Dinamica.NaoAvaliado : SimNao.Nao;
            }
            set
            {
                IniciaAvaliacaoClinica();

                if (value == SimNao.Sim)
                    _avaliacaoClinica.Dinamica.Valor = null;

                _avaliacaoClinica.Dinamica.NaoAvaliado = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.DinamicaNaoAvaliado);
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.DinamicaValor);
                atualizaAvliacaoClinica();
            }
        }

        [ValidaMaximoEMinimo(4, 42)]
        public int? IdadeSemana
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.GestacaoIdade.IsNotNull() ? _avaliacaoClinica.GestacaoIdade.IdadeSemana : (int?)null;
            }
            set
            {
                IniciaAvaliacaoClinica();
                _avaliacaoClinica.GestacaoIdade.IdadeSemana = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.IdadeSemana);
                atualizaAvliacaoClinica();
            }
        }

        [ValidaMaximoEMinimo(0, 6)]
        public int? IdadeDia
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.GestacaoIdade.IsNotNull() ? _avaliacaoClinica.GestacaoIdade.IdadeDia : (int?)null;
            }
            set
            {
                IniciaAvaliacaoClinica();
                _avaliacaoClinica.GestacaoIdade.IdadeDia = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.IdadeDia);
                atualizaAvliacaoClinica();
            }
        }

        public SimNao Desconhecido
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.GestacaoIdade.IsNotNull() ? _avaliacaoClinica.GestacaoIdade.Desconhecido : SimNao.Nao;
            }
            set
            {
                if (value == SimNao.Nao)
                {
                    _avaliacaoClinica.GestacaoIdade.IdadeDia = null;
                    _avaliacaoClinica.GestacaoIdade.IdadeSemana = null;
                }

                IniciaAvaliacaoClinica();
                _avaliacaoClinica.GestacaoIdade.Desconhecido = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.Desconhecido);
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.IdadeDia);
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.IdadeSemana);
                atualizaAvliacaoClinica();
            }
        }

        #region ---- Gestações ----
        [ValidaMaximoEMinimo(0, 20)]
        public int? Gesta
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Gestacao.IsNotNull() ? _avaliacaoClinica.Gestacao.Gesta : (int?)null;
            }
            set
            {
                IniciaAvaliacaoClinica();

                if (_avaliacaoClinica.Gestacao.IsNull())
                    _avaliacaoClinica.Gestacao = new wrpGestacao();

                _avaliacaoClinica.Gestacao.Gesta = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.Gesta);
                
                atualizaAvliacaoClinica();
            }
        }

        [ValidaMaximoEMinimo(0, 20)]
        public int? Para
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Gestacao.IsNotNull() ? _avaliacaoClinica.Gestacao.Para : (int?)null;
            }
            set
            {
                IniciaAvaliacaoClinica();

                if (_avaliacaoClinica.Gestacao.IsNull())
                    _avaliacaoClinica.Gestacao = new wrpGestacao();

                _avaliacaoClinica.Gestacao.Para = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.Para);
                atualizaAvliacaoClinica();
            }
        }

        [ValidaMaximoEMinimo(0, 20)]
        public int? Cesarea
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Gestacao.IsNotNull() ? _avaliacaoClinica.Gestacao.Cesarea : (int?)null;
            }
            set
            {
                IniciaAvaliacaoClinica();

                if (_avaliacaoClinica.Gestacao.IsNull())
                    _avaliacaoClinica.Gestacao = new wrpGestacao();

                _avaliacaoClinica.Gestacao.Cesarea = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.Cesarea);
                atualizaAvliacaoClinica();
            }
        }

        [ValidaMaximoEMinimo(0, 20)]
        public int? Aborto
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Gestacao.IsNotNull() ? _avaliacaoClinica.Gestacao.Aborto : (int?)null;
            }
            set
            {
                IniciaAvaliacaoClinica();

                if (_avaliacaoClinica.Gestacao.IsNull())
                    _avaliacaoClinica.Gestacao = new wrpGestacao();

                _avaliacaoClinica.Gestacao.Aborto = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.Aborto);
                atualizaAvliacaoClinica();
            }
        }

        [ValidaMaximoEMinimo(0, 20)]
        public int? Ectopica
        {
            get
            {
                return _avaliacaoClinica.IsNotNull() && _avaliacaoClinica.Gestacao.IsNotNull() ? _avaliacaoClinica.Gestacao.Ectopica : (int?)null;
            }
            set
            {
                IniciaAvaliacaoClinica();

                if (_avaliacaoClinica.Gestacao.IsNull())
                    _avaliacaoClinica.Gestacao = new wrpGestacao();

                _avaliacaoClinica.Gestacao.Ectopica = value;
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.Ectopica);
                atualizaAvliacaoClinica();
            }
        }
        #endregion

        //public wrpAvaliacaoClinicaCollection Avaliacoes
        //{
        //    get
        //    {

        //        // CUIDADO em usar o SortDesc, cada vez que eh chamado ele insere o mesmo registro novamente no dominio, nao tivemos problema pq eh o mesmo objeto e o nhibernate faz merge do objeto.  
        //        // Fazer conforme abaixo, mas cuidado em usar o NEW 
        //        // Nao consegui corrigir o SortDesc 
        //        //_boletimCO.AvaliacoesClinicas.SortDesc(x => x.DataInclusao);
        //        return new wrpAvaliacaoClinicaCollection(_boletimCO.DomainObject.AvaliacoesClinicas.OrderByDescending(x => x.DataInclusao).ToList());
        //    }
        //}

        public IList<BoletimCOHistoricoDTO> Avaliacoes
        {
            get
            {
                IList<BoletimCOHistoricoDTO> lista = new List<BoletimCOHistoricoDTO>();
                foreach (var item in _boletimCO.DomainObject.AvaliacoesClinicas.OrderByDescending(x => x.DataInclusao).ToList())
                {
                    wrpAvaliacaoClinica wrp = new wrpAvaliacaoClinica(item);

                    lista.Add(new BoletimCOHistoricoDTO(true, "Data/Hora: " + Environment.NewLine + "Usuário: ", wrp.DataInclusao.ToString() + Environment.NewLine + wrp.Usuario.AssinaturaNaLinha));
                    //lista.Add(new BoletimCOHistoricoDTO("Usuário: " , wrp.Usuario.AssinaturaNaLinha));
                    if (wrp.TemGestacao)
                        lista.Add(new BoletimCOHistoricoDTO("Gestações:" , wrp.Gestacao.ToString()));
                    if (wrp.TemGestacaoIdade)
                        lista.Add(new BoletimCOHistoricoDTO("Idade Gestacional: " , wrp.GestacaoIdade.ToString()));
                    if (wrp.TemDinamica)
                        lista.Add(new BoletimCOHistoricoDTO("Dinâmica: " , wrp.Dinamica.ToString()));
                    if (wrp.TemHistoriaAtual)
                        lista.Add(new BoletimCOHistoricoDTO("História Atual: " , wrp.HistoriaAtual));
                    if (wrp.TemTonus)
                        lista.Add(new BoletimCOHistoricoDTO("Tônus: " , wrp.Tonus.Descricao));
                    if (wrp.TemExameEspecular)
                        lista.Add(new BoletimCOHistoricoDTO("Exame Especular: " , wrp.ExameEspecular.Descricao));
                    if (wrp.TemToque)
                        lista.Add(new BoletimCOHistoricoDTO("Toque: " , wrp.Toque.Descricao));
                    if (wrp.TemMonitorizacao)
                        lista.Add(new BoletimCOHistoricoDTO("Monitorização Ante-parto: " , wrp.Monitorizacao.Descricao));
                    if (wrp.TemEcografia)
                        lista.Add(new BoletimCOHistoricoDTO("Ecogafia no Centro Obstétrico: " , wrp.Ecografia.Descricao));
                    if (wrp.TemExameFisico)
                        lista.Add(new BoletimCOHistoricoDTO("Demais aspectos do Exame Físico: " , wrp.ExameFisico));
                    lista.Last().IsVisibilityBorda = Visibility.Visible;
                }

                return lista;
            }
        }

        #endregion

        #region ----- Métodos Privados -----
        private void IniciaAvaliacaoClinica()
        {
            if (_avaliacaoClinica.IsNull())
            {
                _avaliacaoClinica = new wrpAvaliacaoClinica(_usuario);
                _avaliacaoClinica.BoletimDeEmergencia = _boletim;
                _avaliacaoClinica.Dinamica = new wrpDinamica();
                _avaliacaoClinica.GestacaoIdade = new wrpGestacaoIdade();
                _avaliacaoClinica.Tonus = new wrpTextoNA();
                _avaliacaoClinica.Toque = new wrpTextoNA();
                _avaliacaoClinica.Monitorizacao = new wrpTextoNA();
                _avaliacaoClinica.ExameEspecular = new wrpTextoNA();
                _avaliacaoClinica.Ecografia = new wrpTextoNA();
                _boletimCO.AvaliacoesClinicas.Add(_avaliacaoClinica);
                OnPropertyChanged<vmAvaliacaoClinica>(x => x.Avaliacoes);
            }

        }

        private void atualizaAvliacaoClinica()
        {
            if (!ValidaSalvar())
                return;

            if (_avaliacaoClinica.IsNotNull())
            {
                if (!_avaliacaoClinica.TemDinamica && !_avaliacaoClinica.TemEcografia && !_avaliacaoClinica.TemExameEspecular && !_avaliacaoClinica.TemExameFisico
                    && !_avaliacaoClinica.TemGestacao && !_avaliacaoClinica.TemGestacaoIdade && !_avaliacaoClinica.TemHistoriaAtual && !_avaliacaoClinica.TemMonitorizacao
                    && !_avaliacaoClinica.TemTonus && !_avaliacaoClinica.TemToque)
                {
                    _boletimCO.AvaliacoesClinicas.Remove(_avaliacaoClinica);
                    _avaliacaoClinica = null;
                }
            }

            OnPropertyChanged<vmAvaliacaoClinica>(x => x.Avaliacoes);

        }
        #endregion

        #region ----- Métodos Públicos -----
        private bool ValidaSalvar()
        {
            return ValidaSalvar(false);
        }
        
        private bool ValidaSalvar(bool exibeMsg)
        {
            string message = string.Empty;
            if (Gesta.HasValue && (Gesta.Value < 0 || Gesta.Value > 20))
                message = "Verifique os valores digitados para Gestações";

            if (Para.HasValue && (Para.Value < 0 || Para.Value > 20))
                message = "Verifique os valores digitados para Gestações";

            if (Cesarea.HasValue && (Cesarea.Value < 0 || Cesarea.Value > 20))
                message = "Verifique os valores digitados para Gestações";

            if (Aborto.HasValue && (Aborto.Value < 0 || Aborto.Value > 20))
                message = "Verifique os valores digitados para Gestações";

            if (Ectopica.HasValue && (Ectopica.Value < 0 || Ectopica.Value > 20))
                message = "Verifique os valores digitados para Gestações";

            if (IdadeSemana.HasValue && (IdadeSemana.Value < 4 || IdadeSemana.Value > 42))
                message = "Verifique os valores digitados para Idade Gestacional";

            if (IdadeDia.HasValue && (IdadeDia.Value < 0 || IdadeDia.Value > 6))
                message = "Verifique os valores digitados para Idade Gestacional";

            if (DinamicaValor.HasValue && (DinamicaValor.Value < 0 || DinamicaValor.Value > 20))
                message = "Verifique os valores digitados para Dinâmica";

            if (string.IsNullOrWhiteSpace(message))
                return true;

            if ( exibeMsg ) 
                DXMessageBox.Show(message,"Atenção", MessageBoxButton.OK, MessageBoxImage.Question);
            
            return false;
        }

        public override bool IsValid
        {
            get
            {
                if (!ValidaSalvar(true))
                    return false;

                return valida();
            }
        }

        private bool valida()
        {
            List<string> message = new List<string>();

            if (_boletimCO.AvaliacoesClinicas.Count() == 1 && _avaliacaoClinica.IsNotNull())
            {
                if (!_avaliacaoClinica.TemGestacao)
                    message.Add("Anamnese / [Gestação]");
                if (!_avaliacaoClinica.TemGestacaoIdade)
                    message.Add("Anamnese / [Idade da Gestacional]");
                if (!_avaliacaoClinica.TemDinamica)
                    message.Add("Anamnese / [Dinâmica]");
                if (!_avaliacaoClinica.TemHistoriaAtual)
                    message.Add("Anamnese / [História Atual]");
                if (!_avaliacaoClinica.TemTonus)
                    message.Add("Exame Clínico / [Tônus]");
                if (!_avaliacaoClinica.TemExameEspecular)
                    message.Add("Exame Clínico / [Exame Especular]");
                if (!_avaliacaoClinica.TemToque)
                    message.Add("Exame Clínico / [Toque]");
                if (!_avaliacaoClinica.TemMonitorizacao)
                    message.Add("Exame Clínico / [Monitorização]");
                if (!_avaliacaoClinica.TemEcografia)
                    message.Add("Exame Clínico / [Ecografia]");
            }

            if (message.Count > 0)
            {
                MessageBox.Show("Na Primeira Avaliação Clínica os seguintes ítens devem ser informados: ".CombineAndNovaLinha(string.Join(Environment.NewLine, message)), "Atenção", MessageBoxButton.OK, MessageBoxImage.Stop); ;
                return false;
            }

            return true;

        }
        #endregion

        #region ----- Commands -----

        #endregion
    }
}
