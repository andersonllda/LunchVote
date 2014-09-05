using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace HMV.Core.Framework.Helper
{
    /// <summary>
    ///  Contém métodos de ajuda para a Interface, habilitar e desabilitar o cursor de espera.
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        ///   Indica que o processo está ocupado.
        /// </summary>
        private static bool IsBusy;

        /// <summary>
        /// Seta o busystate para espera.
        /// </summary>
        public static void SetBusyState()
        {
            SetBusyState(true);
        }

        /// <summary>
        /// Seta o busystate para espera ou para estado normal
        /// </summary>
        /// <param name="busy">if set to <c>true</c> a app está em busy.</param>
        private static void SetBusyState(bool busy)
        {
            if (busy != IsBusy)
            {
                IsBusy = busy;
                Mouse.OverrideCursor = busy ? Cursors.Wait : null;

                if (Application.Current != null && IsBusy)
                {
                    new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle, dispatcherTimer_Tick, Application.Current.Dispatcher);
                }
            }
        }
       
        private static void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var dispatcherTimer = sender as DispatcherTimer;
            if (dispatcherTimer != null)
            {
                SetBusyState(false);
                dispatcherTimer.Stop();
            }
        }
    }
}
