using System.Collections.Generic;
using System.ComponentModel;
using HMV.Core.Domain.Repository;
using StructureMap;
using System.Linq;
using HMV.Core.Domain.Model;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmExamesPossiveis : INotifyPropertyChanged
    {
        #region Data

        bool? _isChecked = false;
        vmExamesPossiveis _parent = null;
        private List<vmExamesPossiveis> _CarregaExamesPossiveis = new List<vmExamesPossiveis>();
        #endregion // Data

        #region CarregaExamesPossiveis

        public static List<vmExamesPossiveis> CarregaExamesPossiveis()
        {
            IRepositorioDeProcedimento rep = ObjectFactory.GetInstance<IRepositorioDeProcedimento>();
            List<Procedimento> ListaPro = rep.OndeExamesPossiveisNoBoletimEmergencia().List().ToList().OrderBy(x => x.GrupoDeProcedimento.Descricao).Distinct().ToList();
            List<GrupoDeProcedimento> ListaGru = rep.OndeExamesPossiveisNoBoletimEmergencia().List().Select(x => x.GrupoDeProcedimento).OrderBy(x => x.Descricao).Distinct().ToList();
            List<vmExamesPossiveis> g = new List<vmExamesPossiveis>();
            vmExamesPossiveis grupo = null;
            foreach (GrupoDeProcedimento Pai in ListaGru)
            {
                grupo = new vmExamesPossiveis(Pai.Descricao, Pai.ID.ToString());

                List<vmExamesPossiveis> vmexamesPossiveis = new List<vmExamesPossiveis>();

                foreach (Procedimento filho in ListaPro.Where(x => x.GrupoDeProcedimento == Pai).ToList())
                {
                    vmexamesPossiveis.Add(new vmExamesPossiveis(filho.Descricao, filho.ID));
                }
                grupo._vmExamesPossiveis = vmexamesPossiveis;
                g.Add(grupo);

            } 
            return  g;
        }

        public List<vmExamesPossiveis> ExamesPossiveis
        {
            get
            {
                return _CarregaExamesPossiveis = CarregaExamesPossiveis();
            }
        }

        public vmExamesPossiveis() { }

        vmExamesPossiveis(string pName, string pId)
        {
            this.Name = pName;
            this.ID = pId;
            this._vmExamesPossiveis = new List<vmExamesPossiveis>();
        }   
        #endregion // CreateFoos

        #region Properties

        public List<vmExamesPossiveis> _vmExamesPossiveis { get; private set; }

        public bool IsInitiallySelected { get; private set; }

        public string Name { get; private set; }

        public string ID { get; private set; }

        public bool check { get; set; }
        #region IsChecked

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                this.check = value.Value;
                this.SetIsChecked(value, true, true);
            }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            //if (value == _isChecked)
            //    return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                this._vmExamesPossiveis.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            this.OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this._vmExamesPossiveis.Count; ++i)
            {
                bool? current = this._vmExamesPossiveis[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        #endregion // IsChecked

        #endregion // Properties

        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}