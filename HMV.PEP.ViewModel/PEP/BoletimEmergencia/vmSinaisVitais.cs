using System.Collections.Generic;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.ViewModelBaseClasses;
using StructureMap;
using HMV.Core.Framework.Extensions;
using System;
using System.Collections.ObjectModel;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Constraints;


namespace HMV.PEP.ViewModel.BoletimEmergencia
{
    public class vmSinaisVitais : ViewModelBase
    {
        #region Contrutor
        public vmSinaisVitais(wrpBoletimDeEmergencia pBoletim, Usuarios pUsuario, bool boletimCO)
        {
            this._boletimdeemergencia = pBoletim;
            Boletim = pBoletim;
            Usuario = pUsuario;
            this.CarregaSinaisVitais();
            _boletimCO = boletimCO;
        }

        public vmSinaisVitais(wrpBoletimDeEmergencia pBoletim, Usuarios pUsuario, vmBoletimEmergencia pvm)
        {
            this._boletimdeemergencia = pBoletim;
            Boletim = pBoletim;
            Usuario = pUsuario;
            this._vm = pvm;
            this.CarregaSinaisVitais();
            _boletimCO = false;
        }
        #endregion

        #region Propriedades Publicas
        public ObservableCollection<SV> SinaisVitais
        {
            get
            {
                return this._sinaisvitaislst;
            }
        }

        public bool HabilitaGrid { get; set; }       

        public static Usuarios Usuario;
        public static wrpBoletimDeEmergencia Boletim;
        #endregion

        #region Metodos Privados
        public void CarregaSinaisVitais()
        {
            this._sinaisvitaislst = new ObservableCollection<SV>();
            DateTime? auxdata = null;
            SV lst = null;

            if (this._boletimdeemergencia == null)
            {
                this.HabilitaGrid = false;
                this.OnPropertyChanged("HabilitaGrid");
            }
            else
            {
                this.HabilitaGrid = true;
                this.OnPropertyChanged("HabilitaGrid");
                foreach (var sv in this._boletimdeemergencia.SinaisVitais.ToList().OrderBy(x => x.Data))
                {
                    if (auxdata != sv.Data)
                    {
                        if (lst != null)
                            this._sinaisvitaislst.Add(lst);
                        lst = new SV();
                        lst.Data = sv.Data;
                        lst.Locked = true;
                        auxdata = sv.Data;
                    }
                    if (sv.Sigla.Sigla == SinaisVitaisEnum.PA.ToString())
                    {
                        lst.PA = sv.Sigla;
                        lst.PAValor = sv.Valor;
                        lst.PAId = sv.ID;
                    }
                    else if (sv.Sigla.Sigla == SinaisVitaisEnum.TAX.ToString())
                    {
                        lst.TAX = sv.Sigla;
                        lst.TAXValor = sv.Valor;
                        lst.TAXId = sv.ID;
                    }
                    else if (sv.Sigla.Sigla == SinaisVitaisEnum.FC.ToString())
                    {
                        lst.FC = sv.Sigla;
                        lst.FCValor = sv.Valor;
                        lst.FCId = sv.ID;
                    }
                    else if (sv.Sigla.Sigla == SinaisVitaisEnum.FR.ToString())
                    {
                        lst.FR = sv.Sigla;
                        lst.FRValor = sv.Valor;
                        lst.FRId = sv.ID;
                    }
                    else if (sv.Sigla.Sigla == SinaisVitaisEnum.DOR.ToString())
                    {
                        lst.DOR = sv.Sigla;
                        lst.DORValor = sv.Valor;
                        lst.DORId = sv.ID;
                    }
                    else if (sv.Sigla.Sigla == SinaisVitaisEnum.SAT.ToString())
                    {
                        lst.SAT = sv.Sigla;
                        lst.SATValor = sv.Valor;
                        lst.SATId = sv.ID;
                    }
                    else if (sv.Sigla.Sigla == SinaisVitaisEnum.PESO.ToString())
                    {
                        lst.PESO = sv.Sigla;
                        lst.PESOValor = sv.Valor;
                        lst.PESOId = sv.ID;
                    }
                    else if (sv.Sigla.Sigla == SinaisVitaisEnum.BCF.ToString())
                    {
                        lst.BCF = sv.Sigla;
                        lst.BCFValor = sv.Valor;
                        lst.BCFId = sv.ID;
                    }
                }

                if (lst != null)
                    this._sinaisvitaislst.Add(lst);

                this.OnPropertyChanged("SinaisVitais");
            }
        }
        #endregion

