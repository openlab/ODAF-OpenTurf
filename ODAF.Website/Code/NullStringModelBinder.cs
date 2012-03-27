using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace vancouveropendata
{
    public class NullStringModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (bindingContext.ModelType == typeof(string))
            {
                bindingContext.ModelMetadata.ConvertEmptyStringToNull = false;
                if (value == null || string.IsNullOrEmpty(value.AttemptedValue))
                    return "";
            }
            return value.AttemptedValue;

        }
    }
}