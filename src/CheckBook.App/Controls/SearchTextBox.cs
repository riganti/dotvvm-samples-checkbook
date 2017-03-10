using DotVVM.Framework.Binding;
using DotVVM.Framework.Binding.Expressions;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Hosting;
using System;

namespace CheckBook.App.Controls
{
    public class SearchTextBox : HtmlGenericControl
    {
        public static readonly DotvvmProperty TextProperty =
                    DotvvmProperty.Register<string, SearchTextBox>(t => t.Text, "");

        public static readonly DotvvmProperty ChangedProperty =
                    DotvvmProperty.Register<Command, SearchTextBox>(t => t.Changed, null);

        public static readonly DotvvmProperty TypeProperty
            = DotvvmProperty.Register<TextBoxType, SearchTextBox>(c => c.Type, TextBoxType.Normal);

        [MarkupOptions(AllowBinding = false)]
        public TextBoxType Type
        {
            get { return (TextBoxType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text in the control.
        /// </summary>
        public string Text
        {
            get { return Convert.ToString(GetValue(TextProperty)); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command that will be triggered when the control text is changed.
        /// </summary>
        public Command Changed
        {
            get { return (Command)GetValue(ChangedProperty); }
            set { SetValue(ChangedProperty, value); }
        }

        /// <summary>
        /// Adds all attributes that should be added to the control begin tag.
        /// </summary>
        protected override void AddAttributesToRender(IHtmlWriter writer, IDotvvmRequestContext context)
        {
            writer.AddKnockoutDataBind("value", this, TextProperty, () =>
            {
                if (Type != TextBoxType.MultiLine)
                {
                    writer.AddAttribute("value", Text);
                }
            });

            writer.AddKnockoutDataBind("elvisDelayedAfterKey", this, TextProperty);

            if (Type == TextBoxType.MultiLine)
            {
                TagName = "textarea";
            }
            else if (Type == TextBoxType.Normal)
            {
                TagName = "input";
                // do not overwrite type attribute
                if (!Attributes.ContainsKey("type"))
                {
                    writer.AddAttribute("type", "text");
                }
            }
            else
            {
                string type = null;
                switch (Type)
                {
                    case TextBoxType.Password:
                        type = "password";
                        break;

                    case TextBoxType.Telephone:
                        type = "tel";
                        break;

                    case TextBoxType.Url:
                        type = "url";
                        break;

                    case TextBoxType.Email:
                        type = "email";
                        break;

                    case TextBoxType.Date:
                        type = "date";
                        break;

                    case TextBoxType.Time:
                        type = "time";
                        break;

                    case TextBoxType.Color:
                        type = "color";
                        break;

                    case TextBoxType.Search:
                        type = "search";
                        break;

                    default:
                        throw new NotSupportedException($"TextBox Type { Type } not supported");
                }
                writer.AddAttribute("type", type);
                TagName = "input";
            }

            // prepare changed event attribute
            var changedBinding = GetCommandBinding(ChangedProperty);
            if (changedBinding != null)
            {
                writer.AddAttribute("onchange", KnockoutHelper.GenerateClientPostBackScript(nameof(Changed), changedBinding, this, true, isOnChange: true));
            }

            base.AddAttributesToRender(writer, context);
        }

        protected override void RenderContents(IHtmlWriter writer, IDotvvmRequestContext context)
        {
            if (Type == TextBoxType.MultiLine && GetValueBinding(TextProperty) == null)
            {
                writer.WriteText(Text);
            }
            base.RenderContents(writer, context);
        }
    }
}