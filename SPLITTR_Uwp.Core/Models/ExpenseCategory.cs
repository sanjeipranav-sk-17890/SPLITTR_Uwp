using SQLite;

namespace SPLITTR_Uwp.Core.Models
{
    public class ExpenseCategory
    {
        
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public int ParentCategoryId { get; set; }


        protected ExpenseCategory(ExpenseCategory baseCategory)
        {
            Id=baseCategory.Id;
            Name=baseCategory.Name;
            Icon=baseCategory.Icon;
            ParentCategoryId = baseCategory.ParentCategoryId;
        }
        public ExpenseCategory()
        {
            
        }

    }
}
