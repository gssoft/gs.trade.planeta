using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Spa_01.Models
{
    // В этот файл можно добавить пользовательский код. Изменения не буду записаны поверх.
    // 
    // Если необходимо, чтобы Entity Framework удалил и повторно создал базу данных
    // при изменении схемы модели автоматически добавляется следующее
    // код для метода Application_Start в файле Global.asax.
    // Примечание. Это приведет к удалению и повторному созданию вашей базы данных при каждом изменение модели.
    // 
    // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<Spa_01.Models.TodoItemContext>());
    public class TodoItemContext : DbContext
    {
        public TodoItemContext()
            : base("name=DefaultConnection")
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<TodoList> TodoLists { get; set; }
    }
}