        #region Propriedades Privadas
        private wrpBoletimDeEmergencia _boletimdeemergencia { get; set; }    
        private ObservableCollection<SV> _sinaisvitaislst { get; set; }
        private SV _svselecionado { get; set; }
        private vmBoletimEmergencia _vm;
        private bool _boletimCO;
        #endregion

        #region Metodos Publicos
        public void SalvaSinalVital(object Sinal)
        {
            if (this._boletimdeemergencia.DomainObject.SinaisVitais == null)
            {
                this._boletimdeemergencia.DomainObject.SinaisVitais = new List<SinaisVitais>();
                this._boletimdeemergencia.Save();
            }

            (Sinal as SV).Salva();
            
            this.CarregaSinaisVitais();
            if ( this._vm.IsNotNull() )     
                this._vm.Editou = true;
        }
        public void DeletaSinalVital(object Sinal)
        {
            (Sinal as SV).Deleta();
            this.CarregaSinaisVitais();
            if (this._vm.IsNotNull())     
                this._vm.Editou = true;
        }

        public bool BCFVisible
        {
            get
            {
                return _boletimCO;
            }
        }

        public bool SATVisible
        {
            get
            {
                return !_boletimCO;
            }
        }

        public bool PESOVisible
        {
            get
            {
                return !_boletimCO;
            }
        }


        public bool UsuarioVisible
        {
            get
            {
                return _boletimCO;
            }
        }

        #endregion       

        public class SV : ViewModelBase
        {
            public SV()
            {
                this._data = DateTime.Now;

                IRepositorioSinaisVitaisTipo rep = ObjectFactory.GetInstance<IRepositorioSinaisVitaisTipo>();
                var tipos = rep.List();
                this.PA = new wrpSinaisVitaisTipo(tipos.Where(x => x.Sigla == SinaisVitaisEnum.PA.ToString()).Single());
                this.TAX = new wrpSinaisVitaisTipo(tipos.Where(x => x.Sigla == SinaisVitaisEnum.TAX.ToString()).Single());
                this.FC = new wrpSinaisVitaisTipo(tipos.Where(x => x.Sigla == SinaisVitaisEnum.FC.ToString()).Single());
                this.FR = new wrpSinaisVitaisTipo(tipos.Where(x => x.Sigla == SinaisVitaisEnum.FR.ToString()).Single());
                this.DOR = new wrpSinaisVitaisTipo(tipos.Where(x => x.Sigla == SinaisVitaisEnum.DOR.ToString()).Single());
                this.SAT = new wrpSinaisVitaisTipo(tipos.Where(x => x.Sigla == SinaisVitaisEnum.SAT.ToString()).Single());
                this.PESO = new wrpSinaisVitaisTipo(tipos.Where(x => x.Sigla == SinaisVitaisEnum.PESO.ToString()).Single());
                this.BCF = new wrpSinaisVitaisTipo(tipos.Where(x => x.Sigla == SinaisVitaisEnum.BCF.ToString()).Single());

                this._boletim = vmSinaisVitais.Boletim;
                this._usuarios = vmSinaisVitais.Usuario;
                this._listavalida = new List<Valida>();
            }

