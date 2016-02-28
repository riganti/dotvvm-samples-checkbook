using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Binding;

namespace CheckBook.App.Controls
{
    public class UserDetailForm : DotvvmMarkupControl
    {

        public bool HideOptionalPasswordNotice
        {
            get { return (bool)GetValue(HideOptionalPasswordNoticeProperty); }
            set { SetValue(HideOptionalPasswordNoticeProperty, value); }
        }
        public static readonly DotvvmProperty HideOptionalPasswordNoticeProperty
            = DotvvmProperty.Register<bool, UserDetailForm>(c => c.HideOptionalPasswordNotice, false);


    }
}