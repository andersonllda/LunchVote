using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using HMV.Core.Framework.Extensions;
using System.Windows.Data;
using HMV.Core.Framework.ViewModelBaseClasses;
using System.Windows.Media;

namespace HMV.Core.Framework.WPF
{
    internal static class Base
    {
        internal static void AbreWindow(this IBase pBase, TelaType pTelaType)
        {
            if (pTelaType.Tela != null)
                if (pTelaType.Tela == typeof(Window) || pTelaType.Tela.HasBaseType(typeof(Window)))
                {
                    WindowBase win = new WindowBase();
                    //Se tiver BindingDataContext inicializa pelo contrutor SEM parametros da win. Anderson Amaral (fiz isso para evitar erros de binding).
                    if (pTelaType.BidingDataContext.IsEmpty())
                        win = (WindowBase)Activator.CreateInstance(pTelaType.Tela, pTelaType.Parametros.IsNull() ? new object[] { pBase.vmObject } : pTelaType.Parametros);
                    else
                    {
                        try
                        {
                            win = (WindowBase)Activator.CreateInstance(pTelaType.Tela);
                        }
                        catch
                        {
                            throw new NotSupportedException("Tela: " + pBase.GetType().Name + " sem contrutor parameterless (sem parametros)."
                                                            + Environment.NewLine + "Você deve adicionar este construtor quando enviar BindingDataContext para tela a ser aberta!");
                        }

                        Binding bindingDataContext = new Binding(pTelaType.BidingDataContext);
                        bindingDataContext.Source = pTelaType.Parametros.IsNull() ? new object[] { pBase.vmObject }
                                                    : pTelaType.ParametroSourceIndex.IsNull() ? pTelaType.Parametros[0]
                                                    : pTelaType.Parametros[pTelaType.ParametroSourceIndex.Value];
                        BindingOperations.SetBinding(win, WindowBase.DataContextProperty, bindingDataContext);
                    }
                    win.Icon = pBase.IconeImagem;
                    if (pTelaType.ShowNonModal)
                        win.Show();
                    else
                        win.ShowDialog(pBase.OwnerBase);
                }
                else
                    throw new NotSupportedException("Parâmetro inválido verifique a variável enviada na tela: " + pBase.GetType().Name);
        }
    }

    internal interface IBase
    {
        ViewModelBase vmObject { get; }
        ImageSource IconeImagem { get; set; }
        Window OwnerBase { get; }
    }

    #region Classes Auxiliares
    public class TelaType
    {
        public TelaType(Type pTela = null, object[] pParametros = null, string pBindingDataContext = null, int? pParametroSourceIndex = null, bool pShowNonModal = false)
        {
            this._tela = pTela;
            this._parametros = pParametros;
            this._bindingdatacontext = pBindingDataContext;
            this._parametrosourceindex = pParametroSourceIndex;
            this._shownonmodal = pShowNonModal;
        }

        public TelaType SetType<T>(object[] pParametros = null, string pBindingDataContext = null, int? pParametroSourceIndex = null, bool pShowNonModal = false)
        {
            this._tela = typeof(T);
            this._parametros = pParametros;
            this._bindingdatacontext = pBindingDataContext;
            this._parametrosourceindex = pParametroSourceIndex;
            this._shownonmodal = pShowNonModal;
            return this;
        }

        private Type _tela;
        private object[] _parametros;
        private string _bindingdatacontext;
        private int? _parametrosourceindex;
        private bool _shownonmodal;

        public Type Tela { get { return this._tela; } }
        public object[] Parametros { get { return this._parametros; } }
        public int? ParametroSourceIndex { get { return this._parametrosourceindex; } }
        public string BidingDataContext { get { return this._bindingdatacontext; } }
        public bool ShowNonModal { get { return this._shownonmodal; } }
    }

    public class MinimizeEventArgs : EventArgs
    {
        public MinimizeEventArgs(WindowState pState)
        {
            this._state = pState;
        }
        private WindowState _state { get; set; }
        public WindowState State
        {
            get
            {
                return this._state;
            }
        }
    }

    public class RuntimeTab<T>
    {
        public T TipoTab { get; set; }
        public string Descricao { get; set; }
        public Uri Componente { get; set; }
        public Binding Binding { get; set; }
        public object Tag { get; set; }
        public object TagAux { get; set; }
    }
    #endregion
}
