using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winEvolucoesEmergencia.xaml
    /// </summary>
    public partial class winEvolucoesEmergencia : Window
    {
        public winEvolucoesEmergencia()
        {
            InitializeComponent();

            MontaGrid();
        }

        private void MontaGrid()
        {
            List<EvolucaoEmergencia> nova = new List<EvolucaoEmergencia>();

            nova.Add(new EvolucaoEmergencia
            {
                Atendimento = 111111111,
                Idade = "98 anos",
                Leito = "V02",
                Medico = "Miguel Gus",
                NomePaciente = "Isak Silva Silva",
                ListaProblemas = "Cardiopata isquemico grave, Stent há 1 ano\n" +
                                 "Iniciou as 2:00 com dor precordial + sudorese\n" +
                                 "ECG semelhante ao anterior\n" +
                                 "Leuc:5520 Na:137 K:3,8   Ur;55   Creat;1,11   Trop<0,16  CK-MB:3,1\n" +
                                 "BNP:325\n" +
                                 "Ecocardio 06/12:FE 25%    Hipocinesia global"
                ,
                Plano = "Pedir nova série às 11:00 hs "
            });
            nova.Add(new EvolucaoEmergencia
            {
                Atendimento = 222222222,
                Idade = "80 anos",
                Leito = "V03",
                Medico = "Pierangelo",
                NomePaciente = "Nathan Lima",
                ListaProblemas = "# CI\n" +
                                 "# HAS\n" +
                                 "# Hipotireoideo\n" +
                                 "# ICC em vias de compensação\n" +
                                 "#Tratou infecção respiratoria com clavulin + levo\n" +
                                 "IRA \n" +
                                 "Dispneia aos minimos esforços há 40 dias // 2 cursos de atb // Perda de função renal\n" +
                                 "ALÉRGICO A CONTRASTE\n" +
                                 "LABS:\n" +
                                 "Leuc:12330     Creat;2,47   Na:135    K:4,0   Ur:130    Trop<0,16",
                Plano = "Internado\n" +
                        "Liberado pro leito"
            });
            nova.Add(new EvolucaoEmergencia
            {
                Atendimento = 333333333,
                Idade = "77 anos",
                Leito = "V04",
                Medico = "Eubrando",
                NomePaciente = "Cecilia Maria",
                ListaProblemas = "# Sintomas gripais há 3-5 dias (vacina há 3 dias) – desde então com apatia. Sincopes nos últimos 3 dias.\n" +
                                 "# Desvio de comissura labial\n" +
                                 "D-dimers 730 GASO pO2 57% HMG sp. Eletrolitos/Fç renal/Enzimas sp\n" +
                                 "TC Cranio sp\n" +
                                 "AngioTc torax: sem TEP\n" +
                                 "RX torax: cardiomegalia discreta",
                Plano = "Internada\n" +
                        "EEG\n" +
                        "Ecocardio\n" +
                        "Eco carotidas\n" +
                        "Rx abdome\n" +
                        "Liberado pro leito"
            });
            nova.Add(new EvolucaoEmergencia
            {
                Atendimento = 444444444,
                Idade = "85 anos",
                Leito = "L05",
                Medico = "Carlos Delmar",
                NomePaciente = "Otavia de Borba",
                ListaProblemas = "# HAS\n" +
                                 "Veio por aferição de PA “baixa” em casa, tonturas, mal estar...\n" +
                                 "AP: com estertores - sequela de TB?\n" +
                                 "Cr 0,9 BNP 117 TROP <0,16 HMG sp\n" +
                                 "RX Torax: “Não há sinal definido de lesão pulmonar consolidativa.\n" +
                                 "Retificação das cúpulas diafragmáticas. Obliteração do recesso costofrênico esquerdo por provável derrame pleural/paquipleuris”.\n" +
                                 "ECG: FC: 50 BAV 1 grau\n" +
                                 "D-dímeros 3050  AngioTC sem TEP",
                Plano = "Internada"
            });
            nova.Add(new EvolucaoEmergencia
            {
                Atendimento = 555555555,
                Idade = "75 anos",
                Leito = "L06",
                Medico = "Cassiano Teixeira",
                NomePaciente = "Darcy Antonio",
                ListaProblemas = "# Demencia (paralisia supranuclear progressiva) \n" +
                                 "# HAS\n" +
                                 " Sepse pulmonar  Unasyn 3g 6/6h\n" +
                                 "# IRA\n" +
                                 "TROP 0,8\n" +
                                 "PCR 15 Cr 1,4 Lactato 27 EQU sp. HMG sp. GASO sp. Demais sp\n" +
                                 "RX TORAX: “Importante espessamento de paredes brônquicas e opacidades reticulares no lobo inferior do pulmão direito, podendo corresponder a alterações intersticiais.”\n" +
                                  "Broncoespasmo imp agora pela manhã – bricanyl, nbz e hidrocortisona",
                Plano = "Internada\n" +
                        "Culturais\n" +
                        "LABS para 24/04 manhã"
            });
            nova.Add(new EvolucaoEmergencia
            {
                Atendimento = 66666666,
                Idade = "85 anos",
                Leito = "L07",
                Medico = "Maulaz Ericson",
                NomePaciente = "Darcy José",
                ListaProblemas = "# Queda no poço do elevador (20/04)\n" +
                                 "# Transferido do HPS POA\n" +
                                 "# TCE/ Trauma cervical hematoma subdural? Lesão c4/c5?\n" +
                                 "Glasgow 15, sem déficit focal",
                Plano = "Internado\n" +
                        "RM cranio + reg Cervical"
            });
            nova.Add(new EvolucaoEmergencia
            {
                Atendimento = 77777777,
                Idade = "30 anos",
                Leito = "L08",
                Medico = "Alexandre Zago",
                NomePaciente = "Mariza Mariza",
                ListaProblemas = "# DPOC – tabagismo ativo\n" +
                                 "# vem por dor toracica típica  1ª serie e ECG NORMAL\n" +
                                 "CINE: coronárias normais, VE hipertrofico",
                Plano = "Internada\n" +
                        "Labs P/ amanha\n" +
                        "Avaliação pneumo"
            });
            nova.Add(new EvolucaoEmergencia
            {
                Atendimento = 88888888,
                Idade = "19 anos",
                Leito = "A09",
                Medico = "Jonathas",
                NomePaciente = "Adela Pires",
                ListaProblemas = "# cirrose hepática NASH – CHILD A\n" +
                                 "# hemorragia digestiva baixa\n" +
                                 "# angiodisplasia do colon\n" +
                                 "# hipotireoidismo\n" +
                                 "Vem por melena há 1 semana  queda do hematocrito\n" +
                                 "EDA: “Não há estigmas de sangramento recente. Ectasias Venosa.Gastrite Endoscópica Enantematosa Antral Leve.”\n" +
                                 "Ferro parenteral iniciado\n" +
                                 "LABS:\n" +
                                 "HB;8,9    Leuc;3920    Plaq:79000",
                Plano = "Internada\n" +
                        "colono suspensa\n" +
                        "Labs para manhã"
            });
            nova.Add(new EvolucaoEmergencia
            {
                Atendimento = 999999999,
                Idade = "91 anos",
                Leito = "A10",
                Medico = "Eduardo Martins Nedel",
                NomePaciente = "Esther Hulda",
                ListaProblemas = "# Pneumonite aspirativa—d1 Clavulin – trocado por unasyn\n" +
                                 "LABS sp. RX Torax com sequelas compatíveis com alterações prévias\n" +
                                 "# Anticoagulada (marcoumar) # ACFA\n" +
                                 "LABS:SP",
                Plano = "Internada"
            });
            nova.Add(new EvolucaoEmergencia
            {
                Atendimento = 50000005,
                Idade = "72 anos",
                Leito = "A11",
                Medico = "Enio do Valle",
                NomePaciente = "Beatriz",
                ListaProblemas = "# DPOC exacerbado-d2 levofloxacina\n" +
                                 "GA po2 62, pco2 48, hco3 28, sat 92%\n" +
                                 "Leu 14.400 Cr 0,8 Ur 42 Trop <0,16 RX tórax sem foco consolidativo\n" +    
                                 "# Tabagista ativa (alta carga de alcatrão) sem tratamento regular apresentando nova exacerbação",
                Plano = "Internada"
            });

            gdEvolucao.ItemsSource = nova;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public class EvolucaoEmergencia
        {
            public int Atendimento { get; set; }
            public string NomePaciente { get; set; }
            public string Idade { get; set; }
            public string Medico { get; set; }
            public string Leito { get; set; }

            public string ListaProblemas { get; set; }
            public string Plano { get; set; }
        }
    }
}
