using System;
using System.ComponentModel;
using System.Diagnostics;
using Newtonsoft.Json;

namespace PoeCrafting.Entities.Items
{
    public class ItemConfig : IDataErrorInfo, ICloneable
    {
        public string ItemBase { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public int ItemLevel { get; set; } = 84;
        public int Category { get; set; } = 1;

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var baseInfo = (ItemConfig)obj;
            return string.Equals(ItemBase, baseInfo.ItemBase) && string.Equals(ItemType, baseInfo.ItemType) && ItemLevel == baseInfo.ItemLevel && Category == baseInfo.Category;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ItemBase?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (ItemType?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ ItemLevel;
                hashCode = (hashCode * 397) ^ Category.GetHashCode();
                return hashCode;
            }
        }

        public object Clone()
        {
            return new ItemConfig
            {
                ItemLevel = this.ItemLevel,
                ItemBase = this.ItemBase,
                ItemType = this.ItemType,
                Category = this.Category
            };
        }

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                foreach (string property in ValidatedProperties)
                    if (GetValidationError(property) != null)
                        return false;

                return true;
            }
        }

        static readonly string[] ValidatedProperties =
        {
            "ItemBase",
            "ItemType",
            "ItemLevel"
        };

        string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "ItemBase":
                    error = string.IsNullOrEmpty(ItemBase) ? "No item base is selected" : null;
                    break;

                case "ItemType":
                    error = string.IsNullOrEmpty(ItemType) ? "No item type is selected" : null;
                    break;

                case "ItemLevel":
                    error = ItemLevel < 1 || ItemLevel > 84 ? "Invalid item level" : null;
                    break;

                default:
                    Debug.Fail("Unexpected property being validated on Item Config: " + propertyName);
                    break;
            }

            return error;
        }


        string IDataErrorInfo.this[string propertyName] => this.GetValidationError(propertyName);

        string IDataErrorInfo.Error => null;
    }
}
