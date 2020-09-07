using System.Reflection;

namespace Nop.Plugin.Payments.BOGCheckoutIpay.Extensions
{
    public static class TemplateParser<T> where T : class, new()
    {
        /// <summary>
        /// The Method is used to replace "template" string object with model object.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="strTemplate"></param>
        /// <returns></returns>
        public static string GetParsedTemplate(T model, string template)
        {
            var type = model.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.GetValue(model, null) != null)
                {
                    var propValue = string.Empty;

                    if (property.PropertyType == typeof(decimal))
                        propValue = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00}", property.GetValue(model, null));// in all cases we need "0:0.00" format for Ipay 
                    else
                        propValue = property.GetValue(model, null).ToString();

                    template = template.Replace("{{" + property.Name + "}}", propValue);
                }
            }

            return template;
        }
    }
}
