/* **************************************************************
* File Name     :- GreaterThanIfAttribute.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 10th Jan 2013
* Modified Date :- 10th Jan 2013
* Description   :- This file contains GreaterThanIfAttribute custom attribute to validate if
                    years and moths are greater than 24
****************************************************************/

#region Namespace
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
#endregion 

namespace CurrentDesk.BackOffice.Custom
{
    /// <summary>
    /// This Is the custom attribute written to validae fields which are required to have
    /// no of months more than 24
    /// </summary>
    
    [AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public class GreaterThanIfAttribute : RequiredAttribute, IClientValidatable
    {
        #region Property
        public string YearControl { get; set; }
        public string MonthControl { get; set; }
        public string YearControlID { get; set; }
        public string MonthControlID { get; set; }
        #endregion

        #region Constructor
        public GreaterThanIfAttribute(string yearControl, string monthControl, string yearControlID, string monthControlID)
        {
            YearControl = yearControl;
            MonthControl = monthControl;
            YearControlID = yearControlID;
            MonthControlID = monthControlID;
           
        }
        #endregion

        #region Method       

        /// <summary>
        /// Checks For the validation result
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var yearProperty = validationContext.ObjectType.GetProperty(YearControl);
            var monthProperty = validationContext.ObjectType.GetProperty(MonthControl);
            if (yearProperty == null || monthProperty == null)
            {
                return new ValidationResult(String.Format("Property {0}-{1} not found.", YearControl, MonthControl));
            }

            var yearPropertyValue = yearProperty.GetValue(validationContext.ObjectInstance, null);
            var monthPropertyValue = monthProperty.GetValue(validationContext.ObjectInstance, null);

            var conditionIsMet = ((int)yearPropertyValue * 12 +  (int)monthPropertyValue) < 24;
            var res = conditionIsMet ? base.IsValid(value, validationContext) : null;
           
            return res;
        }

        /// <summary>
        /// Implementation of IClientValidatable so that 
        /// it can be accessed by unobtrusive javascript
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule rule = new ModelClientValidationRule();
            ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationType = "greaterthanif";

            rule.ValidationParameters.Add("yearpropertyid", YearControlID);
            rule.ValidationParameters.Add("monthpropertyid", MonthControlID);            

            yield return rule;
        }
        #endregion
    }
}