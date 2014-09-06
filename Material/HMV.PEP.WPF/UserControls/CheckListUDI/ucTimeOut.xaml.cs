using System;
using System.Windows.Input;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.CheckListDeCirurgia;
using StructureMap;
using System.Configuration;
using DevExpress.Xpf.Editors;
using HMV.Core.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Extensions;
using HMV.PEP.ViewModel.PEP.CheckListDeUDI;

namespace HMV.PEP.WPF.UserControls.CheckListUDI
{
    /// <summary>
    /// Interaction logic for ucTimeOut.xaml
    /// </summary>
    public partial class ucTimeOut : UserControlBase, IUserControl
    {
        //private bool _nomeAuxiliar = false;

        public ucTimeOut()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as vmCheckListUDI).vmTimeOut;
        }

        public bool CancelClose
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }    
    }
}