            public void Salva()
            {
                if (this.IsValid)
                {
                    if (!string.IsNullOrEmpty(this._pavalor))
                    {
                        var sinal = new wrpSinaisVitais { Sigla = this.PA, Valor = this._pavalor, DataInclusao = this._data, Usuario = new wrpUsuarios(this._usuarios), HoraInclusao = this._data.ToString("HH:mm") };
                        sinal.Save();
                        this.PAId = sinal.ID;
                        this._boletim.SinaisVitais.Add(sinal);
                    }
                    if (!string.IsNullOrEmpty(this._taxvalor))
                    {
                        var sinal = new wrpSinaisVitais { Sigla = this.TAX, Valor = this._taxvalor, DataInclusao = this._data, Usuario = new wrpUsuarios(this._usuarios), HoraInclusao = this._data.ToString("HH:mm") };
                        sinal.Save();
                        this.TAXId = sinal.ID;
                        this._boletim.SinaisVitais.Add(sinal);
                    }
                    if (!string.IsNullOrEmpty(this._fcvalor))
                    {
                        var sinal = new wrpSinaisVitais { Sigla = this.FC, Valor = this._fcvalor, DataInclusao = this._data, Usuario = new wrpUsuarios(this._usuarios), HoraInclusao = this._data.ToString("HH:mm") };
                        sinal.Save();
                        this.FCId = sinal.ID;
                        this._boletim.SinaisVitais.Add(sinal);
                    }
                    if (!string.IsNullOrEmpty(this._frvalor))
                    {
                        var sinal = new wrpSinaisVitais { Sigla = this.FR, Valor = this._frvalor, DataInclusao = this._data, Usuario = new wrpUsuarios(this._usuarios), HoraInclusao = this._data.ToString("HH:mm") };
                        sinal.Save();
                        this.FRId = sinal.ID;
                        this._boletim.SinaisVitais.Add(sinal);
                    }
                    if (!string.IsNullOrEmpty(this._dorvalor))
                    {
                        var sinal = new wrpSinaisVitais { Sigla = this.DOR, Valor = this._dorvalor, DataInclusao = this._data, Usuario = new wrpUsuarios(this._usuarios), HoraInclusao = this._data.ToString("HH:mm") };
                        sinal.Save();
                        this.DORId = sinal.ID;
                        this._boletim.SinaisVitais.Add(sinal);
                    }
                    if (!string.IsNullOrEmpty(this._satvalor))
                    {
                        var sinal = new wrpSinaisVitais { Sigla = this.SAT, Valor = this._satvalor, DataInclusao = this._data, Usuario = new wrpUsuarios(this._usuarios), HoraInclusao = this._data.ToString("HH:mm") };
                        sinal.Save();
                        this.SATId = sinal.ID;
                        this._boletim.SinaisVitais.Add(sinal);
                    }
                    if (!string.IsNullOrEmpty(this._pesovalor))                   
                    {
                        var sinal = new wrpSinaisVitais { Sigla = this.PESO, Valor = this._pesovalor, DataInclusao = this._data, Usuario = new wrpUsuarios(this._usuarios), HoraInclusao = this._data.ToString("HH:mm") };
                        sinal.Save();
                        this.PESOId = sinal.ID;
                        this._boletim.SinaisVitais.Add(sinal);
                    }
                    if (!string.IsNullOrEmpty(this._bcfvalor))
                    {
                        var sinal = new wrpSinaisVitais { Sigla = this.BCF, Valor = this._bcfvalor, DataInclusao = this._data, Usuario = new wrpUsuarios(this._usuarios), HoraInclusao = this._data.ToString("HH:mm") };
                        sinal.Save();
                        this.BCFId = sinal.ID;
                        this._boletim.SinaisVitais.Add(sinal);
                    }
                    this.Locked = true;                    
                }
            }

            public void Deleta()
            {
                IRepositorioDeSinaisVitais rep = ObjectFactory.GetInstance<IRepositorioDeSinaisVitais>();
                var sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.PAId).SingleOrDefault();
                if (sinal != null)
                {
                    this._boletim.SinaisVitais.Remove(sinal);
                    this._boletim.Save();
                    rep.Delete(sinal.DomainObject);
                }

                sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.TAXId).SingleOrDefault();
                if (sinal != null)
                {
                    this._boletim.SinaisVitais.Remove(sinal);
                    this._boletim.Save();
                    rep.Delete(sinal.DomainObject);
                }

                sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.FCId).SingleOrDefault();
                if (sinal != null)
                {
                    this._boletim.SinaisVitais.Remove(sinal);
                    this._boletim.Save();
                    rep.Delete(sinal.DomainObject);
                }

                sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.FRId).SingleOrDefault();
                if (sinal != null)
                {
                    this._boletim.SinaisVitais.Remove(sinal);
                    this._boletim.Save();
                    rep.Delete(sinal.DomainObject);
                }

                sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.DORId).SingleOrDefault();
                if (sinal != null)
                {
                    this._boletim.SinaisVitais.Remove(sinal);
                    this._boletim.Save();
                    rep.Delete(sinal.DomainObject);
                }

                sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.SATId).SingleOrDefault();
                if (sinal != null)
                {
                    this._boletim.SinaisVitais.Remove(sinal);
                    this._boletim.Save();
                    rep.Delete(sinal.DomainObject);
                }

                sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.PESOId).SingleOrDefault();
                if (sinal != null)
                {
                    this._boletim.SinaisVitais.Remove(sinal);
                    this._boletim.Save();
                    rep.Delete(sinal.DomainObject);
                }
                sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.BCFId).SingleOrDefault();
                if (sinal != null)
                {
                    this._boletim.SinaisVitais.Remove(sinal);
                    this._boletim.Save();
                    rep.Delete(sinal.DomainObject);
                }
            }

            public bool Locked { get; set; }
            public bool CanDelete
            {
                get
                {
                    var sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.PAId).SingleOrDefault();
                    if (sinal != null)
                    {
                        if (sinal.Usuario.DomainObject.cd_usuario == this._usuarios.cd_usuario)
                            return true;
                    }
                    sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.TAXId).SingleOrDefault();
                    if (sinal != null)
                    {
                        if (sinal.Usuario.DomainObject.cd_usuario == this._usuarios.cd_usuario)
                            return true;
                    }
                    sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.FCId).SingleOrDefault();
                    if (sinal != null)
                    {
                        if (sinal.Usuario.DomainObject.cd_usuario == this._usuarios.cd_usuario)
                            return true;
                    }
                    sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.FRId).SingleOrDefault();
                    if (sinal != null)
                    {
                        if (sinal.Usuario.DomainObject.cd_usuario == this._usuarios.cd_usuario)
                            return true;
                    }
                    sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.DORId).SingleOrDefault();
                    if (sinal != null)
                    {
                        if (sinal.Usuario.DomainObject.cd_usuario == this._usuarios.cd_usuario)
                            return true;
                    }
                    sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.SATId).SingleOrDefault();
                    if (sinal != null)
                    {
                        if (sinal.Usuario.DomainObject.cd_usuario == this._usuarios.cd_usuario)
                            return true;
                    }
                    sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.PESOId).SingleOrDefault();
                    if (sinal != null)
                    {
                        if (sinal.Usuario.DomainObject.cd_usuario == this._usuarios.cd_usuario)
                            return true;
                    }
                    sinal = this._boletim.SinaisVitais.Where(x => x.ID == this.BCFId).SingleOrDefault();
                    if (sinal != null)
                    {
                        if (sinal.Usuario.DomainObject.cd_usuario == this._usuarios.cd_usuario)
                            return true;
                    }
                    return false;
                }
            }

            public DateTime Data
            {
                get
                {
                    return _data;
                }
                set
                {
                    if (value.ToShortTimeString() == "00:00")
                        this._data = DateTime.Parse(value.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                    else
                        this._data = value;
                    this.OnPropertyChanged("Data");
                }
            }

            public int PAId { get; set; }
            public wrpSinaisVitaisTipo PA { get; set; }
            [Length(10, Message= "Digite no máximo 10 caracteres.")]
            public string PAValor
            {
                get
                {
                    return _pavalor;
                }
                set
                {
                    this._pavalor = value;
                    this.OnPropertyChanged("PAValor");
                }
            }

            public int TAXId { get; set; }
            public wrpSinaisVitaisTipo TAX { get; set; }
            [Length(10, Message = "Digite no máximo 10 caracteres.")]
            public string TAXValor
            {
                get
                {
                    return _taxvalor;
                }
                set
                {
                    this._taxvalor = value;
                    this.OnPropertyChanged("TAXValor");
                }
            }

            public int FCId { get; set; }
            public wrpSinaisVitaisTipo FC { get; set; }
            [Length(10, Message = "Digite no máximo 10 caracteres.")]
            public string FCValor
            {
                get
                {
                    return _fcvalor;
                }
                set
                {
                    this._fcvalor = value;
                    this.OnPropertyChanged("FCValor");
                }
            }

            public int FRId { get; set; }
            public wrpSinaisVitaisTipo FR { get; set; }
            [Length(10, Message = "Digite no máximo 10 caracteres.")]
            public string FRValor
            {
                get
                {
                    return _frvalor;
                }
                set
                {
                    this._frvalor = value;
                    this.OnPropertyChanged("FRValor");
                }
            }

            public int DORId { get; set; }
            public wrpSinaisVitaisTipo DOR { get; set; }
            [Length(10, Message = "Digite no máximo 10 caracteres.")]
            public string DORValor
            {
                get
                {
                    return _dorvalor;
                }
                set
                {
                    this._dorvalor = value;
                    this.OnPropertyChanged("DORValor");
                }
            }

            public int SATId { get; set; }
            public wrpSinaisVitaisTipo SAT { get; set; }
            [Length(10, Message = "Digite no máximo 10 caracteres.")]
            public string SATValor
            {
                get
                {
                    return _satvalor;
                }
                set
                {
                    this._satvalor = value;
                    this.OnPropertyChanged("SATValor");
                }
            }

            public int PESOId { get; set; }
            public wrpSinaisVitaisTipo PESO { get; set; }
            [Length(10, Message = "Digite no máximo 10 caracteres.")]
            public string PESOValor
            {
                get
                {
                    return _pesovalor;
                }
                set
                {
                    this._pesovalor = value;
                    this.OnPropertyChanged("PESOValor");
                }
            }

            public int BCFId { get; set; }
            public wrpSinaisVitaisTipo BCF { get; set; }
            [Length(10, Message = "Digite no máximo 10 caracteres.")]
            public string BCFValor
            {
                get
                {
                    return _bcfvalor;
                }
                set
                {
                    this._bcfvalor = value;
                    this.OnPropertyChanged("BCFValor");
                }
            }        

            private DateTime _data { get; set; }
            private string _pavalor { get; set; }
            private string _taxvalor { get; set; }
            private string _fcvalor { get; set; }
            private string _frvalor { get; set; }
            private string _dorvalor { get; set; }
            private string _satvalor { get; set; }
            private string _pesovalor { get; set; }
            private string _bcfvalor { get; set; }
            private List<Valida> _listavalida { get; set; }
            private wrpBoletimDeEmergencia _boletim { get; set; }
            private Usuarios _usuarios { get; set; }
            private List<InvalidValue> InvalidValues { get { return new ValidatorEngine().Validate(this).ToList(); } }

            public override string this[string columnName]
            {
                get
                {
                    double aux = 0;
                    if (columnName == "TAXValor" && this._taxvalor != null)
                    {
                        if (double.TryParse(this._taxvalor, out aux))
                        {
                            if (aux >= this.TAX.ValorMinimo && aux <= this.TAX.ValorMaximo)
                            {
                                if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                                    this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = true;
                                else
                                    this._listavalida.Add(new Valida { Campo = columnName, IsValid = true });

                                return null;
                            }
                        }

                        if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                            this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = false;
                        else
                            this._listavalida.Add(new Valida { Campo = columnName, IsValid = false });

                        return "Digite um valor entre " + this.TAX.ValorMinimo + " e " + this.TAX.ValorMaximo + " para este campo!";
                    }
                    if (columnName == "FCValor" && this._fcvalor != null)
                    {
                        if (double.TryParse(this._fcvalor, out aux))
                        {
                            if (aux >= this.FC.ValorMinimo && aux <= this.FC.ValorMaximo)
                            {
                                if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                                    this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = true;
                                else
                                    this._listavalida.Add(new Valida { Campo = columnName, IsValid = true });

                                return null;
                            }
                        }

                        if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                            this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = false;
                        else
                            this._listavalida.Add(new Valida { Campo = columnName, IsValid = false });
                        return "Digite um valor entre " + this.FC.ValorMinimo + " e " + this.FC.ValorMaximo + " para este campo!";
                    }
                    if (columnName == "FRValor" && this._frvalor != null)
                    {
                        if (double.TryParse(this._frvalor, out aux))
                        {
                            if (aux >= this.FR.ValorMinimo && aux <= this.FR.ValorMaximo)
                            {
                                if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                                    this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = true;
                                else
                                    this._listavalida.Add(new Valida { Campo = columnName, IsValid = true });

                                return null;
                            }
                        }

                        if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                            this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = false;
                        else
                            this._listavalida.Add(new Valida { Campo = columnName, IsValid = false });
                        return "Digite um valor entre " + this.FR.ValorMinimo + " e " + this.FR.ValorMaximo + " para este campo!";
                    }
                    if (columnName == "DORValor" && this._dorvalor != null)
                    {
                        if (double.TryParse(this._dorvalor, out aux))
                        {
                            if (aux >= this.DOR.ValorMinimo && aux <= this.DOR.ValorMaximo)
                            {
                                if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                                    this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = true;
                                else
                                    this._listavalida.Add(new Valida { Campo = columnName, IsValid = true });

                                return null;
                            }
                        }

                        if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                            this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = false;
                        else
                            this._listavalida.Add(new Valida { Campo = columnName, IsValid = false });
                        return "Digite um valor entre " + this.DOR.ValorMinimo + " e " + this.DOR.ValorMaximo + " para este campo!";
                    }
                    if (columnName == "SATValor" && this._satvalor != null)
                    {
                        if (double.TryParse(this._satvalor, out aux))
                        {
                            if (aux >= this.SAT.ValorMinimo && aux <= this.SAT.ValorMaximo)
                            {
                                if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                                    this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = true;
                                else
                                    this._listavalida.Add(new Valida { Campo = columnName, IsValid = true });

                                return null;
                            }
                        }

                        if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                            this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = false;
                        else
                            this._listavalida.Add(new Valida { Campo = columnName, IsValid = false });
                        return "Digite um valor entre " + this.SAT.ValorMinimo + " e " + this.SAT.ValorMaximo + " para este campo!";
                    }
                    if (columnName == "PESOValor" && this._pesovalor != null)
                    {
                        if (double.TryParse(this._pesovalor, out aux))
                        {
                            if (aux >= this.PESO.ValorMinimo && aux <= this.PESO.ValorMaximo)
                            {
                                if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                                    this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = true;
                                else
                                    this._listavalida.Add(new Valida { Campo = columnName, IsValid = true });

                                return null;
                            }
                        }

                        if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                            this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = false;
                        else
                            this._listavalida.Add(new Valida { Campo = columnName, IsValid = false });
                        return "Digite um valor entre " + this.PESO.ValorMinimo + " e " + this.PESO.ValorMaximo + " para este campo!";
                    }
                    if (columnName == "BCFValor" && this._bcfvalor != null)
                    {
                        if (double.TryParse(this._bcfvalor, out aux))
                        {
                            if (aux >= this.BCF.ValorMinimo && aux <= this.BCF.ValorMaximo)
                            {
                                if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                                    this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = true;
                                else
                                    this._listavalida.Add(new Valida { Campo = columnName, IsValid = true });

                                return null;
                            }
                        }

                        if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                            this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = false;
                        else
                            this._listavalida.Add(new Valida { Campo = columnName, IsValid = false });
                        return "Digite um valor entre " + this.BCF.ValorMinimo + " e " + this.BCF.ValorMaximo + " para este campo!";
                    }

                    string ret = string.Empty;
                    InvalidValues.Where(x => x.PropertyName == columnName).ToList().ForEach(x => ret += x.Message + Environment.NewLine);
                    if (!string.IsNullOrWhiteSpace(ret))
                        if (this._listavalida.Count(x => x.Campo == columnName) > 0)
                            this._listavalida.Where(x => x.Campo == columnName).Single().IsValid = false;
                        else
                            this._listavalida.Add(new Valida { Campo = columnName, IsValid = false });
                    return ret.TrimEnd(Environment.NewLine.ToCharArray());
                }
            }
            
            public override bool IsValid
            {
                get
                {
                    if (!this.Locked)
                        return (this._listavalida.Count(x=> x.IsValid == false) == 0);

                    return false;
                }
            }

            private class Valida
            {
                public string Campo { get; set; }
                public bool IsValid { get; set; }
            }
        }
    }
}
