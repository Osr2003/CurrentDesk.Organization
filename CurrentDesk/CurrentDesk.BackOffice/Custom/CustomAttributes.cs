/* **************************************************************
* File Name     :- CustomAttributes.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 10th Jan 2013
* Modified Date :- 10th Jan 2013
* Description   :- This file contains BooleanRequiredAttribute custom attribute to validate if boolean
****************************************************************/


#region Namespace

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

#endregion

namespace CurrentDesk.BackOffice.Custom
{
    public class CustomAttributes
    {
    }

    public class BooleanRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            if (value is bool)
                return (bool)value;
            else
                return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
            ModelMetadata metadata,
            ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "booleanrequired"
            };
        }
    }


    //[AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    //public class RequiredIfAttribute : RequiredAttribute , IClientValidatable
    //{
    //    private string OtherProperty { get; set; }
    //    private object Condition { get; set; }

    //    public RequiredIfAttribute(string otherProperty, object condition)
    //    {
    //        OtherProperty = otherProperty;
    //        Condition = condition;
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var property = validationContext.ObjectType.GetProperty(OtherProperty);
    //        if (property == null)
    //            return new ValidationResult(String.Format("Property {0} not found.", OtherProperty));

    //        var propertyValue = property.GetValue(validationContext.ObjectInstance, null);
    //        var conditionIsMet = Equals(propertyValue, Condition);
    //        var res = conditionIsMet ? base.IsValid(value, validationContext) : null;
    //        return res;
    //    }
    
   
   
    //}
}