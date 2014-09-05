using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.DevExpress.v12._1.Assets.Resources;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.WPF.Helpers;
using HMV.Core.Interfaces;
using HMV.PEP.DTO;
using StructureMap;
using HMV.Core.Framework.Extensions;
using HMV.PEP.Services;
using HMV.PEP.WPF.Windows;
using HMV.PEP.WPF.Windows.PAME;
using HMV.Core.WCF.Interfaces.Acesso;
using HMV.Core.Domain.Enum;
using System.Reflection;
using System.Linq;
using DevExpress.Xpf.SpellChecker;
using DevExpress.XtraSpellChecker;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Expression;
using System.IO;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : HMVApplication
    {
        public static winPEP win;
        public static Usuarios Usuario;
        public static UsuarioDTO UsuarioDTO;
        public static string Banco;
        public static string Senha;
        public static IList<CidDTO> listaDeCid;
        public static IList<CidDTO> listaDeCidMv;
        public static bool AtivaLog;
        public static string PathLog;
        public static DateTime TempoInicialLog;
        public static DateTime TempoParcialLog;
        public static bool IsPAME = false;

        private static SpellChecker _spChecker;
        public static SpellChecker SPChecker
        {
            get
            {
                if (_spChecker.IsNull())
                {
                    _spChecker = new SpellChecker();
                    CultureInfo ipCulture = new CultureInfo("pt-br");
                    SpellCheckerOpenOfficeDictionary openOfficeDictionary = new SpellCheckerOpenOfficeDictionary();
                    openOfficeDictionary.Culture = ipCulture;                    
                    openOfficeDictionary.DictionaryPath = System.Configuration.ConfigurationManager.AppSettings["PathDicionarios"].Combine(@"pt_BR.dic");
                    openOfficeDictionary.GrammarPath = System.Configuration.ConfigurationManager.AppSettings["PathDicionarios"].Combine(@"pt_BR.aff");
                    openOfficeDictionary.AlphabetPath = System.Configuration.ConfigurationManager.AppSettings["PathDicionarios"].Combine(@"Alphabet.txt");
                    _spChecker.Dictionaries.Add(openOfficeDictionary);
                    SpellCheckerCustomDictionary customDictionary = new SpellCheckerCustomDictionary();
                    customDictionary.Culture = ipCulture;
                    customDictionary.AlphabetPath = openOfficeDictionary.AlphabetPath;
                    customDictionary.DictionaryPath = System.Configuration.ConfigurationManager.AppSettings["PathDicionarios"].Combine(@"\pt_BRCuston.dic");
                    _spChecker.Dictionaries.Add(customDictionary);
                    _spChecker.Culture = ipCulture;
                    _spChecker.LoadOnDemand = false;                     
                }
                return _spChecker;
            }            
        }

        private void App_Startup(object sender1, StartupEventArgs e)
        {           
            try
            {
                if (!ConfigurationManager.AppSettings["AtivaLogInicializacao"].IsNull() && bool.Parse(ConfigurationManager.AppSettings["AtivaLogInicializacao"].ToString()))
                {
                    AtivaLog = true;
                    TempoInicialLog = DateTime.Now;
                    PathLog = System.Configuration.ConfigurationManager.AppSettings["PathLogInicializacao"] + DateTime.Now.ToString("yyyy.MM.dd[HH.mm]") + "." + Environment.MachineName + "." + Environment.UserName + "[" + this.GetType().Assembly.GetName().Version.ToString() + "].txt";
                    ExportFileHelper.ExportStringToTXT(PathLog, "Inicializou o sistema - " + TempoInicialLog.ToString() + Environment.NewLine);
                }
                String[] command = e.Args[0].ToString().Split(',');
                String usuario = command[0];
                String senha = command[1];
                Senha = command[1];
                Banco = command[2];

                BaseServiceParametro.Banco = Banco;

                String ss = System.Configuration.ConfigurationManager.ConnectionStrings["BANCO"].ToString();

                //HMV.Core.DataAccess.SessionManager.ConfigureDataAccess(ss.Replace("@BANCO", Banco),
                //System.Configuration.ConfigurationManager.AppSettings["ConfigNHibernate"].ToString() + this.GetType().Assembly.GetName().Version.ToString().Replace(".", null));

                if (AtivaLog) 
                {
                    ExportFileHelper.ExportStringToTXT(PathLog, "Carregou o banco - " + DateTime.Now + Environment.NewLine +
                                                       "Duração: " + string.Format("{0:D2}:{1:D2}:{2:D3}", (DateTime.Now - TempoInicialLog).Minutes, (DateTime.Now - TempoInicialLog).Seconds, (DateTime.Now - TempoInicialLog).Milliseconds) + Environment.NewLine);
                    TempoParcialLog = DateTime.Now;
                }
                
                if (AtivaLog)
                {
                    ExportFileHelper.ExportStringToTXT(PathLog, "Executou os registros IOC - " + DateTime.Now + Environment.NewLine +
                                                       "Duração: " + string.Format("{0:D2}:{1:D2}:{2:D3}", (DateTime.Now - TempoParcialLog).Minutes, (DateTime.Now - TempoParcialLog).Seconds, (DateTime.Now - TempoParcialLog).Milliseconds) + Environment.NewLine +
                                                       "Total Parcial: " + string.Format("{0:D2}:{1:D2}:{2:D3}", (DateTime.Now - TempoInicialLog).Minutes, (DateTime.Now - TempoInicialLog).Seconds, (DateTime.Now - TempoInicialLog).Milliseconds) + Environment.NewLine);
                    TempoParcialLog = DateTime.Now;
                }
                
                //IUsuariosService serv = ObjectFactory.GetInstance<IUsuariosService>();
                //Usuario = serv.FiltraPorID(usuario);
                
                HMV.PEP.IoC.IoCWorker.ConfigureWIN();

                InicializacaoService serv = new InicializacaoService();
                Retorno<UsuarioDTO> ret = serv.BuscaConfiguracaoDoUsuario(usuario);

                if (ret.Banco.ToUpper() != App.Banco.ToUpper())
                    MessageBox.Show("Sistema está operando no banco(" + App.Banco + ") diferente do serviço(" + ret.Banco + ").", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);                     

                UsuarioDTO = ret.Data;


                AlergiaTipoTypeDescriptionProvider.Register();
                Siga_ProfissionalTypeDescriptionProvider.Register();
                CidTypeDescriptionProvider.Register();
                ImunizacaoTipoTypeDescriptionProvider.Register();

                XtraLocalizer<DXMessageBoxStringId>.SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DXMessageBoxStringId>(new CustomDXMEssageBoxLocalizeer()));

                Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");

                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
                IList<string> pameXpep = serv.BuscaParametrosPAMExPEP(App.UsuarioDTO.ID);

                //winTesteMenuRelatorio winTeste = new winTesteMenuRelatorio();
                //winTeste.ShowDialog(null);

                //new winEmergencia().ShowDialog(null);

                if (pameXpep.Count > 1)
                {
                    winSelPepPame win = new winSelPepPame();
                    win.Show();
                    win.WindowState = WindowState.Normal;
                    win.UpdateLayout();
                }
                else if (pameXpep.Contains("Prontuario Eletronico"))
                {
                    winPEP win = new winPEP();
                    win.Show();
                }
                else if (pameXpep.Contains("PAME"))
                {
                    winPAME win = new winPAME();
                    win.Show();
                }              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static int BuscaChaveUltimoLog(string texto)
        {
            try
            {
                HMV.Core.Framework.Web.BaseServiceParametro.Banco = App.Banco;
                int idSistema = int.Parse(ConfigurationManager.AppSettings["Sistema"].ToString());
                ILogWCF servLog = ObjectFactory.GetInstance<ILogWCF>();

                HMV.Core.DTO.LogDTO dto = servLog.BuscaUltimoLog(idSistema.ToString(), App.UsuarioDTO.ID, texto).Data;
                if (dto.IsNotNull())
                {
                    return dto.Chave.HasValue ? dto.Chave.Value : 0;
                }
                    
                return 0;
            }
            catch (Exception err)
            {
                HMVEmail.SendErro(err);
                return 0;
            }
        }

        public static void Log(Assembly pAssembly,string texto, int? chave, string observacao)
        {
            try
            {
                HMV.Core.Framework.Web.BaseServiceParametro.Banco = App.Banco;
                int idSistema = int.Parse(ConfigurationManager.AppSettings["Sistema"].ToString());
                ILogWCF servLog = ObjectFactory.GetInstance<ILogWCF>();
                HMV.Core.DTO.LogDTO log = new HMV.Core.DTO.LogDTO
                {
                    IDSistema = idSistema,
                    Acao = Acao.Inserir.ToString(),
                    IDUsuario = App.Usuario.ID,
                    Chave = chave,
                    Data = DateTime.Now,
                    Tabela = texto,
                    Observacao = new GetSettings().Versao + " " + observacao,
                    Dispositivo = Environment.MachineName
                };

                servLog.SalvarLog(log);
            }
            catch (Exception err)
            {
                HMVEmail.SendErro(err, pAssembly);
            }
        }

        public static void Log(Assembly pAssembly, string texto, int? chave)
        {
            App.Log(pAssembly, texto, chave, string.Empty);
        }
    }

    public class GetSettings
    {
        public GetSettings(){}

        public string Versao
        {
            get { return "Versão: " + this.GetType().Assembly.GetName().Version.ToString(); }
        }

        public string Titulo
        {
            get { return "Prontuário Eletrônico Paciente [" + this.Versao + "]"; }
        }

        public string Sistema
        {
            get
            {
                return ConfigurationManager.AppSettings["Sistema"].ToString();
            }
        }

        public string Login
        {
            get
            {
                return App.UsuarioDTO.ID + " - " + App.UsuarioDTO.Nome;
            }
        }

        public string Nome
        {
            get
            {
                return "Prezado Dr(a). " + App.UsuarioDTO.Nome + " Verifique se o paciente selecionado está correto.";
            }
        }

        public bool IsCorpoClinico
        {
            get
            {
                Prestador prestador = ObjectFactory.GetInstance<IPrestadorService>().FiltraPorId(App.UsuarioDTO.Prestador.ID);                
                return prestador != null && prestador.IsCorpoClinico;
            }
        }

        public string Banco
        {
            get
            {
                return App.Banco;
            }
        }

        public string CodigoUsuario
        {
            get
            {
                return App.UsuarioDTO.ID;
            }
        }

        public void Refresh()
        {
            ConfigurationManager.RefreshSection("appSettings");
        }
    }    
}
