using System.ComponentModel.DataAnnotations;
using KPCLib;
using PassXYZLib;

namespace PassXYZ.BlazorUI
{
    public class KeyValueData<T> : IKeyValue
    {
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [Display(Name = "This field")]
        public string Key 
        {
            get 
            {
                if (Data is Item item)
                {
                    return item.Name;
                }

                if (Data is Field field)
                {
                    return field.Key;
                }
                return string.Empty;

            }
            set 
            {
                if (Data is Item item)
                {
                    item.Name = value;
                }

                if (Data is Field field)
                {
                    field.Key = value;
                }
            }

        }

        [Required(ErrorMessage = "{0} cannot be empty.")]
        [Display(Name = "This field")]
        public string Value
        {
            get 
            {
                if (Data is Item item)
                {
                    return item.Notes;
                }

                if (Data is Field field)
                {
                    return field.EditValue;
                }
                return string.Empty;
            }
            set 
            {
                if (Data is Item item) 
                {
                    item.Notes = value;
                }

                if (Data is Field field)
                {
                    field.EditValue = value;
                }
            }
        }

        public bool IsValid 
        { 
            get
            {
                return !(string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Value));
            }            
        }

        public T? Data { get; set; }

        public KeyValueData()
        {
            if (Data is Item item)
            {
                item = new NewItem();
            }

            if (Data is Field field)
            {
                field = new NewField();
            }
        }
    }
}
