using System.Linq;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmItensPediatria : ViewModelBase
    {
        #region Construtor
        public vmItensPediatria(Paciente pPaciente)
        {
            this._paciente = pPaciente;
            if (this._paciente.Atendimentos.IsNotNull())
            {
                this._atendimentoCollection = new wrpAtendimentoCollection(this._paciente.Atendimentos.Where(x => (x.PIN2.Count > 0) || (x.MotivosInternacao.Count > 0)).OrderByDescending(x=>x.HoraAtendimento).ToList());
                this.OnPropertyChanged<vmItensPediatria>(x => x.ExisteMotivoOuPIM2);
                this._urgenciapediatricaatendimentoCollection = new wrpUrgenciaPediatricaAtendimentoCollection(this._paciente.Atendimentos.SelectMany(x => x.UrgenciasPediatricas).OrderByDescending(x => x.HoraInclusao).ToList());
                this.OnPropertyChanged<vmItensPediatria>(x => x.ExisteUrgenciasPediatricas);
            }
        }
        #endregion

        #region Propriedades Publicas
        public wrpAtendimentoCollection AtendimentoCollection
        {
            get
            {
                return this._atendimentoCollection;
            }
            set
            {
                value = this._atendimentoCollection;
                this.OnPropertyChanged<vmItensPediatria>(x => x.AtendimentoCollection);
            }
        }

        public wrpUrgenciaPediatricaAtendimentoCollection UrgenciaPediatricaAtendimentoCollection
        {
            get
            {
                return this._urgenciapediatricaatendimentoCollection;
            }
            set
            {
                value = this._urgenciapediatricaatendimentoCollection;
                this.OnPropertyChanged<vmItensPediatria>(x => x.UrgenciaPediatricaAtendimentoCollection);
            }
        }

        public bool ExisteMotivoOuPIM2
        {
            get { return this._atendimentoCollection.Count > 0 ? true : false; }
        }

        public bool ExisteUrgenciasPediatricas
        {
            get { return this._urgenciapediatricaatendimentoCollection.Count > 0 ? true : false; }
        }
        #endregion

        #region Propriedades Privadas
        private Paciente _paciente { get; set; }
        private wrpAtendimento _atendimento { get; set; }
        private wrpAtendimentoCollection _atendimentoCollection { get; set; }
        private wrpUrgenciaPediatricaAtendimentoCollection _urgenciapediatricaatendimentoCollection { get; set; }
        #endregion
    }
}
