using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace Core.WPF.Infrastructure
{

    public class DataTemplateManager
    {
        public static Dictionary<string, DataTemplate> templates = new Dictionary<string, DataTemplate>();

            public void RegisterDataTemplate<TViewModel, TView>() where TView: FrameworkElement
        {
            RegisterDataTemplate(typeof(TViewModel), typeof(TView), null);
        }

        
        public void RegisterDataTemplate(Type viewModelType, Type dataTemplateType, string Tag="")
        {
            var template = BuildDataTemplate(viewModelType, dataTemplateType) ;
            templates.Add(viewModelType.ToString() + Tag, template);

           // var key = template.DataTemplateKey;
           // Application.Current.Resources.Add(key, template);
        }

        private DataTemplate BuildDataTemplate(Type viewModelType, Type viewType)
        {
            var template = new DataTemplate()
            {
                DataType = viewModelType,
                VisualTree = new FrameworkElementFactory(viewType)
            };

            return template;
        }

  
    }
}
