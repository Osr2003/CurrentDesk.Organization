/* **************************************************************
* File Name     :- RequiredIfAttribute.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 10th Jan 2013
* Modified Date :- 10th Jan 2013
* Description   :- This file contains RequiredIfAttribute custom attribute to validate if
                    conditional control validation
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
    /// This Is the custom attribute written to validate fields 
    /// depending upon the other fields
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public class RequiredIfAttribute :  RequiredAttribute, IClientValidatable
    {
        #region Property
        private string OtherProperty { get; set; }
        private int Condition { get; set; }
        private string Control { get; set; }
        private string ControlID { get; set; }
        #endregion

        #region Constructor
        public RequiredIfAttribute(string otherProperty, int condition , string control, string controlID)
        {
            OtherProperty = otherProperty;
            Condition = condition;
            Control = control;
            ControlID = controlID;
        }
        #endregion

        //public override bool IsValid(object value)
        //{
        //    return true;
        //}

        #region Methods

        /// <summary>
        /// Checks For the validation result
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(OtherProperty);
            if (property == null)
            {
                return new ValidationResult(String.Format("Property {0} not found.", OtherProperty));
            }

            var propertyValue = property.GetValue(validationContext.ObjectInstance, null);
            var conditionIsMet = Equals(propertyValue, Condition);
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
            rule.ValidationType = "requiredif";

            rule.ValidationParameters.Add("dependentproperty", Control);
            rule.ValidationParameters.Add("targetvalue", Condition);
            rule.ValidationParameters.Add("controlid", ControlID);


            yield return rule;
        }

        #endregion
    }
}