using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CodeStage_Decrypter
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Allows for a simplified version of Invoke()
        /// </summary>
        public static void Invoke(this Control c, Action method) => c.Invoke(method);
        public static void Invoke<T>(this Control c, Action<T> method) => c.Invoke(method);
        public static T Invoke<T>(this Control c, Func<T> method) => (T)c.Invoke(method);
        public static T Invoke<T, TResult>(this Control c, Func<T, TResult> method) => (T)c.Invoke(method);
    }
}
