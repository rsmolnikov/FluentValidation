namespace FluentValidation.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using System.Globalization;
    using System.Web.Mvc.Html;

    //class for JavaScript serialization
    internal class ClientValidationField
    {
        private string _field;
        private List<ModelClientValidationRule> _attributes = new List<ModelClientValidationRule>();

        public string Field
        {
            get { return _field; }
            set { _field = value; }
        }
        public List<ModelClientValidationRule> Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }

    }
    public static class HtmlHelperExtensions
    {
        private const string DEFAULTJSONFORMAT = "\r\n<script type=\"text/javascript\">\r\nif (clientValidation == undefined) var clientValidation = new Array(); clientValidation[\"{0}\"] =  {1};\r\n</script>";
        public static void GetClientValidationJson(this HtmlHelper htmlHelper, string formID)
        {
            List<ClientValidationField> FieldValidators = new List<ClientValidationField>();
            foreach (ModelMetadata metadata in htmlHelper.ViewData.ModelMetadata.Properties)
            {
                ClientValidationField field = new ClientValidationField();
                field.Field = metadata.PropertyName;
                foreach (ModelClientValidationRule rule in metadata.GetValidators(htmlHelper.ViewContext).SelectMany<ModelValidator, ModelClientValidationRule>(delegate(ModelValidator v)
                {
                    return v.GetClientValidationRules();
                }))
                {
                    field.Attributes.Add(rule);
                }
                FieldValidators.Add(field);
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            SortedDictionary<string, object> dictionary = new SortedDictionary<string, object>();
            dictionary.Add("ns", formID);
            dictionary.Add("rules", FieldValidators);

            string jsonValidationMetadata = serializer.Serialize(dictionary);
            string str3 = string.Format(CultureInfo.InvariantCulture, DEFAULTJSONFORMAT, new object[] { formID, jsonValidationMetadata });
            htmlHelper.ViewContext.Writer.Write(str3);
        }
    }
    
}