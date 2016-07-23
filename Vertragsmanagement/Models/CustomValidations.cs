using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vertragsmanagement.Models
{
    /// <summary>
    /// A field with this ValidationAttribute need to be a date that is greater than the date of the given Property
    /// </summary>
    public class AfterDateAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// Get the dependant-date Property 
        /// </summary>
        /// <param name="otherProperty">needs to be a date attribute</param>
        public AfterDateAttribute(string otherProperty)
        {
            OtherProperty = otherProperty;
        }
        /// <summary>
        /// the dependant date of the other property
        /// </summary>
        public string OtherProperty { get; set; }

        /// <summary>
        /// Return the Error-Message with the input label in it
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, OtherProperty);
        }

        /// <summary>
        /// This is where the Date Comparison takes place
        /// </summary>
        /// <param name="firstValue">own value</param>
        /// <param name="validationContext">context for retriefing the other value</param>
        /// <returns>first-date > second-date</returns>
        protected override ValidationResult IsValid(object firstValue, ValidationContext validationContext)
        {
            var firstComparable = firstValue as IComparable;
            var secondComparable = GetSecondComparable(validationContext);

            if (firstComparable != null && secondComparable != null)
            {
                if (firstComparable.CompareTo(secondComparable) < 1)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// get second date from Context
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected IComparable GetSecondComparable(
            ValidationContext validationContext)
        {
            var propertyInfo = validationContext
                                  .ObjectType
                                  .GetProperty(OtherProperty);
            if (propertyInfo != null)
            {
                var secondValue = propertyInfo.GetValue(
                    validationContext.ObjectInstance, null);
                return secondValue as IComparable;
            }
            return null;
        }

        /// <summary>
        /// enable clients-side validation
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule>
            GetClientValidationRules(ModelMetadata metadata,
                                     ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationParameters.Add("other", OtherProperty);
            rule.ValidationType = "afterdate";
            yield return rule;
        }

        /// <summary>
        /// get input label of the other property 
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected string GetOtherDisplayName(ValidationContext validationContext)
        {
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(
                null, validationContext.ObjectType, OtherProperty);
            if (metadata != null)
            {
                return metadata.GetDisplayName();
            }
            return OtherProperty;
        }
    }
    /// <summary>
    /// A field with this ValidationAttribute need to be a date that is before the date of the given Property
    /// </summary>
    public class BeforeDateAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// A field with this ValidationAttribute need to be a date that is before the date of the given Property
        /// </summary>
        /// <param name="otherProperty"></param>
        public BeforeDateAttribute(string otherProperty)
        {
            OtherProperty = otherProperty;
        }
        /// <summary>
        /// the dependant date of the other property
        /// </summary>
        public string OtherProperty { get; set; }

        /// <summary>
        /// return Error-Message with input label name in it
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, OtherProperty);
        }

        /// <summary>
        /// This is where the validation takes place
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult
            IsValid(object firstValue, ValidationContext validationContext)
        {
            var firstComparable = firstValue as IComparable;
            var secondComparable = GetSecondComparable(validationContext);

            if (firstComparable != null && secondComparable != null)
            {
                if (firstComparable.CompareTo(secondComparable) > 1)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// return dependant date
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected IComparable GetSecondComparable(
            ValidationContext validationContext)
        {
            var propertyInfo = validationContext
                                  .ObjectType
                                  .GetProperty(OtherProperty);
            if (propertyInfo != null)
            {
                var secondValue = propertyInfo.GetValue(
                    validationContext.ObjectInstance, null);
                return secondValue as IComparable;
            }
            return null;
        }

        /// <summary>
        /// Enable client-side validation
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule>
            GetClientValidationRules(ModelMetadata metadata,
                                     ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationParameters.Add("other", OtherProperty);
            rule.ValidationType = "beforedate";
            yield return rule;
        }
        protected string GetOtherDisplayName(ValidationContext validationContext)
        {
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(
                null, validationContext.ObjectType, OtherProperty);
            if (metadata != null)
            {
                return metadata.GetDisplayName();
            }
            return OtherProperty;
        }
    }

    /// <summary>
    /// A field with this ValidationAttribute is required if the User is only a Dispatcher but not the signer
    /// </summary>
    public class RequiredIfDispatcherAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// 
        /// </summary>
        public RequiredIfDispatcherAttribute()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }

        public override bool IsValid(object value)
        {
            return true;
        }
        /// <summary>
        /// Enable Client-side Validation
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule>
            GetClientValidationRules(ModelMetadata metadata,
                                        ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationType = "requiredifdispatcher";
            yield return rule;
        }
    }
    /// <summary>
    /// A Field with this ValidationAttribute is always required!
    /// </summary>
    public class AlwaysRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// 
        /// </summary>
        public AlwaysRequiredAttribute()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }
        /// <summary>
        /// Test if field is empty or not
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            System.Diagnostics.Debug.Write(value);
            if (value == null || value.ToString() == "")
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Enable client-side-validation
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule>
            GetClientValidationRules(ModelMetadata metadata,
                                     ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationType = "alwaysrequired";
            yield return rule;
        }
    }
}