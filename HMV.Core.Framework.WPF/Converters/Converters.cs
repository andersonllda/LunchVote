using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Types;
using HMV.Core.Interfaces;
using System.Collections.Generic;
using HMV.Core.Domain.Enum.Checkup;
using HMV.Core.Domain.Enum.CentroObstetrico;

namespace HMV.Core.Framework.WPF.Converters
{
    #region ValueConverters
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType() == typeof(bool))
                if ((bool)value)
                    return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
                return true;
            return false;
        }
        #endregion
    }

    public class BoolToOppositeVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType() == typeof(bool))
                if ((bool)value)
                    return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (Visibility)value == Visibility.Collapsed)
                return true;
            return false;
        }
        #endregion
    }

    public class VisibilityToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType() == typeof(Visibility))
                if ((Visibility)value == Visibility.Visible)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
                return true;
            return false;
        }
        #endregion
    }

    public class BoolToOppositeBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && targetType == typeof(bool))
                return !(bool)value;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public class EnumToDescriptionsConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                //MucosasEstado
                if (value.GetType() == typeof(MucosasEstado[]))
                    return Enum<MucosasEstado>.GetDescriptions();
                else if (value.GetType() == typeof(MucosasEstado))
                    return Enum<MucosasEstado>.GetDescriptionOf((MucosasEstado)value);
                //MucosasSituacao
                else if (value.GetType() == typeof(MucosasSituacao[]))
                    return Enum<MucosasSituacao>.GetDescriptions();
                else if (value.GetType() == typeof(MucosasSituacao))
                    return Enum<MucosasSituacao>.GetDescriptionOf((MucosasSituacao)value);
                //EG
                else if (value.GetType() == typeof(EG[]))
                    return Enum<EG>.GetDescriptions();
                else if (value.GetType() == typeof(EG))
                    return Enum<EG>.GetDescriptionOf((EG)value);
                //Registros de Evolucao
                else if (value.GetType() == typeof(RegistrosEvolucao[]))
                    return Enum<RegistrosEvolucao>.GetDescriptions();
                else if (value.GetType() == typeof(RegistrosEvolucao))
                    return Enum<RegistrosEvolucao>.GetDescriptionOf((RegistrosEvolucao)value);
                //CadastroAltaDestino
                else if (value.GetType() == typeof(CadastroAltaDestino[]))
                    return Enum<CadastroAltaDestino>.GetDescriptions();
                else if (value.GetType() == typeof(CadastroAltaDestino))
                    return Enum<CadastroAltaDestino>.GetDescriptionOf((CadastroAltaDestino)value);
                //Resumo do Prontuario Filtros
                else if (value.GetType() == typeof(ResumoDoProntuarioFiltros[]))
                    return Enum<ResumoDoProntuarioFiltros>.GetDescriptions();
                else if (value.GetType() == typeof(ResumoDoProntuarioFiltros))
                    return Enum<ResumoDoProntuarioFiltros>.GetDescriptionOf((ResumoDoProntuarioFiltros)value);

                //Avaliacao Risco
                else if (value.GetType() == typeof(SexoCid[]))
                    return Enum<SexoCid>.GetDescriptions();
                else if (value.GetType() == typeof(SexoCid))
                    return Enum<SexoCid>.GetDescriptionOf((SexoCid)value);
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (targetType == typeof(MucosasEstado) || targetType == typeof(MucosasEstado?))
                {
                    if (value.ToString() == "Corado")
                        return MucosasEstado.Corado;
                    else if (value.ToString() == "Hipocorado")
                        return MucosasEstado.Hipocorado;
                }
                else if (targetType == typeof(MucosasSituacao) || targetType == typeof(MucosasSituacao?))
                {
                    if (value.ToString() == "Desidratado")
                        return MucosasSituacao.Desidratado;
                    else if (value.ToString() == "Hidratado")
                        return MucosasSituacao.Hidratado;
                }
                else if (targetType == typeof(EG) || targetType == typeof(EG?))
                {
                    if (value.ToString() == "Bom")
                        return EG.Bom;
                    else if (value.ToString() == "Mal")
                        return EG.Mal;
                    else if (value.ToString() == "Regular")
                        return EG.Regular;
                }
                else if (targetType == typeof(RegistrosEvolucao) || targetType == typeof(RegistrosEvolucao?))
                {
                    if (value.ToString() == "<< Todos >>")
                        return RegistrosEvolucao.Todos;
                    else if (value.ToString() == "Meus Registros")
                        return RegistrosEvolucao.MeusRegistros;
                    else if (value.ToString() == "Registros do Centro")
                        return RegistrosEvolucao.RegistrosCentro;
                }
                else if (targetType == typeof(CadastroAltaDestino) || targetType == typeof(CadastroAltaDestino?))
                {
                    if (value.ToString() == "Domicílio")
                        return CadastroAltaDestino.Domicilio;
                    else if (value.ToString() == "Internação")
                        return CadastroAltaDestino.Internacao;
                    else if (value.ToString() == "Outros - Especificar")
                        return CadastroAltaDestino.Outros;
                    else if (value.ToString() == "Transferência")
                        return CadastroAltaDestino.Transferencia;
                }
                else if (targetType == typeof(ResumoDoProntuarioFiltros) || targetType == typeof(ResumoDoProntuarioFiltros?))
                {
                    if (value.ToString() == "<< Todos >>")
                        return ResumoDoProntuarioFiltros.Todos;
                    else if (value.ToString() == "Meus Registros")
                        return ResumoDoProntuarioFiltros.MeusRegistros;
                }
                else if (targetType == typeof(SexoCid) || targetType == typeof(SexoCid?))
                {
                    if (value.ToString() == "Ambos")
                        return SexoCid.Ambos;
                    else if (value.ToString() == "Feminino")
                        return SexoCid.Feminino;
                    else if (value.ToString() == "Masculino")
                        return SexoCid.Masculino;
                }
            }
            return null;
        }
        #endregion
    }

    public class EnumToCustonDisplayConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                //StatusMedicamentosEmUso
                if (value.GetType() == typeof(StatusMedicamentosEmUso[]))
                    return Enum<StatusMedicamentosEmUso>.GetCustomDisplay();
                else if (value.GetType() == typeof(StatusMedicamentosEmUso))
                    return Enum<StatusMedicamentosEmUso>.GetCustomDisplayOf((StatusMedicamentosEmUso)value);
                //TipoDeFormacao
                else if (value.GetType() == typeof(TipoDeFormacao[]))
                    return Enum<TipoDeFormacao>.GetCustomDisplay();
                else if (value.GetType() == typeof(TipoDeFormacao))
                    return Enum<TipoDeFormacao>.GetCustomDisplayOf((TipoDeFormacao)value);
                //StatusCurso
                else if (value.GetType() == typeof(StatusCurso[]))
                    return Enum<StatusCurso>.GetCustomDisplay();
                else if (value.GetType() == typeof(StatusCurso))
                    return Enum<StatusCurso>.GetCustomDisplayOf((StatusCurso)value);
                //StatusMatricula
                else if (value.GetType() == typeof(StatusMatricula[]))
                    return Enum<StatusMatricula>.GetCustomDisplay();
                else if (value.GetType() == typeof(StatusMatricula))
                    return Enum<StatusMatricula>.GetCustomDisplayOf((StatusMatricula)value);
                //StatusMatriculaDisciplina
                else if (value.GetType() == typeof(StatusMatriculaDisciplina[]))
                    return Enum<StatusMatriculaDisciplina>.GetCustomDisplay();
                else if (value.GetType() == typeof(StatusMatriculaDisciplina))
                    return Enum<StatusMatriculaDisciplina>.GetCustomDisplayOf((StatusMatriculaDisciplina)value);
                //OrigemPagamentoRPA
                else if (value.GetType() == typeof(OrigemPagamentoRPA[]))
                    return Enum<OrigemPagamentoRPA>.GetCustomDisplay();
                else if (value.GetType() == typeof(OrigemPagamentoRPA))
                    return Enum<OrigemPagamentoRPA>.GetCustomDisplayOf((OrigemPagamentoRPA)value);
                //CheckUp Tabagismo
                else if (value.GetType() == typeof(Tabagismo[]))
                    return Enum<Tabagismo>.GetCustomDisplay();
                else if (value.GetType() == typeof(Tabagismo))
                    return Enum<Tabagismo>.GetCustomDisplayOf((Tabagismo)value);
                //CheckUp Atividade Fisica
                else if (value.GetType() == typeof(AtividadeFisica[]))
                    return Enum<AtividadeFisica>.GetCustomDisplay();
                else if (value.GetType() == typeof(AtividadeFisica))
                    return Enum<AtividadeFisica>.GetCustomDisplayOf((AtividadeFisica)value);
                else if (value.GetType() == typeof(SimNao))
                    return Enum<SimNao>.GetCustomDisplayOf((SimNao)value);
                //Admisao Centro Obstetrico - Tipagem
                else if (value.GetType() == typeof(Tipagem[]))
                    return Enum<Tipagem>.GetCustomDisplay();
                else if (value.GetType() == typeof(Tipagem))
                    return Enum<Tipagem>.GetCustomDisplayOf((Tipagem)value);
                //Admisao Centro Obstetrico - FatorRH
                else if (value.GetType() == typeof(FatorRH[]))
                    return Enum<FatorRH>.GetCustomDisplay();
                else if (value.GetType() == typeof(FatorRH))
                    return Enum<FatorRH>.GetCustomDisplayOf((FatorRH)value);
                //Admisao Centro Obstetrico - Dor
                else if (value.GetType() == typeof(Dor[]))
                    return Enum<Dor>.GetCustomDisplay();
                else if (value.GetType() == typeof(Dor))
                    return Enum<Dor>.GetCustomDisplayOf((Dor)value);
                //Admisao CTI NEO - Dor
                else if (value.GetType() == typeof(HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO.Dor[]))
                    return Enum<HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO.Dor>.GetCustomDisplay();
                else if (value.GetType() == typeof(HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO.Dor))
                    return Enum<HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO.Dor>.GetCustomDisplayOf((HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO.Dor)value);
                //Admisao CO - Dor
                else if (value.GetType() == typeof(HMV.Core.Domain.Enum.CentroObstetrico.Dor[]))
                    return Enum<HMV.Core.Domain.Enum.CentroObstetrico.Dor>.GetCustomDisplay();
                else if (value.GetType() == typeof(HMV.Core.Domain.Enum.CentroObstetrico.Dor))
                    return Enum<HMV.Core.Domain.Enum.CentroObstetrico.Dor>.GetCustomDisplayOf((HMV.Core.Domain.Enum.CentroObstetrico.Dor)value);
                else if (value.GetType() == typeof(HMV.Core.Domain.Enum.StatusPresencaPraticaSupervisionada))
                    return Enum<HMV.Core.Domain.Enum.StatusPresencaPraticaSupervisionada>.GetCustomDisplayOf((HMV.Core.Domain.Enum.StatusPresencaPraticaSupervisionada)value);
                else if (value.GetType() == typeof(HMV.Core.Domain.Enum.StatusPresencaPraticaSupervisionada[]))
                    return Enum<HMV.Core.Domain.Enum.StatusPresencaPraticaSupervisionada>.GetCustomDisplay();
                
                #region Transferencia Assistencial 

                else if (value.GetType() == typeof(CaloriaTipo[]))
                    return Enum<CaloriaTipo>.GetCustomDisplay();
                else if (value.GetType() == typeof(CaloriaTipo))
                    return Enum<CaloriaTipo>.GetCustomDisplayOf((CaloriaTipo)value);

                else if (value.GetType() == typeof(CentralLocalizacao[]))
                    return Enum<CentralLocalizacao>.GetCustomDisplay();
                else if (value.GetType() == typeof(CentralLocalizacao))
                    return Enum<CentralLocalizacao>.GetCustomDisplayOf((CentralLocalizacao)value);
                
                else if (value.GetType() == typeof(CentralTipo[]))
                    return Enum<CentralTipo>.GetCustomDisplay();
                else if (value.GetType() == typeof(CentralTipo))
                    return Enum<CentralTipo>.GetCustomDisplayOf((CentralTipo)value);

                else if (value.GetType() == typeof(CentralTipoHemodialise[]))
                    return Enum<CentralTipoHemodialise>.GetCustomDisplay();
                else if (value.GetType() == typeof(CentralTipoHemodialise))
                    return Enum<CentralTipoHemodialise>.GetCustomDisplayOf((CentralTipoHemodialise)value);

                else if (value.GetType() == typeof(CondicaoTransferencia[]))
                    return Enum<CondicaoTransferencia>.GetCustomDisplay();
                else if (value.GetType() == typeof(CondicaoTransferencia))
                    return Enum<CondicaoTransferencia>.GetCustomDisplayOf((CondicaoTransferencia)value);

                else if (value.GetType() == typeof(DrenosLocalizacao[]))
                    return Enum<DrenosLocalizacao>.GetCustomDisplay();
                else if (value.GetType() == typeof(DrenosLocalizacao))
                    return Enum<DrenosLocalizacao>.GetCustomDisplayOf((DrenosLocalizacao)value);

                else if (value.GetType() == typeof(DrenosTipo[]))
                    return Enum<DrenosTipo>.GetCustomDisplay();
                else if (value.GetType() == typeof(DrenosTipo))
                    return Enum<DrenosTipo>.GetCustomDisplayOf((DrenosTipo)value);

                else if (value.GetType() == typeof(GrauDeDependencia[]))
                    return Enum<GrauDeDependencia>.GetCustomDisplay();
                else if (value.GetType() == typeof(GrauDeDependencia))
                    return Enum<GrauDeDependencia>.GetCustomDisplayOf((GrauDeDependencia)value);

                else if (value.GetType() == typeof(MotivoTransferencia[]))
                    return Enum<MotivoTransferencia>.GetCustomDisplay();
                else if (value.GetType() == typeof(MotivoTransferencia))
                    return Enum<MotivoTransferencia>.GetCustomDisplayOf((MotivoTransferencia)value);

                else if (value.GetType() == typeof(OstomiaLocalizacao[]))
                    return Enum<OstomiaLocalizacao>.GetCustomDisplay();
                else if (value.GetType() == typeof(OstomiaLocalizacao))
                    return Enum<OstomiaLocalizacao>.GetCustomDisplayOf((OstomiaLocalizacao)value);

                else if (value.GetType() == typeof(PerifericoLocalizacao[]))
                    return Enum<PerifericoLocalizacao>.GetCustomDisplay();
                else if (value.GetType() == typeof(PerifericoLocalizacao))
                    return Enum<PerifericoLocalizacao>.GetCustomDisplayOf((PerifericoLocalizacao)value);

                else if (value.GetType() == typeof(RiscoNutricional[]))
                    return Enum<RiscoNutricional>.GetCustomDisplay();
                else if (value.GetType() == typeof(RiscoNutricional))
                    return Enum<RiscoNutricional>.GetCustomDisplayOf((RiscoNutricional)value);

                else if (value.GetType() == typeof(SondasLocalizacao[]))
                    return Enum<SondasLocalizacao>.GetCustomDisplay();
                else if (value.GetType() == typeof(SondasLocalizacao))
                    return Enum<SondasLocalizacao>.GetCustomDisplayOf((SondasLocalizacao)value);

                else if (value.GetType() == typeof(SuporteVentilatorio[]))
                    return Enum<SuporteVentilatorio>.GetCustomDisplay();
                else if (value.GetType() == typeof(SuporteVentilatorio))
                    return Enum<SuporteVentilatorio>.GetCustomDisplayOf((SuporteVentilatorio)value);

                else if (value.GetType() == typeof(UlceraCategoria[]))
                    return Enum<UlceraCategoria>.GetCustomDisplay();
                else if (value.GetType() == typeof(UlceraCategoria))
                    return Enum<UlceraCategoria>.GetCustomDisplayOf((UlceraCategoria)value);

                else if (value.GetType() == typeof(VentilacaoEspontaneaTipo[]))
                    return Enum<VentilacaoEspontaneaTipo>.GetCustomDisplay();
                else if (value.GetType() == typeof(VentilacaoEspontaneaTipo))
                    return Enum<VentilacaoEspontaneaTipo>.GetCustomDisplayOf((VentilacaoEspontaneaTipo)value);

                else if (value.GetType() == typeof(VentilacaoEspontaneaTipoO2[]))
                    return Enum<VentilacaoEspontaneaTipoO2>.GetCustomDisplay();
                else if (value.GetType() == typeof(VentilacaoEspontaneaTipoO2))
                    return Enum<VentilacaoEspontaneaTipoO2>.GetCustomDisplayOf((VentilacaoEspontaneaTipoO2)value);

                else if (value.GetType() == typeof(VentilacaoMecanicaTipo[]))
                    return Enum<VentilacaoMecanicaTipo>.GetCustomDisplay();
                else if (value.GetType() == typeof(VentilacaoMecanicaTipo))
                    return Enum<VentilacaoMecanicaTipo>.GetCustomDisplayOf((VentilacaoMecanicaTipo)value);

                else if (value.GetType() == typeof(VentilacaoNaoInvasivaTipo[]))
                    return Enum<VentilacaoNaoInvasivaTipo>.GetCustomDisplay();
                else if (value.GetType() == typeof(VentilacaoNaoInvasivaTipo))
                    return Enum<VentilacaoNaoInvasivaTipo>.GetCustomDisplayOf((VentilacaoNaoInvasivaTipo)value);

                else if (value.GetType() == typeof(ViaAreaTipo[]))
                    return Enum<ViaAreaTipo>.GetCustomDisplay();
                else if (value.GetType() == typeof(ViaAreaTipo))
                    return Enum<ViaAreaTipo>.GetCustomDisplayOf((ViaAreaTipo)value);
            
                #endregion 

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //StatusMedicamentosEmUso
            if (targetType == typeof(StatusMedicamentosEmUso) || targetType == typeof(StatusMedicamentosEmUso?))
            {
                if (value.ToString() == "Em Uso")
                    return StatusMedicamentosEmUso.EmUso;
                else if (value.ToString() == "Encerrado")
                    return StatusMedicamentosEmUso.Encerrado;
                else if (value.ToString() == "Excluído")
                    return StatusMedicamentosEmUso.Excluído;
                else if (value.ToString() == "Sem uso de medicamentos")
                    return StatusMedicamentosEmUso.NaoFazUso;
            }
            //TipoDeFormacao
            else if (targetType == typeof(TipoDeFormacao) || targetType == typeof(TipoDeFormacao?))
            {
                if (value.ToString() == "Graduação")
                    return TipoDeFormacao.Graduacao;
                else if (value.ToString() == "Especialização")
                    return TipoDeFormacao.Especializacao;
                else if (value.ToString() == "Mestrado")
                    return TipoDeFormacao.Mestrado;
                else if (value.ToString() == "Doutorado")
                    return TipoDeFormacao.Doutorado;
                else if (value.ToString() == "Pós-Doutorado")
                    return TipoDeFormacao.PosDoutorado;
            }

              //Status Curso
            else if (targetType == typeof(StatusCurso) || targetType == typeof(StatusCurso?))
            {
                if (value.ToString() == "A iniciar")
                    return StatusCurso.Iniciar;
                else if (value.ToString() == "Ativo")
                    return StatusCurso.Ativo;
                else if (value.ToString() == "Encerrado")
                    return StatusCurso.Encerrado;
            }

              //StatusMatricula
            else if (targetType == typeof(StatusMatricula) || targetType == typeof(StatusMatricula?))
            {
                foreach (StatusMatricula item in Enum<StatusMatricula>.GetAll())
                {
                    if (Enum<StatusMatricula>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }
            //StatusMatriculaDisciplina
            else if (targetType == typeof(StatusMatriculaDisciplina) || targetType == typeof(StatusMatriculaDisciplina?))
            {
                foreach (StatusMatriculaDisciplina item in Enum<StatusMatriculaDisciplina>.GetAll())
                {
                    if (Enum<StatusMatriculaDisciplina>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }
            //OrigemPagamentoRPA
            else if (targetType == typeof(OrigemPagamentoRPA) || targetType == typeof(OrigemPagamentoRPA?))
            {
                foreach (OrigemPagamentoRPA item in Enum<OrigemPagamentoRPA>.GetAll())
                {
                    if (Enum<OrigemPagamentoRPA>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }
            //CheckUp Tabagismo
            else if (targetType == typeof(Tabagismo) || targetType == typeof(Tabagismo?))
            {
                foreach (Tabagismo item in Enum<Tabagismo>.GetAll())
                {
                    if (Enum<Tabagismo>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }
            //CheckUp Atividade Fisica
            else if (targetType == typeof(AtividadeFisica) || targetType == typeof(AtividadeFisica?))
            {
                foreach (AtividadeFisica item in Enum<AtividadeFisica>.GetAll())
                {
                    if (Enum<AtividadeFisica>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }

            // SimNao
            else if (targetType == typeof(SimNao) || targetType == typeof(SimNao?))
            {
                foreach (SimNao item in Enum<SimNao>.GetAll())
                {
                    if (Enum<SimNao>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }

            // admissao centro obstetrico - Tipagem
            else if (targetType == typeof(Tipagem) || targetType == typeof(Tipagem?))
            {
                foreach (Tipagem item in Enum<Tipagem>.GetAll())
                {
                    if (Enum<Tipagem>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }
            // admissao centro obstetrico - FatorRH
            else if (targetType == typeof(FatorRH) || targetType == typeof(FatorRH?))
            {
                foreach (FatorRH item in Enum<FatorRH>.GetAll())
                {
                    if (Enum<FatorRH>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }
            // admissao centro obstetrico - Dor
            else if (targetType == typeof(Dor) || targetType == typeof(Dor?))
            {
                foreach (Dor item in Enum<Dor>.GetAll())
                {
                    if (Enum<Dor>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }
            // admissao CTI NEO - Dor
            else if (targetType == typeof(HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO.Dor) || targetType == typeof(HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO.Dor?))
            {
                foreach (HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO.Dor item in Enum<HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO.Dor>.GetAll())
                {
                    if (Enum<HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO.Dor>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }
            // admissao CO - Dor
            else if (targetType == typeof(HMV.Core.Domain.Enum.CentroObstetrico.Dor) || targetType == typeof(HMV.Core.Domain.Enum.CentroObstetrico.Dor?))
            {
                foreach (HMV.Core.Domain.Enum.CentroObstetrico.Dor item in Enum<HMV.Core.Domain.Enum.CentroObstetrico.Dor>.GetAll())
                {
                    if (Enum<HMV.Core.Domain.Enum.CentroObstetrico.Dor>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }
            else if (targetType == typeof(HMV.Core.Domain.Enum.StatusPresencaPraticaSupervisionada) || targetType == typeof(HMV.Core.Domain.Enum.StatusPresencaPraticaSupervisionada?))
            {
                if (value == null) return null;
                foreach (HMV.Core.Domain.Enum.StatusPresencaPraticaSupervisionada item in Enum<HMV.Core.Domain.Enum.StatusPresencaPraticaSupervisionada>.GetAll())
                {
                    if (Enum<HMV.Core.Domain.Enum.StatusPresencaPraticaSupervisionada>.GetCustomDisplayOf(item).Equals(value.ToString()))
                        return item;
                }
            }

            #region Transferencia Assistencial 
            
            else if (targetType == typeof(CaloriaTipo) || targetType == typeof(CaloriaTipo?))
            {
                if (value.IsNotNull())
                {
                    foreach (CaloriaTipo item in Enum<CaloriaTipo>.GetAll())
                    {
                        if (Enum<CaloriaTipo>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(CentralLocalizacao) || targetType == typeof(CentralLocalizacao?))
            {
                if (value.IsNotNull())
                {
                    foreach (CentralLocalizacao item in Enum<CentralLocalizacao>.GetAll())
                    {
                        if (Enum<CentralLocalizacao>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(CentralTipo) || targetType == typeof(CentralTipo?))
            {
                if (value.IsNotNull())
                {
                    foreach (CentralTipo item in Enum<CentralTipo>.GetAll())
                    {
                        if (Enum<CentralTipo>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(CentralTipoHemodialise) || targetType == typeof(CentralTipoHemodialise?))
            {
                if (value.IsNotNull())
                {
                    foreach (CentralTipoHemodialise item in Enum<CentralTipoHemodialise>.GetAll())
                    {
                        if (Enum<CentralTipoHemodialise>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(CondicaoTransferencia) || targetType == typeof(CondicaoTransferencia?))
            {
                if (value.IsNotNull())
                {
                    foreach (CondicaoTransferencia item in Enum<CondicaoTransferencia>.GetAll())
                    {
                        if (Enum<CondicaoTransferencia>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(DrenosLocalizacao) || targetType == typeof(DrenosLocalizacao?))
            {
                if (value.IsNotNull())
                {
                    foreach (DrenosLocalizacao item in Enum<DrenosLocalizacao>.GetAll())
                    {
                        if (Enum<DrenosLocalizacao>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(DrenosTipo) || targetType == typeof(DrenosTipo?))
            {
                if (value.IsNotNull())
                {
                    foreach (DrenosTipo item in Enum<DrenosTipo>.GetAll())
                    {
                        if (Enum<DrenosTipo>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(GrauDeDependencia) || targetType == typeof(GrauDeDependencia?))
            {
                if (value.IsNotNull())
                {
                    foreach (GrauDeDependencia item in Enum<GrauDeDependencia>.GetAll())
                    {
                        if (Enum<GrauDeDependencia>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(MotivoTransferencia) || targetType == typeof(MotivoTransferencia?))
            {
                if (value.IsNotNull())
                {
                    foreach (MotivoTransferencia item in Enum<MotivoTransferencia>.GetAll())
                    {
                        if (Enum<MotivoTransferencia>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(OstomiaLocalizacao) || targetType == typeof(OstomiaLocalizacao?))
            {
                if (value.IsNotNull())
                {
                    foreach (OstomiaLocalizacao item in Enum<OstomiaLocalizacao>.GetAll())
                    {
                        if (Enum<OstomiaLocalizacao>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(PerifericoLocalizacao) || targetType == typeof(PerifericoLocalizacao?))
            {
                if (value.IsNotNull())
                {
                    foreach (PerifericoLocalizacao item in Enum<PerifericoLocalizacao>.GetAll())
                    {
                        if (Enum<PerifericoLocalizacao>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(RiscoNutricional) || targetType == typeof(RiscoNutricional?))
            {
                if (value.IsNotNull())
                {
                    foreach (RiscoNutricional item in Enum<RiscoNutricional>.GetAll())
                    {
                        if (Enum<RiscoNutricional>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(SondasLocalizacao) || targetType == typeof(SondasLocalizacao?))
            {
                if (value.IsNotNull())
                {
                    foreach (SondasLocalizacao item in Enum<SondasLocalizacao>.GetAll())
                    {
                        if (Enum<SondasLocalizacao>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(SuporteVentilatorio) || targetType == typeof(SuporteVentilatorio?))
            {
                if (value.IsNotNull())
                {
                    foreach (SuporteVentilatorio item in Enum<SuporteVentilatorio>.GetAll())
                    {
                        if (Enum<SuporteVentilatorio>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(UlceraCategoria) || targetType == typeof(UlceraCategoria?))
            {
                if (value.IsNotNull())
                {
                    foreach (UlceraCategoria item in Enum<UlceraCategoria>.GetAll())
                    {
                        if (Enum<UlceraCategoria>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(VentilacaoEspontaneaTipo) || targetType == typeof(VentilacaoEspontaneaTipo?))
            {
                if (value.IsNotNull())
                {
                    foreach (VentilacaoEspontaneaTipo item in Enum<VentilacaoEspontaneaTipo>.GetAll())
                    {
                        if (Enum<VentilacaoEspontaneaTipo>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(VentilacaoEspontaneaTipoO2) || targetType == typeof(VentilacaoEspontaneaTipoO2?))
            {
                if (value.IsNotNull())
                {
                    foreach (VentilacaoEspontaneaTipoO2 item in Enum<VentilacaoEspontaneaTipoO2>.GetAll())
                    {
                        if (Enum<VentilacaoEspontaneaTipoO2>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(VentilacaoMecanicaTipo) || targetType == typeof(VentilacaoMecanicaTipo?))
            {
                if (value.IsNotNull())
                {
                    foreach (VentilacaoMecanicaTipo item in Enum<VentilacaoMecanicaTipo>.GetAll())
                    {
                        if (Enum<VentilacaoMecanicaTipo>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(VentilacaoNaoInvasivaTipo) || targetType == typeof(VentilacaoNaoInvasivaTipo?))
            {
                if (value.IsNotNull())
                {
                    foreach (VentilacaoNaoInvasivaTipo item in Enum<VentilacaoNaoInvasivaTipo>.GetAll())
                    {
                        if (Enum<VentilacaoNaoInvasivaTipo>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }
            else if (targetType == typeof(ViaAreaTipo) || targetType == typeof(ViaAreaTipo?))
            {
                if (value.IsNotNull())
                {
                    foreach (ViaAreaTipo item in Enum<ViaAreaTipo>.GetAll())
                    {
                        if (Enum<ViaAreaTipo>.GetCustomDisplayOf(item).Equals(value.ToString()))
                            return item;
                    }
                }
            }

            #endregion 

            return null;
        }
        #endregion
    }

    public class EnumToStringConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() != "{DependencyProperty.UnsetValue}")
                return value.ToString();

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class SimNaoToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(SimNao))
                if ((SimNao)value == SimNao.Sim)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return SimNao.Nao;
            if ((bool)value)
                return SimNao.Sim;
            return SimNao.Nao;
        }
        #endregion
    }

    public class SimNaoToOppositeBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if ((SimNao)value == SimNao.Sim)
                return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return SimNao.Nao;
            if ((bool)value)
                return SimNao.Nao;
            return SimNao.Sim;
        }
        #endregion
    }

    public class SimTypeSimNaoNRToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(SimNaoNR))
                if ((SimNaoNR)value == SimNaoNR.Sim)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return SimNaoNR.Sim;
        }
        #endregion
    }

    public class NaoTypeSimNaoNRToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(SimNaoNR))
                if ((SimNaoNR)value == SimNaoNR.Nao)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return SimNaoNR.Nao;
        }
        #endregion
    }

    public class NRTypeSimNaoNRToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(SimNaoNR))
                if ((SimNaoNR)value == SimNaoNR.NaoRealizado)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return SimNaoNR.NaoRealizado;
        }
        #endregion
    }

    public class CasaTypeTipoDeHabitacaoToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(TipoDeHabitacao))
                if ((TipoDeHabitacao)value == TipoDeHabitacao.Casa)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TipoDeHabitacao.Casa;
        }
        #endregion
    }

    public class ApartamentoTypeTipoDeHabitacaoToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(TipoDeHabitacao))
                if ((TipoDeHabitacao)value == TipoDeHabitacao.Apartamento)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TipoDeHabitacao.Apartamento;
        }
        #endregion
    }

    public class OutrosTypeTipoDeHabitacaoToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(TipoDeHabitacao))
                if ((TipoDeHabitacao)value == TipoDeHabitacao.Outros)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TipoDeHabitacao.Outros;
        }
        #endregion
    }

    public class PropriaTypePropriedadeToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(Propriedade))
                if ((Propriedade)value == Propriedade.Propria)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Propriedade.Propria;
        }
        #endregion
    }

    public class AlugadaTypePropriedadeToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(Propriedade))
                if ((Propriedade)value == Propriedade.Alugada)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Propriedade.Alugada;
        }
        #endregion
    }

    public class OutrosTypePropriedadeToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(Propriedade))
                if ((Propriedade)value == Propriedade.Outros)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Propriedade.Outros;
        }
        #endregion
    }

    public class SimNaoToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if (value.GetType() == typeof(SimNao))
                if ((SimNao)value == SimNao.Sim)
                    return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return SimNao.Nao;
            if ((Visibility)value == Visibility.Visible)
                return SimNao.Sim;
            return SimNao.Nao;
        }
        #endregion
    }

    public class SimNaoToOppositeVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;
            if (value.GetType() == typeof(SimNao))
                if ((SimNao)value == SimNao.Sim)
                    return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return SimNao.Sim;
            if ((Visibility)value == Visibility.Visible)
                return SimNao.Nao;
            return SimNao.Sim;
        }
        #endregion
    }

    public class SimNaoToImagemDefault : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(SimNao))
            {
                if ((SimNao)value == SimNao.Sim)
                    return new BitmapImage(new Uri(Application.Current.FindResource("imgDefault").ToString()));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class MethodToValueConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var methodName = parameter as string;
            if (value == null || methodName == null)
                return value;
            var methodInfo = value.GetType().GetMethod(methodName, new Type[0]);
            if (methodInfo == null)
                return value;
            return methodInfo.Invoke(value, new object[0]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter apenas pode ser usado para conversões MODE=ONEWAY.");
        }
        #endregion
    }

    public class NumeroSistemaToDescricaoSistemaConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int id = 0;
            if (int.TryParse(value.ToString(), out id))
            {
                Sistemas sistema = StructureMap.ObjectFactory.GetInstance<ISistemaService>().FiltraPorId(id);
                if (sistema != null)
                    return sistema.DescricaoDetalhada;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class BoolToImageMVMonitorMonitorConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(bool))
            {
                if ((bool)value)
                    return new BitmapImage(new Uri(Application.Current.FindResource("imgMonitorPC").ToString()));
                return new BitmapImage(new Uri(Application.Current.FindResource("imgMonitorSistema").ToString()));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class BoolToImageComAnexoSemAnexoConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                if (value.GetType() == typeof(bool))
                {
                    if ((bool)value)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgRelatorioComAnexo").ToString()));
                    return new BitmapImage(new Uri(Application.Current.FindResource("imgRelatorioSemAnexo").ToString()));
                }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class BoolToImageProcuraArquivoConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(bool))
                {
                    if ((bool)value)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgSearchArquivo").ToString()));
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class BoolToImageVisualizarOuAnexarArquivoConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(bool))
                {
                    if ((bool)value)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgSearchArquivo").ToString()));
                    else
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgAnexo").ToString()));
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class JustificativaFaltaToImageConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(JustificativaFalta))
                {
                    if ((JustificativaFalta)value == JustificativaFalta.Aproveitamento)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgAproveitamento").ToString()));
                    else if ((JustificativaFalta)value == JustificativaFalta.Cancelamento)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgCancelamento").ToString()));
                    else if ((JustificativaFalta)value == JustificativaFalta.ExercicioDomiciliar)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgExercicioDomiciliar").ToString()));
                    else if ((JustificativaFalta)value == JustificativaFalta.Extraordinario)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgAproveitamentoExtraordinario").ToString()));
                    else if ((JustificativaFalta)value == JustificativaFalta.InformarFalta)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgSearch").ToString()));
                }
            }
            return null;// new BitmapImage(new Uri(Application.Current.FindResource("imgBranco").ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class StatusToImageConfirmCloseConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                if (value.GetType() == typeof(Status))
                    if ((Status)value == Status.Ativo)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgConfirm").ToString()));
                    else if ((Status)value == Status.Inativo)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgClose").ToString()));

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class BoolToImagemCheckUncheckFavoritoConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(bool))
            {
                if ((bool)value)
                    return new BitmapImage(new Uri(Application.Current.FindResource("imgDefault").ToString()));
                return new BitmapImage(new Uri(Application.Current.FindResource("imgFavoritoDesabilitado").ToString()));

            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class BoolToImagemConfirmarConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(bool))
            {
                if ((bool)value)
                    return new BitmapImage(new Uri(Application.Current.FindResource("imgConfirm").ToString()));
                return new BitmapImage(new Uri(Application.Current.FindResource("imgConfirm").ToString()));

            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class BoolToImagemConfirmarCopiaConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(bool))
            {
                if ((bool)value)
                    return new BitmapImage(new Uri(Application.Current.FindResource("imgConfirm").ToString()));
                return new BitmapImage(new Uri(Application.Current.FindResource("imgClose").ToString()));

            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class BoolToImageExcluirCancelarConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(bool))
                {
                    if ((bool)value)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgExcluirCancelar").ToString()));
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class BoolToImageProcessosEnfermagemProcessosMedicosConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                if (value.GetType() == typeof(bool))
                {
                    if ((bool)value)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgProcessosEnfermagem").ToString()));
                    return new BitmapImage(new Uri(Application.Current.FindResource("imgProcessosMedico").ToString()));
                }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class StringLengthToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            return ((string)value).Trim().Length > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class IntToFontWeightConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 1)
            {
                return FontWeights.Bold;
            }

            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class SituacaoEmergenciaToImagemConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(SituacaoEmergenciaImagem))
                {
                    if ((SituacaoEmergenciaImagem)value == SituacaoEmergenciaImagem.Azul)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgBlueClock").ToString()));
                    else if ((SituacaoEmergenciaImagem)value == SituacaoEmergenciaImagem.Amarelo)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgYellowClock").ToString()));
                    else if ((SituacaoEmergenciaImagem)value == SituacaoEmergenciaImagem.Vermelho)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgRedClock").ToString()));
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class SituacaoEmergenciaToStringConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(SituacaoEmergenciaImagem))
                {
                    if ((SituacaoEmergenciaImagem)value == SituacaoEmergenciaImagem.Azul)
                        return "Atendimento no prazo.";
                    else if ((SituacaoEmergenciaImagem)value == SituacaoEmergenciaImagem.Amarelo)
                        return "Atendimento com metade do tempo para vencer o prazo.";
                    else if ((SituacaoEmergenciaImagem)value == SituacaoEmergenciaImagem.Vermelho)
                        return "Vencendo prazo de atendimento.";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class BoolToImagemCheckUncheckConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                if (value.GetType() == typeof(bool))
                {
                    if ((bool)value)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgCheckedNew").ToString()));

                    return new BitmapImage(new Uri(Application.Current.FindResource("imgUnCheckedNew").ToString()));
                }
            return new BitmapImage(new Uri(Application.Current.FindResource("imgUnCheckedNew").ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class IntToDateTimeConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int ret = 0;
            if (value != null && (int)value > 0)
                if (int.TryParse(value.ToString(), out ret))
                {
                    string date = string.Empty;
                    if (value.ToString().Length == 4)
                        date = ret.ToString() + "0101";
                    else if (value.ToString().Length == 6)
                        date = ret.ToString() + "01";
                    else
                        date = ret.ToString();

                    DateTime dateret;
                    try
                    {
                        dateret = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                        return dateret;
                    }
                    catch
                    { return null; }
                }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (value.GetType() == typeof(DateTime) || value.GetType() == typeof(DateTime?)))
            {
                DateTime date = (value as DateTime?).Value;

                if (parameter.ToString() == "yyyyMM")
                    return date.Year * 100 + date.Month;
                else
                    if (parameter.ToString() == "yyyy")
                        return date.Year;

                return date.Year * 10000 + date.Month * 100 + date.Day;
            }
            return null;
        }
        #endregion
    }

    public class StringToNumberConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType() == typeof(String))
                return (value as string).RemoveNonNumber();
            return string.Empty;
        }
        #endregion
    }

    public class DoubleToNullConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.ToString().IndexOf('.') > 0)
                    return value.ToString().Replace('.', ',');
                return value.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()) && value.ToString() != ",")
                    if (value.ToString().IndexOf('.') > 0)
                        return value.ToString().Replace('.', ',');
                return value;
            }
            return null;
        }
        #endregion
    }

    public class IntToNullConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return value.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    return value;
            return null;
        }
        #endregion
    }

    public class BinaryToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is byte[])
            {
                try
                {
                    byte[] bytes = value as byte[];

                    MemoryStream stream = new MemoryStream(bytes);

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.EndInit();

                    return image;
                }
                catch { return null; }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (typeof(BitmapImage) == value.GetType())
            {
                FileStream fs = new FileStream(((System.IO.FileStream)(value as BitmapImage).StreamSource).Name, FileMode.Open, FileAccess.Read);
                byte[] ImageData = new byte[fs.Length];
                fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();
                return ImageData;
            }
            return null;
        }
        #endregion
    }

    public class DateTimeActualMonthToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                if (value.GetType() == typeof(string))
                {
                    DateTime ret = DateTime.Now.Date;
                    string val = string.Empty;
                    if (value.ToString().IndexOf('-') > 0)
                        val = value.ToString().Substring(0, value.ToString().IndexOf('-')).Trim();
                    else
                        val = value.ToString();
                    if (DateTime.TryParseExact(val, "MMMM yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out ret))
                        if (ret.Month < DateTime.Now.Month || ret.Year < DateTime.Now.Year)
                            return Visibility.Visible;
                }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
                return true;
            return false;
        }
        #endregion
    }

    public class SimNaoToImageConfirmCloseConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(SimNao))
                    if ((SimNao)value == SimNao.Sim)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgConfirm").ToString()));
                    else
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgClose").ToString()));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class SimNaoToImageConfirmNULLConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(SimNao))
                    if ((SimNao)value == SimNao.Sim)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgConfirm").ToString()));
                    else
                        return null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class FormatoArquivoToImage : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(FormatoArquivo))
                {
                    return new BitmapImage(new Uri(Application.Current.FindResource(HMV.Core.Framework.Types.Enum<FormatoArquivo>.GetCustomDisplayOf((FormatoArquivo)value)).ToString()));
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return FormatoArquivo.Desconhecido;
        }
        #endregion
    }

    public class StatusToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(Status))
                if ((Status)value == Status.Ativo)
                    return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Status.Inativo;
            if ((bool)value)
                return Status.Ativo;
            return Status.Inativo;
        }
        #endregion
    }

    public class DateTimeToTimeString : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                if (value.GetType() == typeof(DateTime) || value.GetType() == typeof(DateTime?))
                    return (value as DateTime?).Value.TimeOfDay.ToString();

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
        #endregion
    }

    public class IntToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                if (value.GetType() == typeof(int))
                    if ((int)value > 0)
                    {
                        return true;
                    }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class StringLengthToOppositeBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;

            return !(((string)value).Trim().Length > 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class StatusAlergiaProblemaToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() == typeof(StatusAlergiaProblema))
            {
                if ((StatusAlergiaProblema)value == StatusAlergiaProblema.Ativo)
                    return true;
                if ((StatusAlergiaProblema)value == StatusAlergiaProblema.Inativo)
                    return false;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return StatusAlergiaProblema.Excluído;
            if ((bool)value)
                return StatusAlergiaProblema.Ativo;
            return StatusAlergiaProblema.Inativo;
        }
        #endregion
    }

    public class StatusAlergiaProblemaToOppositeBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() == typeof(StatusAlergiaProblema))
            {
                if ((StatusAlergiaProblema)value == StatusAlergiaProblema.Inativo)
                    return true;
                if ((StatusAlergiaProblema)value == StatusAlergiaProblema.Ativo)
                    return false;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return StatusAlergiaProblema.Excluído;
            if ((bool)value)
                return StatusAlergiaProblema.Inativo;
            return StatusAlergiaProblema.Ativo;
        }
        #endregion
    }

    public class BoolToImageExportarConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(bool))
                    if ((bool)value)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgExportar").ToString()));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class BoolToImageConfirmarConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(bool))
                    if ((bool)value)
                        return new BitmapImage(new Uri(Application.Current.FindResource("imgConfirm").ToString()));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class NullToStringEmptyConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.IsNull())
                return string.Empty;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class ListStringToComboBoxItemsSourceConveter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<string> list = (List<string>)value;

            List<DataClass> data = new List<DataClass>();
            foreach (string item in list)
            {
                data.Add(new DataClass() { Data = item });
            }
            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> list = new List<string>();
            foreach (var item in (List<DataClass>)value)
            {
                list.Add(item.Data);
            }

            return list;
        }
        #endregion
    }

    public class StringToDataClassConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DataClass { Data = value.ToNullSafeString() };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataClass)
                return (value as DataClass).Data;
            return string.Empty;
        }
        #endregion
    }
    public class DataClass
    {
        public string Data { get; set; }
    }

    public class BoolToImagemDicionario : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new BitmapImage(new Uri(Application.Current.FindResource("imgInclude").ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
    #endregion

    #region MultiValueConverters
    public class MultiBindingAndToBoolConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in values)
            {
                if (!(item is bool) || !(bool)item)
                    return false;
            }
            return true;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class MultiBindingStringToStringConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object val in values)
                if (!string.IsNullOrWhiteSpace(val.ToString()) && val.GetType() == typeof(string))
                    return val.ToString();

            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class MultiBindingIntToStringConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int Valor = 0;
            foreach (object val in values)
                if (int.TryParse(val.ToString(), out Valor))
                    return Valor.ToString();

            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class MultiBindingEnumToStringConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object val in values)
                if (val != null && val.ToString() != "{DependencyProperty.UnsetValue}")
                    return val.ToString();

            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class MultiBindingBoolToVisibilityConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object val in values)
                if (val != null)
                    if (val.GetType() == typeof(bool))
                        if ((bool)val)
                            return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class MultiBindingAgeToStringConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object val in values)
                if (val != null)
                {
                    if (val.GetType() == typeof(Age))
                    {
                        Age age = (Age)val;
                        return age.ToString(2);
                    }
                }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class CenterHeaderGroupBoxConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Type type = typeof(double);
            if (values == null
                || values.Length != 3
                || values[0] == null
                || values[1] == null
                || values[2] == null
                || !type.IsAssignableFrom(values[0].GetType())
                || !type.IsAssignableFrom(values[1].GetType())
                || !type.IsAssignableFrom(values[2].GetType()))
            {
                return DependencyProperty.UnsetValue;
            }

            double pixels = (double)values[0];
            double width = (double)values[1];
            double height = (double)values[2];
            if ((width == 0.0) || (height == 0.0))
            {
                return null;
            }
            Grid visual = new Grid();
            visual.Width = width;
            visual.Height = height;
            ColumnDefinition colDefinition1 = new ColumnDefinition();
            ColumnDefinition colDefinition2 = new ColumnDefinition();
            ColumnDefinition colDefinition3 = new ColumnDefinition();
            colDefinition1.Width = new GridLength(1.0, GridUnitType.Star);
            colDefinition2.Width = new GridLength(pixels);
            colDefinition3.Width = new GridLength(1.0, GridUnitType.Star);
            visual.ColumnDefinitions.Add(colDefinition1);
            visual.ColumnDefinitions.Add(colDefinition2);
            visual.ColumnDefinitions.Add(colDefinition3);
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            rowDefinition1.Height = new GridLength(height / 2.0);
            rowDefinition2.Height = new GridLength(1.0, GridUnitType.Star);
            visual.RowDefinitions.Add(rowDefinition1);
            visual.RowDefinitions.Add(rowDefinition2);
            Rectangle rectangle1 = new Rectangle();
            Rectangle rectangle2 = new Rectangle();
            Rectangle rectangle3 = new Rectangle();
            rectangle1.Fill = Brushes.Black;
            rectangle2.Fill = Brushes.Black;
            rectangle3.Fill = Brushes.Black;
            Grid.SetRowSpan(rectangle1, 2);
            Grid.SetRow(rectangle1, 0);
            Grid.SetColumn(rectangle1, 0);
            Grid.SetRow(rectangle2, 1);
            Grid.SetColumn(rectangle2, 1);
            Grid.SetRowSpan(rectangle3, 2);
            Grid.SetRow(rectangle3, 0);
            Grid.SetColumn(rectangle3, 2);
            visual.Children.Add(rectangle1);
            visual.Children.Add(rectangle2);
            visual.Children.Add(rectangle3);
            return new VisualBrush(visual);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { Binding.DoNothing };
        }
        #endregion
    }


    #endregion
}
